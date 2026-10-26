"""Identity storage: PostgreSQL for central Docker, SQLite for existing local installs."""
from __future__ import annotations
import base64
import hashlib
import os
from pathlib import Path
import secrets
import sqlite3
import time

BASE=Path(__file__).resolve().parent


class ApiError(Exception):
    def __init__(self,message,status=400):self.message,self.status=message,status


class PostgresConnection:
    """Keep a single connection for the serial HTTP server; transaction contexts never close it."""
    def __init__(self,database_url=None):
        import psycopg
        self.driver=psycopg
        self.database_url=database_url or ''
        self.transactions=[]
        self.raw=None
        self.connect()

    def connect(self):
        configured=self.driver.conninfo.conninfo_to_dict(self.database_url).get('options',os.getenv('PGOPTIONS',''))
        options=dict(autocommit=True,connect_timeout=10,application_name='mongscape-identity',
                     options=configured+' -c statement_timeout=15000 -c lock_timeout=5000 -c idle_in_transaction_session_timeout=30000')
        password_file=os.getenv('PGPASSWORD_FILE')
        if password_file:
            password=Path(password_file).read_text(encoding='utf-8').rstrip('\r\n')
            if not password:raise ValueError('PostgreSQL password file is empty')
            options['password']=password
        self.raw=self.driver.connect(self.database_url,**options)

    def ensure_open(self):
        if self.raw.closed or self.raw.broken:
            if self.transactions:raise self.driver.OperationalError('Connection lost during transaction')
            self.connect()

    def execute(self,statement,parameters=()):
        self.ensure_open()
        # Only server-owned SQL templates pass here; user values always remain parameters.
        return self.raw.execute(statement.replace('?','%s'),parameters or None)

    def executemany(self,statement,parameters):
        self.ensure_open()
        with self.raw.cursor() as cursor:cursor.executemany(statement.replace('?','%s'),parameters)

    def __enter__(self):
        self.ensure_open()
        transaction=self.raw.transaction()
        transaction.__enter__()
        self.transactions.append(transaction)
        return self

    def __exit__(self,*args):return self.transactions.pop().__exit__(*args)

    def close(self):self.raw.close()


class Store:
    def __init__(self,path=None,*,database_url=None,backend=None,initialize=True):
        self.backend=backend or ('postgres' if database_url else 'sqlite')
        if self.backend=='postgres':
            self.path=None
            self.db=PostgresConnection(database_url)
            if initialize:
                try:
                    with self.db:self.db.execute((BASE/'schema.postgres.sql').read_text(encoding='utf-8'))
                except Exception:self.db.close();raise
        elif self.backend=='sqlite':
            self.path=Path(path or BASE/'data'/'profiles.sqlite3')
            self.path.parent.mkdir(parents=True,exist_ok=True)
            self.db=sqlite3.connect(self.path)
            self.db.execute('PRAGMA foreign_keys=ON');self.db.execute('PRAGMA journal_mode=WAL')
            if initialize:self.db.executescript((BASE/'schema.sql').read_text(encoding='utf-8-sig'))
        else:raise ValueError('Identity backend must be postgres or sqlite')

    def issue(self,profile_id):
        token=secrets.token_urlsafe(32);now=int(time.time())
        with self.db:
            self.db.execute('DELETE FROM sessions WHERE expires_at < ?',(now,))
            self.db.execute('INSERT INTO sessions VALUES(?,?,?)',
                            (hashlib.sha256(token.encode()).hexdigest(),profile_id,now+8*3600))
        return token

    def authorize(self,token):
        row=self.db.execute('SELECT profile_id FROM sessions WHERE token_hash=? AND expires_at>?',
                            (hashlib.sha256(token.encode()).hexdigest(),int(time.time()))).fetchone()
        if not row:raise ApiError('로그인이 만료되었습니다. 얼굴로 다시 로그인하세요.',401)
        return row[0]

    def profile(self,profile_id,token):
        row=self.db.execute('SELECT id,name,avatar FROM profiles WHERE id=?',(profile_id,)).fetchone()
        if not row:raise ApiError('프로필 없음',404)
        return dict(ok=True,id=row[0],name=row[1],avatar=base64.b64encode(row[2]).decode(),token=token)

    def delete(self,profile_id):
        with self.db:self.db.execute('DELETE FROM profiles WHERE id=?',(profile_id,))
        if self.backend=='sqlite':
            self.db.execute('PRAGMA wal_checkpoint(TRUNCATE)');self.db.execute('VACUUM')

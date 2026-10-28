"""Identity storage: PostgreSQL for central Docker, SQLite for existing local installs."""
from __future__ import annotations
import base64
import hashlib
import json
import os
from pathlib import Path
import secrets
import sqlite3
import time
import unicodedata

BASE=Path(__file__).resolve().parent
SCHEMA_VERSION=2
NAME_MAX=10


def clean_name(value):
    """Nickname rule: trimmed, 1-10 characters, no control/format characters. None when broken."""
    name=value.strip() if isinstance(value,str) else ''
    if not 1<=len(name)<=NAME_MAX or any(unicodedata.category(c)[0]=='C' for c in name):return None
    return name


def name_key(name):
    """Uniqueness key: whitespace removed and casefolded, so 'Min Su' and 'minsu' collide."""
    return ''.join(name.split()).casefold()


class ApiError(Exception):
    def __init__(self,message,status=400):self.message,self.status=message,status


class SchemaError(Exception):
    """Existing DB predates the current schema; the server must not run until it is migrated."""


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
        elif self.backend=='sqlite':
            self.path=Path(path or BASE/'data'/'profiles.sqlite3')
            self.path.parent.mkdir(parents=True,exist_ok=True)
            self.db=sqlite3.connect(self.path)
            self.db.execute('PRAGMA foreign_keys=ON');self.db.execute('PRAGMA journal_mode=WAL')
        else:raise ValueError('Identity backend must be postgres or sqlite')
        if not initialize:return
        try:
            # CREATE TABLE IF NOT EXISTS would keep an old table as is; refuse it instead of failing later.
            version=self.version()
            if version is not None and version<SCHEMA_VERSION:
                raise SchemaError(f'Old identity schema (v{version}). Back up the DB, run migrate_account.py, then restart.')
            if version is not None and version>SCHEMA_VERSION:
                raise SchemaError(f'Identity schema v{version} is newer than this server (v{SCHEMA_VERSION}).')
            if self.backend=='sqlite':self.db.executescript((BASE/'schema.sql').read_text(encoding='utf-8-sig'))
            else:
                with self.db:self.db.execute((BASE/'schema.postgres.sql').read_text(encoding='utf-8'))
        except Exception:self.db.close();raise

    def columns(self,table):
        if self.backend=='sqlite':return {row[1] for row in self.db.execute(f'PRAGMA table_info({table})')}
        return {row[0] for row in self.db.execute(
            'SELECT column_name FROM information_schema.columns WHERE table_schema=current_schema() AND table_name=?',(table,))}

    def indexes(self):
        if self.backend=='sqlite':return {row[0] for row in self.db.execute("SELECT name FROM sqlite_master WHERE type='index'")}
        return {row[0] for row in self.db.execute('SELECT indexname FROM pg_indexes WHERE schemaname=current_schema()')}

    def version(self):
        """None for an empty DB; 1 for the pre-account layout that has no schema_version table."""
        if self.columns('schema_version'):return self.db.execute('SELECT MAX(version) FROM schema_version').fetchone()[0] or 1
        return 1 if self.columns('profiles') else None

    def name_owner(self,key):
        row=self.db.execute('SELECT id FROM profiles WHERE name_key=?',(key,)).fetchone()
        return row[0] if row else None

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

    def account(self,profile_id):
        """Profile object of the API (section 4.2); JSONB arrives parsed from PostgreSQL, TEXT from SQLite."""
        row=self.db.execute('SELECT id,name,weight,style,goal,coins,unlocked,tutorial_done FROM profiles WHERE id=?',(profile_id,)).fetchone()
        if not row:raise ApiError('프로필 없음',404)
        unlocked=json.loads(row[6]) if isinstance(row[6],str) else row[6]
        return dict(id=row[0],name=row[1],weight=float(row[2]),style=row[3],goal=float(row[4]),coins=row[5],
                    unlocked=sorted(unlocked),tutorial_done=bool(row[7]))

    def profile(self,profile_id,token,avatar):
        # avatar is this request's aligned face JPG; it is returned once and never stored.
        # id/name stay at the top level for clients built before the profile object.
        account=self.account(profile_id)
        return dict(ok=True,token=token,avatar=base64.b64encode(avatar).decode(),profile=account,id=account['id'],name=account['name'])

    def delete(self,profile_id):
        with self.db:self.db.execute('DELETE FROM profiles WHERE id=?',(profile_id,))
        if self.backend=='sqlite':
            self.db.execute('PRAGMA wal_checkpoint(TRUNCATE)');self.db.execute('VACUUM')

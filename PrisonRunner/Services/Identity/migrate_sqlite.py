"""Explicit, atomic SQLite -> PostgreSQL import. Source is read-only; sessions are not copied."""
import argparse
import json
import os
from pathlib import Path
import sqlite3
from storage import Store

def migrate(source_path,*,database_url=None):
    source_path=Path(source_path).resolve(strict=True)
    source=sqlite3.connect(source_path.as_uri()+'?mode=ro',uri=True)
    target=None
    try:
        source.execute('BEGIN')
        target=Store(database_url=database_url,backend='postgres')
        counts={}
        with target.db:
            target.db.execute('LOCK TABLE profiles,face_samples,sessions,runs IN ACCESS EXCLUSIVE MODE')
            if any(target.db.execute('SELECT COUNT(*) FROM '+table).fetchone()[0]
                   for table in ('profiles','face_samples','runs','sessions')):
                raise ValueError('Target database must be empty; no data was imported')
            for table,columns in (
                ('profiles','id,name,created_at,consent_version'),
                ('face_samples','profile_id,embedding'),
                ('runs','id,profile_id,distance,score,kcal,goal,activities,created_at'),
            ):
                rows=source.execute('SELECT '+columns+' FROM '+table).fetchall()
                if table=='runs':
                    if any(not isinstance(json.loads(row[6]),list) or len(json.loads(row[6]))!=5 for row in rows):
                        raise ValueError('Invalid activity array in source')
                    rows=[(*row[:6],json.dumps(json.loads(row[6])),row[7]) for row in rows]
                marks=','.join('?' for _ in columns.split(','))
                if rows:target.db.executemany(f'INSERT INTO {table} ({columns}) VALUES ({marks})',rows)
                counts[table]=len(rows)
            counts['sessions']=0
        return counts
    finally:
        source.close()
        if target is not None:target.db.close()

def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('source',type=Path)
    args=parser.parse_args()
    try:counts=migrate(args.source,database_url=os.getenv('IDENTITY_DATABASE_URL'))
    except Exception as error:raise SystemExit('Import failed; target transaction rolled back. '+type(error).__name__)
    print(json.dumps(dict(imported=counts,login_again=True)))

if __name__=='__main__':main()

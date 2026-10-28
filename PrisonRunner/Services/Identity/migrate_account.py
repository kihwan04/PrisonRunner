"""Explicit, atomic upgrade of an existing identity DB (SQLite path or PostgreSQL) to the current schema.
Every step is idempotent and all steps share one transaction. Back up the DB and stop the API first."""
import argparse
from contextlib import contextmanager
import json
import os
from pathlib import Path
import sqlite3
from storage import BASE,Store

# SQLite added ALTER TABLE DROP COLUMN in 3.35; older libraries rebuild the table instead.
SQLITE_DROP_COLUMN=sqlite3.sqlite_version_info>=(3,35,0)

def drop_profile_avatar(store):
    """Face photos are no longer stored; remove the old column and every photo in it."""
    if 'avatar' not in store.columns('profiles'):return False
    if store.backend=='postgres' or SQLITE_DROP_COLUMN:
        store.db.execute('ALTER TABLE profiles DROP COLUMN avatar')
        return True
    # Table rebuild (sqlite.org/lang_altertable.html); foreign keys are off so children are kept.
    store.db.execute('CREATE TABLE profiles_migrated (id TEXT PRIMARY KEY, name TEXT NOT NULL, '
                     'created_at INTEGER NOT NULL, consent_version TEXT NOT NULL)')
    store.db.execute('INSERT INTO profiles_migrated (id,name,created_at,consent_version) '
                     'SELECT id,name,created_at,consent_version FROM profiles')
    store.db.execute('DROP TABLE profiles')
    store.db.execute('ALTER TABLE profiles_migrated RENAME TO profiles')
    return True

# Ordered steps; later schema changes append here and must also skip when already applied.
STEPS=(('drop_profile_avatar',drop_profile_avatar),)

@contextmanager
def transaction(store):
    if store.backend=='postgres':
        with store.db:yield
        return
    db=store.db;db.isolation_level=None # explicit BEGIN so DDL is part of the transaction
    db.execute('PRAGMA foreign_keys=OFF') # no effect inside a transaction; must precede BEGIN
    try:
        db.execute('BEGIN IMMEDIATE')
        try:
            yield
            if db.execute('PRAGMA foreign_key_check').fetchone():raise ValueError('Foreign key check failed')
            db.execute('COMMIT')
        except BaseException:
            db.execute('ROLLBACK');raise
    finally:db.execute('PRAGMA foreign_keys=ON')

def migrate(path=None,*,database_url=None,backend=None):
    """Return the names of the steps applied; an empty list means the DB was already current."""
    backend=backend or ('postgres' if database_url else 'sqlite')
    if backend=='sqlite':
        path=Path(path or BASE/'data'/'profiles.sqlite3')
        if not path.is_file():raise FileNotFoundError('SQLite database not found') # never create an empty DB here
    store=Store(path,database_url=database_url,backend=backend,initialize=False)
    try:
        with transaction(store):applied=[name for name,step in STEPS if step(store)]
        return applied
    finally:store.db.close()

def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--database',type=Path,default=Path(os.getenv('IDENTITY_DATABASE',str(BASE/'data'/'profiles.sqlite3'))))
    parser.add_argument('--database-url',default=os.getenv('IDENTITY_DATABASE_URL'))
    parser.add_argument('--database-backend',choices=('postgres','sqlite'),default=os.getenv('IDENTITY_DATABASE_BACKEND'))
    args=parser.parse_args()
    try:applied=migrate(args.database,database_url=args.database_url,backend=args.database_backend)
    except Exception as error:raise SystemExit('Migration failed; transaction rolled back. '+type(error).__name__)
    print(json.dumps(dict(applied=applied,up_to_date=not applied)))

if __name__=='__main__':main()

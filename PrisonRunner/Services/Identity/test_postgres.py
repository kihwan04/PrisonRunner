"""Real PostgreSQL integration. Creates/removes only generated test schemas in an explicit test DB."""
import hashlib
import json
import os
from pathlib import Path
import sqlite3
import tempfile
import unittest
import uuid
import test_identity as identity_tests
import test_http as http_tests
from storage import ApiError,Store
from migrate_sqlite import migrate
import migrate_account
from prepare_secrets import prepare

URL=os.getenv('MONGSCAPE_TEST_POSTGRES_URL')
PROFILE_INSERT='INSERT INTO profiles (id,name,name_key,created_at,consent_version) VALUES(?,?,?,?,?)'

class IsolatedSchema:
    def setUp(self):
        if not URL:self.skipTest('Set MONGSCAPE_TEST_POSTGRES_URL to an isolated test database')
        import psycopg
        self.psycopg=psycopg
        self.admin=psycopg.connect(URL,autocommit=True)
        self.schema='mongscape_test_'+uuid.uuid4().hex
        self.admin.execute(psycopg.sql.SQL('CREATE SCHEMA {}').format(psycopg.sql.Identifier(self.schema)))
        self.url=psycopg.conninfo.make_conninfo(URL,options='-c search_path='+self.schema)
        super().setUp()
    def make_store(self):return Store(database_url=self.url)
    def migrate_target(self):return dict(database_url=self.url)
    def tearDown(self):
        try:super().tearDown()
        finally:
            assert self.schema.startswith('mongscape_test_') and len(self.schema)==47
            self.admin.execute(self.psycopg.sql.SQL('DROP SCHEMA {} CASCADE').format(self.psycopg.sql.Identifier(self.schema)))
            self.admin.close()

@unittest.skipUnless(URL,'PostgreSQL test URL is not configured')
class PostgresIdentityTests(IsolatedSchema,identity_tests.IdentityTests):pass

@unittest.skipUnless(URL,'PostgreSQL test URL is not configured')
class PostgresHttpTests(IsolatedSchema,http_tests.HttpTests):pass

@unittest.skipUnless(URL,'PostgreSQL test URL is not configured')
class PostgresStorageTests(IsolatedSchema,unittest.TestCase):
    def setUp(self):
        super().setUp();self.store=self.make_store();self.temp=tempfile.TemporaryDirectory()
    def tearDown(self):
        self.store.db.close();self.temp.cleanup();super().tearDown()
    def profile(self):
        return ('visitor',"Test ' v",'test\'v',1,'test-consent')
    def source(self,invalid=False):
        path=Path(self.temp.name)/'source.sqlite3'
        source=Store(path)
        with source.db:
            source.db.execute(PROFILE_INSERT,self.profile())
            source.db.execute('INSERT INTO face_samples VALUES(?,?)',('visitor',b'\0'*512))
            source.db.execute("UPDATE profiles SET coins=40,unlocked='[2]',tutorial_done=1")
            source.db.execute('INSERT INTO runs VALUES(?,?,?,?,?,?,?,?,?,?,?,?)',
                              ('run','visitor',100.5,23,2.5,50,'{}' if invalid else '[1,1,0.5,0,0]',2,1,7,2,61.5))
        token=source.issue('visitor');source.db.close()
        return path,token
    def test_transactions_rollback_and_leave_connection_open(self):
        with self.assertRaises(self.psycopg.errors.UniqueViolation):
            with self.store.db:
                self.store.db.execute(PROFILE_INSERT,self.profile())
                self.store.db.execute(PROFILE_INSERT,self.profile())
        self.assertEqual(self.store.db.execute('SELECT COUNT(*) FROM profiles').fetchone()[0],0)
        with self.store.db:self.store.db.execute(PROFILE_INSERT,self.profile())
        self.assertEqual(self.store.db.execute('SELECT name FROM profiles').fetchone()[0],self.profile()[1])
    def test_application_role_has_no_administrator_privileges(self):
        privileges=self.store.db.execute('SELECT rolsuper,rolcreatedb,rolcreaterole FROM pg_roles WHERE rolname=current_user').fetchone()
        self.assertEqual(privileges,(False,False,False))
        with self.assertRaises(self.psycopg.errors.InsufficientPrivilege):self.store.db.execute('CREATE ROLE forbidden_test_role')
    def test_jsonb_binary_types_and_foreign_key_indexes(self):
        types=dict(self.store.db.execute("SELECT column_name,data_type FROM information_schema.columns WHERE table_schema=current_schema() AND table_name='runs'").fetchall())
        self.assertEqual(types['activities'],'jsonb')
        types=dict(self.store.db.execute("SELECT column_name,data_type FROM information_schema.columns WHERE table_schema=current_schema() AND table_name='profiles'").fetchall())
        self.assertEqual((types['unlocked'],types['tutorial_done'],types['weight']),('jsonb','boolean','double precision'))
        self.assertTrue({'runs_profile_created','runs_mode_score','profiles_name_key','sessions_expiry','sessions_profile','face_samples_pkey'}<=self.store.indexes())
    def test_closed_transport_reconnects(self):
        self.store.db.raw.close()
        self.assertEqual(self.store.db.execute('SELECT 1').fetchone(),(1,))
    def test_docker_environment_uses_application_password_file(self):
        fields=self.psycopg.conninfo.conninfo_to_dict(URL)
        secret=Path(self.temp.name)/'app-password'
        secret.write_text(fields['password'],encoding='utf-8')
        configured={'PGHOST':fields['host'],'PGPORT':fields['port'],'PGDATABASE':fields['dbname'],
                    'PGUSER':fields['user'],'PGPASSWORD_FILE':str(secret),'PGOPTIONS':'-c search_path='+self.schema}
        previous={name:os.environ.get(name) for name in configured}
        other=None
        try:
            os.environ.update(configured)
            other=Store(backend='postgres')
            self.assertEqual(other.db.execute('SELECT current_user').fetchone()[0],'mongscape')
            self.assertEqual(other.backend,'postgres')
        finally:
            if other is not None:other.db.close()
            for name,value in previous.items():
                if value is None:os.environ.pop(name,None)
                else:os.environ[name]=value
    def test_migration_preserves_data_source_and_invalidates_old_sessions(self):
        path,token=self.source();before=hashlib.sha256(path.read_bytes()).digest()
        counts=migrate(path,database_url=self.url)
        self.assertEqual(counts,dict(profiles=1,face_samples=1,runs=1,sessions=0))
        self.assertEqual(hashlib.sha256(path.read_bytes()).digest(),before)
        self.assertEqual(self.store.db.execute('SELECT name FROM profiles').fetchone()[0],self.profile()[1])
        self.assertNotIn('avatar',self.store.columns('profiles'))
        self.assertEqual(self.store.db.execute('SELECT activities,mode,coins,score_version,play_seconds FROM runs').fetchone(),([1,1,.5,0,0],1,7,2,61.5))
        self.assertEqual(self.store.account('visitor'),dict(id='visitor',name=self.profile()[1],weight=65.0,style=0,goal=50.0,coins=40,unlocked=[2],tutorial_done=True))
        with self.assertRaises(ApiError):self.store.authorize(token)
    def test_import_from_legacy_sqlite_with_photos_drops_them(self):
        path=Path(self.temp.name)/'legacy.sqlite3'
        source=sqlite3.connect(path)
        source.executescript(identity_tests.legacy_schema('sqlite'))
        with source:
            source.execute('INSERT INTO profiles VALUES(?,?,?,?,?)',('visitor','Legacy visitor',b'old-photo',1,'test-consent'))
            source.execute('INSERT INTO face_samples VALUES(?,?)',('visitor',b'\0'*512))
        source.close()
        # A v1 copy is refused until migrate_account.py upgrades it (unique nicknames, account columns).
        with self.assertRaises(ValueError):migrate(path,database_url=self.url)
        self.assertEqual(self.store.db.execute('SELECT COUNT(*) FROM profiles').fetchone()[0],0)
        migrate_account.migrate(path)
        self.assertEqual(migrate(path,database_url=self.url),dict(profiles=1,face_samples=1,runs=0,sessions=0))
        self.assertEqual(self.store.db.execute('SELECT id,name,name_key FROM profiles').fetchone(),('visitor','Legacy vis','legacyvis'))
    def test_migration_refuses_nonempty_target(self):
        path,_=self.source();migrate(path,database_url=self.url)
        with self.assertRaises(ValueError):migrate(path,database_url=self.url)
        self.assertEqual(self.store.db.execute('SELECT COUNT(*) FROM profiles').fetchone()[0],1)
    def test_invalid_import_rolls_back_every_table(self):
        path,_=self.source(invalid=True)
        with self.assertRaises(ValueError):migrate(path,database_url=self.url)
        for table in ('profiles','face_samples','runs','sessions'):
            self.assertEqual(self.store.db.execute('SELECT COUNT(*) FROM '+table).fetchone()[0],0)
    def test_secret_generation_preserves_existing_values(self):
        folder=Path(self.temp.name)/'secrets';prepare(folder)
        before={file.name:file.read_bytes() for file in folder.iterdir()}
        prepare(folder)
        self.assertEqual(before,{file.name:file.read_bytes() for file in folder.iterdir()})
        self.assertNotEqual(before['db_app_password'],before['db_admin_password'])

if __name__=='__main__':unittest.main(verbosity=2)

import base64,json,sys,tempfile,unittest,urllib.request,uuid
from pathlib import Path
from unittest import mock
BASE=Path(__file__).resolve().parent
sys.path.insert(0,str(BASE/'.runtime'))
from server import App,Store,Faces,ApiError
from storage import SchemaError
import migrate_account
def legacy_schema(backend):
 # Pre-T4 layout: the current schema plus the stored photo column.
 text=(BASE/('schema.postgres.sql' if backend=='postgres' else 'schema.sql')).read_text(encoding='utf-8-sig')
 line='    name TEXT NOT NULL,\n'
 assert line in text
 return text.replace(line,line+'    avatar '+('BYTEA' if backend=='postgres' else 'BLOB')+' NOT NULL,\n',1)
class IdentityTests(unittest.TestCase):
 @classmethod
 def setUpClass(cls):
  import cv2,numpy as np
  cls.cv,cls.np=cv2,np;cls.faces=Faces(BASE/'models')
  raw=urllib.request.urlopen('https://raw.githubusercontent.com/opencv/opencv/master/samples/data/lena.jpg',timeout=20).read()
  cls.image=cv2.imdecode(np.frombuffer(raw,np.uint8),1)
  cls.images=[]
  for degree in (0,3,-3):
   m=cv2.getRotationMatrix2D((256,256),degree,1);image=cv2.warpAffine(cls.image,m,(512,512));_,jpg=cv2.imencode('.jpg',image)
   cls.images.append(base64.b64encode(jpg).decode())
 def make_store(self):return Store(Path(self.tmp.name)/'test.sqlite3')
 def setUp(self):
  self.tmp=tempfile.TemporaryDirectory();self.store=self.make_store();self.app=App(self.store,self.faces)
 def tearDown(self): self.store.db.close();self.tmp.cleanup()
 def migrate_target(self):return dict(path=Path(self.tmp.name)/'test.sqlite3')
 def enroll(self):return self.app.handle('/enroll',dict(name='Test explorer',consent=True,images=self.images))
 def test_real_model_enroll_reopen_login_record_delete(self):
  first=self.enroll();self.assertTrue(first['ok']);self.assertTrue(first['avatar']);self.assertNotEqual(first['token'],'')
  self.assertNotIn('avatar',self.store.columns('profiles'))
  self.store.db.close();self.store=self.make_store();self.app=App(self.store,self.faces)
  login=self.app.handle('/login',dict(images=self.images));self.assertEqual(first['id'],login['id']);self.assertNotEqual(first['token'],login['token'])
  # The badge photo comes from this request's frames, so it is returned although nothing was stored.
  self.assertEqual(base64.b64decode(login['avatar'])[:2],b'\xff\xd8')
  record=dict(id=str(uuid.uuid4()),distance=123.4,score=567,kcal=3.2,goal=50,activities=[1,1,1,.2,0])
  self.app.handle('/runs',record,login['token']);self.app.handle('/runs',record,login['token'])
  history=self.app.handle('/history',{},login['token']);self.assertEqual(len(history['runs']),1)
  self.assertEqual(history['runs'][0]['score'],567)
  self.app.handle('/delete',{},login['token'])
  for table in ('profiles','face_samples','runs','sessions'):self.assertEqual(self.store.db.execute('SELECT COUNT(*) FROM '+table).fetchone()[0],0)
  with self.assertRaises(ApiError):self.store.authorize(login['token'])
 def test_consent_duplicate_unknown_and_logout(self):
  with self.assertRaises(ApiError):self.app.handle('/enroll',dict(consent=False,images=self.images))
  with self.assertRaises(ApiError):self.app.handle('/login',dict(images=self.images))
  p=self.enroll()
  with self.assertRaises(ApiError):self.enroll()
  with self.assertRaises(ApiError):self.app.handle('/history',{},'wrong token')
  self.app.handle('/logout',{},p['token'])
  with self.assertRaises(ApiError):self.store.authorize(p['token'])
 def test_no_face_and_malformed_images_rejected(self):
  _,image=self.cv.imencode('.jpg',self.np.zeros((480,640,3),self.np.uint8))
  blank=base64.b64encode(image).decode()
  with self.assertRaises(ApiError):self.faces.samples(dict(images=[blank]*3))
  with self.assertRaises(ApiError):self.faces.samples(dict(images=['not jpeg']*3))
  with self.assertRaises(ApiError):self.faces.samples(dict(images=[]))
 def legacy_roundtrip(self):
  backend=self.store.backend;features,_=self.faces.samples(dict(images=self.images))
  with self.store.db:
   for table in ('runs','sessions','face_samples','profiles'):self.store.db.execute('DROP TABLE '+table)
  if backend=='sqlite':self.store.db.executescript(legacy_schema(backend))
  else:
   with self.store.db:self.store.db.execute(legacy_schema(backend))
  with self.store.db:
   self.store.db.execute('INSERT INTO profiles VALUES(?,?,?,?,?)',('legacy','Legacy explorer',b'old-photo',1,'local-exhibition-v1'))
   self.store.db.executemany('INSERT INTO face_samples VALUES(?,?)',[('legacy',f.tobytes()) for f in features])
   self.store.db.execute('INSERT INTO runs VALUES(?,?,?,?,?,?,?,?)',(str(uuid.uuid4()),'legacy',10,5,1,50,'[1,0,0,0,0]',1))
  self.store.db.close()
  with self.assertRaises(SchemaError):self.store=self.make_store()
  self.assertEqual(migrate_account.migrate(**self.migrate_target()),['drop_profile_avatar'])
  self.assertEqual(migrate_account.migrate(**self.migrate_target()),[])
  self.store=self.make_store();self.app=App(self.store,self.faces)
  self.assertNotIn('avatar',self.store.columns('profiles'))
  login=self.app.handle('/login',dict(images=self.images))
  self.assertEqual((login['id'],login['name']),('legacy','Legacy explorer'));self.assertTrue(login['avatar'])
  self.assertEqual(len(self.app.handle('/history',{},login['token'])['runs']),1)
 def test_legacy_avatar_db_refused_until_migrated(self):self.legacy_roundtrip()
 def test_legacy_avatar_db_migrates_by_table_rebuild(self):
  # Path used when SQLite is older than 3.35; PostgreSQL always drops the column directly.
  with mock.patch.object(migrate_account,'SQLITE_DROP_COLUMN',False):self.legacy_roundtrip()
 def test_invalid_and_unauthorized_records_rejected(self):
  p=self.enroll()
  with self.assertRaises(ApiError):self.app.handle('/runs',{},p['token'])
  with self.assertRaises(ApiError):self.app.handle('/delete',{},'bad')
if __name__=='__main__':unittest.main(verbosity=2)

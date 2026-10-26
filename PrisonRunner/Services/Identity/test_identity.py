import base64,json,sys,tempfile,unittest,urllib.request,uuid
from pathlib import Path
BASE=Path(__file__).resolve().parent
sys.path.insert(0,str(BASE/'.runtime'))
from server import App,Store,Faces,ApiError
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
 def enroll(self):return self.app.handle('/enroll',dict(name='Test explorer',consent=True,images=self.images))
 def test_real_model_enroll_reopen_login_record_delete(self):
  first=self.enroll();self.assertTrue(first['ok']);self.assertTrue(first['avatar']);self.assertNotEqual(first['token'],'')
  self.store.db.close();self.store=self.make_store();self.app=App(self.store,self.faces)
  login=self.app.handle('/login',dict(images=self.images));self.assertEqual(first['id'],login['id']);self.assertNotEqual(first['token'],login['token'])
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
 def test_invalid_and_unauthorized_records_rejected(self):
  p=self.enroll()
  with self.assertRaises(ApiError):self.app.handle('/runs',{},p['token'])
  with self.assertRaises(ApiError):self.app.handle('/delete',{},'bad')
if __name__=='__main__':unittest.main(verbosity=2)

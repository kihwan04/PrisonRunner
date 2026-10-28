"""Mongscape exhibition profiles. YuNet + SFace, PostgreSQL/SQLite, Docker/HTTPS host.
This demo has no certified presentation-attack/liveness detection. Do not use for sensitive accounts.
"""
from __future__ import annotations
import argparse, base64, hashlib, json, math, os, sys, time, uuid
from pathlib import Path
from http.server import BaseHTTPRequestHandler, HTTPServer
BASE = Path(__file__).resolve().parent
sys.path.insert(0, str(BASE / '.runtime'))
MAX_BODY = 4 * 1024 * 1024
from storage import ApiError,SchemaError,Store

class Faces:
    def __init__(self, models):
        import cv2, numpy as np
        self.cv, self.np=cv2,np
        cv2.setNumThreads(2)
        self.detector=cv2.FaceDetectorYN.create(str(models/'face_detection_yunet_2023mar.onnx'),'',(320,320),.9,.3,5000)
        self.recognizer=cv2.FaceRecognizerSF.create(str(models/'face_recognition_sface_2021dec.onnx'),'')
    def extract(self, encoded):
        try:
            raw=base64.b64decode(encoded,validate=True)
            img=self.cv.imdecode(self.np.frombuffer(raw,self.np.uint8),self.cv.IMREAD_COLOR)
        except Exception: raise ApiError('카메라 이미지가 잘못되었습니다.')
        if img is None or max(img.shape[:2])>1920: raise ApiError('카메라 이미지 크기를 확인하세요.')
        self.detector.setInputSize((img.shape[1],img.shape[0])); _,faces=self.detector.detect(img)
        if faces is None or len(faces)!=1: raise ApiError('화면에 한 사람의 얼굴만 보이게 해 주세요.')
        f=faces[0]
        if min(f[2],f[3])<55: raise ApiError('얼굴을 카메라에 조금 더 가까이 해 주세요.')
        aligned=self.recognizer.alignCrop(img,f)
        feature=self.recognizer.feature(aligned).flatten().astype('float32')
        feature/=max(1e-9,float(self.np.linalg.norm(feature)))
        _,avatar=self.cv.imencode('.jpg',aligned,[self.cv.IMWRITE_JPEG_QUALITY,90])
        return feature,avatar.tobytes()
    def samples(self, data):
        images=data.get('images',[])
        if not isinstance(images,list) or len(images)!=3: raise ApiError('서로 다른 시점의 카메라 사진 3장이 필요합니다.')
        values=[self.extract(image) for image in images]
        if min(float(a[0]@b[0]) for a in values for b in values)<.6:
            raise ApiError('촬영 중 얼굴이 달라졌습니다. 혼자 서서 다시 시도하세요.')
        return [v[0] for v in values], values[0][1]
    def match(self, store, features):
        scores={}
        for pid,raw in store.db.execute('SELECT profile_id,embedding FROM face_samples'):
            known=self.np.frombuffer(raw,dtype='float32')
            # All three frames must match the same enrolled sample; use the weakest frame.
            score=min(float(known@candidate) for candidate in features)
            scores[pid]=max(scores.get(pid,-1),score)
        ranked=sorted(scores.items(),key=lambda pair:pair[1],reverse=True)
        if not ranked or ranked[0][1]<.6: return None
        if len(ranked)>1 and ranked[0][1]-ranked[1][1]<.08: raise ApiError('얼굴 구분이 불확실합니다. 밝은 곳에서 다시 시도하세요.',409)
        return ranked[0][0]

class App:
    def __init__(self, store, faces): self.store,self.faces=store,faces; self.attempts=[]
    def handle(self,path,data,token=''):
        if path in ('/enroll','/login'):
            now=time.monotonic(); self.attempts=[t for t in self.attempts if now-t<60]
            if len(self.attempts)>=15: raise ApiError('잠시 기다린 후 다시 시도하세요.',429)
            self.attempts.append(now)
            if path=='/enroll' and data.get('consent') is not True: raise ApiError('얼굴 저장 동의가 필요합니다.')
            features,avatar=self.faces.samples(data); pid=self.faces.match(self.store,features)
            if path=='/enroll':
                if pid: raise ApiError('이미 등록된 얼굴입니다. 얼굴 로그인을 눌러 주세요.',409)
                name=str(data.get('name','')).strip()[:20] or '탐험가'
                pid=uuid.uuid4().hex
                with self.store.db:
                    self.store.db.execute('INSERT INTO profiles (id,name,created_at,consent_version) VALUES(?,?,?,?)',(pid,name,int(time.time()),'local-exhibition-v1'))
                    self.store.db.executemany('INSERT INTO face_samples VALUES(?,?) ON CONFLICT DO NOTHING',[(pid,f.tobytes()) for f in features])
            if not pid: raise ApiError('등록된 얼굴을 찾지 못했습니다. 먼저 얼굴을 등록하세요.',401)
            # The face crop only travels back in this response; the DB keeps features, never photos.
            session=self.store.issue(pid); return self.store.profile(pid,session,avatar)
        pid=self.store.authorize(token)
        if path=='/delete': self.store.delete(pid); return dict(ok=True)
        if path=='/logout':
            with self.store.db: self.store.db.execute('DELETE FROM sessions WHERE token_hash=?',(hashlib.sha256(token.encode()).hexdigest(),))
            return dict(ok=True)
        if path=='/runs':
            def number(key,maximum):
                v=data.get(key)
                if type(v) not in (int,float) or not math.isfinite(v) or v<0 or v>maximum: raise ApiError('잘못된 기록: '+key)
                return v
            distance=number('distance',10000000); score=number('score',2147483647); kcal=number('kcal',100000); goal=number('goal',5000)
            if goal<1: raise ApiError('목표 오류')
            activities=data.get('activities',[])
            if not isinstance(activities,list) or len(activities)!=5 or any(type(v) not in (float,int) or not math.isfinite(v) or v<0 for v in activities): raise ApiError('동작별 기록 오류')
            try: rid=str(uuid.UUID(str(data.get('id',''))))
            except ValueError: raise ApiError('기록 ID 오류')
            with self.store.db:
                self.store.db.execute('INSERT INTO runs VALUES(?,?,?,?,?,?,?,?) ON CONFLICT DO NOTHING',
                    (rid,pid,distance,int(score),kcal,goal,json.dumps(activities),int(time.time())))
            return dict(ok=True)
        if path=='/history':
            rows=self.store.db.execute('SELECT distance,score,kcal,goal,created_at FROM runs WHERE profile_id=? ORDER BY created_at DESC LIMIT 20',(pid,)).fetchall()
            return dict(ok=True,runs=[dict(zip(('distance','score','kcal','goal','created_at'),row)) for row in rows])
        raise ApiError('경로 없음',404)

class Handler(BaseHTTPRequestHandler):
    def setup(self):
        super().setup()
        self.connection.settimeout(15)
    def log_message(self,*args): pass # Images, tokens and identities never go into request logs.
    def answer(self,status,payload):
        body=json.dumps(payload,ensure_ascii=False).encode()
        self.send_response(status); self.send_header('Content-Type','application/json; charset=utf-8')
        self.send_header('Cache-Control','no-store'); self.send_header('Content-Length',str(len(body))); self.end_headers(); self.wfile.write(body)
    def do_GET(self):
        if self.path=='/health':
            try:
                self.server.app.store.db.execute('SELECT 1').fetchone()
                self.answer(200,dict(ok=True,service='Mongscape Local Identity',liveness=False,storage=self.server.app.store.backend))
            except Exception:self.answer(503,dict(ok=False,message='Identity database unavailable'))
        else: self.answer(404,dict(ok=False,message='Not found'))
    def do_POST(self):
        try:
            if self.headers.get('Origin'): raise ApiError('브라우저 요청은 허용하지 않습니다.',403)
            if self.headers.get('Host','').lower() not in self.server.allowed_hosts: raise ApiError('호스트 오류',403)
            if self.headers.get('Content-Type','').split(';')[0]!='application/json': raise ApiError('JSON 필요',415)
            try: length=int(self.headers.get('Content-Length','0'))
            except ValueError: raise ApiError('길이 오류')
            if length<=0 or length>MAX_BODY: raise ApiError('요청 크기 오류',413)
            try: data=json.loads(self.rfile.read(length))
            except Exception: raise ApiError('JSON 오류')
            if not isinstance(data,dict): raise ApiError('JSON 객체 필요')
            token=self.headers.get('Authorization','').removeprefix('Bearer ')
            self.answer(200,self.server.app.handle(self.path,data,token))
        except ApiError as e: self.answer(e.status,dict(ok=False,message=e.message))
        except Exception as e:
            print('Identity error:',type(e).__name__,file=sys.stderr,flush=True)
            self.answer(500,dict(ok=False,message='프로필 처리 오류. 서버 로그를 확인하세요.'))

def create_server(host,port,store,faces,allowed_hosts=None):
    hosts={value.strip().lower() for value in (allowed_hosts or f'127.0.0.1:{port},localhost:{port}').split(',') if value.strip()}
    if not hosts or any('*' in value or '/' in value for value in hosts):
        raise ValueError('Allowed hosts must be explicit host[:port] values.')
    server=HTTPServer((host,port),Handler)
    server.app=App(store,faces)
    server.allowed_hosts=hosts
    return server

def main():
    parser=argparse.ArgumentParser()
    parser.add_argument('--database',type=Path,default=Path(os.getenv('IDENTITY_DATABASE',str(BASE/'data'/'profiles.sqlite3'))))
    parser.add_argument('--database-url',default=os.getenv('IDENTITY_DATABASE_URL'))
    parser.add_argument('--database-backend',choices=('postgres','sqlite'),default=os.getenv('IDENTITY_DATABASE_BACKEND'))
    parser.add_argument('--models',type=Path,default=BASE/'models')
    parser.add_argument('--host',default=os.getenv('IDENTITY_HOST','127.0.0.1'))
    parser.add_argument('--port',type=int,default=int(os.getenv('IDENTITY_PORT','8765')))
    parser.add_argument('--allowed-hosts',default=os.getenv('IDENTITY_ALLOWED_HOSTS'))
    parser.add_argument('--init-db',action='store_true')
    args=parser.parse_args()
    try:store=Store(args.database,database_url=args.database_url,backend=args.database_backend)
    except SchemaError as error:raise SystemExit(str(error))
    except Exception as error:raise SystemExit('Identity database connection/setup failed: '+type(error).__name__)
    try:
        if args.init_db: print('Identity database initialized:',store.backend); return
        try: faces=Faces(args.models)
        except Exception as e: raise SystemExit('Prepare runtime/models with SetupIdentity.ps1 or Docker: '+str(e))
        with create_server(args.host,args.port,store,faces,args.allowed_hosts) as server:
            print(f'Mongscape identity ready at {args.host}:{args.port} (exhibition profiles; no liveness)',flush=True)
            server.serve_forever()
    finally: store.db.close()
if __name__=='__main__': main()

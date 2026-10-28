from pathlib import Path
import hashlib,json,zipfile
BASE=Path(__file__).resolve().parents[3]
DEST=BASE/'Deliverables/2026-10-25';DEST.mkdir(parents=True,exist_ok=True)
PROJECT=BASE/'PrisonRunner'
report=PROJECT/'docs/reports/LOCAL_COMPLETION_2026_10_25.md'
(DEST/'구현_검증_안내.md').write_bytes(report.read_bytes())
notes='''Mongscape verified local source, 2026-10-25.
Revised hidden three-way junction, lane blockers, random bananas and cartoon police capture.
Windows build and Unity tests: see the included final verification report. Real webcam testing is deferred by the user.
Read PrisonRunner/docs/reports/LOCAL_COMPLETION_2026_10_25.md.
No user face records or cloud credentials are included.
Keep third-party source assets private and follow THIRD_PARTY_NOTICES.md before redistributing.
'''
source=DEST/'Mongscape-Unity-Source.zip'
with zipfile.ZipFile(source,'w',zipfile.ZIP_DEFLATED,compresslevel=5) as z:
 z.writestr('READ_ME_FIRST.txt',notes)
 for folder in ('Unity/Assets','Unity/Packages','Unity/ProjectSettings','Services/Identity','Tools/QA','Tools/Distribution'):
  for file in sorted((PROJECT/folder).rglob('*')):
   if not file.is_file():continue
   rel=file.relative_to(PROJECT)
   if any(part in ('.runtime','data','python','__pycache__','secrets') for part in rel.parts):continue
   if file.suffix in ('.log','.pyc','.dump') or file.name in ('.env','identity-server.json'):continue
   if rel.parts[:3]==('Services','Identity','models') and file.suffix=='.onnx':continue
   z.write(file,Path('PrisonRunner')/rel)
 for name in ('AGENTS.md','README.md','START_HERE.md','THIRD_PARTY_NOTICES.md','StartGame.cmd','PlayInUnity.cmd','StartIdentity.cmd','SetupIdentity.cmd'):
  z.write(PROJECT/name,'PrisonRunner/'+name)
 z.write(report,'PrisonRunner/docs/reports/LOCAL_COMPLETION_2026_10_25.md')
 z.write(BASE/'StartGame.cmd','StartGame.cmd')
print('Source archive:',source.stat().st_size,flush=True)
service=DEST/'Mongscape-FaceLogin-DB.zip'
with zipfile.ZipFile(service,'w',zipfile.ZIP_DEFLATED,compresslevel=5) as z:
 z.writestr('READ_ME_FIRST.txt','''Local face identity service for Windows x64.
Run Services/Identity/StartIdentity.ps1, or run server.py with Python.
Models, CPython and local Python packages are included. No separate Python installation is needed.
A PostgreSQL database is required: set IDENTITY_DATABASE_URL or PGHOST/PGDATABASE/PGUSER before starting.
This is exhibition-profile recognition without verified liveness detection.
Read the Korean implementation report for details.
''')
 for file in sorted((PROJECT/'Services/Identity').rglob('*')):
  if not file.is_file():continue
  rel=file.relative_to(PROJECT)
  if any(part in ('data','__pycache__','secrets') for part in rel.parts) or file.name in ('.env','identity-server.json') or file.suffix in ('.log','.pyc','.dump'):continue
  z.write(file,rel)
 z.write(report,'구현_검증_안내.md')
print('Identity archive:',service.stat().st_size,flush=True)
for archive in (source,service):
 with zipfile.ZipFile(archive) as z:
  bad=z.testzip();assert bad is None,bad
  assert not any('/data/' in name or '.env' in name for name in z.namelist())
records=[]
for file in sorted(DEST.iterdir()):
 if file.is_file() and file.name!='SHA256.json':records.append(dict(file=file.name,bytes=file.stat().st_size,sha256=hashlib.sha256(file.read_bytes()).hexdigest()))
(DEST/'SHA256.json').write_text(json.dumps(records,ensure_ascii=False,indent=2),encoding='utf-8')
print('Archives verified; private profile data excluded',flush=True)


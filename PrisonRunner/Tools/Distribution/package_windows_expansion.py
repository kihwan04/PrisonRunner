"""Package the verified local Windows player and portable identity sidecar. Never uploads anything."""
from pathlib import Path
import hashlib,json,shutil,zipfile
ROOT=Path(__file__).resolve().parents[3]
PROJECT=ROOT/'PrisonRunner';DEST=ROOT/'Deliverables/2026-10-25';DEST.mkdir(parents=True,exist_ok=True)
BUILD=PROJECT/'Builds/Mongscape/Windows'
if not (BUILD/'Mongscape.complete').is_file():raise SystemExit('A successful Windows build is required before packaging.')
STAGE=DEST/'Mongscape-Windows';STAGE.mkdir(exist_ok=True)
def copy_files(source,dest,exclude):
 for f in sorted(source.rglob('*')):
  if not f.is_file():continue
  rel=f.relative_to(source)
  if any(p in exclude for p in rel.parts) or f.suffix in ('.log','.pyc'):continue
  target=dest/rel;target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(f,target)
copy_files(BUILD,STAGE/'Game',{'Mongscape_BurstDebugInformation_DoNotShip'})
copy_files(PROJECT/'Services/Identity',STAGE/'Services/Identity',{'data','__pycache__','secrets'})
(STAGE/'StartGame.cmd').write_text('''@echo off
setlocal
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Services\\Identity\\StartIdentity.ps1"
start "" /d "%~dp0Game" "%~dp0Game\\Mongscape.exe" %*
''',encoding='ascii')
(STAGE/'StartIdentity.cmd').write_text('''@echo off
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Services\\Identity\\StartIdentity.ps1"
''',encoding='ascii')
(STAGE/'README_KO.md').write_text('''# Mongscape Windows 실행본

압축을 모두 해제한 뒤 **StartGame.cmd**를 실행하세요. Unity와 Python을 별도로 설치할 필요가 없습니다.

- 탈출 준비에서 **키보드**를 고르면 웹캠 없이 플레이합니다.
- A/←, D/→: 좌우 이동 / 수레 기울이기. 수레에서 C: 가운데. Space: 점프. S/↓: 숙이기. W/↑: 하이니.
- 세 갈래가 보이는 넓은 구간에서 옆으로 한 걸음씩 이동해 선택합니다. 갈림길에서는 A/D가 한 차선씩 이동하고 C는 가운데입니다. 목적지는 무작위로 섞이며 진입 전에는 표시하지 않습니다. 갈라진 길은 다시 합쳐지지 않습니다.
- 선택이 확정되면 하차할 때까지 다른 레일로 이동할 수 없습니다. 도보 구간에서는 다시 자유롭게 레인을 바꿉니다. 분기 뒤에는 광산·감옥·동물원 중 무작위 맵이 연결됩니다.
- 터널의 빈 벽·바닥·천장과 맵 사이의 입구를 보강했습니다. 거리 안개와 구조 생성 거리를 맞춰 먼 곳의 청크 끝이 드러나지 않게 했습니다.
- 광산의 돌 수레, 감옥 철창, 동물원 사료 철장으로 막힌 차선은 점프로 넘을 수 없습니다. 옆 차선으로 피하세요. 바나나 껍질도 무작위로 많이 나옵니다.
- 겹친 원본 바닥을 정리하고 타일 필터를 보강했습니다. 도보는 8m/s부터 최대 12m/s까지 빨라지고 차단물도 더 자주 나옵니다. 안전한 차선은 최소 하나 유지됩니다.
- 바나나를 밟으면 화면 상단에 작은 빨강·파랑 사이렌이 나타납니다. 달리는 화면의 손·곤봉 경고는 제거했습니다. 누적 두 번이면 따라잡습니다. 잡히면 원숭이가 엎드려 뻗친 자세로 만화식 곤봉 연출을 받은 뒤 결과가 나옵니다. 로그인 얼굴 사진도 머리를 따라 유지됩니다.
- 동물원에서는 내려오는 기린 목과 코끼리 코를 뛰어넘습니다.
- F1: 설정. F3: 성능 표시. 저사양에서는 보통 또는 저사양 화질을 선택하세요.
- 얼굴 로그인과 손동작 조작에는 웹캠이 필요합니다. 최초 등록 동의 후 얼굴 특징과 사진·기록을 이 PC의 Services/Identity/data에 저장합니다.
- 얼굴 로그인은 전시 프로필용이며 사진 위조 방지 인증을 포함하지 않습니다. 얼굴 화면에서 내 얼굴과 기록을 삭제할 수 있습니다.
- 탈출 준비에서 칼로리 목표를 직접 숫자로 입력합니다(1~5000 kcal). 달성하거나 초과해도 게임은 계속됩니다. 칼로리는 몸무게와 인식된 운동 시간에 따른 예상치입니다. 키보드 조작은 운동 칼로리로 적립하지 않습니다.

exe 옆의 Data/DLL/MonoBleedingEdge와 Services 폴더를 함께 보관하세요. 프로그램을 종료해도 로컬 얼굴 서버는 다음 실행을 위해 유지될 수 있습니다.
''',encoding='utf-8')
shutil.copy2(PROJECT/'THIRD_PARTY_NOTICES.md',STAGE/'THIRD_PARTY_NOTICES.md')
report=PROJECT/'docs/reports/LOCAL_COMPLETION_2026_10_25.md'
if report.exists():shutil.copy2(report,STAGE/'구현_검증_안내.md')
archive=DEST/'Mongscape-Windows-x64.zip'
with zipfile.ZipFile(archive,'w',zipfile.ZIP_DEFLATED,compresslevel=5) as z:
 for f in sorted(STAGE.rglob('*')):
  if not f.is_file():continue
  rel=f.relative_to(STAGE)
  if any(part in ('data','__pycache__','secrets') for part in rel.parts) or f.name in ('.env','identity-server.json') or f.suffix in ('.log','.pyc','.dump'):continue
  z.write(f,rel)
with zipfile.ZipFile(archive) as z:
 assert z.testzip() is None
 assert 'Game/Mongscape.exe' in z.namelist()
 assert 'Services/Identity/python/python.exe' in z.namelist()
 assert not any('/data/' in name or name.endswith('.env') for name in z.namelist())
print(json.dumps(dict(archive=str(archive),bytes=archive.stat().st_size,sha256=hashlib.sha256(archive.read_bytes()).hexdigest()),indent=2),flush=True)

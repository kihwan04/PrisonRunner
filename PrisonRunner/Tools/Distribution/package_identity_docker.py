"""Prepare central identity deployment and an optional rebuilt Windows client. Never uploads."""
import argparse
import hashlib
import json
import zipfile
from pathlib import Path

PROJECT=Path(__file__).resolve().parents[2]
OUTPUT=PROJECT.parent/'Deliverables'/'2026-10-26'/'ForkShort'
SERVICE=PROJECT/'Services'/'Identity'
SERVER_FILES=(
    'Dockerfile','.dockerignore','requirements.txt','compose.yaml','compose.https.yaml',
    'Caddyfile','.env.example','README_DOCKER.md','server.py','schema.sql','setup_models.py',
    'models/provenance.json','models/face_detection_yunet.LICENSE','models/face_recognition_sface.LICENSE',
    'ConfigureClient.ps1','identity-server.example.json','test_identity.py','test_http.py',
    'storage.py','schema.postgres.sql','init-postgres.sh','prepare_secrets.py','migrate_sqlite.py',
    'test_postgres.py','setup_postgres_runtime.py',
)


def write_archive(destination,entries,texts=None):
    with zipfile.ZipFile(destination,'w',zipfile.ZIP_DEFLATED,compresslevel=5) as archive:
        for name,path in sorted(entries.items()):archive.write(path,name)
        for name,value in (texts or {}).items():archive.writestr(name,value)
    with zipfile.ZipFile(destination) as archive:
        if archive.testzip() is not None:raise RuntimeError('Archive CRC failed')
        for name in archive.namelist():
            parts=Path(name).parts
            if any(part in ('data','.runtime','python','__pycache__','secrets') for part in parts):
                raise RuntimeError('Private/runtime directory in deployment archive: '+name)
            if Path(name).name in ('.env','identity-server.json') or name.endswith(('.sqlite3','.db','.log','.dump')):
                raise RuntimeError('Local configuration/data in deployment archive: '+name)
    print(destination.name,destination.stat().st_size,'bytes',flush=True)


def main():
    global OUTPUT
    parser=argparse.ArgumentParser()
    parser.add_argument('--include-windows-client',action='store_true')
    parser.add_argument('--output-directory',type=Path,default=OUTPUT)
    args=parser.parse_args()
    OUTPUT=args.output_directory.resolve()
    if not OUTPUT.is_relative_to(PROJECT.parent):raise RuntimeError('Delivery folder must stay inside the workspace')
    OUTPUT.mkdir(parents=True,exist_ok=True)
    server_entries={name:SERVICE/name for name in SERVER_FILES}
    server_entries['THIRD_PARTY_NOTICES.md']=PROJECT/'THIRD_PARTY_NOTICES.md'
    write_archive(OUTPUT/'Mongscape-Identity-PostgreSQL-Docker.zip',server_entries)
    if args.include_windows_client:
        build=PROJECT/'Builds/Mongscape/Windows'
        if not (build/'Mongscape.complete').is_file():raise RuntimeError('Completed Windows build required')
        if not (build/'Mongscape_Data/Managed/Muhanok.Infrastructure.dll').is_file():
            raise RuntimeError('Expected Mono Windows build missing')
        assembly=(build/'Mongscape_Data/Managed/Muhanok.Infrastructure.dll').read_bytes()
        if b'IdentityServerConfig' not in assembly or 'identity-server.json'.encode('utf-16le') not in assembly:
            raise RuntimeError('Windows build does not contain the remote identity configuration')
        presentation=(build/'Mongscape_Data/Managed/Muhanok.Presentation.dll').read_bytes()
        if b'PlayerProgressStore' not in assembly or b'MainMenuScreen' not in presentation:
            raise RuntimeError('Windows build does not contain the integrated player/menu features')
        if b'RunHudModel' not in presentation:
            raise RuntimeError('Windows build does not contain the compact HUD')
        domain=(build/'Mongscape_Data/Managed/Muhanok.Domain.dll').read_bytes()
        if b'ConnectorChunk' not in domain:
            raise RuntimeError('Windows build does not contain the shortened fork layout')
        entries={}
        for file in build.rglob('*'):
            if not file.is_file():continue
            rel=file.relative_to(build)
            if any('DoNotShip' in part for part in rel.parts):continue
            if file.suffix in ('.log','.pdb') or file.name=='identity-server.json':continue
            entries['Game/'+rel.as_posix()]=file
        for name in ('ConfigureClient.ps1','identity-server.example.json','README_DOCKER.md'):
            entries[name]=SERVICE/name
        entries['THIRD_PARTY_NOTICES.md']=PROJECT/'THIRD_PARTY_NOTICES.md'
        font_license=PROJECT/'Unity/Assets/Game/Content/UI/Resources/MuhanokUI/BlackHanSans-OFL.txt'
        if font_license.is_file():entries['LICENSES/BlackHanSans-OFL.txt']=font_license
        guide='''# Mongscape PostgreSQL 중앙 서버용 Windows 클라이언트

압축 전체를 해제하세요. Unity/Python/Docker는 게임 PC에 설치할 필요가 없습니다.
1. PostgreSQL/Docker 서버를 먼저 배포하세요. 비밀번호 파일 준비와 실행 방법은 README_DOCKER.md에 있습니다.
2. identity-server.example.json을 Game/identity-server.json으로 복사하고 url을 실제 HTTPS 서버 주소로 바꾸세요.
3. StartGame.cmd를 실행하세요. 얼굴 등록/로그인과 기록은 연결된 서버 DB를 사용합니다.

이 ZIP은 실제 서버 주소나 개인 얼굴 DB를 포함하지 않습니다. 서버를 설정하지 않아도 키보드 게임은 실행할 수 있습니다.
환경 변수 MONGSCAPE_IDENTITY_URL이 있으면 파일보다 우선합니다. 주소 변경 후 게임을 재시작하세요.
A/D 좌우 이동, Space 점프, S 숙이기, W 하이니, F1 설정, F3 성능 표시.
기존 2026-10-25 실행본은 원격 주소 설정을 지원하지 않으므로 이 새 클라이언트를 사용하세요.
현재 클라이언트는 lilohdong/muhanok-test의 메뉴·프로필·상점·거리 랭킹과 기존 얼굴 로그인·칼로리 목표·수레 갈림길을 합친 소스의 빌드입니다.
세 갈래 철로는 48m로 줄였으며 선택 후 긴 직선 없이 다음 맵 입구로 이어집니다. 좌우 간격과 진입 곡선도 함께 조정했습니다.
메인 메뉴에서 게임 시작, 랭킹, 상점, 프로필, 얼굴 로그인, 설정과 게임 종료를 선택합니다. Ctrl+Shift+F12는 개발자/전시 모드입니다.
상점·거리 랭킹·화면 설정은 PC 로컬이고, 얼굴 로그인·개인 운동 기록은 PostgreSQL 서버를 사용합니다.
거리·최고 기록·운동 모드의 칼로리 목표는 오른쪽 위에 모으고 코인은 왼쪽 위에 표시합니다. 방향 안내는 한 번에 하나, 상세 수치는 일시정지/결과에서 확인합니다.
'''
        launcher='@echo off\r\nsetlocal\r\nstart "" /d "%~dp0Game" "%~dp0Game\\Mongscape.exe" %*\r\n'
        write_archive(OUTPUT/'Mongscape-RemoteClient-Windows-x64.zip',entries,
                      {'StartGame.cmd':launcher,'README_KO.md':guide})
    records=[]
    for file in sorted(OUTPUT.glob('Mongscape-*.zip')):
        with file.open('rb') as stream:digest=hashlib.file_digest(stream,'sha256').hexdigest()
        records.append(dict(file=file.name,bytes=file.stat().st_size,
                            sha256=digest))
    (OUTPUT/'SHA256.json').write_text(json.dumps(records,indent=2),encoding='utf-8')
    guide='''# Mongscape PostgreSQL 중앙 얼굴 서버 · 전달 안내 (2026-10-26)

- `Mongscape-Identity-PostgreSQL-Docker.zip`: PostgreSQL 17 + 얼굴 API + Caddy HTTPS 배포 파일입니다. README_DOCKER.md를 따라 먼저 비밀번호 파일을 생성하고 실제 도메인을 `.env`에 설정합니다.
- `Mongscape-RemoteClient-Windows-x64.zip`: 게임 PC마다 풀어서 사용합니다. EXE 옆 `Game/identity-server.json`에 같은 실제 HTTPS 서버 주소를 지정한 뒤 StartGame.cmd를 실행합니다. 게임 PC에 Docker나 Python을 설치할 필요가 없습니다.
- `SHA256.json`: ZIP 파일 크기와 SHA-256 검증값입니다.

실제 서버와 도메인은 아직 없고 외부 배포/인증서 발급은 하지 않았습니다. 이 PC에 Docker가 없어 컨테이너 빌드/기동은 미검증입니다. 개인 얼굴 데이터, DB, 토큰, 실제 서버 주소는 ZIP에 넣지 않았습니다. 이 도구는 로컬 파일만 준비하며 공개 다운로드는 저장소 릴리스를 확인하세요.

앞선 서버 작업 검증: 실제 PostgreSQL 17.11에서 얼굴/HTTP/트랜잭션/권한/SQLite 이전 테스트 18개, 기존 SQLite 얼굴·HTTP 테스트 9개 통과. PostgreSQL은 임시 로컬 클러스터에서 비관리자 앱 계정으로 확인했고 종료했습니다. 실제 개인 DB를 이전하지 않았습니다. 이번 철로 변경에서는 서버를 변경하지 않았습니다.
Windows EXE는 lilohdong/muhanok-test b2215ef의 UI 기능을 기존 로컬 게임/얼굴/PostgreSQL 연결과 합친 소스에서 빌드합니다. 메뉴·상점·거리 랭킹·개발자 모드와 기존 갈림길/운동/얼굴 기능을 함께 사용합니다.
철로 단축 후 Unity EditMode 115/115, PlayMode 37/37을 통과했습니다. Windows 빌드·세 방향 입구 화면·기본 플레이·정상 종료를 확인했습니다. 고정된 값의 HUD 모델 갱신도 관리 메모리 할당 0바이트를 유지했습니다(화면 렌더링 전체를 뜻하지 않음). 근거는 프로젝트의 docs/reports/FORK_SHORT_2026_10_26.md에 있습니다. 이번 경로 검수는 FPS 측정을 다시 수행한 결과가 아닙니다. 상점 지갑과 거리 랭킹은 PC 로컬 저장이며 서버 DB의 개인 운동 기록과 구분합니다.
Docker 컨테이너 기동·외부 HTTPS·실물 웹캠/본인 얼굴/여러 PC의 실제 접속을 이번 작업에서 검증한 것은 아닙니다.

다음 단계: 실제 서버와 도메인 준비 → 비밀번호 파일 생성 → PostgreSQL/Docker HTTPS 배포 → 외부 health 응답의 storage=postgres 확인 → 두 게임 PC에서 등록/로그인/기록 공유 확인.
'''
    (OUTPUT/'배포_실행_안내.md').write_text(guide,encoding='utf-8')


if __name__=='__main__':main()

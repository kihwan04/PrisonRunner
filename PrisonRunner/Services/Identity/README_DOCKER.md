# Mongscape 중앙 얼굴 서버 · PostgreSQL + Docker

여러 게임 PC가 하나의 얼굴 로그인 서버와 PostgreSQL DB를 공유합니다. 게임 PC에는 Python·Docker·PostgreSQL을 설치하지 않습니다. 몸동작 추론은 각 PC의 Unity에서 계속 처리합니다.

```text
게임 PC들 → HTTPS/Caddy → 얼굴 API(Python·OpenCV·Psycopg 3) → PostgreSQL 17
                                                               ↓
                                                       postgres-data volume
```

배포 파일은 [공개 릴리스](https://github.com/kihwan04/PrisonRunner/releases/tag/v2026.10.26-r2)에서 제공합니다. 실제 서버·도메인은 아직 배포하지 않았습니다. 실제 PostgreSQL 17.11에서 로그인/기록/HTTP/이전 테스트를 통과했지만, 이 PC에는 Docker가 없어 Linux 컨테이너 기동과 외부 HTTPS는 아직 검증하지 못했습니다.

## 1. 배포 파일과 비밀번호 준비

Docker Engine/Compose v2가 있는 Linux 서버에 이 ZIP을 풉니다. Docker Desktop의 Linux 컨테이너 모드도 사용할 수 있습니다. 이 폴더 안에서 명령을 실행합니다.

먼저 DB 관리자와 앱 계정의 비밀번호 파일을 만듭니다. 서버에 Python 3이 있으면:

```sh
python3 prepare_secrets.py
```

Python 설치 없이 Docker만 사용하려면 Linux 서버에서:

```sh
docker run --rm --user "$(id -u):$(id -g)" -v "$PWD:/work" -w /work python:3.12-slim-bookworm python prepare_secrets.py
```

`secrets/db_admin_password`, `secrets/db_app_password`가 생성됩니다. 값은 출력하지 않고 기존 파일은 덮어쓰지 않습니다. 비밀번호 폴더는 Git/ZIP/이미지에서 제외합니다. Linux에서는 폴더를 0700, 파일을 읽기 전용으로 두어 Compose가 비관리자 컨테이너 사용자에게 해당 파일을 마운트할 수 있도록 합니다.

DB의 `postgres` 관리자와 API의 `mongscape` 계정을 분리합니다. 앱 계정은 SUPERUSER/CREATEDB/CREATEROLE 권한이 없고 자기 DB의 테이블을 사용합니다. DB 포트 5432는 외부에 공개하지 않습니다.

## 2. 서버 PC에서 먼저 실행

외부 도메인 없이 Docker와 PostgreSQL을 확인하려면:

```sh
docker compose up -d --build --wait
docker compose ps
curl --fail http://127.0.0.1:8765/health
```

health 응답의 `ok`가 true이고 **`storage`가 `postgres`**인지 확인합니다. DB 준비 → API 준비 순서로 시작하며 health는 실제 DB 연결도 확인합니다. 이미 로컬 Python 서버가 8765를 사용한다면 해당 서버를 종료하거나 `.env`의 `IDENTITY_LOCAL_PORT`를 바꿉니다.

최초 이미지 빌드에서 Python/OpenCV/NumPy/Psycopg와 얼굴 모델을 내려받습니다. 얼굴 모델은 manifest의 SHA-256으로 검사합니다. Windows `.runtime`, 실제 얼굴 DB와 비밀번호는 이미지에 넣지 않습니다.

## 3. 여러 PC를 위한 HTTPS

실제 도메인의 DNS를 서버 공인 주소로 연결하고 TCP 80/443을 준비합니다. AAAA 레코드가 있다면 IPv6도 실제 서버로 연결돼야 합니다. `.env.example`을 `.env`로 복사해 도메인을 수정합니다.

```dotenv
IDENTITY_DOMAIN=faces.your-domain.com
IDENTITY_LOCAL_PORT=8765
```

도메인에는 `https://`나 경로를 붙이지 않습니다. 예시 주소는 실제 주소로 바꿉니다.

```sh
docker compose --env-file .env -f compose.yaml -f compose.https.yaml up -d --build --wait
docker compose --env-file .env -f compose.yaml -f compose.https.yaml logs --tail=100
curl --fail https://faces.your-domain.com/health
```

Caddy가 HTTPS 인증서 발급/갱신과 API 전달을 담당합니다. Compose 기동 성공 뒤에도 외부 HTTPS health로 DNS·인증서를 따로 확인합니다. API/Caddy는 얼굴 이미지나 토큰을 요청 로그에 기록하지 않습니다.

## 4. 게임 PC 연결

2026-10-26 원격 연결을 지원하는 Windows 클라이언트 ZIP을 전부 풉니다. PostgreSQL 변경은 서버 내부이므로 같은 EXE와 게임 API를 사용합니다. 2026-10-25의 localhost 고정 실행본은 원격 설정을 지원하지 않습니다.

각 PC에서 EXE가 있는 `Game` 폴더에 `identity-server.json`을 둡니다.

```json
{
  "url": "https://faces.your-domain.com"
}
```

동봉한 `identity-server.example.json`을 복사하거나 다음 PowerShell 도구를 사용합니다.

```powershell
.\ConfigureClient.ps1 -ServerUrl 'https://faces.your-domain.com' -GameDirectory 'C:\Mongscape\Game'
```

환경 변수 `MONGSCAPE_IDENTITY_URL`이 파일보다 우선합니다. 개발용 Unity는 `Unity/identity-server.json`을 읽습니다. 설정은 게임 시작 때 읽으므로 변경 뒤 게임을 다시 실행합니다. 원격 주소는 HTTPS, 로컬 Docker 확인은 loopback HTTP만 허용합니다.

게임에 DB 주소·사용자·비밀번호를 넣지 않습니다. 게임은 얼굴 API만 호출합니다. 얼굴 등록/로그인은 선택 사항이며 서버 없이도 키보드와 몸동작 플레이를 사용할 수 있습니다.

## 5. PostgreSQL 직접 살펴보기

서버에서 DB 콘솔을 엽니다. 앱 계정으로 실행합니다.

```sh
docker compose exec postgres psql -U mongscape -d mongscape
```

```sql
\dt
SELECT COUNT(*) AS profiles FROM profiles;
SELECT COUNT(*) AS runs FROM runs;
SELECT id, score, distance, kcal, created_at FROM runs ORDER BY created_at DESC LIMIT 10;
\q
```

`profiles`는 이름·얼굴 사진, `face_samples`는 특징 벡터, `sessions`는 토큰 해시·만료, `runs`는 운동 기록입니다. 얼굴 바이너리는 BYTEA, 동작별 기록은 JSONB, 시간은 Unix 초(BIGINT)입니다. 관련 프로필을 삭제하면 FK CASCADE로 특징·기록·세션도 삭제됩니다. 전시용 순위·화면 설정은 각 게임 PC의 로컬 저장입니다.

## 6. 기존 SQLite 데이터를 옮기려면

자동으로 이전하지 않습니다. 빈 PostgreSQL 대상에만 별도 도구로 가져옵니다. 기존 대상에 데이터가 있으면 거부하고, 오류가 나면 전체 트랜잭션을 롤백합니다. 프로필·사진·특징·운동 기록과 ID를 유지하고 로그인 세션은 복사하지 않으므로 다시 얼굴 로그인합니다.

원본 SQLite를 직접 옮기지 말고 먼저 SQLite backup API로 일관된 복사본을 준비하세요. 예를 들어 로컬 DB가 있는 폴더에서:

```sh
python -c "import sqlite3; s=sqlite3.connect('file:profiles.sqlite3?mode=ro',uri=True); d=sqlite3.connect('profiles-copy.sqlite3'); s.backup(d); d.close(); s.close()"
```

복사본을 배포 서버로 직접 전달한 뒤, Linux 서버에서 다음을 실행합니다. API가 아직 사용자 등록을 받기 전에 진행합니다.

```sh
docker compose exec -T identity python -c "import sys; from pathlib import Path; Path('/tmp/legacy.sqlite3').write_bytes(sys.stdin.buffer.read())" < ./profiles-copy.sqlite3
docker compose exec -T identity python migrate_sqlite.py /tmp/legacy.sqlite3
docker compose exec -T identity python -c "from pathlib import Path; Path('/tmp/legacy.sqlite3').unlink()"
```

도구는 원본을 읽기 전용으로 열고 대상 테이블을 잠근 상태에서 빈 DB인지 확인합니다. 결과는 개인 이름/사진 없이 테이블별 건수만 출력합니다. 원본과 백업을 Git/공개 ZIP에 넣지 않습니다. 실제 사용자 DB를 이번 작업에서 이전한 것은 아닙니다.

## 7. DB 보존과 백업

DB는 `postgres-data` volume에 보존합니다. 컨테이너 재생성은 기록을 지우지 않습니다. `docker compose down -v`는 volume까지 삭제하므로 데이터 유지가 필요할 때 사용하지 않습니다.

```sh
docker compose exec -T postgres pg_dump -U postgres -d mongscape -Fc -f /tmp/mongscape.dump
docker compose cp postgres:/tmp/mongscape.dump ./mongscape-backup.dump
```

백업은 개인 얼굴 정보와 기록을 포함하므로 별도로 보관하고 Git/ZIP에서 제외합니다. 복구는 백업을 새 DB에 복원해 검증한 뒤 진행합니다. 기존 SQLite Docker volume은 PostgreSQL로 자동 변환하거나 삭제하지 않습니다.

비밀번호 초기화 스크립트는 새 DB volume의 최초 초기화 때만 실행됩니다. 이미 초기화한 DB에서 비밀번호 파일만 바꿔도 DB 암호가 자동 변경되지는 않습니다.

## 로컬 SQLite와 확인 범위

기존 Python 로컬 실행은 PostgreSQL 설정이 없으면 SQLite를 사용합니다. PostgreSQL을 로컬 Python에서 직접 연결하려면 Psycopg를 설치하고 `IDENTITY_DATABASE_BACKEND=postgres`와 PGHOST/PGPORT/PGDATABASE/PGUSER/PGPASSWORD_FILE 또는 `IDENTITY_DATABASE_URL`을 지정합니다. Windows용 선택 설치 도구는 `setup_postgres_runtime.py`입니다.

API는 현재 단일 프로세스에서 요청을 순서대로 처리하며 얼굴 등록/로그인은 서버 전체 분당 15회 제한입니다. PostgreSQL을 도입했다고 얼굴 추론 병렬 처리까지 구현한 것은 아닙니다. 실제 검증 결과와 남은 확인은 프로젝트의 `docs/reports/POSTGRES_IDENTITY_2026_10_26.md`를 확인하세요. 얼굴 로그인은 전시 프로필용이고 검증된 사진 위조 방지 기능은 없습니다.

공식 문서: [PostgreSQL Docker 이미지](https://hub.docker.com/_/postgres), [Psycopg 트랜잭션](https://www.psycopg.org/psycopg3/docs/basic/transactions.html), [Docker Compose](https://docs.docker.com/compose/gettingstarted/), [Caddy HTTPS](https://caddyserver.com/docs/automatic-https).

# PostgreSQL 중앙 얼굴 서버 · 2026-10-26

중앙 얼굴 서버의 기본 DB를 **PostgreSQL 17**로 바꾸고 Docker 배포 파일을 다시 준비했습니다. 기존 로컬 Python의 SQLite 경로와 Windows 게임 API는 유지했습니다. 실제 외부 배포와 GitHub 업로드는 하지 않았습니다.

## 생성·수정 파일과 구현

- `Services/Identity/storage.py`: PostgreSQL/Psycopg와 기존 SQLite 연결, 매개변수 SQL, 명시적 트랜잭션, 끊긴 연결의 재연결.
- `schema.postgres.sql`: 프로필/특징/세션/운동 기록, BYTEA·JSONB, FK 삭제 연쇄와 조회/삭제용 인덱스.
- `server.py`: DB 선택과 공통 저장 SQL, 실제 DB를 확인하는 health 응답의 `storage` 필드.
- `compose.yaml`, `Dockerfile`, `requirements.txt`, `.dockerignore`: PostgreSQL 17·영구 volume·healthcheck·Psycopg 3.3.6. DB 포트는 외부에 공개하지 않음.
- `init-postgres.sh`, `prepare_secrets.py`: 관리자/앱 계정 분리, 앱의 SUPERUSER/CREATEDB/CREATEROLE 제거, 비밀번호 파일 최초 생성과 기존 값 보존.
- `migrate_sqlite.py`: 원본 읽기 전용, 빈 PostgreSQL에 트랜잭션으로 가져오기, 오류 시 전체 롤백. 프로필/특징/운동 기록을 유지하고 세션은 이전하지 않음.
- `test_postgres.py`, `Tools/QA/verify_postgres.py`: 실제 PostgreSQL·비관리자 앱 계정 검증. 생성한 임시 스키마/클러스터만 사용하고 종료.
- `setup_postgres_runtime.py`: 선택적인 Windows 드라이버 설치. 공식 PyPI wheel SHA-256 검사.
- `test_identity.py`, `test_http.py`: 동일 얼굴/HTTP 검증을 두 DB에 적용하도록 테스트 DB 생성 경로 분리.
- `.gitignore`와 패키징 도구: 비밀번호·DB dump·실제 설정 제외. PostgreSQL 서버 ZIP과 호환 Windows 클라이언트를 새 폴더에 전달.
- 현재 실행/설계/스택/진행/배포 안내: PostgreSQL 기준으로 갱신. 이전 SQLite 보고서와 ZIP은 보존.

## 실제 검증

| 검사 | 결과 |
| --- | --- |
| 실제 엔진 | 공식 EDB portable PostgreSQL 17.11. OS 서비스 설치 없이 임시 폴더에서 실행 |
| PostgreSQL 얼굴/HTTP/저장/이전 | **18/18 통과** |
| 기존 SQLite 얼굴 모델 | **4/4 통과** |
| 기존 SQLite HTTP | **5/5 통과** |

PostgreSQL에서 실제 YuNet/SFace 등록·재로그인·기록 중복 방지·삭제 연쇄, Host/Origin/인증/JSON/요청 크기, API 재시작 뒤 기록 유지, 트랜잭션 롤백, BYTEA/JSONB, 인덱스, 비관리자 권한, 연결 재연결을 확인했습니다. Docker와 같은 PGHOST/PGUSER/PGPASSWORD_FILE 방식도 검사했습니다.

SQLite 이전은 사진/특징/운동 기록 보존, 원본 해시 불변, 세션 미이전, 비어 있지 않은 대상 거부, 오류 시 전체 롤백을 확인했습니다. 실제 개인 DB를 이전/업로드하지 않았고 공개 테스트 사진과 임시 데이터만 사용했습니다.

원시 로그 위치는 `Builds/QA/postgres-results.json`에 있습니다. 제한 환경의 Windows 프로세스 생성 차단을 해소한 사용자 권한에서 최종 검증을 완료했고 검증용 DB를 종료했습니다.

## Windows와 전달 파일

게임 C#·씬·프리팹과 API 요청 형식은 이번 DB 작업에서 변경하지 않았습니다. 앞서 빌드·스모크를 완료한 2026-10-26 Windows EXE를 재동봉하며 재빌드를 수행한 결과로 표현하지 않습니다. 이전 C# 컴파일 오류 0개 기록은 [초기 중앙 서버 보고서](DOCKER_IDENTITY_2026_10_26.md)에 있습니다.

새 전달 폴더는 작업 폴더의 `Deliverables/2026-10-26/PostgreSQL/`입니다. 서버 ZIP, Windows 클라이언트 ZIP, 실행 안내와 SHA-256을 제공합니다. 실제 비밀번호·개인 DB·사진·토큰·실제 서버 주소는 포함하지 않습니다.

## 남은 확인과 다음 작업

Docker가 없어 실제 Linux 컨테이너 기동과 Caddy 인증서 발급은 아직 검증하지 못했습니다. 서버·도메인 준비 → 비밀번호 생성 → Compose 기동 → 외부 HTTPS health의 `storage=postgres` 확인 → 두 게임 PC의 등록/로그인/기록 공유 순서로 확인해야 합니다. 실물 웹캠/본인 얼굴 검증도 별도입니다.

현재 API는 단일 프로세스에서 요청을 순서대로 처리하고 등록/로그인은 서버 전체 분당 15회 제한입니다. 얼굴 추론 병렬 처리나 대규모 운영 검증까지 완료한 구현은 아닙니다.

실행·SQL 확인·백업·이전 명령은 [Docker 서버 안내](../../Services/Identity/README_DOCKER.md)에 있습니다.

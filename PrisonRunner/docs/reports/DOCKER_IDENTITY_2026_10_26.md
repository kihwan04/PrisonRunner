# 중앙 얼굴 로그인 서버 준비 · 2026-10-26

사용자 요청에 따라 여러 PC의 게임이 한 Docker 서버의 얼굴 프로필과 운동 기록을 공유하도록 배포 파일과 Windows 클라이언트를 준비했습니다. 서버/도메인이 아직 없으므로 실제 외부 배포는 하지 않았으며 GitHub 업로드도 보류했습니다.

## 생성 파일

- `Services/Identity/Dockerfile`, `.dockerignore`, `requirements.txt`: Linux Python/OpenCV 이미지와 최소 빌드 컨텍스트.
- `Services/Identity/compose.yaml`, `compose.https.yaml`, `Caddyfile`, `.env.example`: API, HTTPS 진입점, 영구 DB volume, healthcheck.
- `Services/Identity/ConfigureClient.ps1`, `identity-server.example.json`, `README_DOCKER.md`: 각 PC의 서버 주소 설정과 배포/운영 안내.
- `Services/Identity/test_http.py`: HTTP 경계와 서로 다른 클라이언트의 기록 공유/서버 재시작 검증.
- `Unity/Assets/Game/Tests/EditMode/IdentityServerConfigTests.cs`와 `.meta`: 주소 허용/차단 및 환경 변수 우선순위 테스트.
- `Tools/Distribution/package_identity_docker.py`: 개인 데이터를 제외한 서버 ZIP과 새 Windows 클라이언트 ZIP 생성.
- `Deliverables/2026-10-26/`: 서버/클라이언트 ZIP, SHA-256 목록과 전달 안내.

## 수정 파일과 동작

- `Services/Identity/server.py`: bind 주소/포트/DB 경로/허용 Host 설정. 기존 loopback 기본값과 API/DB 스키마를 유지하고 Docker에서 외부 프록시에 연결할 수 있도록 구성했습니다.
- `Services/Identity/setup_models.py`: 저장소 provenance에 고정한 모델 크기와 SHA-256을 검사합니다.
- `Unity/Assets/Game/Infrastructure/LocalProfileClient.cs`: 환경 변수 → EXE 옆 설정 파일 → loopback 기본값. 원격 HTTPS, 로컬 loopback HTTP만 허용합니다. 잘못된 주소를 다른 서버로 자동 전환하지 않습니다.
- `Unity/Assets/Game/Presentation/UI/IdentityScreen.cs`: 얼굴 정보가 연결된 서버에 저장된다는 동의 안내.
- `Services/Identity/StartIdentity.ps1`, `StartGame.cmd`: 서버 주소가 설정돼 있으면 로컬 Python/별도 DB를 시작하지 않습니다.
- `.gitignore`: 설치별 서버 설정과 DB 백업을 제외합니다.
- `Tools/QA/RuleRunner.cs`, `verify_expansion.ps1`: 새 주소/HTTP 테스트를 검증 경로에 포함했습니다.
- `Tools/Distribution/package_expansion.py`: 모델 manifest와 Docker 배포 파일을 소스에 포함하고 로컬 설정/모델 바이너리를 제외합니다.
- `README.md`, `START_HERE.md`, `docs/TECH_STACK.md`, `AGENTS.md`: 중앙 서버와 기존 로컬 방식의 실행 안내.

## 검증

- Unity 6000.3.10f1 C# 컴파일러로 11개 어셈블리 컴파일 성공, **컴파일 오류 0개**. 기존 맵 코드의 obsolete/이름 숨김 경고는 남아 있습니다.
- Unity 엔진 없이 실행 가능한 규칙/입력/주소 테스트 **86/86 통과**. 이번 서버 주소 관련 테스트는 13개입니다. 전체 Unity EditMode/PlayMode 테스트를 이번 작업에서 다시 실행한 결과는 아닙니다.
- 실제 YuNet/SFace 모델을 사용한 얼굴 등록/로그인/DB 재연결/기록/삭제 테스트 **4/4 통과**. 임시 DB와 공개 테스트 이미지를 사용했고 개인 얼굴 데이터는 사용하지 않았습니다.
- HTTP Host/Origin/세션/요청 크기/JSON/서버 재시작과 기록 공유 테스트 **5/5 통과**.
- 두 Compose YAML 파싱, volume/포트/healthcheck/서비스 연결 구조 검사 통과. 실제 모델 두 개의 고정 SHA-256 검사 통과.
- Windows PowerShell 설정 스크립트 파싱, 테스트 폴더에 주소 설정, 원격 설정 시 로컬 서버 시작을 건너뛰는 동작 통과.
- 기존 사용자 권한에서 Unity Windows 빌드 성공, 종료 코드 0. 기본 실행 파일은 `Builds/Mongscape/Windows/Mongscape.exe`입니다. 첫 제한 실행 환경에서는 Unity 라이선스 접근 실패로 종료됐으며 사용자 권한 재실행에서 해결됐습니다.
- 새 EXE의 로딩 → 플레이 → 120청크 재사용 → 충돌 → 결과 → 재시작 스모크 통과, 종료 코드 0. 종료 때 기존 Development Build의 메모리 정리 진단이 남습니다.
- 서버/Windows 클라이언트 ZIP의 CRC 검사와 개인 데이터 경로 제외 검사 통과. 클라이언트 DLL에 새 원격 설정 코드가 포함됐는지 확인했고 ZIP SHA-256 목록을 생성했습니다.

새 Windows EXE의 기본 플레이 스모크 결과와 ZIP 무결성은 전달 폴더의 최종 검증 기록을 확인하세요.

## 남은 확인과 다음 작업

이 PC에는 Docker가 없어 실제 Linux 컨테이너 이미지 빌드/기동은 검증하지 못했습니다. 서버와 도메인이 준비되면 Docker Compose 실행 → 외부 HTTPS health 확인 → 서로 다른 게임 PC에서 얼굴 등록/로그인과 기록 공유를 확인해야 합니다. 실제 웹캠/본인 얼굴 검증도 아직 하지 않았습니다.

기존 로컬 얼굴 DB는 자동으로 서버에 복사하지 않습니다. 배포 ZIP에는 얼굴 사진/프로필/DB/토큰/실제 서버 주소를 포함하지 않습니다. 기존 2026-10-25 ZIP은 보존하며 중앙 서버 주소는 이번 새 Windows 클라이언트에서 설정합니다.

# Mongscape · 몽스케이프 (무한옥)

키보드 또는 웹캠 몸동작으로 광산·수레 갈림길·감옥·동물원을 달리는 Windows 운동 러너입니다. Unity 안에서 몸동작을 인식하고, 선택적 얼굴 로그인과 개인 운동 기록은 별도 서버에 저장합니다.

## 다운로드와 실행

[최신 데모 릴리스 · v2026.10.26](https://github.com/kihwan04/PrisonRunner/releases/tag/v2026.10.26)

| 파일 | 용도 |
| --- | --- |
| [Windows 게임](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26/Mongscape-RemoteClient-Windows-x64.zip) | ZIP 전체를 풀고 `StartGame.cmd` 실행 |
| [PostgreSQL Docker 서버](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26/Mongscape-Identity-PostgreSQL-Docker.zip) | 여러 게임 PC가 공유할 얼굴 로그인 API·DB·HTTPS 배포 파일 |
| [최신 기획서 V13](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26/Mongscape-Planning-V13.zip) | 화면별 기획서와 문서 검수 기록 |
| [서버·클라이언트 연결 안내](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26/Deployment-Guide-KO.md) | 서버 설정과 게임 PC의 주소 지정 |
| [SHA-256](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26/SHA256SUMS.txt) | 다운로드 파일 검증값 |

게임 PC에는 Unity·Python·Docker를 따로 설치하지 않아도 됩니다. `Game` 폴더 전체를 유지하세요. 얼굴 기능 없이 키보드·몸동작으로 플레이할 수 있습니다. 얼굴 로그인을 사용하려면 서버를 먼저 배포하고 EXE 옆 `identity-server.json`에 서버 주소를 지정합니다.

키보드: **A/D** 이동, **C** 갈림길 가운데 선택, **Space** 점프, **S** 숙이기, **W** 무릎 들기, **F1** 설정, **F3** 성능 표시.

## 이번 버전

- 메인 메뉴·PC 프로필·코인 상점·거리 랭킹과 개발자/전시 모드 통합.
- 거리·최고 기록·운동 목표를 오른쪽 위에 모은 HUD. 방향 안내는 한 번에 하나만 표시.
- 세 갈래 철로를 **120m에서 48m로 단축**, 선택 후 긴 직선을 제거하고 다음 맵 입구로 연결.
- PostgreSQL 17·Docker Compose·Caddy HTTPS 중앙 얼굴 서버와 원격 서버 주소 설정.

코인·상점·메뉴 거리 랭킹은 각 PC에 저장됩니다. 얼굴 프로필과 프로필별 운동 기록은 연결된 서버에 저장됩니다.

## 개발과 문서

Unity **6000.3.10f1**로 `PrisonRunner/Unity`를 열고 **Mongscape → Play GameScene**을 실행합니다. 게임 규칙은 C#, 렌더링은 URP, 몸동작은 Unity AI Inference/BlazePose, 얼굴 서버는 Python/OpenCV/Psycopg를 사용합니다.

| 목적 | 문서 |
| --- | --- |
| 게임 실행과 서버 주소 | [처음 읽기](PrisonRunner/START_HERE.md) |
| 기술 스택과 실행 구조 | [TECH_STACK](PrisonRunner/docs/TECH_STACK.md) |
| Docker·PostgreSQL·HTTPS | [서버 배포 안내](PrisonRunner/Services/Identity/README_DOCKER.md) |
| 현재 범위와 검증 | [진행 상태](PrisonRunner/docs/PROGRESS.md) |
| 기획·개발·과거 기록 | [문서 목록](PrisonRunner/docs/README.md) |
| 에셋 출처·재배포 조건 | [외부 리소스 안내](PrisonRunner/THIRD_PARTY_NOTICES.md) |

철로 단축 후 Unity EditMode **115/115**, PlayMode **37/37**과 Windows 실행 검수를 통과했습니다. 앞선 서버 작업에서는 PostgreSQL 18개·SQLite 9개 테스트를 통과했습니다. 실제 Docker 컨테이너 기동·외부 HTTPS·여러 PC 접속·실물 웹캠은 별도 검수가 필요합니다. 자동 메뉴 캡처 종료 중 발생한 네이티브 오류 등 남은 항목은 진행 상태와 검수 보고서에 기록했습니다.

개인 얼굴 DB·토큰·비밀번호·설치별 서버 주소·Unity 캐시와 재배포 제한이 있는 Mixamo 원본은 공개 소스에서 제외합니다. 공개 소스의 애니메이션은 자체 Blender 동작을 사용하며 리소스 조건은 외부 리소스 안내를 따릅니다. 이전 릴리스와 기획 기록은 보존합니다.

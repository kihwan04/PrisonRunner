# Mongscape(무한옥) Windows 데모 · v2026.10.26

키보드 또는 웹캠 전신·상체 동작으로 플레이하는 Unity 러너 게임입니다. 메뉴·프로필·상점·거리 랭킹, 오른쪽 위 HUD, 48m로 줄인 세 갈래 철로와 PostgreSQL 중앙 얼굴 서버 배포 파일을 포함합니다.

| 파일 | 사용 방법 |
| --- | --- |
| [Windows 게임](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26/Mongscape-RemoteClient-Windows-x64.zip) | 전체 압축 해제 후 `StartGame.cmd` 실행 |
| [PostgreSQL Docker 서버](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26/Mongscape-Identity-PostgreSQL-Docker.zip) | 중앙 서버에 얼굴 API·PostgreSQL 17·Caddy HTTPS 배포 |
| [기획서 V13](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26/Mongscape-Planning-V13.zip) | 최신 화면별 기획 문서와 검수 기록 |
| [연결 안내](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26/Deployment-Guide-KO.md) | 서버 배포·게임 PC의 서버 주소 지정 |
| [SHA-256](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26/SHA256SUMS.txt) | 파일 검증값 |

게임 PC에 Unity·Python·Docker 설치는 필요 없습니다. 얼굴 없이 키보드/몸동작으로 플레이할 수 있습니다. 얼굴 등록·로그인·개인 기록을 쓰려면 서버를 별도로 배포하고 EXE 옆 `identity-server.json`에 실제 HTTPS 주소를 지정하세요. PC 코인·거리 랭킹은 로컬 저장입니다.

철로 단축 후 Unity EditMode **115/115**, PlayMode **37/37**, Windows 빌드·3방향 입구 검수·기본 플레이/재시작 검사가 통과했습니다. 앞선 서버 작업은 실제 PostgreSQL 17.11 **18개**, SQLite **9개** 테스트를 통과했습니다. 실제 Docker 기동·외부 HTTPS·여러 PC 연결·실물 웹캠은 별도 확인이 필요합니다. 자동 전체 메뉴 캡처 종료 중 발생한 네이티브 오류와 개발 빌드 종료 메모리 정리 진단은 남아 있습니다.

실행과 데이터 범위는 [처음 읽기](https://github.com/kihwan04/PrisonRunner/blob/v2026.10.26/PrisonRunner/START_HERE.md), [기술 스택](https://github.com/kihwan04/PrisonRunner/blob/v2026.10.26/PrisonRunner/docs/TECH_STACK.md), [서버 안내](https://github.com/kihwan04/PrisonRunner/blob/v2026.10.26/PrisonRunner/Services/Identity/README_DOCKER.md), [진행 상태](https://github.com/kihwan04/PrisonRunner/blob/v2026.10.26/PrisonRunner/docs/PROGRESS.md), [철로 검수](https://github.com/kihwan04/PrisonRunner/blob/v2026.10.26/PrisonRunner/docs/reports/FORK_SHORT_2026_10_26.md)를 확인하세요.

개인 얼굴 DB·사진·토큰·비밀번호·설치별 설정과 Mixamo 원본 FBX는 공개 소스/ZIP에 넣지 않습니다. 공개 소스는 자체 Blender 동작으로 연결합니다. 리소스 조건은 [외부 리소스 안내](https://github.com/kihwan04/PrisonRunner/blob/v2026.10.26/PrisonRunner/THIRD_PARTY_NOTICES.md)를 따릅니다. 프로젝트 전체에 새로운 오픈소스 라이선스를 부여한 배포가 아닙니다. 기획서의 원래 이름과 V10~V12 원본 보관 기록은 유지했습니다.

Mixamo 원본을 제외한 공개 소스에서도 자체 Humanoid 애니메이션 연결 18개와 PlayMode 37/37을 추가 확인했습니다.

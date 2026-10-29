# Mongscape(무한옥) Windows 데모 · v2026.10.26-r2

전체 플레이 영상과 최신 Windows 게임입니다. 목표·소모 칼로리 HUD, 갈림길 방향 표지와 선택 레일, 손 커서 메뉴, 자동 화질 조절, 연결된 기린 목·코끼리 코와 쓰러지는 나무, 복원된 경찰 사이렌을 포함합니다.

| 파일 | 사용 방법 |
| --- | --- |
| [전체 플레이 영상](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26-r2/Mongscape-Gameplay-Full.mp4) | 2분 6초, 720p/30fps, 게임 소리 포함 |
| [Windows 게임](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26-r2/Mongscape-RemoteClient-Windows-x64.zip) | 전체 압축 해제 후 StartGame.cmd 실행 |
| [PostgreSQL Docker 서버](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26-r2/Mongscape-Identity-PostgreSQL-Docker.zip) | 중앙 얼굴 API·PostgreSQL 17·Caddy HTTPS 배포 파일 |
| [기획서 V13](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26-r2/Mongscape-Planning-V13.zip) | 화면별 기획 문서와 당시 검수 기록 |
| [연결 안내](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26-r2/Deployment-Guide-KO.md) | 서버 배포와 게임 PC 주소 지정 |
| [SHA-256](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26-r2/SHA256SUMS.txt) | 다운로드 검증값 |

영상은 자동 키보드 입력으로 타이틀부터 결과 화면까지 한 번에 녹화했습니다. 순간이동·무적·구간 편집 없이 일반 동작 명령과 실제 충돌을 사용하며, 바나나 두 번 접촉 뒤 경찰에게 잡혀 끝납니다. 키보드이므로 운동 칼로리는 0.0/미집계입니다. 웹캠·마이크·개인 얼굴과 기존 저장 데이터는 녹화하지 않았습니다. 30fps는 녹화 형식이며 모든 PC의 성능 보장은 아닙니다.

게임 PC에는 Unity·Python·Docker 설치가 필요 없습니다. 얼굴 없이 키보드/몸동작으로 플레이할 수 있습니다. 얼굴 기능은 별도 서버 배포 후 Game/identity-server.json에 실제 HTTPS 주소를 지정하세요. PC 코인·상점·거리 랭킹은 로컬 저장입니다.

공개 소스 전체 검사 EditMode 129/129, PlayMode 41/41 통과, Windows 빌드·연속 녹화·MP4 전체 디코딩 종료 코드 0. 최신 검사와 영상의 정확한 범위는 [녹화·출시 검수](https://github.com/kihwan04/PrisonRunner/blob/v2026.10.26-r2/PrisonRunner/docs/reports/GAMEPLAY_RECORDING_2026_10_26.md)를 확인하세요. 앞선 서버 검사 PostgreSQL 18개·SQLite 9개는 당시 결과입니다. Docker 기동·외부 HTTPS·여러 PC 연결·실물 웹캠/휴대폰은 미검증입니다. 기존 자동 메뉴 캡처 종료 네이티브 오류와 Unity 종료 메모리 진단은 남아 있습니다.

개인 DB·얼굴·토큰·비밀번호·설치별 설정과 Mixamo 원본은 공개 소스/ZIP에서 제외합니다. 공개 소스는 자체 Blender 동작으로 연결하며 Windows 게임은 기존 실행용 애니메이션을 포함합니다. [리소스 조건](https://github.com/kihwan04/PrisonRunner/blob/v2026.10.26-r2/PrisonRunner/THIRD_PARTY_NOTICES.md)을 따르며 프로젝트 전체에 새로운 오픈소스 라이선스를 부여한 배포가 아닙니다. 이전 릴리스와 기획 기록은 보존합니다.

2026-10-29 게시: 기존 develop의 Tailscale 배포 설정과 문서 정리를 보존합니다. 녹화일은 2026-10-26이며 PR #20의 신규 기능은 포함하지 않습니다.

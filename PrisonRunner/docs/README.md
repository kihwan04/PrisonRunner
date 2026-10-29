# 문서 목록

현재 기준: **2026-10-29**, develop 소스 통합 기준. 현재 실행·개발 안내는 아래 문서를 사용합니다.

| 목적 | 문서 |
| --- | --- |
| PR #20 통합·검수 | [로컬 통합 보고서](reports/PR20_LOCAL_MERGE_2026_10_29.md) |
| PR 원래 계획 | [T1~T15 기능 계획](FEATURE_PLAN_2026_10_28.md) |
| 게임을 바로 실행 | [처음 읽기](../START_HERE.md) |
| 사용 기술과 설치·빌드 환경 | [기술 스택](TECH_STACK.md) |
| 현재 완성 범위·테스트·남은 일 | [진행 상태](PROGRESS.md) |
| 게임·몸동작·얼굴 서버의 연결 | [아키텍처](ARCHITECTURE.md) |
| 다음 개발 순서 | [개발 계획](DEVELOPMENT_PLAN.md) |
| 로그인·프로필·상점·회원/PC 랭킹 | [게임 UI](GAME_FLOW_UI.md) |
| 전신·상체·키보드 조작과 보정 | [몸동작 인식](BODY_TRACKING.md) |
| 여러 PC용 Docker·HTTPS 서버 | [서버 배포 안내](../Services/Identity/README_DOCKER.md) |
| 설정 화면을 수정 | [설정 UI](SETTINGS_UI.md) |
| 화질·성능 측정 | [성능 안내](PERFORMANCE.md) |
| 아트 작업 기준 | [아트 방향](ART_DIRECTION.md) |
| 리소스 후보·교체 지점 | [에셋 목록](ASSET_CATALOG.md) |
| 사용 리소스와 공개 조건 | [외부 리소스 안내](../THIRD_PARTY_NOTICES.md), [무료 에셋 출처](FREE_ASSETS_IMPORTED.json) |
| 브랜치·커밋 규칙 | [Git 작업 흐름](GIT_WORKFLOW.md), [작업 규칙](../AGENTS.md) |
| 공개 데모·다운로드 안내 | [릴리스 안내](PUBLIC_RELEASE.md) |

## 기획서와 검증 근거

| 자료 | 범위 |
| --- | --- |
| [화면별 게임기획서 V13](무한옥_화면별_게임기획서_V13.docx) | 최신 화면별 기획 문서. 2026-10-05 작성, 이후 구현과 차이가 있을 수 있음 |
| [V13 문서 검수](SCREEN_PLAN_V13_QA.md) | 페이지·사진·설명도 검수 기록 |
| [2026-10-25 게임 빌드 보고서](reports/LOCAL_COMPLETION_2026_10_25.md) | 갈림길·바닥·난이도·경찰 연출과 당시 Unity 테스트 |
| [짧은 철로 검수](reports/FORK_SHORT_2026_10_26.md) | 48m 세 갈래·맵 진입 단축·Unity 152개 테스트와 실행 확인 |
| [HUD 정리·성능 보고서](reports/HUD_POLISH_2026_10_26.md) | 최신 HUD 간소화, Unity 150개 테스트, 전후 맵 성능과 화면 |
| [UI 저장소 통합 보고서](reports/MUHANOK_TEST_MERGE_2026_10_26.md) | lilohdong/muhanok-test main b2215ef와 기존 게임의 로컬 통합 |
| [PostgreSQL 중앙 서버 보고서](reports/POSTGRES_IDENTITY_2026_10_26.md) | 2026-10-26 당시 PostgreSQL 18개·SQLite 9개 기록. 현재는 PostgreSQL 전용 |
| [초기 중앙 서버 준비 보고서](reports/DOCKER_IDENTITY_2026_10_26.md) | PostgreSQL 전환 전 서버 주소 설정과 Windows 빌드 검증 |

2026-10-27에 대체된 문서·V10~V12 기획서·이전 버전 캡처를 삭제했습니다. 2026-10-28에는 검수 캡처 폴더 `docs/previews/`도 삭제했습니다(이후 캡처 도구가 만드는 파일은 커밋하지 않음). 필요하면 Git 기록에서 복원합니다. 이전 안내의 Python PoseServer·UDP·WebSocket은 현재 몸동작 처리 방식이 아닙니다.

현재 통합 소스·문서는 develop 병합 대상입니다. 기존 공개 릴리스는 통합 이전 버전이며 새 릴리스·서버 배포는 하지 않습니다. `reports/`의 업로드 보류·로컬 전달 경로는 각 기록을 작성한 시점의 상태이며 현재 게시 상태와 구분합니다.

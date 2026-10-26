# muhanok-test 로컬 통합 결과

기준일: 2026-10-26. [lilohdong/muhanok-test](https://github.com/lilohdong/muhanok-test)의 기본 main 브랜치를 가져와 현재 Mongscape 소스와 통합했습니다. 새 Git 커밋·push·저장소 공개 설정 변경은 하지 않았습니다.

## 통합 기준과 보존

- 가져온 커밋: `b2215ef589e028c2a7efce6af814ac7f02e2ce55` (main). develop/ui의 변경은 이 main에 포함돼 있습니다.
- 공통 기준: `6e50060444aaa86221bd761af9c22134a3529d51`. 기존 작업 소스와 차이를 비교하고 충돌 부분을 수동 조합했습니다.
- 작업 전 소스 4,682개 파일을 루트 `Distribution/IntegrationMuhanokTest/Before/WorkingSource.zip`에 백업했습니다. manifest, staged/unstaged patch와 기존 index/HEAD/MERGE_HEAD도 같은 곳에 보관했습니다.
- 이 체크아웃에 이미 있던 병합 진행 상태와 스테이징은 그대로 두었습니다. 원래 Git index와 HEAD/MERGE_HEAD가 바뀌지 않았는지 확인했습니다. 이번 작업은 소스 내용 통합이며 Git 병합 커밋 완료를 뜻하지 않습니다.
- 상대 저장소의 문서 삭제는 적용하지 않았습니다. 정리해 둔 현재 안내와 과거 기획서 보관함을 유지했습니다. 기존 얼굴 서버 및 포즈 추론 파일은 백업 해시와 비교해 보존을 확인했습니다.

## 최종 기능

새 메인 메뉴·PC 프로필·코인 상점·거리 랭킹(전체/오늘)·개발자/전시 모드·구간 바·Black Han Sans 글꼴을 적용했습니다. 기존 얼굴 로그인/등록/삭제, 캐릭터 얼굴 표시, 칼로리 목표·동작별 기록·콤보, 손 가리키기 선택, 확장 수레 갈림길·경찰 연출과 중앙 얼굴 서버 연결을 함께 유지했습니다.

얼굴 로그인은 메인 메뉴와 준비 화면에서 열 수 있습니다. 얼굴 로그인 중 표시 이름은 서버 프로필을 사용하고 PC의 기본 이름은 덮어쓰지 않습니다. 일시정지에서 메인으로 나가면 기록을 저장하지 않고, 정상 결과는 기존 서버 개인 기록 저장 경로에 연결합니다. 다음 플레이어 전환은 얼굴 로그아웃과 캐릭터 사진 해제를 포함합니다.

PC 저장은 `PlayerProgressStore`의 `Player/profile.json`, `wallet.json`, `records.json`으로 통합했습니다. 기존 Exhibition 파일이 있고 새 파일이 없으면 코인·해금·기록을 처음 한 번 읽어 이전합니다. 원본은 보존하고 존재하지 않던 칼로리를 만들지 않습니다. PC 지갑·거리 랭킹은 중앙 DB로 자동 동기화하지 않습니다. 자세한 흐름은 [게임 UI 안내](../GAME_FLOW_UI.md)에 있습니다.

## 실제 수행한 검증

| 검증 | 결과 |
| --- | --- |
| C# 11개 어셈블리 컴파일 | 오류 0개. 기존 API 사용/이름 숨김 경고는 남음 |
| Unity 6000.3.10f1 EditMode | 99/99 통과 |
| Unity 6000.3.10f1 PlayMode | 36/36 통과 |
| Windows x64 개발 빌드 | 성공, 종료 코드 0 |
| 실제 EXE UI 검수 | 14개 PNG 생성, `MUHANOK_UI_CAPTURE_PASS` 확인. 메뉴·프로필·상점·준비·얼굴·HUD·결과 화면 육안 확인 |
| 실제 EXE 스모크 | `MUHANOK_RUNTIME_SMOKE_PASS`: 로딩·바닥 미끄러짐·120청크 풀링·단일 충돌·결과·재시작, 종료 코드 0 |
| 추가 회귀 사례 | 구형 저장 이전과 재실행, 랭킹 500개 제한, 칼로리 목표 복사, 메뉴/얼굴/준비 연결, 일시정지 종료 무기록, 얼굴 이름/PC 이름 분리 |

Unity 테스트·빌드 로그·XML·화면·스모크 로그는 프로젝트 `Builds/QA/MuhanokTestMerge/`에 있습니다. `Tools/QA/verify_unity_merge.ps1`로 EditMode/PlayMode/Build를 각각 재실행할 수 있습니다. 자동 검수는 별도 임시 플레이어 데이터와 합성 프로필을 사용했습니다.

같은 날 앞선 PostgreSQL 작업에서 실제 PostgreSQL 17.11 18개, SQLite 9개 테스트를 통과했습니다. 이번 UI 통합에서 서버 저장·SQL 코드를 변경하지 않아 이 결과는 앞선 서버 검증으로 구분합니다. [서버 보고서](POSTGRES_IDENTITY_2026_10_26.md).

## 전달과 남은 범위

최신 게임은 프로젝트 `Builds/Mongscape/Windows/Mongscape.exe`, 실행 바로가기는 루트 `StartGame.cmd`입니다. 전달 ZIP과 SHA-256은 루트 `Deliverables/2026-10-26/MergedMuhanokTest/`에 있습니다. Windows ZIP에는 글꼴 OFL 원문도 포함합니다. 실제 얼굴 DB·토큰·비밀번호·설치별 서버 주소를 배포 ZIP에 넣지 않습니다.

실제 Docker 컨테이너·외부 HTTPS·다른 PC와의 접속·본인 얼굴·실물 웹캠 몸동작은 이번 자동 검수 범위에 포함하지 않습니다. 개발 빌드 종료 때 Unity/추론 패키지의 메모리 정리 진단이 남으며, 게임 스모크는 정상 종료했습니다. 서버와 도메인 준비 후 [Docker 안내](../../Services/Identity/README_DOCKER.md)에 따라 배포하고 실물 장비에서 확인해야 합니다.

# 무료 아트 개선 검증 (2026-10-03)

> 이전 V2 검증 기록. 최신 결과는 [ART_V3_QA](ART_V3_QA.md)와 `previews/ArtV3`를 따른다.

## 생성·수정 파일

- 임포트: `Unity/Assets/External/Staging/Kenney`의 CC0 FBX 403개 및 텍스처·라이선스.
- 아트: `Content/ArtV2`의 메시·URP 재질·텍스처·1인칭 손·Volume, 기존 ID 캐릭터/소품/장애물/13개 맵 프리팹, 16개 animation clip, `GameScene.unity`.
- 도구: `MuhanokArtUpgrade*.cs`, `MuhanokArtGeometry.cs`, `MuhanokArtReview.cs`, `RuntimeVisualReview.cs`, 무료 임포트/갤러리 Python 도구.
- 런타임: `FirstPersonHands`, `WorldAtmosphere`, `CoinController`, 기존 Bootstrap/HUD/ChunkSpawner/MuhanokMapChunk/ScoreSystem/Intro 수정.
- 문서: `FREE_ART_V2`, `FREE_ASSETS_IMPORTED.json`, README/ASSET_CATALOG/PROGRESS/QA_REPORT 및 실제 화면.
- 원래 청크 백업: `References/BlockoutBackup`. 기존 다른 시연 씬 유지.

## 구현

- 광산 3종→광산/시설 전환→감방 복도 2종→검문소→계단→세탁실→주방→정비실→운동장→외벽. 이후 시설 경로 반복.
- 같은 24m 길이/바닥/레인 규격. 포털/캐노피로 경계 구성, 조명/안개 보간.
- 광산 레일/침목/볼트/랜턴/광석/암석/작업 소품, 실내 설비/감방 철창, 야외 울타리/감시탑.
- 얼굴·의상·손가락·꼬리·경찰 소품 및 자체 animation 개선. 표정의 scale 축 초기값 문제 수정.
- 인트로 가까운 얼굴 카메라, 1인칭 손, 코인 +10점과 재사용 초기화, RGBA 타이틀 로고.
- 발광 재질 EmissiveIsBlack 수정, 환경광/그림자 조정, 세탁기 방향 수정, 중앙을 막던 검문소 문을 열린 프레임으로 변경.
- 자체 환경 메시를 재질별로 결합. 실제 FPS 목표 달성은 별도 측정 필요.

## 검증 결과

| 검증 | 결과 | 근거 |
|---|---|---|
| Unity EditMode | 18/18 PASS | `Unity/Logs/MuhanokEditModeArtV2.xml` |
| Unity PlayMode | 5/5 PASS | `Unity/Logs/MuhanokPlayModeArtV2.xml` |
| Python motion classifier | 4/4 PASS | PoseServer 폴더에서 unittest 실행 |
| 프리팹/씬/Timeline 참조 | 31개 프리팹 PASS | `MuhanokArtV2.log` |
| GPU 아트 촬영 | 30장 PASS | 4개 흐름 + 13개 맵 + 13개 경계, `MuhanokArtReview.log` |
| Windows 64bit 개발 빌드 | PASS, 약 182 MiB | `Unity/Logs/MuhanokArtBuild.log`, `Builds/Windows/Muhanok.exe` |
| 실제 실행 파일 smoke | PASS / exit 0 | `Unity/Logs/MuhanokRuntimeArtV2.log` |
| 실제 플레이어 GPU 화면 | 4장 시각 검수 PASS / exit 0 | `MuhanokPlayerCapture.log`, `Player_*.png` |
| Unity 즉시 테스트 | Play 진입 확인 | `MuhanokInteractivePlay.log`의 `MUHANOK_EDITOR_READY` |

플레이어 캡처는 실제 창을 표시해 촬영했다. 숨긴 창에서는 이미지가 검게 나왔으므로 실패한 이미지를 최종 검수 결과에 사용하지 않았다. 최종 PNG는 실제 로고/HUD, 발광 재질·Bloom 및 1인칭 손을 확인했다. Development Build 표시는 개발 빌드 표식이다. 전체 화면 갤러리: `docs/previews/ArtV2/index.html`.

PlayMode는 실제 InputSystem 클릭/키 입력, 인트로/FOV80, 120청크 경계·고정 풀 수·안전 패턴, 결과/재시작, UDP 연결 복구, 미지정 캐릭터 fallback을 검증했다. 추가 코인 테스트는 큰 프레임 이동 획득, 1회만 가산, 다른 레인 미획득, 풀 재활성화 및 Retry 초기화를 확인했다.

컴파일 오류를 수정하고 재실행했다. 최신 테스트/아트 생성 로그에는 C# compile error와 runtime exception이 없다. Unity 내장 `com.unity.modules.screencapture` 1.0.0은 실제 플레이어 화면의 로고/HUD까지 캡처하기 위해 추가했다. 기존 패키지 버전 업그레이드는 하지 않았다.

## 남은 문제

- 참고 이미지의 완성도에 아직 미치지 않는다. 캐릭터 rig/표정/손 형태, 광산 암벽과 재질 밀도, 음향/연출 다듬기가 필요하다.
- Asset Store ‘Mine’은 계정 로그인 대기. 미다운로드·미임포트이며 Kenney 에셋과 혼동하지 않는다.
- 실제 웹캠 동작 인식 정확도, 소리 체감과 지속적인 FPS 측정은 수동 검증 필요.
- 일부 URP shadow atlas 경고가 발생할 수 있다. 다수 광원에 추가 성능 조정 여지가 있다.

다음 추천 작업: 무료 rigged 캐릭터 검수, 암벽·소품 디테일 강화, 실제 플레이에서 구역별 가독성과 성능 조정.

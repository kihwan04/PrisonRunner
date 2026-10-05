# 화면 비율·연속 경로 검증 — 2026-10-03

구현과 에셋 출처는 [ART_V3](ART_V3.md), 실제 렌더링은 [화면 갤러리](previews/ArtV3/index.html).

| 검증 | 결과 | 기록 |
|---|---|---|
| Unity EditMode | **24/24 PASS** | `Unity/Logs/MuhanokEditModeArtV3.xml` |
| Unity PlayMode | **6/6 PASS** | `Unity/Logs/MuhanokPlayModeArtV3.xml` |
| Python motion classifier | **4/4 PASS** | `PoseServer`에서 unittest 실행 |
| 모델/아트 생성·직렬화 | **31 prefabs PASS** | `Unity/Logs/MuhanokArtV3Polish.log` |
| 에디터 GameView Fit | **PASS**, 1280×720 / 창에 맞는 scale | `Unity/Logs/MuhanokArtReviewV3.log` |
| GPU 맵/경계 검수 | **30장 PASS** | 위 로그, 카메라 렌더 4 흐름 + 13 맵 + 13 경계 |
| Windows x64 개발 빌드 | **PASS**, 약 210 MiB | `Unity/Logs/MuhanokBuildArtV3.log` |
| 실제 실행 파일 smoke | **PASS / exit 0** | `Unity/Logs/MuhanokRuntimeArtV3.log` |
| 실제 플레이어 전체 화면 | **30장 PASS / exit 0** | `Unity/Logs/MuhanokPlayerCaptureV3.log`, `Player_*_1280x720.png` |
| 실제 와이드 창 1702×726 | **4장 PASS / exit 0**, 좌우 여백·전체 UI 확인 | `Unity/Logs/MuhanokCaptureWideV3.log` |
| 실제 4:3 창 1024×768 | **4장 PASS / exit 0**, 상하 여백·전체 UI 확인 | `Unity/Logs/MuhanokCaptureFourThreeV3.log` |
| PNG 비어 있음·여백 픽셀 검사 | **38장 PASS** | `previews/ArtV3/capture-verification.json` |

EditMode는 16:9·와이드·4:3·세로 비율과 비대칭 안전 영역, 120개 경로의 X/Y 연속성 및 연결부 기울기를 검증했다. 첫 실행에서 미세한 부동소수점 경계 오차 두 건을 발견해 제한 범위를 보정한 뒤 24개 전부 통과했다.

PlayMode는 실제 InputSystem, 인트로→1인칭, 120개 청크의 X/Y/Z 전체 소켓 연결과 일정한 풀 개수, 곡선·상승/고도 유지/하강 경로의 실제 코인 획득과 중복 방지, 장애물 안전 패턴, UDP 재연결, 결과→Retry를 검증했다. 6개 전부 통과했다.

실행 파일 smoke는 실제 8초 Timeline, FOV80, 120청크 풀 재사용, Caught→Result→Retry를 확인했다. 개발용 맵 촬영은 명시적인 `--muhanok-capture --muhanok-mapcapture` 옵션에서만 실행한다. 이 검수 모드에서는 장애물 평가를 생략하여 13개 맵을 촬영한다. 정상 플레이에는 적용하지 않는다.

실제 창은 화면이 보이도록 열어 ScreenCapture로 HUD까지 촬영했다. 숨긴 창의 검은 이미지를 사용하지 않았다. 처음 카메라 캡처에서 아치 크기·손 방향·야외 울타리 높이를 발견해 수정한 뒤 재촬영했다. 갤러리에서는 실제 플레이어 화면을 우선 표시한다.

마지막 HUD 검수에서 거리 라벨과 숫자의 칸을 분리해 단위 잘림을 수정하고 타이틀 안내를 가운데 정렬했다. 이후 다시 빌드·실제 화면 촬영을 수행했다. 실행 중인 Unity Play 및 GameView 1280×720 / scale 1 적용은 `Unity/Logs/MuhanokInteractiveV3.log`의 `MUHANOK_EDITOR_READY`, `MUHANOK_GAME_VIEW_FIT`로 확인했다.

최신 생성·테스트·빌드·플레이어 로그에 C# 컴파일 오류와 런타임 예외가 없다. 개발 빌드 표시는 정상 표식이며, POSE Lost는 실제 웹캠 송신이 연결되지 않은 상태다. 키보드 입력은 유지한다.

Blender 공식 SHA256 검증 및 7개 FBX 내보내기 성공: `Unity/Logs/BlenderModules.log`의 `BLENDER_AUTHORING_PASS`. 생성된 모델은 자체 제작 모델로 Asset Store의 캐릭터나 rig로 표현하지 않는다.

남은 문제: 캐릭터 스키닝·표정과 일부 재질·연출은 참고 이미지에 미치지 않는다. 동물은 정적이며 분기 선택·차량 물리는 아직 없다. Asset Store Mine은 로그인 확인 후 약관 승인 대기로 미다운로드·미임포트다. 실제 웹캠 인식 정확도와 장시간 성능은 수동 검수 필요.

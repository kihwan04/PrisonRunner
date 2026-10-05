# 무한옥 검증 기록 — 2026-10-03

최신 무료 아트 개선 버전 검증: [ART_V2_QA](ART_V2_QA.md). 아래는 이전 blockout 버전의 검증 기록이다.

## 검증 환경

- Windows / Unity 6000.3.10f1 / URP 17.0.1 / Cinemachine 3.1.7 / Input System 1.18.0.
- 기존 설치 패키지 버전 유지. 필수 Timeline 1.8.10 및 animation/audio/director/particlesystem 내장 모듈만 활성화. 이유는 IMPLEMENTATION_PLAN에 기록.
- Python 3.12.14 / MediaPipe 0.10.35 / OpenCV contrib 4.14.0.94. 정확한 의존성은 PoseServer/requirements-lock.txt.

## 통과 결과

| 검증 | 결과 / 범위 | 원본 결과 |
|---|---|---|
| Unity EditMode | 17/17. 순수 규칙, 상태 전이, 안전 패턴, 입력 경로, pose 패킷 검증/상태 | Unity/Logs/MuhanokEditMode.xml |
| Unity PlayMode | 4/4. 실제 씬, Input System 가상 클릭/키보드, intro/takeover, 모든 action, 120청크, 결과/반복 retry, UDP 연결/재연결, 캐릭터 누락 fallback | Unity/Logs/MuhanokPlayMode.xml |
| 직렬화 참조 | 30 prefab / GameScene / Timeline PASS | Unity/Logs/MuhanokWindowsBuild.log |
| Windows 빌드 | StandaloneWindows64 development build 성공. Build report 167,977,525 bytes | Builds/Windows/Muhanok.exe |
| 실행 파일 smoke | exit 0. title, 정상 8초 intro, 1인칭 FOV 80, 120청크 재사용, 잡힘/결과/재시작 | Unity/Logs/MuhanokRuntime.log |
| 기존 감옥 시연 회귀 | exit 0. 기존 CellBlockVerticalSlice 실행, 7 active/total 청크와 11 장애물 pool 안정성 PASS | Unity/Logs/MuhanokLegacySmoke.log |
| Python 단위 테스트 | 4/4. 동작 분류, confidence, 분실/재보정, 비정상 좌표 | PoseServer/tests/test_motion_detector.py |
| Python 런타임 | pip check / py_compile / 실제 UDP datagram / 공식 Lite 모델 CPU 추론 성공 | PoseServer/README.md의 재현 명령 |
| 화면 검수 | title stage / idea / takeover / prison first-person camera 렌더 검수 | docs/previews/Muhanok_*.png |

로그와 Windows 빌드는 생성물이라 Git에서 제외한다. 프로젝트 메뉴 `Muhanok/Build Windows Demo`, Unity Test Runner, Python README 명령으로 재현 가능하다. 로그의 Unity 종료 시 allocator 진단 출력은 장시간 메모리 안정성을 증명하지 않는다.

## 남은 수동 검증

- 실제 웹캠 전신 입력, 사용자별 보정/threshold, 장치 탈착 후 복구.
- 실제 창에서 한글 HUD/터치 조작/음량/카메라 멀미 및 장시간 플레이.
- 시안 수준의 최종 원숭이/경찰 rig, 죄수복, 애니메이션, 배경/재질 교체. 현재 primitive blockout이며 상용 패키지는 구매/임포트하지 않았다.
- 프리뷰는 camera 렌더 캡처라 IMGUI HUD가 포함되지 않는다. headless smoke는 화면 표시나 음질을 검증하지 않는다.

에셋 후보와 교체 규칙은 ASSET_CATALOG.md를 따른다. 정상 keyboard gameplay는 webcam 연결 없이 사용할 수 있다.

## 최신 화면 비율·연속 경로/Blender 검증
화면 비율, 실제 높이·곡선·코인, 야간 사육장 변경의 최신 결과는 [ART_V3_QA](ART_V3_QA.md)를 따른다. EditMode 24/24, PlayMode 6/6, Python 4/4 통과. Windows 빌드·runtime smoke 및 실제 1280×720/1702×726/1024×768 화면 검수 통과. Asset Store 패키지는 아직 미임포트이며 캐릭터 품질 차이와 실제 웹캠/장시간 성능 검수는 남아 있다.

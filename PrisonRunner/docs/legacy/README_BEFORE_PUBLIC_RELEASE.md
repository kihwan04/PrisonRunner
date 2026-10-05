# PrisonRunner

## 맵별 점프·슬라이드와 가속 (2026-10-05)

모든 도보 맵에 점프·슬라이드 구간을 추가했다. 계단의 하이니와 입구 문 아래 슬라이드, 수레의 좌우 기울이기는 유지한다. Space는 점프, S/↓는 슬라이드다. 기본 9m/s에서 이동 거리 120m마다 1m/s씩 올라 최대 17m/s에 도달하며, 현재 속도는 HUD에 표시된다. [변경 내용과 검증](docs/MAP_EXERCISES_SPEED.md). 운동 장애물만 재생성하려면 `Muhanok/Apply Map Exercises and Progressive Speed`를 사용한다.

## 무한옥 실행 (2026-10-04)

최신 작업은 [ART_V9](docs/ART_V9.md), 검수 [ART_V9_QA](docs/ART_V9_QA.md), [실제 게임 화면](docs/previews/ArtV9/index.html). 전용 단선 절벽 카트, 무너진 철로의 반대쪽 기울이기, 점프·숙이기·하이니 운동 장애물, 바나나 누적 두 번 후 추격을 적용했다. 광산에서 계단을 올라 내려오는 문 아래로 슬라이딩하면 바로 감옥 복도로 이어지고, 문은 뒤에서 바닥까지 닫힌다. 맵 전체 재생성 메뉴는 `Muhanok → Upgrade Dedicated Cart Tracks and Exercise V9`. 무료 경찰은 아직 미임포트이며 현재 경찰은 자체 모델이다. 참고 그림/서브웨이 서퍼스급 전체 아트 품질에는 차이가 남아 있다.

Unity Hub에서 `Unity/`를 **6000.3.10f1**로 열고 `Assets/Game/Scenes/GameScene.unity`에서 Play한다. `게임 시작`을 누르면 선택 몸무게 입력 화면이 열린다. 입력하거나 `입력 없이 시작`을 누르면 제공된 로딩 이미지를 인트로로 사용하고 0.85초 전환 후 바로 1인칭 플레이로 이어진다.

빠른 Unity 테스트: 상단 `Muhanok → Play GameScene`을 누르면 씬을 열고 Play 모드로 진입한다. 저장하지 않은 다른 씬이 있으면 먼저 Unity의 저장 확인 창이 표시된다. 종료는 상단 Play 버튼을 다시 누른다.

`PlayInUnity.cmd`를 더블클릭해도 된다. Unity가 닫혀 있으면 지정 버전으로 열고, 이미 열려 있으면 현재 플레이를 종료한 뒤 GameScene 테스트를 요청한다.
이미 열린 Unity가 백그라운드에 있어 변경된 스크립트를 아직 가져오지 않았다면 Unity 창을 한 번 활성화한다. 새 스크립트 컴파일이 끝나면 대기 중인 실행 요청을 처리한다.

Unity 없이 실행하려면 `Builds/Windows/Muhanok.exe`를 연다. 실행 파일과 `Muhanok_Data`, `UnityPlayer.dll`, `MonoBleedingEdge` 등 옆의 파일/폴더를 함께 유지한다. 현재 개발 빌드는 Windows 64비트, 1280×720 창 모드다. 몸동작 입력은 `PoseServer/StartPose.cmd`를 함께 실행한다.

- A/D 또는 방향키: 3레인 이동 (간격 2.2m, 0.20초)
- Space: 점프 / S 또는 ↓: 숙이기 / W 또는 ↑: 하이니
- 고체 장애물 한 번 충돌이면 즉시 이동 정지/게임오버. 바나나·물웅덩이는 2.4초간 50% 감속. 한 플레이에서 바나나 두 번을 밟으면 감속 때문에 1.2초 뒤 경찰에게 잡힌다. `다시 하기`는 scene reload 없이 타이틀로 복귀한다.
- 수레 구간: A/D 또는 몸을 좌우로 기울여 무너진 레일 반대쪽으로 균형 잡기. 단선 자동 전진/후방 3인칭. 점프/하이니는 받지 않으며 하차 후 1인칭으로 다시 달린다.
- 계단 전반은 W/하이니, 계단 끝 내려오는 문은 S/몸 숙이기 슬라이딩. 문은 통과 뒤 완전히 닫히고 바로 감옥 복도가 이어진다.
- 몸동작 입력은 [PoseServer 실행 안내](PoseServer/README.md). 웹캠이 없어도 키보드 플레이 가능.
- 최신 경로: 광산 → 계단 → 감옥 복도 → 사육장. 문은 바닥까지 닫히며 두 레인은 피할 수 있게 남겨 둔다. 일시정지 버튼으로 진행을 멈춘다.
- 몸무게를 입력한 경우 카메라가 인식한 활동 시간으로 **예상 kcal**를 표시한다. 키보드만 사용한 시간은 제외하며 몸무게를 저장/전송하지 않는다. 실제 대사량 측정값은 아니다.
- 튜닝: `Assets/Game/Content/Data/GameSettings.asset`
- 새 콘텐츠 생성: `Muhanok/Build GameScene` (기존 ID prefab은 보존)
- Windows 빌드: `Muhanok/Build Windows Demo`
- 테스트: Test Runner의 Muhanok EditMode/PlayMode assemblies

최신 아트 V4: Blender 스킨 캐릭터 2개·Humanoid 뼈대·자체 동작 16개·수레·손을 적용했다. ProBuilder 6.1.2로 13개 편집용 경로를 만들고 실제 맵에 베이크한다. 카메라/HUD는 16:9와 안전 영역을 유지하며, `Muhanok/Fit Game View (16:9)`로 Unity 확대율도 조정한다. `Assets/Game/Authoring/ProBuilder`를 편집한 뒤 `Muhanok/Bake Edited ProBuilder Routes Into Game`으로 반영한다. Mixamo 실제 달리기·점프·숙이기·비틀거림 4개를 다운로드해 자체 캐릭터에 리타게팅했다. 참고 이미지와의 최종 아트 차이가 남아 있다.

최신 경로·로딩·칼로리·환경 작업은 [ART_V5](docs/ART_V5.md), 검증은 [ART_V5_QA](docs/ART_V5_QA.md), [실제 화면 갤러리](docs/previews/ArtV5/index.html)에 있다. Blender 원본·ProBuilder·실제 Mixamo 적용 기록 [TOOLS_V4](docs/TOOLS_V4.md), [TOOLS_V4_QA](docs/TOOLS_V4_QA.md)와 이전 검수 기록/시연 씬은 보존한다. 아래는 기존 MVP 안내다.

Unity 기반 3레인 감옥 테마 Endless Runner 캡스톤 프로젝트입니다. 키보드 조작을 먼저 완성하고, Python MediaPipe Pose에서 인식한 동작을 WebSocket으로 Unity에 전달합니다.

## 기존 MVP 상태 기록 (현재 무한옥 구현 이전)

이전 MVP는 Phase 1 러너 조작과 Phase 2 무한 맵을 포함한 키보드 플레이 장면이었다. 현재 PoseServer와 GameScene은 구현되어 있으며, 최종 아트 교체는 남아 있다.

## 실행

1. Unity Hub에서 `Unity/` 폴더를 Unity 6000.3.10f1로 엽니다.
2. 패키지 임포트와 스크립트 컴파일이 끝나면 `Assets/Game/Scenes/RunnerMVP.unity`를 엽니다.
3. Play를 누릅니다. Ground, Capsule Player, Cube 장애물은 장면의 `Runner MVP Bootstrap`이 플레이 시작 시 생성합니다.

| 입력 | 동작 |
| --- | --- |
| A / ← | 왼쪽 레인으로 한 칸 이동 |
| D / → | 오른쪽 레인으로 한 칸 이동 |
| Space | 점프 |
| S / ↓ 누르고 있기 | 숙이기 |

전진은 자동입니다. 장애물에 부딪히면 잠시 느려집니다. 점수는 실제 전진 거리(m)입니다. 맵은 세 종류의 Placeholder Chunk를 앞에 생성하고 지나간 Chunk와 장애물을 재사용합니다.

## 구조

- `docs/`: 개발 계획, 아키텍처, 아트 방향
- `References/`: 아트 참조 이미지
- `Unity/`: Unity 프로젝트
- `PoseServer/`: 웹캠 및 자세 인식 서버
- `Tools/AssetPipeline/`: 에셋 준비 도구

구현 순서와 완료 기준은 [개발 계획](docs/DEVELOPMENT_PLAN.md)을 참조하세요. 코드 작성 전 [작업 규칙](AGENTS.md)과 [아키텍처](docs/ARCHITECTURE.md)를 확인하세요.

## Git 작업 흐름

`main`은 발표용 버전, `develop`은 기능 통합 브랜치입니다. 개별 기능은 `feature/*`에서 작업합니다. 현재 구현은 `develop`에 통합하며, 발표 검증 후 `main`에 반영합니다. 자세한 규칙은 [Git 작업 흐름](docs/GIT_WORKFLOW.md)을 참조하세요.

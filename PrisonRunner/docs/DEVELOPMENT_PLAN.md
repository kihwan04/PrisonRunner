# DEVELOPMENT_PLAN.md

## 최종 목표

웹캠 앞에서 사용자가 몸을 움직이면
Unity의 캐릭터가 반응하는
3레인 Stylized Prison Endless Runner를 완성한다.

---

# Phase 1 - Runner MVP

구현:

- Unity 기본 프로젝트
- URP
- 3 Lane
- 자동 진행
- Keyboard Input
- Left / Right
- Jump
- Crouch
- Ground
- Obstacle
- Collision
- Score
- Runner Camera

캐릭터는 Capsule을 사용한다.

완료 기준:

키보드만으로 기본 게임을 플레이할 수 있다.

---

# Phase 2 - Endless Map

구현:

- MapChunk
- EntrySocket
- ExitSocket
- Chunk Spawn
- Chunk Recycling
- Object Pooling
- ObstacleSocket
- Random Pattern

기본 Chunk:

PrisonCorridor
CellBlock
PrisonYard

완료 기준:

맵이 끊기지 않고 계속 생성된다.

---

# Phase 3 - Pose Detection

Python + MediaPipe를 사용한다.

구현:

IDLE
LEFT
RIGHT
JUMP
CROUCH
HIGH_KNEE

추가:

Smoothing
Cooldown
Confidence
Threshold Config

완료 기준:

Unity 없이 Python Console에서
Action이 안정적으로 출력된다.

---

# Phase 4 - Unity Pose 연결

WebSocket으로 연결한다.

PoseServer
↓
PoseInputProvider
↓
IGameInput
↓
Player

Keyboard Mode와 Pose Mode를 모두 유지한다.

완료 기준:

몸동작으로 Capsule을 조작할 수 있다.

---

# Phase 5 - Character

Art Direction을 기준으로 캐릭터를 제작한다.

Concept
↓
Image-to-3D
↓
Retopology
↓
Rig
↓
Animation
↓
Unity Humanoid

필요 Animation:

Idle
Run
Jump
Land
Crouch
HighKnee
Hit
Stumble

완료 기준:

Capsule을 최종 캐릭터로 교체한다.

---

# Phase 6 - Prison Map

Modular Prison Kit를 제작한다.

필수:

Cell Block
Prison Corridor
Prison Yard
Check Point
Maintenance Tunnel
Security Zone
Stair Section

완료 기준:

최소 5종 이상의 MapChunk가 게임에서 반복된다.

---

# Phase 7 - Polish

UI
Sound
VFX
Lighting
Animation Polish
Difficulty
Score
Optimization

---

# Phase 8 - Presentation Build

테스트:

Keyboard Mode
Pose Mode
Webcam Disconnect
Pose Lost
Low FPS
Long Play
Map Recycling
Collision
Score

발표용 Build를 생성한다.

---

# 우선순위

Gameplay
>
Pose Control
>
Endless Map
>
Character
>
Environment
>
Animation Polish
>
VFX / UI

그래픽 때문에 Gameplay 개발을 멈추지 않는다.

# ARCHITECTURE.md

## 목적

Unity 게임 코드와
MediaPipe
WebSocket
외부 API
3D Asset Pipeline을 분리한다.

Clean Architecture를 그대로 적용하지 않고
게임 개발에 필요한 부분만 사용하는
Clean Architecture Lite를 사용한다.

---

## 전체 구조

Player Body
↓
Webcam
↓
MediaPipe Pose
↓
PoseServer
↓
WebSocket
↓
PoseInputProvider
↓
IGameInput
↓
Player Action
↓
Unity Character

---

## Unity 구조

Unity/Assets/Game/

Core/
Application/
Infrastructure/
Presentation/

---

## Core

순수 게임 규칙을 담당한다.

예:

PlayerAction
Lane
Score
ObstacleRule
GameState

가능하면 UnityEngine에 의존하지 않는다.

Core는 다음을 몰라야 한다.

MediaPipe
WebSocket
Higgsfield
Unity UI
Camera

---

## Application

게임의 동작 흐름을 담당한다.

예:

MoveLane
Jump
Crouch
HitObstacle
AddScore
SpawnMapChunk

Core를 사용해서 실제 게임 행동을 수행한다.

---

## Infrastructure

외부 시스템과 연결한다.

예:

KeyboardInputProvider
PoseInputProvider
WebSocketClient
SaveSystem

MediaPipe와 Unity 통신도 이 영역에서 처리한다.

---

## Presentation

Unity 표현을 담당한다.

예:

PlayerView
PlayerAnimator
GameUI
ScoreUI
RunnerCamera
SceneController

MonoBehaviour는 주로 이 영역에서 사용한다.

---

## 입력 구조

KeyboardInputProvider
        ↓
      IGameInput
        ↑
PoseInputProvider

게임은 어떤 입력 장치가 사용되는지 몰라야 한다.

---

## Player 구조

IGameInput
↓
PlayerActionController
↓
PlayerMovement
↓
PlayerView
↓
Animator

PlayerMovement 안에

WebSocket
MediaPipe
Keyboard 입력 코드를 넣지 않는다.

---

## Map 구조

MapGenerator
↓
MapChunkPool
↓
MapChunk
↓
ObstacleSocket
↓
ObstaclePool

각 MapChunk는

EntrySocket
ExitSocket
LeftLane
CenterLane
RightLane

규격을 공유한다.

---

## MapChunk 종류

PrisonCorridor
CellBlock
PrisonYard
CheckPoint
MaintenanceTunnel
SecurityZone
StairSection

---

## PoseServer 구조

PoseServer/

src/
config/
tests/

PoseServer가 담당하는 것:

Webcam
MediaPipe
Landmark 분석
Smoothing
Action 판단
Confidence
WebSocket 전송

Unity 게임 규칙은 PoseServer에 작성하지 않는다.

---

## Pose Action

IDLE
LEFT
RIGHT
JUMP
CROUCH
HIGH_KNEE

전송 예:

```json
{
  "action": "LEFT",
  "confidence": 0.92,
  "timestamp": 123456789
}
```

---

## Art 구조

Unity/Assets/Art/

Characters/
Environment/
Obstacles/
Materials/
Animations/

게임 코드와 Art Asset을 분리한다.

---

## Asset 교체 원칙

Placeholder Capsule
↓
Final Character

Cube Obstacle
↓
Final Obstacle

Gray MapChunk
↓
Final Prison Map

교체 과정에서 게임 로직은 변경하지 않는다.

---

## 설정

속도
Lane 간격
점프 높이
Pose Threshold
난이도
장애물 확률

등은 코드에 흩어놓지 않는다.

Unity 설정은 가능한 ScriptableObject로 관리한다.

Pose 설정은 config 파일로 관리한다.

---

## 핵심 원칙

의존성은 가능한 내부 게임 규칙 방향으로 향한다.

외부 기술을 교체해도
핵심 게임 시스템이 영향을 적게 받는 구조를 만든다.

단

아키텍처를 지키기 위해
불필요한 Interface나 Class를 만들지 않는다.

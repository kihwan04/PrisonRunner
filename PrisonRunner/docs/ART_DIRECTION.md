# ART_DIRECTION.md

## Art Style

스타일 이름:

Stylized Prison Runner

기준 이미지:

References/prison-art-reference.png

---

## 핵심 컨셉

Cute Character
+
Dark Industrial Prison

캐릭터는 귀엽고 둥글다.

환경은 차갑고 묵직하다.

둘의 대비가 게임의 핵심 비주얼이다.

---

## Character

주인공은 Stylized Prisoner다.

특징:

- 큰 머리
- 둥근 몸
- 짧은 다리
- 굵은 팔
- 큰 손
- 큰 신발
- 단순한 얼굴
- 과장된 표정

Orange Prison Jumpsuit를 기본 의상으로 사용한다.

실제 인간 비율보다
게임 캐릭터 비율을 우선한다.

---

## Character Visibility

러너 게임이므로
정면 얼굴보다 실루엣과 뒷모습이 중요하다.

Run
Jump
Crouch
Lane Change

동작이 멀리서도 명확하게 보여야 한다.

---

## Environment

주요 색상:

Dark Navy
Blue Gray
Steel Gray
Cyan

감옥은 현실적이고 무거운 느낌을 주되
Photorealistic 스타일로 만들지 않는다.

---

## Accent Color

Player:
Orange

Danger:
Red

Interactive:
Yellow

Lighting:
Warm Yellow

Background:
Cool Blue

---

## Environment Areas

Prison Entrance

Cell Block

Prison Corridor

Prison Yard

Check Point

Security Zone

Maintenance Tunnel

Stair Section

---

## Modular Kit

Floor
Wall
Corner
Ceiling
CellDoor
MetalBars
Fence
Gate
Stair
WatchTower
Lamp
CCTV
Pipe
Vent
Bench

---

## Obstacle

WoodenCrate
MetalCrate
Barrel
FoodCart
Barrier
Laser
LowPipe
Fence
BrokenFloor
Stair

---

## Gameplay Readability

배경보다 장애물을 명확하게 보이게 한다.

Jump 장애물:
낮고 넓게

Crouch 장애물:
머리 높이에 명확하게

Lane Block:
하나의 레인을 확실하게 막는다.

High Knee:
계단 형태로 표현한다.

---

## Lighting

Unity URP를 사용한다.

환경:
Cool Light

감옥 내부 조명:
Warm Light

위험 구간:
Red Emission

중요 오브젝트:
밝은 강조 표현 허용

---

## Material

Stylized PBR을 사용한다.

Rounded Edge를 선호한다.

작은 디테일보다는
큰 형태와 실루엣을 우선한다.

Texture:

일반 Asset = 1K 권장

Hero Character = 최대 2K

---

## Asset Pipeline

Concept
↓
Reference
↓
AI 3D 또는 기존 Asset
↓
Retopology
↓
Texture
↓
Rig
↓
Unity
↓
Prefab

---

## Tool Direction

Concept:
Higgsfield 또는 Image Generation

3D:
Tripo 또는 Meshy

Mesh 수정:
Blender

Animation:
Auto Rig 또는 Mixamo

Game:
Unity

Code:
C# / Python

---

## 금지

Photorealistic Character

지나치게 현실적인 신체 비율

과도한 Texture Detail

가독성을 해치는 장식

서로 다른 스타일의 Asset 혼용

Gameplay보다 그래픽을 우선하는 작업

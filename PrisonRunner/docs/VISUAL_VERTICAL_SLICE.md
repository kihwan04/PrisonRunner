# Visual Vertical Slice 준비

첫 Cell Block 제작 장면과 확인 방법은 [CELLBLOCK_VERTICAL_SLICE.md](CELLBLOCK_VERTICAL_SLICE.md)를 따른다. 아래는 기존 Prototype의 교체 구조 설명이다.

## 유지하는 동작

입력, Movement, 충돌 반응, 점수, 맵 생성 규칙과 Pooling은 유지한다. 초기 상태에서는 Capsule, Cube, 세 가지 Placeholder Map과 Lane Debug Line이 그대로 표시된다. 최종 3D 에셋은 아직 추가하지 않는다.

## 플레이어

Play 시작 시 `PlayerRoot`에 기존 CharacterController, PlayerMovement, PlayerActionController, DistanceScore를 두고, 자식 `PlayerVisual`에 표시용 모델만 둔다. Movement가 기존 방식으로 PlayerVisual의 높이와 스케일을 조절하므로 숙이기 동작은 유지된다.

`Runner MVP Bootstrap`의 **Player Visual Prefab**을 비워 두면 기존 Capsule을 사용한다. 교체 Prefab은 `PlayerVisual` 아래 생성한다. PlayerVisual은 서 있을 때 지면에서 1m 위에 있고, Placeholder의 중심이 원점이다. 발 Pivot 모델은 Prefab 내부에서 모델을 Y=-1m로 배치한 래퍼를 사용한다. Gameplay Collider는 PlayerRoot에 그대로 둔다. Animator 기반 숙이기는 후속 작업에서 검토한다.

## 맵과 장애물

각 MapChunk는 아래처럼 구성된다.

```text
MapChunk
├─ Gameplay
│  ├─ GroundCollider
│  ├─ EntrySocket / ExitSocket
│  └─ ObstacleSocket → Obstacle(BoxCollider, RunnerObstacle) → Visual(Cube)
└─ VisualRoot
   ├─ PlaceholderVisual
   ├─ PrefabVisual (교체 Prefab 지정 시)
   └─ LaneDebugLines
```

Bootstrap의 **Corridor / Cell Block / Prison Yard Visual Prefab** 슬롯에 종류별 Prefab을 지정한다. 비어 있는 종류는 기존 Placeholder를 유지한다. MapChunk의 `SetVisualPrefab()`은 표시물만 교체하고 GroundCollider, Socket과 장애물을 유지한다. Prefab을 제거하면 Placeholder를 다시 표시한다. 교체 인스턴스는 청크와 함께 풀에서 재사용한다.

Environment Prefab은 청크 EntrySocket을 원점으로, +Z 방향 길이 36m, 레인 X=-3/0/3m에 맞춘다. 바닥 표면은 Y=0m이다. Visual 전용 Prefab은 Renderer/Animator 중심으로 구성하고 Gameplay 스크립트를 포함하지 않는다. Visual에 포함된 Collider와 Rigidbody는 인스턴스 생성 시 비활성화 후 제거하여 기존 충돌에 관여하지 않게 한다.

**Show Lane Debug Lines**는 Edit Mode에서 Bootstrap Inspector로 설정하고, Play Mode에서도 변경할 수 있다. 런타임 MapGenerator에서는 풀에 있는 비활성 청크까지 적용한다. 개별 MapChunk에서도 토글할 수 있으며, 해당 청크가 다시 생성될 때 Generator의 공통 설정을 따른다. 이 토글은 Collider나 Socket을 변경하지 않는다.

장애물은 기존 `Obstacle`의 BoxCollider와 `Visual` Cube 분리를 유지한다. Collider의 크기와 중심은 기존 게임 규칙을 따른다. 장애물 모델 교체 연결은 실제 에셋을 검수하는 후속 작업에서 진행한다.

## 카메라와 후처리

`Runner Camera Rig`가 기존 위치 보간과 LookAt을 담당하고, 자식 `Main Camera`가 표시를 담당한다. `SetPresentationEffects(positionOffset, eulerOffset, fovOffset)`에 향후 Shake/FOV 연출을 연결한다. 현재 효과 값은 모두 0이고 기존 FOV와 시점을 유지한다. `ResetPresentationEffects()`로 기본 상태로 복귀한다.

장면의 `Prison Visual Volume`은 `Assets/Settings/PrisonVisualProfile.asset`을 사용한다. 초기 효과는 Tonemapping(Neutral), Color Adjustments(중립), Bloom(Intensity 0.15), Vignette(Intensity 0.12)만 사용한다. Motion Blur 등 다른 효과는 프로필에서 제거했다. Main Camera의 URP Post Processing과 Default Volume Layer 연결은 유지한다. 최종 아트가 들어온 뒤 이 전용 프로필에서 색감과 효과 강도를 조정한다.

## 에셋 위치와 검증

`Assets/Art/Characters`, `Environment`, `Obstacles`, `Materials`, `Animations`를 사용한다. 빈 폴더도 `.gitkeep`과 Unity `.meta`로 유지한다.

Unity에서 `RunnerMVP`를 열고 기본 키보드 조작, 숙이기와 충돌을 확인한다. Lane Debug Line을 끄고 켜도 바닥과 장애물 충돌이 유지되는지 확인한다. Prefab 슬롯을 비워 둔 상태의 외형과 카메라가 기존 Prototype과 동일해야 한다. 플레이 도중 Hierarchy에서 위 구조를 확인한다.

`PrisonRunner > Run Phase 2 Smoke Check`는 Visual 교체/복원, 충돌 분리, Debug 토글, 숙이기 복원, 카메라 효과 복원, URP 프로필 연결과 기존 맵 재사용을 함께 검사한다. 이 검사에 사용하는 Cube는 실행 중에만 만들고 장면이나 에셋으로 저장하지 않는다.

다음 작업은 검수한 캐릭터와 감옥 청크 Prefab을 슬롯에 연결하고, 크기/Pivot/가독성을 확인하는 것이다.

## 자동 검사 결과

2026-09-28, Unity 6000.3.10f1의 Batch Mode에서 C# 컴파일 오류 없이 Visual 검사와 Phase 2 맵 검사를 통과했다. 최종 검사 시 활성/전체 청크는 7/7개, 장애물은 9개였다. 그래픽 장치를 사용하지 않은 검사이므로 화면 색감과 렌더 결과는 Unity에서 직접 확인해야 한다. 검사 로그에 Burst 해시 캐시 로딩 오류가 함께 기록되었으므로 Console에서도 확인한다.

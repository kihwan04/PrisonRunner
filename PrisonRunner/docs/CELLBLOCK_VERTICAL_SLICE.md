# Cell Block Vertical Slice

## 열기

`Unity/Assets/Game/Scenes/CellBlockVerticalSlice.unity`를 열고 Play한다. 기존 `RunnerMVP` 장면은 유지한다. 이번 장면은 기존 맵 종류 세 슬롯에 동일한 Cell Block Visual Prefab을 연결해 한 구간의 반복성을 확인한다. 맵 선택, 생성, 충돌, 입력, 이동, 점수 규칙은 바꾸지 않는다.

## 모듈과 시각 구조

- `Art/Environment/Blockouts/CellBlock/CellBlockModule_6m.prefab`: 좌우 감방, 철창과 문, 벽/기둥, 바닥/천장 보, 배관, CCTV, 침대/벤치, 감방 조명 위치. ProBuilder Mesh로 편집한다.
- `CellBlockVisual_36m.prefab`: 6m 모듈 여섯 개, 열린 게이트, 간판, 천장 조명과 약한 붉은 강조 조명. Entry 원점 Z=0, Exit Z=36, 바닥 Y=0.
- 장면의 `Entry Visual Buffer`는 시작 카메라 뒤쪽을 덮는 6m 시각 모듈 하나다. Collider나 맵 생성 규칙을 추가하지 않는다.
- 중앙 통로 폭은 기존 11m, 레인 중심은 X=-3/0/3m다. 낮은 장식은 통로 바깥에, 천장/게이트는 점프 높이 위에 둔다. Visual Prefab에는 Collider를 넣지 않는다.
- `MapChunk/Gameplay`는 기존 바닥 Collider와 장애물 Socket을 유지하고, 청크 아트는 `MapChunk/VisualRoot/PrefabVisual`에만 연결한다.
- `PlayerRoot`의 기존 Movement/Collider를 유지한다. 주황 Capsule은 `PlayerVisual`에 그대로 둔다.
- `Art/Obstacles/CellBlock`의 Metal Crate / Barricade / Low Pipe는 기존 장애물의 `Visual` 자식에 연결한다. 원래 Cube와 Renderer는 풀의 크기 계약을 위해 유지하고 표시만 끈다. BoxCollider 크기와 중심은 바꾸지 않는다.

## 색상, 조명, 카메라

`Art/Materials/CellBlock`에 URP/Lit 기본 Material 일곱 개를 둔다: Dark Navy, Blue Gray, Dark Steel, Player Orange, Warning Red, Warm Yellow, Cool Blue. 별도 Shader와 Texture는 만들지 않는다.

장면은 차가운 방향광/환경광, 청크당 따뜻한 천장 Point Light 세 개와 약한 Red Accent 한 개를 사용한다. 감방 램프 위치는 약한 Emission Mesh로 표시한다. Point Light 그림자는 끈다. Linear Fog는 32–95m에 적용하고 카메라 배경색도 같은 색으로 맞춰 청크 끝의 Skybox가 보이지 않게 한다. 기존 `PrisonVisualProfile`의 Tonemapping/중립 Color Adjustments/Bloom 0.15/Vignette 0.12는 유지한다.

`Settings/CameraPresets/CellBlockRunnerCamera.prefab`의 Cinemachine Camera가 기존 PlayerRoot를 Follow/LookAt한다. World Space Follow Offset=(0,3.15,-7.8), FOV=64, 레인/점프 damping=(0.9,0.45,0.08), 전방 Aim Offset=(0,1.15,7)이다. `RunnerCameraEffects` 확장에 Shake/FOV 연결 지점을 두고 초기 효과는 0으로 유지한다. Bootstrap의 Camera Prefab 슬롯을 비우면 기존 카메라를 사용한다.

## 확인과 재생성

1. 세 레인 전환/점프/숙이기/충돌/점수와 청크 이음새를 확인한다.
2. 철창·문·CCTV·게이트와 배관이 플레이 공간을 가리지 않는지, 주황 Capsule과 붉은 장애물이 잘 보이는지 확인한다.
3. Bootstrap의 **Show Lane Debug Lines**를 Play 중 켜고 끈다. 기본값은 꺼짐이다.
4. 레인 변경 시 과한 흔들림, 점프 시 천장 관통, 장애물 크기와 충돌의 시각적 불일치를 확인한다.
5. `PrisonRunner > Run Cell Block Slice Smoke Check`로 구조/충돌/시각 교체/카메라/후처리/맵 풀링을 검사한다. 기존 `Run Phase 2 Smoke Check`로 Prototype 회귀 검사도 실행한다.

`PrisonRunner > Build Cell Block Vertical Slice`는 위 프리팹/Material/장면을 같은 경로에 다시 생성한다. 수동 아트 편집 후 실행하면 해당 편집을 덮어쓰므로 재생성 전에 변경을 보관한다. 이 메뉴는 Gameplay 코드를 생성하거나 수정하지 않는다.

## 참조와 범위

`ART_DIRECTION.md`의 색상/형태 기준으로 제작한다. 지정된 `References/prison-art-reference.png`는 현재 저장소에 없어 이미지와 직접 비교하지 못했다. AI 3D 에셋, Pose, 전체 맵, 동적 FOV와 Shake 연출은 이번 단계에 포함하지 않는다. GPU별 프레임 성능과 최종 아트 비교는 후속 검수가 필요하다.

## 변경 파일

- 새 장면: `Assets/Game/Scenes/CellBlockVerticalSlice.unity`
- ProBuilder 프리팹/저장 Mesh: `Assets/Art/Environment/Blockouts/CellBlock/`
- 장애물 시각 프리팹: `Assets/Art/Obstacles/CellBlock/`
- 기본 Material: `Assets/Art/Materials/CellBlock/`
- Cinemachine 프리셋: `Assets/Settings/CameraPresets/CellBlockRunnerCamera.prefab`
- 표현 코드: `RunnerCinemachineCamera.cs`, `RunnerCameraEffects.cs`, `PrisonObstacleVisuals.cs`; `RunnerSceneBootstrap.cs`에는 카메라 프리팹 슬롯/연결만 추가.
- 생성/검사: `CellBlockSliceBuilder.cs`, `CellBlockSliceSmokeCheck.cs`, `Phase2SmokeCheck.cs`, `VisualSliceSmokeCheck.cs`
- 문서: 이 문서, `VISUAL_VERTICAL_SLICE.md`, 실제 렌더 캡처 `previews/CellBlockRunner.png`

새 Unity 에셋의 `.meta`도 함께 관리한다. Gameplay/Application/Core/Input/Collision/Score/Map 생성 코드와 Package 버전은 변경하지 않는다.

## 검증 결과

2026-09-28, Unity 6000.3.10f1에서 C# 컴파일 오류 없이 Cell Block 구조/저장 Mesh 참조/시각 Collider 분리/장애물 규격/시각 교체와 복원/Lane 토글/Cinemachine 효과 복원/기존 Volume 검사를 통과했다. 12회 청크 진행 후 활성 청크 7개, 전체 청크 9개, 장애물 12개로 Pooling 검사도 통과했다.

기존 `RunnerMVP`의 Phase 2/Visual 회귀 검사도 통과했다. Gameplay, Input, Collision, Score, Map 생성 파일과 Package Manifest/Lock의 변경이 없는 것을 확인했다. 실제 URP 렌더 캡처에서 실내 구성, 진입부 시각 모듈, 원거리 Fog/배경색 연결을 확인했다. 저장한 화면은 [CellBlockRunner.png](previews/CellBlockRunner.png)다.

검사 로그에는 이전부터 있던 Burst 해시 캐시의 `Unity.ShaderGraph.Editor` 참조 로딩 오류가 계속 기록된다. 이번 C# 컴파일과 Play 검사, URP 렌더는 통과했으며 해당 캐시나 Package 버전은 변경하지 않았다. 실제 키보드 플레이 감각과 프레임 성능은 Unity에서 직접 확인한다.

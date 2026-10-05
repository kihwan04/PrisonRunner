# 무한옥 에셋 후보와 교체 지점

최신 [ART_V9](ART_V9.md): Blender 수레 그립 Humanoid 동작과 실제 빈 수레, ProBuilder 좁은 절벽 철로/반쪽 붕괴, 계단 끝의 내려오는 전폭 슬라이딩 문을 실제 게임에 적용했다. 카트→하차→계단→슬라이딩 문→감옥의 전환을 검수한다. 무료 에셋/자체 제작만 사용했고 기존 무료 경찰 미임포트 상태는 유지한다.

최신 [ART_V8](ART_V8.md): 실제 Blender 빈 수레 `MineCartEmpty.fbx`, 밤하늘 shader/사육장 외곽 수목, 서비스 출구 연결부와 바닥 아이템을 적용했다. 무료 경찰 107256은 [제작사 공식 0원 배포](https://shop.bitgem3d.com/products/police-officer-low-poly-3d-proto-series)도 확인했으나 이메일/이름/주소를 요구해 사용자 직접 절차 완료 대기다. 실제 경찰 원본 파일은 아직 미임포트이며 자체 경찰을 사용한다. 상품 계정 취득/주문 화면 준비를 실제 임포트와 혼동하지 않는다.

최신 실제 제작/적용은 [ART_V7](ART_V7.md). 사용자 로딩 이미지에 맞춘 갈색 연결 스킨 원숭이, 표정 blend shape 2개, 광산 암벽·재질·실제 그림자, Kenney 상세 수목과 완성된 우리 바깥 지면을 적용했다. 무료 경찰 107256은 취득됐지만 파일 다운로드 인증이 실패해 미임포트 상태다. 현재 경찰은 자체 Blender 모델이다. 아래 V6 이전 후보/기록과 구분한다.

최신 사용자 지정 상품: [Police Officer - Proto Series / BitGem, 107256](https://assetstore.unity.com/packages/3d/characters/police-officer-proto-series-107256)은 무료이고 rigged 모델이다. 제작자 설명상 애니메이션은 포함하지 않으며 Mixamo/Mecanim 호환. Unity 로그인 및 2026-10-04 상품 취득을 확인했다. 편집기 다운로드 서비스의 계정 연결과 실제 파일 임포트가 남아 있다. [Cartoon Monkeys, 108270](https://assetstore.unity.com/packages/3d/characters/cartoon-monkeys-108270)은 확인 당시 $5.50이므로 무료만 사용한다는 선택에 따라 구매하지 않는다. 현재 자체 Blender 모델과 실제 Mixamo 동작을 유지한다. 최신 품질 작업은 [ART_V6](ART_V6.md), 경로/로딩 기능은 [ART_V5](ART_V5.md).

**최신 적용 기록: [TOOLS_V4](TOOLS_V4.md), 이전 환경 기록: [ART_V3](ART_V3.md). 사용자 선택에 따라 무료 에셋만 사용한다. 현재 캐릭터는 자체 Blender Humanoid이며 실제 Mixamo 동작 4개를 리타게팅했다. 13개 ProBuilder 편집 경로를 실제 런타임 메시로 베이크한다. 아래 유료 후보·가격은 이전 조사 기록이며 이번 작업에서 구매하거나 적용하지 않는다. 세탁기 등 포함 여부와 현재 모델 상태는 최신 기록이 우선한다. Asset Store 로그인은 확인했으며 Mine은 약관 승인 대기로 아직 미임포트다.**

2026-10-03 공식 판매처/제작자 페이지 확인. 아래는 **추천 후보**이며 상용 파일은 구매·다운로드하지 않았다. 가격은 확인 시점 USD이며 세금/할인에 따라 달라진다. 추천 적합도는 시안과 비교한 판단이다. 원본 시안과 완전히 동일한 모델이라는 의미가 아니다.

## 먼저 사용할 조합

| 용도 | 후보 / 공식 링크 | 확인 사항 | 프로젝트 적용 |
|---|---|---|---|
| 원숭이 | [Cartoon Monkeys — Saucy sushi shop](https://assetstore.unity.com/packages/3d/characters/cartoon-monkeys-108270) | 명세에 지정된 상품, $5.50, Unity 5.6.2 원본 | `CHR_Monkey_Prisoner`. 주황 죄수복/번호는 별도 수정 필요. URP 재질과 원숭이 rig/clip 호환성 검수 필요 |
| 경찰 | [Cartoon Police Officer in Uniform — ElegantMesh](https://elegantmesh_studios.artstation.com/store/w20bB/cartoon-police-officer-in-uniform) | 제작자 설명상 rigged, FBX 제공. 시안의 둥근 cartoon 방향 후보. Unity Humanoid 호환은 직접 검수 필요 | `CHR_Police_Officer`. 명세의 기존 "Police Officer" 상품 ID/원본 파일은 현재 저장소에서 확인되지 않음. 임의로 동일 상품이라 단정하지 않음 |
| 광산 소품 | [Stylized Mine Props Pack — Undertaker3D](https://assetstore.unity.com/packages/3d/props/stylized-mine-props-pack-311249) | $14.99, Unity 6000.0.24, URP 호환 표기. 레일/광산 소품 계열 | `PROP_MineCart`, `PROP_Pickaxe`, 광산 청크 장식 교체 후보. 포함 파일 목록은 구매 전 Package Content로 확인 |
| 광산 지형 무료 | [Modular Cave Kit — Kenney](https://kenney.nl/assets/modular-cave-kit) | 40개, CC0, modular cave | `CH_Mine_*`의 `EnvironmentRoot`. Unity material/scale는 임포트 후 검수 |
| 감옥 / 운동장 | [Modular Prison (URP) — DEXSOFT](https://assetstore.unity.com/packages/3d/environments/urban/modular-prison-urp-342320) | $29.99, Unity 6000.1.1, URP, 약 3.1GB. [제작자](https://dexsoft-games.com/products/modular-prison/)는 80+ meshes/예제 level 명시 | 현대 감옥 금속 복도/운동장/외벽에 가장 가까운 방향. 텍스처 디테일을 줄이고 조명/색상을 통일 |
| 스타일화 감옥 대안 | [Stylized Modular Prison Dungeon — Nordskogen](https://assetstore.unity.com/packages/3d/environments/dungeons/stylized-modular-prison-dungeon-327461) | $15, Unity 6000.0.45, URP/HDRP | cartoon 톤에 어울리지만 medieval dungeon 테마이므로 현대 감옥과 혼용 전 수정 필요 |
| 지하정비 / 설비 무료 | [Factory Kit — Kenney](https://kenney.nl/assets/factory-kit) | 140개, CC0, industrial | `CH_Maintenance`, utility pipes/warehouse obstacles. 구체적 모델 포함 여부는 다운받은 목록에서 확인 |
| 주방 / 세탁실 가구 무료 | [Furniture Kit — Kenney](https://kenney.nl/assets/furniture-kit) | 140개, CC0, interior/table/chair/bed | `CH_Kitchen` 가구 후보. 세탁기는 현 prefab을 유지하거나 별도 모델 필요; 포함됐다고 단정하지 않음 |
| 먼지 / 충돌 VFX 무료 | [Cartoon FX Remaster Free — Jean Moreno](https://assetstore.unity.com/packages/vfx/particles/cartoon-fx-remaster-free-109565) | 무료, URP 호환 표기, smoke/cartoon particles | `VFX_Dust_Run`, `VFX_Hit_Stumble`. 정품 임포트 후 효과를 골라 속도/크기 제한 |
| 전구 / 경광등 / 레이저 | 현재 프로젝트의 primitive + emission + Light | 추가 구매 없이 교체 가능 | `VFX_LightBulb_Idea`, `VFX_AlarmLight`, `OBS_LaserGate` |
| UI 무료 | [UI Pack — Kenney](https://kenney.nl/assets/ui-pack) | 430개, CC0 | HUD 패널/Retry 버튼 후보. 로고는 `무한옥` 한글 텍스트 자체 제작 유지 |

## ID 전체 매핑

| Asset ID | 현재 구현 | 교체 후보 |
|---|---|---|
| CH_Mine_Start / CH_Mine_Straight_A / CH_Mine_Straight_B | 암석·목재·레일·따뜻한 조명 blockout | Modular Cave Kit + Stylized Mine Props |
| CH_Transition_MineToPrison | 목재 arch/금속 통로 연결 | 두 kit의 경계 piece 직접 제작 |
| CH_Prison_Corridor_A / CH_Prison_Corridor_B | 금속 벽·cell bars·차가운 조명 | Modular Prison URP |
| CH_Checkpoint | 경광등·보안 장애물 | Modular Prison + 현재 alarm/laser |
| CH_Stair | HighKnee/Jump로 통과 가능한 계단 장애물 | Modular Prison stair mesh |
| CH_Laundry | 세탁기 형태 blockout | 기존 blockout 유지, 별도 washer 필요 |
| CH_Kitchen | 조리대 blockout | Furniture Kit + 감옥 식당 props |
| CH_Maintenance | utility pipe blockout | Factory Kit |
| CH_Yard / CH_OuterWall | 외벽·bar fence blockout | Modular Prison 외부 meshes |
| CHR_Monkey_Prisoner / CHR_Police_Officer | primitive 관절 + placeholder Animator | 위 캐릭터 후보 / 보유 원본 |
| PROP_MineCart / PROP_Pickaxe | primitive + 자체 생성 rumble audio | Stylized Mine Props |
| OBS_Crate_Low / OBS_LowPipe / OBS_Barrier_Left / OBS_Barrier_Right | 기존 CellBlock crate/pipe/barricade visual 재사용 | 현재 visual 유지 가능 |
| OBS_Cart_Crossing | 수레형 blockout | Stylized Mine Props cart |
| OBS_LaserGate / OBS_Stair_HighKnee | emission gate / low step blockout | 자체 visual 유지 또는 imported mesh |
| VFX_LightBulb_Idea / VFX_AlarmLight | emission bulb/beacon + light | 자체 유지 |
| VFX_Dust_Run / VFX_Hit_Stumble | particle placeholder | Cartoon FX Remaster Free |
| UI_Logo_Muhanok / UI_HUD_Set | ID prefab + HUDPresenter 텍스트/패널 | 자체 로고 + UI Pack |

## 안전하게 교체하는 방법

1. 정품 패키지를 `Assets/External/Staging`에 임포트하고 URP/크기/pivot/rig를 확인한다.
2. `GameScene`의 `Muhanok Bootstrap`에서 Monkey/Police/Cart/Pickaxe/VFX prefab 필드를 지정한다. 코드 수정 없이 art만 교체 가능하다.
3. 캐릭터 Animator state명을 `Content/Animations`의 `AN_Monkey_*`, `AN_Guard_*`와 맞추거나 Animator Override Controller로 실제 clip을 매핑한다. gameplay 이동은 root motion에 의존하지 않는다.
4. 청크 prefab의 `Entry`, `Exit`, `Lane_*`, `ObstacleSockets`, `ObstacleController`는 유지하고 `EnvironmentRoot`만 교체한다. 길이 24m, X -2.2/0/+2.2를 유지한다.
5. 장애물은 게임 판정 root의 `ObstacleController.Kind`를 유지하고 시각 mesh만 교체한다. 현재 충돌은 z 통과를 검사하는 swept gameplay 판정으로 처리한다.
6. 에셋 연결 후 EditMode/PlayMode 테스트와 시각 검수를 실행한다. Build GameScene 메뉴는 이미 존재하는 ID prefab을 덮어쓰지 않는다.

현재 시안 수준의 최종 모델/재질/애니메이션이 완성된 상태는 아니다. 구매 후보 탐색과 실제 prefab 교체 경로를 준비한 playable prototype이다.

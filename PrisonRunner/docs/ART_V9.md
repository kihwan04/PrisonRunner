# V9 · 전용 카트 철로와 운동 장애물

2026-10-04. 최신 사용자 요청에 따라 V8의 3레인 카트 장애물과 바나나 비치명 규칙을 변경했다. 첨부한 Temple Run 카트 화면을 구도와 경로 설계의 기준으로 사용했다.

| 구간 / 상황 | 실제 게임 동작 |
|---|---|
| 광산 | 3레인 달리기. 첫 구간 후반의 낮은 배관은 몸을 숙여 통과한다. |
| 탑승 지점 | 광산 마지막 4m에 승강장과 중앙 철로. Z24에서 자동 탑승하고 후방 3인칭 카메라로 전환한다. |
| 카트 길 | Z24~72, 48m의 전용 단선 목재 다리. 곡선 철로, 아래의 깊은 절벽, 먼 작업 통로/차가운 조명, 가까운 따뜻한 등불. 일반 상자·차단문·배관 장애물을 배치하지 않는다. |
| 무너진 철로 | 두 카트 청크에 각각 2개. 실제 메시에서 한쪽 레일·다리 바닥을 없애고 매달린 레일과 깨진 침목을 배치했다. 없어진 쪽의 반대쪽으로 몸을 기울이면 통과한다. 잘못 기울이거나 중앙이면 추락/게임오버. |
| 카트 조작 | A/D 또는 인식된 LEFT/RIGHT로 좌우 균형. 수레와 원숭이는 단선 중심을 따라 이동하고 몸/수레/카메라가 기울어진다. CENTER는 중립. 점프/하이니는 카트에서 받지 않는다. |
| 하차 | Z72에서 승강장으로 하차하고 1인칭 달리기로 복귀. 카메라는 현재 출력 위치에서 보간한다. 바로 다음 연결 청크의 점프·숙이기를 지나 계단으로 이어진다. |
| 계단 → 감옥 | 계단 전반은 하이니, 계단 끝 Z115에는 세 레인을 가로지르는 내려오는 문. 접근하면서 문이 내려오고 통과 순간 바닥에서 0.95m의 틈을 남긴다. 몸을 숙여 슬라이딩하면 통과하고, 3m 지나면 문이 뒤에서 바닥까지 완전히 닫힌다. Z120부터 바로 감옥 복도로 이어진다. 서서 통과하거나 레인 이동만 하면 충돌 종료. |
| 운동 장애물 | 낮은 상자/통나무는 점프, 낮은 배관은 숙이기, 계단은 하이니, 바닥까지 내려오는 감옥 문은 레인 이동. 계단을 점프로 대체할 수 없다. |
| 운동 행 | 광산 배관, 시설 연결의 점프/숙이기, 계단 하이니, 감옥 출구 배관에는 세 레인 모두 같은 종류의 장애물이 놓인다. 지정 운동으로 통과할 수 있으며 레인 이동만으로 모든 운동을 생략할 수 없다. 문은 여전히 빈 레인을 남긴다. |
| 바나나 | 한 플레이에서 누적. 첫 번째는 2.4초 50% 감속과 추격 압력 45%. 두 번째부터는 감속 중 1.2초 동안 경찰이 따라잡아 Caught. 중간에 감속이 끝나도 접촉 횟수는 남는다. 점프로 피하면 횟수가 증가하지 않는다. 일시정지는 추격 시간을 멈추고 다시 하기는 초기화한다. |
| 물웅덩이 | 2.4초 50% 감속만 적용. 바나나 횟수로 계산하지 않는다. |
| 고체 충돌 | 잘못된 동작으로 한 번 실제 접촉하면 즉시 이동 정지. 0.65초 후 결과. |
| 시작 / 운동 기록 | 타이틀 → 선택 몸무게 → 제공 로딩 이미지 → 0.85초 전환 → 바로 실행. 기존 예상 kcal는 연결된 웹캠이 실제 운동 동작을 인식한 시간만 반영한다. 고정 4 MET 가정의 추정값이며 키보드 테스트/정지/로딩 시간은 반영하지 않는다. |

순서는 광산 → 카트 → 시설 연결 → 계단 → 닫히는 문 아래로 슬라이딩 → 감옥 복도 → 출구 → 사육장 → 광산 복귀다. 전체 240m를 반복하며 120개 청크의 높이·중심·접선 연결과 풀 재사용 검증을 유지한다. 카트는 점프에서 잠시 쉬면서 몸을 좌우로 기울이는 운동 구간이다.

## 제작과 적용

- Blender 4.5.9로 수레에 앉아 손을 앞으로 내미는 `AN_Monkey_CartRide.fbx`를 제작했다. 자체 Humanoid 모델에 실제 리타게팅하고 반복 상태로 등록했다. 자체 원본 동작 17개와 기존 Mixamo 실제 4개 동작을 유지한다.
- ProBuilder 6.1.2 카트 원본 2개를 좁은 다리/한쪽 바닥 붕괴로 다시 만들고 실제 런타임 메시로 베이크했다. 기존 13개 편집용 원본은 유지한다.
- 실제 3D 빈 수레 프리팹, 손잡이·전조등·원숭이 보조 조명·굴러가는 소리를 연결했다. 카트에서 1인칭 손/가짜 카트 테두리는 숨긴다.
- 단선 경로의 곡선 폭을 ±3.2m로 확대했다. 다리, 지지대, 암벽, 먼 작업 통로와 화살표 표지가 실제 같은 곡선을 따르도록 변형된다. 카트 공간은 차가운 절벽/안개와 따뜻한 조명의 색 대비를 사용한다.
- 큰 공중 운동 문자는 제거하고 실제 표지판과 한국어 HUD로 다음 동작을 알려 준다. 세 가지 창 비율에서 16:9 및 안전 영역을 유지한다.
- 검수용 명령줄 실행은 개인 최고 기록을 저장하지 않는다. 정상 게임 기록 저장은 유지한다.

## 파일

새 파일:

- `Unity/Assets/Game/Presentation/Map/CartRailGap.cs`: 실제 끊긴 레일의 통과/기울이기 판정.
- `Unity/Assets/Game/Presentation/Map/ClosingSlideGate.cs`, `Unity/Assets/Game/Editor/MuhanokArtUpgrade.SlideGate.cs`: 계단 끝 문 내려옴/슬라이딩/뒤에서 완전히 닫힘과 실제 3D 문 제작.
- `Unity/Assets/Game/Presentation/Player/CartRideView.cs`: 실제 수레·원숭이 표시/기울기/추락/음향.
- `Unity/Assets/Game/Editor/MuhanokArtUpgrade.Cart.cs`: Blender 동작 등록, 수레 프리팹, 절벽 다리, 승강장, 운동 종류 재생성.
- `Unity/Assets/External/Staging/Blender/Characters/AN_Monkey_CartRide.fbx` 및 대응 `.meta`.
- `Unity/Assets/Game/Content/Prefabs/PROP_CartRide.prefab` 및 대응 `.meta`.
- 이 문서, `ART_V9_QA.md`, `previews/ArtV9/` 실제 검수 이미지/갤러리.

주요 수정 파일:

- `Domain/RunRules.cs`, `RouteSurface.cs`: 두 번 바나나 추격, 기울이기/좁은 이동, 운동 구분, 단선 곡선/붕괴 위치.
- `Presentation/Map/MuhanokMapChunk.cs`, `ChunkSpawner.cs`, `FloorItemController.cs`, `CoinController.cs`, `WorldAtmosphere.cs`: 실제 배치·판정·풀 리셋·조명 전환.
- `Presentation/Player/RunnerView.cs`, `FirstPersonHands.cs`, `Presentation/Camera/FirstPersonCameraRig.cs`: 카트 동작과 시점/연속 전환.
- `Presentation/UI/HUDPresenter.cs`, `HUDPresenter.Start.cs`: 다음 운동, 바나나 누적/추격과 추락/잡힘 표시.
- `Bootstrap/GameBootstrap.cs`, `RuntimeVisualReview.cs`: 수레 조합, 실제 플레이/검수 흐름.
- `Editor/MuhanokToolUpgrade.cs`, `MuhanokArtUpgrade.Maps.cs`, `.Route.cs`, `MuhanokVisualPolish.cs`: 재생성/검증.
- `Tests/EditMode/RunRulesTests.cs`, `Tests/PlayMode/CollisionAndCartTests.cs`, `GameSceneTests.cs`, `StartAndGateTests.cs`: 규칙과 실제 씬 회귀 검증.
- `Tools/AssetPipeline/build_blender_characters.py`, `create_art_gallery.py`, `verify_capture_frames.py`: 원본 제작/실제 GPU 검수.
- 원숭이 Animator/override, 13개 청크 프리팹과 관련 메시/재질, 카트 ProBuilder 원본 2개, `GameScene.unity`, `GameSettings.asset`, Blender 원본/manifest, README/AGENTS/상태 문서도 갱신했다.

전체 재생성은 `Muhanok → Upgrade Dedicated Cart Tracks and Exercise V9`, 코드 진입점은 `MuhanokVisualPolish.UpgradeV9`. 현재 카트 ProBuilder 메시를 수정한 뒤 게임에 반영하려면 `Muhanok → Bake Edited ProBuilder Routes Into Game`을 사용한다.

## 남은 품질 차이

무료 에셋과 자체 Blender/ProBuilder 모델만 사용했다. 화면의 경찰은 자체 모델이다. 무료 BitGem 경찰 107256은 기존 정상 다운로드 인증 문제로 원본이 아직 임포트되지 않았다. 기존 공식 0원 배포의 개인정보 입력 화면은 사용자 직접 처리 대기다. Higgsfield는 호출 가능한 연결이 없으며 실행했다고 보고하지 않는다.

전용 경로와 공간 구성/동작 판정은 개선했지만 원숭이·경찰의 세부 모델, 걷는 길의 장식 다양성, 접지/표정 연출은 참고 로딩 그림과 상용 benchmark에 차이가 있다. **Subway Surfers/Temple Run과 동일한 전체 아트 완성도로 판정한 결과는 아니다.** 다음 우선순위는 캐릭터 모델/표정/접지와 맵별 고유 장식의 추가 제작이다. 웹캠 개인별 운동 인식 및 칼로리 추정 정확도는 자동 입력 검증만으로 인증하지 않는다.

# V8 · 로딩 인트로, 수레 조작, 충돌/감속, 연속 공간

2026-10-04. 최신 사용자 요청이 기존 8초 인트로/추격 게이지 누적 충돌 규칙보다 우선한다. 상용 게임은 아트와 동작의 목표이며, 현재 결과를 서브웨이 서퍼스와 같은 품질로 판정하지 않는다.

## 실제 구현

| 상황 | 현재 동작 |
|---|---|
| 게임 시작 | 타이틀 → 선택 몸무게 → 제공된 로딩 이미지/실제 준비 진행률 → 0.85초 전환 → 바로 1인칭 플레이 |
| 로딩 인트로 | 정상 시작에 별도의 8초 Timeline을 추가 재생하지 않는다. 전환 중 게임과 입력을 멈춘다. 기존 Timeline은 별도 연출/검증용으로 보존한다. |
| 고체 장애물 | 상자/통나무/차단문/낮은 파이프 등의 실제 충돌 한 번이면 즉시 Caught, 이동 정지. 0.65초 후 결과. 점프/숙이기/레인 회피는 장애물 종류에 따라 가능. |
| 바나나·물웅덩이 | 해당 레인을 지상으로 밟으면 2.4초 동안 속도 50%, 이후 회복. 게임오버/추격 압력 증가 없음. 점프로 피할 수 있다. |
| 수레 | 광산 두 청크 Z24~72, 48m 연속 탑승. 자동 전진 10.5m/s, A/D 조향, S 숙이기. 점프/하이니 금지. 조향 시 카메라 기울기, 실제 빈 수레/손 그립/굴러가는 소리, 탑승/하차 보간. |
| 맵 | 광산 → 수레 → 광산/시설 연결 → 계단 → 감옥 복도/닫히는 문 → 서비스 출구 → 사육장 내리막 → 사육장/광산 복귀 연결. 10×24m 반복, 높이와 접선 연결 유지. |
| 표현 | 실제 Blender 빈 수레 FBX, 서비스 출구 낮은 벽/차양/조명/식물 미리보기, 더 넓은 사육장 지면/뒤쪽 수목, 실제 night sky shader의 구름·별·달·후광. |
| HUD | 둥근 패널, 자체 코드로 만든 코인/왕관 아이콘, 파란 일시정지, 수레 진행도/감속 상태. 16:9와 안전 영역 유지. |

수레는 완전한 휴식 구간이 아니라 점프 운동을 잠시 쉬면서 조향과 숙이기로 피하는 구간이다. [Temple Run 2 제작사 페이지](https://imangistudios.com/thegames/temple-run-2/)를 참고했으며 구체적인 규칙/연출은 이 프로젝트에서 구현한 해석이다. Subway Surfers의 캐릭터/레벨 파일을 가져오지는 않았다.

## 새 파일

- `Unity/Assets/Game/Presentation/Map/FloorItemController.cs`: 독립적인 바닥 감속 아이템, 실제 통과/레인/점프 판정 및 풀 리셋.
- `Unity/Assets/Game/Content/ArtV2/NightSky.shader`: 실제 3D 실행에 사용하는 밤하늘/달 shader.
- `Unity/Assets/Game/Tests/PlayMode/CollisionAndCartTests.cs`: 실제 청크 바닥 아이템/충돌과 수레 표현을 검증.
- `Unity/Assets/External/Staging/Blender/MineCartEmpty.fbx`: Blender로 만든 빈 수레. 원본과 파이프라인도 갱신.
- `docs/ART_V8.md`, `docs/ART_V8_QA.md`, `docs/previews/ArtV8/`: 구현/검수/실제 화면 기록.

## 주요 수정 파일

- `Domain/RunRules.cs`, `Domain/RouteSurface.cs`: 한 번 충돌 종료, 감속/회복, 수레 규칙, LoadingIntro 흐름.
- `Bootstrap/GameBootstrap.cs`, `Presentation/Flow/IntroSequenceDirector.cs`: 로딩 뒤 바로 플레이, 준비 완료 후 전환, 숨은 입력 방지.
- `Presentation/Map/ChunkSpawner.cs`, `MuhanokMapChunk.cs`, `ObstacleController.cs`: 아이템 풀링/평가, 수레 안전 레인 보장, 흐름에 맞는 충돌 가드.
- `Presentation/Player/FirstPersonHands.cs`, `Presentation/Camera/FirstPersonCameraRig.cs`: 빈 수레/그립/음향/카메라 기울기.
- `Presentation/UI/HUDPresenter.cs`, `HUDPresenter.Start.cs`, `Presentation/Map/WorldAtmosphere.cs`: HUD, 결과/감속/수레 상태, 구간별 분위기 전환.
- `Editor/MuhanokArtUpgrade.cs`, `.Maps.cs`, `.Zoo.cs`, `.Route.cs`, `MuhanokVisualPolish.cs`: 재생성 가능한 실제 모델/맵/아이템/밤하늘/설정 적용.
- `Bootstrap/RuntimeSmokeCheck.cs`, `RuntimeVisualReview.cs`, EditMode/PlayMode 테스트: 최신 시작과 실제 충돌을 검증.
- `Tools/AssetPipeline/build_blender_modules.py`, `create_art_gallery.py`, `verify_capture_frames.py`: 빈 수레 FBX 및 실제 GPU 검수 기록.
- 생성된 청크 13개, 손 프리팹, 씬, 베이크 메시, 재질, `GameSettings.asset`도 갱신했다. 이전 시연 씬은 보존한다.

경로는 `Unity/Assets/Game/` 기준. 전체 재생성: `Muhanok → Upgrade Gameplay and Connected Maps V8` / `MuhanokVisualPolish.UpgradeV8`. ProBuilder 6.1.2 원본 경로는 편집용으로 남기고 실제 실행 메시로 베이크한다. Unity/URP 등 기존 패키지를 임의 업그레이드하지 않았다.

## 실제 적용과 남은 작업

무료 Kenney CC0, 자체 Blender 모델/모듈, 실제 Mixamo 4개 동작과 기존 생성 재질 atlas가 적용돼 있다. 유료 상품은 구매하지 않았다. Higgsfield/외부 3D 생성 서비스는 현재 호출할 연결 도구가 없어 실행하지 않았다.

무료 BitGem 경찰 107256은 Unity 계정 취득은 확인됐지만 편집기 정상 다운로드 서비스가 인증 실패해 모델 파일은 미임포트다. 반복 로그인 요구 대신 [제작사 공식 0원 배포](https://shop.bitgem3d.com/products/police-officer-low-poly-3d-proto-series)를 확인했고 1개/0원 주문 화면까지 준비했다. 이메일·이름·주소가 필수여서 임의 개인정보 제출이나 주문 확정은 하지 않았다. `docs/previews/ArtV8/PoliceFreeCheckout.png`에 빈 입력 상태를 기록했다. 사용자 직접 절차 완료 후 파일 다운로드/리타게팅/실제 재질 검수가 필요하다. 현재 보이는 경찰은 자체 모델이다.

참고 로딩 그림보다 캐릭터 실루엣/표정/달리기 접지, 동물 모델과 동작, 소품 다양성/배치가 단순하다. **로딩 그림과 동일한 전체 그래픽 완성도는 아직 미달**이며, 게임 규칙 구현/공간 연결/화면 보강과 구분해서 평가한다. 다음 우선순위는 무료 경찰 원본 적용과 캐릭터·맵별 구성의 추가 아트 제작이다. 웹캠 실사용/운동 코칭의 정확도는 자동 입력 검증만으로 인증하지 않는다.

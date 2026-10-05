# 화면 비율과 연속 경로 개선 — 2026-10-03

이번 사용자 요청은 화면 잘림 해결, 각 맵의 자연스러운 연결, Blender를 포함한 무료 아트 개선이다. 현재 결과는 참고 이미지와 동일한 최종 품질이라고 주장하지 않는다.

## 화면

- 실제 게임 카메라와 HUD는 안전 영역 안에서 **16:9**를 유지한다. 넓은 창은 좌우, 4:3·세로 창은 위아래에 검은 여백을 두며 UI를 늘리거나 잘라내지 않는다.
- Unity `Muhanok/Fit Game View (16:9)` 메뉴는 1280×720 렌더 크기를 선택하고 게임 뷰 확대율을 창에 맞춘다. `PlayInUnity.cmd` 실행 시 자동 적용한다.
- 원래 스크린샷에서 저장된 GameView 확대율은 약 116%였다. 런타임 UI 배율과 에디터 확대율을 함께 수정했다.

## 연결된 맵

광산 시작 → 굽은 선로·수레 시점 → 광산 → 광산/시설 전환 → 감방 A/B → 검문소 → 산업 계단 → 세탁실 → 주방 → 정비실 → 야간 동물 사육장 → 시설 복귀. 이후 감방부터 반복한다.

- 길이는 모두 24m, 레인 간격은 2.2m다. Entry/Exit 소켓을 X/Y/Z 전체 좌표로 맞춘다.
- `RouteSurface`가 선로 곡선과 높이를 제공한다. 계단에서 3m 올라가고 세탁실·주방·정비실에서 유지한 뒤 야외 경사로에서 내려온다. 모든 연결부의 위치와 기울기가 연속이다.
- 바닥·레일·벽 메시, 광원·소품·장애물·코인, 플레이어와 카메라가 같은 경로를 따른다. 실제 판정은 기존 Domain 규칙을 사용한다.
- 광산에서 콘크리트로 바뀌는 벽과 레일 끝, 시설/야외 캐노피, 공통 프레임·경계 표시를 추가했다. 안개와 환경광은 경계에 접근하면서 보간한다.
- 수레 구간에는 1인칭 수레 난간과 작은 손 움직임이 나타난다. 입력/충돌 규칙은 기존 러너 방식이다. 분기 선택·별도 차량 물리는 구현하지 않았다.
- 사육장은 야간 하늘·달, 나무·관목·돌, 철제 울타리와 기린·코끼리로 구성했다. 동물은 정적 모델이다.
- 정적 소품과 자체 환경 메시를 재질별로 결합했다. 랜턴의 추가 그림자는 꺼서 shadow atlas 경고를 줄였다.

## 무료 에셋과 Blender

Kenney 공식 CC0 키트에 [Nature Kit](https://kenney.nl/assets/nature-kit)의 실제 FBX **329개**를 추가했다. 기존 403개와 합쳐 **732 FBX**가 Staging에 있다. 전부 씬에 배치한 것은 아니다. 출처·ZIP SHA256은 `FREE_ASSETS_IMPORTED.json`에 기록했다.

Blender **4.5.9 LTS**를 [공식 배포 서버](https://download.blender.org/release/Blender4.5/)에서 포터블로 받아 공식 SHA256 파일과 비교했다. Unity·UPM 버전은 변경하지 않았다.

| 파일 | 용도 |
|---|---|
| `References/Blender/MuhanokModules.blend` | 편집 가능한 자체 제작 원본, 이름별 collection |
| `Tools/AssetPipeline/build_blender_modules.py` | Blender 모델 재생성 스크립트 |
| `Unity/Assets/External/Staging/Blender/Models` | 암벽·천장 아치·바위 더미·양손·기린·코끼리 **7 FBX** |
| `Tools/BlenderRuntime/blender-4.5.9-windows-x64/blender.exe` | 프로젝트 안의 포터블 Blender 실행 파일 |
| `Assets/Game/Editor/MuhanokArtUpgrade.Route.cs` | 공통 연결부와 영구 경로 메시 생성 |
| `Assets/Game/Domain/RouteSurface.cs` | Unity에 의존하지 않는 곡선·높이 규칙 |
| `Assets/Game/Presentation/UI/GameViewport.cs` | 카메라·HUD 화면 비율과 안전 영역 |

`Content/ArtV2` 경로는 기존 참조를 유지한다. 이번 실제 화면은 `docs/previews/ArtV3/index.html`에서 확인한다. 블록아웃 백업과 이전 실제 화면은 보존했다.

## Asset Store 상태와 남은 문제

로그인은 확인했다. 무료 [Mine](https://assetstore.unity.com/packages/3d/environments/dungeons/mine-92461)을 추가하려면 서비스 약관/EULA 승인 단계가 필요하여 사용자 승인을 요청한 상태다. **이 패키지는 아직 다운로드·임포트하지 않았다.** 현재 모델은 Kenney CC0와 자체 Blender 제작 모델이며 이를 Asset Store 임포트라고 표현하지 않는다.

캐릭터의 정교한 스키닝·표정, 고급 재질·음향·연출과 분기 선로는 참고 이미지와 차이가 있다. 실제 웹캠 동작 정확도와 장시간 FPS는 별도 플레이 검수가 필요하다. 먼저 현재 실행 화면을 기준으로 캐릭터와 환경 디테일을 다듬는 것이 다음 작업이다.

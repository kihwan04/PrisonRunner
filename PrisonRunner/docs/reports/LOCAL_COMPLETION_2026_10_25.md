# Mongscape 로컬 수정본 · 바닥 깜빡임 / 난이도 강화 / 상단 사이렌

2026-10-25 최신 수정본입니다. GitHub 커밋·푸시·PR·릴리스 업로드는 하지 않았습니다. 웹캠 실물 확인은 사용자 요청으로 보류했습니다.

## 실행과 전달 파일

- **Mongscape-Windows-x64.zip**: 모두 압축 해제한 뒤 **StartGame.cmd** 실행. Unity와 Python을 따로 설치할 필요가 없습니다.
- **Mongscape-Unity-Source.zip**: Unity 6000.3.10f1 프로젝트, 리소스, 서버 소스, 검증·배포 도구.
- **Mongscape-FaceLogin-DB.zip**: Python, OpenCV, YuNet/SFace 모델을 포함한 독립 로컬 얼굴 DB 서버.
- **Mongscape-EmptyProfiles.sqlite3**: 빈 DB 스키마 확인용. 실제 실행 DB는 Services/Identity/data/profiles.sqlite3에 자동 생성합니다.

기존 파일과 이름이 같으므로 이번 ZIP을 새 폴더에 풀어 실행하세요. 개발 폴더에서는 루트 StartGame.cmd 또는 PrisonRunner/StartGame.cmd로 최신 검증 빌드를 실행합니다. 얼굴 사진·사용자 프로필·토큰·실제 DB는 ZIP에 포함하지 않습니다.

## 이번 요청에 맞춘 변경

| 요청 | 수정된 동작 |
| --- | --- |
| 바닥 깜빡임 | 새 갈림길·맵 연결 바닥과 같은 높이에 남아 있던 원본 RouteStructure를 함께 숨깁니다. 일반 맵의 원본 바닥은 장식 타일보다 2.5cm 낮춰 겹침을 없앱니다. 청크 반환 시 원래 상태를 복원하므로 재사용해도 높이가 계속 내려가지 않습니다. 돌·나무·공용 아틀라스는 삼중 선형 필터와 이방성 8로 먼 타일의 무늬 떨림을 줄였습니다. |
| 더 어려운 달리기 | 도보 시작 8m/s, 거리 160m마다 0.6m/s씩 점진 가속해 최대 12m/s입니다. 수레는 6m/s로 휴식합니다. 도보 차단물 확률은 광산·감옥 72%, 동물원 52%, 차단 행의 두 레인 폐쇄 비율은 85%입니다. 바나나와 차단물에서 모두 안전한 차선은 최소 하나 유지합니다. |
| 딱 세 갈래 | 공통 철로에서 좌·중·우 세 터널로 갈라집니다. 수레·캐릭터·카메라·코인·청크 연결이 선택한 경로를 따라가며 다시 합쳐지지 않습니다. 다음 순환도 해당 위치에서 이어집니다. |
| 뚫려 보이던 맵 | 공동에서 세 터널로 변하는 구간의 단면을 연속으로 다시 만들었습니다. 바닥·벽·천장에 닫힌 두께를 주고, 선택하지 않은 출구의 끝을 막았습니다. 일반 광산·감옥·동물원 청크에도 장식 거리 설정과 분리된 구조 바닥·벽을 보강했습니다. |
| 모르는 목적지 / 뽑기 | 매 순환 세 목적지를 중복 없이 무작위로 섞습니다. 같은 방향에서도 다른 목적지가 나옵니다. 맵 이름 선택 카드·색상 안내·사전 목적지 표시는 제거했습니다. |
| 선택 방식 | 108~144m에서 실제 세 차선을 사이드스텝으로 이동합니다. 120~144m는 차선 간 이동이 가능한 넓은 공통 플랫폼입니다. 144m를 지날 때 실제 도착한 차선으로 확정하고 터널이 갈라집니다. 수레·캐릭터·카메라도 함께 옆으로 이동합니다. 키보드는 A/D 한 차선씩 이동, C 가운데입니다. 전신 입력은 몸의 좌우 구역을 따르고 상체 방식은 상체 이동을 사용합니다. |
| 선택 후 잠금 | 144~216m의 선택한 레일 구간에서는 A/D/C와 몸동작 방향 변경을 모두 무시하고 중앙 자세를 유지합니다. 하차 때 좌우 좌표를 정리해 옆으로 튀지 않으며 도보 회피 조작은 다시 허용합니다. |
| 목적지 | 세 방향에 동물원·감옥·광산을 무작위로 중복 없이 배정합니다. 이전의 감옥/혼합처럼 초반 모습이 같은 두 경로를 정리해 세 실제 맵으로 진입하게 했습니다. 한 순환 48청크, 1,152m는 유지합니다. |
| 자연스러운 연결 | 선택한 터널 끝은 도보 통로 폭으로 점차 넓어집니다. 동물원·광산·감옥 입구를 별도 연결 청크로 맞추고, 야외 구간 끝에도 덮인 광산 진입로를 둡니다. 계단과 내리막은 기존 높이 곡선으로 연결합니다. |
| 아예 못 가는 차선 | 광산 돌 수레, 감옥 잠긴 문, 동물원 사료 철장에 2.18m 폭·3.4m 높이의 완전 차단면과 X 보강대를 추가했습니다. 한두 차선을 막으며 점프·숙이기로 통과할 수 없습니다. 옆 차선으로 피해야 합니다. |
| 공정한 배치 | 차단물과 바나나에 모두 막히지 않는 차선을 최소 하나 남깁니다. 차단 행이 있는 청크의 기존 운동 행을 꺼서 충돌하지 않게 하고 코인은 열린 차선으로 유도합니다. |
| 바나나 | 도보 구간에 무작위 레인·방향으로 껍질을 여러 개 배치합니다. 누적 두 번 밟으면 기존 1.2초 추격 후 경찰에게 잡힙니다. |
| 바나나 경고 사이렌 | 바나나를 밟으면 화면 상단에 작은 빨강·파랑 경찰 사이렌 아이콘이 약 3.6초 나타납니다. 달리는 화면의 경찰 손·소매·곤봉과 큰 경고 문구는 제거했습니다. 일시정지·잡힘·결과·재시작에서는 경고를 숨깁니다. 누적 두 번 추격 중에는 경고를 유지합니다. |
| 경찰 연출 | 잡히면 원숭이가 손·발끝으로 몸을 받치는 엎드려 뻗친 자세를 취합니다. 경찰이 세 번 곤봉을 휘두르는 3.6초 만화식 연출 후 결과로 넘어갑니다. 다시 시작하면 원래 자세와 애니메이터를 복원합니다. 레일 붕괴는 별도 추락 연출을 유지합니다. |
| 로그인 얼굴 | 얼굴 사진이 실제 머리 관절의 위치와 회전을 따라갑니다. 엎드린 연출에서도 로그인한 얼굴을 붙인 원숭이로 보입니다. 검수에는 체크 무늬를 사용했으며 본인 얼굴·웹캠 실물 검증은 보류했습니다. |
| 칼로리 직접 입력 | 준비 화면에서 목표를 숫자로 입력합니다(1~5000 kcal). 목표 도달·초과는 표시만 하며 플레이와 운동 기록은 계속됩니다. |
| 최적화 | 터널 메시·물리 충돌 데이터를 미리 만들고 재사용합니다. 선택 뒤에는 다른 터널의 뒤쪽 레일과 랜턴을 숨기며, 닫힌 암벽 밖의 보이지 않는 조명을 끕니다. 앞쪽 구조를 96m까지 준비하고 안개는 36~68m에 적용하며 카메라 원거리도 제한합니다. 기존 장애물·동물·바나나 풀을 유지합니다. |

동물원의 기린 목·코끼리 코 점프, 운동 콤보, 얼굴 로그인, 얼굴 캐릭터 마스크, 손으로 버튼 가리키기 기능은 유지했습니다.

## 조작

- 도보: A/←, D/→ 레인 이동, Space 점프, S/↓ 숙이기, W/↑ 하이니.
- 갈림길: A/←, D/→ 한 차선씩 사이드스텝, C 가운데. 전신은 실제 옆걸음, 상체는 상체 좌우 이동입니다. 일반 단선 수레에서는 기존 좌우 기울이기를 유지합니다.
- F1 설정, F3 성능 표시. 얼굴 없이 키보드로 바로 플레이할 수 있습니다.

칼로리는 몸무게와 인식된 활동 시간을 이용한 예상치이며 동작군별로 기록합니다. 전신 4 MET, 상체 2.5 MET 가정을 사용합니다. 키보드 입력을 운동으로 적립하지 않으며 몸무게를 생략하면 칼로리를 계산하지 않습니다.

## 실제 검증

- Unity EditMode **80/80 통과**: 거리별 난이도 가속과 상한·수레 휴식·일시정지, 랜덤 배정·중복 방지, 선택 후 방향 명령 잠금, 하차 좌표 연속성 등.
- Unity PlayMode **34/34 통과**: 갈림길·연결로 바닥의 겹친 면 제거, 일반 바닥 보정과 풀 재사용, 선택 확정 프레임의 캐릭터 경로 동기화, 바나나 경고·일시정지·재시작 상태, 기존 빈 면·장애물·얼굴·경찰 연출 회귀 검증.
- 휴대용 Python의 실제 인식 모델을 사용하는 DB 서버 통합 테스트 **4/4 통과**. 임시 DB/공개 테스트 사진을 사용했으며 사용자 얼굴 검증으로 표현하지 않습니다.
- 최종 Windows 빌드 성공, C# 컴파일 오류 0개.
- 초기 자동 검수의 종료 직전 전체 화면 복원 과정에서 발생한 Win32 창 콜백 충돌은 해당 복원을 제거한 뒤 재현되지 않았습니다. 최종 전체 검수와 일반 실행 검사는 모두 종료 코드 0입니다. 검수 설정은 디스크에 저장하지 않습니다.
- 최종 EXE에서 로딩 → 플레이 → 120청크 재사용 → 충돌 → 경찰 연출 → 결과 → 재시작 통과, **종료 코드 0**.
- 최종 EXE에서 12개 구간·화질 조합(갈림길·광산·감옥·동물원), 세 맵의 터널 출구·입구·첫 구간, 야외에서 광산으로 돌아오는 연결, 상단 사이렌·얼굴·엎드린 연출을 확인했습니다. 실제 화면 42장 촬영. 원시 수치와 테스트 XML은 docs/previews/Polish20261025에 있습니다. 빠르게 지나가는 모든 타일의 시간적 깜빡임을 자동 수치화한 검사는 아닙니다.

## 성능

NVIDIA GeForce GTX 1060 3GB, Direct3D 11, 1280×720. 각 조합 2초 준비 후 7초 측정, 해당 거리의 가속을 적용하고 실제 웹캠 추론 부하는 제외했습니다. 프레임 상한에 가까운 수치이며 최적화 전후 향상률이나 최대 처리 성능은 아닙니다.

| 화질 | 갈림길 FPS | 광산 FPS | 감옥 FPS | 동물원 FPS | 상한 |
| --- | ---: | ---: | ---: | ---: | ---: |
| 보통 | 59.7 | 59.4 | 59.8 | 59.8 | 60 |
| 높음 | 59.8 | 59.8 | 59.7 | 59.8 | 60 |
| 저사양 | 30.0 | 30.0 | 30.0 | 30.0 | 30 |

## 생성·수정 파일과 주요 함수

이번 새 파일: Presentation/Map/GroundSurfaceOwner.cs와 .meta, 새 검수 이미지와 검증 자료. 앞선 RouteMesh.cs, RouteEnvelopeView.cs, RouteTransitionView.cs도 포함합니다.

수정 파일과 핵심 함수:

- Map/GroundSurfaceOwner.Prepare/Restore, RuntimeContent.Decorate/Release: 교체 바닥의 단독 소유, 원본 바닥 높이 분리, 풀 반환 복구.
- Domain/RunRules.cs의 RunConfig/RunSession.IntervalSpeed, Content/Data/GameSettings.asset, RuntimeContent.Decorate: 8~12m/s 점진 가속, 수레 휴식 속도, 차단물 빈도 증가.
- Player/PolicePursuitCue.Configure/Update, UI/RunHud.DrawSiren/Draw, UI/HUDPresenter.PoliceAlert, Bootstrap/GameBootstrap.Awake/Update: 손·곤봉 경고 제거, 작은 사이렌 상태 전달, 선택 확정 시 캐릭터의 한 프레임 지연 방지.
- ArtV2/Textures/stone.png.meta, wood.png.meta, External/Staging/GeneratedV6/HandPaintedAtlas.png.meta: 삼중 선형 필터와 이방성 필터 강화.
- Domain/CartFork.Offset, RoutePlan.UpdateChoice/DestinationAt/CycleOffset, RouteSurface.Sample: 숨겨진 목적지와 합류 없는 경로.
- Domain/RoutePlan.ExtendedLap/RailChoiceLocked, RunnerController.Command/Tick/CartLean: 세 실제 목적지, 레일 잠금·하차 좌표 복원.
- Map/RouteMesh.Strip, RouteEnvelopeView.Build/Attach, RouteTransitionView.BuildShell/Attach: 닫힌 구조 면, 지형 하부, 맵 전환용 입구.
- Map/RailForkView.Section/BuildTunnels/RefreshSelection/PrewarmMeshes: 끊김 없는 단면과 미선택 출구 봉합, 재사용할 물리 데이터 사전 생성.
- Map/ChunkRenderBudget.Apply, WorldAtmosphere.LateUpdate, FirstPersonCameraRig.Configure, ForkSurface.shader: 보이지 않는 조명 제거와 생성 거리·안개·원거리 표시 정합성.
- Map/MuhanokMapChunk.PlaceOnRoute/UpdateCartRoute, ChunkSpawner.Tick/Evaluate: 선택한 경로의 청크·코인·소켓 연결 및 동적 바나나 판정.
- Map/RailForkView: 세 터널·암석 재질·목재 입구·랜턴, 메시 재사용.
- Map/RuntimeContent.Decorate/Release/BuildBlock/BuildBanana/CombineParts, ObstacleController.Evaluate: 맵별 차단물·껍질·풀·빈 차선.
- Player/PoliceCaptureSequence.Begin/Tick/ResetSequence, Bootstrap/GameBootstrap.StateChanged/Update/LateUpdate, FirstPersonCameraRig.FollowCapture: 경찰 접근·타격·3인칭·결과 흐름.
- Player/ProneCapturePose.Begin/Apply/ResetPose, FaceAvatarView.Apply/LateUpdate: 엎드린 자세·머리 사진 회전·자세 복원.
- Infrastructure/Pose/PoseInput의 기존 상대 차선 입력을 갈림길에서도 사용하도록 GameBootstrap의 모드 콜백과 RunnerController.Command를 연결했습니다.
- Player/CartRideView/FirstPersonHands, Audio/GameAudio.Bonk: 잡힘 동안 캐릭터 표시, 1인칭 손 숨김, 만화식 효과음.
- Infrastructure/KeyboardGameInput.Poll, UI/RunHud: 가운데 키와 사전 경로 선택 카드 제거.
- Domain/WorkoutEstimate.SetGoal(string), UI/SetupScreen.Draw: 목표 직접 입력·검증.
- ExpansionRulesTests/ExpansionPlayModeTests/GameSceneTests, Bootstrap/RuntimeExpansionReview, 배포 패키징 도구와 본 보고서.

기존 얼굴 DB·입력·게임 코드 변경 사항을 보존했고 Unity와 패키지 버전을 업그레이드하지 않았습니다.

## 남은 확인과 다음 작업

- 웹캠 연결 후 본인 얼굴 등록/로그인, 전신·상체 인식, 손 메뉴 사용감을 확인하는 작업은 사용자 요청으로 보류했습니다.
- 얼굴 로그인은 로컬 전시 프로필용이며 검증된 사진 위조 방지 기능은 없습니다.
- 이 PC에서는 오디오 출력 장치가 없어 실제 스피커 소리는 확인하지 못했습니다. 효과음 코드는 포함합니다.
- Development Build 종료 시 Unity GPU 버퍼·할당 정리 진단 경고가 남습니다. 최종 스모크 종료 코드는 0이며 장시간 반복 메모리 검증은 별도입니다.
- 다음 권장 작업은 웹캠 실물 조작 감도 확인과 사용자 플레이 피드백 반영입니다. GitHub 업로드는 계속 보류합니다.

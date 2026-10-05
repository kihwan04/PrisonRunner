# V9 실제 검수 · 2026-10-04

## 결과

- Blender 4.5.9: 실제 실행 PASS. 자체 원숭이 수레 그립 Humanoid FBX를 추가했다. 모델 2개, 자체 동작 17개, 기존 Mixamo 실제 동작 4개를 사용한다.
- Unity 6000.3.10f1 재생성/참조 검증 PASS. `Unity/Logs/ArtV9Upgrade.log`, `MUHANOK_V9_PASS`. 카트 경로 2개, 반쪽 철로 붕괴 4곳, ProBuilder 13개 원본/실행 베이크, 실제 스킨/표정/재질 검증을 포함한다.
- EditMode **42/42 PASS**, `Unity/Logs/MuhanokEditModeV9.xml`. 두 번 바나나 지연 추격, 일시정지/다시 하기, 반복 물웅덩이 생존, 카트 좌우 전환/좁은 이동, 지정 운동과 120개 경로 경계/화면 비율 검증.
- 최종 PlayMode **20/20 PASS**, 2026-10-04 06:54:42 UTC 완료. `Unity/Logs/MuhanokPlayModeV9.xml`. 실제 단선 청크에는 일반 장애물이 없고 올바른 기울이기로 양쪽 붕괴 모두 통과한다. 잘못 기울이면 BrokenRail 잡힘/추락이다. 실제 3레인 운동 행과 점프/숙이기/하이니, 레인 중간 틈으로 운동 우회 금지, 실제 수레/음향/Animator 상태와 카메라, 기존 입력/로딩/문/체중/표정도 검증한다.
- 계단 끝 문 PASS: 접근 시 내려옴, 통과 시 0.95m 틈, 숙이기로 통과, 서 있으면 즉사, 3m 지나면 바닥까지 완전히 닫힘, 풀 리셋 시 다시 열림. 계단 출구와 감옥 입구의 위치/높이가 일치한다.
- 잡힘 연출 PASS: 경찰이 플레이어를 바라보고 2m 이상 거리를 두도록 검증했다. 이전 실제 검수에서 발견한 카메라와 경찰 머리 겹침을 수정했다.
- 최종 Windows 빌드 PASS. 출력 총량 **239,715,731 bytes**, `Unity/Logs/ArtV9Build.log`. 실행 파일은 `Builds/Windows/Muhanok.exe`이며 옆 데이터/라이브러리 폴더를 함께 유지한다.

최종 실행 기록:

- 실제 GPU 캡처 **51장 PASS**. 1280×720: 시작/카트 8장, 10개 맵/연결부 20장, 문 내려옴/슬라이딩/감옥 진입 3장, 바나나 감속/잡힘/철로 추락/결과 4장. 1702×726/1024×768: 시작/카트 흐름 각 8장. PNG 비어 있음/픽셀 크기와 16:9 여백 검증은 `previews/ArtV9/capture-verification.json`.
- 실행 파일 smoke PASS. `Unity/Logs/ArtV9RuntimeSmoke.log`: loading intro → 직접 gameplay → floor slip → 120개 청크 순환/풀 유지 → 한 번 충돌 → result/retry.
- 최종 적용/테스트/빌드/실행 캡처/smoke에서 C# compile error, shader error 및 unhandled exception을 관찰하지 않았다. 최종 캡처에는 PlayerPrefs 저장 예외가 없으며 검수 모드가 개인 기록을 바꾸지 않는다.
- 실제 갤러리 `previews/ArtV9/index.html`의 이미지 참조 **39개 모두 유효**. 세 창 비율 전체 캡처 51장은 같은 폴더에 있다.
- GUI Unity GameScene Play 진입 `MUHANOK_EDITOR_READY`와 `MUHANOK_GAME_VIEW_FIT: 1280x720 / scale 0.6444445` 확인. 현재 창에 맞춰 전체 Game View를 축소해 표시하며 열린 상태로 둔다. `Unity/Logs/ArtV9Interactive.log`.
- 편집기의 Play 진입 전 Scene View에서 24개 point shadow map의 atlas 자동 축소 경고가 한 번 관찰됐다. Play 중에는 실제 가까운 두 점광원 그림자 예산이 적용된다. 이 편집기 경고는 C# 컴파일 실패와 구분한다.

## 실제 화면과 검수 방식

실행 파일의 3D 카메라와 HUD를 그대로 촬영한다. 지도 검수는 자동 입력으로 실제 점프/숙이기/하이니/철로 균형 판정을 통과한다. 장애물 판정은 비활성화하지 않는다. 렌더링한 사진을 합성/보정하거나 생성 이미지로 대체하지 않는다. 제공된 로딩 그림은 로딩/전환에 사용하며 `FirstLiveFrame`부터는 실제 3D다.

- `Player_CartRide`, `Player_CartBank`: 실제 원숭이와 수레, 좁은 곡선 다리, 아래 절벽, 깨진 레일과 안전 방향 표지.
- `Player_ClosingGate`: 계단 끝에서 내려오는 문과 뒤쪽 감옥 복도.
- `Player_SlideUnderGate`: 실제 숙이기 판정과 낮아진 1인칭 시점으로 문 아래를 통과한다.
- `Player_PrisonEntry`: 같은 길의 감옥 복도로 들어선 뒤 서 있는 시점.
- `Player_Slip`, `Player_BananaCatch`: 실제 바나나 MonoBehaviour 접촉 한 번/두 번과 감속/추격/잡힘. 검수에서 실제 아이템을 다시 준비해 두 번째 접촉을 재현한다.
- `Player_RailCrash`, `Player_GameOver`: 실제 철로 판정에 잘못 기울여 추락하고 결과가 표시된다.
- 맵 10개와 각 연결부 20장: 경로 중심/높이가 연속인 실제 실행 상태.

검수 모드는 개인 최고 기록을 저장하지 않는다. 정상 게임에서는 기록 저장을 유지한다. 16:9, 넓은 창, 4:3 창에서 게임 화면과 UI의 비율/여백을 검사한다. 그림의 픽셀 비율만으로 상용 그래픽 완성도나 GPU 프레임률을 인증하지 않는다.

## 제한과 다음 작업

무료 BitGem 경찰은 기존 정상 다운로드 인증 문제로 원본 미임포트다. 화면의 경찰은 자체 모델이다. 무료 공식 주문의 필수 개인정보는 사용자 직접 처리 대기이며 이번 작업에서 주문하거나 계정 정보를 대신 입력하지 않았다.

아트는 전용 경로, 실제 모델/조명/재질/애니메이션을 보강했지만 상용 benchmark와 동일한 전체 품질에는 미달이다. 다음 추천은 캐릭터 실루엣/표정/접지와 맵별 고유 장식의 추가 제작이다. 개인별 웹캠 운동 인식과 칼로리 추정 정확도 검증은 별도로 남는다. Higgsfield는 연결 도구가 없어 실행하지 않았다.

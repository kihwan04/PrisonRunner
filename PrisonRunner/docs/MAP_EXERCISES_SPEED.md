# 맵별 운동과 점진 가속

2026년 10월 5일. V10 맵과 캐릭터 위에 플레이 기능만 추가한다.

## 플레이

- Space / 몸 점프: 점프. 바닥에서 점프하면 진행 중인 슬라이드를 해제한다. 공중에서 연속 점프는 불가능하다.
- S / ↓ / 몸 숙이기: 슬라이드. 키보드는 누르는 동안 유지되고 몸동작은 기존 CROUCH 입력을 사용한다.
- A/D / 좌우 방향키: 레인 변경. 수레에서는 기존 좌우 기울이기로 철로 붕괴를 피한다.
- W / ↑ / 하이니: 계단 운동. 점프만으로 하이니 판정을 대신하지 않는다.

11개 도보 청크에 점프 장애물과 슬라이드 장애물을 배치했다. 일반 도보 청크는 로컬 Z7의 낮은 상자와 Z19의 낮은 파이프를 세 레인에 배치한다. 첫 광산 진입에는 점프 행을 생략해 준비 시간을 준다. 이후 광산 방문부터 두 동작을 모두 사용한다. 계단은 Z3 점프, Z12 하이니, Z19 기존 닫히는 문 아래 슬라이드 순서다. 감옥 타입 4/5/6의 완전히 닫히는 문은 Z23에서 한 레인만 막고 다른 두 레인을 남긴다. 기울이기를 사용하는 두 수레 청크에는 도보 장애물을 넣지 않는다.

도보와 수레는 기본 9m/s에서 시작해 실제 이동 거리 120m당 1m/s씩 연속으로 가속한다. 480m에서 13m/s, 960m부터 최대 17m/s다. 바나나·물웅덩이 감속은 기존처럼 2.4초간 50%이며 진행 거리와 가속은 실제 이동량을 따른다. 일시정지는 진행을 멈추고 재시작은 기본 속도로 복귀한다. HUD에 현재 속도를 표시하고 다음 운동 안내는 속도에 맞춰 최대 약 1.35초 앞에서 표시한다.

## 변경 파일

추가: `Unity/Assets/Game/Editor/MuhanokGameplayUpgrade.cs`와 `.meta`, 이 문서.

수정: `Domain/RunRules.cs`, `Content/Data/GameSettings.asset`, `Presentation/Map/MuhanokMapChunk.cs`, `ChunkSpawner.cs`, `Presentation/UI/HUDPresenter.cs`, `HUDPresenter.Start.cs`, `Editor/MuhanokArtUpgrade.Maps.cs`, `Tests/EditMode/RunRulesTests.cs`, `Tests/PlayMode/CollisionAndCartTests.cs`, `StartAndGateTests.cs`. `Content/Prefabs`의 기존 맵 13개에는 운동 배열을 저장했다. README와 AGENTS.md에도 현재 기능을 기록한다.

Unity 메뉴 `Muhanok/Apply Map Exercises and Progressive Speed`는 기존 환경·캐릭터를 보존하며 운동 장애물만 다시 배치한다. 전체 아트 재생성 경로도 같은 설정을 호출한다. 실행 파일은 `Builds/Windows/Muhanok.exe`다.

## 검증

Unity 6000.3.10f1에서 최종 EditMode 49개와 PlayMode 23개를 모두 통과했다. 실패 0개, C# 컴파일 오류 0개다. 결과는 `Unity/Logs/GameplayEditModeFinal.xml`, `GameplayPlayModeFinal.xml`이다. `GameplayUpgrade.log`의 `MUHANOK_GAMEPLAY_UPGRADE_PASS`로 실제 맵 저장도 확인했다.

최종 Windows 빌드는 `GameplayBuildFinal.log`의 `MUHANOK_BUILD_PASS`로 성공을 확인했다. 실행 파일을 갱신했으며 `GameplayRuntimeSmoke.log`의 `MUHANOK_RUNTIME_SMOKE_PASS`로 로딩 후 직접 플레이, 바닥 감속, 120개 청크 재사용, 한 번 충돌 종료, 결과 화면과 재시작을 확인했다. 실행 검증 종료 코드는 0이다.

최고 속도 검증은 실제 풀링된 맵에서 두 바퀴를 30fps로 진행하며 점프·슬라이드·하이니·문 회피·수레 기울이기를 평가한다. 최고 속도에서도 통과 가능하며 생성한 청크 개수는 증가하지 않았다. 바닥 아이템과 실제 웹캠 동작의 정확도는 이 최고 속도 자동 조작 테스트 범위에 포함하지 않는다. 기존 바닥 아이템, 바나나 누적 추격, 충돌 종료와 재시작은 별도 기존 테스트를 함께 통과했다. 현재 남은 기능 오류는 자동 테스트에서 발견되지 않았으며, 실제 몸동작 난이도와 시각적인 운동 타이밍 검수는 남아 있다.

다음 추천 작업: 키보드 및 실제 몸동작 플레이로 최대 속도와 운동 안내 시간을 사용자 체력에 맞게 조정한다.

# 동물원 장애물: 이어진 목·코와 쓰러지는 나무

기준일: 2026-10-26. 기존 HUD·손 메뉴·중앙 서버 연결을 유지한 로컬 변경입니다. 이번 작업에서 Git 커밋·푸시·새 공개 릴리스 게시를 하지 않았습니다.

## 변경한 모습과 조작

- **기린**: 낮게 떨어져 있던 목 회전축과 분리된 막대를 제거했습니다. 몸통 어깨 → 휘어지는 목 → 머리로 이어지는 메시를 만들고 무늬·귀·눈·뿔·다리·꼬리를 함께 표시합니다. 다가오면 목을 숙여 낮게 길을 가로막습니다.
- **코끼리**: 머리 아래에서 떨어져 시작하던 코를 머리의 코 뿌리에 붙였습니다. 귀·눈·상아·몸통·다리와 이어진 곡선 코를 표시하며, 가까워지면 코를 길로 뻗습니다. 끝은 둥글게 처리했습니다.
- **나무**: 길 옆 뿌리를 축으로 서 있던 나무가 기울어지고, 낮게 쓰러져 길을 막습니다. 나무껍질·노출된 뿌리·가지·수관을 함께 표시합니다.
- 세 종류는 동물원의 기존 차단물과 겹치지 않는 비어 있는 청크에 나누어 등장합니다. 32~14m 접근 구간에서 준비 동작을 보여주고, 충돌 위치에서는 낮은 장애물이 완성됩니다. 세 차선 점프로 넘는 기존 동물원 운동 규칙을 사용합니다. 서 있으면 충돌하고 점프하면 통과합니다.

사진과 외부 3D 모델을 새로 내려받지 않았습니다. 단순화한 동물 형태·연결 메시·기린 무늬 텍스처는 코드로 생성했습니다. 실제 동물의 해부 구조나 상용 게임 아트 품질을 재현했다고 주장하지 않습니다.

## 구조와 비용

목과 코는 두 개의 연속된 곡선을 고정 크기 정점/법선 버퍼에 반영합니다. 어깨·코 뿌리와의 연결점은 변하지 않습니다. 같은 자세에서는 메시 갱신을 건너뛰고, 매 프레임 새 배열이나 메시를 만들지 않습니다. 정적인 몸·머리·나무 부품은 같은 재질끼리 합칩니다.

동물은 기존 풀을 사용하고 나무도 미리 네 개를 만들어 재사용합니다. 풀에 반환할 때 목·코·나무의 준비 자세를 복원합니다. 동물 표면 메시와 게임 충돌 규칙은 분리합니다. 원래 씬과 프리팹을 다시 생성하지 않았습니다.

핵심 소스: [동물 생성](../../Unity/Assets/Game/Presentation/Map/RuntimeContent.Animals.cs), [연결 메시](../../Unity/Assets/Game/Presentation/Map/AnimalReachMesh.cs), [배치·재사용](../../Unity/Assets/Game/Presentation/Map/RuntimeContent.cs), [나무 동작](../../Unity/Assets/Game/Presentation/Map/FallenTreeHazard.cs), [씬 검증](../../Unity/Assets/Game/Tests/PlayMode/ZooEncounterTests.cs), [EXE 화면 검수](../../Unity/Assets/Game/Bootstrap/RuntimeZooReview.cs). 새 소스는 대응 .meta와 함께 관리합니다.

## 이번 검증

Unity PlayMode **41/41**, 실패 0. 새 테스트는 목/코의 전 구간 연결과 곡선 이음, 세 종류 등장과 원본 장애물 중복 방지, 나무 준비→낙하→점프/충돌→재사용 초기화를 확인합니다. Windows 빌드·12장 EXE 화면 검수·기본 스모크 종료 코드는 모두 0이며 컴파일 오류는 없었습니다. 기존 맵 코드의 경고는 남습니다.

실제 EXE에서 세 종류를 모두 찾았고 연결점 오차 최대값은 **0.0000m**입니다. 검수는 NVIDIA GeForce GTX 1060 3GB, 1280×720, 키보드 기준입니다. 이번에는 FPS 벤치마크를 새로 수행하지 않았습니다.

- [기린 플레이 시점](../previews/ZooReach20261026/Giraffe_Blocking.png) · [기린 연결 옆 시점](../previews/ZooReach20261026/Giraffe_Connected_Side.png)
- [코끼리 플레이 시점](../previews/ZooReach20261026/Elephant_Blocking.png) · [코끼리 연결 옆 시점](../previews/ZooReach20261026/Elephant_Connected_Side.png)
- [기울어지는 나무](../previews/ZooReach20261026/FallingTree_Moving.png) · [쓰러진 나무](../previews/ZooReach20261026/FallingTree_Blocking.png)
- [검증 원시 자료](../previews/ZooReach20261026/review.json) · [검증 요약](../previews/ZooReach20261026/validation.json)

실물 웹캠·몸동작 감도·여러 GPU·장시간 플레이를 이번 작업에서 새로 측정하지 않았습니다. 앞선 128개 EditMode 결과는 이전 HUD/손 메뉴 작업의 기록이며 이번 작업에서는 다시 실행하지 않았습니다. 기존 자동 전체 메뉴 캡처 종료 오류와 Unity 종료 메모리 정리 진단도 해결 완료로 표시하지 않습니다.

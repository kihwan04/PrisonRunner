# V5 검증 기록

2026-10-03 구현, 2026-10-04 정리. 현재 작업 버전은 V6이며 이 문서는 V5의 완료한 검사만 기록한다.

- EditMode 34/34 PASS: 게임 규칙, 경로의 X/Y/Z 경계, 선택 몸무게/입력 범위/활동 시간/칼로리 계산.
- PlayMode 12/12 PASS: 기존 실제 Mixamo 동작, 게임 흐름, UDP, 코인, 연속 경로와 선택 입력·로딩·일시정지·문 재사용. 로딩 텍스처의 실제 1672×941 크기까지 검사한다.
- Windows 개발 빌드 PASS: 243,715,491 bytes. 13개 ProBuilder 원본/베이크 정점, 두 유효 자체 Humanoid, 실제 Mixamo 상태 연결 8개, 프리팹 31개와 장면/Timeline 참조 검증.
- 실제 Player 캡처 38장 PASS: 1280×720에서 흐름 6장/맵 10장/연결 10장, 1702×726과 1024×768에서 각각 흐름 6장. 빈 화면/크기/16:9 검은 여백을 픽셀 검사했다.
- 실제 runtime smoke PASS: title → 8초 Timeline → FOV 80 1인칭 → 120개 청크 풀링 → caught → result → retry.
- 최종 컴파일 오류 없음. 최초 검사에서 제외한 옛 경로를 찾던 조건을 최신 순서로 수정했다. 로딩 기본 임포트가 원본을 2의 거듭제곱으로 변형하던 문제를 해결하고 장식 막대를 실제 진행률로 덮었다.
- Point shadow atlas가 일부 구간에서 자동으로 해상도를 낮추는 경고가 남아 V6에서 예산 관리로 보완한다. 그래픽이 참고 이미지와 동일하다는 판정은 하지 않는다. 실제 웹캠 사용자 운동은 별도 수동 검증 필요.

로그: Unity/Logs/MuhanokEditModeV5.xml, MuhanokPlayModeV5.xml, ArtV5LoadingFix.log, ArtV5Build.log, ArtV5PlayerCapture.log, ArtV5WideCapture.log, ArtV5FourThreeCapture.log, ArtV5RuntimeSmoke.log.

화면: docs/previews/ArtV5/index.html 및 capture-verification.json.

생성/수정 파일과 기능: [ART_V5](ART_V5.md). 다음 작업은 최신 V6 문서를 따른다.

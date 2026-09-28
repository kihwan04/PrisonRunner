# AGENTS.md

## 프로젝트
- Unity 기반 3레인 Endless Runner 캡스톤 프로젝트다.
- 배경은 Stylized 3D 감옥이다.
- 웹캠 + Python + MediaPipe Pose로 몸동작을 인식한다.
- 인식 결과를 Unity 캐릭터 조작으로 연결한다.
- 구현은 프로젝트 개발 규칙을 따른다.
- 기능 완성을 그래픽 완성보다 우선한다.

## 반드시 읽을 문서
- docs/DEVELOPMENT_PLAN.md
- docs/ARCHITECTURE.md
- docs/ART_DIRECTION.md

## 개발 원칙
- 작업 전 기존 코드를 먼저 확인한다.
- 요청하지 않은 전체 리팩터링을 하지 않는다.
- 정상 동작하는 기능을 이유 없이 수정하지 않는다.
- 한 번에 하나의 기능 단위로 개발한다.
- Placeholder로 먼저 기능을 검증한다.
- 과도한 추상화를 하지 않는다.
- 컴파일 가능한 상태를 유지한다.

## 게임
- 캐릭터는 자동으로 전진한다.
- LEFT / RIGHT는 레인 이동이다.
- JUMP는 점프다.
- CROUCH는 숙이기다.
- HIGH_KNEE는 계단 동작이다.
- IDLE은 기본 달리기다.
- 기본적으로 3개의 레인을 유지한다.
- 충돌 시 속도 감소 또는 Stumble을 적용한다.

## 아키텍처
- Clean Architecture Lite를 사용한다.
- Core는 순수 게임 규칙을 담당한다.
- Application은 게임 흐름을 담당한다.
- Infrastructure는 외부 시스템을 담당한다.
- Presentation은 Unity 표현 계층이다.
- 자세한 구조는 docs/ARCHITECTURE.md를 따른다.

## 입력
- 게임 로직은 Keyboard나 MediaPipe를 직접 참조하지 않는다.
- 모든 입력은 공통 입력 인터페이스를 통해 전달한다.
- KeyboardInputProvider를 항상 유지한다.
- PoseInputProvider는 별도로 구현한다.
- Pose 입력이 실패해도 키보드 플레이가 가능해야 한다.

## MediaPipe
- Python이 Webcam 및 Pose 분석을 담당한다.
- Unity는 최종 Action만 전달받는다.
- 허용 Action은 IDLE LEFT RIGHT JUMP CROUCH HIGH_KNEE다.
- confidence와 timestamp를 함께 전달한다.
- Pose 인식 실패 시 IDLE로 처리한다.

## Unity
- MonoBehaviour 하나에 여러 책임을 몰아넣지 않는다.
- Player 입력 이동 애니메이션 점수 맵 생성을 분리한다.
- 설정값은 가능한 ScriptableObject로 관리한다.
- Visual Mesh와 Collider를 분리한다.
- 모델 교체가 게임 로직에 영향을 주면 안 된다.

## Endless Map
- MapChunk 기반 구조를 사용한다.
- Chunk마다 EntrySocket과 ExitSocket을 가진다.
- 장애물 위치는 ObstacleSocket으로 관리한다.
- 모든 Chunk는 동일한 Lane 규격을 사용한다.
- 지나간 Chunk와 장애물은 Object Pooling으로 재사용한다.
- 피할 수 없는 장애물 패턴을 생성하지 않는다.

## Art
- 모든 그래픽은 docs/ART_DIRECTION.md를 따른다.
- Reference 이미지는 References/ 폴더에서 관리한다.
- Stylized 3D 스타일을 유지한다.
- 게임 가독성을 장식보다 우선한다.

## 외부 에셋
- AI 생성 에셋은 바로 Production에 넣지 않는다.
- 먼저 Staging에서 확인한다.
- 3D Model은 크기 Pivot Material Collider를 검수한다.
- API Key와 .env는 Git에 올리지 않는다.

## Git
- main은 발표 가능한 안정 버전이다.
- develop은 통합 개발 브랜치다.
- feature/*에서 개별 기능을 개발한다.
- 기능 완료 후 develop으로 병합한다.
- 발표 가능한 시점에 develop을 main으로 병합한다.
- 하나의 Commit은 하나의 목적만 가진다.
- 브랜치명과 커밋 메시지는 기능과 변경 목적만 표현한다.
- 작성 도구명이나 생성 주체에 대한 문구를 넣지 않는다.
- 자세한 작업 흐름은 docs/GIT_WORKFLOW.md를 따른다.

## 금지
- Unity 버전을 임의로 변경하지 않는다.
- 필요 없는 Package를 설치하지 않는다.
- API Key를 코드에 작성하지 않는다.
- Library Temp Logs Build를 Git에 Commit하지 않는다.
- Scene 전체를 이유 없이 다시 만들지 않는다.

## 작업 종료 보고
작업 완료 후 반드시 다음을 보고한다.
1. 생성한 파일
2. 수정한 파일
3. 구현한 기능
4. 테스트 결과
5. Compile Error 여부
6. 남은 문제
7. 다음 추천 작업

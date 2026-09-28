# PrisonRunner

Unity 기반 3레인 감옥 테마 Endless Runner 캡스톤 프로젝트입니다. 키보드 조작을 먼저 완성하고, Python MediaPipe Pose에서 인식한 동작을 WebSocket으로 Unity에 전달합니다.

## 현재 상태

Phase 1 러너 조작과 Phase 2 무한 맵을 포함한 Unity 6.3.10f1 URP 프로젝트와 키보드 플레이 장면이 있습니다. `PoseServer/`와 최종 아트 에셋은 아직 비어 있습니다.

## 실행

1. Unity Hub에서 `Unity/` 폴더를 Unity 6000.3.10f1로 엽니다.
2. 패키지 임포트와 스크립트 컴파일이 끝나면 `Assets/Game/Scenes/RunnerMVP.unity`를 엽니다.
3. Play를 누릅니다. Ground, Capsule Player, Cube 장애물은 장면의 `Runner MVP Bootstrap`이 플레이 시작 시 생성합니다.

| 입력 | 동작 |
| --- | --- |
| A / ← | 왼쪽 레인으로 한 칸 이동 |
| D / → | 오른쪽 레인으로 한 칸 이동 |
| Space | 점프 |
| S / ↓ 누르고 있기 | 숙이기 |

전진은 자동입니다. 장애물에 부딪히면 잠시 느려집니다. 점수는 실제 전진 거리(m)입니다. 맵은 세 종류의 Placeholder Chunk를 앞에 생성하고 지나간 Chunk와 장애물을 재사용합니다.

## 구조

- `docs/`: 개발 계획, 아키텍처, 아트 방향
- `References/`: 아트 참조 이미지
- `Unity/`: Unity 프로젝트
- `PoseServer/`: 웹캠 및 자세 인식 서버
- `Tools/AssetPipeline/`: 에셋 준비 도구

구현 순서와 완료 기준은 [개발 계획](docs/DEVELOPMENT_PLAN.md)을 참조하세요. 코드 작성 전 [작업 규칙](AGENTS.md)과 [아키텍처](docs/ARCHITECTURE.md)를 확인하세요.

## Git 작업 흐름

`main`은 발표용 버전, `develop`은 기능 통합 브랜치입니다. 개별 기능은 `feature/*`에서 작업합니다. 현재 구현은 `develop`에 통합하며, 발표 검증 후 `main`에 반영합니다. 자세한 규칙은 [Git 작업 흐름](docs/GIT_WORKFLOW.md)을 참조하세요.

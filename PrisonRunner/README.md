# PrisonRunner

Unity 기반 3레인 감옥 테마 Endless Runner 캡스톤 프로젝트입니다. 키보드 조작을 먼저 완성하고, Python MediaPipe Pose에서 인식한 동작을 Unity에 전달합니다.

## 초기 구조

- `docs/`: 개발 계획, 아키텍처, 아트 방향과 Git 작업 흐름
- `References/`: 아트 참조 이미지
- `Unity/`: Unity 프로젝트
- `PoseServer/`: 웹캠 및 자세 인식 서버
- `Tools/AssetPipeline/`: 에셋 준비 도구

최초 `main`은 문서와 기본 구조만 포함합니다. 구현은 기능 브랜치에서 작업해 `develop`에 통합하고 발표 검증을 마친 뒤 `main`에 반영합니다.

작업 전 [작업 규칙](AGENTS.md), [개발 계획](docs/DEVELOPMENT_PLAN.md), [아키텍처](docs/ARCHITECTURE.md), [아트 방향](docs/ART_DIRECTION.md)을 확인하세요. 브랜치와 커밋 규칙은 [Git 작업 흐름](docs/GIT_WORKFLOW.md)을 따릅니다.

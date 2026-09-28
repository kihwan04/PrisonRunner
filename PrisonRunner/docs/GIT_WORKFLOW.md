# Git 작업 흐름

## 브랜치 역할

| 브랜치 | 역할 | 병합 대상 |
| --- | --- | --- |
| `main` | 검증을 마친 발표용 버전. 최초에는 프로젝트 문서와 기본 구조만 보관한다. | 발표 검증 후 `develop`에서 병합 |
| `develop` | 기능 통합과 다음 발표 버전 준비 | `main` |
| `feature/unity-setup` | Unity 버전, URP, 패키지와 프로젝트 설정 | `develop` |
| `feature/runner-controls` | 공통 입력, 키보드 조작, 이동, 충돌, 점수와 카메라 | `develop` |
| `feature/endless-map` | 맵 청크, 장애물 소켓, 재사용 풀과 플레이 장면 통합 | `develop` |

완료된 기능 브랜치는 이번 초기 이력을 확인할 수 있도록 로컬에 유지한다. 새 기능은 최신 `develop`에서 별도의 `feature/<기능명>` 브랜치를 생성한다. 아직 착수하지 않은 기능의 브랜치는 미리 만들지 않는다.

## 작업 순서

1. `develop`에서 기능 브랜치를 생성한다.
2. 의존하는 기능이 먼저 `develop`에 통합되었는지 확인한다.
3. 목적별로 커밋하고 해당 기능을 검증한다.
4. `develop`에서 `git merge --no-ff feature/<기능명>`으로 통합한다.
5. 플레이, 빌드와 발표 시나리오를 검증한 뒤 `develop`을 `main`에 병합한다.

Unity 설정 → 러너 조작 → 무한 맵 순으로 통합한다. 맵을 참조하는 장면 초기화 코드와 장면의 빌드 등록은 무한 맵 브랜치에 함께 넣어 중간 커밋에 누락된 의존성이 생기지 않게 한다.

## 커밋 규칙

`<유형>(<범위>): <변경 목적>` 형식을 사용한다. 유형은 `feat`, `fix`, `chore`, `docs`, `refactor`, `test` 중 변경에 맞는 것을 선택한다.

```text
chore(project): initialize project structure and development documentation
chore(unity): configure Unity project and render pipeline
feat(runner): add keyboard controls and runner components
feat(map): add pooled endless map and playable scene
```

브랜치명과 메시지에는 기능과 변경 목적만 적는다. 작성 도구명, 생성 주체 문구와 자동 서명은 넣지 않는다. Unity의 `Library`, `Temp`, `Logs`, 빌드 산출물과 비밀 설정은 추적하지 않는다. 소스와 대응하는 `.meta` 파일은 함께 커밋한다.

## 원격 저장소

원격 주소가 등록된 뒤 브랜치를 업로드한다. `main`과 `develop`은 보호 브랜치로 운영하고, 기능 통합 시 리뷰와 필요한 검증을 거친다. 원격 주소가 없는 상태에서는 로컬 브랜치와 이력만 관리한다.

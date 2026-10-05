# 공개 배포 검수 (2026-10-05)

## 생성·정리

저장소 루트 `README.md`, `StartGame.cmd`, `.gitignore`를 추가했다. 프로젝트에 `START_HERE.md`, `StartGame.cmd`, `THIRD_PARTY_NOTICES.md`, `docs/README.md`, `PoseServer/SetupPose.cmd`, 배포 생성·검증·게시 스크립트 세 개를 추가했다. 기존 프로젝트 README를 최신 실행 기준으로 다시 작성하고 이전 내용을 `docs/legacy/README_BEFORE_PUBLIC_RELEASE.md`에 보존했다. 기존 V10~V12 기획서·과거 캡처·Unity 씬과 제작 원본의 경로는 유지한다.

## 공개 소스 범위

Unity Assets와 대응 meta, Packages, ProjectSettings, Python 입력 소스, 제작 도구, 기획서·검수·캡처, 프로젝트에서 제작한 Blender 원본을 공개한다. Library·Temp·Logs·UserSettings·가상환경·실행 빌드·다운로드 캐시·Blender 자동 백업·로컬 모델·비밀 설정은 Git에서 제외한다. 게임 실행 파일은 GitHub Releases ZIP으로 전달한다.

Mixamo 원본 FBX와 대응 meta는 공개에서 제외한다. 다운로드 설정과 해시를 기록한 Provenance.json은 보존한다. `MuhanokMixamoImport`는 누락된 override만 자체 Blender 동작으로 자동 복구하며 기존 연결이 있으면 유지한다. PlayMode의 동작 검증은 실제 사용 가능 리소스에 맞춰 Mixamo 또는 자체 FBX 경로를 확인한다.

추적·공개 후보 및 전체 Git 이력의 파일 목록에서 비밀 설정과 Mixamo 원본이 추적된 기록이 없음을 확인했다. 공개 소스 텍스트에서 일반적인 GitHub/OpenAI 토큰과 개인 키 표식을 검색했으며 발견되지 않았다. 단일 파일 최대 크기는 약 20.5MB로 GitHub의 100MB 파일 제한보다 작다.

## 검증

- 공개에서 제외하는 Mixamo 원본 4개와 meta를 작업 공간 내 별도 백업에 잠시 옮겨 실제 공개 소스 조건으로 검증했다.
- Unity 6000.3.10f1의 `ValidatePublicSource`: 누락 override 8개 복구, Humanoid 연결 총 18개 검증 통과.
- 해당 환경에서 EditMode **49/49**, PlayMode **23/23**, 실패 0개. C# 컴파일 오류 0개.
- 검증 후 로컬 Mixamo 파일과 기존 overrideController 원본을 모두 복원했다. 기존 게임 빌드의 애니메이션은 유지한다.
- PoseServer 단위 테스트 **4/4** 통과.
- 실행 ZIP을 직접 압축 해제한 게임에서 `--muhanok-smoke` 종료 코드 0, `MUHANOK_RUNTIME_SMOKE_PASS` 확인. 로딩 후 플레이, 바닥 감속, 120개 청크 재사용, 한 번 충돌 종료, 결과 화면과 재시작을 검증한다.
- 실행·기획 ZIP의 CRC 무결성 및 파일 SHA256 목록을 생성한다. 게임 ZIP에서 개발 디버그 폴더 DoNotShip을 제외하고 exe 인접 필수 라이브러리·데이터 폴더를 포함한다.

검증 로그와 XML은 로컬 `Distribution/`에 보관하며 공개 소스에는 포함하지 않는다. 이번 실행에서는 처음 Python 테스트를 저장소 루트에서 호출해 import 경로 오류가 났으며 PoseServer 작업 폴더에서 재실행하여 4개 모두 통과했다.

## 남은 점과 다음 작업

실제 웹캠 몸동작 정확도와 사용자별 최고 속도 난이도는 현장 검수가 남아 있다. 공개 소스 기본 동작은 자체 제작 애니메이션이므로 실행용 게임의 Mixamo 동작과 모습이 다를 수 있다. 다음 검수는 다른 Windows PC에서 ZIP을 내려받아 키보드와 웹캠을 각각 플레이하는 것이다.

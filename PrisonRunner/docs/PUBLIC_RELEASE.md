무한옥 Windows 데모와 최신 화면별 기획서를 공개합니다. 게임은 V10 아트 + 2026-10-05 운동·가속 구현, 기획서는 V13입니다.

## 받을 파일

| 파일 | 용도 |
| --- | --- |
| **Muhanok-Windows-x64.zip** | 바로 플레이하는 Windows 64비트 게임. Unity 설치 불필요 |
| **Muhanok-Planning-V13.zip** | 최신 화면별 게임기획서 V13 DOCX와 문서 검수 기록 |
| **SHA256SUMS.txt** | 다운로드 파일 무결성 확인용 해시 |
| GitHub 자동 생성 Source code | Unity 개발 소스. 실행 파일은 위 Windows ZIP에 있음 |

## 실행

Windows ZIP 전체를 압축 해제하고 **StartGame.cmd**를 더블클릭하세요. 게임 화면에서 **게임 시작 → 입력 없이 시작**을 누르면 됩니다. 몸무게는 원하면 입력합니다. `Game` 폴더 전체를 유지하세요.

A/D 또는 좌우 방향키는 이동, Space는 점프, S 또는 ↓는 숙이기, W 또는 ↑는 하이니입니다. 수레에서 A/D는 좌우 기울이기입니다. 키보드 플레이에는 Python과 웹캠이 필요 없습니다.

웹캠 플레이는 Python 3.11 또는 3.12 설치 후 **PoseServer/SetupPose.cmd → PoseServer/StartPose.cmd** 순서입니다. 전신을 보이게 서서 초기 보정을 기다리세요.

## 개발 소스

Unity **6000.3.10f1**로 `PrisonRunner/Unity`를 열고, 임포트·컴파일 완료 후 **Muhanok → Play GameScene**을 누릅니다. 공개 소스에서는 Mixamo 원본 FBX를 제외했으며 누락된 동작을 자체 Blender 동작으로 자동 연결합니다. 실행용 ZIP에는 기존 Mixamo 게임 동작이 들어 있습니다. 출처와 직접 복원 방법은 `PrisonRunner/THIRD_PARTY_NOTICES.md`에 있습니다.

## 확인한 범위

- 공개 소스 환경에서 자체 애니메이션 연결 18개 유효성 검사 통과.
- Unity EditMode 49개, PlayMode 23개 통과. C# 컴파일 오류 0개.
- Python 몸동작 입력 단위 테스트 4개 통과.
- 배포 ZIP 압축 해제 후 게임 로딩·플레이·감속·120개 청크 재사용·충돌 종료·결과·재시작 자동 검증 통과.
- ZIP 압축 무결성 검사 및 SHA256 기록.

실제 웹캠 인식 정확도와 최고 속도 체감 난이도는 현장 검수가 남아 있습니다. 상용 게임 수준의 최종 아트 완성 버전은 아니며 무료 경찰 패키지는 미임포트 상태로 자체 경찰 모델을 사용합니다.

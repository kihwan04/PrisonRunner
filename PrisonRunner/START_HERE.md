# Mongscape 처음 실행하기

기준일: **2026-10-26**. 이전 이름은 무한옥입니다. 현재 Windows 클라이언트는 중앙 얼굴 서버 주소 설정을 지원합니다.

## 이 PC에서 실행

저장소 맨 위 또는 이 폴더의 `StartGame.cmd`를 실행합니다. 완료된 `Builds/Mongscape/Windows/Mongscape.exe`를 먼저 열고, 빌드가 없으면 현재 소스를 Unity에서 실행합니다.

## 다른 PC에서 실행

[최신 Windows 게임 ZIP](https://github.com/kihwan04/PrisonRunner/releases/download/v2026.10.26-r2/Mongscape-RemoteClient-Windows-x64.zip)을 내려받아 전부 압축 해제하고 `StartGame.cmd`를 실행합니다. `Game` 폴더 전체를 보관하세요. Unity·Python·Docker를 게임 PC에 따로 설치할 필요는 없습니다.

타이틀 → 메인 메뉴 → 게임 시작 → 최초 PC 프로필 입력 → 탈출 준비에서 플레이 방식·칼로리 목표 선택 → 시작하기 순서입니다. 메인 메뉴에는 상점·거리 랭킹·프로필·얼굴 로그인도 있습니다. 얼굴 등록/로그인은 선택 사항이고 몸무게를 생략해도 플레이할 수 있습니다.

| 키 | 동작 |
| --- | --- |
| A / ←, D / → | 레인 이동. 일반 단선 수레에서는 기울이기 |
| C | 수레 갈림길 선택 구간에서 가운데 차선 |
| Space | 점프 |
| S / ↓ | 숙이기 |
| W / ↑ | 하이니 |
| F1 / F3 | 설정 / 성능 표시 |

전신·상체 조작과 자세 보정은 [몸동작 안내](docs/BODY_TRACKING.md)를 확인하세요.

## 얼굴 로그인 서버 선택

| 방식 | 준비 |
| --- | --- |
| 얼굴 없이 플레이 | 키보드 또는 몸동작 방식을 선택해 바로 시작 |
| 여러 PC에서 기록 공유 | PostgreSQL/Docker 서버를 먼저 배포하고 각 게임의 EXE 옆 `identity-server.json`에 같은 HTTPS 주소 지정 |
| 기존 로컬 얼굴 서버 | 개발 폴더에서 `SetupIdentity.cmd`로 준비한 뒤 `StartIdentity.cmd` 실행 |

중앙 서버 설정 예시입니다. **실제 서버 주소로 바꾼 뒤** 사용합니다.

```json
{
  "url": "https://faces.your-domain.com"
}
```

ZIP에서는 `Game/identity-server.json`, 개발용 Unity에서는 `Unity/identity-server.json`입니다. 환경 변수 `MONGSCAPE_IDENTITY_URL`이 있으면 파일보다 우선합니다. 주소 변경 후 게임을 다시 실행하세요.

얼굴 사진·특징·이름·운동 기록은 연결된 서버 DB에 저장됩니다. 원격 서버를 지정하면 로컬 Python 서버를 추가로 시작하지 않습니다. 서버·도메인은 아직 준비하지 않았으므로 [Docker 배포 안내](Services/Identity/README_DOCKER.md)에 따라 구성해야 합니다.

2026-10-25의 이전 로컬 ZIP은 localhost 전용입니다. 이 ZIP에 설정 파일만 추가해서 원격 연결을 사용할 수는 없습니다. 이전 릴리스의 `Muhanok.exe`는 현재 실행 대상이 아닙니다.

## 개발·확인 범위

소스 편집은 Unity Hub에서 `Unity/`를 **6000.3.10f1**로 엽니다. 자세한 구조는 [기술 스택](docs/TECH_STACK.md), 빌드·테스트와 남은 확인 사항은 [진행 상태](docs/PROGRESS.md)를 확인하세요.

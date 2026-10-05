# Blender · ProBuilder · Mixamo V4 검증

2026-10-03. 실제 설치 Unity 6000.3.10f1 / URP 17.0.1 / ProBuilder 6.1.2 / Blender 4.5.9 LTS에서 실행한다. 완료된 검사와 아직 미실행인 외부 서비스 작업을 구분한다.

## 완료한 검사

- Blender 제작 스크립트 2개 실제 실행 성공. 캐릭터 스킨 메시/뼈대/자체 동작 16개, 무골격 T-pose, 환경 소품 8개 FBX와 `.blend` 원본 저장.
- 두 캐릭터 Avatar `isValid`, `isHuman` 통과. 뼈대 정지 파일만 확인한 것이 아니라 실제 재생에서 팔/다리 회전과 스킨 메시 크기를 검사한다.
- Mixamo 적용 후 PlayMode 10/10 PASS: 두 캐릭터의 실제 외부 달리기/추격 관절, 점프·숙이기·비틀거림 외부 클립 재생/루트 이동 잠금/미터 단위 신체/숙이기 높이, 손 소켓·곡괭이 이동, 기존 6개 게임 흐름/UDP/코인/경로/입력 검사.
- Mixamo 실제 다운로드 FBX 4개 모두 유효 Humanoid. 두 OverrideController의 실제 외부 동작 상태 연결 8개 PASS. 루트 이동/회전을 잠가 Domain 이동과 중복되지 않는다. 채굴/아이디어 등 자체 Blender 동작은 보존한다.
- EditMode 24/24 PASS: 순수 게임 규칙, 연속 경로/높이/경계, 화면 비율.
- 실제 저장 ProBuilder 프리팹 13개 검증. 모든 바닥 정점이 런타임 베이크 메시와 일치. 런타임 `RouteStructure`에 ProBuilder 컴포넌트 0개.
- Windows 64비트 개발 빌드 PASS. 직렬화 참조 31개 프리팹·GameScene·Timeline 검사 PASS.
- 컴파일 오류 없음. Blender FBX 메시의 Z-up 좌표를 월드 Y 높이와 혼동한 초기 테스트 조건은 보정했고, 곡괭이 FBX 단위 문제는 실제 소켓 보정과 크기 조절로 해결했다.

로그: `Unity/Logs/BlenderCharactersV4.log`, `BlenderModulesV4.log`, `ToolsV4UpgradeFinal.log`, `ToolsV4MixamoImport.log`, `MuhanokPlayModeMixamoV4.xml`, `MuhanokEditModeToolsV4.xml`, `ToolsV4MixamoBuild.log`.

## 최종 화면/실행 확인

- Mixamo 포함 최종 실행 파일에서 다시 캡처한 38장 PASS: 1280×720에서 타이틀/아이디어/전환/1인칭 + 13개 맵/13개 경계, 1702×726 및 1024×768에서 각 4장. Pillow로 픽셀 검사하여 빈 화면이 아님과 검은 여백의 위치를 확인했다. `docs/previews/ArtV4/capture-verification.json`. 실행 로그: `ToolsV4MixamoPlayerCapture.log`, `ToolsV4MixamoWideCapture.log`, `ToolsV4MixamoFourThreeCapture.log`.
- 새 캐릭터 클로즈업에서 머리/전구 잘림을 확인한 뒤 카메라 거리/타깃 높이/전구 크기를 조정했다. 최종 실제 캡처에서 전구 전체가 화면 안에 보인다.
- Mixamo 포함 최종 runtime smoke PASS: title → 8초 Timeline → FOV 80 1인칭 → 120개 청크 풀링 → caught → result → retry. `Unity/Logs/ToolsV4MixamoRuntimeSmoke.log`.
- 제한된 실행 환경의 첫 smoke에서는 Windows PlayerPrefs 저장 권한 오류가 발생했다. 사용자 권한으로 재실행한 최종 로그에는 해당 예외가 없고 smoke PASS를 확인했다. 소스의 저장 기능을 우회하지 않았다.
- 최종 빌드 약 213 MiB. 실행 파일 SHA256: `44697487931CA64D09883DE957C845C6F138AAF3CD975397E99865BA6AAB987F`.
- 갤러리는 최종 `Player_` 실제 실행 파일 사진을 우선 표시한다. 카메라 전용 앞선 검수 사진과 Blender T-pose 중립 조명 렌더는 구분한다.
- Unity GameScene Play로 열어 두었다. `ToolsV4MixamoInteractive.log`의 `MUHANOK_EDITOR_READY`와 `MUHANOK_GAME_VIEW_FIT`로 1280×720 및 창에 맞춘 확대율을 확인한다. 게임 화면 클릭 후 A/D 레인 이동, Space 점프, S 숙이기, W 하이니.

## 생성한 파일

- `Tools/AssetPipeline/build_blender_characters.py`
- `References/Blender/MuhanokCharacters.blend`
- `Unity/Assets/External/Staging/Blender/Characters`: 모델 2/동작 16/무골격 T-pose 1 FBX, 자체 동작 출처 JSON
- `Unity/Assets/External/Staging/Blender/Models/MineCart.fbx`
- `References/MixamoDownloads`: 실제 FBX 원본 4개와 선택/설정/SHA256 출처 기록
- `Unity/Assets/External/Staging/Mixamo/Animations`: 실제 임포트 외부 동작 4개
- `Unity/Assets/Game/Authoring/ProBuilder`: 편집용 프리팹 13개와 지속 저장 메시
- `Unity/Assets/Game/Content/ArtV2/{Monkey,Police}Humanoid.overrideController`, 베이크 구조 메시
- `Unity/Assets/Game/Editor/MuhanokToolUpgrade.cs`, `MuhanokMixamoImport.cs`
- `Unity/Assets/Game/Tests/PlayMode/BlenderHumanoidTests.cs`
- `docs/TOOLS_V4.md`, 본 문서, `docs/previews/ArtV4` 실제 검수 사진/갤러리

## 수정한 파일

- `Tools/AssetPipeline/build_blender_modules.py`: 접힌 손가락과 수레
- `References/Blender/MuhanokModules.blend`, 기존 Blender 환경 FBX
- `MuhanokArtUpgrade.cs`, `MuhanokArtUpgrade.Maps.cs`: 스킨 캐릭터/손/수레/코인/ProBuilder 구조 베이크
- 캐릭터·수레·1인칭 손·13개 맵 프리팹과 해당 재질/메시, GameScene 아트 연결
- `RunnerView.cs`, `GameBootstrap.cs`: 단위 보정 소켓, 곡괭이 장착, 전구 크기
- `FirstPersonCameraRig.cs`: 새 캐릭터/전구를 담는 클로즈업 구도. t=8 전환 pose와 FOV 80 유지
- `MuhanokArtReview.cs`, `RuntimeVisualReview.cs`, 갤러리/캡처 검사 도구: V4 출력
- `README.md`, `AGENTS.md`, 진행/계획 문서

## 남은 문제와 다음 작업

- Mixamo 실제 다운로드·리타게팅과 재생 검증을 완료했다. 자체 Blender 동작 16개와 외부 동작 4개의 출처를 구분한다.
- 참고 이미지 수준까지 암벽/배경 반복·조명 깊이·손 실루엣·표정·시네마틱 세부 조정이 남는다.
- Mine Asset Store 상품은 약관 승인 대기로 미임포트. 동물은 정적 모델. 실제 웹캠 몸동작 정확도와 사람의 조작 감각은 별도 확인 필요.
- 추가 패키지 설치나 기존 패키지 업그레이드는 하지 않았다. 용량 경고 때 이미 설치한 Blender ZIP 다운로드 캐시만 정리했다. 프로그램/편집 원본/기존 검수 사진은 보존했다.

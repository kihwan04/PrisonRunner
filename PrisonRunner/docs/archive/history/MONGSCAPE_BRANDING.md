> 보관 기록입니다. 현재 실행·개발 안내는 [문서 목록](../../README.md)을 확인하세요.

# Mongscape 이름 변경

게임 이름은 **Mongscape**(몽스케이프), 로고 표기는 **MONGSCAPE**다. 작업 저장소는 `kihwan04/PrisonRunner`이며 별도 `leewono/muhanok` 프로젝트에는 변경을 적용하지 않는다.

## 기준 소스

현재 구현 기준은 처음 내려받은 `leewono/muhanok`의 `6e50060`이다. 이 커밋을 본인 `kihwan04/PrisonRunner` 작업 브랜치에 병합하고 이름과 투명 로고를 적용한다. 원본 `muhanok` 체크아웃에는 쓰지 않는다. Unity 내부 몸동작 인식, 전신/상체/키보드 선택, 관람객 순위, UI Toolkit 설정창, 성능 개선과 화면 분리 구조를 유지한다. 타이틀 표시는 최신 `TitleScreen.cs`가 담당한다.

## 최신 기준 소스 검증

- Unity 6000.3.10f1에서 C# 컴파일과 32개 프리팹·GameScene·Timeline 참조 검사 통과.
- Unity EditMode 테스트 64개 전부 통과, 실패·건너뛴 항목 0개. 결과는 로컬 `Unity/Logs/mongscape-latest-editmode.xml`이다.
- Domain·Application·Bootstrap·Infrastructure·PoseInference·게임 설정·패키지 파일은 `6e50060`과 동일하다. UI의 이름·로고 표시는 최신 `TitleScreen`에 적용했다.
- 첫 Windows 배포 빌드는 추론 연산 셰이더 컴파일이 오래 걸려 중단했다. 미완료 파일은 `Builds/Mongscape/Windows-incomplete-6e50060`에 보관한다. 현재 실행은 최신 소스의 Unity GameScene으로 확인하며, 이전 실행 파일로 돌아가지 않는다.
- 이전 소스의 Mongscape ZIP은 로컬 `Distribution/previous-source-05cb105`로 분리했다. 최신 Windows 배포 ZIP은 아직 없다. GitHub 업로드는 보류 중이다.

## 적용 범위

- Unity 제품명과 시작 화면의 기본 텍스트, 타이틀 로고.
- Unity 편집기 메뉴의 표시 이름: `Mongscape`.
- 새 Windows 실행 파일: `Builds/Mongscape/Windows/Mongscape.exe`.
- 새 배포 파일: `Mongscape-Windows-x64.zip`, `Mongscape-Planning-V13.zip`.
- 실행 도구, 현재 README와 배포 안내. 새 빌드가 없으면 현재 소스를 Unity에서 실행한다. 이전 게임 실행 파일로 돌아가지 않는다.

기존 배포 파일과 문서 원본은 이름 변경 전 기록이다. 기존 GitHub 다운로드 링크는 실제 공개 파일을 계속 가리킨다. 새 이름의 파일을 공개하려면 새 빌드와 배포 ZIP을 만든 후 새 태그로 배포해야 한다. `publish_github.ps1`에는 `-Commit`과 `-Tag`를 명시한다.

내부 네임스페이스, 저장 키, 에셋 경로와 GUID는 유지한다. 로고는 기존 `Unity/Assets/Game/Content/ArtV2/Textures/MuhanokLogo.png`의 이미지 내용만 교체하여 씬과 프리팹의 참조를 유지했다. 기획서 V10~V13과 이전 게임 캡처는 수정하지 않았다.

## 최초 이름 변경 단계의 검증 기록

- Unity 6000.3.10f1에서 32개 프리팹·GameScene·Timeline 참조 검사와 Windows 빌드 성공. C# 컴파일 오류 없음.
- 새 실행 파일의 자동 검증 통과: 로딩 → 플레이, 바닥 미끄러짐, 120개 청크 재사용, 한 번의 고체 충돌로 종료, 결과 화면, 재시작 초기화. 프로세스 종료 코드 0.
- Windows 배포 ZIP의 202개 구성 파일 해시와 실행 도구, 두 ZIP의 압축 무결성 검증 통과.
- 일반 게임 창의 1280×720 타이틀 프레임에서 MONGSCAPE 로고 표시 확인. [실제 타이틀 캡처](../../previews/Mongscape/Title_1280x720.png).

이번 변경에서 웹캠 인식과 전체 맵 캡처는 검증하지 않았다. 이전 공개 버전의 테스트 기록과 구분한다. 새 빌드와 ZIP은 로컬 산출물이며 기존 GitHub Release는 변경하지 않았다.

## 로고 제작

내장 `image_gen` 도구로 기존 로고의 제목을 교체하고 사각 배경을 제거했다. 최종 저장 위치는 `Unity/Assets/Game/Content/ArtV2/Textures/MuhanokLogo.png`다. 현재 로고는 1942×809 RGBA 투명 PNG이며 기존 바나나·곡괭이·돌과 금색 조명 스타일을 유지한다.

초기 이름 변경 프롬프트:

> Use case: text-localization. Edit target: the supplied existing game title logo image. Replace all large Korean title text with the exact English word "MONGSCAPE", spelled M O N G S C A P E. Preserve the original stylized 3D carved stone, thick dark rock extrusion, warm gold rim lighting, banana, pickaxe, scattered golden ore and dark amber background. A strong readable game wordmark, nine letters on one centered line, comfortably inside the canvas; cream stone letters MONGS and rich red stone CAPE, matching the existing material style. Preserve the 3:2 landscape composition, 1536x1024 dimensions. No Korean text, no additional text, no new characters, no watermark. This is a name change for an existing Unity game; keep the same visual identity.

## 타이틀 합성 보정

초기 로고에는 불투명한 갈색 사각 배경이 남아 있어 게임 위에 별도 그림을 붙인 것처럼 보였다. 로고의 배경을 알파 채널로 제거하고 1280×720 기준 `(56, 88, 450, 188)` 영역에 비율을 유지해 표시한다. Unity 임포터는 알파 투명도, 원본 비율, Clamp, 비압축을 적용하고 밉맵을 끈다. 아트 재생성에서도 같은 설정을 적용한다. 최신 타이틀의 맥박 효과가 있는 시작 안내와 F1 설정 표시는 유지한다.

보정 후 Unity 콘텐츠 참조 검사와 Windows 재빌드가 성공했다. 일반 게임 창의 실제 타이틀 캡처에서 사각 배경이 사라지고 글자 사이로 광산 배경이 보이는 것을 확인했다. PNG에는 완전 투명 픽셀 503,062개와 부드러운 가장자리 알파가 있다. 위 타이틀 캡처 링크와 로컬 배포 ZIP도 보정된 빌드로 갱신했다.

배경 제거 프롬프트(`transparent_background=true`):

> Use case: background-extraction. Edit target: supplied MONGSCAPE logo. Remove the entire black/brown/amber rectangular background and all diffuse background glow. Deliver a genuinely transparent PNG cutout with an alpha channel: ONLY the nine-letter MONGSCAPE stone wordmark, its attached gold banana and pickaxe, and small attached rock/ore details. Preserve exact spelling MONGSCAPE and the existing cream/red carved-stone materials and warm golden edge highlights. Keep all letters and tools complete with clean antialiased edges, no dark matte, no rectangular halo, no opaque panel, no backdrop, no floor, no gradient, no checkerboard pixels, no extra text. Clean compact 2.4:1 horizontal composition, ideally 1536x640, with 5% transparent breathing room around the complete silhouette. This will overlay a 3D mine game title screen, so the background must be truly transparent, including holes in letters and gaps between disconnected details. Preserve the logo identity; make only this cutout change.

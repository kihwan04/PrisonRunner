# 무한옥 V10 캐릭터 접지와 철창 독방

2026년 10월 5일 사용자 요청을 실제 Unity 게임과 편집 가능한 화면별 기획서에 적용했다.

## 감옥 공간

타입 4와 5는 양쪽에 3개씩 독방을 배치한다. 각 방은 전면 철창과 잠긴 문, 바닥, 양쪽 칸막이, 공용 뒷벽과 천장으로 둘러싸인다. 철창은 복도 중심에서 3.97 m, 뒷벽은 7.9 m, 방 폭은 약 4.8 m다. 침대는 안쪽 벽으로 옮기고 담요, 벤치와 물그릇을 방 안에 배치했다. 복도 앞쪽의 독립 침대처럼 보이는 구성을 교체했다.

복도 청크마다 자체 스킨 원숭이 2명을 철창 뒤에 두었다. 수감 번호, 대기 동작, 얼굴의 눈 깜빡임과 입 모양이 실제로 재생된다. NPC는 `CellActors` 아래에서 활성 청크와 함께 풀링되며 정적 환경 메시 병합에서 제외한다. 충돌체를 추가하지 않아 플레이어의 차선을 막지 않는다. 독방 조명은 그림자 없는 얼굴 보조등과 안쪽 랜턴을 사용한다.

## 캐릭터와 접지

Blender 캐릭터에 주황 손목·허리 밴드, 등에 0723 번호 패치와 손발톱을 추가했다. 원본은 `References/Blender/MuhanokCharacters.blend`, 재생성 스크립트는 `Tools/AssetPipeline/build_blender_characters.py`다. 2개 Humanoid와 18개 자체 동작을 내보낸다. 달리기·점프·숙이기·비틀거림의 실제 Mixamo 임포트는 기존 그대로 유지하며, `AN_Monkey_CellIdle`은 별도 자체 제작 동작이다.

Animator의 IK Pass와 `FootGrounding`을 사용한다. 발목 기준 바닥에서 0.13 m를 목표로 삼고 낮은 발의 위치와 경사를 보정한다. 들어 올린 발은 보정을 줄이고 점프·수레에서는 해제한다. 접지 중 수평 이동 0.28 m 이내에서 발 위치를 유지하며, 큰 이동과 풀링·순간 이동에서는 잠금을 해제한다. Domain의 이동과 충돌은 변경하지 않는다. 달리기 동작 속도는 이동 진행과 감속 비율에 맞춰 조절한다.

## 화면별 기획서

`무한옥_화면별_게임기획서_V10.docx`는 실제 화면 사진 20장과 동작·판정 표를 담은 21페이지 Word 파일이다. 타이틀, 선택 몸무게, 로딩 인트로, 실제 광산 전환, 광산, 수레, 철로 붕괴 회피, 하차, 계단, 닫히는 문, 슬라이딩, 감옥, 독방 근접, 캐릭터 접지, 보안 출구, 사육장, 감속, 경찰에게 잡힘, 일시정지, 결과 화면을 설명한다.

`RuntimeVisualReview`는 명시적 개발용 캡처 플래그에서만 동작한다. 일반 플레이 화면과 제작 검수용 카메라를 구분한다. 근접 화면은 실제 모델을 촬영하며 AI 콘셉트 이미지로 대체하지 않는다. 로딩 화면은 사용자가 제공한 이미지를 실제 로딩 UI에 표시한 화면이다.

Word 제작은 Codex의 Python 및 python-docx를 사용했다. Windows 기본 의존성에 LibreOffice가 없어 공식 MSI의 유효한 서명을 확인하고 `Tools/DocumentRuntime`에만 추출했다. 사용자의 데스크톱 오피스를 변경하지 않았다. 패키지의 `render_docx.py`를 사용하며, 작업 폴더의 전용 LibreOffice와 Codex의 Poppler 경로를 명시한다. 중간 PDF와 페이지 PNG는 `docs/document-qa/V10`에만 보관한다.

## 실행과 검수

Unity 메뉴 `Muhanok/Upgrade Enclosed Cells and Foot Contact V10`으로 재생성한다. 실제 테스트는 `Assets/Game/Scenes/GameScene.unity`를 Play하거나 `Builds/Windows/Muhanok.exe`를 실행한다. A/D 이동, Space 점프, S 숙이기, W 하이니. 수레에서는 A/D가 몸 기울이기다.

전체 게임 캡처는 `--muhanok-capture --muhanok-mapcapture`, 근접 재촬영은 `--muhanok-capture --muhanok-detailcapture`를 실행 파일에 전달한다. 갤러리와 프레임 검수는 `create_art_gallery.py --version ArtV10`과 `verify_capture_frames.py --version ArtV10`이다. 기획서 재생성은 Codex 번들 Python으로 `create_screen_plan.py`, 렌더링은 `render_screen_plan.py`를 실행한다.

## 변경 파일과 제한

주요 추가 소스는 `FootGrounding.cs`, `CellPrisoner.cs`, `MuhanokArtUpgrade.Cells.cs`, `CellAndGroundingTests.cs`다. `RunnerView`, 맵 생성, 도구 임포터, V10 재생성 메뉴와 캡처 도구를 수정했다. 13개 런타임 청크, 캐릭터 2종, 컨트롤러와 애니메이션 에셋은 생성기를 통해 갱신했다. 문서 생성·렌더링 도구, 실제 사진 갤러리, 검수 문서도 추가했다.

이번 버전은 서브웨이 서퍼스·템플런과 동등한 최종 아트 품질을 달성했다고 판정한 결과가 아니다. 그림 속 캐릭터와 현재 자체 모델의 형태·조명·애니메이션에는 차이가 있다. 무료 BitGem 경찰은 정상 다운로드 인증 실패로 아직 임포트하지 못했고 현재 경찰은 자체 모델이다. 접지 보정은 짧은 범위의 IK이며 모든 운동 속도에서 완전히 미끄러짐 없는 보행을 보장하지 않는다. 다음 아트 작업은 모델 실루엣, 자연스러운 손가락 그립, 발걸음 타이밍과 최종 조명 다듬기다.

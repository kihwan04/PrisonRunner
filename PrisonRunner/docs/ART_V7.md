# V7 · 로딩과 실제 3D 장면 연결 (2026-10-04)

사용자가 제공한 로딩 그림에 맞춰 실제 캐릭터와 광산을 수정했다. 로딩 이미지만 개선한 작업이 아니다. 원본 그림은 그대로 사용하며, 그 아래에 준비한 실제 게임 장면으로 0.85초 동안 전환한다. 참고 이미지 및 서브웨이 서퍼스·링피트 수준의 최종 품질 달성으로 판정하지 않는다.

## 구현

- Blender 원숭이를 갈색 털·맨발·베이지색 배의 연결된 스킨 메시로 재제작했다. 겹치는 신체 조각을 voxel union하고 근접 원본 정점의 뼈 가중치를 새 메시로 옮겼다. 어깨·다리 비율, 넓은 주둥이와 개별 치아를 조정했다. Humanoid와 실제 Mixamo 동작 4개 연결은 유지한다.
- FBX에 Blink와 JawOpen blend shape를 내보냈다. 실제 Animator 상태에 따라 아이디어·달리기·비틀거림 표정과 눈 깜빡임을 적용한다. 표정이 플레이어 이동 좌표를 바꾸지 않는다.
- 광산 암벽을 불규칙한 단면과 모서리의 메시로 바꾸고 6개 암석 재질에 실제 atlas를 적용했다. 목재 무늬 대비와 반복 크기를 낮추고 금속 띠·리벳, 작은 광석 군집을 추가했다.
- 기존 광산 조명의 그림자 후보 지정 오류를 수정했다. 가까운 점광원 2개/각 512px 예산을 실제로 사용한다. 기존 URP SSAO의 접촉 음영을 강화했다.
- 사육장 우리 바깥 지면을 실제 메시로 채웠다. 기존 Kenney CC0의 상세 야자수·나무·덤불·풀을 적용한다. 동물은 자체 Blender 모델이며 전용 골격 애니메이션은 아니다.
- 로딩 완료 시 카메라와 인트로 첫 포즈를 미리 평가한다. 이미지가 사라지는 동안 인트로 시간은 진행하지 않는다. 전환 뒤 타이틀 로고가 다시 나타나지 않는다.
- 인트로 카메라를 가까운 구도로 바꾸고 실제 머리 위치에 초점을 맞추는 심도를 적용한다. 1인칭 진입 시 심도를 끄고 게임 경로를 선명하게 유지한다.
- 광산 → 계단 → 감옥 복도 → 사육장, 바닥까지 닫히는 문, 선택 몸무게·예상 칼로리, 16:9 화면과 넓은 창/4:3 여백은 유지한다.

## 생성·수정 파일

생성한 코드: `Unity/Assets/Game/Presentation/Player/FacialPerformance.cs`, `Presentation/Camera/CinematicFocus.cs`, `Tests/PlayMode/FacialPerformanceTests.cs`와 해당 Unity meta 파일. 신규 실제 GPU 캡처와 갤러리는 `docs/previews/ArtV7/`에 있다.

수정한 제작 원본: `Tools/AssetPipeline/build_blender_characters.py`, `build_blender_modules.py`, `References/Blender/MuhanokCharacters.blend`, `MuhanokModules.blend`, Blender Staging의 캐릭터·동작 16개·환경 FBX. 자체 제작 결과이며 다운로드한 상품이라고 표시하지 않는다.

수정한 Unity 코드: `Bootstrap/GameBootstrap.cs`, `RuntimeVisualReview.cs`, `Presentation/UI/HUDPresenter.cs`, `HUDPresenter.Start.cs`, `Presentation/Flow/IntroSequenceDirector.cs`, `Presentation/Camera/FirstPersonCameraRig.cs`, `Presentation/Map/WorldAtmosphere.cs`, `Editor/MuhanokToolUpgrade.cs`, `MuhanokArtUpgrade.Maps.cs`, `MuhanokArtUpgrade.Zoo.cs`, `MuhanokVisualPolish.cs`, `Content/ArtV2/HandPaintedTriplanar.shader`. 빌드된 캐릭터 프리팹·맵 13개·메시·재질·Volume·GameScene 및 PC_Renderer 설정도 실제 반영했다. 시작 전환 관련 PlayMode 테스트와 캡처 도구 2개를 갱신했다.

이미지 atlas는 기존 V6 생성 파일이며 새로 생성했다고 표시하지 않는다. 출처와 정확한 프롬프트는 `References/GeneratedV6/Provenance.md`에 있다.

## 바로 테스트

Unity 상단 `Muhanok → Play GameScene` 또는 프로젝트 `PlayInUnity.cmd`. 타이틀 클릭 → 몸무게 입력 또는 생략 → 로딩 → 실제 광산 인트로 → 1인칭. 재생성은 `Muhanok → Upgrade Loading Continuity and Models V7`을 사용한다. 수정한 ProBuilder 원본은 기존 Bake 메뉴로 반영한다.

## 실제 외부 에셋 상태와 남은 문제

무료 BitGem 경찰 107256의 계정 취득은 확인했지만 파일 다운로드·임포트는 완료되지 않았다. 사용자는 Hub/Unity에 로그인 상태라고 확인했다. GUI 편집기의 정상 다운로드 서비스는 `signedIn=False / batch=False`와 인증 실패를 반환했다. 현재 보이는 경찰은 자체 Blender 모델이다. 브라우저 외부 앱 연결 우회나 로그인 토큰 접근은 하지 않았다. 유료 원숭이 108270은 구매하지 않았다. Higgsfield 실행 도구는 연결되지 않았으며 사용했다고 기록하지 않는다.

원숭이의 얼굴·손·몸통 실루엣, 경찰의 형태, 동물 동작, 계단 파손, 구간마다 독특한 구조와 소품 구성은 참고 그림과 여전히 차이가 있다. 2D 로딩 그림에는 게임보다 복잡한 모델·배경과 장면 전체 조명 구성이 있다. 전환 기능이 이 차이를 해결했다고 주장하지 않는다. 다음 우선순위는 경찰 원본 파일 확보 후 Humanoid/Mixamo 검수와, 현재 실제 화면에 맞춘 캐릭터 실루엣 및 구간별 아트 제작이다.

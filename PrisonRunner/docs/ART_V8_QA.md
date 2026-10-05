# V8 실제 검수 · 2026-10-04

## 최종 결과

- Blender 4.5.9 실행 PASS. 기존 수레에서 광석 적재물을 분리한 실제 `MineCartEmpty.fbx`를 추가하고 제작 원본/파이프라인을 갱신했다. 환경 모듈 총 9개.
- 최종 Unity 적용 PASS: `Unity/Logs/ArtV8Upgrade.log`, `MUHANOK_V8_PASS`. 실제 프리팹 바닥 아이템, 밤하늘 shader, Humanoid/Mixamo/13개 ProBuilder 경로와 기존 콘텐츠 참조 검증 포함.
- EditMode **38/38 PASS**, `Unity/Logs/MuhanokEditModeV8.xml`. 한 번 충돌 종료/이동 정지, 비치명 감속/회복/일시정지, 수레 조작과 점프 제한, LoadingIntro→Running 흐름 및 기존 규칙.
- 최종 PlayMode **16/16 PASS**, 종료 2026-10-04 04:25:09 UTC. `Unity/Logs/MuhanokPlayModeV8.xml`. 실제 authored banana를 밟았을 때 생존/감속, 점프 회피, 실제 장애물 MonoBehaviour 한 번 접촉 시 Caught/거리 정지, 수레 프리팹/음향 참조/실제 기울기, 로딩 뒤 8초 중복 인트로 없음, 기존 표정/경로/문/입력/칼로리 검사.
- 최종 Windows 빌드 PASS, 출력 총량 **246,349,153 bytes**. `Unity/Logs/ArtV8Build.log`. `Builds/Windows/Muhanok.exe`와 옆 데이터 폴더/라이브러리를 함께 실행한다.
- 최종 실제 GPU 캡처 **46장 PASS**. 1280×720: 흐름 8장/10개 맵과 연결부 20장/실제 바나나 감속·실제 한 번 충돌 결과 2장. 1702×726/1024×768: 흐름 각 8장. 비어 있지 않은 PNG 및 16:9 여백 검사 `docs/previews/ArtV8/capture-verification.json`.
- 실행 파일 smoke PASS, `Unity/Logs/ArtV8RuntimeSmoke.log`: loading intro → 직접 gameplay → floor slip → 120개 청크 순환/풀 유지 → 한 번 충돌 → result/retry.
- 최종 적용/테스트/빌드/캡처/smoke 로그에서 C# compile error, shader error, unhandled exception, 점광원 shadow atlas 자동 축소 경고를 관찰하지 않았다.
- GUI Unity도 GameScene Play 진입 `MUHANOK_EDITOR_READY`, 1280×720 확대율 1 `MUHANOK_GAME_VIEW_FIT`를 확인했고 열린 상태로 둔다. 편집기의 Play 진입 전 Scene View에서 24개 shadow map 자동 축소 경고가 한 번 기록됐다. 게임 실행에서는 가까운 2개 점광원 예산이 적용된다. Package Manager 온라인 검색은 `unable to verify the first certificate`로 실패했다. 인증서 검증을 끄거나 임의 패키지 업그레이드로 우회하지 않았다. 이 편집기 온라인 연결 문제와 게임의 C# 컴파일/실행 검증은 구분한다.

## 실제 화면에서 확인

HUD 패널이 처음 캡처에서 보이지 않아 코드의 SmoothStep 인수 오류를 수정하고 다시 적용/PlayMode/빌드/캡처했다. 최종 이미지에서 둥근 배경, 코인/왕관 아이콘과 파란 일시정지를 확인했다. 사육장 하늘이 너무 어두워 실제 shader 색을 조정하고 구름/달/후광, 바깥 수목을 추가한 뒤 최종 화면에서 확인했다.

`Player_FirstLiveFrame`: 제공 로딩 이미지가 완전히 사라진 실제 1인칭 3D. `Player_CartRide/CartBank/CartExit`: 실제 빈 수레와 그립, 좌우 기울기, 하차 후 달리기. 맵 03/04 연결부: 시설 바닥→높이 3m 계단→감옥. 맵 07/08 연결부: 서비스 출구→사육장 진입. `Player_Slip`: 실제 바나나 판정으로 생존/감속 HUD. `Player_GameOver`: 실제 authored solid 접촉 한 번으로 결과 화면.

캡처 동안 검수 드라이버가 자동 조향한다. 장애물이나 충돌 판정을 숨기거나 비활성화하지 않았다. 바나나/게임오버 검수는 실제 맵 MonoBehaviour 판정을 호출했다. 캡처에 합성 3D 장면을 넣거나 사진을 보정하지 않았다. 상세 [갤러리](previews/ArtV8/index.html).

## 남은 범위

현재 경찰은 자체 Blender 모델. Unity 계정에 취득된 무료 BitGem 경찰은 실제 파일 다운로드/임포트가 완료되지 않았다. 제작사 공식 0원 주문 화면까지 준비했지만 필수 이메일/이름/주소는 사용자 직접 처리 대기다. `previews/ArtV8/PoliceFreeCheckout.png`. 주문 완료나 원본 적용으로 보고하지 않는다. Higgsfield는 실행하지 않았다.

공간 연결/규칙/HUD와 일부 배경 표현은 개선됐지만 캐릭터 실루엣/달리기 접지·동물·세부 소품 구성은 참고 그림보다 단순하다. **서브웨이 서퍼스/로딩 그림 수준의 전체 그래픽 완성도는 아직 미달**이다. 다음 추천 작업은 무료 경찰 원본 확보와 실제 리타게팅, 캐릭터 모델/맵별 배치에 대한 추가 아트 제작이다. 웹캠 운동 코칭/예상 kcal 정확도와 최종 GPU 성능 최적화는 자동 입력/현재 장비 테스트와 구분한다.

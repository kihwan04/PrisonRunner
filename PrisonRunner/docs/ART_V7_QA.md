# V7 실제 검수 · 2026-10-04

## 결과

- 최종 Blender 실행 PASS. 연결된 원숭이 스킨 메시, 실제 FBX 표정 2개, 16개 자체 동작과 환경 모델을 다시 내보냈다. 제작 스크립트 Python 문법 검사 PASS.
- 최종 Unity 적용 PASS: `Unity/Logs/ArtV7Upgrade.log`의 `MUHANOK_VISUAL_V7_PASS`. 31개 프리팹/씬/Timeline 참조, Humanoid/Mixamo 연결, 13개 ProBuilder 경로와 런타임 베이크 검수 통과.
- PlayMode **14/14 PASS**, 2026-10-04 03:28:21 UTC 종료. `Unity/Logs/MuhanokPlayModeV7.xml`. 표정의 실제 mesh blend shape 구동과 root 고정, 로딩 전환 동안 인트로 시간 정지/전환 후 재개, 실제 광산 그림자 존재 및 최대 2개 제한, 맵·문·입력·칼로리 관련 기존 검사 포함.
- Windows 빌드 PASS, 출력 총량 **244,205,206 bytes**. `Unity/Logs/ArtV7Build.log`. `Builds/Windows/Muhanok.exe`와 옆의 데이터 폴더/라이브러리를 함께 실행한다.
- 최종 실제 GPU 캡처 **44장 PASS**. 1280×720에서 타이틀·몸무게·로딩·전환 중간·첫 실제 장면·아이디어·카메라 진입·1인칭 8장과 10개 맵/연결부 20장. 1702×726과 1024×768에서 각각 8장. 비어 있지 않은 실제 PNG와 16:9 여백을 검사했다. `docs/previews/ArtV7/capture-verification.json`.
- 실제 이미지에서 광산 재질·단면·조명, 그림 없이 보이는 첫 장면, 전환 중간, 인트로 초점, 감옥 연결부, 사육장 무료 수목을 확인했다. 기능/아트 적용 확인이며 참고 그림과 동일한 품질 인증이 아니다.
- 최종 실행 파일 smoke PASS: `Unity/Logs/ArtV7RuntimeSmoke.log`. title → 8초 Timeline → 1인칭 → 120개 청크 → caught/result/retry.
- 최종 적용/빌드/테스트/세 종류의 GPU 캡처/smoke 로그에서 C# compile error, shader error, unhandled exception, 점광원 shadow atlas 자동 축소 경고가 관찰되지 않았다.

## 실제 적용과 미완료를 구분

현재 캐릭터는 자체 Blender 모델이다. 실제 Mixamo 동작 4개와 Kenney CC0 소품/수목, 기존 생성 재질 atlas는 적용돼 있다. BitGem 무료 경찰 107256은 계정 취득만 확인됐고 정상 Unity 다운로드 서비스의 인증 실패로 실제 모델 파일은 미임포트다. Higgsfield는 실행하지 않았다. 유료 원숭이는 구매하지 않았다.

원숭이·경찰의 모델 형태, 동물 애니메이션, 계단 파손, 세부 소품과 구간별 구성은 아직 참고 그림보다 단순하다. 따라서 **사용자가 요청한 로딩 그림 수준의 전체 아트 완성도는 미달**이다. 이미지 전환이 작동한다는 사실과 실제 3D 품질은 구분한다. 다음 우선순위는 무료 경찰 원본 확보/리타게팅, 캐릭터 실루엣과 구간별 배경 제작이다.

몸동작은 기존 게임 규칙/자동 입력으로 검증했다. 실제 웹캠 운동 코칭 및 칼로리 정확도는 별도 장비 검증이 필요하며, 현재는 예상 kcal로 표시한다.

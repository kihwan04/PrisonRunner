# 무한옥 진행 기록

## 최신 V9 · 전용 카트와 슬라이딩으로 연결되는 감옥 (2026-10-04)

- 단선 절벽 카트, 실제 한쪽 철로/바닥 붕괴 네 곳, 몸 기울이기, 실제 3인칭 수레와 Blender 그립 동작/음향.
- 광산/하차 → 계단 하이니 → 내려오는 문 아래 슬라이딩 → 감옥 복도. 문은 지나간 뒤 완전히 닫힌다. 위치/높이/카메라 보간을 유지한다.
- 점프/숙이기/하이니 운동 행, 중간 레인 위치로 운동 행 우회 금지, 바나나 누적 2회는 감속 후 경찰 잡힘, 물웅덩이는 감속만.
- 상세 제작/검수 ART_V9.md / ART_V9_QA.md. 현재 경찰은 자체 모델이고 전체 아트는 상용 benchmark와 차이가 남는다.
- 최종 EditMode 42/42, PlayMode 20/20, Windows 빌드 239,715,731 bytes, 실제 GPU 51장/세 창 비율 및 120개 청크 실행 smoke PASS. GUI GameScene Play/비율 맞춤을 확인했다. 큰 공중 문자와 잡힘 연출의 카메라 겹침은 실제 검수 후 수정했다.

## 최신 V8 · 규칙·수레·로딩 인트로·연속 맵 (2026-10-04)

- 장애물 한 번 충돌 즉시 이동 정지/결과, 실제 바나나/물웅덩이 2.4초 50% 감속/회복과 풀 리셋.
- 제공된 로딩 이미지가 인트로이며 전환 후 바로 1인칭 실행. 정상 시작에 별도 8초 연출을 재생하지 않는다.
- Blender 빈 수레 FBX, 두 청크 연속 탑승/조향/숙이기/손 그립/굴러가는 소리/기울기/하차.
- 서비스 출구·차양·낮은 벽·사육장 미리보기, 더 넓은 지면/외곽 수목, 실제 밤하늘/달/구름, 둥근 HUD와 코인/왕관.
- 구현 ART_V8.md, 최종 검수 ART_V8_QA.md. 이전 V7 기록은 당시 결과를 보존한다.
- 무료 경찰 공식 0원 주문 화면 확인; 필수 이메일/주소를 임의 입력하지 않았다. 실제 원본은 미임포트이며 참고/상용 그림 수준의 전체 아트는 미달이다.

## 최신 V7 · 로딩과 실제 3D 장면 연결 (2026-10-04)

- 갈색 연결 스킨 원숭이·FBX 표정 2개·신체 비율/주둥이/치아, 불규칙 암벽과 6개 암석 재질, 목재 대비·광산 실제 그림자·SSAO, 사육장 바깥 지면과 상세 무료 수목을 반영했다.
- 첫 3D 포즈를 준비한 뒤 로딩 그림을 0.85초간 전환한다. 전환 중 인트로 시간 정지, 타이틀 재등장 제거, 인트로 심도와 게임 선명도 분리.
- 최종 PlayMode 14/14, Windows 빌드 244,205,206 bytes, GPU 44장/세 가지 화면 비율, 실행 파일 120개 청크 smoke PASS. 최종 compile/shader error 및 unhandled exception 없음. docs/ART_V7.md, ART_V7_QA.md, previews/ArtV7/index.html.
- 현재 경찰은 자체 Blender 모델이다. 계정 취득된 무료 경찰 107256은 정상 편집기 다운로드 인증 실패로 미임포트. Higgsfield 미연결, 유료 원숭이 미구매. 사용자 그림/상용 benchmark와 같은 전체 아트 품질에 도달한 상태는 아니다.

## V6 · 실제 표면 재질과 동작 개선 (2026-10-04)

- 생성 albedo atlas를 실제 맵에 투영하고 Blender 손 형태/동작 반응을 개선했다. 두 개 512px 점광원 그림자 예산과 표지판 거리/안개를 적용했다.
- 실제 GPU 첫 검수에서 발견한 Forward+ 조명 variant 누락을 고쳤다. 최종 Windows 빌드/38장 화면 비율/120개 청크 smoke PASS. PlayMode 13/13 PASS. docs/ART_V6.md, docs/ART_V6_QA.md와 docs/previews/ArtV6/index.html 참고.
- 무료 BitGem 경찰 107256은 계정 취득 확인. 현재 미임포트. 브라우저 외부 Unity 연결은 보안 정책이 차단했다. 사용자는 Unity에도 로그인되어 있다고 확인했고, 기존 프로젝트의 정상 Package Manager API 연결 상태를 검수한다. 유료 원숭이는 구매하지 않았다.
- Higgsfield 실행 도구는 연결되지 않았다. 서브웨이 서퍼스/링피트와 동등한 최종 아트 수준에 도달했다고 판정하지 않는다.

## 최신 — 무료 아트 개선

- 사용자 무료 에셋만 사용 선택 반영. 공식 Kenney CC0 4개 kit / FBX 403개 실제 임포트. Asset Store Mine은 Unity 계정 로그인 대기이며 미임포트.
- 13개 맵의 환경 장식/소품/조명 구성, 자연스러운 24m 경계, 실내/실외 안개·환경광 전환. 1인칭 손·코인 획득·타이틀 로고 추가.
- 자체 제작 캐릭터와 클립 개선. 참고 이미지 수준의 캐릭터/광산 아트 완성은 아직 남아 있다.
- Unity EditMode 18/18, PlayMode 5/5, Python 4/4 PASS. 31개 프리팹/Timeline/씬 참조 PASS. GPU 30장 캡처 PASS.
- 검수에서 발광 재질/어두운 조명/세탁기 방향/입 scale/검문소 통로를 수정 후 재촬영.
- 변경 파일·검증 근거·품질 차이: `docs/ART_V2_QA.md`, 실제 에셋/라이선스/로고 생성 기록: `docs/FREE_ART_V2.md`.
- 새 Windows 개발 빌드 약 182 MiB PASS. 실제 실행 파일 smoke exit 0 / PASS. 실제 창을 표시한 GPU 캡처 4장 시각 검수 PASS. `Builds/Windows/Muhanok.exe` 및 `docs/previews/ArtV2/index.html`.
- 개선 버전을 Unity에 열고 실제 Play 진입 확인: `Unity/Logs/MuhanokInteractivePlay.log`의 `MUHANOK_EDITOR_READY`. 상단 Play 버튼으로 종료하거나 결과에서 다시 하기. URP shadow atlas 해상도 경고는 있으나 compile error/runtime exception은 없음.

아래는 이전 구현부터의 작업 기록이다.

## Phase 0 — 조사 완료
- 기존 runner/map/camera 및 editor smoke 검토. 원본 장면 보존.
- IMPLEMENTATION_PLAN.md에 재사용/누락/패키지 차이/검증 위험 기록.
- baseline Unity batch 실행: 라이선스 초기화 대기 중. 성공 결과 미확인.
- Placeholder: monkey/police/mine/전용 clip 없음.
- 다음: Phase 1 순수 규칙과 게임 흐름.

## Phase 1 / Phase 2 — 구현
- 순수 flow/runner/chase/score, keyboard/router, HUD/result/retry 및 명시적 bootstrap 연결.
- 기존 LaneRules 재사용. 원본 scene/scripts 보존.
- 24m 청크 13종, 2.2m lane anchor, 두 행 안전 패턴, 26개 청크 선할당 pool.
- Unity 첫 compile exit 0 (MuhanokCompile.log). 새 scene 생성/PlayMode 검증은 다음 compile에서 수행.
- 현재 모든 캐릭터/전용 animation은 placeholder.

## Phase 3 — Intro
- `Content/Animations/Intro_8s.playable`: Timeline custom track / StartRun Signal / 8초 sequence.
- 곡괭이질 → 수레 접근 및 rumble → 고개 돌림 → 전구 → 곡괭이 낙하 → 탈출 → 경찰 → 동일 위치/방향/FOV의 1인칭 takeover.
- Timeline clip 참조 직렬화 오류를 PlayMode에서 발견하고 별도 파일의 ScriptableObject로 수정. 실제 저장 후 재로딩 테스트 통과.
- 본편 camera FOV 80, root motion 비활성, scene load 없음. animator placeholder 16개 ID.

## Phase 4 — Pose
- thread + bounded UDP queue + main-thread JSON parse + threshold/cooldown/edge debounce/timestamp ordering/timeout/reconnect.
- malformed/unknown/NaN/low confidence 무시. Lost에도 키보드 유지. 정상 연결 후 낮은 confidence는 Lost로 표시.
- Python Tasks PoseLandmarker sender / 45프레임 보정 / smoothing / 6 commands / CLI simulation.
- 로컬 `.venv` 생성. MediaPipe 0.10.35 / OpenCV contrib 4.14.0 설치. pip check 통과. `requirements-lock.txt` 기록.
- Google 공식 Lite `.task` 모델 다운로드 및 `--check` 실제 CPU 추론 통과 (blank frame 0 detections).
- 실제 웹캠/사용자 동작 정확도 검증은 아직 수행하지 않음.

## Phase 5~7 — Chase / UI / Content
- normalized pressure light .18 / heavy .35 / 회복 / stumble speed loss / caught catch animation / result / PlayerPrefs best.
- 무한옥 title any click/touch, HUD distance/score/best/chase/pose, 결과/다시 하기.
- `GameSettings.asset` 튜닝. 30개 정확한 ID prefab 생성. 기존 CellBlock crate/pipe/barricade visual 재사용.
- 캐릭터/맵/소품/VFX/animation은 blockout/placeholder. 상용 에셋 구매/임포트 없음.
- `ASSET_CATALOG.md`: 캐릭터, 광산, 감옥, 정비, 가구, VFX, UI 후보 및 ID별 교체 경로. GameScene bootstrap에서 교체 가능.

## Phase 8 — 자동 QA 완료
- Unity EditMode 최종 17/17 통과. state guards, lane bounds/easing, jump/action reset, pressure/clamp/cooldown, score, pattern, input routing, pose validation/debounce/restart/Lost.
- Unity PlayMode 4/4 통과. 실제 InputSystem 가상 클릭/keyboard → router, intro/first-person, 120개 chunk gap/overlap/pool bounds, caught/result/retry, actual UDP disconnect/reconnect, missing character fallback.
- Python unittest 4/4 통과. all commands, low confidence, lost/recalibration, NaN.
- Burst 임시 cache 손상을 `Library/BurstCache/JIT`만 정리해 복구. 패키지 업그레이드 없음.
- Unity rendering preview 4개 생성/시각 검수 (`docs/previews/Muhanok_*.png`). camera 렌더만 캡처하므로 IMGUI overlay는 이미지에 포함되지 않음.
- Windows 64비트 개발 빌드 성공: `Builds/Windows/Muhanok.exe` (전체 build report 약 160 MiB). 해당 폴더의 동반 파일이 필요함.
- 실제 Windows 실행 파일의 headless runtime smoke exit 0 / PASS: title → 실제 8초 Timeline → FOV 80 1인칭 → 120개 청크 진행 및 풀 수 유지 → caught → result → retry 초기화.
- 프리팹 30개 / GameScene / Timeline 직렬화 참조 검증 통과.
- 기존 CellBlockVerticalSlice 회귀 smoke exit 0 / PASS: 7 active/total 청크, 11 장애물.
- Python UDP simulation 실제 datagram 수신, pip check, 공식 모델 로딩 및 CPU 추론 통과.
- 자동 QA 범위와 남은 수동 검증: `QA_REPORT.md`. 최종 아트, 웹캠 동작 정확도, 실제 화면 HUD/오디오 체감은 아직 수동 검증 필요.

## Unity 즉시 테스트 준비
- `MuhanokQuickPlay.cs`: `Muhanok/Play GameScene` 메뉴, GameScene 열기/Play/Game 탭 focus. 다른 씬의 저장 확인 유지.
- `PlayInUnity.cmd`: 지정 Unity 버전 실행 또는 기존 editor에 실행 요청. `Library`의 일회성 요청은 batch/test 실행에서 무시.
- README에 빠른 실행/종료 방법 추가. 현재 열린 Unity에 요청 전달 완료.
- 새 editor 스크립트를 설치된 Unity의 Roslyn 및 Unity/netstandard 참조로 별도 컴파일: exit 0, compile error 없음. 현재 실행 요청은 대기 중이며 Play 진입 로그는 아직 없음.
- Computer Use 창 캡처(`FrameArrived timed out`) 및 활성화(`activate_window` timeout)는 실패. 현재 editor가 백그라운드일 때 새 소스 refresh는 Unity 창을 활성화해야 진행될 수 있음. 실제 Play 진입은 editor 로그로 확인 후 기록.


## 화면 비율·Blender·연속 경로 V3
- 카메라/HUD 안전 영역 16:9, GameView 1280×720 자동 맞춤. 실제 1702×726/1024×768에서도 로고·HUD 잘림 해결 확인.
- 곡선 선로, 계단 +3m→시설 고도 유지→야외 하강, X/Y/Z 전체 소켓 정렬·경계 기울기 연속. 소품·광원·장애물·코인·플레이어·카메라 경로 일치.
- 광산/콘크리트 벽·선로 전환, 시설/야외 캐노피, 야간 사육장과 기린·코끼리. 동물은 자체 정적 모델.
- Kenney Nature Kit 329 FBX 추가로 CC0 총 732 FBX. Blender 4.5.9 LTS 공식 ZIP/SHA256 검증, 7 FBX와 편집 가능한 .blend 생성.
- 정적 모델 재질별 결합, 랜턴 추가 그림자 축소, 실제 GPU 검수 후 아치 크기·손 방향·울타리 조정.
- EditMode 24/24, PlayMode 6/6, Python 4/4 PASS. 실제 Windows 빌드 210 MiB 및 runtime smoke PASS. 카메라 30장 + 실제 플레이어 38장 검수.
- Asset Store 로그인 확인. Mine은 서비스 약관/EULA 승인 대기로 미다운로드·미임포트. 참고 이미지와 캐릭터/연출 품질 차이 남음.
- 최신 파일·에셋·테스트 기록: docs/ART_V3.md, docs/ART_V3_QA.md, docs/previews/ArtV3/index.html.

## Blender · ProBuilder · Mixamo V4
- 사용자 선택 도구: Blender / ProBuilder / Mixamo. 무료만 사용한다.
- 자체 Blender 스킨 캐릭터 2개, 유효 Humanoid Avatar, 직접 제작 동작 16개, Mixamo 업로드용 무골격 T-pose. 실제 Mixamo 달리기/점프/숙이기/비틀거림 4개를 별도 다운로드하여 리타게팅했다. 자체 채굴/아이디어 동작도 유지한다.
- 손 관절에 미터 단위 소켓, 앞으로 내려찍는 채굴 동작, 접힌 손가락 1인칭 손, 판자/철제 테두리/리벳/원형 바퀴 광산 수레.
- 기존 ProBuilder 6.1.2로 13개 경로 편집 원본과 실제 런타임 베이크 연결. 바닥 정점 일치/런타임 편집 컴포넌트 없음 검증. 원본을 편집 후 베이크 메뉴로 다시 반영할 수 있다.
- 새 머리/전구가 클로즈업에 잘리지 않도록 구도 수정. 16:9·넓은 창·4:3 실제 Player 캡처 38장 픽셀 검증 PASS.
- EditMode 24/24, Mixamo 적용 후 PlayMode 10/10 PASS, Windows 빌드 약 213 MiB. 외부 Humanoid 상태 연결 8개 검증 PASS. 최신: docs/TOOLS_V4.md, docs/TOOLS_V4_QA.md, docs/previews/ArtV4/index.html.
- Adobe 로그인 완료, Mixamo 실제 적용 완료. 출처/다운로드 설정/원본 SHA256은 References/MixamoDownloads/Provenance.json. Mine 상품 미임포트 상태 유지. 참고 이미지와 환경/손/표정/조명 품질 차이는 남는다.

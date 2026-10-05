# 무한옥 구현 계획 (2026-10-03)

## 최신 V9 · 카트 철로와 계단에서 감옥으로 이어지는 슬라이딩

광산의 카트 길을 단선 절벽 다리로 분리했다. 네 개의 한쪽 레일 붕괴는 반대쪽 몸 기울이기로 통과하고 일반 장애물은 카트에 배치하지 않는다. 하차 → 계단 하이니 → 내려오는 전폭 문 아래 슬라이딩 → 감옥 복도 순서로 이어진다. 문은 통과 순간 0.95m 틈을 남기고 뒤에서 바닥까지 닫힌다. 점프/숙이기/하이니 전용 행, 바나나 누적 2회 후 1.2초 추격/잡힘을 구현했다. 상세 ART_V9.md / ART_V9_QA.md. 무료 모델/자체 제작과 상용 benchmark의 품질 차이는 남는다.

## 최신 V8 · 사용자 규칙과 연속 공간

정상 시작은 제공 로딩 이미지를 인트로로 사용하고 바로 플레이한다. 고체 장애물 한 번 충돌 종료, 바나나/물웅덩이 2.4초 50% 감속, 두 청크 연속 수레 조향/숙이기/하차를 구현했다. 빈 수레 Blender 원본, 서비스 출구/사육장 수목과 밤하늘/HUD를 실제 적용한다. 상세 ART_V8.md, 검증 ART_V8_QA.md가 이전 흐름보다 우선한다. 무료 경찰은 공식 0원 배포 주문 화면까지 준비했으나 필수 개인정보 직접 처리 대기이고, 현재 자체 모델이다. 상용 수준의 전체 그래픽 완성도는 미달이다.

## 최신 V7 · 로딩과 실제 모델 연결

실제 연결 스킨 원숭이와 FBX 표정 2개, 불규칙 암벽·암석 재질 6개·목재 대비·접촉 음영, 사육장 바깥 지면과 상세 무료 수목을 적용했다. 첫 3D 장면을 준비한 뒤 로딩 이미지에서 0.85초간 전환하고 기존 8초 인트로를 시작한다. 상세 구현/남은 품질 차이는 ART_V7.md. 정상 다운로드 인증이 실패한 무료 경찰은 미임포트 상태이며 현재 경찰은 자체 Blender 모델이다. 상용 benchmark와 같은 품질로 판정하지 않는다.

## V6 품질 개선 · 2026-10-04

서브웨이 서퍼스와 링피트를 품질 기준으로 삼는다. 실제 맵에 생성 albedo atlas를 투영하고 Blender 손과 동작 반응, 가까운 점광원 두 개의 그림자 예산을 적용했다. 상세 구현은 docs/ART_V6.md. 무료 BitGem 경찰 107256은 Unity 로그인 확인 후 약관 동의 대기이며 아직 임포트하지 않았다. Higgsfield 실행 도구는 현재 연결되어 있지 않다. 상용 벤치마크 수준에 도달한 것으로 판정하지 않는다.

## 최신 사용자 경로 · V5

광산 → 계단 → 감옥 복도 → 사육장 순서가 이전 13개 구간 출현 계획보다 우선한다. 기존 13개 편집 원본은 보존하고 10개 청크를 반복한다. 선택 몸무게 입력, 제공 이미지 로딩, 바닥까지 닫히는 문과 예상 칼로리 HUD를 추가했다. 현재 구현/남은 아트 차이는 docs/ART_V5.md, 검증은 docs/ART_V5_QA.md. 유료 원숭이를 구매하지 않고 무료 경찰 다운로드는 재로그인 대기 상태다.

## Phase 0 조사

실제 프로젝트: `Unity/`, Unity **6000.3.10f1**. 기존 장면 `RunnerMVP`, `CellBlockVerticalSlice`는 보존한다. 기존 AGENTS/ARCHITECTURE/DEVELOPMENT_PLAN/ART_DIRECTION과 첨부 명세를 읽었다. 이번 명세의 UDP 및 1인칭 요구가 기존 WebSocket/3인칭 문서보다 우선한다.

| 영역 | 현재 기능 | 재사용 / 보완 |
|---|---|---|
| Core | LaneRules, IGameInput, RunnerInput | 레인 경계 규칙 재사용, 명령 공통화 |
| Runner | PlayerMovement, PlayerActionController | 기존 시연 유지, 새 순수 규칙과 view 어댑터 추가 |
| Map | MapGenerator, MapChunk, ObstacleSocket | 기존 풀링/안전 행 패턴을 따르는 24m/2.2m 콘텐츠 추가 |
| Camera | RunnerCamera, RunnerCinemachineCamera | Cinemachine 3.1.7 API 재사용, 1인칭 전환 추가 |
| Art | CellBlock 모듈, Crate/Pipe/Barricade | 감옥 환경과 장애물 재사용 가능한 참조 유지 |
| Pose | 비어 있음 | Python Landmarker + UDP receiver 추가 |
| Flow/UI | 거리 HUD | title/intro/caught/result/retry와 최고 기록 추가 |
| Tests | Editor smoke checks | 순수 규칙 및 새 GameScene smoke 추가 |

asmdef 없음. 실제 캐릭터 FBX/Animator 없음. 누락 asset은 명세 ID의 primitive prefab으로 먼저 구현하며 상용 에셋을 구매했다고 간주하지 않는다. 기존 씬의 직렬화된 참조는 유지한다.

## 패키지와 위험

manifest: Cinemachine 3.1.7, InputSystem 1.18.0, ProBuilder 6.1.2, URP 17.0.1. 첨부 문서의 URP 17.3.0으로 업그레이드하지 않는다. Timeline은 현재 설치되지 않았다. 패키지 변경 금지를 지켜 우선 Unity 내장 Playables의 8초 director로 연출을 구현하고, Timeline 패키지 기반 authoring은 별도 미완료 항목으로 기록한다. 패키지 변경 없이 Timeline 자체 구현 완료라고 보고하지 않는다.

Unity batch compile/PlayMode/build가 실제로 성공해야 검증 완료다. 라이선스 초기화, 네트워크, build module 문제는 소스 오류와 구분한다. 웹캠은 실제 장비 검증이 별도로 필요하다. 기존 캐릭터 패키지는 로컬에 없어 정품 임포트 후 rig/clip retargeting 필요.

## 단계

1. Phase 1: 순수 Flow/RunSession/RunnerController/Chase/Score, keyboard, view, result/retry.
2. Phase 2: 13개 24m 청크 ID, Entry/Exit/lanes/sockets, prewarm pool, 안전 패턴/difficulty.
3. Phase 3: 같은 GameScene에서 pickaxe/cart/idea/escape/guard/continuous first-person takeover.
4. Phase 4: UDP JSON 검증, timeout/debounce/cooldown, Python MediaPipe sender.
5. Phase 5~6: chase/best/HUD, asset manifest, prefab 교체, VFX/audio fallback.
6. Phase 7~8: 순수 로직, 실제 PlayMode 100+ 청크/flow/retry/UDP, build smoke.

## 파일 계획

기존 시연 코드는 보존. `Assets/Game/{Domain,Bootstrap}` 및 기존 Application/Infrastructure/Presentation 하위에 무한옥 전용 타입 추가. 기존 Core의 LaneRules 재사용. `Content`에 ID별 prefab/config, `Scenes/GameScene.unity`, `Editor/MuhanokContentBuilder.cs`, `Editor/MuhanokSmokeCheck.cs`, `PoseServer` Python 및 tests, `docs/PROGRESS.md`, `docs/ASSET_CATALOG.md` 작성. `README` 실행 경로 갱신. 각 단계 후 compile 확인 및 결과 기록.

## 필수 패키지 추가 사유 (compile 결과 이후)
실제 manifest에는 Audio/Animation/Director/ParticleSystem 내장 모듈도 꺼져 있어 해당 타입을 컴파일할 수 없다. 필수 애니메이션/소리/인트로/VFX 구현을 위해 이 내장 모듈 1.0.0을 활성화한다. 명세의 Timeline을 구현하기 위해 com.unity.timeline 1.8.10을 추가한다 (Unity 6 공식 지원 버전). 기존 패키지 버전은 변경하지 않는다. 영향: Timeline 편집/런타임 assembly와 내장 모듈 활성화만 추가. 최초 계획의 Playables fallback은 정식 Timeline track/signal로 보완한다.
기존 RunnerSceneBootstrap을 GUID를 유지해 Bootstrap 폴더로 이동하여 Presentation → Infrastructure 의존을 composition root로 옮긴다. 기존 장면과 구현은 보존한다.

## Blender · ProBuilder · Mixamo V4
사용자가 선택한 세 도구로 진행한다. Blender 스킨 캐릭터/자체 동작/소품과 기존 ProBuilder 6.1.2 편집 원본·베이크 연결을 실제 적용했다. Adobe 로그인 완료 후 실제 Mixamo 동작 4개를 다운로드하여 자체 Humanoid 캐릭터에 적용했다. 원본·설정·해시는 References/MixamoDownloads/Provenance.json에 기록했다. 현재 상태와 검증은 docs/TOOLS_V4.md, docs/TOOLS_V4_QA.md를 우선 읽는다. 원본 ProBuilder 구조는 덮어쓰지 않으며 저장 후 베이크 메뉴로 반영한다. 실제 외부 동작 상태 연결과 두 캐릭터의 PlayMode 동작도 검증한다. 참고 이미지 수준의 최종 아트 차이는 남은 작업으로 유지한다.


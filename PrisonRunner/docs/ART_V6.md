# V6 · 서브웨이 서퍼스 / 링피트 품질 기준

2026-10-04. 사용자는 현재 실제 맵 품질이 낮다고 평가했고, 서브웨이 서퍼스의 캐릭터/환경/애니메이션 완성도와 링피트의 운동 표현을 벤치마크로 지정했다. 사용자 지정 Asset Store 모델 중 무료 Police Officer 107256만 우선 적용하며 유료 원숭이는 구매하지 않는다. 기존 광산 → 계단 → 감옥 복도 → 사육장, 선택 몸무게와 예상 칼로리 요구는 유지한다.

## 제작 기준과 실제 상태

벤치마크 게임과 동일한 최종 품질에 도달했다는 주장을 하지 않는다. 자동 게임 규칙 테스트 통과와 그래픽 완성도는 별개다. 실제 실행 파일의 동일 구도 사진으로 이전 버전과 비교한다. 로딩 이미지와 실제 모델 렌더를 구분한다.

- 단색 표면을 개선하기 위해 이미지 생성 도구로 암석/목재/콘크리트/사육장 포장석 albedo atlas를 제작했다. 실제 메시에 월드 좌표로 투영해 기존 FBX UV가 늘어나는 문제를 피한다. 재질은 Staging에서 확인 후 장면에 연결한다.
- Blender에서 손바닥 모서리와 손가락을 둥글게 재제작했다. 런타임 손은 점프·숙이기·충돌·레인 변경에 부드럽게 반응하고 수레 구간은 반복 경로에서도 맞춰진다.
- 가까운 두 개의 점광원만 512px 그림자를 사용해 기존 2048px atlas의 자동 축소를 피한다. 점광원당 6면이 필요하다. 태양/야외 방향광과 기존 그림자 atlas 크기는 유지한다.
- 먼 표지판은 안개와 가시 거리 영향을 받아 여러 구간의 글씨가 멀리까지 겹치는 현상을 줄인다.
- 실제 Mixamo 달리기/점프/숙이기/비틀거림 4개는 유지한다. 무료 경찰 모델을 임포트하면 같은 gameplay 이동/상태 이름에 리타게팅하고 실제 관절 움직임과 신체 크기를 검증한다.

## 도구와 외부 에셋

- Blender 4.5.9, 기존 ProBuilder 6.1.2, 실제 Mixamo 클립 및 내장 이미지 생성 도구를 사용한다.
- [Higgsfield 공식 MCP/플러그인 안내](https://higgsfield.ai/mcp)를 확인했다. 현재 세션에 Higgsfield 실행 도구가 연결되어 있지 않아 사용했다고 표시하지 않는다. 제공되는 플러그인 검색 호출도 현재 도구 목록에는 없다. 추가 유료 구독/크레딧을 구매하지 않는다.
- [무료 Police Officer / BitGem](https://assetstore.unity.com/packages/3d/characters/police-officer-proto-series-107256): Unity 로그인 및 2026-10-04 상품 취득 표시를 확인했다. 브라우저 취득과 실제 파일 임포트를 구분한다. 배치 편집기 다운로드 서비스는 계정 미로그인으로 응답했으며 GUI 편집기에서 확인한다. 파일 확보/실제 임포트 전에는 적용 완료로 기록하지 않는다.

## 파일

생성: MuhanokVisualPolish.cs, HandPaintedTriplanar.shader, PointShadowBudget.cs, VisualBudgetTests.cs, Assets/External/Staging/GeneratedV6/HandPaintedAtlas.png.

수정: build_blender_modules.py 및 Blender 모델/.blend, DepthText.shader, FirstPersonHands.cs, MuhanokArtUpgrade.Maps.cs, RuntimeVisualReview.cs, PlayMode assembly 참조와 갤러리/검수 도구. 실제 직렬화된 재질/프리팹/GameScene도 적용 도구가 저장한다.

생성 재질의 도구와 프롬프트는 [출처 기록](../References/GeneratedV6/Provenance.md)에 있다. 실제 최종 검사 결과는 검증 후 별도로 기록한다.

현재 검증은 [ART_V6_QA](ART_V6_QA.md), 실제 실행 화면 38장은 [갤러리](previews/ArtV6/index.html). PlayMode 13/13 및 Windows 실행/120개 청크 smoke 통과. 첫 GPU 검수에서 발견한 Forward+ 조명 variant 누락을 수정하고 모든 화면을 다시 검수했다.

무료 경찰은 계정 취득이 확인됐지만 아직 임포트 전이다. 사용자는 Unity에서도 로그인 상태라고 확인했다. 정상 Package Manager API와 브라우저의 외부 앱 연결은 별개다. `Unity에서 열기` 브라우저 연결은 보안 정책이 차단했다. 기존 Unity 프로젝트 안의 정상 다운로드 서비스 상태를 확인한다. 인증 토큰을 추출하거나 보안 검증을 끄지 않는다. 기존 자체 경찰이 아직 화면에 표시된다.

실제 GUI 편집기 서비스 확인 결과도 `signedIn=False / batch=False`다. 사용자에게 재로그인을 반복 요청하지 않고 Hub 표시와 Asset Store 연결의 차이를 기록한다. My Assets의 정상 다운로드 완료 후 무료 경찰 적용을 이어가야 한다. 현재 품질 개선본은 GameScene에서 바로 실행할 수 있다.

## 품질 차이와 다음 검수

원숭이의 연결된 신체 실루엣과 표정, 계단의 파손 디테일, 사육장의 동물 골격 동작, 환경 조명/반복감과 운동 코칭 표현은 상용 벤치마크 수준까지 추가 제작이 필요하다. 자동으로 통과했다고 간주하지 않는다. 실제 웹캠 입력의 운동량 추정/정확도도 사람 검증이 필요하다.

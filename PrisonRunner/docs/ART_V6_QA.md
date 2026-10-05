# V6 실제 검증 · 2026-10-04

이 문서는 실제 적용/실행 결과이며 상용 벤치마크 품질 인증이 아니다.

- Unity 6000.3.10f1 PlayMode: 13/13 PASS, `Unity/Logs/MuhanokPlayModeV6.xml`. 기존 맵/입력/문/선택 몸무게/애니메이션 검증과 실제 풀링된 새 재질 및 점광원 그림자 상한 검증.
- Windows 빌드: PASS, 250,060,283 bytes, `Unity/Logs/ArtV6Build.log`. 31 prefab 참조, 실제 Blender Humanoid/Mixamo 연결, 13개 ProBuilder 경로와 베이크 메시 검수 통과.
- 실제 Windows GPU 화면: 38장 PASS. 1280×720에서 시작/몸무게/로딩/인트로/1인칭 6장 + 10개 맵/연결 경계 20장. 1702×726과 1024×768에서 각각 6장. 비어 있는 이미지와 잘못된 16:9 여백을 검사했다. `docs/previews/ArtV6/capture-verification.json`.
- Runtime smoke: PASS, `Unity/Logs/ArtV6RuntimeSmoke.log`. title → 8초 Timeline → 1인칭 → 120개 청크 → caught/result/retry.
- 실제 화면 첫 검수에서 새 재질이 어두워지는 문제를 발견했다. 설치된 URP의 Forward+ cluster 조명 variant를 추가하고 재빌드/38장 재검수했다. 현재 바닥·목재·콘크리트와 사육장 포장석의 재질/조명 정상 표시를 확인했다.
- 최종 빌드/플레이 캡처의 C# compile error / shader error / unhandled exception 없음. 기존 점광원 shadow atlas 자동 축소 경고도 최종 3종 캡처에서 관찰되지 않았다.
- 게임 규칙은 키보드/자동 입력으로 검증했다. 실제 웹캠 운동 코칭/칼로리 정확도는 검증하지 않았으며 예상 칼로리로 표시한다.

## 경찰과 도구 상태

무료 Police Officer 107256은 Asset Store의 2026-10-04 취득 표시를 확인했다. 증거 `docs/previews/ArtV6/PoliceAcquired.png`. 실제 파일 다운로드/임포트 전이다. Unity Package Manager의 정상 다운로드 서비스를 사용했으며 브라우저 쿠키/로그인 토큰을 읽지 않았다. 배치 편집기는 미로그인으로 응답해 GUI 편집기에서 확인한다. 현재 플레이 화면의 경찰은 기존 자체 Blender 모델이다.

Higgsfield 실행 도구는 연결되어 있지 않다. Blender, ProBuilder, 실제 Mixamo 4개, 내장 이미지 생성 재질은 사용했으나 Higgsfield 사용으로 기록하지 않는다.

Asset Store의 `Unity에서 열기` 외부 앱 연결은 브라우저 보안 정책에 의해 거부됐다. 이 연결을 우회하지 않는다. 기존 Unity 프로젝트 내부의 정상 Package Manager API로 상태를 확인한다. GUI 편집기의 패키지 검색에서 인증서 검증 오류도 관찰됐으며 인증서 검증을 끄지 않았다. 사용자는 편집기에서도 로그인 상태라고 확인했다.

추가 확인: GUI 편집기 자체의 IUnityConnectProxy.isUserLoggedIn은 False였다(`MUHANOK_EDITOR_ASSET_ACCOUNT: signedIn=False / batch=False`). Hub의 로그인 표시와 이 프로젝트의 Asset Store 다운로드 인증을 구분한다. 정상 서비스가 auth 요청을 거부해 모델 파일을 확보하지 못했다. 사용자가 Package Manager → My Assets에서 Police Officer 다운로드를 직접 완료하면 캐시 파일 검수/임포트를 이어갈 수 있다. 자동 다운로드 도구의 최종 컴파일은 오류 없이 완료했다.

## 시각적 차이

생성 재질은 실제 3D 표면에 투영되지만 암벽의 큰 단면과 동물의 단순한 신체, 손의 폼, 원숭이 표정/신체 실루엣, 계단 파손/장식, 반복되는 구성은 개선이 더 필요하다. 참고 이미지/서브웨이 서퍼스/링피트와 동등한 최종 품질이 아니다. 다음 우선순위는 취득한 경찰의 실제 Humanoid/Mixamo 리타게팅 검수다.

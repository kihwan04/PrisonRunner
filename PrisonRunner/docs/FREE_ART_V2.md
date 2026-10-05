# 무한옥 무료 에셋 적용 기록

> 이전 V2 기록. 최신 화면 비율·곡선/계단·Blender·야간 사육장과 Asset Store 상태는 [ART_V3](ART_V3.md)를 따른다.

2026-10-03 사용자 선택: **무료 에셋만 사용**. 유료 상품 구매와 유료 플러그인 설치는 하지 않았다.

## 실제 다운로드·임포트한 에셋

제작자 공식 배포 ZIP에서 FBX·PNG·라이선스만 `Unity/Assets/External/Staging/Kenney`에 임포트했다. 외부 스크립트는 임포트하지 않았다. 총 **403 FBX**, CC0. 다운로드별 SHA256은 `FREE_ASSETS_IMPORTED.json`에 기록했다.

| 공식 출처 | 실제 FBX 수 | 적용 내용 |
|---|---:|---|
| [Survival Kit](https://kenney.nl/assets/survival-kit) | 80 | 광산 암석, 상자, 통, 작업대, 곡괭이 |
| [Factory Kit](https://kenney.nl/assets/factory-kit) | 143 | 정비 설비, 배관, 보안 모니터, 스캐너, 측면 계단 |
| [Furniture Kit](https://kenney.nl/assets/furniture-kit) | 140 | 감방 이층침대, 세탁기, 건조기, 싱크, 조리 설비, 벤치 |
| [Modular Cave Kit](https://kenney.nl/assets/modular-cave-kit) | 40 | 임포트 완료, 현재 맵에서는 직접 인스턴스화하지 않음 |

임포트 도구: `Tools/AssetPipeline/import_free_kits.py`. 모델의 피벗·높이를 정규화하고 원본 텍스처를 유지한 URP 재질로 변환했다. 일부 소품은 게임 색상에 맞는 재질을 적용했다. Asset Store에서 받은 패키지라는 의미는 아니다.

## 무료 Asset Store 후보와 상태

| 후보 | 상태 |
|---|---|
| [Mine — Gregory Seguru](https://assetstore.unity.com/packages/3d/environments/dungeons/mine-92461) | 무료 확인. ‘내 에셋에 추가’ 후 Unity 로그인으로 이동. 계정 인증 필요, 미다운로드·미임포트. 구버전 원본이므로 URP 검수 필요 |
| [Quirky Series - FREE Animals Pack](https://assetstore.unity.com/packages/3d/characters/animals/quirky-series-free-animals-pack-178235) | 무료/URP 표기 확인. 미다운로드. 원숭이 포함 여부와 rig/clip은 실제 패키지 검수 전 미확정 |
| [Cartoon FX Remaster Free](https://assetstore.unity.com/packages/vfx/particles/cartoon-fx-remaster-free-109565) | 기존 조사 후보, 미임포트 |

Unity 로그인 화면을 브라우저에 열어 사용자에게 인증을 요청했다. 로그인 정보는 입력하거나 조회하지 않았다. 유료 후보는 이번 범위에서 제외했다.

## 맵과 캐릭터

| ID | 현재 아트 |
|---|---|
| CH_Mine_Start / CH_Mine_Straight_A / B | 암석, 목재 지지대, 3레인 레일·침목·볼트, 광석, 작업 소품, 따뜻한 랜턴 |
| CH_Transition_MineToPrison | 광산 12m + 시설 통로 12m, 목재/금속 프레임 |
| CH_Prison_Corridor_A / B | 콘크리트 벽, 감방 철창, 이층침대, 서비스 배관 |
| CH_Checkpoint | 열린 보안 게이트, 측면 스캐너와 모니터, 경고등 |
| CH_Stair | 중앙 러너 경로, 측면 계단·난간, HighKnee 장애물 |
| CH_Laundry | 실제 세탁기/건조기 모델, 카트와 작업 소품 |
| CH_Kitchen | 실제 싱크·조리 설비·환기 후드·가구 |
| CH_Maintenance | 실제 기계·배관·작업 설비 |
| CH_Yard / CH_OuterWall | 울타리·벤치·외벽·감시탑·시설 복귀 캐노피 |
| CHR_Monkey_Prisoner / CHR_Police_Officer | 자체 제작 관절형 로우폴리 모델. 외부 캐릭터 rig가 아님 |
| PROP_MineCart | 자체 제작 금속 수레, 바퀴·리벳·광석·전조등 |
| OBS_* | 상자/배관/줄무늬 차단물/수레/레이저/계단 |
| FP_Hands | 자체 제작 1인칭 손·팔, 상태별 표시와 달리기 움직임 |
| 코인 | 회전 금색 토큰, 획득 +10점, 풀 재사용 초기화 |

청크 13종 모두 길이 24m, 바닥 높이 0, 레인 -2.2/0/+2.2를 유지한다. 자체 생성 환경 메시를 재질별로 결합하고 기존 스폰·충돌 판정을 보존했다. 실내/실외에서 안개와 환경광을 보간한다. 인트로→본편 사이 씬 재로딩은 없다.

## 생성 로고

`Unity/Assets/Game/Content/ArtV2/Textures/MuhanokLogo.png`: ImageGen 내장 생성 후 투명 배경 편집. RGBA 검사와 시각 검수 후 타이틀에 연결했다. 게임 화면을 대신하는 이미지는 아니다.

생성 프롬프트 요약: “Transparent production game logo, exact Korean text 무한옥, chunky chipped stone letters, ivory 무한 and vermilion 옥, small banana, pickaxe and gold ore details, warm amber rim lighting, clean silhouette, no background.” 편집: “Remove all brown backing/shadows outside the logo; preserve lettering and decorative silhouette; fully transparent surrounding pixels.”

## 남은 품질 차이

무료 모델과 자체 제작 아트로 개선한 플레이 가능한 버전이다. 참고 이미지 수준의 캐릭터 모델링·스키닝·표정, 광산 암벽의 형태와 재질 밀도, 정교한 연출·음향까지 완성했다고 볼 수 없다. 실제 화면은 `docs/previews/ArtV2`에서 확인할 수 있다. 다음 우선순위는 무료 rigged 캐릭터 검수와 광산 지형의 디테일 보강이다.

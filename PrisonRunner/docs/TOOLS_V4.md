# 무한옥 Blender · ProBuilder · Mixamo 작업

2026-10-03. 무료 도구와 자체 모델만 사용한다. 참고 이미지와 같은 품질에 도달했다는 의미는 아니다. 이전 CC0 환경 에셋과 16:9·연속 경로 구현을 유지한다.

## 실제 적용한 것

- Blender 4.5.9 LTS에서 원숭이/경찰 모델, 스킨 메시와 Humanoid 뼈대를 제작했다. 큰 눈, 눈 하이라이트, 입/치아, 귀, 주황색 죄수복, 등번호 0723, 경찰 제복/모자/배지/선글라스/콧수염을 포함한다.
- 기본 동작 16개는 **직접 제작한 Blender 동작**이다. Mixamo에서 다운로드한 동작으로 표시하지 않는다. 기존 Animator 상태 이름은 유지하고 OverrideController로 새 FBX 동작을 적용한다.
- Adobe 로그인 완료 후 Mixamo에서 실제 동작 4개를 다운로드했다. `Running`(Running With Intention), `Jump`(Jumping In Place), `Crouching Idle`(Low Crouching Idle), `Jogging Stumble`(Brief Stumble While Jogging)을 원숭이/경찰 Humanoid에 리타게팅한다. 채굴·아이디어 등 나머지 동작은 자체 Blender 동작이다.
- 사람형 오른손에 미터 단위 소켓을 추가했다. FBX 뼈대의 단위 스케일이 곡괭이에 전파되지 않는다. 앞으로 내려찍는 채굴 자세와 손가락을 접은 1인칭 손을 적용했다.
- Blender에서 나무 판자/철제 테두리/리벳/원형 바퀴/광석이 있는 열린 광산 수레를 제작했다. 기존 수레 프리팹 GUID, 동작과 오디오는 보존했다.
- 설치되어 있던 ProBuilder **6.1.2**를 사용했다. 버전을 변경하지 않았다. 13개 편집용 경로에 0.5m 단위 바닥 면, 입구/출구 프레임과 계단 라이저가 있다. 곡선과 높이 변화는 기존 RouteSurface 규격을 따른다.
- 편집용 ProBuilder 메시를 별도 정적 메시로 구워 실제 게임의 `RouteStructure`에 적용한다. 플레이 프리팹에는 ProBuilder 컴포넌트가 남지 않는다. 기존 24m 소켓, 3레인, 장애물 회피와 풀링을 유지한다.
- 코인 발광을 줄여 바나나 문양과 테두리를 읽기 쉽게 했다.

## 원본과 편집

| 파일/폴더 | 용도 |
| --- | --- |
| `References/Blender/MuhanokCharacters.blend` | 캐릭터/뼈대/동작 원본 |
| `References/Blender/MuhanokModules.blend` | 암벽·손·동물·수레 원본 |
| `Tools/AssetPipeline/build_blender_characters.py` | 캐릭터와 동작 재생성 |
| `Tools/AssetPipeline/build_blender_modules.py` | 환경 소품 재생성 |
| `Unity/Assets/External/Staging/Blender/Characters` | 캐릭터 2 FBX, 동작 16 FBX, Mixamo 업로드용 무골격 T-pose 1 FBX |
| `Unity/Assets/Game/Authoring/ProBuilder` | 13개 편집용 프리팹/지속 저장 메시 |
| `Unity/Assets/Game/Editor/MuhanokToolUpgrade.cs` | Humanoid 임포트·편집용 맵·런타임 베이크·검증 |
| `Unity/Assets/Game/Editor/MuhanokMixamoImport.cs` | 실제 다운로드 파일이 있을 때만 동작 교체 |
| `docs/previews/ArtV4/index.html` | 실제 Unity 캡처 검수 |

Unity에서 편집용 프리팹을 열고 ProBuilder로 수정한 뒤 저장한다. `Muhanok > Bake Edited ProBuilder Routes Into Game`으로 플레이 프리팹에 반영한다. 기존 편집용 프리팹은 전체 업그레이드 메뉴로 덮어쓰지 않는다.

경로의 EntrySocket `(0,0,0)`, ExitSocket `(0,rise,24)`와 3개 레인 중심은 유지해야 한다. 임의로 길이/높이/곡선을 바꾸려면 Domain 경로 규격과 코인/장애물 소켓도 함께 변경해야 한다. 베이크 메뉴는 건축 메시 변경을 적용하며 이동 규칙을 자동 설계하지 않는다.

## Mixamo 실제 상태

Adobe 로그인이 완료되어 **실제 외부 FBX 4개를 다운로드하고 적용했다**. 기본 X Bot의 스킨 없는 동작을 자체 Blender Humanoid로 리타게팅했으며, 캐릭터를 외부 서비스로 업로드할 필요가 없었다. Adobe 공식 안내: https://helpx.adobe.com/creative-cloud/help/mixamo-rigging-animation.html

다운로드 설정은 FBX for Unity / Without Skin / 30fps / Keyframe Reduction none이다. 달리기와 비틀거림에는 In Place를 선택했다. 원본 사본과 선택한 동작 이름·SHA256은 `References/MixamoDownloads/Provenance.json`에 보존했다. 실제 적용 파일은 다음과 같다.

```
Unity/Assets/External/Staging/Mixamo/Animations/Run.fbx
Unity/Assets/External/Staging/Mixamo/Animations/Jump.fbx
Unity/Assets/External/Staging/Mixamo/Animations/Crouch.fbx
Unity/Assets/External/Staging/Mixamo/Animations/Stumble.fbx
```

다른 다운로드로 교체할 때 Unity의 `Muhanok > Import Downloaded Mixamo Motions`를 실행한다. 실제 파일이 없으면 현재 Blender 동작을 유지한다. 유효한 Humanoid/실제 클립을 검사한 뒤 원숭이와 경찰에 리타게팅하고 루트 이동을 잠근다. Run은 달리기/추격/출발/레인 이동, 나머지는 해당 상태에 매핑한다. 채굴·아이디어·기타 동작은 현재 Blender 동작을 유지한다. 빌드 검증은 두 OverrideController의 실제 외부 클립 연결도 검사한다.

## 남은 시각 작업

참고 이미지에 비해 환경의 반복, 암벽 형태, 그림자와 조명 깊이, 손의 실루엣, 캐릭터 세부 표정과 컷 연출 차이가 남는다. 기존 동물은 정적 모델이다. Mine Asset Store 상품은 약관 승인 대기로 여전히 미임포트 상태다. 현재 변경을 완성된 상용 아트로 보고하지 않는다.

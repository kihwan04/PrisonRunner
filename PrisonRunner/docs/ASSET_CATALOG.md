# 현재 에셋과 교체 기준

기준일: 2026-10-26. 실제 사용 리소스와 출처를 찾기 위한 목록입니다. 과거 상품 추천·가격·취득 경과는 Git 기록에 남아 있습니다.

| 영역 | 현재 리소스와 위치 | 교체 시 확인 |
| --- | --- | --- |
| 캐릭터·기본 동작 | 자체 Blender Humanoid. `References/Blender/`, `Unity/Assets/External/Staging/Blender/` | Humanoid 매핑, 손/발/머리 관절, 동작과 접지 |
| 게임 애니메이션 | Mixamo 달리기·점프·숙이기·비틀거림. [취득 설정·해시](../References/MixamoDownloads/Provenance.json) | 공개 소스에서 원본 FBX 제외, 자체 동작 대체 경로 유지 |
| 무료 환경 키트 | Kenney Factory/Furniture/Modular Cave/Nature/Survival. [출처·해시](FREE_ASSETS_IMPORTED.json) | CC0 라이선스 파일 보존, 크기·피벗·재질·충돌 |
| 환경 메시·재질 | 자체 Blender/ProBuilder 및 런타임 경로 메시. 생성 재질 [출처](../References/GeneratedV6/Provenance.md) | 청크 경계, 바닥 겹침, 표시 거리와 풀 재사용 |
| 몸동작 모델 | `Unity/Assets/Game/Content/Pose/Resources/MuhanokPose/` | BlazePose 검출/full/lite 모델과 샘플링 셰이더 참조 |
| 얼굴 모델 | `Services/Identity/models/` | YuNet/SFace manifest·라이선스, 다운로드 SHA-256 |
| 얼굴 사진 | 등록 동의 뒤 연결된 서버 DB에 저장 | 개인 데이터이므로 Git/배포 ZIP에 넣지 않음 |
| 소리 | 실행 중 코드로 합성 | 실제 스피커와 프리셋별 동작 확인 |

화면의 경찰은 자체 모델입니다. 과거 후보 상품의 취득 확인과 실제 파일 임포트를 혼동하지 않습니다. 새로운 구매나 다운로드를 수행한 목록이 아닙니다.

현재 화면 기준은 [2026-10-25 검수 보고서](reports/LOCAL_COMPLETION_2026_10_25.md)입니다(당시 캡처는 Git 기록에 있음). 리소스의 공개/재배포 조건은 [THIRD_PARTY_NOTICES.md](../THIRD_PARTY_NOTICES.md), 제작 기준은 [아트 방향](ART_DIRECTION.md)을 확인하세요.

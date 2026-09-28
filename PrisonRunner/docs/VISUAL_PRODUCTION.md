# Visual Production 환경

## 패키지와 제작 공간

Unity Package Manager의 공식 Registry/Built-in 패키지만 사용한다. URP와 Shader Graph는 기존 설치를 유지한다. ProBuilder와 Cinemachine은 누락된 경우에만 현재 Unity 버전과 호환되는 공식 패키지를 추가한다. Splines는 Cinemachine이 요구하는 공식 의존성으로만 추가한다. 기존 패키지 버전은 유지하며, 충돌이 발생하면 변경을 강행하지 않는다.

Meshy, Tripo, Higgsfield, Mixamo, Blender는 Unity Package가 아닌 외부 제작 도구다. 제작 결과 파일만 아래 Staging 경로로 가져온다. Asset Store 패키지와 외부 Git URL은 사용하지 않는다.

```text
Assets/
├─ Art/
│  ├─ Characters/
│  ├─ Environment/
│  │  └─ Blockouts/CellBlock, Corridor, Yard/
│  ├─ Obstacles/
│  ├─ Materials/Shaders/
│  ├─ Animations/
│  └─ VFX/
└─ External/Staging/
   ├─ Characters/
   ├─ Environment/
   └─ Obstacles/
```

ProBuilder는 이후 Blockouts 아래에서 감옥 구역의 크기와 시야를 검증할 때 사용한다. 현재 블록아웃 모델은 만들지 않는다. Shader Graph는 Materials/Shaders 아래에서 검수한 Stylized Material을 만들 때 사용한다. 현재 커스텀 Shader/Graph는 만들지 않는다.

## Staging → 검수 → Art

1. 외부 제작 파일을 종류별 `Assets/External/Staging`으로 임포트한다.
2. 단위/크기, Pivot, 전방 축, Material/Texture, Collider/Rigidbody, 리깅/애니메이션과 사용 권한을 검수한다.
3. 검수 완료한 파일과 관련 Texture/Material을 **Unity Project 창에서** 해당 `Assets/Art` 폴더로 이동한다. `.meta`와 GUID를 유지하고 외부 파일 관리자에서 재복사하지 않는다.
4. Production Prefab의 모델·Material·Texture 의존성이 Staging을 참조하지 않는지 확인한다.
5. 검수한 Art Prefab만 Bootstrap의 Visual 슬롯에 지정한다. Staging 에셋은 게임 장면에 직접 배치하거나 슬롯에 연결하지 않는다.

## Gameplay와 Visual 경계

`PlayerRoot`의 기존 CharacterController, Movement와 입력을 유지하고 `PlayerVisual` 모델만 교체한다. `MapChunk/Gameplay`의 바닥 Collider와 Socket도 유지하고 `MapChunk/VisualRoot`의 Prefab만 교체한다. 교체 슬롯과 Pivot/레인 규격은 [Visual 준비 문서](VISUAL_VERTICAL_SLICE.md)를 따른다. 슬롯이 비어 있으면 현재 Placeholder를 유지한다.

## 카메라 전환 준비

Cinemachine 설치는 기존 Runner Camera를 활성화된 Cinemachine Camera로 교체하지 않는다. 현재 Rig/Camera, 추적 값, FOV와 입력 감각을 유지한다. 이후 별도 전환 작업에서만 Cinemachine Camera와 Main Camera의 Brain을 연결하고 기존 추적값/FOV를 기준으로 비교한다. 두 카메라 제어를 동시에 활성화하지 않는다.

## 초기 Global Volume

`RunnerMVP`의 `Prison Visual Volume`은 Global 상태로 `PrisonVisualProfile`을 사용한다. 초기 구성은 Tonemapping(Neutral), Color Adjustments(노출/대비/색조/채도 0, 흰색 필터), Bloom(Intensity 0.15), Vignette(Intensity 0.12)의 네 가지다. Motion Blur 등 추가 효과는 사용하지 않는다. Main Camera의 Post Processing과 Volume Layer 연결은 유지한다.

## Unity 확인 순서

Package Manager에서 설치 상태와 에러를 확인한다. Project 창에서 Art/Staging 폴더가 보이는지 확인한다. `RunnerMVP`를 열어 Global Volume 프로필의 네 효과와 약한 강도를 확인하고, Play에서 Placeholder/기본 카메라/키보드 조작과 충돌이 유지되는지 확인한다. Cinemachine Brain 또는 활성 Cinemachine Camera는 아직 장면에 연결하지 않는다.

## 설치 확인 결과

| 패키지 | 실제 설치 버전 | 처리 |
| --- | --- | --- |
| URP | 17.3.0 (Editor Built-in) | 기존 유지. manifest의 17.0.1 선언도 변경하지 않음 |
| Shader Graph | 17.3.0 (URP 의존성) | 기존 유지 |
| ProBuilder | 6.1.2 | 공식 UPM 추가 |
| Cinemachine | 3.1.7 | 공식 UPM 추가 |
| Splines | 2.8.3 | Cinemachine 의존성으로 추가 |
| Settings Manager | 2.1.1 | ProBuilder 의존성으로 추가 |

기존 모든 패키지의 실제 버전은 설치 전후 동일하다. 패키지 충돌 없이 설치되었다. 다운로드 인증서 검증 문제는 Windows의 기존 신뢰 인증서를 설치 프로세스에만 전달해 처리했으며, TLS 검증과 시스템 신뢰 설정을 변경하지 않았다. 기존 검사에서도 나타난 Burst 해시 캐시 로딩 오류는 별도로 Console에서 확인한다.

## 검증 결과

Unity 6000.3.10f1 Batch Mode에서 C# 컴파일 오류 없이 Visual 교체/Collider 보존/초기 후처리 검사와 기존 Phase 2 맵 재사용 검사를 통과했다. 최종 활성/전체 청크는 7/8개, 장애물은 11개였다. Gameplay, 입력, 점수와 현재 Runner Camera 코드는 변경하지 않았다. MapChunk의 표시 루트 이름만 VisualRoot로 맞췄다. 그래픽 장치를 사용하지 않은 검사이므로 실제 화면은 Unity에서 확인한다. Burst 캐시 로딩 오류는 검사 로그에 남아 있으나 패키지 충돌은 없고 두 회귀 검사는 통과했다.

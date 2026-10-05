# 무한옥 Pose 입력

처음 받았다면 Python 3.11 또는 3.12를 설치하고 `SetupPose.cmd`를 더블클릭한다. 설치가 완료되면 `StartPose.cmd`로 카메라 입력 프로그램을 시작한다. `.venv`와 모델은 PC마다 설치하며 다운로드 ZIP에 포함하지 않는다. `requirements-lock.txt`는 기존 개발 PC에서 검증한 버전 기록이다.

Python 3.11 또는 3.12 환경에서 다음을 실행한다.

```powershell
python -m venv .venv
.venv\Scripts\python -m pip install -r requirements.txt
.venv\Scripts\python pose_sender.py --download-model
```

Unity GameScene을 Play한 뒤 웹캠 앞에서 전신이 보이게 중앙에 45프레임 서서 보정한다. 미러 화면 기준으로 좌우 몸 이동, 점프, 숙이기, 무릎 높이 들기를 인식한다. R 재보정 / ESC 종료. UDP는 localhost:5055, Unity GameSettings에서 변경 가능 (`--port`도 동일하게 변경).

플레이 중 웹캠 프레임이 끊기면 Lost heartbeat를 보내면서 약 2초마다 장치를 다시 연다. 연결이 돌아오면 보정을 다시 수행한다. 실제 장치 탈착/몸동작 정확도는 별도로 검증해야 한다.

카메라 없는 `--simulate` 입력 시뮬레이션과 단위 테스트는 외부 패키지 없이 가능하다. `--check`는 설치된 MediaPipe와 모델을 이용해 빈 프레임에서 추론을 검증한다:

```powershell
python pose_sender.py --simulate LEFT
python -m unittest discover -s tests -v
python pose_sender.py --check
```

모델 출처: [Google Pose Landmarker Python 공식 가이드](https://ai.google.dev/edge/mediapipe/solutions/vision/pose_landmarker/python). 모델은 `--download-model`로 Google의 공개 모델 저장소에서 내려받는다. 원본 프레임/관절 좌표는 Unity에 전송하지 않고 최종 명령만 전송한다. 이 구현의 실제 웹캠 정확도와 사용자별 threshold는 현장에서 확인해야 한다.

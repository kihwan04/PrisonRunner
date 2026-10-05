"""Webcam -> MediaPipe Tasks PoseLandmarker -> localhost UDP motion JSON."""
import argparse
import json
from pathlib import Path
import socket
import time
import urllib.request
from motion_detector import MotionDetector

MODEL_URL = "https://storage.googleapis.com/mediapipe-models/pose_landmarker/pose_landmarker_lite/float16/1/pose_landmarker_lite.task"


def packet(command, confidence):
    return json.dumps({"type": "motion", "command": command, "confidence": confidence, "timestamp": time.time()}, allow_nan=False).encode("utf-8")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--port", type=int, default=5055)
    parser.add_argument("--camera", type=int, default=0)
    parser.add_argument("--model", type=Path, default=Path(__file__).parent / "models/pose_landmarker_lite.task")
    parser.add_argument("--download-model", action="store_true")
    parser.add_argument("--check", action="store_true", help="Load the model and run one blank frame without opening a webcam")
    parser.add_argument("--simulate", choices=["CENTER", "LEFT", "RIGHT", "JUMP", "CROUCH", "HIGH_KNEE"])
    args = parser.parse_args()
    if not 1 <= args.port <= 65535:
        parser.error("port must be between 1 and 65535")
    if args.download_model and not args.model.exists():
        args.model.parent.mkdir(parents=True, exist_ok=True)
        urllib.request.urlretrieve(MODEL_URL, args.model)
    with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as udp:
        target = ("127.0.0.1", args.port)
        if args.simulate:
            # Re-arm edge commands with CENTER; hold commands get heartbeats.
            for command in ["CENTER", args.simulate, args.simulate, "CENTER"]:
                udp.sendto(packet(command, 0.95), target)
                time.sleep(0.4)
            return
        if not args.model.exists():
            parser.error("Missing model. Run with --download-model first.")
        import cv2
        import mediapipe as mp
        options = mp.tasks.vision.PoseLandmarkerOptions(
            base_options=mp.tasks.BaseOptions(model_asset_path=str(args.model)),
            running_mode=mp.tasks.vision.RunningMode.VIDEO,
            num_poses=1,
            min_pose_detection_confidence=0.65,
            min_pose_presence_confidence=0.65,
            min_tracking_confidence=0.65,
        )
        if args.check:
            import numpy as np
            with mp.tasks.vision.PoseLandmarker.create_from_options(options) as landmarker:
                image = mp.Image(image_format=mp.ImageFormat.SRGB, data=np.zeros((480, 640, 3), dtype=np.uint8))
                result = landmarker.detect_for_video(image, 0)
                print(f"POSE_MODEL_SMOKE_PASS: MediaPipe {mp.__version__}, OpenCV {cv2.__version__}, {len(result.pose_landmarks)} detections on blank frame")
            return
        detector = MotionDetector()
        capture = cv2.VideoCapture(args.camera)
        if not capture.isOpened():
            raise SystemExit("Webcam unavailable. Unity keyboard controls remain available.")
        start = time.monotonic()
        last_ms = -1
        try:
            with mp.tasks.vision.PoseLandmarker.create_from_options(options) as landmarker:
                while True:
                    ok, frame = capture.read()
                    now = time.monotonic()
                    if not ok:
                        udp.sendto(packet("CENTER", 0), target)
                        capture.release()
                        detector.reset()
                        print("Webcam lost; retrying. Unity keyboard controls remain available.")
                        # Keep lost heartbeats flowing and allow ESC while the device reconnects.
                        for _ in range(20):
                            udp.sendto(packet("CENTER", 0), target)
                            if cv2.waitKey(1) & 0xFF == 27:
                                return
                            time.sleep(0.1)
                        capture = cv2.VideoCapture(args.camera)
                        continue
                    # Mirroring makes screen left match the player's perceived left.
                    frame = cv2.flip(frame, 1)
                    image = mp.Image(image_format=mp.ImageFormat.SRGB, data=cv2.cvtColor(frame, cv2.COLOR_BGR2RGB))
                    ms = max(last_ms + 1, int((now - start) * 1000))
                    last_ms = ms
                    result = landmarker.detect_for_video(image, ms)
                    landmarks = result.pose_landmarks[0] if result.pose_landmarks else None
                    command, confidence, status = detector.detect(landmarks, now)
                    udp.sendto(packet(command, confidence), target)
                    cv2.putText(frame, f"{status} / {command} ({confidence:.2f})", (18, 32), cv2.FONT_HERSHEY_SIMPLEX, .7, (0, 220, 255), 2)
                    cv2.putText(frame, "Stand centered for calibration. R recalibrates. ESC exits.", (18, 64), cv2.FONT_HERSHEY_SIMPLEX, .5, (230, 230, 230), 1)
                    cv2.imshow("Muhanok Pose", frame)
                    key = cv2.waitKey(1) & 0xFF
                    if key == 27:
                        break
                    if key in (ord("r"), ord("R")):
                        detector.reset()
        finally:
            udp.sendto(packet("CENTER", 0), target)
            capture.release()
            cv2.destroyAllWindows()


if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        pass

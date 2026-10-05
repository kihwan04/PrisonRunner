"""Device-independent normalized MediaPipe landmarks -> game commands."""
from dataclasses import dataclass
import math


@dataclass
class DetectorConfig:
    calibration_frames: int = 45
    confidence: float = 0.65
    lean: float = 0.28
    crouch: float = 0.24
    jump: float = 0.16
    knee: float = 0.17
    smooth: float = 0.35


class MotionDetector:
    def __init__(self, config=None):
        self.config = config or DetectorConfig()
        self.reset()

    def reset(self):
        self.baseline = None
        self.samples = []
        self.filtered = None
        self.last_seen = None

    def detect(self, landmarks, now):
        if not landmarks or len(landmarks) < 29:
            if self.last_seen is not None and now - self.last_seen > 2:
                self.reset()
            return "CENTER", 0.0, "Lost"
        indices = (11, 12, 23, 24, 25, 26, 27, 28)
        confidence = min(getattr(landmarks[i], "visibility", 0.0) or 0.0 for i in indices)
        if not math.isfinite(confidence) or confidence < self.config.confidence:
            return "CENTER", 0.0, "Lost"
        self.last_seen = now
        ls, rs, lh, rh, lk, rk, la, ra = (landmarks[i] for i in indices)
        sx, sy = (ls.x + rs.x) / 2, (ls.y + rs.y) / 2
        hy = (lh.y + rh.y) / 2
        width = max(abs(ls.x - rs.x), 0.08)
        features = (sx, sy, hy, min(lk.y - lh.y, rk.y - rh.y), width)
        if not all(math.isfinite(v) for v in features):
            return "CENTER", 0.0, "Lost"
        if self.baseline is None:
            self.samples.append(features)
            if len(self.samples) >= self.config.calibration_frames:
                self.baseline = tuple(sum(s[i] for s in self.samples) / len(self.samples) for i in range(5))
                self.filtered = features
            return "CENTER", 0.0, "Calibrating"
        a = self.config.smooth
        self.filtered = tuple(a * v + (1 - a) * old for v, old in zip(features, self.filtered))
        x, y, hip, knee, _ = self.filtered
        bx, by, bh, bk, scale = self.baseline
        if (y - by) / scale > self.config.crouch:
            command = "CROUCH"
        elif (by - y) / scale > self.config.jump and (bh - hip) / scale > self.config.jump:
            command = "JUMP"
        elif (bk - knee) / scale > self.config.knee:
            command = "HIGH_KNEE"
        elif (x - bx) / scale < -self.config.lean:
            command = "LEFT"
        elif (x - bx) / scale > self.config.lean:
            command = "RIGHT"
        else:
            command = "CENTER"
        return command, min(1.0, confidence), "Connected"

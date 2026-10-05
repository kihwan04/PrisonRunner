import unittest
from types import SimpleNamespace
from motion_detector import MotionDetector, DetectorConfig


def pose(x=0, y=0, knee=0, visibility=.95):
    result = [SimpleNamespace(x=.5, y=.5, visibility=visibility) for _ in range(33)]
    for index, px, py in [(11, .4, .3), (12, .6, .3), (23, .43, .55), (24, .57, .55), (25, .43, .75), (26, .57, .75), (27, .43, .95), (28, .57, .95)]:
        result[index].x, result[index].y = px + x, py + y
    result[25].y += knee
    return result


class MotionTests(unittest.TestCase):
    def setUp(self):
        self.detector = MotionDetector(DetectorConfig(calibration_frames=2, smooth=1))
        self.detector.detect(pose(), 0)
        self.detector.detect(pose(), .1)

    def test_commands(self):
        for expected, landmarks in [("CENTER", pose()), ("LEFT", pose(x=-.1)), ("RIGHT", pose(x=.1)), ("JUMP", pose(y=-.05)), ("CROUCH", pose(y=.06)), ("HIGH_KNEE", pose(knee=-.08))]:
            self.assertEqual(self.detector.detect(landmarks, 1)[0], expected)

    def test_low_confidence(self):
        self.assertEqual(self.detector.detect(pose(visibility=.1), 1)[1], 0)

    def test_disconnect_and_recalibrate(self):
        self.detector.detect(None, 4)
        self.assertEqual(self.detector.detect(pose(), 5)[2], "Calibrating")

    def test_nan(self):
        self.assertEqual(self.detector.detect(pose(x=float("nan")), 1)[2], "Lost")


if __name__ == "__main__":
    unittest.main()

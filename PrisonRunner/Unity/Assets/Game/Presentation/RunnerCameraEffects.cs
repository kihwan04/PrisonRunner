using Unity.Cinemachine;
using UnityEngine;

namespace PrisonRunner.Presentation
{
    public sealed class RunnerCameraEffects : CinemachineExtension
    {
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private Vector3 eulerOffset;
        [SerializeField] private float fieldOfViewOffset;

        public void SetPresentationEffects(Vector3 position, Vector3 rotation, float fov)
        {
            positionOffset = position;
            eulerOffset = rotation;
            fieldOfViewOffset = fov;
        }

        public void ResetPresentationEffects()
        {
            SetPresentationEffects(Vector3.zero, Vector3.zero, 0f);
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase camera,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage != CinemachineCore.Stage.Finalize) return;
            state.PositionCorrection += state.RawOrientation * positionOffset;
            state.OrientationCorrection *= Quaternion.Euler(eulerOffset);
            var lens = state.Lens;
            lens.FieldOfView = Mathf.Clamp(lens.FieldOfView + fieldOfViewOffset, 1f, 179f);
            state.Lens = lens;
        }
    }
}

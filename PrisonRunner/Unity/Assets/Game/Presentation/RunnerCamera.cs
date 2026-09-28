using UnityEngine;

namespace PrisonRunner.Presentation
{
    public sealed class RunnerCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(0f, 4.5f, -8f);
        [SerializeField] private float followSharpness = 8f;
        [Header("Presentation Effects (neutral by default)")]
        [SerializeField] private Vector3 effectPositionOffset;
        [SerializeField] private Vector3 effectEulerOffset;
        [SerializeField] private float fieldOfViewOffset;
        private Transform target;
        private Camera outputCamera;
        private Vector3 baseCameraPosition;
        private Quaternion baseCameraRotation;
        private float baseFieldOfView;

        public void Configure(Transform player, Camera camera = null)
        {
            target = player;
            outputCamera = camera;
            if (outputCamera != null)
            {
                baseCameraPosition = outputCamera.transform.localPosition;
                baseCameraRotation = outputCamera.transform.localRotation;
                baseFieldOfView = outputCamera.fieldOfView;
            }
            transform.position = target.position + offset;
        }

        // Future shake and speed effects write here without changing the follow pose.
        public void SetPresentationEffects(Vector3 positionOffset, Vector3 eulerOffset, float fovOffset)
        {
            effectPositionOffset = positionOffset;
            effectEulerOffset = eulerOffset;
            fieldOfViewOffset = fovOffset;
        }

        public void ResetPresentationEffects()
        {
            SetPresentationEffects(Vector3.zero, Vector3.zero, 0f);
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
            transform.LookAt(target.position + new Vector3(0f, 1f, 9f));
            if (outputCamera != null)
            {
                outputCamera.transform.localPosition = baseCameraPosition + effectPositionOffset;
                outputCamera.transform.localRotation = baseCameraRotation * Quaternion.Euler(effectEulerOffset);
                outputCamera.fieldOfView = Mathf.Clamp(baseFieldOfView + fieldOfViewOffset, 1f, 179f);
            }
        }
    }
}

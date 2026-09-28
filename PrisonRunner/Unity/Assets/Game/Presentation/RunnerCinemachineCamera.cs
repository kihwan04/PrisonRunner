using Unity.Cinemachine;
using UnityEngine;

namespace PrisonRunner.Presentation
{
    [RequireComponent(typeof(CinemachineCamera))]
    public sealed class RunnerCinemachineCamera : MonoBehaviour
    {
        public void Configure(Transform player, Camera outputCamera)
        {
            var virtualCamera = GetComponent<CinemachineCamera>();
            virtualCamera.Follow = player;
            virtualCamera.LookAt = player;
            var brain = outputCamera.GetComponent<CinemachineBrain>();
            if (brain == null) brain = outputCamera.gameObject.AddComponent<CinemachineBrain>();
            brain.UpdateMethod = CinemachineBrain.UpdateMethods.LateUpdate;
            brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.Cut, 0f);
            virtualCamera.PreviousStateIsValid = false;
        }
    }
}

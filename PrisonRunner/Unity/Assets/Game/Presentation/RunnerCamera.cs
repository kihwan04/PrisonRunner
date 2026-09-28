using UnityEngine;

namespace PrisonRunner.Presentation
{
    public sealed class RunnerCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(0f, 4.5f, -8f);
        [SerializeField] private float followSharpness = 8f;
        private Transform target;

        public void Configure(Transform player)
        {
            target = player;
            transform.position = target.position + offset;
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
        }
    }
}

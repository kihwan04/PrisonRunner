using UnityEngine;

namespace PrisonRunner.Application
{
    public sealed class DistanceScore : MonoBehaviour
    {
        private float startZ;

        public int Metres => Mathf.FloorToInt(Mathf.Max(0f, transform.position.z - startZ));

        private void Start()
        {
            startZ = transform.position.z;
        }
    }
}

using UnityEngine;

namespace PrisonRunner.Presentation
{
    public static class VisualPrefabUtility
    {
        // Visual assets cannot add collision or physics to the gameplay hierarchy.
        public static GameObject Instantiate(GameObject prefab, Transform parent)
        {
            GameObject instance = Object.Instantiate(prefab, parent, false);
            foreach (Collider collider in instance.GetComponentsInChildren<Collider>(true))
            {
                collider.enabled = false;
                Object.Destroy(collider);
            }

            foreach (Rigidbody body in instance.GetComponentsInChildren<Rigidbody>(true))
            {
                body.detectCollisions = false;
                body.isKinematic = true;
                Object.Destroy(body);
            }

            return instance;
        }
    }
}

using System.Collections.Generic;
using PrisonRunner.Application;
using UnityEngine;

namespace PrisonRunner.Presentation
{
    public sealed class PrisonObstacleVisuals : MonoBehaviour
    {
        [SerializeField] private GameObject metalCrate;
        [SerializeField] private GameObject barricade;
        [SerializeField] private GameObject lowPipe;
        private readonly Dictionary<Transform, GameObject> applied = new Dictionary<Transform, GameObject>();
        private readonly Dictionary<Transform, GameObject> instances = new Dictionary<Transform, GameObject>();

        private void LateUpdate()
        {
            foreach (var obstacle in FindObjectsByType<RunnerObstacle>(FindObjectsSortMode.None))
            {
                Transform visual = obstacle.transform.Find("Visual");
                if (visual == null) continue;
                GameObject prefab = obstacle.name.Contains("Lane Block") ? metalCrate
                    : obstacle.name.Contains("Jump Cube") ? barricade : lowPipe;
                if (prefab == null) continue;
                if (applied.TryGetValue(visual, out var previous) && previous == prefab) continue;
                if (instances.TryGetValue(visual, out var instance) && instance != null)
                {
                    instance.SetActive(false);
                    Destroy(instance);
                }
                // Keep the original Cube/Renderer and its pooled scale contract intact.
                visual.GetComponent<Renderer>().enabled = false;
                instances[visual] = VisualPrefabUtility.Instantiate(prefab, visual);
                applied[visual] = prefab;
            }
        }
    }
}

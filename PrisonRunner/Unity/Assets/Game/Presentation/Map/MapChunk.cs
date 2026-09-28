using System.Collections.Generic;
using UnityEngine;

namespace PrisonRunner.Presentation.Map
{
    public enum MapChunkKind
    {
        PrisonCorridor,
        CellBlock,
        PrisonYard
    }

    public sealed class MapChunk : MonoBehaviour
    {
        public const float Length = 36f;
        public const float LaneSpacing = 3f;
        public const int RowCount = 2;

        [SerializeField] private GameObject visualPrefab;
        [SerializeField] private bool showLaneDebugLines = true;
        private Transform placeholderRoot;
        private Transform laneDebugRoot;
        private GameObject visualInstance;
        private GameObject appliedVisualPrefab;
        private bool visualSettingsDirty;

        private readonly ObstacleSocket[,] obstacleSockets = new ObstacleSocket[RowCount, 3];
        private readonly List<GameObject> activeObstacles = new List<GameObject>();

        public MapChunkKind Kind { get; private set; }
        public Transform EntrySocket { get; private set; }
        public Transform ExitSocket { get; private set; }
        public Transform GameplayRoot { get; private set; }
        public Transform VisualRoot { get; private set; }
        public IReadOnlyList<GameObject> ActiveObstacles => activeObstacles;

        public ObstacleSocket GetObstacleSocket(int row, int lane)
        {
            return obstacleSockets[row, lane + 1];
        }

        public void RegisterObstacle(GameObject obstacle)
        {
            activeObstacles.Add(obstacle);
        }

        public void ClearObstacles()
        {
            activeObstacles.Clear();
        }

        public void Initialize(MapChunkKind kind, Material groundMaterial, Material lineMaterial, Material areaMaterial)
        {
            Kind = kind;
            name = kind.ToString();
            GameplayRoot = CreateChild("Gameplay", transform);
            VisualRoot = CreateChild("VisualRoot", transform);
            placeholderRoot = CreateChild("PlaceholderVisual", VisualRoot);
            laneDebugRoot = CreateChild("LaneDebugLines", VisualRoot);

            EntrySocket = CreateChild("EntrySocket", GameplayRoot);
            ExitSocket = CreateChild("ExitSocket", GameplayRoot);
            ExitSocket.localPosition = Vector3.forward * Length;

            GameObject floorCollider = new GameObject("GroundCollider");
            floorCollider.transform.SetParent(GameplayRoot, false);
            BoxCollider ground = floorCollider.AddComponent<BoxCollider>();
            ground.center = new Vector3(0f, -0.25f, Length * 0.5f);
            ground.size = new Vector3(11f, 0.5f, Length);

            for (int row = 0; row < RowCount; row++)
            {
                for (int lane = -1; lane <= 1; lane++)
                {
                    Transform socket = CreateChild("ObstacleSocket R" + row + " L" + lane, GameplayRoot);
                    socket.localPosition = new Vector3(lane * LaneSpacing, 0f, row == 0 ? 12f : 26f);
                    obstacleSockets[row, lane + 1] = socket.gameObject.AddComponent<ObstacleSocket>();
                    obstacleSockets[row, lane + 1].Configure(row, lane);
                }
            }

            CreateVisualCube("Floor", new Vector3(0f, -0.25f, Length * 0.5f), new Vector3(11f, 0.5f, Length), groundMaterial);
            for (int side = -1; side <= 1; side += 2)
            {
                CreateVisualCube("Lane Divider", new Vector3(side * LaneSpacing * 0.5f, 0.015f, Length * 0.5f), new Vector3(0.06f, 0.02f, Length), lineMaterial, laneDebugRoot);
            }

            BuildPlaceholderArea(areaMaterial);
            SetVisualPrefab(visualPrefab);
            SetLaneDebugLinesVisible(showLaneDebugLines);
        }

        public void SetVisualPrefab(GameObject prefab)
        {
            visualPrefab = prefab;
            if (VisualRoot == null)
            {
                return;
            }

            if (appliedVisualPrefab != prefab)
            {
                if (visualInstance != null)
                {
                    visualInstance.SetActive(false);
                    Destroy(visualInstance);
                }

                visualInstance = prefab != null ? VisualPrefabUtility.Instantiate(prefab, VisualRoot) : null;
                if (visualInstance != null)
                {
                    visualInstance.name = "PrefabVisual";
                }
                appliedVisualPrefab = prefab;
            }

            placeholderRoot.gameObject.SetActive(prefab == null);
        }

        public void SetLaneDebugLinesVisible(bool visible)
        {
            showLaneDebugLines = visible;
            if (laneDebugRoot != null)
            {
                laneDebugRoot.gameObject.SetActive(visible);
            }
        }

        private void OnValidate()
        {
            visualSettingsDirty = true;
        }

        private void LateUpdate()
        {
            if (!visualSettingsDirty)
            {
                return;
            }

            visualSettingsDirty = false;
            SetVisualPrefab(visualPrefab);
            SetLaneDebugLinesVisible(showLaneDebugLines);
        }

        private void BuildPlaceholderArea(Material material)
        {
            for (int side = -1; side <= 1; side += 2)
            {
                float x = side * 5.7f;
                if (Kind == MapChunkKind.PrisonCorridor)
                {
                    CreateVisualCube("Corridor Wall", new Vector3(x, 1.6f, Length * 0.5f), new Vector3(0.35f, 3.2f, Length), material);
                }
                else if (Kind == MapChunkKind.CellBlock)
                {
                    CreateVisualCube("Cell Rail", new Vector3(x, 2.3f, Length * 0.5f), new Vector3(0.18f, 0.2f, Length), material);
                    for (float z = 2f; z < Length; z += 4f)
                    {
                        CreateVisualCube("Cell Bar", new Vector3(x, 1.2f, z), new Vector3(0.18f, 2.4f, 0.18f), material);
                    }
                }
                else
                {
                    CreateVisualCube("Yard Fence Rail", new Vector3(x, 1.1f, Length * 0.5f), new Vector3(0.18f, 0.18f, Length), material);
                    for (float z = 2f; z < Length; z += 6f)
                    {
                        CreateVisualCube("Yard Fence Post", new Vector3(x, 1.1f, z), new Vector3(0.22f, 2.2f, 0.22f), material);
                    }
                }
            }
        }

        private void CreateVisualCube(string objectName, Vector3 position, Vector3 scale, Material material, Transform parent = null)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = objectName;
            cube.transform.SetParent(parent != null ? parent : placeholderRoot, false);
            cube.transform.localPosition = position;
            cube.transform.localScale = scale;
            cube.GetComponent<Renderer>().sharedMaterial = material;
            Collider collider = cube.GetComponent<Collider>();
            collider.enabled = false;
            Destroy(collider);
        }

        private static Transform CreateChild(string objectName, Transform parent)
        {
            GameObject child = new GameObject(objectName);
            child.transform.SetParent(parent, false);
            return child.transform;
        }
    }
}

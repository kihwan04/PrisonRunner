using System.Collections.Generic;
using PrisonRunner.Application;
using UnityEngine;

namespace PrisonRunner.Presentation.Map
{
    public sealed class MapGenerator : MonoBehaviour
    {
        [SerializeField] private float spawnAhead = 180f;
        [SerializeField] private float recycleBehind = 20f;
        [SerializeField, Range(0f, 1f)] private float obstacleChance = 0.8f;

        private readonly Queue<MapChunk> activeChunks = new Queue<MapChunk>();
        private readonly Queue<MapChunk>[] chunkPools =
        {
            new Queue<MapChunk>(),
            new Queue<MapChunk>(),
            new Queue<MapChunk>()
        };
        private readonly Queue<GameObject> obstaclePool = new Queue<GameObject>();

        private Transform player;
        private Vector3 nextEntry;
        private int spawnedCount;
        private Material groundMaterial;
        private Material lineMaterial;
        private Material[] areaMaterials;
        private Material[] obstacleMaterials;

        public void Configure(Transform playerTarget, Material ground, Material line, Material[] areas, Material[] obstacles)
        {
            player = playerTarget;
            groundMaterial = ground;
            lineMaterial = line;
            areaMaterials = areas;
            obstacleMaterials = obstacles;
            nextEntry = Vector3.zero;
            UpdateChunks();
        }

        private void Update()
        {
            if (player != null)
            {
                UpdateChunks();
            }
        }

        private void UpdateChunks()
        {
            while (activeChunks.Count > 0 && activeChunks.Peek().ExitSocket.position.z < player.position.z - recycleBehind)
            {
                RecycleChunk(activeChunks.Dequeue());
            }

            while (nextEntry.z < player.position.z + spawnAhead)
            {
                SpawnChunk();
            }
        }

        private void SpawnChunk()
        {
            MapChunkKind kind = spawnedCount < 3
                ? (MapChunkKind)spawnedCount
                : (MapChunkKind)Random.Range(0, chunkPools.Length);
            Queue<MapChunk> pool = chunkPools[(int)kind];
            MapChunk chunk;
            if (pool.Count > 0)
            {
                chunk = pool.Dequeue();
            }
            else
            {
                GameObject chunkObject = new GameObject(kind.ToString());
                chunkObject.transform.SetParent(transform, false);
                chunk = chunkObject.AddComponent<MapChunk>();
                chunk.Initialize(kind, groundMaterial, lineMaterial, areaMaterials[(int)kind]);
            }

            chunk.transform.position += nextEntry - chunk.EntrySocket.position;
            chunk.gameObject.SetActive(true);
            if (spawnedCount > 0)
            {
                PlaceObstacles(chunk);
            }

            activeChunks.Enqueue(chunk);
            nextEntry = chunk.ExitSocket.position;
            spawnedCount++;
        }

        private void PlaceObstacles(MapChunk chunk)
        {
            for (int row = 0; row < MapChunk.RowCount; row++)
            {
                if (Random.value > obstacleChance)
                {
                    continue;
                }

                // One occupied lane per row leaves two lanes open, even for a full-height block.
                int lane = Random.Range(-1, 2);
                int type = Random.Range(0, 3);
                ObstacleSocket socket = chunk.GetObstacleSocket(row, lane);
                GameObject obstacle = RentObstacle();
                obstacle.transform.SetParent(socket.transform, false);
                obstacle.transform.localPosition = Vector3.zero;
                ConfigureObstacle(obstacle, type);
                obstacle.SetActive(true);
                chunk.RegisterObstacle(obstacle);
            }
        }

        private GameObject RentObstacle()
        {
            if (obstaclePool.Count > 0)
            {
                return obstaclePool.Dequeue();
            }

            GameObject obstacle = new GameObject("Obstacle");
            obstacle.AddComponent<BoxCollider>();
            obstacle.AddComponent<RunnerObstacle>();

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "Visual";
            visual.transform.SetParent(obstacle.transform, false);
            Collider visualCollider = visual.GetComponent<Collider>();
            visualCollider.enabled = false;
            Destroy(visualCollider);
            return obstacle;
        }

        private void ConfigureObstacle(GameObject obstacle, int type)
        {
            Vector3 center;
            Vector3 size;
            if (type == 0)
            {
                obstacle.name = "Obstacle Lane Block";
                center = new Vector3(0f, 1.1f, 0f);
                size = new Vector3(2f, 2.2f, 1.5f);
            }
            else if (type == 1)
            {
                obstacle.name = "Obstacle Jump Cube";
                center = new Vector3(0f, 0.4f, 0f);
                size = new Vector3(2f, 0.8f, 1.5f);
            }
            else
            {
                obstacle.name = "Obstacle Crouch Beam";
                center = new Vector3(0f, 1.55f, 0f);
                size = new Vector3(2f, 0.6f, 1.5f);
            }

            BoxCollider collider = obstacle.GetComponent<BoxCollider>();
            collider.center = center;
            collider.size = size;
            Transform visual = obstacle.transform.GetChild(0);
            visual.localPosition = center;
            visual.localScale = size;
            visual.GetComponent<Renderer>().sharedMaterial = obstacleMaterials[type];
        }

        private void RecycleChunk(MapChunk chunk)
        {
            foreach (GameObject obstacle in chunk.ActiveObstacles)
            {
                obstacle.SetActive(false);
                obstacle.transform.SetParent(transform, false);
                obstaclePool.Enqueue(obstacle);
            }

            chunk.ClearObstacles();
            chunk.gameObject.SetActive(false);
            chunkPools[(int)chunk.Kind].Enqueue(chunk);
        }
    }
}

using PrisonRunner.Application;
using PrisonRunner.Infrastructure;
using PrisonRunner.Presentation.Map;
using UnityEngine;

namespace PrisonRunner.Presentation
{
    public sealed class RunnerSceneBootstrap : MonoBehaviour
    {
        private Material groundMaterial;
        private Material lineMaterial;
        private Material playerMaterial;
        private Material blockMaterial;
        private Material jumpMaterial;
        private Material crouchMaterial;

        private void Awake()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            groundMaterial = MakeMaterial(shader, new Color(0.16f, 0.20f, 0.25f));
            lineMaterial = MakeMaterial(shader, new Color(0.42f, 0.63f, 0.68f));
            playerMaterial = MakeMaterial(shader, new Color(1f, 0.38f, 0.08f));
            blockMaterial = MakeMaterial(shader, new Color(0.75f, 0.16f, 0.16f));
            jumpMaterial = MakeMaterial(shader, new Color(0.95f, 0.65f, 0.12f));
            crouchMaterial = MakeMaterial(shader, new Color(0.95f, 0.65f, 0.12f));

            Transform player = BuildPlayer();
            Material[] areas =
            {
                MakeMaterial(shader, new Color(0.12f, 0.22f, 0.30f)),
                MakeMaterial(shader, new Color(0.27f, 0.32f, 0.38f)),
                MakeMaterial(shader, new Color(0.21f, 0.38f, 0.43f))
            };
            Material[] obstacles = { blockMaterial, jumpMaterial, crouchMaterial };
            GameObject map = new GameObject("Endless Map");
            map.AddComponent<MapGenerator>().Configure(player, groundMaterial, lineMaterial, areas, obstacles);
        }

        private static Material MakeMaterial(Shader shader, Color color)
        {
            Material material = new Material(shader);
            material.color = color;
            return material;
        }

        private Transform BuildPlayer()
        {
            GameObject player = new GameObject("Capsule Player");
            player.transform.position = new Vector3(0f, 0f, 2f);
            CharacterController collider = player.AddComponent<CharacterController>();
            collider.height = 2f;
            collider.radius = 0.35f;
            collider.center = Vector3.up;
            collider.stepOffset = 0.2f;

            GameObject visualRoot = new GameObject("Visual");
            visualRoot.transform.SetParent(player.transform);
            visualRoot.transform.localPosition = Vector3.up;
            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.name = "Orange Capsule Mesh";
            capsule.transform.SetParent(visualRoot.transform);
            capsule.transform.localPosition = Vector3.zero;
            capsule.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
            capsule.GetComponent<Renderer>().sharedMaterial = playerMaterial;
            Collider meshCollider = capsule.GetComponent<Collider>();
            meshCollider.enabled = false;
            Destroy(meshCollider);

            PlayerMovement movement = player.AddComponent<PlayerMovement>();
            movement.Configure(visualRoot.transform);
            DistanceScore score = player.AddComponent<DistanceScore>();
            GameObject inputObject = new GameObject("Keyboard Input");
            KeyboardInputProvider keyboard = inputObject.AddComponent<KeyboardInputProvider>();
            PlayerActionController actions = player.AddComponent<PlayerActionController>();
            actions.Configure(keyboard, movement);

            Camera camera = Camera.main;
            if (camera != null)
            {
                camera.farClipPlane = 1000f;
                camera.gameObject.AddComponent<RunnerCamera>().Configure(player.transform);
            }

            GameObject hud = new GameObject("Runner HUD");
            hud.AddComponent<RunnerHud>().Configure(score);
            return player.transform;
        }
    }
}

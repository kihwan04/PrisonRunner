using Unity.Cinemachine;
using Unity.Cinemachine.TargetTracking;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using PrisonRunner.Presentation;

namespace PrisonRunner.Editor
{
    public static class CellBlockSliceBuilder
    {
        private const string Art = "Assets/Art/Environment/Blockouts/CellBlock";
        private const string Materials = "Assets/Art/Materials/CellBlock";
        private static int meshIndex;
        private static Material navy, blue, steel, orange, red, warm, cool;

        [MenuItem("PrisonRunner/Build Cell Block Vertical Slice")]
        public static void Build()
        {
            EnsureFolder(Art + "/Meshes");
            EnsureFolder(Materials);
            EnsureFolder("Assets/Art/Obstacles/CellBlock");
            EnsureFolder("Assets/Settings/CameraPresets");
            meshIndex = 0;
            navy = Material("DarkNavy", new Color(0.10f, 0.15f, 0.23f));
            blue = Material("BlueGray", new Color(0.25f, 0.32f, 0.40f));
            steel = Material("DarkSteel", new Color(0.16f, 0.20f, 0.25f), 0.65f);
            orange = Material("PlayerOrange", new Color(1f, 0.38f, 0.08f));
            red = Material("WarningRed", new Color(0.75f, 0.12f, 0.13f));
            warm = Material("WarmYellow", new Color(1f, 0.69f, 0.29f), 0f, 1.1f);
            cool = Material("CoolBlue", new Color(0.18f, 0.34f, 0.48f), 0f, 0.15f);

            GameObject module = new GameObject("CellBlockModule_6m");
            BuildModule(module.transform);
            GameObject modulePrefab = PrefabUtility.SaveAsPrefabAsset(module, Art + "/CellBlockModule_6m.prefab");
            UnityEngine.Object.DestroyImmediate(module);

            GameObject chunk = new GameObject("CellBlockVisual_36m");
            Transform architecture = Group(chunk.transform, "Architecture");
            for (int i = 0; i < 6; i++)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(modulePrefab, architecture);
                instance.name = "Cell Bay " + (i + 1).ToString("00");
                instance.transform.localPosition = Vector3.forward * (i * 6f);
            }
            BuildGate(chunk.transform);
            Transform lighting = Group(chunk.transform, "Lighting");
            for (int i = 0; i < 3; i++)
            {
                float z = 6f + 12f * i;
                Box(lighting, "Ceiling Fixture", new Vector3(0f, 5.35f, z), new Vector3(2f, 0.18f, 0.5f), steel);
                Box(lighting, "Warm Diffuser", new Vector3(0f, 5.23f, z), new Vector3(1.65f, 0.06f, 0.32f), warm);
                PointLight(lighting, "Warm Ceiling Light", new Vector3(0f, 4.6f, z), new Color(1f, 0.77f, 0.43f), 22f, 14f);
            }
            PointLight(lighting, "Gate Red Accent", new Vector3(4.9f, 3.5f, 33.8f), new Color(1f, 0.18f, 0.12f), 0.55f, 4.5f);
            GameObject chunkPrefab = PrefabUtility.SaveAsPrefabAsset(chunk, Art + "/CellBlockVisual_36m.prefab");
            UnityEngine.Object.DestroyImmediate(chunk);

            GameObject crate = BuildCrate();
            GameObject barrier = BuildBarricade();
            GameObject pipe = BuildPipe();
            GameObject camera = BuildCamera();
            BuildScene(chunkPrefab, crate, barrier, pipe, camera);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("CELLBLOCK_BUILD_PASS: 36m visual chunk, six 6m modules, seven materials, obstacle visuals and Cinemachine preset.");
            if (UnityEngine.Application.isBatchMode) EditorApplication.Exit(0);
        }

        private static void BuildModule(Transform parent)
        {
            Transform floor = Group(parent, "FloorAndCeiling");
            Box(floor, "Aisle Floor", new Vector3(0f, -0.12f, 3f), new Vector3(11f, 0.24f, 6f), blue);
            Box(floor, "Floor Joint", new Vector3(0f, 0.005f, 0.02f), new Vector3(11f, 0.01f, 0.035f), steel);
            Box(floor, "Ceiling", new Vector3(0f, 5.85f, 3f), new Vector3(19.4f, 0.25f, 6f), navy);
            Box(floor, "Overhead Cross Beam", new Vector3(0f, 5.5f, 0.2f), new Vector3(19.4f, 0.4f, 0.4f), steel);
            for (int side = -1; side <= 1; side += 2)
            {
                Transform cell = Group(parent, side < 0 ? "LeftCell" : "RightCell");
                Box(cell, "Cell Floor", new Vector3(side * 7.45f, -0.12f, 3f), new Vector3(3.9f, 0.24f, 6f), navy);
                Box(cell, "Rear Wall", new Vector3(side * 9.5f, 2.65f, 3f), new Vector3(0.3f, 5.3f, 6f), navy);
                Box(cell, "Dividing Wall", new Vector3(side * 7.6f, 2.3f, 0.15f), new Vector3(3.8f, 4.6f, 0.3f), blue);
                Box(cell, "Front Pillar", new Vector3(side * 5.95f, 2.8f, 0.2f), new Vector3(0.4f, 5.6f, 0.4f), blue);
                Box(cell, "Cell Lintel", new Vector3(side * 5.95f, 4.1f, 3f), new Vector3(0.3f, 0.8f, 6f), blue);
                Box(cell, "Upper Background Panel", new Vector3(side * 9.27f, 4.75f, 3f), new Vector3(0.08f, 0.7f, 4.4f), cool);
                Box(cell, "Upper Service Ledge", new Vector3(side * 7.8f, 4.4f, 3f), new Vector3(3.5f, 0.18f, 6f), steel);
                Transform bars = Group(cell, "BarsAndDoor");
                for (int i = 0; i < 11; i++)
                {
                    float z = 0.55f + i * 0.49f;
                    Box(bars, "Vertical Bar", new Vector3(side * 5.9f, 1.85f, z), new Vector3(0.09f, 3.7f, 0.09f), steel);
                }
                Box(bars, "Lower Rail", new Vector3(side * 5.9f, 0.32f, 3f), new Vector3(0.11f, 0.12f, 5.45f), steel);
                Box(bars, "Upper Rail", new Vector3(side * 5.9f, 3.55f, 3f), new Vector3(0.11f, 0.12f, 5.45f), steel);
                Transform door = Group(bars, "Cell Door");
                Box(door, "Door Top", new Vector3(side * 5.83f, 3.2f, 3f), new Vector3(0.18f, 0.18f, 1.6f), blue);
                Box(door, "Door Bottom", new Vector3(side * 5.83f, 0.12f, 3f), new Vector3(0.18f, 0.18f, 1.6f), blue);
                for (int edge = -1; edge <= 1; edge += 2)
                    Box(door, "Door Upright", new Vector3(side * 5.83f, 1.65f, 3f + edge * 0.8f), new Vector3(0.18f, 3.1f, 0.16f), blue);
                Box(door, "Lock Plate", new Vector3(side * 5.7f, 1.45f, 3.6f), new Vector3(0.12f, 0.32f, 0.23f), steel);

                Transform dressing = Group(cell, "MinimalDressing");
                Box(dressing, "Bed Frame", new Vector3(side * 8.2f, 0.36f, 4.1f), new Vector3(1.4f, 0.15f, 2f), steel);
                Box(dressing, "Bed Mattress", new Vector3(side * 8.2f, 0.5f, 4.1f), new Vector3(1.35f, 0.14f, 1.95f), blue);
                Box(dressing, "Bench", new Vector3(side * 7.8f, 0.45f, 1.35f), new Vector3(1.8f, 0.16f, 0.5f), steel);
                Box(dressing, "Cell Lamp Housing", new Vector3(side * 8.4f, 3.85f, 2.8f), new Vector3(0.9f, 0.18f, 0.3f), steel);
                Box(dressing, "Cell Lamp Glow", new Vector3(side * 8.4f, 3.72f, 2.8f), new Vector3(0.7f, 0.05f, 0.2f), warm);
                Cylinder(cell, "Service Pipe", new Vector3(side * 5.95f, 4.95f, 3f), 0.13f, 6f, steel, new Vector3(90f, 0f, 0f));
                for (int clamp = 0; clamp < 2; clamp++)
                    Box(cell, "Pipe Clamp", new Vector3(side * 5.95f, 4.95f, 1.2f + clamp * 3.6f), new Vector3(0.34f, 0.34f, 0.12f), blue);
                Transform cctv = Group(cell, "CCTV Mount");
                Box(cctv, "Bracket", new Vector3(side * 5.75f, 4.3f, 4.8f), new Vector3(0.35f, 0.12f, 0.12f), steel);
                Box(cctv, "Camera Body", new Vector3(side * 5.57f, 4.2f, 4.8f), new Vector3(0.25f, 0.22f, 0.45f), blue);
                Box(cctv, "Camera Lens", new Vector3(side * 5.57f, 4.17f, 4.55f), new Vector3(0.14f, 0.11f, 0.05f), cool);
            }
        }

        private static void BuildGate(Transform root)
        {
            Transform gate = Group(root, "Open Entry Gate");
            for (int side = -1; side <= 1; side += 2)
            {
                Box(gate, "Gate Post", new Vector3(side * 5.75f, 2.4f, 35.3f), new Vector3(0.5f, 4.8f, 0.7f), steel);
                Box(gate, "Warning Strip", new Vector3(side * 5.47f, 1.7f, 35.25f), new Vector3(0.04f, 0.75f, 0.25f), red);
            }
            Box(gate, "Gate Header", new Vector3(0f, 5f, 35.3f), new Vector3(12f, 0.65f, 0.7f), blue);
            Box(gate, "Gate Sign Panel", new Vector3(0f, 5f, 34.91f), new Vector3(4f, 0.48f, 0.05f), navy);
            // Geometry-only B04 marker uses the same simple materials as the blockout.
            Transform marker = Group(gate, "B04 Gate Marker");
            SignStroke(marker, "B Left", -0.8f, 5f, 0.04f, 0.3f);
            SignStroke(marker, "B Right", -0.58f, 5f, 0.04f, 0.3f);
            foreach (float y in new[] { 4.86f, 5f, 5.14f })
                SignStroke(marker, "B Rail", -0.69f, y, 0.22f, 0.04f);
            SignStroke(marker, "0 Left", -0.16f, 5f, 0.04f, 0.3f);
            SignStroke(marker, "0 Right", 0.08f, 5f, 0.04f, 0.3f);
            foreach (float y in new[] { 4.86f, 5.14f })
                SignStroke(marker, "0 Rail", -0.04f, y, 0.24f, 0.04f);
            SignStroke(marker, "4 Left", 0.36f, 5.08f, 0.04f, 0.16f);
            SignStroke(marker, "4 Right", 0.6f, 5f, 0.04f, 0.3f);
            SignStroke(marker, "4 Rail", 0.48f, 5f, 0.24f, 0.04f);
        }

        private static void SignStroke(Transform parent, string name, float x, float y, float width, float height)
        {
            Box(parent, name, new Vector3(x, y, 34.865f), new Vector3(width, height, 0.015f), warm);
        }

        private static GameObject BuildCrate()
        {
            var root = new GameObject("MetalCrateVisual");
            Box(root.transform, "Crate Body", Vector3.zero, Vector3.one * 0.94f, blue);
            for (int side = -1; side <= 1; side += 2)
            {
                Box(root.transform, "Front Band", new Vector3(0f, side * 0.34f, -0.485f), new Vector3(0.98f, 0.08f, 0.025f), steel);
                Box(root.transform, "Side Band", new Vector3(side * 0.48f, 0f, 0f), new Vector3(0.025f, 0.98f, 0.08f), steel);
            }
            Box(root.transform, "Red Warning Plate", new Vector3(0f, 0.12f, -0.49f), new Vector3(0.3f, 0.12f, 0.025f), red);
            return SaveVisual(root, "MetalCrateVisual");
        }

        private static GameObject BuildBarricade()
        {
            var root = new GameObject("BarricadeVisual");
            Box(root.transform, "Low Barrier", new Vector3(0f, 0.1f, 0f), new Vector3(0.98f, 0.78f, 0.48f), blue);
            Box(root.transform, "Warning Face", new Vector3(0f, 0.06f, -0.25f), new Vector3(0.9f, 0.23f, 0.03f), red);
            for (int side = -1; side <= 1; side += 2)
                Box(root.transform, "Barrier Foot", new Vector3(side * 0.33f, -0.38f, 0f), new Vector3(0.12f, 0.2f, 0.9f), steel);
            return SaveVisual(root, "BarricadeVisual");
        }

        private static GameObject BuildPipe()
        {
            var root = new GameObject("LowPipeVisual");
            Cylinder(root.transform, "Horizontal Pipe", Vector3.zero, 0.42f, 0.96f, steel, new Vector3(0f, 0f, 90f));
            for (int side = -1; side <= 1; side += 2)
                Cylinder(root.transform, "Red Coupling", new Vector3(side * 0.35f, 0f, 0f), 0.46f, 0.09f, red, new Vector3(0f, 0f, 90f));
            return SaveVisual(root, "LowPipeVisual");
        }

        private static GameObject SaveVisual(GameObject root, string name)
        {
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, "Assets/Art/Obstacles/CellBlock/" + name + ".prefab");
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject BuildCamera()
        {
            var root = new GameObject("Cell Block Runner Camera");
            var camera = root.AddComponent<CinemachineCamera>();
            camera.Lens.FieldOfView = 64f;
            var follow = root.AddComponent<CinemachineFollow>();
            follow.FollowOffset = new Vector3(0f, 3.15f, -7.8f);
            follow.TrackerSettings.BindingMode = BindingMode.WorldSpace;
            follow.TrackerSettings.PositionDamping = new Vector3(0.9f, 0.45f, 0.08f);
            var aim = root.AddComponent<CinemachineRotationComposer>();
            aim.TargetOffset = new Vector3(0f, 1.15f, 7f);
            aim.Damping = new Vector2(0.6f, 0.35f);
            root.AddComponent<RunnerCameraEffects>();
            root.AddComponent<RunnerCinemachineCamera>();
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, "Assets/Settings/CameraPresets/CellBlockRunnerCamera.prefab");
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static void BuildScene(GameObject chunk, GameObject crate, GameObject barricade, GameObject pipe, GameObject camera)
        {
            var scene = EditorSceneManager.OpenScene("Assets/Game/Scenes/RunnerMVP.unity");
            // The camera begins behind Entry Z=0. A single visual bay covers the starting view.
            var entrance = (GameObject)PrefabUtility.InstantiatePrefab(
                AssetDatabase.LoadAssetAtPath<GameObject>(Art + "/CellBlockModule_6m.prefab"));
            entrance.name = "Entry Visual Buffer";
            entrance.transform.position = Vector3.back * 6f;
            var bootstrap = UnityEngine.Object.FindFirstObjectByType<RunnerSceneBootstrap>();
            var settings = new SerializedObject(bootstrap);
            // Showcase one visual chunk on all existing kinds without changing map selection or gameplay.
            foreach (string field in new[] { "corridorVisualPrefab", "cellBlockVisualPrefab", "prisonYardVisualPrefab" })
                settings.FindProperty(field).objectReferenceValue = chunk;
            settings.FindProperty("runnerCameraPrefab").objectReferenceValue = camera;
            settings.FindProperty("showLaneDebugLines").boolValue = false;
            settings.ApplyModifiedPropertiesWithoutUndo();
            var styles = bootstrap.gameObject.AddComponent<PrisonObstacleVisuals>();
            var obstacleSettings = new SerializedObject(styles);
            obstacleSettings.FindProperty("metalCrate").objectReferenceValue = crate;
            obstacleSettings.FindProperty("barricade").objectReferenceValue = barricade;
            obstacleSettings.FindProperty("lowPipe").objectReferenceValue = pipe;
            obstacleSettings.ApplyModifiedPropertiesWithoutUndo();
            var sun = UnityEngine.Object.FindFirstObjectByType<Light>();
            sun.color = new Color(0.56f, 0.69f, 0.95f);
            sun.intensity = 0.7f;
            sun.transform.rotation = Quaternion.Euler(45f, -25f, 0f);
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.40f, 0.50f, 0.64f);
            RenderSettings.ambientEquatorColor = new Color(0.32f, 0.40f, 0.51f);
            RenderSettings.ambientGroundColor = new Color(0.18f, 0.24f, 0.33f);
            RenderSettings.reflectionIntensity = 0.5f;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.075f, 0.11f, 0.17f);
            RenderSettings.fogStartDistance = 32f;
            RenderSettings.fogEndDistance = 95f;
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = RenderSettings.fogColor;
            EditorSceneManager.SaveScene(scene, "Assets/Game/Scenes/CellBlockVerticalSlice.unity");
        }

        private static void Box(Transform parent, string name, Vector3 position, Vector3 size, Material material)
        {
            var mesh = ShapeGenerator.GenerateCube(PivotLocation.Center, size);
            SaveMesh(mesh, parent, name, position, Vector3.zero, material);
        }

        private static void Cylinder(Transform parent, string name, Vector3 position, float radius, float height, Material material, Vector3 rotation)
        {
            var mesh = ShapeGenerator.GenerateCylinder(PivotLocation.Center, 10, radius, height, 0, 1);
            SaveMesh(mesh, parent, name, position, rotation, material);
        }

        private static void SaveMesh(ProBuilderMesh mesh, Transform parent, string name, Vector3 position, Vector3 rotation, Material material)
        {
            mesh.name = name;
            mesh.transform.SetParent(parent, false);
            mesh.transform.localPosition = position;
            mesh.transform.localEulerAngles = rotation;
            mesh.ToMesh();
            mesh.Refresh();
            var filter = mesh.GetComponent<MeshFilter>();
            string path = Art + "/Meshes/BlockMesh_" + (++meshIndex).ToString("000") + ".asset";
            var stored = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (stored == null) AssetDatabase.CreateAsset(filter.sharedMesh, path);
            else
            {
                EditorUtility.CopySerialized(filter.sharedMesh, stored);
                EditorUtility.SetDirty(stored);
                filter.sharedMesh = stored;
            }
            var meshSettings = new SerializedObject(mesh);
            meshSettings.FindProperty("m_Mesh").objectReferenceValue = filter.sharedMesh;
            meshSettings.FindProperty("assetGuid").stringValue = AssetDatabase.AssetPathToGUID(path);
            meshSettings.ApplyModifiedPropertiesWithoutUndo();
            mesh.preserveMeshAssetOnDestroy = true;
            mesh.GetComponent<MeshRenderer>().sharedMaterial = material;
            foreach (Collider collider in mesh.GetComponents<Collider>()) UnityEngine.Object.DestroyImmediate(collider);
        }

        private static Transform Group(Transform parent, string name)
        {
            var group = new GameObject(name).transform;
            group.SetParent(parent, false);
            return group;
        }

        private static void PointLight(Transform parent, string name, Vector3 position, Color color, float intensity, float range)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;
            var light = root.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
        }

        private static Material Material(string name, Color color, float metallic = 0f, float emission = 0f)
        {
            string path = Materials + "/" + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, path);
            }
            material.color = color;
            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Smoothness", 0.3f);
            if (emission > 0f)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * emission);
            }
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}

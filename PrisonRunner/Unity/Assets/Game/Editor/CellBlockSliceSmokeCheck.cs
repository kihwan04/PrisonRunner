using System;
using System.IO;
using System.Linq;
using PrisonRunner.Application;
using PrisonRunner.Presentation;
using PrisonRunner.Presentation.Map;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace PrisonRunner.Editor
{
    public static class CellBlockSliceSmokeCheck
    {
        public static void Validate(MapChunk[] chunks)
        {
            foreach (var chunk in chunks)
            {
                Transform prefab = chunk.VisualRoot.Find("PrefabVisual");
                Require(prefab != null, "Cell block visual is missing.");
                Require(prefab.GetComponentsInChildren<ProBuilderMesh>().Length > 300,
                    "Editable ProBuilder cell geometry is missing.");
                Require(prefab.GetComponentsInChildren<ProBuilderMesh>().All(m =>
                    m.GetComponent<MeshFilter>().sharedMesh != null && m.GetComponent<MeshFilter>().sharedMesh.vertexCount > 0),
                    "Saved ProBuilder mesh reference is missing.");
                Require(prefab.GetComponentsInChildren<Collider>(true).Length == 0,
                    "Cell block introduced visual colliders.");
                Require(prefab.GetComponentsInChildren<Transform>().Count(t => t.name.StartsWith("Cell Bay")) == 6,
                    "Chunk must contain six modular cell bays.");
                foreach (var renderer in prefab.GetComponentsInChildren<MeshRenderer>())
                {
                    Bounds bounds = renderer.bounds;
                    // Floor below the feet and architecture above a full jump are allowed.
                    if (bounds.max.y <= 0.03f || bounds.min.y >= 4.4f) continue;
                    Require(bounds.min.x >= 5.3f || bounds.max.x <= -5.3f,
                        "Decoration intrudes into the playable aisle: " + renderer.name);
                }
                Require(prefab.GetComponentsInChildren<Light>().Length == 4,
                    "Unexpected point light count.");
            }

            foreach (var obstacle in UnityEngine.Object.FindObjectsByType<RunnerObstacle>(FindObjectsSortMode.None))
            {
                BoxCollider collider = obstacle.GetComponent<BoxCollider>();
                Vector3 expectedSize = obstacle.name.Contains("Lane Block") ? new Vector3(2f, 2.2f, 1.5f)
                    : obstacle.name.Contains("Jump Cube") ? new Vector3(2f, 0.8f, 1.5f) : new Vector3(2f, 0.6f, 1.5f);
                Require(collider.size == expectedSize, "Obstacle collision size changed.");
                Transform visual = obstacle.transform.Find("Visual");
                Require(visual.childCount == 1 && !visual.GetComponent<Renderer>().enabled,
                    "Obstacle skin was not applied.");
                Require(!visual.GetComponentsInChildren<Collider>(true).Any(c => c.enabled),
                    "Obstacle skin introduced collision.");
                foreach (var renderer in visual.GetComponentsInChildren<MeshRenderer>())
                {
                    if (renderer.transform == visual) continue;
                    Bounds bounds = collider.bounds;
                    bounds.Expand(0.04f);
                    Require(bounds.Contains(renderer.bounds.min) && bounds.Contains(renderer.bounds.max),
                        "Obstacle skin exceeds its gameplay volume.");
                }
            }
            Require(RenderSettings.fog && RenderSettings.fogMode == FogMode.Linear
                && RenderSettings.fogStartDistance == 32f && RenderSettings.fogEndDistance == 95f,
                "Distance fog is missing.");
            Require(Camera.main.clearFlags == CameraClearFlags.SolidColor
                && Camera.main.backgroundColor == RenderSettings.fogColor,
                "Far corridor must blend into the fog without a bright skybox opening.");
            Debug.Log("CELLBLOCK_SLICE_SMOKE_PASS: six bays, clear aisle, editable meshes, unchanged obstacle bounds and fog.");
        }

        public static void ValidateCamera(PlayerMovement player, Camera output)
        {
            var camera = UnityEngine.Object.FindFirstObjectByType<CinemachineCamera>();
            Require(camera != null && camera.Follow == player.transform && camera.LookAt == player.transform,
                "Cinemachine must follow the existing PlayerRoot.");
            var follow = camera.GetComponent<CinemachineFollow>();
            Require(follow != null && follow.TrackerSettings.PositionDamping.x > follow.TrackerSettings.PositionDamping.z
                && follow.TrackerSettings.PositionDamping.y > 0f, "Lane and jump damping is missing.");
            Require(Mathf.Abs(output.fieldOfView - 64f) < 0.01f, "Cinemachine preset FOV is not applied.");
            var effects = camera.GetComponent<RunnerCameraEffects>();
            Require(effects != null, "Camera effect extension is missing.");
            camera.PreviousStateIsValid = false;
            camera.InternalUpdateCameraState(Vector3.up, -1f);
            CameraState neutral = camera.State;
            effects.SetPresentationEffects(Vector3.right * 0.1f, Vector3.up * 2f, 5f);
            camera.PreviousStateIsValid = false;
            camera.InternalUpdateCameraState(Vector3.up, -1f);
            Require(Mathf.Abs(camera.State.Lens.FieldOfView - neutral.Lens.FieldOfView - 5f) < 0.01f
                && Vector3.Distance(camera.State.PositionCorrection, neutral.PositionCorrection) > 0.09f,
                "Cinemachine effects extension failed.");
            effects.ResetPresentationEffects();
            camera.PreviousStateIsValid = false;
            camera.InternalUpdateCameraState(Vector3.up, -1f);
            Require(Mathf.Abs(camera.State.Lens.FieldOfView - neutral.Lens.FieldOfView) < 0.01f
                && Vector3.Distance(camera.State.PositionCorrection, neutral.PositionCorrection) < 0.001f,
                "Cinemachine effects did not reset to neutral.");
        }

        public static void CapturePreview()
        {
            Camera camera = Camera.main;
            var target = new RenderTexture(1280, 720, GraphicsFormat.R8G8B8A8_SRGB, CoreUtils.GetDefaultDepthOnlyFormat());
            target.Create();
            var request = new RenderPipeline.StandardRequest
            {
                destination = target, mipLevel = 0, slice = 0, face = CubemapFace.Unknown
            };
            RenderPipeline.SubmitRenderRequest(camera, request);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = target;
            var image = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
            image.Apply();
            string path = Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath, "../../docs/previews/CellBlockRunner.png"));
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, image.EncodeToPNG());
            RenderTexture.active = previous;
            UnityEngine.Object.Destroy(image);
            target.Release();
            UnityEngine.Object.Destroy(target);
            Debug.Log("CELLBLOCK_PREVIEW_SAVED: " + path);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
        }
    }
}

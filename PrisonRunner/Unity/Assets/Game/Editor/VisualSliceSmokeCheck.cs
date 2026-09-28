using System;
using System.Linq;
using PrisonRunner.Application;
using PrisonRunner.Core;
using PrisonRunner.Presentation;
using PrisonRunner.Presentation.Map;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PrisonRunner.Editor
{
    public static class VisualSliceSmokeCheck
    {
        public static void Validate(PlayerMovement player, MapChunk[] chunks)
        {
            Transform playerVisual = player.transform.Find("PlayerVisual");
            CharacterController controller = player.GetComponent<CharacterController>();
            Require(player.name == "PlayerRoot" && playerVisual != null, "Player hierarchy is not separated.");
            Require(controller.height == 2f && controller.radius == 0.35f && controller.stepOffset == 0.2f,
                "Player collision settings changed.");
            Require(!playerVisual.GetComponentsInChildren<Collider>(true).Any(c => c.enabled), "Player visual has collision.");

            player.Tick(new RunnerInput { CrouchHeld = true }, 0.01f);
            Require(controller.height == 1f && playerVisual.localScale.y == 0.5f, "Placeholder crouch changed.");
            player.Tick(default, 0.01f);
            Require(controller.height == 2f && playerVisual.localScale == Vector3.one, "Placeholder stand changed.");

            foreach (MapChunk chunk in chunks)
            {
                Require(!chunk.VisualRoot.GetComponentsInChildren<Collider>(true).Any(c => c.enabled),
                    "Environment visual has collision.");
                int collisionCount = chunk.GameplayRoot.GetComponentsInChildren<Collider>().Length;
                chunk.SetLaneDebugLinesVisible(false);
                Require(!chunk.VisualRoot.Find("LaneDebugLines").gameObject.activeSelf, "Lane debug off failed.");
                chunk.SetLaneDebugLinesVisible(true);
                Require(chunk.VisualRoot.Find("LaneDebugLines").gameObject.activeSelf, "Lane debug on failed.");
                Require(collisionCount == chunk.GameplayRoot.GetComponentsInChildren<Collider>().Length,
                    "Debug toggle changed gameplay collision.");
            }

            MapChunk sample = chunks[0];
            BoxCollider ground = sample.GameplayRoot.Find("GroundCollider").GetComponent<BoxCollider>();
            Vector3 groundSize = ground.size;
            Transform entry = sample.EntrySocket;
            Transform exit = sample.ExitSocket;
            ObstacleSocket socket = sample.GetObstacleSocket(0, 0);
            GameObject source = GameObject.CreatePrimitive(PrimitiveType.Cube);
            source.SetActive(false);
            source.AddComponent<Rigidbody>();
            sample.SetVisualPrefab(source);
            Transform prefabVisual = sample.VisualRoot.Find("PrefabVisual");
            Require(prefabVisual != null && !sample.VisualRoot.Find("PlaceholderVisual").gameObject.activeSelf,
                "Visual prefab replacement failed.");
            Require(!prefabVisual.GetComponentsInChildren<Collider>(true).Any(c => c.enabled),
                "Replacement prefab introduced collision.");
            Require(prefabVisual.GetComponentsInChildren<Rigidbody>(true).All(b => b.isKinematic && !b.detectCollisions),
                "Replacement prefab introduced physics.");
            sample.SetVisualPrefab(null);
            UnityEngine.Object.Destroy(source);
            Require(sample.VisualRoot.Find("PlaceholderVisual").gameObject.activeSelf, "Placeholder fallback failed.");
            Require(ground.enabled && ground.size == groundSize && sample.EntrySocket == entry && sample.ExitSocket == exit
                && sample.GetObstacleSocket(0, 0) == socket, "Visual replacement changed gameplay structure.");

            MapGenerator generator = UnityEngine.Object.FindFirstObjectByType<MapGenerator>();
            generator.ConfigureVisuals(null, null, null, false);
            Require(generator.GetComponentsInChildren<MapChunk>(true).All(c => !c.VisualRoot.Find("LaneDebugLines").gameObject.activeSelf),
                "Generator debug settings missed pooled chunks.");
            generator.ConfigureVisuals(null, null, null, true);

            Camera camera = Camera.main;
            RunnerCamera rig = camera.GetComponentInParent<RunnerCamera>();
            Require(rig != null && rig.transform != camera.transform, "Camera effects are not separated from follow.");
            Vector3 neutralPosition = camera.transform.localPosition;
            Quaternion neutralRotation = camera.transform.localRotation;
            float neutralFov = camera.fieldOfView;
            rig.SetPresentationEffects(Vector3.right * 0.1f, Vector3.up * 2f, 5f);
            rig.SendMessage("LateUpdate");
            Require(Vector3.Distance(camera.transform.localPosition, neutralPosition + Vector3.right * 0.1f) < 0.0001f
                && Mathf.Abs(camera.fieldOfView - neutralFov - 5f) < 0.0001f, "Camera effect hook failed.");
            rig.ResetPresentationEffects();
            rig.SendMessage("LateUpdate");
            Require(Vector3.Distance(camera.transform.localPosition, neutralPosition) < 0.0001f
                && Quaternion.Angle(camera.transform.localRotation, neutralRotation) < 0.001f
                && Mathf.Abs(camera.fieldOfView - neutralFov) < 0.0001f, "Camera effects did not return to neutral.");

            Volume volume = UnityEngine.Object.FindFirstObjectByType<Volume>();
            Require(volume != null && volume.isGlobal && volume.sharedProfile.name == "PrisonVisualProfile",
                "Dedicated visual post processing profile is missing.");
            Require(camera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing, "Camera post processing is off.");
            Debug.Log("VISUAL_SLICE_SMOKE_PASS: visual replacement, colliders, debug lines, crouch, camera and volume.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception(message);
            }
        }
    }
}

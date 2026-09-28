using PrisonRunner.Core;
using UnityEngine;

namespace PrisonRunner.Application
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float forwardSpeed = 9f;
        [SerializeField] private float laneSpacing = 3f;
        [SerializeField] private float laneChangeSpeed = 12f;
        [SerializeField] private float jumpHeight = 1.8f;
        [SerializeField] private float gravity = 24f;
        [SerializeField] private float crouchHeight = 1f;
        [SerializeField] private float stumbleSeconds = 0.75f;
        [SerializeField] private float stumbleSpeedMultiplier = 0.35f;

        private CharacterController controller;
        private Transform visual;
        private int lane = LaneRules.Center;
        private float verticalSpeed;
        private float stumbleUntil;
        private bool crouching;

        public int CurrentLane => lane;
        public bool IsCrouching => crouching;

        public void Configure(Transform visualRoot)
        {
            controller = GetComponent<CharacterController>();
            visual = visualRoot;
        }

        public void Tick(RunnerInput input, float deltaTime)
        {
            if (controller == null || deltaTime <= 0f)
            {
                return;
            }

            if (input.LeftPressed != input.RightPressed)
            {
                lane = LaneRules.Move(lane, input.LeftPressed ? -1 : 1);
            }

            SetCrouch(input.CrouchHeld);

            if (controller.isGrounded && verticalSpeed < 0f)
            {
                verticalSpeed = -2f;
            }

            if (input.JumpPressed && controller.isGrounded && !crouching)
            {
                verticalSpeed = Mathf.Sqrt(2f * gravity * jumpHeight);
            }

            verticalSpeed -= gravity * deltaTime;
            float targetX = lane * laneSpacing;
            float nextX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * deltaTime);
            float speed = Time.time < stumbleUntil ? forwardSpeed * stumbleSpeedMultiplier : forwardSpeed;
            Vector3 displacement = new Vector3(nextX - transform.position.x, verticalSpeed * deltaTime, speed * deltaTime);
            CollisionFlags flags = controller.Move(displacement);
            if ((flags & CollisionFlags.Above) != 0 && verticalSpeed > 0f)
            {
                verticalSpeed = 0f;
            }
        }

        private void SetCrouch(bool requested)
        {
            if (requested == crouching)
            {
                return;
            }

            if (!requested && !CanStand())
            {
                return;
            }

            crouching = requested;
            controller.height = crouching ? crouchHeight : 2f;
            controller.center = Vector3.up * (controller.height * 0.5f);
            if (visual != null)
            {
                visual.localScale = crouching ? new Vector3(1f, 0.5f, 1f) : Vector3.one;
                visual.localPosition = Vector3.up * (controller.height * 0.5f);
            }
        }

        private bool CanStand()
        {
            Collider[] overlaps = Physics.OverlapCapsule(
                transform.position + Vector3.up * controller.radius,
                transform.position + Vector3.up * (2f - controller.radius),
                controller.radius * 0.95f,
                ~0,
                QueryTriggerInteraction.Ignore);

            foreach (Collider overlap in overlaps)
            {
                if (overlap != controller)
                {
                    return false;
                }
            }

            return true;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.GetComponent<RunnerObstacle>() != null && hit.moveDirection.z > 0.1f)
            {
                stumbleUntil = Mathf.Max(stumbleUntil, Time.time + stumbleSeconds);
            }
        }
    }
}

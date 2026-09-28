using PrisonRunner.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PrisonRunner.Infrastructure
{
    public sealed class KeyboardInputProvider : MonoBehaviour, IGameInput
    {
        public RunnerInput ReadInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return default;
            }

            return new RunnerInput
            {
                LeftPressed = keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame,
                RightPressed = keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame,
                JumpPressed = keyboard.spaceKey.wasPressedThisFrame,
                CrouchHeld = keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed
            };
        }
    }
}

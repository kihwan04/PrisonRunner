using PrisonRunner.Core;
using UnityEngine;

namespace PrisonRunner.Application
{
    public sealed class PlayerActionController : MonoBehaviour
    {
        private IGameInput input;
        private PlayerMovement movement;

        public void Configure(IGameInput inputProvider, PlayerMovement playerMovement)
        {
            input = inputProvider;
            movement = playerMovement;
        }

        private void Update()
        {
            if (input != null && movement != null)
            {
                movement.Tick(input.ReadInput(), Time.deltaTime);
            }
        }
    }
}

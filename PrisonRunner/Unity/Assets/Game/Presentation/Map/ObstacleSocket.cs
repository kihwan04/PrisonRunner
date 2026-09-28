using UnityEngine;

namespace PrisonRunner.Presentation.Map
{
    public sealed class ObstacleSocket : MonoBehaviour
    {
        public int Row { get; private set; }
        public int Lane { get; private set; }

        public void Configure(int row, int lane)
        {
            Row = row;
            Lane = lane;
        }
    }
}

using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Presentation.Map
{
    // The missing rail is part of the authored bridge mesh, not a crate on the track.
    public sealed class CartRailGap : MonoBehaviour
    {
        public int RequiredLean;
        public bool Passed {get;private set;}
        public void ResetGap()=>Passed=false;
        public void Evaluate(RunSession run,float previousZ)
        {
            if(Passed||!gameObject.activeInHierarchy||run.Paused||run.Flow.State!=GameFlowState.Running)return;
            if(previousZ>transform.position.z||run.Runner.Z<transform.position.z)return;
            Passed=true;
            if(!run.Runner.Riding||run.Runner.CartLean*RequiredLean<.55f)run.Fail(CaptureReason.BrokenRail);
            else run.Score.CleanPass(run.Config.CleanBonus);
        }
    }
}

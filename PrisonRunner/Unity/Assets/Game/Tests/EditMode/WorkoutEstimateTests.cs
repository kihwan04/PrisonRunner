using Muhanok.Domain;
using NUnit.Framework;

namespace Muhanok.Tests
{
    public sealed class WorkoutEstimateTests
    {
        [Test] public void OptionalWeightDoesNotInventCalories()
        {
            var w=new WorkoutEstimate();Assert.IsTrue(w.SetWeight(""));
            w.Tick(60,true,true,MotionCommand.HighKnee);Assert.IsFalse(w.WeightKg.HasValue);Assert.AreEqual(0,w.EstimatedKcal);
        }
        [TestCase("NaN")][TestCase("Infinity")][TestCase("0")][TestCase("500")][TestCase("70kg")]
        public void InvalidWeightCannotEnableTracking(string input){var w=new WorkoutEstimate();Assert.IsFalse(w.SetWeight(input));Assert.IsNull(w.WeightKg);}
        [Test] public void WeightAndActualRecognizedTimeScaleEstimate()
        {
            var w=new WorkoutEstimate(); Assert.IsTrue(w.SetWeight("70"));
            w.Tick(60,true,true,MotionCommand.HighKnee); Assert.AreEqual(4.9,w.EstimatedKcal,.0001);
            w.Tick(60,true,true,MotionCommand.Jump); Assert.AreEqual(9.8,w.EstimatedKcal,.0001);
        }
        [Test] public void LostPoseIdleKeyboardAndPauseAreExcluded()
        {
            var w=new WorkoutEstimate();w.SetWeight("70");
            w.Tick(60,true,false,MotionCommand.Jump);w.Tick(60,false,true,MotionCommand.Jump);
            w.Tick(60,true,true,MotionCommand.Center);w.Tick(double.NaN,true,true,MotionCommand.Jump);
            Assert.AreEqual(0,w.ActiveSeconds);Assert.AreEqual(0,w.EstimatedKcal);
        }
        [Test] public void RetryClearsEstimateButKeepsOptionalWeight()
        {
            var run=new RunSession(new RunConfig());run.Workout.SetWeight("70");run.Workout.Tick(60,true,true,MotionCommand.Left);
            run.Reset();Assert.AreEqual(70,run.Workout.WeightKg);Assert.AreEqual(0,run.Workout.EstimatedKcal);
            run.Workout.SetWeight("");Assert.IsNull(run.Workout.WeightKg);
        }
        [Test] public void RequestedFourAreasRunInOrderAcrossCycles()
        {
            var expected=new[]{0,1,2,3,7,4,5,6,11,12};
            for(int lap=0;lap<3;lap++)for(int i=0;i<expected.Length;i++)Assert.AreEqual(expected[i],RouteSurface.TypeAt(lap*10+i));
        }
    }
}

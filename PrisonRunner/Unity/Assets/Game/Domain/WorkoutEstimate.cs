using System;
using System.Globalization;

namespace Muhanok.Domain
{
    // 2024 Adult Compendium code 22240: total-body active game, moderate effort (4 MET).
    // This estimates gross expenditure; camera recognition does not measure metabolism.
    public sealed class WorkoutEstimate
    {
        public const double AssumedMet=4;
        public double? WeightKg {get; private set;}
        public double ActiveSeconds {get; private set;}
        public double Met {get;set;}=AssumedMet;
        public double EstimatedKcal=>WeightKg.HasValue?Met*3.5*WeightKg.Value/200*ActiveSeconds/60:0;
        public double GoalKcal {get; private set;}=50;
        private readonly double[] activitySeconds=new double[5];
        public bool GoalReached=>WeightKg.HasValue && EstimatedKcal>=GoalKcal;
        public double GoalProgress=>Math.Min(1,EstimatedKcal/GoalKcal);
        public bool SetGoal(double kcal) { if(double.IsNaN(kcal)||double.IsInfinity(kcal)||kcal<1||kcal>5000)return false; GoalKcal=kcal;return true; }
        public bool SetGoal(string value) => double.TryParse(value,NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out double kcal) && SetGoal(kcal);
        public double SecondsFor(int activity)=>activitySeconds[Math.Max(0,Math.Min(4,activity))];
        public double KcalFor(int activity)=>WeightKg.HasValue?Met*3.5*WeightKg.Value/200*SecondsFor(activity)/60:0;
        public static readonly string[] ActivityNames={"달리기·팔 흔들기","점프·두 손 들기","숙이기","하이니","좌우 이동·기울이기"};
        // Allocate one time slice to one activity. All use the play-style MET assumption, not invented per-joint measurements.
        public void TickActivity(double seconds,bool playing,bool tracked,bool active,int activity)
        {
            double before=ActiveSeconds; Tick(seconds,playing,tracked&&active);
            activitySeconds[Math.Max(0,Math.Min(4,activity))]+=ActiveSeconds-before;
        }
        public bool SetWeight(string value)
        {
            if(string.IsNullOrWhiteSpace(value)){WeightKg=null; Reset(); return true;}
            if(!double.TryParse(value,NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out double kg)
                ||double.IsNaN(kg)||double.IsInfinity(kg)||kg<10||kg>350) return false;
            WeightKg=kg; Reset(); return true;
        }
        public void Tick(double seconds,bool playing,bool poseConnected,MotionCommand recognizedMotion)
        {
            if(!WeightKg.HasValue||!playing||!poseConnected||recognizedMotion==MotionCommand.Center
                ||double.IsNaN(seconds)||double.IsInfinity(seconds)||seconds<=0) return;
            ActiveSeconds+=seconds;
        }
        // Camera styles count seconds in which the classifier saw real exercise (including running in place).
        public void Tick(double seconds,bool playing,bool active)
        {
            if(!WeightKg.HasValue||!playing||!active||double.IsNaN(seconds)||double.IsInfinity(seconds)||seconds<=0) return;
            ActiveSeconds+=seconds;
        }
        public void Reset(){ActiveSeconds=0;Array.Clear(activitySeconds,0,activitySeconds.Length);}
    }
}

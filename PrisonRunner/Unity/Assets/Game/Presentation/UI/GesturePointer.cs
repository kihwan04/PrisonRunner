using Muhanok.Domain;
using UnityEngine;
namespace Muhanok.Presentation
{
    public static class GesturePointer
    {
        private static readonly GestureDwell dwell=new GestureDwell();
        public static Vector2 Position {get;private set;}
        public static bool Visible {get;private set;}
        private static string hovered;
        private static int lastFrame=-1;
        public static void Feed(PoseFrame frame,bool valid,double now)
        {
            Visible=false;
            if(valid && frame!=null && frame.HasPose && now-frame.Time<.25)
            {
                var l=frame.Points[PoseJoint.LeftWrist]; var r=frame.Points[PoseJoint.RightWrist];
                int index=r.Visibility>=.6f && r.Y<frame.Points[PoseJoint.RightElbow].Y ? PoseJoint.RightWrist : PoseJoint.LeftWrist;
                var hand=frame.Points[index]; var elbow=frame.Points[index==PoseJoint.RightWrist?PoseJoint.RightElbow:PoseJoint.LeftElbow];
                var a=frame.Points[11]; var b=frame.Points[12];
                float span=Mathf.Max(.14f,Mathf.Abs(a.X-b.X)); float center=(a.X+b.X)*.5f;
                Visible=hand.Visibility>=.6f && elbow.Visibility>=.5f && hand.Y<elbow.Y-.02f;
                if(Visible)
                {
                    var next=new Vector2(Mathf.InverseLerp(center-span*1.8f,center+span*1.8f,hand.X)*1280,
                        Mathf.InverseLerp((a.Y+b.Y)*.5f-span*1.4f,(a.Y+b.Y)*.5f+span*.9f,hand.Y)*720);
                    Position=Vector2.Lerp(Position,next,.4f);
                }
            }
            if(!Visible) dwell.Reset();
        }
        public static bool Activate(Rect rect,string id)
        {
            if(Event.current.type!=EventType.Repaint || !GUI.enabled || !Visible || !rect.Contains(Position)) return false;
            hovered=id+rect.ToString();
            return dwell.Tick(hovered,Time.realtimeSinceStartupAsDouble,true);
        }
        public static void Draw()
        {
            if(Event.current.type!=EventType.Repaint) return;
            if(lastFrame!=Time.frameCount) { if(hovered==null) dwell.Tick(null,Time.realtimeSinceStartupAsDouble,false); hovered=null; lastFrame=Time.frameCount; }
            if(!Visible) return;
            GUI.color=new Color(.2f,1,.6f,.9f); GUI.DrawTexture(new Rect(Position.x-9,Position.y-9,18,18),Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(Position.x-24,Position.y+17,48*(float)dwell.Progress,5),Texture2D.whiteTexture); GUI.color=Color.white;
        }
        public static void Reset(){Visible=false;dwell.Reset();hovered=null;}
    }
}

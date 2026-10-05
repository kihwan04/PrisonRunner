using System;
using UnityEngine;

namespace Muhanok.Presentation
{
    public sealed partial class HUDPresenter
    {
        public Texture2D LoadingImage;
        public Action<string> BeginRun;
        public Action PauseRun;
        public bool SetupOpen {get; private set;}
        public bool Loading {get; private set;}
        public bool Revealing=>revealRemaining>0;
        public float RevealOpacity=>Mathf.SmoothStep(0,1,revealRemaining/.85f);
        public bool BlocksStart=>SetupOpen||Loading||Revealing;
        private float revealRemaining;
        public float LoadingProgress {get; set;}
        private string weightText="",validation="";
        private Texture2D panelTexture;
        private GUIStyle panelStyle;
        private Texture2D coinIcon,distanceIcon;
        public void OpenSetup(){SetupOpen=true; validation="";}
        public void CloseSetup(){SetupOpen=false; Loading=false;revealRemaining=0;}
        public void BeginLoading(){SetupOpen=false;Loading=true;}
        public void EndLoading(){Loading=false;revealRemaining=.85f;}
        private void Update(){revealRemaining=Mathf.Max(0,revealRemaining-Time.unscaledDeltaTime);}
        private void Card(Rect r,Color color)
        {
            if(panelStyle==null)
            {
                panelTexture=new Texture2D(48,48,TextureFormat.RGBA32,false);panelTexture.hideFlags=HideFlags.DontSave;
                panelTexture.wrapMode=TextureWrapMode.Clamp;
                for(int y=0;y<48;y++)for(int x=0;x<48;x++)
                {
                    float dx=Mathf.Max(12-x,x-35),dy=Mathf.Max(12-y,y-35);
                    float distance=Mathf.Sqrt(Mathf.Max(0,dx)*Mathf.Max(0,dx)+Mathf.Max(0,dy)*Mathf.Max(0,dy));
                    float alpha=1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(11,12,distance));
                    panelTexture.SetPixel(x,y,new Color(1,1,1,alpha));
                }
                panelTexture.Apply();panelStyle=new GUIStyle();panelStyle.normal.background=panelTexture;panelStyle.border=new RectOffset(12,12,12,12);
            }
            var old=GUI.color;GUI.color=color;GUI.Box(r,GUIContent.none,panelStyle);GUI.color=old;
        }
        private void DrawStartOverlay()
        {
            if(Loading||Revealing)
            {
                float alpha=Loading?1:RevealOpacity;
                if(Loading){GUI.color=Color.black; GUI.DrawTexture(new Rect(0,0,1280,720),Texture2D.whiteTexture);}
                GUI.color=new Color(1,1,1,alpha);
                var imageRect=new Rect(0,0,1280,720);
                if(LoadingImage!=null)
                {
                    float scale=Mathf.Min(1280f/LoadingImage.width,720f/LoadingImage.height);
                    imageRect=new Rect((1280-LoadingImage.width*scale)*.5f,(720-LoadingImage.height*scale)*.5f,LoadingImage.width*scale,LoadingImage.height*scale);
                    GUI.DrawTexture(imageRect,LoadingImage,ScaleMode.StretchToFill);
                }
                // Coordinates of the reference's decorative bar, in its original 1672 x 941 image.
                // Repaint that same bar so no second, fake progress indicator remains visible.
                var progress=new Rect(imageRect.x+imageRect.width*537f/1672f,imageRect.y+imageRect.height*846f/941f,imageRect.width*600f/1672f,imageRect.height*28f/941f);
                GUI.color=new Color(1,.90f,.70f,alpha); GUI.DrawTexture(progress,Texture2D.whiteTexture);
                var inside=new Rect(progress.x+2,progress.y+2,progress.width-4,progress.height-4);
                GUI.color=new Color(.08f,.045f,.025f,alpha); GUI.DrawTexture(inside,Texture2D.whiteTexture);
                GUI.color=new Color(1,.77f,.12f,alpha); GUI.DrawTexture(new Rect(inside.x,inside.y,inside.width*Mathf.Clamp01(LoadingProgress),inside.height),Texture2D.whiteTexture);
                GUI.color=new Color(1,1,1,alpha); var centered=new GUIStyle(small){alignment=TextAnchor.MiddleCenter};
                GUI.Label(new Rect(420,progress.yMax+8,440,30),"탈출 준비 중  "+Mathf.RoundToInt(LoadingProgress*100)+"%",centered); return;
            }
            Card(new Rect(0,0,1280,720),new Color(.025f,.035f,.06f,.80f));
            Card(new Rect(375,150,530,450),new Color(.055f,.075f,.11f,.97f));
            GUI.color=new Color(1,.78f,.32f); GUI.Label(new Rect(420,185,440,50),"탈출 준비",Style(36,FontStyle.Bold));
            GUI.color=Color.white;
            GUI.Label(new Rect(420,251,450,34),"몸무게 (선택 입력)",label);
            var inputStyle=new GUIStyle(GUI.skin.textField){font=font,fontSize=30,alignment=TextAnchor.MiddleLeft};
            GUI.SetNextControlName("MuhanokWeight"); weightText=GUI.TextField(new Rect(420,302,350,54),weightText,7,inputStyle);
            GUI.Label(new Rect(790,309,70,40),"kg",label);
            GUI.Label(new Rect(420,375,440,62),"몸동작 시간으로 예상 소모 칼로리를 계산합니다.\n입력하지 않아도 게임을 시작할 수 있습니다.",Style(17,FontStyle.Normal));
            GUI.color=new Color(1,.55f,.35f); GUI.Label(new Rect(420,441,440,28),validation,small); GUI.color=Color.white;
            if(FlatButton(new Rect(420,488,440,51),"시작하기",true))
            {
                var probe=new Muhanok.Domain.WorkoutEstimate();
                if(probe.SetWeight(weightText)) BeginRun?.Invoke(weightText);
                else validation="10~350kg 범위의 숫자를 입력해 주세요.";
            }
            if(FlatButton(new Rect(420,551,440,32),"입력 없이 시작",false)) BeginRun?.Invoke("");
        }
        private GUIStyle flatButton;
        private bool FlatButton(Rect rect,string text,bool primary)
        {
            if(flatButton==null)
            {
                flatButton=new GUIStyle(button);
                foreach(var state in new[]{flatButton.normal,flatButton.hover,flatButton.active,flatButton.focused}) state.background=null;
            }
            GUI.color=primary?new Color(1,.76f,.23f):new Color(.12f,.42f,.67f);
            Card(rect,GUI.color);
            GUI.color=primary?new Color(.15f,.09f,.025f):Color.white;
            bool pressed=GUI.Button(rect,text,flatButton); GUI.color=Color.white; return pressed;
        }
        private void DrawRunCards()
        {
            if(coinIcon==null){coinIcon=CreateHudIcon(false);distanceIcon=CreateHudIcon(true);}
            Card(new Rect(24,24,230,65),new Color(.035f,.045f,.065f,.88f));
            Card(new Rect(24,100,230,65),new Color(.035f,.045f,.065f,.88f));
            GUI.color=Color.white;GUI.DrawTexture(new Rect(39,32,49,49),coinIcon);
            GUI.color=Color.white; GUI.Label(new Rect(123,34,120,45),session.Score.Coins.ToString(),Style(34,FontStyle.Bold));
            GUI.color=Color.white;GUI.DrawTexture(new Rect(39,108,49,49),distanceIcon);
            GUI.color=Color.white; GUI.Label(new Rect(121,108,130,40),session.Score.Distance.ToString("N0")+" m",label);
            var center=new GUIStyle(label){alignment=TextAnchor.MiddleCenter};
            Card(new Rect(514,23,252,47),new Color(.035f,.045f,.065f,.78f));
            GUI.Label(new Rect(514,29,252,35),Muhanok.Domain.RouteSurface.AreaAt(session.Runner.Z),center);
            if(session.Workout.WeightKg.HasValue)
            {
                Card(new Rect(24,177,230,50),new Color(.035f,.045f,.065f,.88f));
                GUI.Label(new Rect(38,190,220,30),"예상  "+session.Workout.EstimatedKcal.ToString("F1")+" kcal",small);
            }
            if(session.Runner.Riding)
            {
                GUI.Label(new Rect(950,32,220,35),"수레 · 좌우 균형",small);
                GUI.color=new Color(.1f,.12f,.17f);GUI.DrawTexture(new Rect(950,71,220,12),Texture2D.whiteTexture);
                GUI.color=new Color(1,.72f,.12f);GUI.DrawTexture(new Rect(950,71,220*Muhanok.Domain.RouteSurface.CartProgress(session.Runner.Z),12),Texture2D.whiteTexture);
            }
            else
            {
                GUI.color=session.Runner.Slowing?new Color(1,.65f,.2f):Color.white;
                GUI.Label(new Rect(935,36,242,38),session.Flow.State==Muhanok.Domain.GameFlowState.Caught&&session.Reason==Muhanok.Domain.CaptureReason.BananaPursuit?"바나나 2회 · 추격 종료":session.BananaPursuit?"경찰이 따라잡고 있어요!":session.Runner.Slowing?"미끄러짐 · 감속":session.BananaHits>0?"바나나 1 / 2 · 추격 주의":"장애물 충돌 시 종료",Style(17,FontStyle.Bold));
            }
            GUI.color=Color.white;
            Card(new Rect(24,238,230,44),new Color(.035f,.045f,.065f,.88f));
            GUI.Label(new Rect(38,247,210,30),"속도  "+session.CurrentSpeed.ToString("F1")+" m/s",small);
            DrawActionCue();
            if(FlatButton(new Rect(1190,23,66,64),session.Paused?"▶":"Ⅱ",false)) PauseRun?.Invoke();
        }
        private void DrawActionCue()
        {
            if(Map==null||session.Paused)return;
            float nearest=float.MaxValue;string cue=null;
            foreach(var active in Map.Active)
            {
                if(session.Runner.Riding)
                {
                    foreach(var gap in active.Chunk.RailGaps)
                    {
                        float d=gap.transform.position.z-session.Runner.Z;
                        if(gap.Passed||d<0||d>14||d>=nearest)continue;
                        nearest=d;cue=gap.RequiredLean<0?"←  왼쪽으로 기울이세요":"오른쪽으로 기울이세요  →";
                    }
                }
                else
                {
                    ConsiderActionCue(active.Chunk.Obstacles,ref nearest,ref cue);
                    ConsiderActionCue(active.Chunk.ExerciseObstacles,ref nearest,ref cue);
                }
            }
            if(cue==null)return;
            Card(new Rect(415,560,450,54),new Color(.035f,.045f,.065f,.83f));
            var style=Style(23,FontStyle.Bold);style.alignment=TextAnchor.MiddleCenter;
            GUI.color=Color.white;GUI.Label(new Rect(415,565,450,45),cue,style);
        }
        private void ConsiderActionCue(Muhanok.Presentation.Map.ObstacleController[] hazards,ref float nearest,ref string cue)
        {
            if(hazards==null)return;
            float lookAhead=Mathf.Max(9f,session.TargetSpeed*1.35f);
            foreach(var hazard in hazards)
            {
                float d=hazard.transform.position.z-session.Runner.Z;
                if(!hazard.gameObject.activeSelf||hazard.Passed||d<0||d>lookAhead||d>=nearest)continue;
                if(!hazard.FullExerciseRow)
                {
                    Muhanok.Domain.RouteSurface.Sample(hazard.transform.position.z,out float offset,out _);
                    if(Mathf.Abs(session.Runner.Lane*session.Config.LaneSpacing+offset-hazard.transform.position.x)>.95f)continue;
                }
                nearest=d;
                cue=hazard.GetComponent<Muhanok.Presentation.Map.ClosingSlideGate>()!=null?"↓  문 아래로 슬라이딩!":hazard.Kind==Muhanok.Domain.ObstacleKind.Pipe?"↓  슬라이드하세요":hazard.Kind==Muhanok.Domain.ObstacleKind.Stair?"무릎을 높이 들어 올리세요":hazard.Kind==Muhanok.Domain.ObstacleKind.Crate||hazard.Kind==Muhanok.Domain.ObstacleKind.Cart?"↑  점프하세요":"←  옆 레인으로 피하세요  →";
            }
        }
        private static Texture2D CreateHudIcon(bool crown)
        {
            var image=new Texture2D(64,64,TextureFormat.RGBA32,false){hideFlags=HideFlags.DontSave,wrapMode=TextureWrapMode.Clamp};
            for(int y=0;y<64;y++)for(int x=0;x<64;x++)
            {
                float dx=x-31.5f,dy=y-31.5f, radius=Mathf.Sqrt(dx*dx+dy*dy);
                float alpha=1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(29,30,radius));
                Color color=radius>25?new Color(1,.87f,.30f):radius>22?new Color(.72f,.40f,.035f):new Color(1,.71f,.055f);
                if(radius<17)color=new Color(1,.78f,.10f);
                if(radius>16&&radius<18)color=new Color(1,.91f,.43f);
                if(crown)
                {
                    float top=46-Mathf.Min(Mathf.Abs(x-13),Mathf.Min(Mathf.Abs(x-32),Mathf.Abs(x-51)))*1.55f;
                    alpha=x>=10&&x<=53&&y>=12&&y<=top?1:0;
                    color=y<18?new Color(.85f,.49f,.03f):new Color(1,.78f,.11f);
                    if((x-32)*(x-32)+(y-23)*(y-23)<12)color=new Color(1,.94f,.62f);
                }
                color.a=alpha;image.SetPixel(x,y,color);
            }
            image.Apply();return image;
        }
        private void DrawPause()
        {
            Card(new Rect(0,0,1280,720),new Color(.025f,.035f,.06f,.75f));
            Card(new Rect(420,255,440,210),new Color(.055f,.075f,.11f,.97f));
            GUI.color=Color.white; GUI.Label(new Rect(470,285,350,65),"잠깐 쉬어가기",Style(34,FontStyle.Bold));
            if(FlatButton(new Rect(470,375,340,55),"계속하기",true)) PauseRun?.Invoke();
        }
    }
}

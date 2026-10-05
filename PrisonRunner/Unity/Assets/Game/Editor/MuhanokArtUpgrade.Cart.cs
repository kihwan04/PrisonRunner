using System.Linq;
using Muhanok.Domain;
using Muhanok.Presentation.Map;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Muhanok.Editor
{
    public static partial class MuhanokArtUpgrade
    {
        private static void EnsureCartAnimation()
        {
            var controller=AssetDatabase.LoadAssetAtPath<AnimatorController>(Root+"/Animations/Monkey.controller");
            var state=controller.layers[0].stateMachine.states.FirstOrDefault(s=>s.state.name=="AN_Monkey_CartRide").state;
            if(state==null)state=controller.layers[0].stateMachine.AddState("AN_Monkey_CartRide");
            state.motion=AssetDatabase.LoadAllAssetsAtPath(Characters+"/AN_Monkey_CartRide.fbx").OfType<AnimationClip>().First(c=>!c.name.StartsWith("__"));
            EditorUtility.SetDirty(controller);
        }
        private static GameObject CreateRideCart()
        {
            var root=new GameObject("PROP_CartRide");
            Prop("blender","MineCartEmpty",root.transform,Vector3.zero,1.10f);
            var source=root.AddComponent<AudioSource>();source.clip=AssetDatabase.LoadAssetAtPath<AudioClip>(Root+"/Audio/MineCart_Rumble.wav");
            source.loop=true;source.playOnAwake=false;source.volume=.24f;source.spatialBlend=0;
            var fill=Node("RiderSoftLight",root.transform,new Vector3(0,1.5f,-1.1f)).gameObject.AddComponent<Light>();
            fill.type=LightType.Point;fill.color=new Color(1,.76f,.52f);fill.intensity=1.35f;fill.range=3.3f;fill.shadows=LightShadows.None;
            for(int side=-1;side<=1;side+=2)
            {
                Shape("ForwardLamp",root.transform,new Vector3(side*.46f,.60f,.66f),Vector3.one*.16f,glow,true);
                Shape("Grip",root.transform,new Vector3(side*.52f,.93f,.43f),new Vector3(.24f,.07f,.09f),iron);
            }
            string path=Root+"/Prefabs/PROP_CartRide.prefab";
            PrefabUtility.SaveAsPrefabAsset(root,path);Object.DestroyImmediate(root);
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        private static void CartCavern(Transform env,int type)
        {
            var abyss=Mat("CavernDepth",new Color(.055f,.09f,.12f),0,.1f);
            Shape("RavineFarFloor",env,new Vector3(0,-12,12),new Vector3(38,.5f,24),abyss);
            Shape("CavernCeiling",env,new Vector3(0,11,12),new Vector3(34,1,24),rock);
            for(int side=-1;side<=1;side+=2)
            {
                Shape("RavineCliffBacking",env,new Vector3(side*16,-1,12),new Vector3(3,22,24),rock);
                for(int z=1;z<24;z+=4)
                {
                    Prop("blender","CaveWall",env,new Vector3(side*14,-10,z),14,side<0?0:180);
                    Prop("blender","RockPile",env,new Vector3(side*(7+Next()*4),-8,z),2.4f,Next()*360);
                    Beam("SuspendedBridgeBrace",env,new Vector3(side*.96f,-.13f,z),new Vector3(side*13,-7,z),.23f,wood);
                    Beam("DistantGalleryRail",env,new Vector3(side*11,-3,z),new Vector3(side*11,-3,z+4),.10f,iron);
                    Shape("DistantGallery",env,new Vector3(side*11,-4,z+2),new Vector3(2,.15f,4),wood);
                    Lantern(env,new Vector3(side*11,-2.8f,z),true);
                }
                for(int z=2;z<24;z+=6)
                {
                    Shape("BridgeLampPost",env,new Vector3(side*1.38f,1.4f,z),new Vector3(.20f,2.8f,.25f),wood);
                    Shape("BridgeIronShoe",env,new Vector3(side*1.38f,.20f,z),new Vector3(.28f,.4f,.30f),iron);
                    Lantern(env,new Vector3(side*1.2f,2.65f,z),false,side<0);
                }
            }
            for(int station=0;station<48;station++)
            {
                float z=station*.5f+.25f;int missing=RouteSurface.CartMissingSide(type,z);
                for(int side=-1;side<=1;side+=2)
                {
                    if(side==missing)continue;
                    Shape("SingleRail",env,new Vector3(side*.70f,.11f,z),new Vector3(.095f,.14f,.502f),iron);
                    Shape("RailFastener",env,new Vector3(side*.70f,.05f,z),new Vector3(.18f,.045f,.13f),pale);
                }
                Shape("BridgeSleeper",env,new Vector3(missing==0?0:-missing*.49f,-.035f,z),new Vector3(missing==0?2.04f:1.0f,.09f,.22f),wood);
                for(int side=-1;side<=1;side+=2)if(side!=missing)
                    Shape("DeckEdge",env,new Vector3(side*1.02f,-.10f,z),new Vector3(.09f,.18f,.502f),iron);
            }
            foreach(float z in new[]{8f,18f})
            {
                int missing=RouteSurface.CartMissingSide(type,z),safe=-missing;
                var broken=Shape("HangingBrokenRail",env,new Vector3(missing*.7f,-.75f,z-.5f),new Vector3(.10f,.14f,1.7f),iron);
                broken.transform.localRotation=Quaternion.Euler(50,0,missing*22);
                for(int i=0;i<3;i++)
                {
                    var plank=Shape("SplinteredSleeper",env,new Vector3(missing*(.5f+i*.12f),-.2f-i*.32f,z-1+i),new Vector3(.65f,.10f,.24f),wood);
                    plank.transform.localRotation=Quaternion.Euler(i*27,15,missing*35);
                }
                var sign=Node("TrackWarningSign",env,new Vector3(missing*1.5f,1.4f,z-3.1f));
                Shape("ArrowBoard",sign,Vector3.zero,new Vector3(1.04f,.65f,.10f),iron);
                Beam("ArrowShaft",sign,new Vector3(-safe*.32f,0,-.075f),new Vector3(safe*.32f,0,-.075f),.08f,brass);
                for(int wing=-1;wing<=1;wing+=2)Beam("ArrowHead",sign,new Vector3(safe*.32f,0,-.075f),new Vector3(safe*.07f,wing*.22f,-.075f),.08f,brass);
                for(int i=0;i<3;i++)Shape("SafeSideGoldMarker",env,new Vector3(safe*.92f,.12f,z-2.7f+i*.65f),new Vector3(.12f,.06f,.32f),brass);
            }
            for(int z=5;z<24;z+=12)Prop("blender","CaveArch",env,new Vector3(0,7,z),4.3f);
            AmbientMotes(env,new Vector3(0,2,12),24,true);
        }
        private static void CartDock(Transform env,bool exit)
        {
            float z=exit?2:22;
            Label(exit?"DISEMBARK / SERVICE STAIRS":"BOARD / BALANCE LEFT OR RIGHT",env,new Vector3(0,3.7f,z),.11f,cream.color);
            for(int side=-1;side<=1;side+=2)
            {
                Shape("DockPlatform",env,new Vector3(side*2.0f,.1f,z),new Vector3(1.9f,.2f,4),wood);
                Shape("PlatformEdge",env,new Vector3(side*1.05f,.24f,z),new Vector3(.12f,.08f,4),brass);
                PropCart(env,new Vector3(side*4.1f,0,z),.75f);
            }
            for(int side=-1;side<=1;side+=2)Shape("DockSingleTrack",env,new Vector3(side*.7f,.12f,z),new Vector3(.10f,.15f,4),iron);
        }
        private static void AddRailGaps(GameObject root,int type)
        {
            var prior=root.transform.Find("CartRailGaps");if(prior!=null)Object.DestroyImmediate(prior.gameObject);
            var chunk=root.GetComponent<MuhanokMapChunk>();chunk.RailGaps=new CartRailGap[type==1||type==2?2:0];
            if(chunk.RailGaps.Length==0)return;
            var group=Node("CartRailGaps",root.transform,Vector3.zero);
            for(int i=0;i<2;i++)
            {
                float z=i==0?8:18;var gap=Node("BrokenRail_"+i,group,new Vector3(0,0,z)).gameObject.AddComponent<CartRailGap>();
                gap.RequiredLean=-RouteSurface.CartMissingSide(type,z);chunk.RailGaps[i]=gap;
            }
        }
        private static void MatchWalkingMotions(GameObject root,int type)
        {
            var chunk=root.GetComponent<MuhanokMapChunk>();
            foreach(var obstacle in chunk.Obstacles)
            {
                int row=obstacle.GetComponentInParent<MuhanokObstacleSocket>().Row;
                string replacement=type==0&&row==1||type==3&&row==1||type==6&&row==1||type==7&&row==1?"OBS_LowPipe":type==3&&row==0?"OBS_Crate_Low":type==7?"OBS_Stair_HighKnee":null;
                if(replacement!=null)
                {
                    Clear(obstacle.transform);var gate=obstacle.GetComponent<DescendingGate>();if(gate!=null)Object.DestroyImmediate(gate);
                    var source=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Prefabs/"+replacement+".prefab");
                    var instance=Object.Instantiate(source);obstacle.Kind=instance.GetComponent<ObstacleController>().Kind;
                    while(instance.transform.childCount>0)instance.transform.GetChild(0).SetParent(obstacle.transform,false);
                    Object.DestroyImmediate(instance);
                }
            }
            chunk.Gates=root.GetComponentsInChildren<DescendingGate>(true);
            if(type==1||type==2)return;
            // Motion guidance lives in the HUD; the actual route remains visually clear.
        }
    }
}

using Muhanok.Domain;
using Muhanok.Presentation.Map;
using UnityEngine;

namespace Muhanok.Editor
{
    public static partial class MuhanokArtUpgrade
    {
        private static void ZooGroundDetails(Transform env,int type)
        {
            var water=Mat("NightWetStone",new Color(.08f,.20f,.31f),.55f,.95f);
            var grass=Mat("ZooGrass",new Color(.14f,.32f,.15f),0,.2f);
            var sand=Mat("ZooPaving",new Color(.47f,.40f,.26f),0,.34f,"stone");
            var earth=Mat("EnclosureEarth",new Color(.22f,.34f,.19f),0,.15f);
            for(int side=-1;side<=1;side+=2)
                for(int segment=0;segment<12;segment++)
                {
                    // Complete the actual enclosure floor; trees and animals must not sit over the void.
                    Shape("EnclosureGround",env,new Vector3(side*13.3f,-.16f,segment*2+1),new Vector3(17f,.30f,2.01f),earth);
                    if(segment%2==0)
                    {
                        Prop("nature-kit","grass_leafsLarge",env,new Vector3(side*5.1f,0,segment*2+.8f),.42f,Next()*360);
                        Prop("nature-kit","plant_bushDetailed",env,new Vector3(side*9.5f,0,segment*2+.4f),.7f,Next()*360);
                    }
                }
            for(int side=-1;side<=1;side+=2)
                for(int z=2;z<24;z+=5)
                {
                    Prop("nature-kit","tree_detailed_dark",env,new Vector3(side*(13.7f+Next()*2),0,z),7+Next()*1.5f,Next()*360);
                    Prop("nature-kit","tree_palmDetailedTall",env,new Vector3(side*(18.5f+Next()*2),0,z+2),6+Next()*2,Next()*360);
                    Prop("nature-kit","plant_bushDetailed",env,new Vector3(side*11.6f,0,z+1),1.5f,Next()*360);
                }
            for(int z=1;z<24;z+=2)
            {
                for(int x=-2;x<=2;x++)
                    Shape("PavingStone",env,new Vector3(x*1.2f,.025f,z),new Vector3(1.16f,.05f,1.92f),sand);
                for(int side=-1;side<=1;side+=2)
                {
                    for(int tuft=0;tuft<3;tuft++)
                    {
                        var leaf=Shape("GrassBlade",env,new Vector3(side*(3.45f+Next()*.25f),.13f,z+Next()),new Vector3(.11f,.30f,.30f),grass);
                        leaf.transform.localRotation=Quaternion.Euler(0,Next()*180,side*25);
                    }
                    if(z%4==1) Lantern(env,new Vector3(side*3.65f,2.9f,z+.5f),false);
                }
            }
            for(int z=6;z<24;z+=9)
            {
                var puddle=Shape("EnclosureWater",env,new Vector3(z%2==0?-6.7f:6.7f,.065f,z),new Vector3(2.7f,.018f,3.1f),water,true);
                puddle.transform.localRotation=Quaternion.Euler(0,z*17,0);
            }
        }
        private static void BananaPeel(Transform parent,Vector3 position,float scale)
        {
            var banana=Node("BananaPeel",parent,position);banana.localScale=Vector3.one*scale;
            var peel=Mat("BananaPeelYellow",new Color(1,.72f,.06f),0,.35f);
            Shape("Stem",banana,new Vector3(0,.34f,0),new Vector3(.15f,.48f,.15f),peel);
            for(int petal=0;petal<4;petal++)
            {
                float angle=petal*Mathf.PI*.5f;
                var a=new Vector3(Mathf.Cos(angle)*.08f,.25f,Mathf.Sin(angle)*.08f);
                var b=new Vector3(Mathf.Cos(angle)*.37f,.045f,Mathf.Sin(angle)*.37f);
                var c=new Vector3(Mathf.Cos(angle)*.65f,.10f,Mathf.Sin(angle)*.65f);
                Beam("FoldedPeel",banana,a,b,.16f,peel);Beam("PeelTip",banana,b,c,.15f,peel);
            }
        }
        private static void AddZooHazards(GameObject root,int type)
        {
            if(type<11)return;
            foreach(var obstacle in root.GetComponent<MuhanokMapChunk>().Obstacles)
            {
                Clear(obstacle.transform);var old=obstacle.GetComponent<DescendingGate>();if(old!=null)Object.DestroyImmediate(old);
                var socket=obstacle.GetComponentInParent<MuhanokObstacleSocket>();
                if(socket.Row==0)
                {
                    obstacle.Kind=ObstacleKind.Crate;
                    var bark=Mat("ZooLogBark",new Color(.26f,.13f,.055f),0,.18f,"wood");
                    var heart=Mat("ZooLogHeart",new Color(.62f,.38f,.17f),0,.25f,"wood");
                    var log=Shape("FallenLog",obstacle.transform,new Vector3(0,.49f,0),new Vector3(1.9f,.95f,.8f),bark,true);
                    for(int side=-1;side<=1;side+=2) Shape("CutEnd",obstacle.transform,new Vector3(side*.84f,.49f,0),new Vector3(.08f,.73f,.66f),heart,true);
                }
                else
                {
                    obstacle.Kind=ObstacleKind.Barrier;
                    Prop("survival-kit","box-large",obstacle.transform,Vector3.zero,1.18f,8,wood);
                }
            }
        }
        private static void AddFloorItems(GameObject root,int type)
        {
            var old=root.transform.Find("FloorItems");if(old!=null)Object.DestroyImmediate(old.gameObject);
            var chunk=root.GetComponent<MuhanokMapChunk>();chunk.FloorItems=new FloorItemController[type==11?1:type==12?2:0];
            if(type<11)return;
            var group=Node("FloorItems",root.transform,Vector3.zero);
            for(int index=0;index<chunk.FloorItems.Length;index++)
            {
                var node=Node(index==0?"BananaSlip":"PuddleSlip",group,new Vector3(index==0?-2.2f:2.2f,.035f,index==0?10:17));
                var item=node.gameObject.AddComponent<FloorItemController>();item.Kind=index==0?FloorItemKind.BananaPeel:FloorItemKind.Puddle;
                if(index==0)BananaPeel(node,new Vector3(0,.04f,0),.75f);
                else Shape("Puddle",node,Vector3.zero,new Vector3(1.65f,.025f,2),Mat("NightWetStone",new Color(.08f,.20f,.31f),.55f,.95f),true);
                chunk.FloorItems[index]=item;
            }
        }
        private static void ServiceExit(Transform env)
        {
            for(int side=-1;side<=1;side+=2)
            {
                Shape("ExitLowWall",env,new Vector3(side*4.65f,.65f,20),new Vector3(.42f,1.30f,8.1f),concrete);
                for(int z=16;z<=24;z+=4)
                {
                    Shape("ServiceExitPost",env,new Vector3(side*4.25f,2.35f,z),new Vector3(.26f,4.7f,.26f),iron);
                    Lantern(env,new Vector3(side*3.65f,3.0f,z+.1f),false);
                    Prop("nature-kit","plant_bushDetailed",env,new Vector3(side*5.1f,0,z+1),.70f);
                }
                Prop("nature-kit","tree_palmDetailedTall",env,new Vector3(side*7.0f,0,21),4.6f);
            }
            Shape("ExitAwning",env,new Vector3(0,4.8f,18),new Vector3(9.5f,.22f,4),iron);
            Label("ANIMAL ENCLOSURES",env,new Vector3(0,4.20f,20),.16f,cream.color);
        }
    }
}

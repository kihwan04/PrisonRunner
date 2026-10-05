using Muhanok.Domain;
using Muhanok.Presentation.Map;
using UnityEngine;

namespace Muhanok.Editor
{
    public static partial class MuhanokArtUpgrade
    {
        private static void AddClosingPrisonEntry(GameObject root,int type)
        {
            var chunk=root.GetComponent<MuhanokMapChunk>();
            foreach(var old in root.GetComponentsInChildren<ClosingSlideGate>(true))Object.DestroyImmediate(old);
            chunk.SlideGates=new ClosingSlideGate[type==7?3:0];if(type!=7)return;
            for(int lane=0;lane<3;lane++)
            {
                var hazard=chunk.Obstacles[3+lane];Clear(hazard.transform);hazard.Kind=ObstacleKind.Pipe;
                var gate=hazard.gameObject.AddComponent<ClosingSlideGate>();chunk.SlideGates[lane]=gate;
                gate.Panel=Node("ClosingEntryShutter",hazard.transform,Vector3.zero);
                Shape("DoorSteel",gate.Panel,Vector3.zero,new Vector3(2.20f,3.30f,.25f),pale);
                for(int rib=0;rib<10;rib++)Shape("DoorSlat",gate.Panel,new Vector3(0,-1.48f+rib*.33f,-.16f),new Vector3(2.20f,.07f,.06f),iron);
                Shape("DoorWarningBand",gate.Panel,new Vector3(0,-1.25f,-.19f),new Vector3(2.2f,.66f,.055f),cream);
                for(int stripe=-3;stripe<=3;stripe++)
                {
                    var bar=Shape("DiagonalWarning",gate.Panel,new Vector3(stripe*.34f,-1.25f,-.225f),new Vector3(.16f,.63f,.03f),red);
                    bar.transform.localRotation=Quaternion.Euler(0,0,-24);
                }
                Shape("DoorBottomSeal",gate.Panel,new Vector3(0,-1.59f,0),new Vector3(2.20f,.12f,.34f),black);
                Shape("DoorDrive",hazard.transform,new Vector3(0,4.38f,0),new Vector3(2.2f,.34f,.40f),iron);
                var warning=Mat("GateWarningRed",new Color(1,.035f,.015f),0,.45f);Emit(warning,Color.red*3);
                Shape("EntryAlarm",hazard.transform,new Vector3(0,4.13f,-.34f),new Vector3(.23f,.20f,.15f),warning,true);
                gate.ResetGate();
            }
            var env=root.transform.Find("EnvironmentRoot");
            for(int side=-1;side<=1;side+=2)
                Shape("EntryShutterTrack",env,new Vector3(side*3.45f,2.2f,19),new Vector3(.18f,4.4f,.36f),iron);
            Label("CELL BLOCK",env,new Vector3(0,4.17f,23),.12f,cream.color);
        }
    }
}

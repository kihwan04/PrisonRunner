using System;

namespace Muhanok.Domain
{
    // Logical lane/jump coordinates stay unchanged. These offsets place the continuous world surface.
    public static class RouteSurface
    {
        public const float Length=24f;
        private static readonly int[] route={0,1,2,3,7,4,5,6,11,12};
        public static int SequenceLength=>route.Length;
        public static int TypeAt(int sequence)=>route[Math.Max(0,sequence)%route.Length];
        public static bool IsCart(float z){int type=TypeAt((int)Math.Floor(Math.Max(0,z)/Length));return type==1||type==2;}
        public static float CartProgress(float z)=>IsCart(z)?Math.Max(0,Math.Min(1,(z%(Length*route.Length)-Length)/(Length*2))):0;
        // Negative is a missing left rail, positive is a missing right rail.
        public static int CartMissingSide(int type,float localZ)
        {
            if(type!=1&&type!=2)return 0;
            int row=Math.Abs(localZ-8)<=1.5f?0:Math.Abs(localZ-18)<=1.5f?1:-1;
            return row<0?0:((row+type)%2==0?1:-1);
        }
        public static float EntryHeight(int type)=>type>=4&&type<=6||type>=8&&type<=11?3f:0f;
        public static string AreaAt(float distance)
        {
            int type=TypeAt((int)Math.Floor(Math.Max(0,distance)/Length));
            return type==1||type==2?"광산 수레":type<4?"광산":type==7?"계단":type>=11?"사육장":"감옥 복도";
        }
        public static float Rise(int type)=>type==7?3f:type==11?-3f:0f;
        public static float LocalHeight(int type,float z)
        {
            float t=Math.Max(0,Math.Min(1,z/Length));
            return Rise(type)*t*t*(3-2*t);
        }
        public static float LocalX(int type,float z)
        {
            float amount=type==1?3.2f:type==2?-3.2f:type==11?.7f:type==12?-.7f:0;
            double wave=Math.Sin(Math.PI*Math.Max(0,Math.Min(1,z/Length)));
            return amount*(float)(wave*wave);
        }
        public static void Sample(float distance,out float x,out float y)
        {
            int sequence=Math.Max(0,(int)Math.Floor(distance/Length)); int type=TypeAt(sequence);
            float z=distance-sequence*Length;
            x=LocalX(type,z); y=EntryHeight(type)+LocalHeight(type,z);
        }
    }
}

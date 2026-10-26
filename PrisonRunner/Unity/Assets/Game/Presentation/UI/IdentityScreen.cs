using System;
using UnityEngine;
namespace Muhanok.Presentation
{
    public interface IIdentityData
    {
        bool IdentityBusy {get;}
        bool SignedIn {get;}
        string IdentityMessage {get;}
        Texture Portrait {get;}
        void FaceLogin(bool enroll);
        void FaceLogout(bool delete);
    }
    internal sealed class IdentityScreen
    {
        private readonly HudSkin skin; private bool deleteConfirm;
        private GUIStyle paragraph,notice;
        public IdentityScreen(HudSkin skin){this.skin=skin;}
        public void Draw(IHudData data,IIdentityData identity,Action close)
        {
            paragraph??=new GUIStyle(skin.Style(20,FontStyle.Normal)){wordWrap=true};
            notice??=new GUIStyle(skin.Style(17,FontStyle.Normal)){wordWrap=true};
            skin.Card(new Rect(0,0,1280,720),new Color(.025f,.035f,.06f,.99f));
            GUI.Label(new Rect(80,45,950,56),"나의 얼굴로 탈출 준비",skin.Style(38,FontStyle.Bold));
            PosePreview.Draw(skin,new Rect(70,130,560,400),data,true);
            GUI.Label(new Rect(680,145,510,160),@"연결된 얼굴 서버에 얼굴 특징과 얼굴 사진, 플레이 기록을 저장합니다.
최초 등록에 동의한 사람만 이용할 수 있습니다.
등록 후에는 이름 입력 없이 얼굴로 로그인합니다.",paragraph);
            GUI.Label(new Rect(680,305,510,90),"전시용 얼굴 로그인입니다. 사진·영상 위조를 차단하는 보안 인증 기능은 포함하지 않습니다.",notice);
            GUI.enabled=!identity.IdentityBusy;
            if(identity.SignedIn)
            {
                if(identity.Portrait!=null)GUI.DrawTexture(new Rect(685,405,90,90),identity.Portrait,ScaleMode.ScaleToFit);
                if(skin.FlatButton(new Rect(800,410,370,52),"로그아웃",false))identity.FaceLogout(false);
                if(skin.FlatButton(new Rect(680,515,490,48),deleteConfirm?"얼굴·프로필·기록 영구 삭제 확인":"내 얼굴과 기록 삭제",false))
                {if(deleteConfirm){identity.FaceLogout(true);deleteConfirm=false;}else deleteConfirm=true;}
            }
            else
            {
                deleteConfirm=false;
                if(skin.PrimaryButton(new Rect(680,405,490,58),"얼굴로 로그인"))identity.FaceLogin(false);
                if(skin.FlatButton(new Rect(680,485,490,58),"저장에 동의하고 얼굴 등록",false))identity.FaceLogin(true);
            }
            GUI.enabled=true;
            GUI.Label(new Rect(70,552,560,60),"손을 들어 버튼을 가리키고 0.9초 유지하면 선택됩니다.",paragraph);
            GUI.Label(new Rect(70,615,950,60),identity.IdentityMessage,skin.Style(18,FontStyle.Normal));
            GUI.enabled=!identity.IdentityBusy;
            if(skin.FlatButton(new Rect(1020,615,190,52),"돌아가기",false)){deleteConfirm=false;close();}
            GUI.enabled=true;
        }
    }
}

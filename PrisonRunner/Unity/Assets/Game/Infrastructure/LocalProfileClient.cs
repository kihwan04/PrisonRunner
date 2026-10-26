using System;
using System.Collections;
using System.IO;
using System.Text;
using Muhanok.Domain;
using UnityEngine;
using UnityEngine.Networking;
namespace Muhanok.Infrastructure
{
    [Serializable] public sealed class IdentityReply { public bool ok; public string id,name,avatar,token,message; }
    public sealed class LocalProfileClient : MonoBehaviour
    {
        private string endpoint;
        private string configurationError;
        private void Awake()
        {
            try { endpoint=IdentityServerConfig.Load(); }
            catch(Exception) { configurationError="얼굴 서버 주소 설정을 확인하세요. 원격 서버는 HTTPS 주소가 필요합니다.";Message=configurationError; }
        }
        public static class IdentityServerConfig
        {
            [Serializable] private sealed class Settings { public string url; }
            public static string Load()
            {
                string configured=Environment.GetEnvironmentVariable("MONGSCAPE_IDENTITY_URL");
                if(string.IsNullOrWhiteSpace(configured))
                {
                    string path=Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath,"../identity-server.json"));
                    if(File.Exists(path))
                    {
                        var settings=JsonUtility.FromJson<Settings>(File.ReadAllText(path));
                        if(settings==null||string.IsNullOrWhiteSpace(settings.url))throw new FormatException("Missing identity URL");
                        configured=settings.url;
                    }
                }
                return Normalize(string.IsNullOrWhiteSpace(configured)?"http://127.0.0.1:8765":configured);
            }
            public static string Normalize(string value)
            {
                if(!Uri.TryCreate(value?.Trim(),UriKind.Absolute,out var uri)||string.IsNullOrEmpty(uri.Host)
                    ||!string.IsNullOrEmpty(uri.UserInfo)||!string.IsNullOrEmpty(uri.Query)||!string.IsNullOrEmpty(uri.Fragment)
                    ||uri.AbsolutePath!="/"||(uri.Scheme!="https"&&!(uri.Scheme=="http"&&uri.IsLoopback)))
                    throw new FormatException("Use an HTTPS origin, or loopback HTTP for local Docker");
                return uri.GetLeftPart(UriPartial.Authority);
            }
        }
        [Serializable] private sealed class FaceRequest { public string name; public bool consent; public string[] images; }
        [Serializable] private sealed class RunRecord { public string id; public float distance; public int score; public double kcal,goal; public double[] activities; }
        public bool Busy {get;private set;}
        public string Message {get;private set;}="얼굴 등록 후 다음 방문부터 얼굴로 로그인하세요.";
        public IdentityReply Profile {get;private set;}
        public Texture2D Avatar {get;private set;}
        public event Action Changed;
        public void Authenticate(Texture camera,bool enroll,string name)
        {
            if(Busy)return;
            if(configurationError!=null){Message=configurationError;return;}
            if(!(camera is WebCamTexture webcam)||!webcam.isPlaying||webcam.width<64){Message="카메라가 준비된 뒤 다시 시도하세요.";return;}
            StartCoroutine(Capture(webcam,enroll,name));
        }
        private IEnumerator Capture(WebCamTexture camera,bool enroll,string name)
        {
            Busy=true; Message="정면을 보고 잠시 가만히 계세요. 얼굴 3장 확인 중…";
            var images=new string[3];
            for(int i=0;i<3;i++)
            {
                yield return new WaitForSecondsRealtime(.4f);
                if(camera==null||!camera.isPlaying||camera.width<64){Busy=false;Message="카메라가 중단되었습니다.";yield break;}
                var pixels=camera.GetPixels32(); int w=camera.width,h=camera.height;
                // WebCamTexture pixels start at the lower left. Correct the driver's rotation and vertical mirror before encoding.
                int rotation=camera.videoRotationAngle; bool swapped=rotation==90||rotation==270;
                int ow=swapped?h:w,oh=swapped?w:h;
                var corrected=new Color32[pixels.Length];
                for(int y=0;y<h;y++)for(int x=0;x<w;x++)
                {
                    int cy=camera.videoVerticallyMirrored?h-1-y:y,dx=x,dy=cy;
                    if(rotation==90){dx=cy;dy=w-1-x;}else if(rotation==180){dx=w-1-x;dy=h-1-cy;}else if(rotation==270){dx=h-1-cy;dy=x;}
                    corrected[dy*ow+dx]=pixels[y*w+x];
                }
                var texture=new Texture2D(ow,oh,TextureFormat.RGB24,false);texture.SetPixels32(corrected);texture.Apply();
                images[i]=Convert.ToBase64String(texture.EncodeToJPG(85));Destroy(texture);
            }
            yield return Send(enroll?"/enroll":"/login",JsonUtility.ToJson(new FaceRequest{name=name,consent=enroll,images=images}),"",reply=>
            {
                if(!reply.ok)return;
                Profile=reply;
                if(Avatar!=null)Destroy(Avatar);
                Avatar=new Texture2D(2,2,TextureFormat.RGB24,false);
                try { Avatar.LoadImage(Convert.FromBase64String(reply.avatar)); } catch { Destroy(Avatar);Avatar=null; }
                Message=reply.name+"님 로그인 완료"; Changed?.Invoke();
            });
            Busy=false;
        }
        private IEnumerator Send(string path,string json,string token,Action<IdentityReply> done)
        {
            if(configurationError!=null){done(new IdentityReply{message=configurationError});yield break;}
            using(var request=new UnityWebRequest(endpoint+path,"POST"))
            {
                request.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));request.downloadHandler=new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type","application/json"); if(!string.IsNullOrEmpty(token))request.SetRequestHeader("Authorization","Bearer "+token);
                request.timeout=30;yield return request.SendWebRequest();
                IdentityReply reply=null;
                try { reply=JsonUtility.FromJson<IdentityReply>(request.downloadHandler.text); } catch { }
                if(reply==null)reply=new IdentityReply{message="얼굴 서버 연결 실패. 서버 실행 상태와 주소 설정을 확인하세요."};
                if(!reply.ok) Message=reply.message;
                done?.Invoke(reply);
            }
        }
        public void SignOut(bool delete=false)
        {
            if(Busy)return;
            if(Profile!=null)StartCoroutine(EndSession(delete));else Clear();
        }
        private IEnumerator EndSession(bool delete)
        {
            Busy=true;bool success=false; string token=Profile.token;
            if(!delete)Clear();
            yield return Send(delete?"/delete":"/logout","{}",token,reply=>success=reply.ok);
            if(success || !delete) { Clear();Message=delete?"얼굴과 프로필·기록을 삭제했습니다.":"로그아웃했습니다."; }
            Busy=false;
        }
        private void Clear(){Profile=null;if(Avatar!=null)Destroy(Avatar);Avatar=null;Changed?.Invoke();}
        public void SaveRun(RunSession run)
        {
            if(Profile==null)return;
            var record=new RunRecord{id=Guid.NewGuid().ToString(),distance=run.Score.Distance,score=run.Score.Score,kcal=run.Workout.EstimatedKcal,goal=run.Workout.GoalKcal,activities=new double[5]};
            for(int i=0;i<5;i++)record.activities[i]=run.Workout.KcalFor(i);
            StartCoroutine(Send("/runs",JsonUtility.ToJson(record),Profile.token,reply=>{if(!reply.ok)Debug.LogWarning("Mongscape: local run save failed: "+reply.message);}));
        }
        private void OnDestroy(){if(Avatar!=null)Destroy(Avatar);}
    }
}

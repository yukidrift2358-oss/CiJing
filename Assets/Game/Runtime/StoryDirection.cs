using UnityEngine;
using UnityEngine.UI;
namespace CiJing {
public sealed class StoryDirection:MonoBehaviour {
 public Text detailTitle;public AtmosphereGraphic ambience;public RectTransform landscape;public RawImage prop;public InkPainting painting;public bool Reduced;
 int chapter,cue;float elapsed;Vector2 target,home;float zoom;Material ink;
 static readonly string[][] Titles={new[]{"笺上旧字","一段桨痕","归舟灯"},new[]{"檐下余雨","窗前折枝","落花入笺"},new[]{"月下孤灯","水上传书","一封回信"}};
 static readonly string[][] Items={new[]{"petalletter","oar","boatlantern"},new[]{"rainbowl","blossombranch","petalletter"},new[]{"moonlantern","streamletter","replyletter"}};
 public void Initialize(int c,bool reduced){chapter=c;Reduced=reduced;ink=new Material(Resources.Load<Shader>("PropInk"));prop.material=ink;home=prop.rectTransform.anchoredPosition;Cue(0);}
 public void Cue(int page){cue=page;elapsed=0;if(detailTitle)detailTitle.text="卷中拾影 · "+Titles[chapter][page%3];target=new Vector2(-30-page*13,-12+chapter*8);zoom=1.035f+page*.012f;prop.texture=Resources.Load<Texture2D>("PropsInk/"+Items[chapter][page%3]);var aspect=(float)prop.texture.width/prop.texture.height;prop.rectTransform.sizeDelta=aspect>1?new Vector2(170,170/aspect):new Vector2(170*aspect,170);home=new Vector2(475-prop.rectTransform.sizeDelta.x*.5f,-(405-prop.rectTransform.sizeDelta.y*.5f));painting.Pulse(.38f+page*.13f,.5f);if(Reduced)Snap();}
 void Snap(){landscape.localScale=Vector3.one*zoom;landscape.anchoredPosition=target;prop.rectTransform.anchoredPosition=home;prop.color=Color.white;}
 public void SetReduced(bool value){Reduced=value;if(ambience)ambience.enabled=!value;painting.Motion=!value;if(value)Snap();}
 void Update(){if(Reduced)return;elapsed+=FilmClock.Delta;float t=1-Mathf.Exp(-FilmClock.Delta*.55f);landscape.localScale=Vector3.Lerp(landscape.localScale,Vector3.one*zoom,t);landscape.anchoredPosition=Vector2.Lerp(landscape.anchoredPosition,target,t);prop.rectTransform.anchoredPosition=home+new Vector2(Mathf.Sin(elapsed*.35f+chapter)*4,Mathf.Sin(elapsed*.65f)*2);prop.color=new Color(1,1,1,Mathf.SmoothStep(0,1,elapsed/.7f));}
 void OnDestroy(){if(ink)Destroy(ink);}
}
}

using UnityEngine;
using UnityEngine.UI;
namespace CiJing {
// Original brush geometry for environmental movement; illustration assets remain
// separate, so a curtain lift cannot distort the painted garden underneath.
public sealed class ScoreSceneView:MaskableGraphic {
 public SceneMoment Moment;public int Chapter;public bool GateGarden;float clock;public Material GardenInk,CurtainInk;
 public RectTransform Curtain,Branch,Letter,Boat;public CanvasGroup LetterInk,ReflectionInk;
 SceneMoment shown;
 public void Set(SceneMoment value,bool snap=false){Moment=value;if(snap)shown=value;SetVerticesDirty();}
 void Update(){clock+=FilmClock.Delta;float k=1-Mathf.Exp(-FilmClock.Delta*4);shown.rain=Mathf.Lerp(shown.rain,Moment.rain,k);shown.wind=Mathf.Lerp(shown.wind,Moment.wind,k);shown.ripple=Mathf.Lerp(shown.ripple,Moment.ripple,k);shown.curtain=Mathf.Lerp(shown.curtain,Moment.curtain,k);shown.boat=Mathf.Lerp(shown.boat,Moment.boat,k);shown.petals=Moment.petals;shown.reflection=Moment.reflection;
  if(GardenInk){GardenInk.SetFloat("_Loss",Mathf.Clamp01(shown.petals*.5f));GardenInk.SetFloat("_Reveal",GateGarden?Mathf.SmoothStep(0,1,Mathf.InverseLerp(.35f,.9f,shown.curtain)):1);}
  if(ReflectionInk)ReflectionInk.alpha=Mathf.MoveTowards(ReflectionInk.alpha,Moment.reflection?.42f:0,FilmClock.Delta*.8f);
  if(Curtain){Curtain.localScale=new Vector3(1,1-shown.curtain*.76f,1);Curtain.localEulerAngles=new Vector3(0,0,Mathf.Sin(clock*1.7f)*shown.wind*1.4f);}
  if(Branch)Branch.localEulerAngles=new Vector3(0,0,Mathf.Sin(clock*2)*shown.wind*2);
  if(LetterInk){bool readable=Moment.letter&&Moment.loosePaper<.4f;LetterInk.alpha=Mathf.MoveTowards(LetterInk.alpha,readable?1:0,FilmClock.Delta*1.8f);LetterInk.interactable=LetterInk.blocksRaycasts=readable;Letter.anchoredPosition=Vector2.Lerp(Letter.anchoredPosition,new Vector2(715-Moment.loosePaper*110,-92+Moment.loosePaper*18),k);}
  if(Boat)Boat.anchoredPosition=new Vector2(70+Mathf.Clamp01(shown.boat)*690,-175+Mathf.Sin(clock)*2);
  SetVerticesDirty();
 }
 protected override void OnDestroy(){if(GardenInk)Destroy(GardenInk);if(CurtainInk)Destroy(CurtainInk);base.OnDestroy();}
 protected override void OnPopulateMesh(VertexHelper v){v.Clear();float w=rectTransform.rect.width,h=rectTransform.rect.height;var c=new Color(.20f,.30f,.29f,.2f);
  for(int i=0;i<38;i++){float x=(i*131.7f)%w;float y=(clock*(85+i%5*12)+i*67)%h;var rain=c;rain.a=Mathf.Clamp01(shown.rain)*.32f;Line(v,new Vector2(x,-y),new Vector2(x-6,-y-20),.7f,rain);}
  for(int ring=0;ring<4;ring++){float t=(clock*.7f+ring*.25f)%1;float radius=18+t*100;float cx=160+ring*190;var col=c;col.a=shown.ripple*(1-t)*.28f;for(int i=0;i<30;i++){float a=i*Mathf.PI*2/30,b=(i+1)*Mathf.PI*2/30;Line(v,new Vector2(cx+Mathf.Cos(a)*radius,-(h-44)+Mathf.Sin(a)*radius*.12f),new Vector2(cx+Mathf.Cos(b)*radius,-(h-44)+Mathf.Sin(b)*radius*.12f),.9f,col);}}
  for(int i=0;i<Mathf.Min(shown.petals,10);i++){float t=Mathf.Clamp01(clock%6/3);float x=650-i*43+Mathf.Sin(clock+i)*8*shown.wind;float y=150+20*(i%3);var p=new Color(.61f,.30f,.32f,.5f);Line(v,new Vector2(x,-y),new Vector2(x+5,-y-2),3,p);}
  if(shown.reflection){for(int i=0;i<8;i++){float x=660+i*7,y=h-60+i*1.5f;Line(v,new Vector2(x,-y),new Vector2(x+24-i,-y),1,new Color(.36f,.43f,.35f,.4f));}}
  for(int i=0;i<Mathf.Min(Moment.birds,5);i++){float x=110+i*85+(clock*12)%160,y=70+i%2*22;float flap=Mathf.Sin(clock*3+i)*6;Line(v,new Vector2(x-10,-y-flap),new Vector2(x,-y),1,c);Line(v,new Vector2(x,-y),new Vector2(x+10,-y-flap),1,c);}
 }
 static void Line(VertexHelper v,Vector2 a,Vector2 b,float width,Color c){if(c.a<.001f)return;Vector2 n=new Vector2(-(b-a).y,(b-a).x).normalized*width;int at=v.currentVertCount;v.AddVert(a-n,c,Vector2.zero);v.AddVert(a+n,c,Vector2.zero);v.AddVert(b+n,c,Vector2.zero);v.AddVert(b-n,c,Vector2.zero);v.AddTriangle(at,at+1,at+2);v.AddTriangle(at,at+2,at+3);}
}
}

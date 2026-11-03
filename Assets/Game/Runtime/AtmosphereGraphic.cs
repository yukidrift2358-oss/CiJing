using UnityEngine;
using UnityEngine.UI;
namespace CiJing {
public sealed class AtmosphereGraphic:MaskableGraphic {
 public GameApp app;public int chapter;
 void Update(){SetVerticesDirty();}
 protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();float t=Time.unscaledTime;
 if(chapter==0){for(int i=0;i<7;i++){float x=130+i*85+Mathf.Sin(t*.2f+i)*18,y=190+i%3*14;float wing=Mathf.Sin(t*3+i)*5;Line(vh,new Vector2(x-11,y+wing),new Vector2(x,y),new Color(.25f,.35f,.32f,.5f),1.6f);Line(vh,new Vector2(x,y),new Vector2(x+11,y+wing),new Color(.25f,.35f,.32f,.5f),1.6f);}}
 if(chapter==1){for(int i=0;i<35;i++){float x=(i*71+t*23)%980,y=310-(i*43+t*55)%300;Line(vh,new Vector2(x,y),new Vector2(x+4,y+13),new Color(.43f,.60f,.59f,app.playing?.22f:.07f),1); }for(int i=0;i<9;i++){float x=750+i*21+Mathf.Sin(t*.7f+i)*35,y=300-(t*12+i*32)%300;Line(vh,new Vector2(x,y),new Vector2(x+5,y+2),new Color(.73f,.43f,.42f,.45f),3);}}
 if(chapter==2){for(int i=0;i<16;i++){float y=15+i*9,x=490+Mathf.Sin(t*.4f+i)*40;Line(vh,new Vector2(x-i*3,y),new Vector2(x+35+i*3,y),new Color(.96f,.93f,.73f,.13f+Mathf.Sin(t+i)*.06f),1.6f);}}
 }
 void Line(VertexHelper v,Vector2 a,Vector2 b,Color c,float width){a.y-=310;b.y-=310;Vector2 n=new Vector2(-(b-a).y,(b-a).x).normalized*width*.5f;int k=v.currentVertCount;v.AddVert(a-n,c,Vector2.zero);v.AddVert(a+n,c,Vector2.zero);v.AddVert(b+n,c,Vector2.zero);v.AddVert(b-n,c,Vector2.zero);v.AddTriangle(k,k+1,k+2);v.AddTriangle(k,k+2,k+3);}
}
}

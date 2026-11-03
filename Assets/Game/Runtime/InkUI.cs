using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace CiJing {
static class InkMesh {
 public static void Triangle(VertexHelper v,Vector2 a,Vector2 b,Vector2 c,Color color){int k=v.currentVertCount;v.AddVert(a,color,Vector2.zero);v.AddVert(b,color,Vector2.zero);v.AddVert(c,color,Vector2.zero);v.AddTriangle(k,k+1,k+2);}
 public static void Line(VertexHelper v,Vector2 a,Vector2 b,float width,Color color){Vector2 n=new Vector2(a.y-b.y,b.x-a.x).normalized*width*.5f;Triangle(v,a-n,a+n,b+n,color);Triangle(v,a-n,b+n,b-n,color);}
 public static Vector2 Ring(Vector2 center,float rx,float ry,int i,int count,float seed){float a=i*Mathf.PI*2/count,r=1+.018f*Mathf.Sin(a*5+seed)+.013f*Mathf.Cos(a*9-seed);return center+new Vector2(Mathf.Cos(a)*rx*r,Mathf.Sin(a)*ry*r);}
}
public sealed class InkButtonGraphic:MaskableGraphic {
 public bool primary;public float Focus,Press;public bool Disabled;
 protected override void OnPopulateMesh(VertexHelper v){
  v.Clear();float w=rectTransform.rect.width,h=rectTransform.rect.height;
  Color pigment=primary?new Color(.57f,.20f,.13f):new Color(.22f,.30f,.26f);
  if(Disabled)pigment=new Color(.46f,.45f,.40f);else pigment=Color.Lerp(pigment,new Color(.35f,.12f,.08f),Press*.55f);
  // Open calligraphic controls: no rectangular fill or enclosing border.
  Color wash=pigment;wash.a=Disabled?0:Focus*.065f+Press*.05f;
  Vector2 center=new Vector2(w*.5f,-h*.5f);
  for(int i=0;i<48;i++)InkMesh.Triangle(v,center,InkMesh.Ring(center,w*.43f,h*.39f,i,48,2),InkMesh.Ring(center,w*.43f,h*.39f,i+1,48,2),wash);
  Color stroke=pigment;stroke.a=Disabled?.15f:(primary?.65f:.18f+Focus*.5f);
  float length=w*(primary?.42f:.28f);
  for(int i=0;i<20;i++){
   float t=i/20f,u=(i+1)/20f;
   InkMesh.Line(v,new Vector2((w-length)*.5f+t*length,-h+7+Mathf.Sin(t*8)*.8f),new Vector2((w-length)*.5f+u*length,-h+7+Mathf.Sin(u*8)*.8f),.3f+Mathf.Sin(t*Mathf.PI)*(primary?1.8f:.8f),stroke);
  }
  if(primary){
   Vector2 seal=new Vector2(13,-h*.5f);Color stamp=pigment;stamp.a=Disabled?.12f:.65f;
   for(int i=0;i<24;i++)InkMesh.Line(v,InkMesh.Ring(seal,3.5f,4.5f,i,24,1),InkMesh.Ring(seal,3.5f,4.5f,i+1,24,1),1.2f,stamp);
  }

 }
 public void Refresh(){SetVerticesDirty();}
}
public sealed class InkButtonFeedback:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerUpHandler,ISelectHandler,IDeselectHandler {
 public UnityEngine.UI.Button button;public InkButtonGraphic brush;public UnityEngine.UI.Text title;public bool primary;
 bool hover,pressed,focus;float value,press;bool disabled;
 public void OnPointerEnter(PointerEventData e){hover=true;}
 public void OnPointerExit(PointerEventData e){hover=focus=pressed=false;}
 public void OnPointerDown(PointerEventData e){if(e.button==PointerEventData.InputButton.Left)pressed=true;}
 public void OnPointerUp(PointerEventData e){pressed=false;}
 public void OnSelect(BaseEventData e){focus=true;}
 public void OnDeselect(BaseEventData e){focus=false;}
 void Update(){
  bool off=!button.IsInteractable();float target=off?0:(hover||focus?1:0),targetPress=off?0:(pressed?1:0);
  float next=Mathf.MoveTowards(value,target,Time.unscaledDeltaTime*9),nextPress=Mathf.MoveTowards(press,targetPress,Time.unscaledDeltaTime*14);
  if(next==value&&nextPress==press&&off==disabled)return;
  value=next;press=nextPress;disabled=off;brush.Focus=value;brush.Press=press;brush.Disabled=off;brush.Refresh();
  title.color=off?new Color(.47f,.47f,.41f):(primary?Color.Lerp(new Color(.58f,.22f,.15f),new Color(.36f,.12f,.08f),Mathf.Max(value*.45f,press)):Color.Lerp(new Color(.16f,.25f,.24f),new Color(.55f,.20f,.14f),value));
 }
 void OnDisable(){hover=pressed=focus=false;}
}
public sealed class InkStoneGraphic:MaskableGraphic {
 bool empty=true,selected,active;int tone,rank;Color pigment;
 public void Set(Stone stone,bool chosen,bool sounding,Color tint){if(empty==stone.Empty&&selected==chosen&&active==sounding&&tone==stone.tone&&rank==stone.rank)return;empty=stone.Empty;selected=chosen;active=sounding;tone=stone.tone;rank=stone.rank;pigment=tint;SetVerticesDirty();}
 protected override void OnPopulateMesh(VertexHelper v){
  v.Clear();float w=rectTransform.rect.width,h=rectTransform.rect.height;Vector2 center=new Vector2(w*.5f,-h*.5f);
  Color edge=selected||active?new Color(.62f,.24f,.16f,.85f):new Color(.23f,.32f,.28f,empty?.16f:.5f);
  Color fill=Color.Lerp(new Color(.96f,.945f,.88f),pigment,.22f);fill.a=empty?0:.82f;
  for(int i=0;i<64;i++){
   Vector2 a=InkMesh.Ring(center,w*.41f,h*.39f,i,64,tone),b=InkMesh.Ring(center,w*.41f,h*.39f,i+1,64,tone);
   if(!empty)InkMesh.Triangle(v,center,a,b,fill);
   if(!empty||i%16<10)InkMesh.Line(v,a,b,empty?.7f:1.05f,edge);
   if(selected||active){Color halo=edge;halo.a=.22f;InkMesh.Line(v,InkMesh.Ring(center,w*.46f,h*.44f,i,64,tone),InkMesh.Ring(center,w*.46f,h*.44f,i+1,64,tone),1,halo);}
  }
  if(!empty){Color mark=pigment;mark.a=.7f;InkMesh.Line(v,new Vector2(w*.38f,-h*.84f),new Vector2(w*.61f,-h*.84f),1.2f,mark);if(rank==2)InkMesh.Line(v,new Vector2(w*.42f,-h*.89f),new Vector2(w*.58f,-h*.89f),.8f,mark);}
 }
}
}

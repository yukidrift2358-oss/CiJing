using UnityEngine;
using UnityEngine.UI;
namespace CiJing {
public sealed class InkBorder:MaskableGraphic {
 protected override void OnPopulateMesh(VertexHelper v){v.Clear();float w=rectTransform.rect.width,h=rectTransform.rect.height;Line(v,new Vector2(w*.16f,-h+10),new Vector2(w*.40f,-h+9),.6f);Line(v,new Vector2(w*.42f,-h+10),new Vector2(w*.49f,-h+10),.3f);}

 void Line(VertexHelper v,Vector2 a,Vector2 b,float width){Vector2 n=new Vector2(-(b-a).y,(b-a).x).normalized*width;int k=v.currentVertCount;v.AddVert(a-n,color,Vector2.zero);v.AddVert(a+n,color,Vector2.zero);v.AddVert(b+n*.6f,color,Vector2.zero);v.AddVert(b-n*.6f,color,Vector2.zero);v.AddTriangle(k,k+1,k+2);v.AddTriangle(k,k+2,k+3);}
}
}

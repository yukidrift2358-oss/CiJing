using UnityEngine;
using UnityEngine.UI;
namespace CiJing {
public sealed class InkBorder:MaskableGraphic {
 protected override void OnPopulateMesh(VertexHelper v){v.Clear();float w=rectTransform.rect.width,h=rectTransform.rect.height;Line(v,new Vector2(3,-1),new Vector2(w-5,-2),.8f);Line(v,new Vector2(w-2,-4),new Vector2(w-1,-h+6),.65f);Line(v,new Vector2(w-5,-h+2),new Vector2(2,-h+1),1.1f);Line(v,new Vector2(1,-h+4),new Vector2(2,-4),.6f);}
 void Line(VertexHelper v,Vector2 a,Vector2 b,float width){Vector2 n=new Vector2(-(b-a).y,(b-a).x).normalized*width;int k=v.currentVertCount;v.AddVert(a-n,color,Vector2.zero);v.AddVert(a+n,color,Vector2.zero);v.AddVert(b+n*.6f,color,Vector2.zero);v.AddVert(b-n*.6f,color,Vector2.zero);v.AddTriangle(k,k+1,k+2);v.AddTriangle(k,k+2,k+3);}
}
}

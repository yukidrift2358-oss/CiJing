using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace CiJing {
public sealed class PropArtwork:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {
 public RectTransform art;public RawImage image;public CanvasGroup annotation;public Text badge;
 public bool Seen;public bool Focused{get{return hover;}}
 bool hover;Material ink;
 public void Initialize(){
  var shader=Resources.Load<Shader>("PropInk");
  if(!shader)throw new System.Exception("PropInk shader missing");
  ink=new Material(shader);image.material=ink;
  annotation.alpha=0;annotation.blocksRaycasts=false;annotation.interactable=false;
 }
 public void OnPointerEnter(PointerEventData e){hover=true;}
 public void OnPointerExit(PointerEventData e){hover=false;}
 void Update(){
  annotation.alpha=Mathf.MoveTowards(annotation.alpha,hover?1:0,Time.unscaledDeltaTime*7);
  annotation.blocksRaycasts=hover;annotation.interactable=hover;
  if(ink)ink.SetFloat("_Focus",annotation.alpha);
  if(badge)badge.text=Seen?"已收入词笺 · 点击重看":"点击查看 · 寻找词句";
 }
 void OnDisable(){hover=false;if(annotation)annotation.alpha=0;}
 void OnDestroy(){if(ink)Destroy(ink);}
}
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace CiJing {
public sealed class PropArtwork:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {
 public RectTransform art;public RawImage image;public bool Seen;public Text badge;bool hover;float initialY;Vector3 baseScale;
 void Start(){initialY=art.anchoredPosition.y;baseScale=art.localScale;}
 public void OnPointerEnter(PointerEventData e){hover=true;}public void OnPointerExit(PointerEventData e){hover=false;}
 void Update(){if(!art)return;float target=hover?1.055f:1;art.localScale=Vector3.Lerp(art.localScale,baseScale*target,Time.unscaledDeltaTime*10);image.color=Color.Lerp(image.color,hover?new Color(1,.98f,.91f,1):Color.white,Time.unscaledDeltaTime*8);if(badge)badge.text=Seen?"词笺已收入":"点击物件 · 寻迹";}
}
}

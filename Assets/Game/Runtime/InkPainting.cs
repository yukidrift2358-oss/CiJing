using UnityEngine;
using UnityEngine.UI;
namespace CiJing {
[RequireComponent(typeof(RawImage))]
public sealed class InkPainting:MonoBehaviour {
 Material ink;public float Current{get;private set;}public float Target{get;private set;}public bool Motion=true;
 public void Initialize(float growth){var image=GetComponent<RawImage>();var shader=Resources.Load<Shader>("InkGrowth");if(shader==null)throw new System.Exception("InkGrowth shader missing");ink=new Material(shader);image.texture=Resources.Load<Texture2D>("Painting/stage0");ink.SetTexture("_LineTex",Resources.Load<Texture2D>("Painting/stage1"));ink.SetTexture("_ColorTex",Resources.Load<Texture2D>("Painting/stage2"));image.material=ink;Current=Target=Mathf.Clamp01(growth);ink.SetFloat("_Growth",Current);}
 public void Grow(float target){Target=Mathf.Max(Target,Mathf.Clamp01(target));Pulse(.46f,.52f);}
 public void Pulse(float x,float y){if(ink)ink.SetVector("_InkPulse",new Vector4(x,y,Time.time,1));}
 void Update(){if(!ink)return;Current=Mathf.MoveTowards(Current,Target,FilmClock.Delta*.09f);ink.SetFloat("_Growth",Current);ink.SetFloat("_Motion",Motion?1:0);}
 public void Snap(float value){Current=Target=Mathf.Clamp01(value);if(ink)ink.SetFloat("_Growth",Current);}
 void OnDestroy(){if(ink)Destroy(ink);}
}
}

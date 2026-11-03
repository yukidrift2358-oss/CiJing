using UnityEngine;
namespace CiJing {
public sealed class MusicBed:MonoBehaviour {
 AudioSource source;public float Volume=.35f;public bool Duck;int chapter=-1;
 void Awake(){source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.loop=true;}
 public void Chapter(int value){value=Mathf.Clamp(value,0,2);if(chapter==value&&source.clip)return;chapter=value;source.clip=Resources.Load<AudioClip>("Audio/music"+value);if(!source.clip)throw new System.Exception("Music sample missing");source.volume=0;source.Play();}
 void Update(){source.volume=Mathf.MoveTowards(source.volume,Volume*(Duck?.16f:1),Time.unscaledDeltaTime*.5f);}
 public bool Active{get{return source&&source.isPlaying;}}
}
}

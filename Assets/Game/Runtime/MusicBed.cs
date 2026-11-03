using UnityEngine;
namespace CiJing {
public sealed class MusicBed:MonoBehaviour {
 AudioSource source,reply;float responseTime=-10;AudioClip flute;public float Volume=.35f;public bool Duck;int chapter=-1;
 void Awake(){source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.loop=true;reply=gameObject.AddComponent<AudioSource>();reply.playOnAwake=false;flute=Resources.Load<AudioClip>("Audio/flute");}
 public void Chapter(int value){value=Mathf.Clamp(value,0,2);if(chapter==value&&source.clip)return;chapter=value;source.clip=Resources.Load<AudioClip>("Audio/music"+value);if(!source.clip)throw new System.Exception("Music sample missing");source.volume=0;source.Play();}
 void Update(){source.volume=Mathf.MoveTowards(source.volume,Mathf.Clamp01(Volume*1.35f)*(Duck?.48f:1),Time.unscaledDeltaTime*.5f);}
 public void Respond(int tone,int rank){if(!flute||Time.unscaledTime-responseTime<1.25f)return;responseTime=Time.unscaledTime;reply.clip=flute;reply.pitch=Mathf.Pow(2,new[]{0,2,4,7,9}[tone]/12f);reply.volume=Volume*(rank==2?.22f:.12f);reply.Play();}
 public bool Active{get{return source&&source.isPlaying;}}
}
}

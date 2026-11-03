using UnityEngine;
namespace CiJing {
public sealed class MusicBed:MonoBehaviour {
 AudioSource source,reply,ornament,rain,wind,coda;float responseTime=-10;AudioClip flute;AudioClip[] beds=new AudioClip[3],details=new AudioClip[3],codas=new AudioClip[4];public float Volume=.35f;public bool Duck;int chapter=-1;SceneMoment environment;
 AudioSource Loop(string name){var a=gameObject.AddComponent<AudioSource>();a.playOnAwake=false;a.loop=true;a.volume=0;a.clip=Resources.Load<AudioClip>("Audio/"+name);return a;}
 void Awake(){source=Loop("score0");ornament=Loop("ornament0");rain=Loop("scene-rain");wind=Loop("scene-wind");reply=gameObject.AddComponent<AudioSource>();reply.playOnAwake=false;coda=gameObject.AddComponent<AudioSource>();coda.playOnAwake=false;flute=Resources.Load<AudioClip>("Audio/flute");for(int i=0;i<3;i++){beds[i]=Resources.Load<AudioClip>("Audio/score"+i);details[i]=Resources.Load<AudioClip>("Audio/ornament"+i);}for(int i=0;i<4;i++)codas[i]=Resources.Load<AudioClip>("Audio/coda"+i);rain.Play();wind.Play();}
 public void Chapter(int value){value=Mathf.Clamp(value,0,2);if(chapter==value&&source.isPlaying)return;chapter=value;if(!beds[value]||!details[value])throw new System.Exception("Adaptive score missing");source.clip=beds[value];ornament.clip=details[value];source.volume=ornament.volume=0;double at=AudioSettings.dspTime+.08;source.PlayScheduled(at);ornament.PlayScheduled(at);}
 public void Environment(SceneMoment state){environment=state;}
 void Update(){float dt=FilmClock.Delta;bool under=Duck||coda.isPlaying;source.volume=Mathf.MoveTowards(source.volume,Volume*(under?.33f:.82f),dt*.7f);ornament.volume=Mathf.MoveTowards(ornament.volume,Volume*(under?.08f:.26f)*(environment.reflection?1.3f:1),dt*.5f);rain.volume=Mathf.MoveTowards(rain.volume,Volume*Mathf.Clamp01(environment.rain)*.42f,dt*.9f);wind.volume=Mathf.MoveTowards(wind.volume,Volume*Mathf.Clamp01(environment.wind)*.32f,dt*.9f);coda.volume=Volume*.65f;}
 public void Cadence(int chapter,bool flower){StopResponse();int n=chapter==0?0:chapter==2?3:flower?1:2;if(!codas[n])throw new System.Exception("Cadence resource missing "+n);coda.clip=codas[n];coda.volume=Volume*.65f;coda.Play();}
 public bool CadenceActive{get{return coda&&coda.isPlaying;}}
 public void Respond(int tone,int rank){if(!flute||Time.unscaledTime-responseTime<1.6f)return;responseTime=Time.unscaledTime;reply.clip=flute;reply.pitch=Mathf.Pow(2,new[]{0,2,4,7,9}[tone]/12f);reply.volume=Volume*(rank==2?.13f:.075f);reply.panStereo=.35f;reply.Play();}
 public void StopResponse(){if(reply)reply.Stop();responseTime=-10;}
 public bool ResponseActive{get{return reply&&reply.isPlaying;}}
 public bool Active{get{return source&&source.isPlaying;}}
}
}

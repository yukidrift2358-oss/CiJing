using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace CiJing {
public sealed partial class GameApp {
 PropArtwork[] propViews;
 static readonly string[][] PropAssets={new[]{"oar","waterstone","boatlantern"},new[]{"rainbowl","blossombranch","petalletter"},new[]{"moonlantern","streamletter","replyletter"}};
 ScrollRect exploration;Button[] exploreButtons;Text exploreText,exploreProgress;
 static readonly string[][] PropNames={new[]{"船桨","岸边水痕","归舟灯"},new[]{"檐下水钵","折枝","落花书笺"},new[]{"西楼灯","溪流书笺","对岸回信"}};
 static readonly string[][] PoemClues={new[]{"兴尽晚回舟","误入藕花深处","争渡，争渡"},new[]{"昨夜雨疏风骤","却道海棠依旧","应是绿肥红瘦"},new[]{"月满西楼","花自飘零水自流","一种相思，两处闲愁"}};
 string Mission(){return new[]{"循桨痕，寻归舟。","循檐声，卷帘见花。","隔岸寻音，接一封回书。"}[Content.Levels[level].chapter];}
 bool ExplorationReady(){return freeMode||save.evidence[level]!=0;}
 string QuestSummary(){int count=Enumerable.Range(0,3).Count(i=>(save.evidence[level]&(1<<i))!=0);return Mission()+"\n已读 "+count+" 处旧痕\n"+(count==0?"读一处景物，即可试音。":"可归水纹试音，余笺随兴探寻。")+(save.sceneDiscoveries[level]!=0?"\n画中有新迹，可展读画记。":"\n风雨有声，留白处亦有变化。");}
 void OpenExploration(){if(freeMode){Hint();return;}if(completing)return;if(modal)CloseModal();playing=false;Silence();var sheet=Panel(page,0,0,1600,900,paper);sheet.raycastTarget=true;modal=sheet.rectTransform;Label(modal,"寻 迹 · "+Content.Chapters[Content.Levels[level].chapter],60,28,1050,60,34);Label(modal,Mission(),60,94,1100,40,24);Button(modal,"回到音石",1310,40,230,55,CloseModal,true);
 var view=Rect("Horizontal journey",modal,60,150,1480,420);exploration=view.gameObject.AddComponent<ScrollRect>();exploration.horizontal=true;exploration.vertical=false;exploration.movementType=ScrollRect.MovementType.Clamped;exploration.inertia=true;var viewport=Rect("Viewport",view,0,0,1480,420);viewport.gameObject.AddComponent<RectMask2D>();var capture=viewport.gameObject.AddComponent<Image>();capture.color=paper;capture.raycastTarget=true;var world=Rect("Scroll landscape",viewport,0,0,3000,420);exploration.viewport=viewport;exploration.content=world;var painted=Rect("Panorama",world,0,-620,3000,1688).gameObject.AddComponent<RawImage>();painted.texture=Resources.Load<Texture2D>("Painting/stage"+(save.unlocked>=6?2:1));painted.color=new Color(1,1,1,.7f);painted.raycastTarget=false;exploreButtons=new Button[3];propViews=new PropArtwork[3];
 for(int i=0;i<3;i++){
  int prop=i;float x=200+i*1120;bool known=(save.evidence[level]&(1<<i))!=0;
  var touch=Panel(world,x-55,115,240,285,new Color(1,1,1,0));touch.raycastTarget=true;
  var inspect=touch.gameObject.AddComponent<Button>();inspect.transition=Selectable.Transition.None;inspect.onClick.AddListener(()=>ObserveProp(prop));
  var art=Rect("Ink item "+PropAssets[Content.Levels[level].chapter][i],touch.transform,30,5,180,180);
  var image=art.gameObject.AddComponent<RawImage>();image.texture=Resources.Load<Texture2D>("PropsInk/"+PropAssets[Content.Levels[level].chapter][i]);
  if(!image.texture)throw new Exception("Item artwork missing: "+PropAssets[Content.Levels[level].chapter][i]);
  image.raycastTarget=false;
  float aspect=(float)image.texture.width/image.texture.height;
  float width=aspect>=1?180:180*aspect,height=aspect>=1?180/aspect:180;
  bool suspended=PropAssets[Content.Levels[level].chapter][i].EndsWith("lantern");
  float centerY=suspended?95:(i<2?235:195);
  art.sizeDelta=new Vector2(width,height);art.anchoredPosition=new Vector2(120-width*.5f,-(centerY-height*.5f));
  var behavior=touch.gameObject.AddComponent<PropArtwork>();behavior.art=art;behavior.image=image;behavior.Seen=known;propViews[i]=behavior;
  var note=Rect("Hover annotation",touch.transform,0,suspended?205:45,240,76);behavior.annotation=note.gameObject.AddComponent<CanvasGroup>();
  Panel(note,0,0,240,76,paper);
  exploreButtons[i]=Button(note,PropNames[Content.Levels[level].chapter][i],0,0,240,43,()=>ObserveProp(prop));
  exploreButtons[i].transition=Selectable.Transition.None;

  behavior.badge=Label(note,"",0,43,240,33,18,accent,TextAnchor.MiddleCenter);behavior.badge.raycastTarget=false;
  behavior.Initialize();
 }


 var driver=view.gameObject.AddComponent<ScrollJourneyInput>();driver.scroll=exploration;Label(modal,"拖动画卷 / A D 移动 · 鼠标移近景物寻迹",60,585,1000,32,21);Button(modal,"向左",1140,580,170,42,()=>ScrollBy(-.45f));Button(modal,"向右",1340,580,170,42,()=>ScrollBy(.45f));Panel(modal,60,635,1480,185,paper);InkEdge(modal,0,0,Color.clear);exploreText=Label(modal,"桨边、灯下、散笺间，皆有词痕。移近景物，待题签浮现便可点读；拖动画卷，往右续寻。",85,640,1420,125,25);exploreProgress=Label(modal,QuestSummary(),85,820,1150,66,20);Button(modal,"线索册",1290,824,220,48,ShowEvidenceBook);}
 void ScrollBy(float direction){if(exploration){exploration.velocity=Vector2.zero;exploration.horizontalNormalizedPosition=Mathf.Clamp01(exploration.horizontalNormalizedPosition+direction);}}
 void ObserveProp(int index){if(!exploration)return;save.evidence[level]|=1<<index;if(propViews!=null&&propViews[index])propViews[index].Seen=true;Persist();var facts=level<3?new[]{Content.Levels[level].clue,"水面上的短印与长痕，分别对应短音与长音。",Content.Levels[level].hint}:VerseRules.Evidence(level);string fact=facts[Mathf.Min(index,facts.Length-1)];exploreText.text="“"+PoemClues[Content.Levels[level].chapter][index]+"”\n"+PropNames[Content.Levels[level].chapter][index]+"："+fact;exploreProgress.text="已收入 "+Enumerable.Range(0,3).Count(i=>(save.evidence[level]&(1<<i))!=0)+" / 3 处线索。"+(ExplorationReady()?"已可试音；余笺可慢慢寻。":"往右续寻，尚有遗笺。");StartCoroutine(PlayClue(index));if(clueText)clueText.text=QuestSummary();RefreshGuidance();}
 IEnumerator PlayClue(int index){var notes=Content.Levels[level].target.Where(x=>!x.Empty).ToArray();if(notes.Length>0){Sound(notes[index%notes.Length]);yield return new WaitForSeconds(.35f);if(exploration)Sound(notes[(index+1)%notes.Length]);}}
 void ShowEvidenceBook(){string body="";for(int i=0;i<3;i++){body+="\n"+PropNames[Content.Levels[level].chapter][i]+"：";body+=(save.evidence[level]&(1<<i))==0?"尚未展读。":(level<3?Content.Levels[level].clue:VerseRules.Evidence(level)[i]);}OpenModal("线 索 册",body,"返回卷轴",()=>{CloseModal();OpenExploration();});Button(modal,"请求推演",950,580,230,48,()=>OpenModal("试音一式",string.Join("  ",Content.Levels[level].target.Select((s,i)=>s.Empty?"":("一二三四五六七八"[i]+s.ToString())).Where(x=>x!="")),"返回卷轴",()=>{CloseModal();OpenExploration();}));}
 void ExplorePropForTest(int i){Canvas.ForceUpdateCanvases();var button=exploreButtons[i];var center=button.GetComponent<RectTransform>().TransformPoint(button.GetComponent<RectTransform>().rect.center);float x=i*1120+200;exploration.horizontalNormalizedPosition=Mathf.Clamp01((x-500)/(3000f-1480));Canvas.ForceUpdateCanvases();var p=RectTransformUtility.WorldToScreenPoint(canvas.worldCamera,button.GetComponent<RectTransform>().TransformPoint(button.GetComponent<RectTransform>().rect.center));var e=new PointerEventData(EventSystem.current){position=p};var hits=new System.Collections.Generic.List<RaycastResult>();EventSystem.current.RaycastAll(e,hits);var hit=hits.Select(h=>h.gameObject.GetComponentInParent<Button>()).FirstOrDefault(b=>b!=null);if(hit!=button){Capture(System.IO.Path.Combine(Application.dataPath,"../Validation/explore-failure.png"));throw new Exception("Scroll clue not reachable "+i+" at "+p+" hits "+string.Join(",",hits.Select(h=>h.gameObject.name+":"+h.gameObject.transform.parent.name)));}ExecuteEvents.Execute(button.gameObject,e,ExecuteEvents.pointerClickHandler);}
 void ExploreArtworkForTest(int i){Canvas.ForceUpdateCanvases();float x=i*1120+200;exploration.horizontalNormalizedPosition=Mathf.Clamp01((x-500)/(3000f-1480));Canvas.ForceUpdateCanvases();var button=propViews[i].GetComponent<Button>();var point=RectTransformUtility.WorldToScreenPoint(canvas.worldCamera,propViews[i].art.TransformPoint(propViews[i].art.rect.center));var e=new PointerEventData(EventSystem.current){position=point};var hits=new System.Collections.Generic.List<RaycastResult>();EventSystem.current.RaycastAll(e,hits);var hit=hits.Select(h=>h.gameObject.GetComponentInParent<Button>()).FirstOrDefault(b=>b!=null);if(hit!=button)throw new Exception("Painted object click missed "+i);ExecuteEvents.Execute(button.gameObject,e,ExecuteEvents.pointerClickHandler);}
 IEnumerator ExploreHoverForTest(int i,string dir,int n){
  var view=propViews[i];var pointer=new PointerEventData(EventSystem.current);
  ExecuteEvents.Execute(view.gameObject,pointer,ExecuteEvents.pointerExitHandler);
  yield return new WaitForSeconds(.22f);
  if(view.annotation.alpha>.01f||view.annotation.blocksRaycasts)throw new Exception("Idle annotation still visible");
  if(i==2)Capture(System.IO.Path.Combine(dir,"explore-idle-"+n+".png"));
  ExecuteEvents.Execute(view.gameObject,pointer,ExecuteEvents.pointerEnterHandler);
  yield return new WaitForSeconds(.22f);
  if(view.annotation.alpha<.99f||!view.annotation.blocksRaycasts)throw new Exception("Hover annotation missing");
  if(i==2)Capture(System.IO.Path.Combine(dir,"explore-hover-"+n+".png"));
  ExplorePropForTest(i);
  ExecuteEvents.Execute(view.gameObject,pointer,ExecuteEvents.pointerExitHandler);
  yield return new WaitForSeconds(.22f);
  if(view.annotation.alpha>.01f||view.annotation.blocksRaycasts)throw new Exception("Annotation did not disappear on exit");
 }
 void ShowAudioSettings(){OpenModal("乐 声","竖琴落音，长笛应句，风雨随画而生。\n配乐与音石分开调节；试听时配乐退后，\n留白处保留风雨余韵与隔水回声。","收起",()=>{Persist();CloseModal();});AudioSlider(modal,"音石",410,440,save.volume,v=>{save.volume=v;Persist();});AudioSlider(modal,"配乐",410,520,save.musicVolume,v=>{save.musicVolume=v;Persist();});}
 IEnumerator AudioControlsTest(){yield return new WaitForSeconds(.6f);ShowAudioSettings();yield return null;var sliders=modal.GetComponentsInChildren<Slider>();if(sliders.Length!=2)throw new Exception("Two independent audio controls required");sliders[0].value=.42f;sliders[1].value=.24f;yield return null;var read=JsonUtility.FromJson<SaveData>(System.IO.File.ReadAllText(savePath));if(Mathf.Abs(read.volume-.42f)>.01f||Mathf.Abs(read.musicVolume-.24f)>.01f)throw new Exception("Audio settings did not persist independently");Capture(System.IO.Path.Combine(Application.dataPath,"../Validation/audio-settings.png"));if(!music.Active||runtimeErrors>0)throw new Exception("Audio playback failure");Debug.Log("CIJING_AUDIO_CONTROLS_PASS");yield return new WaitForSeconds(.3f);Application.Quit();}
 void AudioSlider(Transform parent,string title,float x,float y,float value,Action<float> changed){Label(parent,title,x,y,130,40,25);var track=Rect("Volume",parent,x+150,y,550,40);var slider=track.gameObject.AddComponent<Slider>();var back=Panel(track,0,16,550,6,new Color(.2f,.3f,.25f,.3f));back.raycastTarget=true;var fillArea=Rect("Fill area",track,0,16,550,6);var fill=Panel(fillArea,0,0,0,0,accent);fill.rectTransform.anchorMin=Vector2.zero;fill.rectTransform.anchorMax=Vector2.one;fill.rectTransform.offsetMin=fill.rectTransform.offsetMax=Vector2.zero;var handleArea=Rect("Handle area",track,0,20,550,0);var handle=Panel(handleArea,0,0,18,24,red);handle.sprite=disc;handle.rectTransform.pivot=new Vector2(.5f,.5f);slider.fillRect=fill.rectTransform;slider.handleRect=handle.rectTransform;slider.targetGraphic=handle;slider.minValue=0;slider.maxValue=1;slider.value=value;slider.onValueChanged.AddListener(v=>changed(v));}
}
public sealed class ScrollJourneyInput:MonoBehaviour {public ScrollRect scroll;void Update(){float direction=(Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow)?1:0)-(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow)?1:0);if(direction!=0){scroll.velocity=Vector2.zero;scroll.horizontalNormalizedPosition=Mathf.Clamp01(scroll.horizontalNormalizedPosition+direction*FilmClock.Delta*.45f);}}}
}

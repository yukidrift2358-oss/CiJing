using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace CiJing {
public enum GuideTopic { None=-1,Move,Merge,Rain,Rest,Wind,Split,Replay,Free }
public sealed partial class GameApp {
 GuideTopic guideTopic=GuideTopic.None;GuideFocus guideFocus;Text guideHideText;Button guidePracticeButton;bool guideRainSeen,guideSeekSeen;float guideSuccessTime;string guideSuccess;
 bool guidePractice;GuideTopic practiceTopic;int practiceStep;Board practiceBoard;SceneMoment practiceMoment;ScoreSceneView practiceView;Text practiceWords;Button practiceAction;bool practiceBusy;int practiceToken;
 static readonly string[] GuideNames={"落音入水","两音续长","角声落雨","留白见影","羽声起风","拆风留花","回看与续听","自在合声"};
 static readonly string[] GuideWords={
  "点行囊中的短音，再点上方水纹。\n也可拖过去；落定后，按听曲看舟行。",
  "两枚同类短音放到一起，可合成长音。\n长音占一处，需用两份材料。",
  "将角音放入水纹，按听曲。\n留心檐下水钵：短角落一滴，长角续雨。",
  "雨后留一拍空处，水波便会渐平。\n等倒影出现，便能看见留白的作用。",
  "羽音起风，长声会多留一阵。\n风过之后，看看帘角与花瓣的变化。",
  "选中长羽，点拆分或按 X。\n若只轻揭帘角呢？试试两次短风。",
  "点上方一至八拍，回看当时的景。\n再点续听；要通关，仍须完整听曲。",
  "选一枚短音，点合声，再选另一枚。\n宫角、商羽、角徵可相和；取音可补短音。"};
 bool GuideAvailable(GuideTopic t){return t!=GuideTopic.None&&((save.guideDone|save.guideDismissed)&(1<<(int)t))==0;}
 GuideTopic ContextGuide(){if(freeMode)return GuideAvailable(GuideTopic.Free)?GuideTopic.Free:GuideTopic.None;GuideTopic t=level==0?GuideTopic.Move:level==1?GuideTopic.Merge:level==3?(GuideAvailable(GuideTopic.Rain)?GuideTopic.Rain:GuideTopic.Rest):level==4?GuideTopic.Wind:level==5?GuideTopic.Split:level>=6?GuideTopic.Rest:GuideTopic.None;return GuideAvailable(t)?t:GuideTopic.None;}
 void AddGuideReflection(Transform parent,ScoreSceneView view,float x,float y){var raw=Rect("Flower reflection",parent,x,y,115,33).gameObject.AddComponent<RawImage>();raw.texture=Resources.Load<Texture2D>("PropsInk/blossombranch");raw.uvRect=new Rect(0,1,1,-1);raw.raycastTarget=false;raw.color=new Color(.43f,.52f,.48f,.8f);view.ReflectionInk=raw.gameObject.AddComponent<CanvasGroup>();view.ReflectionInk.alpha=0;view.ReflectionInk.blocksRaycasts=false;}
 void BuildGuidance(){guideRainSeen=guideSeekSeen=false;guideSuccessTime=0;guideSuccess=null;guideTopic=ContextGuide();
  guidePracticeButton=Button(page,"试一试",1200,160,105,38,()=>StartGuidePractice(guideTopic==GuideTopic.None?ContextForPractice():guideTopic));guidePracticeButton.GetComponentInChildren<Text>().fontSize=19;
  var hide=Button(page,"收起",1310,160,90,38,DismissGuide);guideHideText=hide.GetComponentInChildren<Text>();guideHideText.fontSize=19;
  var book=Button(page,"引导",1410,160,110,38,ShowGuideBook);book.GetComponentInChildren<Text>().fontSize=20;
  guideFocus=Rect("Ink guide focus",page,0,0,1600,900).gameObject.AddComponent<GuideFocus>();guideFocus.raycastTarget=false;RefreshGuidance();
 }
 GuideTopic ContextForPractice(){return freeMode?GuideTopic.Free:level==0?GuideTopic.Move:level==1?GuideTopic.Merge:level==5?GuideTopic.Split:level>=3&&level<=4?GuideTopic.Rain:GuideTopic.Replay;}
 void RefreshGuidance(){if(!clueText)return;if(guideTopic==GuideTopic.None){clueText.text=freeMode?"五音在侧，疏密随心。\n取音、合声与藏曲皆可重试。\n曲与画可一同保存。":QuestSummary();if(guideHideText)guideHideText.text="重看";}else{clueText.text="<color=#934638>"+GuideNames[(int)guideTopic]+"</color>\n"+GuideWords[(int)guideTopic];if(guideHideText)guideHideText.text="收起";}
  if(guideSuccessTime>0)clueText.text=guideSuccess;
  if(guideFocus)guideFocus.gameObject.SetActive(!modal&&guideTopic!=GuideTopic.None&&guideSuccessTime<=0);
 }
 void DismissGuide(){if(guideTopic==GuideTopic.None){StartGuidePractice(ContextForPractice());return;}save.guideDismissed|=1<<(int)guideTopic;Persist();guideTopic=GuideTopic.None;guideSuccessTime=0;RefreshGuidance();}
 void GuideCompleted(GuideTopic topic,string message){int bit=1<<(int)topic;if((save.guideDone&bit)!=0)return;save.guideDone|=bit;Persist();if(guideTopic!=topic)return;guideSuccessTime=3.4f;guideSuccess="<color=#934638>"+GuideNames[(int)topic]+" · 已试</color>\n"+message+"\n可在引导中重看，也可继续此景。";RefreshGuidance();}
 void GuideTick(float dt){if(!inGame||!guideFocus)return;bool visible=!modal&&!completing;guideFocus.gameObject.SetActive(visible&&guideTopic!=GuideTopic.None&&guideSuccessTime<=0);if(!visible)return;if(guideSuccessTime>0){guideSuccessTime-=dt;if(guideSuccessTime<=0){guideTopic=ContextGuide();RefreshGuidance();}return;}
  int index=-1;switch(guideTopic){case GuideTopic.Move:index=selected>=0?0:Array.FindIndex(board.cells,8,8,s=>!s.Empty);break;case GuideTopic.Merge:if(selected>=0){for(int i=0;i<16;i++)if(i!=selected&&board.cells[i].rank==1&&board.cells[i].tone==board.cells[selected].tone){index=i;break;}}else index=Array.FindIndex(board.cells,8,8,s=>s.rank==1);break;case GuideTopic.Rain:index=Array.FindIndex(board.cells,0,16,s=>s.tone==2&&!s.Empty);break;case GuideTopic.Split:index=selected>=0&&board.cells[selected].tone==4&&board.cells[selected].rank==2?-1:Array.FindIndex(board.cells,0,16,s=>s.tone==4&&s.rank==2);break;}
  if(index>=0)guideFocus.Area=new Rect(154+index%8*119,index<8?600:733,88,88);
  else if(guideTopic==GuideTopic.Replay)guideFocus.Area=guideSeekSeen?new Rect(1245,500,130,42):new Rect(95,490,42,42);
  else if(guideTopic==GuideTopic.Split)guideFocus.Area=new Rect(1150,603,150,48);
  else if(guideTopic==GuideTopic.Free)guideFocus.Area=new Rect(1150,791,150,32);
  else if(guideTopic==GuideTopic.Rest)guideFocus.Area=new Rect(735,357,130,40);
  else guideFocus.Area=new Rect(570,250,160,80);
 }
 void GuidePlaced(bool merged){GuideCompleted(GuideTopic.Move,"音石已入水，听曲时舟身会随声移动。");if(merged)GuideCompleted(GuideTopic.Merge,"短声合为长声，位置少占一处，材料仍是两份。");if(board.cells.Any(s=>s.echo>0))GuideCompleted(GuideTopic.Free,"异音已相和；听曲时，两种景物会一同回应。");}
 void GuideSplit(){GuideCompleted(GuideTopic.Split,"长风已拆开。短风只轻揭帘角，花可以留在枝头。");}
 void GuideScoreBeat(int index){if(partialAudition)return;var note=board.cells[index];if(!note.Empty&&note.tone==2){guideRainSeen=true;GuideCompleted(GuideTopic.Rain,"角声响起，雨滴落进了画中。");}if(!note.Empty&&note.tone==4)GuideCompleted(GuideTopic.Wind,"帘随风起；长风之后，还会落下花瓣。");if(guideTopic==GuideTopic.Rest&&note.Empty&&moment.reflection&&(guideRainSeen||Content.Levels[level].chapter==2))GuideCompleted(GuideTopic.Rest,"声音停住，余波仍在退去；倒影终于显出来了。");}
 void GuideFailure(){if(!ExplorationReady()||!GuideAvailable(GuideTopic.Replay))return;guideSuccessTime=0;guideTopic=GuideTopic.Replay;guideSeekSeen=false;RefreshGuidance();}
 void GuideSeek(){guideSeekSeen=true;if(guideTopic==GuideTopic.Replay){clueText.text="<color=#934638>回看与续听</color>\n此刻显示选定拍结束时的景。\n点续听，从这一拍重新听起。\n修改后，用听曲完整演奏。";}}
 void ShowGuideBook(){OpenModal("循 声 小 卷","择一事亲试，或收卷继续。\n练习另用一小卷，不动此景的音石与进度。","返回",CloseModal);for(int i=0;i<8;i++){int n=i;var b=Button(modal,GuideNames[i]+((save.guideDone&(1<<i))!=0?" · 已试":""),420+i%2*395,390+i/2*58,355,48,()=>StartGuidePractice((GuideTopic)n));b.GetComponentInChildren<Text>().fontSize=23;}}
 void StartGuidePractice(GuideTopic topic){if(modal)CloseModal();if(playing){playing=false;Silence();if(playLabel)playLabel.text="听 曲";}practiceToken++;guidePractice=true;practiceTopic=topic;practiceStep=0;practiceBoard=new Board();practiceBusy=false;practiceMoment=SceneScore.Begin(topic==GuideTopic.Move||topic==GuideTopic.Merge?0:1);
  OpenModal("亲 试 · "+GuideNames[(int)topic],"","返回此景",CloseModal);
  var area=Rect("Practice ink scene",modal,425,285,745,175);var art=Rect("Practice garden",area,0,-20,745,195).gameObject.AddComponent<RawImage>();art.texture=Resources.Load<Texture2D>("SceneScore/garden");art.raycastTarget=false;
  practiceView=Rect("Practice movement",area,0,0,745,175).gameObject.AddComponent<ScoreSceneView>();practiceView.raycastTarget=false;practiceView.Chapter=1;practiceView.GardenInk=new Material(Resources.Load<Shader>("SceneArt"));practiceView.GardenInk.SetFloat("_Growth",.6f);art.material=practiceView.GardenInk;
  var curtain=Rect("Practice curtain",area,330,0,260,145).gameObject.AddComponent<RawImage>();curtain.texture=Resources.Load<Texture2D>("SceneScore/curtain");curtain.raycastTarget=false;practiceView.Curtain=curtain.rectTransform;
  if(topic==GuideTopic.Move||topic==GuideTopic.Merge||topic==GuideTopic.Free){var boatArt=Rect("Practice boat",area,80,150,145,60).gameObject.AddComponent<BoatGraphic>();boatArt.raycastTarget=false;boatArt.color=ink;practiceView.Boat=boatArt.rectTransform;}
  AddGuideReflection(area,practiceView,575,121);music.Environment(practiceMoment);practiceView.Set(practiceMoment,true);practiceWords=Label(modal,"",430,468,740,98,23);practiceAction=Button(modal,"试一声",540,578,235,52,PracticeAdvance,true);var skip=Button(modal,"略过练习",820,578,235,52,CloseModal);skip.GetComponentInChildren<Text>().fontSize=22;ShowPracticeStep();
 }
 void ShowPracticeStep(){if(!practiceWords)return;string[] lines=PracticeLines(practiceTopic);practiceWords.text=lines[Math.Min(practiceStep,lines.Length-1)];practiceAction.GetComponentInChildren<Text>().text=practiceStep==0?"试一声":practiceStep==1?(practiceTopic==GuideTopic.Split?"重试短风":practiceTopic==GuideTopic.Replay?"从此续听":"留一拍空") :"收入小卷";}
 static string[] PracticeLines(GuideTopic topic){switch(topic){
  case GuideTopic.Split:return new[]{"听一声长羽，看看帘与花会怎样。","长风已落花，拆分不会倒转此刻的景。\n回到起初，试试两声短风。","重试后，两次短风卷帘，残花仍在枝头。\n正式游玩：选长羽后拆分，再从头听曲。"};
  case GuideTopic.Replay:return new[]{"听一滴雨落下，记住此刻的水面。","现在回到雨声之后的那一拍。\n正式游玩时，点上方拍位，再点续听即可。","续听可比较局部变化；完整听曲才会判定通关。"};
  case GuideTopic.Move:return new[]{"试落一声宫，看看舟行留下的水波。","声落之后，也给水面留一点时间。","正式游玩时，点行囊音石，再点水纹空位；\n也可拖动。按听曲，景物才会依拍回应。"};
  case GuideTopic.Merge:return new[]{"这一声是长宫，由两枚短宫相合。","长声用了两份材料，却只占一处水纹。","正式游玩时，两枚同类短音放在一起即可合成。\n长音可拆；行囊须有一处空位。"};
  case GuideTopic.Free:return new[]{"宫与角相和，一声中既有行舟，也有落雨。","合声能拆成原来的两枚短音，材料不增不减。","选短音、点合声，再选另一枚短音。\n宫角、商羽、角徵可合；取音只用于自由谱曲。"};
  case GuideTopic.Wind:return new[]{"试一声长羽，留心帘角与枝头。","长风会继续一阵。停下来，花瓣才会渐渐落定。","长羽起长风，短羽只轻拂。\n声音长短会改变景物，不只改变音色。"};
  default:return new[]{"试一声角，让雨先落入画中。","别急着补下一声。点留一拍空，等水波退去。","雨声歇了，水面才显出倒影。\n若用了长角，便要多留几拍，等连雨收住。"};}}
 void PracticeAdvance(){if(practiceBusy)return;if(practiceStep>=2){GuideCompleted(practiceTopic,"小卷已试，可在原景中再用一次。");CloseModal();return;}StartCoroutine(PracticeAction(practiceToken));}
 IEnumerator PracticeAction(int token){practiceBusy=true;practiceAction.interactable=false;bool first=practiceStep==0;
  if(first){int tone=practiceTopic==GuideTopic.Move||practiceTopic==GuideTopic.Merge||practiceTopic==GuideTopic.Free?0:practiceTopic==GuideTopic.Wind||practiceTopic==GuideTopic.Split?4:2;practiceBoard.cells[8]=new Stone(tone);if(practiceTopic==GuideTopic.Free){practiceBoard.cells[9]=new Stone(2);practiceBoard.Weave(8,9);practiceBoard.Move(9,0);}else if(practiceTopic==GuideTopic.Merge||practiceTopic==GuideTopic.Wind||practiceTopic==GuideTopic.Split){practiceBoard.cells[9]=new Stone(tone);practiceBoard.Move(8,9);practiceBoard.Move(9,0);}else practiceBoard.Move(8,0);var note=practiceBoard.cells[0];practiceMoment=SceneScore.Step(practiceMoment,note,practiceView.Chapter);Sound(note);}
  else if(practiceTopic==GuideTopic.Split){if(!practiceBoard.Split(0))throw new Exception("Practice split failed");practiceMoment=SceneScore.Begin(1);practiceMoment=SceneScore.Step(practiceMoment,practiceBoard.cells[0],1);Sound(practiceBoard.cells[0]);practiceMoment=SceneScore.Step(practiceMoment,default(Stone),1);practiceView.Set(practiceMoment);yield return new WaitForSeconds(.7f);if(token!=practiceToken||!guidePractice||!practiceView)yield break;practiceMoment=SceneScore.Step(practiceMoment,practiceBoard.cells[8],1);Sound(practiceBoard.cells[8]);}
  else practiceMoment=SceneScore.Step(practiceMoment,default(Stone),practiceView.Chapter);
  practiceView.Set(practiceMoment);music.Environment(practiceMoment);yield return new WaitForSeconds(1.6f);
  if(token!=practiceToken||!guidePractice||!modal||!practiceAction)yield break;
  practiceStep++;practiceBusy=false;practiceAction.interactable=true;ShowPracticeStep();
 }
 void CloseGuidePractice(){Silence();practiceToken++;guidePractice=false;practiceView=null;practiceWords=null;practiceAction=null;practiceBusy=false;if(music)music.Environment(moment);RefreshGuidance();}
 IEnumerator GuidanceTestOnly(){var dir=Path.Combine(Application.dataPath,"../Validation");Directory.CreateDirectory(dir);yield return new WaitForSeconds(.6f);yield return TestGuidance(dir);if(runtimeErrors>0)throw new Exception("Guidance runtime errors: "+runtimeErrors);ShowGuideBook();yield return null;Capture(Path.Combine(dir,"guide-book.png"));Debug.Log("CIJING_GUIDANCE_ONLY_PASS");yield return new WaitForSeconds(.3f);Application.Quit();}
 void ClickGuideForTest(Button button){Canvas.ForceUpdateCanvases();var rt=button.GetComponent<RectTransform>();var point=RectTransformUtility.WorldToScreenPoint(canvas.worldCamera,rt.TransformPoint(rt.rect.center));var e=new PointerEventData(EventSystem.current){position=point};var hits=new System.Collections.Generic.List<RaycastResult>();EventSystem.current.RaycastAll(e,hits);if(hits.Select(h=>h.gameObject.GetComponentInParent<Button>()).FirstOrDefault(x=>x)!=button){Capture(Path.Combine(Application.dataPath,"../Validation/guide-click-failure.png"));throw new Exception("Guide button blocked: "+button.GetComponentInChildren<Text>().text+" at "+point+" hits "+string.Join(",",hits.Select(h=>h.gameObject.name+"/"+h.gameObject.transform.parent.name)));}ExecuteEvents.Execute(button.gameObject,e,ExecuteEvents.pointerClickHandler);}
 IEnumerator TestGuidance(string dir){save.guideDone=save.guideDismissed=0;LoadLevel(3);CloseModal();var original=(Stone[])board.cells.Clone();int mass=board.Mass,oldEvidence=save.evidence[3],oldUnlocked=save.unlocked;StartGuidePractice(GuideTopic.Rest);yield return null;ClickGuideForTest(practiceAction);yield return new WaitForSeconds(1.8f);if(practiceStep!=1||practiceMoment.rain<=0)throw new Exception("Rain practice did not play");Capture(Path.Combine(dir,"guide-rain.png"));ClickGuideForTest(practiceAction);yield return new WaitForSeconds(1.8f);if(practiceStep!=2||!practiceMoment.reflection)throw new Exception("Rest practice did not reveal reflection");Capture(Path.Combine(dir,"guide-rest.png"));ClickGuideForTest(practiceAction);if(modal||board.Mass!=mass||!board.cells.SequenceEqual(original)||save.evidence[3]!=oldEvidence||save.unlocked!=oldUnlocked)throw new Exception("Practice modified main puzzle");
  StartGuidePractice(GuideTopic.Split);yield return null;ClickGuideForTest(practiceAction);yield return new WaitForSeconds(1.8f);if(practiceMoment.petals!=2)throw new Exception("Long wind practice failed");ClickGuideForTest(practiceAction);yield return new WaitForSeconds(2.6f);if(practiceMoment.petals!=0||practiceMoment.curtain<.9f)throw new Exception("Split wind practice failed");Capture(Path.Combine(dir,"guide-split.png"));CloseModal();
  save.guideDone=save.guideDismissed=0;LoadLevel(0);CloseModal();DismissGuide();if(guideTopic!=GuideTopic.None||(save.guideDismissed&1)==0)throw new Exception("Guide dismissal failed");DrawGame();if(guideTopic!=GuideTopic.None)throw new Exception("Dismissed guide returned");ShowGuideBook();StartGuidePractice(GuideTopic.Move);yield return null;ClickGuideForTest(practiceAction);CloseModal();yield return new WaitForSeconds(1.8f);if(modal||guidePractice||playing||voices.Any(v=>v.isPlaying)||music.ResponseActive)throw new Exception("Closed practice retained input, coroutine or note audio");
  save.guideDone=save.guideDismissed=0;LoadLevel(3);CloseModal();save.evidence[3]=1;Play();yield return new WaitForSeconds(6.1f);if(guideTopic!=GuideTopic.Replay)throw new Exception("Wrong melody did not offer replay");Capture(Path.Combine(dir,"guide-replay.png"));SeekScore(2);ResumeScore();if((save.guideDone&(1<<(int)GuideTopic.Replay))==0)throw new Exception("Seek resume guide incomplete");playing=false;Silence();Persist();var data=JsonUtility.FromJson<SaveData>(File.ReadAllText(savePath));if(data.guideDone!=save.guideDone)throw new Exception("Guide progress save failed");
  for(int n=0;n<8;n++){ShowGuideBook();yield return null;var button=modal.GetComponentsInChildren<Button>().First(b=>b.GetComponentInChildren<Text>().text.StartsWith(GuideNames[n]));ClickGuideForTest(button);if(!guidePractice||practiceTopic!=(GuideTopic)n)throw new Exception("Guide book topic not reachable");CloseModal();}
  File.WriteAllText(Path.Combine(dir,"guidance-results.json"),"{\"passed\":true,\"isolatedPractice\":true,\"rainAndRest\":true,\"splitWind\":true,\"dismissAndReview\":true,\"cancelSafe\":true,\"failureReplay\":true,\"savedProgress\":true}");Debug.Log("CIJING_GUIDANCE_PASS rain, rest, split, isolation, dismissal, review, cancel, failure replay, save");
 }
}
public sealed class GuideFocus:MaskableGraphic {
 public Rect Area;float time;void Update(){time+=FilmClock.Delta;SetVerticesDirty();}
 protected override void OnPopulateMesh(VertexHelper v){v.Clear();float rx=Area.width*.5f+5,ry=Area.height*.5f+5;Vector2 center=new Vector2(Area.center.x,-Area.center.y);for(int i=0;i<54;i++){float a=i*Mathf.PI*2/60,b=(i+1)*Mathf.PI*2/60;Vector2 p=center+new Vector2(Mathf.Cos(a)*rx,Mathf.Sin(a)*ry),q=center+new Vector2(Mathf.Cos(b)*rx,Mathf.Sin(b)*ry);Vector2 n=new Vector2(-(q-p).y,(q-p).x).normalized*(.65f+.2f*Mathf.Sin(i));Color c=new Color(.62f,.25f,.18f,.35f+.14f*Mathf.Sin(time*2));int at=v.currentVertCount;v.AddVert(p-n,c,Vector2.zero);v.AddVert(p+n,c,Vector2.zero);v.AddVert(q+n,c,Vector2.zero);v.AddVert(q-n,c,Vector2.zero);v.AddTriangle(at,at+1,at+2);v.AddTriangle(at,at+2,at+3);}}
}
}

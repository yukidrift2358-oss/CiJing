using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
namespace CiJing {
public sealed partial class GameApp {
 ScoreSceneView scoreView;SceneMoment moment;Text sceneCaption,seekCaption;int inspected=-1;bool partialAudition;
 void BuildScoreScene(){
  inspected=-1;moment=SceneScore.Begin(Content.Levels[level].chapter);
  RawImage garden=null;var window=Rect("Sound becomes landscape",page,60,220,970,210);window.gameObject.AddComponent<RectMask2D>();
  if(Content.Levels[level].chapter==1||freeMode){var art=Rect("Begonia garden",window,0,-26,970,275).gameObject.AddComponent<RawImage>();art.texture=Resources.Load<Texture2D>("SceneScore/garden");art.raycastTarget=false;garden=art;}
  var view=Rect("Breathing ink",window,0,0,970,210).gameObject.AddComponent<ScoreSceneView>();view.Chapter=Content.Levels[level].chapter;view.raycastTarget=false;scoreView=view;view.GateGarden=!freeMode&&Content.Levels[level].chapter==1;if(garden){var mat=new Material(Resources.Load<Shader>("SceneArt"));mat.SetFloat("_Growth",Mathf.Clamp01((level-2)/6f));garden.material=mat;view.GardenInk=mat;}
  if(view.Chapter==1||freeMode){var curtain=Rect("Lifted gauze",window,635,-20,340,205).gameObject.AddComponent<RawImage>();curtain.texture=Resources.Load<Texture2D>("SceneScore/curtain");curtain.raycastTarget=false;view.Curtain=curtain.rectTransform;view.CurtainInk=new Material(Resources.Load<Shader>("SceneArt"));view.CurtainInk.SetFloat("_Growth",.45f);curtain.material=view.CurtainInk;curtain.color=new Color(1,1,1,.78f);var bowl=Rect("Rain bowl",window,105,132,95,67).gameObject.AddComponent<RawImage>();bowl.texture=Resources.Load<Texture2D>("PropsInk/rainbowl");bowl.raycastTarget=false;
   var letter=Rect("Letter under petals",window,715,92,130,72);var image=letter.gameObject.AddComponent<RawImage>();image.texture=Resources.Load<Texture2D>("PropsInk/petalletter");image.raycastTarget=false;view.Letter=letter;view.LetterInk=letter.gameObject.AddComponent<CanvasGroup>();view.LetterInk.alpha=0;AddRelicTrace(letter,2,130,72);var touch=Button(letter,"拾 笺",0,36,130,36,()=>ReadRelic(1));touch.GetComponentInChildren<Text>().fontSize=19;}
  if(view.Chapter==0){var b=Rect("A boat follows the score",window,70,175,145,60);var g=b.gameObject.AddComponent<BoatGraphic>();g.color=new Color(.18f,.25f,.22f,.85f);g.raycastTarget=false;view.Boat=b;}
  Panel(page,65,394,950,36,new Color(.96f,.945f,.88f,.95f));sceneCaption=Label(page,moment.caption,80,395,900,32,22,ink);
  seekCaption=Label(page,"点水纹拍位，可回看当时的景。",1100,456,440,35,19,accent);
  Button(page,"前 拍",1100,500,130,42,()=>SeekScore(Mathf.Max(0,inspected-1)));
  Button(page,"续 听",1245,500,130,42,ResumeScore);
  Button(page,"画 记",1390,500,130,42,ReadSceneMemory);
  for(int i=0;i<8;i++){int n=i;var hit=Panel(page,95+i*119,490,42,42,Color.clear);hit.raycastTarget=true;var b=hit.gameObject.AddComponent<Button>();b.transition=Selectable.Transition.None;b.onClick.AddListener(()=>SeekScore(n));}
  BuildGardenExperience(window);AddGuideReflection(window,view,680,141);if(freeMode&&save.composition!=null){moment=SceneScore.At(board.cells,Content.Levels[level].chapter,7);sceneCaption.text=moment.caption;}scoreView.Set(moment,true);music.Environment(moment);
 }
 void ScoreBeat(int index){inspected=index;moment=SceneScore.At(board.cells,Content.Levels[level].chapter,index);if(scoreView)scoreView.Set(moment);if(sceneCaption)sceneCaption.text=moment.caption;if(seekCaption)seekCaption.text="第 "+(index+1)+" 拍 · "+(board.cells[index].Empty?"留白":board.cells[index].ToString());music.Environment(moment);UpdateGardenExperience();GuideScoreBeat(index);
  if(!partialAudition&&SceneScore.Outcome(moment)!=0){int before=save.sceneDiscoveries[level];save.sceneDiscoveries[level]|=SceneScore.Outcome(moment);if(before!=save.sceneDiscoveries[level]){Persist();if(clueText&&!freeMode)clueText.text=QuestSummary();}}RefreshGuidance();
 }
 void SeekScore(int index){if(modal||completing)return;playing=false;Silence();partialAudition=true;beat=Mathf.Clamp(index,0,7);ScoreBeat(beat);UpdateSlots();playLabel.text="听 曲";SetFeedback("回看第 "+(beat+1)+" 拍。续听可从此处接起；听曲可完整重奏。");GuideSeek();}
 void ResumeScore(){if(modal||completing)return;if(inspected<0){Play();return;}Silence();if(guideSeekSeen)GuideCompleted(GuideTopic.Replay,"已从选定拍接着听。试好之后，用听曲完整演奏。");partialAudition=true;playing=true;beat=inspected-1;elapsed=.7f;playLabel.text="止 音";SetFeedback("从第 "+(inspected+1)+" 拍续听；完整奏毕才收入本景。");}
 void RecordSceneOutcome(){int result=SceneScore.Outcome(moment);save.sceneOutcomes[level]=result;save.sceneDiscoveries[level]|=result;if(freeMode){save.compositionScene=result;save.compositionChapter=Content.Levels[level].chapter;save.composition=(Stone[])board.cells.Clone();}Persist();}
 void ReadSceneMemory(){if(playing){playing=false;Silence();playLabel.text="听 曲";}int known=save.sceneDiscoveries[level];string body="";
  if(known==0)body="画中尚无新迹。\n\n角声落雨，羽声起风；宫声行舟。\n长声可续风雨，短声只留一痕。\n声歇之后，留心水影与花枝。";
  else {if((known&1)!=0)body+="水定 · 余波退去，倒影才显。\n";if((known&2)!=0)body+="落笺 · 长风吹落残红，花下露出旧书。\n";if((known&4)!=0)body+="留花 · 两次微风卷起帘角，残花仍在枝头。\n";if((known&8)!=0)body+="回音 · 连留两拍，隔水方闻人语。\n";body+="\n"+SceneScore.Verse(SceneScore.Outcome(moment)!=0?SceneScore.Outcome(moment):known)+"\n（画中题句为本作自撰）";}
  if(!freeMode&&level==5)body+="\n\n两滴雨先听清，风过后候水定。\n长风可见笺，两次微风可留一枝残红；帘起再观花。";
  OpenModal("画 中 余 音",body,"收 笺",CloseModal);Button(modal,"旧景余音",1040,650,200,58,ShowRevisits);
 }
 void OpenSoundPalette(){if(!freeMode||playing)return;OpenModal("取 音", "行囊若有空处，便可添一枚短音。\n五音任取，可合可拆；主线的音石仍循原景。", "收 起",CloseModal);for(int i=0;i<5;i++){int tone=i;Button(modal,"宫商角徵羽"[i].ToString(),430+i*145,480,125,65,()=>{int slot=Array.FindIndex(board.cells,8,8,x=>x.Empty);if(slot<0){CloseModal();SetFeedback("行囊已满，先移一枚入水。");return;}board.cells[slot]=new Stone(tone);weaving=false;selected=-1;CloseModal();UpdateSlots();SetFeedback("添得一枚短音，可续此曲。");});}}
 IEnumerator TestSceneScore(string dir){
  level=5;LoadLevel(5);CloseModal();save.evidence[5]=1;board.Restore(new Stone[16]);board.cells[0]=new Stone(2);board.cells[4]=new Stone(2);board.cells[2]=new Stone(4);board.cells[6]=new Stone(4);UpdateSlots();
  if(!board.Matches(Content.Levels[5]))throw new Exception("Gentle route rejected");
  SeekScore(3);if(moment.curtain<.4f||moment.petals!=0)throw new Exception("Seeking gentle curtain failed");Capture(Path.Combine(dir,"causal-gentle-midpoint.png"));
  SeekScore(7);if((SceneScore.Outcome(moment)&4)==0)throw new Exception("Gentle flowers missing");yield return new WaitForSeconds(.5f);Capture(Path.Combine(dir,"causal-gentle-ending.png"));
  ResumeScore();yield return new WaitForSeconds(1.7f);if(solved)throw new Exception("Partial audition completed mainline");
  Play();yield return new WaitForSeconds(6.1f);if(!gardenPending)throw new Exception("Garden observation was skipped");ObserveGarden();yield return null;ClickModalActionForTest("将所见入卷");if(!solved||save.sceneOutcomes[5]!=5)throw new Exception("Gentle full route failed");yield return new WaitForSeconds(2.3f);CloseModal();
  LoadLevel(5);CloseModal();save.evidence[5]=1;board.Restore(new Stone[16]);board.cells[0]=new Stone(2);board.cells[4]=new Stone(2);board.cells[2]=new Stone(4,2);UpdateSlots();SeekScore(7);yield return new WaitForSeconds(.5f);if(!moment.letter)throw new Exception("Petal letter not revealed");Capture(Path.Combine(dir,"causal-letter-ending.png"));ReadSceneMemory();yield return null;Capture(Path.Combine(dir,"causal-memory.png"));CloseModal();
  var longRain=new Stone[16];longRain[0]=new Stone(2,2);if(SceneScore.At(longRain,1,1).reflection)throw new Exception("Long rain settled too early");if(!SceneScore.At(longRain,1,4).reflection)throw new Exception("Rain never settled");
  StartFree();board.Restore(longRain);SeekScore(4);RecordSceneOutcome();var data=JsonUtility.FromJson<SaveData>(File.ReadAllText(savePath));if(data.compositionScene!=SceneScore.Outcome(moment)||data.composition[0].rank!=2)throw new Exception("Scene and melody save mismatch");
  board.Restore(new Stone[16]);OpenSoundPalette();yield return null;var pick=modal.GetComponentsInChildren<Button>();var toneButton=Array.Find(pick,x=>x.GetComponentInChildren<Text>().text=="角");if(!toneButton)throw new Exception("Free sound palette missing");toneButton.onClick.Invoke();if(board.cells[8].tone!=2||board.cells[8].rank!=1)throw new Exception("Free sound choice failed");
  Debug.Log("CIJING_CAUSAL_PASS two endings, seek, resume, complete gate, settling, scene save, free palette");
 }
}
}

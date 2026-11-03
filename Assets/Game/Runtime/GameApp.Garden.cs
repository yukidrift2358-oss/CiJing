using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace CiJing {
public sealed partial class GameApp {
 Stone[] gardenPerformed;bool gardenPending;Text gardenProgress;Button gardenInspect;AudioLowPassFilter noteAir;AudioSource waterVoice;
 void BuildGardenExperience(Transform window){gardenPending=false;gardenPerformed=null;gardenProgress=null;gardenInspect=null;
  if(!freeMode&&Content.Levels[level].chapter==1){gardenProgress=Label(page,GardenGoals.Progress(moment),80,438,700,37,21,accent);gardenInspect=Button(page,"隔帘听雨",810,436,220,42,ObserveGarden,true);gardenInspect.GetComponentInChildren<Text>().fontSize=22;gardenInspect.interactable=false;}
  if(!freeMode){var lamp=Rect("A familiar lamp",window,12,4,75,115).gameObject.AddComponent<RawImage>();lamp.texture=Resources.Load<Texture2D>("PropsInk/boatlantern");lamp.raycastTarget=true;var touch=lamp.gameObject.AddComponent<Button>();touch.transition=Selectable.Transition.None;touch.onClick.AddListener(()=>ReadRelic(0));var wick=Rect("Changed lamp wick",lamp.transform,31,65,15,22).gameObject.AddComponent<RelicTrace>();wick.kind=Content.Levels[level].chapter==0?0:1;wick.raycastTarget=false;var name=Label(window,Content.Levels[level].chapter==0?"归舟灯":"灯芯尚温",0,119,115,25,17,accent,TextAnchor.MiddleCenter);}
 }
 void InvalidateGardenIfChanged(){if(!gardenPending||gardenPerformed==null||gardenPerformed.SequenceEqual(board.cells))return;gardenPending=false;gardenPerformed=null;UpdateGardenExperience();RefreshGuidance();SetFeedback("音石已有改动，须完整重听，再将所见入卷。");}
 void UpdateGardenExperience(){if(gardenProgress)gardenProgress.text=GardenGoals.Progress(moment);if(gardenInspect){gardenInspect.interactable=moment.curtain>=.7f;gardenInspect.GetComponentInChildren<Text>().text=gardenPending?"观花 · 比照":"观 花";}if(scoreView&&scoreView.GardenInk)scoreView.GardenInk.SetFloat("_Reveal",Mathf.SmoothStep(0,1,Mathf.InverseLerp(.35f,.9f,moment.curtain)));}
 void FinishPerformance(){if(partialAudition){SetFeedback("此处余音已尽。完整听曲后，方可收入本景。");return;}RecordSceneOutcome();if(freeMode){FinishSecretSong();return;}string reason;bool matches=VerseRules.Check(board.cells,level,out reason);if(matches&&ExplorationReady()){if(level==5){gardenPerformed=(Stone[])board.cells.Clone();gardenPending=true;UpdateGardenExperience();RefreshGuidance();SetFeedback("雨收风歇，帘外已明。点观花，看看今晨与先前所想有何不同。");}else Complete();}else {SetFeedback(ExplorationReady()?reason:PuzzleFeedback());GuideFailure();}}
 void ObserveGarden(){if(freeMode||Content.Levels[level].chapter!=1||moment.curtain<.7f||completing)return;InvalidateGardenIfChanged();bool full=gardenPending&&!partialAudition&&level==5;playing=false;Silence();if(playLabel)playLabel.text="听 曲";bool flower=moment.petals==0;string guess=save.gardenGuess==1?"你先前以为花仍如昨。":save.gardenGuess==2?"你先前猜想，雨后花会稀些。":"隔帘时，花的模样尚不可知。";
  string seen=flower?"帘外仍有一枝残红，满树花色却已淡去；新叶比花更显眼。微风留住的是一枝花，并未将昨日整个留住。":"枝头的红已经稀了，叶色却浓。花下露出的纸笺，与掌心那封信有相同的折痕。";
  string body=guess+"\n\n"+seen+"\n\n“知否，知否？应是绿肥红瘦。”\n李清照《如梦令》";
  if(!full)body+="\n\n此为试观；完整奏毕、待水影清楚后，方可收入词笺。";
  OpenModal("卷 帘 亲 见",body,full?"将所见入卷":"再 听",()=>{CloseModal();if(full){gardenPending=false;save.gardenRead|=flower?1:2;save.relics|=flower?4:2;Complete();}});
  var raw=Rect("Evidence from the garden",modal,1095,330,140,150).gameObject.AddComponent<RawImage>();raw.texture=Resources.Load<Texture2D>("PropsInk/"+(flower?"blossombranch":"petalletter"));raw.raycastTarget=false;var bodyText=Array.Find(modal.GetComponentsInChildren<Text>(),t=>t.text==body);bodyText.rectTransform.sizeDelta=new Vector2(665,350);bodyText.fontSize=23;if(!flower)AddRelicTrace(raw.transform,2,140,150);
 }
 void AskBehindCurtain(){if(modal)CloseModal();if(save.gardenRead!=0||moment.curtain>=.7f){OpenModal("隔帘旧猜",(save.gardenGuess==1?"当时所记：花仍如昨。":save.gardenGuess==2?"当时所记：雨后花稀。":"当时并未作猜。")+"\n\n帘已卷过，旧猜不再改写；眼前所见，可收入此行留卷。","收 起",CloseModal);return;}OpenModal("隔 帘 一 问","昨夜只听见雨声与风声，尚未看过庭中。\n你猜，今晨海棠是什么模样？\n\n这一猜只作留记，卷帘后再与所见比照。","暂不作答",CloseModal);Button(modal,"花仍如昨",440,490,280,60,()=>SaveGardenGuess(1));Button(modal,"雨后花稀",860,490,280,60,()=>SaveGardenGuess(2));}
 void SaveGardenGuess(int choice){save.gardenGuess=choice;Persist();CloseModal();SetFeedback("先将所想留在帘内，待亲见时再读。");}
 void LeaveExploration(){if(level==3&&save.gardenGuess==0)AskBehindCurtain();else CloseModal();}
 void AddRelicTrace(Transform parent,int kind,float width,float height){var mark=Rect("Shared fold and wick",parent,width*.25f,height*.35f,width*.5f,height*.4f).gameObject.AddComponent<RelicTrace>();mark.kind=kind;mark.raycastTarget=false;}
 void ReadRelic(int which){if(playing){playing=false;Silence();}if(modal)CloseModal();bool warm=Content.Levels[level].chapter>0;string text=which==0?(warm?"新灯芯还温着，灯罩上的旧缺口却没有变。\n\n这是你留在溪亭的那盏灯。舟到花阴之前，已有别人添过油。":"灯芯短了，罩沿有一个小缺口。\n\n信上只写：归时，留一盏灯。你记住了这个缺口。"):"纸背三道浅折，一长两短。\n开卷时那封无款的信，也有同样的折痕。\n\n这封旧笺曾被人展开，又认真折回原处。";
  if(which==0&&warm)save.relics|=1;if(which==1)save.relics|=2;Persist();OpenModal(which==0?"灯 下 旧 痕":"纸 背 浅 折",text,"收 起",CloseModal);var art=Rect("Relic close view",modal,1000,320,180,210).gameObject.AddComponent<RawImage>();art.texture=Resources.Load<Texture2D>("PropsInk/"+(which==0?"boatlantern":"petalletter"));art.raycastTarget=false;AddRelicTrace(art.transform,which==0?(warm?1:0):2,180,210);Array.Find(modal.GetComponentsInChildren<Text>(),t=>t.text==text).rectTransform.sizeDelta=new Vector2(570,320);}
 void ShowKeepsake(){bool flower=(save.sceneOutcomes[5]&4)!=0;string text="灯、枝影与旧纸，记下此行所见。\n\n"+((save.relics&1)!=0?"灯芯：有人替你续过火。":"灯芯的来处，尚待入景探看。")+"\n"+((save.relics&2)!=0?"纸折：开卷旧信与花间书笺出自同一路。":"花间书笺的折痕，尚未展读。")+"\n"+((save.gardenRead!=0||save.sceneOutcomes[5]!=0)?(flower?"回信附着一笔枝影，记着你留下的残红。":"回信夹着花间旧笺，缺失的半句已有回答。"):"庭中之景，尚未亲见。")+"\n\n"+((save.revisited&3)==3?"旧水痕与隔岸余音皆已读懂。":"旧景仍有余音，学过新意再回听。");
  OpenModal("此 行 留 卷",text,"收 卷",CloseModal);var body=Array.Find(modal.GetComponentsInChildren<Text>(),t=>t.text==text);body.fontSize=23;body.rectTransform.sizeDelta=new Vector2(585,340);var lamp=Rect("The lamp remembered",modal,1005,305,150,160).gameObject.AddComponent<RawImage>();lamp.texture=Resources.Load<Texture2D>("PropsInk/boatlantern");lamp.raycastTarget=false;AddRelicTrace(lamp.transform,1,150,160);if(save.gardenRead!=0||save.sceneOutcomes[5]!=0){var token=Rect("A personal reply",modal,1000,475,180,110).gameObject.AddComponent<RawImage>();token.texture=Resources.Load<Texture2D>("PropsInk/"+(flower?"blossombranch":"petalletter"));token.raycastTarget=false;}
 }
}
public sealed class RelicTrace:MaskableGraphic {
 public int kind;
 protected override void OnPopulateMesh(VertexHelper v){v.Clear();float w=rectTransform.rect.width,h=rectTransform.rect.height;var c=new Color(.31f,.26f,.20f,.65f);if(kind==2){for(int i=0;i<3;i++)Line(v,new Vector2(w*.1f,-h*(.2f+i*.28f)),new Vector2(w*(i==0?.92f:.66f),-h*(.3f+i*.28f)),.8f,c);}else {Line(v,new Vector2(w*.5f,0),new Vector2(w*.5f,-h*.6f),1.3f,c);if(kind==1){for(int i=0;i<5;i++)Line(v,new Vector2(w*.5f-i*.7f,-h*.35f),new Vector2(w*.5f+i*.6f,-h*.05f),1,new Color(.72f,.43f,.18f,.14f));}}}
 static void Line(VertexHelper v,Vector2 a,Vector2 b,float width,Color c){var n=new Vector2(-(b-a).y,(b-a).x).normalized*width;int i=v.currentVertCount;v.AddVert(a-n,c,Vector2.zero);v.AddVert(a+n,c,Vector2.zero);v.AddVert(b+n,c,Vector2.zero);v.AddVert(b-n,c,Vector2.zero);v.AddTriangle(i,i+1,i+2);v.AddTriangle(i,i+2,i+3);}
}
}

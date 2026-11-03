using System;
namespace CiJing {
// Each beat is evaluated from the same initial state. Seeking never accumulates
// physical drift or unlocks a mainline ending without a complete performance.
[Serializable] public struct SceneMoment {
 public float rain,wind,ripple,curtain,boat,loosePaper;
 public int petals,still,birds;
 public bool reflection,letter,reply,overshot;
 public string caption;
}
public static class SceneScore {
 public static SceneMoment Begin(int chapter){return new SceneMoment{boat=.12f,caption=chapter==1?"帘垂庭寂，且听檐声。":"水静如笺，待一声起。"};}
 public static SceneMoment Step(SceneMoment s,Stone note,int chapter){
  s.rain=Math.Max(0,s.rain-.65f);s.wind=Math.Max(0,s.wind-.6f);s.ripple=Math.Max(0,s.ripple-.32f);s.loosePaper=Math.Max(0,s.loosePaper-.3f);
  if(note.Empty){s.still++;s.caption="声歇一拍，余波渐平。";}
  else {s.still=0;Apply(ref s,note.tone,note.rank);if(note.echo>0)Apply(ref s,note.echo-1,1);}
  s.reflection=s.still>=1&&s.rain<.4f&&s.ripple<.45f;
  if(s.curtain>.4f&&s.petals>=2&&s.still>=1)s.letter=true;
  if(chapter==2&&s.still>=2&&s.ripple<.35f)s.reply=true;
  if(s.letter&&note.Empty)s.caption="花落笺初见，帘边有旧书。";
  else if(s.reply&&note.Empty)s.caption="停桨听归语，隔水有人应。";
  else if(s.reflection)s.caption="水定花影现，静处见春深。";
  if(s.loosePaper>.4f)s.caption="风急纸轻，且候余声歇。";
  if(s.overshot)s.caption="长桨过前汀，归舟须缓行。";
  return s;
 }
 static void Apply(ref SceneMoment s,int tone,int rank){
  switch(tone){
   case 0:s.boat+=rank==2?.34f:.13f;s.overshot=s.boat>1;s.ripple=rank==2?1:.65f;s.caption=rank==2?"长桨穿荷隙，舟过一重烟。":"轻桨拨微澜，舟移半寸天。";break;
   case 1:s.ripple=.45f;s.caption=rank==2?"长声渡月水，余韵到西楼。":"一声遥相问，月色到汀洲。";break;
   case 2:s.rain=rank==2?1.5f:.7f;s.ripple=rank==2?1:.65f;s.caption=rank==2?"连雨遮花影，须待水声收。":"疏雨点空阶，涟漪各自开。";break;
   case 3:s.ripple=.55f;s.birds+=rank==2?3:1;s.caption="一声惊宿鹭，白影出芦汀。";break;
   case 4:s.wind=rank==2?1.5f:.75f;s.curtain=Math.Min(1,s.curtain+(rank==2?.9f:.48f));if(rank==2)s.petals+=2;if(s.letter)s.loosePaper=rank==2?1:.25f;s.caption=rank==2?"长风卷帘起，残红落旧笺。":"微风轻揭帘，花仍在枝头。";break;
  }
 }
 public static SceneMoment At(Stone[] notes,int chapter,int beat){var s=Begin(chapter);for(int i=0;i<=beat&&i<8;i++)s=Step(s,notes[i],chapter);return s;}
 public static int Outcome(SceneMoment s){return (s.reflection?1:0)|(s.letter?2:0)|(s.curtain>.7f&&s.petals==0?4:0)|(s.reply?8:0);}
 public static string Verse(int outcome){return (outcome&2)!=0?"花间拾得无名纸，半是春痕半是书。":(outcome&4)!=0?"帘外一枝留晓色，微风不忍动残红。":(outcome&8)!=0?"不问归期长与短，隔溪犹有一声回。":"水静方知山色近，声疏才觉暮云闲。";}
 public static bool GentleWindow(Stone[] b){int rain=0,wind=0;for(int i=0;i<8;i++){if(b[i].Empty)continue;if(b[i].echo>0||b[i].rank!=1)return false;if(b[i].tone==2)rain++;else if(b[i].tone==4)wind++;else return false;}if(rain!=2||wind!=2)return false;int first=-1;for(int i=0;i<8;i++)if(!b[i].Empty&&b[i].tone==2){if(first<0)first=i;else if(i-first!=4)return false;}for(int i=0;i<8;i++)if(!b[i].Empty&&b[i].tone==4&&(i-first+8)%4!=2)return false;return true;}
}
}

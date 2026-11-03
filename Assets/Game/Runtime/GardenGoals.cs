using System;
namespace CiJing {
// Linear performances: success is derived from the resulting world, not an
// expected array of beat positions. Material limits remain authored per scene.
public static class GardenGoals {
 public static bool Check(Stone[] notes,int level,out string reason){
  int mass=0;for(int i=0;i<8;i++){var n=notes[i];if(n.Empty)continue;if(n.echo>0||n.tone!=2&&n.tone!=4){reason="此庭只听檐雨与窗风，余音暂留行囊。";return false;}mass+=n.Mass;}
  var s=SceneScore.At(notes,1,7);
  if(level==3){if(mass!=3||s.rainShorts!=3||s.rainLongs!=0){reason="三处檐水，各须一滴短雨；连雨听不清滴数。";return false;}if(s.crowdedRain){reason="两滴雨接得太近，留一拍空，听清各自的余波。";return false;}if(!s.reflection){reason="末滴余波未定，曲尾再留一点静处。";return false;}}
  else if(level==4){if(mass!=4||s.windLongs!=2){reason="两阵长风才能掠过深枝；四份羽音可合成两声。";return false;}if(s.crowdedWind){reason="前一阵风尚未退，后一阵已来；枝影还辨不清。";return false;}if(s.wind>.35f||s.still<1){reason="帘已起，花瓣还在飘；曲尾须候风歇。";return false;}}
  else {if(mass!=4||s.rainShorts!=2||s.rainLongs!=0||s.windMass!=2){reason="两份角作两滴檐雨，两份羽作长风或两次微风。";return false;}if(!s.heardBehind){reason="帘动之前先听一滴雨，才知昨夜与今晨的分别。";return false;}if(s.crowdedRain){reason="雨滴相接，窗前仍听不清；把两滴稍稍疏开。";return false;}if(s.curtain<.9f){reason="帘角尚低，还看不全枝头。";return false;}if(s.wind>.35f){reason="帘已起，风尚未歇；再候一拍。";return false;}if(!s.reflection){reason="帘已起，水波未定；留白之后再观花。";return false;}if(s.loosePaper>.4f){reason="纸角仍在风中，稍候便可展读。";return false;}}
  reason=level==5?"帘外已明，点观花，比照先前所闻。":"声歇景明，庭中已有新迹。";return true;
 }
 public static string Goal(int level){return level==3?"听清三滴檐雨，待水定见影。":level==4?"两阵长风掠枝，待落瓣停稳。":"听过帘前雨，卷帘后静看花影。";}
 public static string Progress(SceneMoment s){return "檐雨 "+s.rainShorts+" 滴　帘"+(s.curtain>=.9f?"已起":"尚垂")+"　水影"+(s.reflection?"已显":"待静");}
}
}

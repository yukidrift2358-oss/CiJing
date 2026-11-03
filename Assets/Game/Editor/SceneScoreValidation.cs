using System;
using System.IO;
using UnityEngine;
namespace CiJing.EditorTools {
public static class SceneScoreValidation {
 static int checks;static void Check(bool value,string why){checks++;if(!value)throw new Exception("Scene score: "+why);}
 public static void Run(){checks=0;var notes=new Stone[16];notes[0]=new Stone(2,2);Check(!SceneScore.At(notes,1,1).reflection,"continuous rain must hide reflection");Check(SceneScore.At(notes,1,4).reflection,"silence must settle rain");
  notes=new Stone[16];notes[0]=new Stone(2);notes[4]=new Stone(2);notes[2]=new Stone(4,2);var petal=SceneScore.At(notes,1,7);Check(petal.letter&&petal.petals==2,"long wind reveals letter");Check(!SceneScore.At(notes,1,2).letter,"letter cannot appear before falling settles");string reason;Check(VerseRules.Check(notes,5,out reason),"petal route solves");
  notes[2]=new Stone(4);notes[6]=new Stone(4);var flower=SceneScore.At(notes,1,7);Check(flower.petals==0&&!flower.letter&&flower.curtain>.9f,"short breezes preserve flowers");Check(VerseRules.Check(notes,5,out reason),"flower route solves");Check(SceneScore.Outcome(flower)!=SceneScore.Outcome(petal),"routes have distinct outcomes");
  notes[7]=new Stone(0);Check(!VerseRules.Check(notes,5,out reason),"extra stroke rejected");notes[7]=default(Stone);notes[4]=default(Stone);Check(!VerseRules.Check(notes,5,out reason),"missing rain rejected");
  for(int n=0;n<9;n++){var target=new Stone[16];Array.Copy(Content.Levels[n].target,target,8);for(int i=0;i<8;i++){var a=SceneScore.At(target,Content.Levels[n].chapter,i);var b=SceneScore.Begin(Content.Levels[n].chapter);for(int j=0;j<=i;j++)b=SceneScore.Step(b,target[j],Content.Levels[n].chapter);Check(JsonUtility.ToJson(a)==JsonUtility.ToJson(b),"seek must equal full prefix");}}
  var compound=new Stone[16];compound[0]=new Stone(0,2){echo=3};var result=SceneScore.At(compound,0,0);Check(result.rain>0&&result.boat>.12f,"compound drives both meanings");
  var displaced=SceneScore.Step(petal,new Stone(4,2),1);Check(displaced.loosePaper>.4f,"strong wind must move exposed paper");for(int i=0;i<3;i++)displaced=SceneScore.Step(displaced,default(Stone),1);Check(displaced.loosePaper<.4f,"paper must settle in silence");
  var boat=SceneScore.Begin(0);for(int i=0;i<3;i++)boat=SceneScore.Step(boat,new Stone(0,2),0);Check(boat.overshot,"long oars can pass landing");
  var data=new SaveData{composition=compound,compositionScene=SceneScore.Outcome(result)};var back=JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(data));Check(back.composition[0].echo==3&&back.compositionScene==data.compositionScene,"scene composition roundtrip");
  File.WriteAllText("Validation/scene-score-results.json","{\"passed\":true,\"assertions\":"+checks+",\"twoRoutes\":true,\"prefixReplay\":true}");Debug.Log("CIJING_SCENE_RULES_PASS "+checks);
 }
}
}

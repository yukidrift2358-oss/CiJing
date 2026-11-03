using System;
using System.Linq;
using System.Collections.Generic;
namespace CiJing {
public static class VerseRules {
 public static string[] Evidence(int n){switch(n){
 case 3:return new[]{"檐下三处积水，每处只落一声短角。","两次间隔各空两拍，另一次空一拍；首音不必固定。","把八拍看作一圈，最后一滴与第一滴之间也有间隔。"};
 case 4:return new[]{"两道枝影相隔半圈，各需一声长羽。","两次起音之间相隔四拍。","四颗短羽应分成两对，空位保持安静。"};
 case 5:return new[]{"两滴短角相隔半圈。","一声长羽居两滴雨的正中；也可拆成两声短羽，分置两侧中点。","长风可落花见笺；两次微风可卷帘留花。风后的停顿不要填满。"};
 case 6:return new[]{"前半句有商和角，后半句倒过来回答。","四声均为短音，分别落在一、三、五、七拍。","相对的两岸不是照抄：把前两声倒序，便是后两声。"};
 case 7:return new[]{"一声长宫在前三拍内启程，下一拍留空。","第一声短羽在宫之后两拍，第二声短羽再隔三拍。","两声羽各有所答，须分置，勿合。"};
 case 8:return new[]{"上半句：长宫起，空一拍，短商落，再空一拍。","下半句保留上半句的长短与留白，整体提高一个五声音阶级。","宫的下一级是商，商的下一级是角。两半句各用三份材料。"};
 default:return new[]{Content.Levels[n].clue};}}
 public static string Guide(int n){return string.Join("\n",Evidence(n));}
 static int[] At(Stone[] b,int tone,int rank){return Enumerable.Range(0,8).Where(i=>b[i].tone==tone&&b[i].rank==rank).ToArray();}
 static int Count(Stone[] b){return b.Take(8).Count(x=>!x.Empty);}
 public static bool Check(Stone[] b,int n,out string reason){reason="";if(b.Take(8).Any(x=>x.echo>0)){reason="此景须用单音。合声可拆，藏曲另有回响。";return false;}if(n<3){var l=Content.Levels[n];for(int i=0;i<8;i++)if(b[i].rank!=l.target[i].rank||(!b[i].Empty&&b[i].tone!=l.target[i].tone)){reason="再比较起音的位置、长短与留白。";return false;}return true;}
 bool ok=false;switch(n){
 case 3:var rain=At(b,2,1);if(rain.Length!=3||Count(b)!=3){reason="檐下需要三滴短角；其他声音先留在行囊。";break;}var gaps=new int[3];for(int i=0;i<3;i++)gaps[i]=(rain[(i+1)%3]-rain[i]+8)%8-1;Array.Sort(gaps);ok=gaps.SequenceEqual(new[]{1,2,2});reason="三次停顿应为两拍、两拍、一拍，记得检查首尾。";break;
 case 4:var wind=At(b,4,2);ok=wind.Length==2&&Count(b)==2&&wind[1]-wind[0]==4;reason="两声都应是长羽，起音相隔半圈。";break;
 case 5:if(SceneScore.GentleWindow(b)){reason="微风卷帘，枝上留花。";return true;}var r=At(b,2,1);var w=At(b,4,2);ok=r.Length==2&&w.Length==1&&Count(b)==3&&r[1]-r[0]==4&&((w[0]-r[0]+8)%8==2||(w[0]-r[0]+8)%8==6)&&b[(w[0]+1)%8].Empty;reason="检查雨声是否相对，长风是否恰在正中，以及风后的留白。";break;
 case 6:ok=Count(b)==4&&Enumerable.Range(0,8).Where(i=>i%2==1).All(i=>b[i].Empty)&&b[0].rank==1&&b[2].rank==1&&b[4].rank==1&&b[6].rank==1&&b[0].tone!=b[2].tone&&new[]{1,2}.Contains(b[0].tone)&&new[]{1,2}.Contains(b[2].tone)&&b[4].tone==b[2].tone&&b[6].tone==b[0].tone;reason="四个短音落在奇数拍；后半句需要倒过来回答前半句。";break;
 case 7:var g=At(b,0,2);var y=At(b,4,1);ok=g.Length==1&&y.Length==2&&Count(b)==3&&g[0]<=2&&y.Contains(g[0]+2)&&y.Contains(g[0]+5)&&b[g[0]+1].Empty;reason="长宫在前三拍起，隔两拍羽答，第二声羽再隔三拍。";break;
 case 8:ok=Count(b)==4&&b[0].tone==0&&b[0].rank==2&&b[2].tone==1&&b[2].rank==1&&b[4].tone==1&&b[4].rank==2&&b[6].tone==2&&b[6].rank==1&&new[]{1,3,5,7}.All(i=>b[i].Empty);reason="前句宫长、商短；后句提高一级，商长、角短，留白保持。";break;
 }if(ok)reason="声与景合，归路已明。";return ok;}
}
}

using System;
using System.Linq;
using System.Collections.Generic;
namespace CiJing {
public static class VerseRules {
 public static string[] Evidence(int n){switch(n){
 case 3:return new[]{"三处檐水，各落一滴短角。","雨滴之间至少留一拍，免得余波相接。","曲尾等水面平静，倒影出现即可；拍位不必固定。"};
 case 4:return new[]{"两阵长羽掠过深枝，花瓣随风而落。","待前一阵风退去，再起后一阵；曲尾也须候风歇。","四颗短羽合成两声长羽。枝影辨清、落瓣停稳，便可续行。"};
 case 5:return new[]{"帘未动前先听檐雨，两滴短角之间留空。","两份羽音可合为长风，也可分作两次微风；帘起即可，不限中点。","雨收、风歇、水影清楚，再点观花。长风见笺，微风留一枝残红。"};
 case 6:return new[]{"前半句有商和角，后半句倒过来回答。","四声均为短音，分别落在一、三、五、七拍。","相对的两岸不是照抄：把前两声倒序，便是后两声。"};
 case 7:return new[]{"一声长宫在前三拍内启程，下一拍留空。","第一声短羽在宫之后两拍，第二声短羽再隔三拍。","两声羽各有所答，须分置，勿合。"};
 case 8:return new[]{"上半句：长宫起，空一拍，短商落，再空一拍。","下半句保留上半句的长短与留白，整体提高一个五声音阶级。","宫的下一级是商，商的下一级是角。两半句各用三份材料。"};
 default:return new[]{Content.Levels[n].clue};}}
 public static string Guide(int n){return string.Join("\n",Evidence(n));}
 static int[] At(Stone[] b,int tone,int rank){return Enumerable.Range(0,8).Where(i=>b[i].tone==tone&&b[i].rank==rank).ToArray();}
 static int Count(Stone[] b){return b.Take(8).Count(x=>!x.Empty);}
 public static bool Check(Stone[] b,int n,out string reason){reason="";if(n>=3&&n<=5)return GardenGoals.Check(b,n,out reason);if(b.Take(8).Any(x=>x.echo>0)){reason="此景须用单音。合声可拆，藏曲另有回响。";return false;}if(n<3){var l=Content.Levels[n];for(int i=0;i<8;i++)if(b[i].rank!=l.target[i].rank||(!b[i].Empty&&b[i].tone!=l.target[i].tone)){reason="再比较起音的位置、长短与留白。";return false;}return true;}
 bool ok=false;switch(n){
 case 6:ok=Count(b)==4&&Enumerable.Range(0,8).Where(i=>i%2==1).All(i=>b[i].Empty)&&b[0].rank==1&&b[2].rank==1&&b[4].rank==1&&b[6].rank==1&&b[0].tone!=b[2].tone&&new[]{1,2}.Contains(b[0].tone)&&new[]{1,2}.Contains(b[2].tone)&&b[4].tone==b[2].tone&&b[6].tone==b[0].tone;reason="四个短音落在奇数拍；后半句需要倒过来回答前半句。";break;
 case 7:var g=At(b,0,2);var y=At(b,4,1);ok=g.Length==1&&y.Length==2&&Count(b)==3&&g[0]<=2&&y.Contains(g[0]+2)&&y.Contains(g[0]+5)&&b[g[0]+1].Empty;reason="长宫在前三拍起，隔两拍羽答，第二声羽再隔三拍。";break;
 case 8:ok=Count(b)==4&&b[0].tone==0&&b[0].rank==2&&b[2].tone==1&&b[2].rank==1&&b[4].tone==1&&b[4].rank==2&&b[6].tone==2&&b[6].rank==1&&new[]{1,3,5,7}.All(i=>b[i].Empty);reason="前句宫长、商短；后句提高一级，商长、角短，留白保持。";break;
 }if(ok)reason="声与景合，归路已明。";return ok;}
}
}

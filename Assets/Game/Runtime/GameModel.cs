using System;
using System.Collections.Generic;
using System.Linq;
namespace CiJing {
[Serializable] public struct Stone {
 public int tone, rank,echo;
 public Stone(int t,int r=1){tone=t;rank=r;echo=0;}
 public bool Empty { get {return rank==0;} }
 public int Mass {get{return Empty?0:1<<(rank-1);}}
 public override string ToString(){return Empty?"·":echo>0?"宫商角徵羽"[tone]+"与"+"宫商角徵羽"[echo-1]+"·和":"宫商角徵羽"[tone]+(rank==1?"·短":"·长");}
}
public sealed class VerseLevel {
 public int chapter; public string title, intro, clue, hint, after; public Stone[] target;
 public VerseLevel(int c,string n,string i,string cl,string h,string a,params int[] pattern){chapter=c;title=n;intro=i;clue=cl;hint=h;after=a;target=pattern.Select(x=>x==0?default(Stone):new Stone((Math.Abs(x)-1)%5,x<0?2:1)).ToArray();}
}
public static class Content {
 public static readonly string[] Chapters={"溪亭回舟","雨后海棠","月满西楼"};
 public static readonly string[] Poems={"常记溪亭日暮，沉醉不知归路。\n兴尽晚回舟，误入藕花深处。\n争渡，争渡，惊起一滩鸥鹭。","昨夜雨疏风骤，浓睡不消残酒。\n试问卷帘人，却道海棠依旧。\n知否，知否？应是绿肥红瘦。","红藕香残玉簟秋。轻解罗裳，独上兰舟。\n云中谁寄锦书来？雁字回时，月满西楼。\n花自飘零水自流。一种相思，两处闲愁。\n此情无计可消除，才下眉头，却上心头。"};
 public static readonly string[] Notes={"藕花即荷花。词从日暮游兴写到回舟迷路，最后以水鸟飞起收束。\n本章的音石机关与行船路线为游戏改编。","雨疏与风骤写出不同的动静；绿肥红瘦写花叶的消长。\n本章的目标是看见雨后的变化，不是让落花重新开满枝头。","锦书指书信；雁字指雁群的行列，并引出对音讯的期待。\n本章两处空间的声音呼应为词意改编，不是传记事件复原。"};
 public static readonly VerseLevel[] Levels={
 new VerseLevel(0,"一 · 离岸","日暮溪亭远，轻舟一棹开。","岸边留下两个桨印：第一处是宫，第五处是商。其余水面安静。","在第 1 位放宫·短，第 5 位放商·短，其余留空。","两声桨响，小舟离开了溪亭。",1,0,0,0,2,0,0,0),
 new VerseLevel(0,"二 · 荷间水道","荷深遮去路，水细认前痕。","第一处宫的水痕连绵，需一次长划；第五处商只留一个短印。","把两颗宫·短合并为宫·长，放在第 1 位；商·短放第 5 位。","小舟从荷叶间穿过，远处透出一线晚霞。",-1,0,0,0,2,0,0,0),
 new VerseLevel(0,"三 · 争渡","争渡声方急，沙禽已满汀。","宫在第一、三处分别响起；徵在第六处延续。留心：两个宫要分开。","第 1、3 位放宫·短；两颗徵合成徵·长，放第 6 位。","水声惊动了浅滩，鸥鹭掠过天际。你终于看见开阔的水面。",1,0,1,0,0,-4,0,0),
 new VerseLevel(1,"一 · 夜雨","檐雨三更歇，空庭数滴闻。","角是雨滴的清音：第一、四、七处各有一滴，其余停息。","角·短分别放第 1、4、7 位。不要把雨滴合成长声。","雨滴之间有空隙，才听得见庭院里的风。",3,0,0,3,0,0,3,0),
 new VerseLevel(1,"二 · 风骤","疏枝摇夜影，风过不成眠。","羽是风声：第二处连绵，第六处也连绵。四颗短羽可以合成两颗长羽。","两对羽·短分别合并，得到两颗羽·长，放第 2、6 位。","风越过窗棂，枝头的花瓣轻轻落下。",0,-5,0,0,0,-5,0,0),
 new VerseLevel(1,"三 · 卷帘","卷帘花已瘦，满径见新青。","雨角落在第一、五处；风羽在第三处延续。雨与风之间，要留下空隙。","角·短放第 1、5 位；羽·长放第 3 位。其余留空。","花少了一些，叶色却更浓了。眼前的海棠，并不与昨日相同。",3,0,-5,0,3,0,0,0),
 new VerseLevel(2,"一 · 月上西楼","月白楼前水，遥声渡晚汀。","第一处商发问，第五处商回应。相同的声音，要留在不同的位置。","两颗商·短分别放第 1、5 位，不需要合成。","隔着几拍水声，另一处回答了你。",2,0,3,0,3,0,2,0),
 new VerseLevel(2,"二 · 锦书未至","锦字随流水，余音过小桥。","第二处宫延续，第六处羽轻答。留白让声音能够传远。","宫·长放第 2 位；羽·短放第 6 位。", "月色渐满，水面仍向远处延伸。",0,-1,0,5,0,0,5,0),
 new VerseLevel(2,"三 · 两处闲愁","两岸各无语，隔波一曲回。","宫在第一、五处延续；商在第三、七处轻答。两岸都需要自己的声音。","第 1、5 位各放宫·长，第 3、7 位各放商·短，其余留空。","两处声音在同一圈水纹里相遇。留给你的空白，也可以写下一句。",-1,0,2,0,-2,0,3,0)
 };
}
[Serializable] public class SaveData { public int secrets;public int version=1,rulesRevision=3,level,unlocked; public bool finished; public Stone[] cells; public Stone[] composition; public int[] evidence=new int[9];public float musicVolume=.35f;public float volume=.65f; }
public sealed class Board {
 public Stone[] cells=new Stone[16]; readonly Stack<Stone[]> history=new Stack<Stone[]>();
 public void Load(VerseLevel level){Array.Clear(cells,0,16);history.Clear();int k=8;foreach(var t in level.target)for(int j=0;j<t.Mass;j++)cells[k++]=new Stone(t.tone);}
 public void Restore(Stone[] value){if(value==null||value.Length!=16)throw new ArgumentException("Board length");cells=(Stone[])value.Clone();history.Clear();}
 public bool Move(int from,int to){if(from<0||from>=16||to<0||to>=16||from==to||cells[from].Empty)return false;var a=cells[from];var b=cells[to];if(!b.Empty&&(a.echo>0||b.echo>0||a.tone!=b.tone||a.rank!=b.rank||a.rank>=2))return false;history.Push((Stone[])cells.Clone());cells[to]=b.Empty?a:new Stone(a.tone,a.rank+1);cells[from]=default(Stone);return true;}
 public bool Weave(int from,int to){if(from<0||to<0||from>=16||to>=16||from==to)return false;var a=cells[from];var b=cells[to];if(a.rank!=1||b.rank!=1||SecretSongs.Recipe(a.tone,b.tone)<0)return false;history.Push((Stone[])cells.Clone());cells[to]=new Stone(Math.Min(a.tone,b.tone),2){echo=Math.Max(a.tone,b.tone)+1};cells[from]=default(Stone);return true;}
 public bool Split(int i){if(i<0||i>=16||cells[i].rank!=2)return false;int spare=Array.FindIndex(cells,8,8,s=>s.Empty);if(spare<0)return false;history.Push((Stone[])cells.Clone());var old=cells[i];cells[i]=new Stone(old.tone);cells[spare]=new Stone(old.echo>0?old.echo-1:old.tone);return true;}
 public bool Undo(){if(history.Count==0)return false;cells=history.Pop();return true;}
 public bool Matches(VerseLevel l){string reason;return VerseRules.Check(cells,Array.IndexOf(Content.Levels,l),out reason);}
 public int Mass {get{return cells.Sum(s=>s.Mass);}}
}
}

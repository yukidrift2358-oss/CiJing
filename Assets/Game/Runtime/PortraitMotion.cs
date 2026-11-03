using UnityEngine;
using UnityEngine.UI;
namespace CiJing {
public sealed class PortraitMotion:MonoBehaviour {
 public Text dialogue;public bool reduceMotion;public string line="";int count;float clock;
 public bool Typing{get{return count<line.Length;}}
 public void SetLine(string value){line=value;count=0;clock=.22f;dialogue.text="";}
 public void FinishLine(){count=line.Length;dialogue.text=line;clock=0;}
 void Update(){if(!Typing)return;clock-=Time.unscaledDeltaTime;if(clock>0)return;char c=line[count++];dialogue.text=line.Substring(0,count);clock="。！？；".IndexOf(c)>=0?.42f:"，、：".IndexOf(c)>=0?.19f:.045f;}
}
public static class StoryText {
 public static string Speaker(int page){return page==1?"画 中 人":"卷 中 记";}
 public static string[] Intro(int c){if(c==0)return new[]{
 "展卷时，一封无款的信落在掌心。纸上只写着：归时，留一盏灯。溪亭还在，亭前的水路却断在荷叶深处。",
 "日暮的路，眼睛未必认得。你听，桨过水面，有轻重，也有停歇；循着那些旧痕，舟便知道往哪里去。",
 "岸边横着旧桨，灯下压着半张词笺。你将它展平，想起信中那个“归”字，便拨开水上的第一声。"};if(c==1)return new[]{
 "舟到花阴，灯还亮着，亭中却无人。案上另有一笺，被雨水洇去半行，只剩“海棠”二字清楚。",
 "昨夜风急，我也以为花该落尽了。天亮不敢卷帘，偏又想知道——枝头如今是什么模样？",
 "檐下的水钵存着余雨，一段折枝倚在窗边。你不急着补回纸上缺字，先俯身听那几声未尽的滴水。"};return new[]{
 "花笺背面，淡墨写着“西楼”。你循水走到月下，看见对岸也有一盏灯；两处灯影之间，横着尚未展开的回信。",
 "隔水传声，哪能字字如初？有时颠倒了次序，有时换了高低。只要那一声仍有人接着，信便没有失落。",
 "你把书笺轻轻放在岸边。水面空着几拍，仿佛话到嘴边，仍给远处的人留了一次开口的余地。"};}
 public static string[] Outro(int c){if(c==0)return new[]{
 "最后一声桨响落下，荷叶间透出暮光。浅滩上的鸥鹭惊起，归舟灯在水面晃了晃，终于照见完整的岸线。",
 "原来归路一直在。只是来时只顾看花，忘了听水。今夜这盏灯，你替后来的人留下吧。",
 "你把灯挂回亭前。风吹动灯下的薄笺，露出一笔新添的枝影；远处檐声渐起，像另一页词正等人展读。"};if(c==1)return new[]{
 "帘子卷起时，一片落瓣停在窗沿。花确实少了，叶色却比昨夜深；纸上被水洇开的地方，也渐渐显出细细的脉络。",
 "我总想着把昨日留住，倒没看见今日的枝叶。花谢有花谢的去处，何必把它一瓣瓣送回枝头。",
 "你将落瓣夹进词笺，没有补上那半行旧字。帘外雨声止住，月色沿水而来，照见纸背的“西楼”。"};return new[]{
 "两岸的声音接成一曲，灯影在水中慢慢靠拢。你展信来看，纸上并无别人的答语，只有月光压住的一角空白。",
 "来信的人，未必知道你何时读到。可你已走过这一路，总有一句话，值得寄回去。",
 "你提笔，在空白处写下今夜所闻。归舟、檐雨、隔岸的回声，都留在卷里；余下的水面，便由你续成自己的曲子。"};}
}
}

using UnityEngine;
using UnityEngine.UI;
namespace CiJing {
public sealed class PortraitMotion:MonoBehaviour {
 public RectTransform head,body;public RawImage headImage,otherEye;public Texture normal,blink;public Text dialogue;public string line="";public bool reduceMotion,forceBlink;float shown;int last=-1;Vector2 headHome,bodyHome;

 public bool Typing{get{return shown<line.Length;}}
 public void SetLine(string value){line=value;shown=0;last=-1;}
 public void FinishLine(){shown=line.Length;dialogue.text=line;}
 void Update(){shown=Mathf.Min(line.Length,shown+Time.unscaledDeltaTime*24);int count=(int)shown;if(count!=last){last=count;dialogue.text=line.Substring(0,count);}}

}
public static class StoryText {
 public static string[] Intro(int c){if(c==0)return new[]{"你收到一封没有落款的信，上面只写着：回到溪亭。可画中的归路已经散了。","画中人：有人把归途藏进了词里。找到船桨、水痕和灯上的词句，才能接上回舟的路。","沿卷轴向右寻找。每读懂一景，散落的画面就会回来一点。"};if(c==1)return new[]{"小舟靠岸了，庭中却没有等信的人。帘外的雨声，藏着下一页词笺的去处。","画中人：雨滴之间有间隔，风声却会延续。先分别听清，再把它们放在一起。","这一次，纸上不再记着每颗音石的位置。檐水与枝影，会告诉你声音之间的关系。"};return new[]{"你在落花书笺上找到另一句话：月满西楼。信的回声，从对岸传来。","画中人：隔岸的声音，有时倒序相答，有时换一个音高。听见相同，也要留心不同。","请给回答留出位置。此卷的最后一句，仍等着你自己来写。"};}
 public static string[] Outro(int c){if(c==0)return new[]{"桨声停了，鸥鹭掠过浅滩。刚才遮住归路的荷叶，也成了值得记住的风景。","画中人：现在再读这几句，你已经走过它们的起落。"};if(c==1)return new[]{"帘子卷起。落瓣没有回到枝上，叶色却比昨日更显眼。","画中人：读到绿肥红瘦时，记住眼前发生的变化。"};return new[]{"两岸的乐句终于相接，月下的空白仍留在那里。","画中人：接下来不再只有一种解法。你可以留下一个音，也可以留下一段安静。"};}
}
}

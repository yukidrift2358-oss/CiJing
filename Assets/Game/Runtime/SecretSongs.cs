using System;
using System.Collections;
using System.Linq;
using UnityEngine;
namespace CiJing {
public static class SecretSongs {
 public static readonly int[] Starts={0,1,2},Replies={4,6,4};
 public static readonly int[][] Pairs={new[]{0,2},new[]{1,4},new[]{2,3}};
 public static readonly string[] Names={"疏雨","雁回","落花"};
 public static readonly string[] Verses={"一桨分烟水，疏声到晚汀。","雁影过寒渚，归书落小窗。","花落无人扫，春声在石间。"};
 public static int Recipe(int a,int b){for(int i=0;i<3;i++)if(Pairs[i].Contains(a)&&Pairs[i].Contains(b)&&a!=b)return i;return -1;}
 public static int Resolve(Stone[] cells){for(int n=0;n<3;n++){var s=cells[Starts[n]];var reply=cells[Replies[n]];if(s.rank==2&&s.echo>0&&Recipe(s.tone,s.echo-1)==n&&reply.rank==1&&reply.echo==0&&reply.tone==Pairs[n][0]&&Enumerable.Range(0,8).All(i=>i==Starts[n]||i==Replies[n]||cells[i].Empty))return n;}return -1;}
}
public sealed partial class GameApp {
 void ArmWeave(){if(playing||modal||completing)return;if(selected<0||board.cells[selected].rank!=1){SetFeedback("择一枚短音，点“合声”以待另一音。宫角、商羽、角徵可相和。");return;}weaving=true;SetFeedback("已候和声，请择另一短音。宫角、商羽、角徵可合，亦可拆。");}
 void ShowSecretSongs(){string text="异音相和，别有余声。（以下题句为本作自撰）\n宫角 · 疏雨　商羽 · 雁回　角徵 · 落花\n\n藏曲：入“自在谱曲”，依下列拍位布合声与低音短答，其余留白。\n疏雨：一、五拍；雁回：二、七拍；落花：三、五拍。\n试听一圈，题句自现；合声可拆，错步可撤。\n";for(int i=0;i<3;i++)text+="\n"+SecretSongs.Names[i]+"："+((save.secrets&(1<<i))!=0?SecretSongs.Verses[i]:"墨痕未显");OpenModal("拾 遗 · 藏 曲",text,"收卷",CloseModal);var body=modal.GetComponentsInChildren<UnityEngine.UI.Text>().First(t=>t.text==text);body.fontSize=20;body.lineSpacing=1.05f;}
 void FinishSecretSong(){int n=SecretSongs.Resolve(board.cells);if(n<0){SetFeedback("曲罢水犹静。藏曲须依拍位合声、低音相答，其余留白。详见藏曲笺。");return;}bool fresh=(save.secrets&(1<<n))==0;save.secrets|=1<<n;Persist();SetFeedback((fresh?"新得题句 · ":"重听 · ")+SecretSongs.Names[n]+"："+SecretSongs.Verses[n]);}
 IEnumerator TestSecretSongs(string dir){save.secrets=0;for(int n=0;n<3;n++){
  StartFree();yield return null;board.Restore(new Stone[16]);int a=SecretSongs.Pairs[n][0],b=SecretSongs.Pairs[n][1];board.cells[8]=new Stone(a);board.cells[9]=new Stone(b);board.cells[10]=new Stone(a);UpdateSlots();
  ClickSlotForTest(8);ArmWeave();ClickSlotForTest(9);if(board.cells[9].echo!=b+1||board.Mass!=3)throw new Exception("Compound merge failed");
  var snapshot=(Stone[])board.cells.Clone();if(!board.Split(9)||board.cells[8].tone!=b||!board.Undo()||board.cells[9].echo!=b+1)throw new Exception("Compound split undo failed");
  string encoded=JsonUtility.ToJson(new SaveData{composition=snapshot});if(JsonUtility.FromJson<SaveData>(encoded).composition[9].echo!=b+1)throw new Exception("Compound save failed");
  UpdateSlots();ClickSlotForTest(9);ClickSlotForTest(SecretSongs.Starts[n]);ClickSlotForTest(10);ClickSlotForTest(SecretSongs.Replies[n]);if(SecretSongs.Resolve(board.cells)!=n)throw new Exception("Hidden melody failed");
  int extra=Enumerable.Range(0,8).First(i=>i!=SecretSongs.Starts[n]&&i!=SecretSongs.Replies[n]);board.cells[extra]=new Stone(a);if(SecretSongs.Resolve(board.cells)>=0)throw new Exception("Extra voice accepted");board.cells[extra]=default(Stone);Play();yield return new WaitForSeconds(6.8f);if((save.secrets&(1<<n))==0)throw new Exception("Playback did not unlock verse");UpdateSlots();Capture(System.IO.Path.Combine(dir,"secret-song-"+n+".png"));yield return null;
 }ShowSecretSongs();Capture(System.IO.Path.Combine(dir,"secret-book.png"));CloseModal();if((save.secrets&7)!=7)throw new Exception("Hidden verses not recorded");Debug.Log("CIJING_SECRETS_PASS 3 compound recipes, split undo, save, 3 hidden melodies");}
}
}

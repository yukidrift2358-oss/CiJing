using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
namespace CiJing {
public static class FilmClock {public static bool Active;public static float Delta{get{return Active?1f/15:Time.unscaledDeltaTime;}}}
public sealed partial class GameApp {
 System.Diagnostics.Process encoder;StreamWriter filmEvents;RenderTexture filmRT;Texture2D filmPixels;int filmFrame;string filmRoot,filmName;StreamWriter filmMarks;
 void FilmSound(Stone stone){if(filmEvents!=null)filmEvents.WriteLine(filmFrame+","+stone.tone+","+stone.rank+","+stone.echo);}
 void FilmStart(string name){filmName=name;filmFrame=0;filmMarks=new StreamWriter(Path.Combine(filmRoot,name+"-marks.csv"));filmMarks.WriteLine("frame,phase");filmMarks.WriteLine("0,start");filmEvents=new StreamWriter(Path.Combine(filmRoot,name+".csv"));filmEvents.WriteLine("frame,tone,rank,echo");var exe=Path.GetFullPath(Application.dataPath+"/../../../CiJing/Tools/AudioLib/imageio_ffmpeg/binaries/ffmpeg-win-x86_64-v7.1.exe");encoder=System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(exe,"-y -loglevel error -f rawvideo -pix_fmt rgb24 -s 1280x720 -r 15 -i pipe:0 -vf vflip -c:v libx264 -preset ultrafast -crf 18 -pix_fmt yuv420p \""+Path.Combine(filmRoot,name+".mp4")+"\""){UseShellExecute=false,RedirectStandardInput=true,CreateNoWindow=true});}
 void FilmStop(){encoder.StandardInput.Close();encoder.WaitForExit();if(encoder.ExitCode!=0)throw new Exception("Film encoder failed");filmEvents.Close();filmMarks.Close();filmEvents=null;encoder.Dispose();encoder=null;}
 IEnumerator FilmHold(float seconds){int count=Mathf.CeilToInt(seconds*15);for(int i=0;i<count;i++){yield return null;Canvas.ForceUpdateCanvases();var cam=Camera.main;var old=cam.targetTexture;cam.targetTexture=filmRT;cam.Render();var previous=RenderTexture.active;RenderTexture.active=filmRT;filmPixels.ReadPixels(new Rect(0,0,1280,720),0,0);filmPixels.Apply();byte[] bytes=filmPixels.GetRawTextureData();encoder.StandardInput.BaseStream.Write(bytes,0,bytes.Length);RenderTexture.active=previous;cam.targetTexture=old;filmFrame++;}}
 IEnumerator FilmStory(){if(!portrait)yield break;for(int i=0;i<3;i++){yield return FilmHold(7);portrait.FinishLine();yield return FilmHold(1);NextStory();}yield return FilmHold(.5f);}
 IEnumerator FilmAll(){FilmClock.Active=true;Time.captureFramerate=15;Application.targetFrameRate=60;filmRoot=Path.GetFullPath(Application.dataPath+"/../../CiJing-Video-v1/raw");Directory.CreateDirectory(filmRoot);filmRT=new RenderTexture(1280,720,24);filmPixels=new Texture2D(1280,720,TextureFormat.RGB24,false);yield return null;
  FilmStart("menu");yield return FilmHold(5);FilmStop();
  for(int n=0;n<9;n++){save.evidence[n]=0;LoadLevel(n);yield return null;FilmStart("level-"+n);yield return FilmStory();filmMarks.WriteLine(filmFrame+",explore");
   for(int prop=0;prop<3;prop++){ExploreArtworkForTest(prop);var e=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);UnityEngine.EventSystems.ExecuteEvents.Execute(propViews[prop].gameObject,e,UnityEngine.EventSystems.ExecuteEvents.pointerEnterHandler);yield return FilmHold(2.2f);UnityEngine.EventSystems.ExecuteEvents.Execute(propViews[prop].gameObject,e,UnityEngine.EventSystems.ExecuteEvents.pointerExitHandler);}
   ShowEvidenceBook();yield return FilmHold(3);CloseModal();yield return null;OpenExploration();CloseModal();yield return null;yield return FilmHold(1.5f);
   filmMarks.WriteLine(filmFrame+",board");var target=Content.Levels[n].target;
   for(int i=0;i<8;i++)for(int k=0;k<target[i].Mass;k++){int from=Array.FindIndex(board.cells,8,8,s=>!s.Empty&&s.tone==target[i].tone);ClickSlotForTest(from);yield return FilmHold(.45f);ClickSlotForTest(i);yield return FilmHold(.65f);}
   if(n==1){selected=0;SplitSelected();yield return FilmHold(2);Undo();yield return FilmHold(2);ClickSlotForTest(4);ClickSlotForTest(3);Play();yield return FilmHold(6.2f);ClickSlotForTest(3);ClickSlotForTest(4);yield return FilmHold(1.5f);}
   filmMarks.WriteLine(filmFrame+",play");Play();yield return FilmHold(8.5f);filmMarks.WriteLine(filmFrame+",result");if(!solved)throw new Exception("Film did not solve "+n);if(n%3==2){yield return FilmStory();yield return FilmHold(5);}else yield return FilmHold(3);CloseModal();FilmStop();}
  for(int n=0;n<3;n++){StartFree();board.Restore(new Stone[16]);int a=SecretSongs.Pairs[n][0],b=SecretSongs.Pairs[n][1];board.cells[8]=new Stone(a);board.cells[9]=new Stone(b);board.cells[10]=new Stone(a);UpdateSlots();yield return null;FilmStart("secret-"+n);ShowSecretSongs();yield return FilmHold(5);CloseModal();yield return null;ClickSlotForTest(8);yield return FilmHold(1);ArmWeave();yield return FilmHold(2);ClickSlotForTest(9);yield return FilmHold(2);ClickSlotForTest(9);ClickSlotForTest(SecretSongs.Starts[n]);yield return FilmHold(1);ClickSlotForTest(10);ClickSlotForTest(SecretSongs.Replies[n]);yield return FilmHold(1);Play();yield return FilmHold(8);yield return FilmHold(2);FilmStop();}
  StartFree();yield return null;FilmStart("limits");ShowAbout();yield return FilmHold(10);FilmStop();FilmClock.Active=false;Time.captureFramerate=0;Debug.Log("CIJING_FILM_PASS 9 levels and 3 secret songs");Application.Quit();
 }
}
}

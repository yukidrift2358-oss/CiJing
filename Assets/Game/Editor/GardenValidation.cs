using System;
using System.IO;
namespace CiJing.EditorTools {
public static class GardenValidation {
 static int checks;static void Check(bool ok,string why){checks++;if(!ok)throw new Exception("Garden contract: "+why);}
 static Stone[] Notes(params int[] data){var b=new Stone[16];for(int i=0;i<data.Length;i+=3)b[data[i]]=new Stone(data[i+1],data[i+2]);return b;}
 public static void Run(){checks=0;string reason;
  Check(VerseRules.Check(Notes(0,2,1,2,2,1,4,2,1),3,out reason),"three distinct drops need not use canonical gaps");
  Check(!VerseRules.Check(Notes(0,2,1,1,2,1,4,2,1),3,out reason),"adjacent drops are not distinct");
  Check(VerseRules.Check(Notes(0,4,2,3,4,2),4,out reason),"wind bursts need not be opposite");
  Check(!VerseRules.Check(Notes(0,4,2,1,4,2),4,out reason),"overlapping wind bursts fail");
  Check(VerseRules.Check(Notes(0,2,1,2,2,1,4,4,2),5,out reason),"noncanonical petal route");
  Check(VerseRules.Check(Notes(0,2,1,2,2,1,3,4,1,5,4,1),5,out reason),"noncanonical flower route");
  Check(!VerseRules.Check(Notes(0,4,2,2,2,1,4,2,1),5,out reason),"opening before any rain skips listening");
  Check(!VerseRules.Check(Notes(0,2,1,2,2,1,7,4,2),5,out reason),"late wind cannot end calm");
  Check(!VerseRules.Check(Notes(0,2,2,3,4,2),5,out reason),"continuous rain cannot stand for two distinct drops");
  Check(!VerseRules.Check(Notes(0,2,1,2,2,1,4,4,2,6,0,1),5,out reason),"foreign note shortcut rejected");
  int letter=0,flower=0;for(int r1=0;r1<8;r1++)for(int r2=r1+1;r2<8;r2++)for(int w1=0;w1<8;w1++){if(w1==r1||w1==r2)continue;if(VerseRules.Check(Notes(r1,2,1,r2,2,1,w1,4,2),5,out reason))letter++;for(int w2=w1+1;w2<8;w2++){if(w2==r1||w2==r2)continue;if(VerseRules.Check(Notes(r1,2,1,r2,2,1,w1,4,1,w2,4,1),5,out reason))flower++;}}
  Check(letter>5&&flower>5,"both outcomes need multiple independently valid arrangements");
  Check(RevisitRules.Check(Notes(0,1,1,3,0,1),0,out reason),"reflection reverses original pair");Check(!RevisitRules.Check(Notes(0,0,1,3,1,1),0,out reason),"shore order is not reflection order");
  Check(RevisitRules.Check(Notes(1,0,1,4,1,1),1,out reason),"two quiet beats allow distant answer");Check(!RevisitRules.Check(Notes(0,0,1,2,1,1),1,out reason),"one quiet beat is not two");
  File.WriteAllText("Validation/garden-rules-results.json","{\"passed\":true,\"assertions\":"+checks+",\"validLetterLayouts\":"+letter+",\"validFlowerLayouts\":"+flower+",\"returnScenes\":2}");
 }
}
}

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace CiJing.EditorTools {
public static class BuildGame {
 public static void Run(){
 Directory.CreateDirectory("Validation");
 ValidateRules();AssetDatabase.ImportAsset("Assets/Resources/Fonts/Poetry.ttf",ImportAssetOptions.ForceUpdate);
 var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
 var go=new GameObject("Main Camera",typeof(Camera),typeof(AudioListener));go.tag="MainCamera";var cam=go.GetComponent<Camera>();cam.orthographic=true;cam.orthographicSize=5;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.94f,.93f,.86f);go.transform.position=new Vector3(0,0,-10);var data=go.AddComponent<UniversalAdditionalCameraData>();data.renderPostProcessing=true;
 var volume=new GameObject("Subtle atmosphere").AddComponent<Volume>();volume.isGlobal=true;var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Game/Atmosphere.asset");if(profile==null){profile=ScriptableObject.CreateInstance<VolumeProfile>();AssetDatabase.CreateAsset(profile,"Assets/Game/Atmosphere.asset");}profile.components.Clear();var tone=profile.Add<Tonemapping>();tone.mode.Override(TonemappingMode.ACES);var bloom=profile.Add<Bloom>();bloom.intensity.Override(.25f);bloom.threshold.Override(1);var vignette=profile.Add<Vignette>();vignette.intensity.Override(.12f);volume.sharedProfile=profile;
 var light=new GameObject("Global Light").AddComponent<Light2D>();light.lightType=Light2D.LightType.Global;light.intensity=1;
 new GameObject("CiJingGame").AddComponent<GameApp>();
 QualitySettings.SetQualityLevel(QualitySettings.names.Length-1);QualitySettings.vSyncCount=1;
 EditorSceneManager.SaveScene(scene,"Assets/Scenes/CiJing.unity");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/CiJing.unity",true)};
 PlayerSettings.companyName="CiJingStudio";PlayerSettings.productName="词境";PlayerSettings.bundleVersion="0.3.0";PlayerSettings.defaultScreenWidth=1600;PlayerSettings.defaultScreenHeight=900;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.resizableWindow=true;PlayerSettings.runInBackground=true;PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone,ScriptingImplementation.Mono2x);PlayerSettings.colorSpace=ColorSpace.Linear;
 foreach(var id in AssetDatabase.FindAssets("t:Texture2D",new[]{"Assets/Resources/Art"})){var p=AssetDatabase.GUIDToAssetPath(id);var importer=(TextureImporter)AssetImporter.GetAtPath(p);importer.maxTextureSize=2048;importer.textureCompression=TextureImporterCompression.CompressedHQ;importer.SaveAndReimport();}
 AssetDatabase.SaveAssets();
 string output=Path.GetFullPath("../Deliverables/CiJing-v0.3-Windows/CiJing.exe");Directory.CreateDirectory(Path.GetDirectoryName(output));var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/CiJing.unity"},locationPathName=output,target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
 File.WriteAllText("Validation/build-result.json","{\"result\":\""+report.summary.result+"\",\"errors\":"+report.summary.totalErrors+",\"warnings\":"+report.summary.totalWarnings+",\"size\":"+report.summary.totalSize+"}");if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");Debug.Log("CIJING_BUILD_PASS");
 }
 static void Check(bool value,string message){if(!value)throw new Exception(message);}
 public static void ValidateRules(){int assertions=0;foreach(var level in Content.Levels){var b=new Board();b.Load(level);int mass=b.Mass;for(int i=0;i<8;i++){var s=level.target[i];for(int j=0;j<s.Mass;j++){int n=Array.FindIndex(b.cells,8,8,x=>!x.Empty&&x.tone==s.tone);Check(n>=0&&b.Move(n,i),"Level solvability "+level.title);assertions++;}}Check(b.Matches(level),"Target mismatch");Check(b.Mass==mass,"Mass lost");assertions+=2;int levelIndex=Array.IndexOf(Content.Levels,level);if(levelIndex>=3){var bad=(Stone[])b.cells.Clone();int empty=Array.FindIndex(bad,0,8,x=>x.Empty);bad[empty]=new Stone(3);string why;Check(!VerseRules.Check(bad,levelIndex,out why),"Unexpected extra note accepted");assertions++;if(levelIndex==3||levelIndex==4||levelIndex==5||levelIndex==7){var alternate=new Stone[16];for(int t=0;t<8;t++)alternate[(t+1)%8]=b.cells[t];Check(VerseRules.Check(alternate,levelIndex,out why),"Alternate valid rhythm rejected");assertions++;}}}
 var board=new Board();board.cells[8]=new Stone(0);board.cells[9]=new Stone(1);Check(!board.Move(8,9),"Different families merged");Check(!board.Move(8,8),"Self merge");Check(!board.Move(-1,0),"Invalid move");board.cells[10]=new Stone(0);Check(board.Move(8,10),"Pair merge");Check(board.cells[10].rank==2&&board.Mass==3,"Merge conservation");Check(board.Split(10),"Split failed");Check(board.Mass==3,"Split conservation");Check(board.Undo()&&board.cells[10].rank==2,"Undo split");assertions+=8;
 File.WriteAllText("Validation/rule-results.json","{\"assertions\":"+assertions+",\"solvableLevels\":9,\"passed\":true}");}
}
}

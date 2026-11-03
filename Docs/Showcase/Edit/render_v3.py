import csv,json,re,subprocess,sys,wave,concurrent.futures,shutil,os
from pathlib import Path
B=Path(__file__).resolve().parent.parent;W=B.parents[1];sys.path.insert(0,str(W/'CiJing/Tools/AudioLib'))
import numpy as np
FF=os.environ.get('FFMPEG_PATH') or shutil.which('ffmpeg')
if not FF:
 import imageio_ffmpeg
 FF=imageio_ffmpeg.get_ffmpeg_exe()
SR=48000
SEG=B/'edit/segments';SEG.mkdir(exist_ok=True)
def run(args):
 p=subprocess.run([FF,'-hide_banner','-loglevel','error','-y',*map(str,args)],capture_output=True)
 if p.returncode:raise RuntimeError(p.stderr.decode(errors='replace')[-2000:])
 return p.stdout
def duration(p):
 s=subprocess.run([FF,'-i',str(p)],capture_output=True).stderr.decode(errors='replace');m=re.search(r'Duration: (\d+):(\d+):(\d+\.\d+)',s)
 return int(m[1])*3600+int(m[2])*60+float(m[3])
def decode(p):return np.frombuffer(run(['-i',p,'-f','f32le','-ac',1,'-ar',SR,'pipe:1']),np.float32).copy()
def add(a,b,at,gain=1):
 i=max(0,round(at*SR));n=min(len(b),len(a)-i)
 if n>0:a[i:i+n]+=b[:n]*gain
def wav(p,a):
 peak=np.max(np.abs(a))
 if peak>.96:a=a*(.96/peak)
 with wave.open(str(p),'wb') as f:f.setnchannels(1);f.setsampwidth(2);f.setframerate(SR);f.writeframes((a*32767).astype('<i2').tobytes())
def stamp(s,ass=False):
 if ass:return f'{int(s//3600)}:{int(s%3600//60):02}:{s%60:05.2f}'
 ms=round(s*1000);return f'{ms//3600000:02}:{ms//60000%60:02}:{ms//1000%60:02},{ms%1000:03}'
HEADER='''[Script Info]
ScriptType: v4.00+
PlayResX: 1920
PlayResY: 1080
[V4+ Styles]
Format: Name,Fontname,Fontsize,PrimaryColour,SecondaryColour,OutlineColour,BackColour,Bold,Italic,Underline,StrikeOut,ScaleX,ScaleY,Spacing,Angle,BorderStyle,Outline,Shadow,Alignment,MarginL,MarginR,MarginV,Encoding
Style: Caption,Microsoft YaHei,32,&H002E382E,&H002E382E,&H00E6F0F5,&H00E6F0F5,0,0,0,0,100,100,0,0,1,1,0,2,100,100,25,1
Style: Title,Microsoft YaHei,32,&H00394736,&H00394736,&H00E6F0F5,&H00E6F0F5,0,0,0,0,100,100,0,0,1,0,0,7,160,100,10,1
Style: Note,Microsoft YaHei,23,&H00365588,&H00365588,&H00E6F0F5,&H00E6F0F5,0,0,0,0,100,100,0,0,1,1,0,9,100,160,18,1
[Events]
Format: Layer,Start,End,Style,Name,MarginL,MarginR,MarginV,Effect,Text
'''
def subtitle(p,title,lines,notes):
 def e(a,b,style,t):return f'Dialogue: 0,{stamp(a,True)},{stamp(b,True)},{style},,0,0,0,,{t}\n'
 txt=HEADER+e(0,9999,'Title',title)
 for a,b,t in lines:txt+=e(a,b,'Caption',t)
 for a,b,t in notes:txt+=e(a,b,'Note',t)
 p.write_text(txt,encoding='utf-8-sig')
A=W/'CiJing/Assets/Resources/Audio';samples={(t,r):decode(A/f'harp{t}{r}.wav') for t in range(5) for r in range(2)}
score=[decode(A/f'score{i}.wav') for i in range(3)];orn=[decode(A/f'ornament{i}.wav') for i in range(3)];codas=[decode(A/f'coda{i}.wav') for i in range(4)];flute=decode(A/'flute.wav');rain=decode(A/'scene-rain.wav');wind=decode(A/'scene-wind.wav')
def marks(n):return {r['phase']:int(r['frame'])/15 for r in csv.DictReader((B/'raw'/f'{n}-marks.csv').open())}
def gameaudio(name,d):
 # Rebuild audible note events from recorded frame timestamps. This is an editorial mix,
 # not a capture of Unity's exact realtime mixer/filters; documented in production notes.
 chapter=int(name.split('-')[1])//3 if name.startswith('level-') else 2;n=int(name.split('-')[1]) if name.startswith('level-') else -1
 a=np.zeros(round(d*SR),np.float32);m=marks(name);playing=m.get('play',9999)
 t=np.arange(len(a))/SR;duck=np.where((t>=playing)&(t<playing+5.6),.42,1).astype(np.float32)
 a+=(np.resize(score[chapter],len(a))*.17+np.resize(orn[chapter],len(a))*.045)*duck
 last=-10
 for row in csv.DictReader((B/'raw'/f'{name}.csv').open()):
  when=int(row['frame'])/15;tone=int(row['tone']);rank=int(row['rank']);echo=int(row['echo']);add(a,samples[tone,rank-1],when,.46)
  if echo:add(a,samples[echo-1,1],when+.11,.32)
  if chapter==2 and when>=playing:add(a,samples[tone,rank-1],when+.22,.045)
  if when-last>=1.6:
   last=when;pitch=2**([0,2,4,7,9][tone]/12);clip=np.interp(np.arange(0,len(flute),pitch),np.arange(len(flute)),flute).astype(np.float32);add(a,clip,when,.045 if rank==2 else .026)
  if chapter==1 and when>=playing:
   if tone in (2,4):add(a,(rain if tone==2 else wind)[:int(SR*(1.4 if rank==1 else 2.1))],when,.06)
 if n>=0:
  at=m.get('result',d)+ (3 if n==5 else -2.9)
  add(a,codas[0 if chapter==0 else 2 if chapter==1 else 3],at,.23)
 return a
def still_video(path,dur,out):
 run(['-loop',1,'-i',path,'-t',dur,'-vf','scale=1600:900:force_original_aspect_ratio=decrease,pad=1600:900:(ow-iw)/2:(oh-ih)/2:color=0xf5f0e6,pad=1920:1080:160:60:color=0xf5f0e6','-r',30,'-c:v','libx264','-threads',2,'-preset','veryfast','-crf',21,'-pix_fmt','yuv420p',out])
def concat(files,out):
 listing=out.with_suffix('.txt');listing.write_text(''.join("file '"+p.as_posix()+"'\n" for p in files),encoding='utf-8');run(['-f','concat','-safe',0,'-i',listing,'-c','copy',out])
SHOTS=B/'reference-stills'
def render(item):
 ident=item['id'];vo=decode(B/'voice'/f'{ident}.mp3');lines=json.loads((B/'voice'/f'{ident}.json').read_text(encoding='utf-8'))['captions'];vd=len(vo)/SR
 name=item['source'];special=name in ['stills','history','endcard'];overview=item['kind']=='overview';d=max(vd+1.2,12)
 rawdur=duration(B/'raw'/f'{name}.mp4') if not special else 0
 if not overview and not special:d=max(rawdur,vd+1.2)
 audio=np.zeros(round(d*SR),np.float32);audio+=np.resize(score[0],len(audio))*.10;parts=[];notes=[];at=0
 def picture(path,seconds,label):
  nonlocal at
  out=SEG/f'{ident}-part{len(parts)}.mp4';still_video(path,seconds,out);parts.append(out);notes.append((at,at+seconds,label));at+=seconds
 if name=='history':
  histories=[('history-v0.3.png','v0.3 历史运行截图'),('history-v0.6.png','v0.6 历史运行截图'),('history-v0.13.png','v0.13 历史运行截图'),('guide-rest.png','v0.15 运行截图')]
  for p,label in histories:
   path=SHOTS/p
   if not path.exists():raise RuntimeError('Missing factual history '+str(path))
   picture(path,d/len(histories),label+' · 定格阅读')
 elif name=='endcard':picture(B/'diagrams/endcard.png',d,'')
 else:
  if item.get('diagram'):picture(B/'diagrams'/f'{item["diagram"]}.png',min(7,d*.2),'规则示意')
  shots=item.get('shots',[])
  if not special:
   start=marks(name).get(item.get('phase'),0) if overview else 0
   need=(d-at)*(.64 if shots else 1);available=max(.1,min(need,rawdur-start));out=SEG/f'{ident}-part{len(parts)}.mp4'
   run(['-ss',start,'-i',B/'raw'/f'{name}.mp4','-t',available,'-vf','scale=1600:900,pad=1920:1080:160:60:color=0xf5f0e6','-r',30,'-an','-c:v','libx264','-threads',2,'-preset','veryfast','-crf',21,out]);parts.append(out)
   a=gameaudio(name,rawdur);i=round(start*SR);add(audio,a[i:i+round(available*SR)],at,1);at+=available
   if name.startswith('secret-'):notes.append((0,d,'藏曲演示 · 配方材料预置'))
  if shots:
   each=(d-at)/len(shots)
   for shot in shots:picture(SHOTS/shot,each,'v0.15 运行截图 · 定格阅读')
  elif at<d-.03:
   # Hold the genuine final frame with explicit labeling; never imply ongoing input.
   frame=SEG/f'{ident}-hold.png';run(['-sseof',-.15,'-i',B/'raw'/f'{name}.mp4','-frames:v',1,frame]);picture(frame,d-at,'定格分析')
 visual=SEG/f'{ident}-visual.mp4';concat(parts,visual)
 # Narration gets priority, with a gentler bed beneath speech, plus uncovered game tails.
 gain=np.ones(len(audio),np.float32);gain[:min(len(gain),len(vo))]=.60;audio*=gain;add(audio,vo,0,.96)
 path=SEG/f'{ident}.wav';wav(path,audio);sub=SEG/f'{ident}.ass';subtitle(sub,item['title'],lines,notes)
 out=SEG/f'{ident}.mp4';run(['-i',visual,'-i',path,'-t',d,'-vf','ass='+sub.relative_to(W).as_posix(),'-r',30,'-c:v','libx264','-threads',2,'-preset','veryfast','-crf',21,'-c:a','aac','-b:a','160k','-ar',SR,'-movflags','+faststart',out])
 print(ident,round(d,2),'OK',flush=True);return dict(id=ident,title=item['title'],duration=duration(out),lines=lines,path=str(out))
if __name__=='__main__':
 items=json.loads((B/'edit/script.json').read_text(encoding='utf-8'));only=set(filter(None,os.environ.get('CIJING_RENDER_IDS','').split(',')));results=json.loads((B/'edit/rendered.json').read_text(encoding='utf-8')) if only else {}
 with concurrent.futures.ThreadPoolExecutor(max_workers=3) as pool:
  jobs={pool.submit(render,x):x for x in items if not only or x['id'] in only}
  for job in concurrent.futures.as_completed(jobs):results[jobs[job]['id']]=job.result()
 (B/'edit/rendered.json').write_text(json.dumps(results,ensure_ascii=False,indent=2),encoding='utf-8')
 for title,kind in [('策划讲解版','overview'),('全流程演示','full')]:
  order=[x for x in items if (x['kind']=='overview')==(kind=='overview')];files=[Path(results[x['id']]['path']) for x in order]
  joined=SEG/f'{title}-joined.mp4';concat(files,joined);out=B/f'词境_{title}_v3.mp4'
  run(['-i',joined,'-r',30,'-fps_mode','cfr','-c:v','libx264','-threads',4,'-preset','veryfast','-crf',21,'-c:a','aac','-ar',SR,'-b:a','160k','-movflags','+faststart',out])
  at=0;subs=[];chapters=[]
  for item in order:
   r=results[item['id']];chapters.append(f'{stamp(at)}  {r["title"]}')
   for a,b,t in r['lines']:subs.append((a+at,b+at,t))
   at+=r['duration']
  (B/f'{title}.srt').write_text('\n\n'.join(f'{i+1}\n{stamp(a)} --> {stamp(b)}\n{t}' for i,(a,b,t) in enumerate(subs)),encoding='utf-8-sig')
  (B/f'{title}-章节.txt').write_text('\n'.join(chapters)+f'\n总时长 {duration(out):.2f} 秒\n',encoding='utf-8')
 print('V3_VIDEOS_COMPLETE',flush=True)

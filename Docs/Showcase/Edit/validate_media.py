from pathlib import Path
import json,re,subprocess,hashlib
from render_v3 import FF,duration,B,stamp
items=json.loads((B/'edit/script.json').read_text(encoding='utf-8'));rendered=json.loads((B/'edit/rendered.json').read_text(encoding='utf-8'));results={}
for label,overview in [('策划讲解版',True),('全流程演示',False)]:
 p=B/f'词境_{label}_v3.mp4';d=duration(p)
 cmd=[FF,'-hide_banner','-v','error','-nostats','-i',str(p),'-progress','pipe:1','-f','null','-']
 r=subprocess.run(cmd,capture_output=True,text=True,encoding='utf-8',errors='replace')
 if r.returncode or r.stderr.strip():raise RuntimeError(r.stderr)
 frames=int(re.findall(r'frame=(\d+)',r.stdout)[-1]);assert abs(frames/30-d)<.15,(frames,d)
 levels=subprocess.run([FF,'-hide_banner','-i',str(p),'-vn','-af','volumedetect','-f','null','-'],capture_output=True).stderr.decode(errors='replace')
 peak=float(re.search(r'max_volume: ([-\d.]+)',levels)[1]);mean=float(re.search(r'mean_volume: ([-\d.]+)',levels)[1]);assert peak<0,(p,peak)
 order=[x for x in items if (x['kind']=='overview')==overview];at=0;doc=[];last=0;caption_count=0
 for x in order:
  seg=rendered[x['id']];caption=seg['lines'];clean=lambda s:re.sub(r'[^\w]','',s)
  assert clean(''.join(l[2] for l in caption))==clean(x['text']),x['id']
  for a,b,t in caption:
   assert 0<=a<b<=seg['duration']+.1,(x['id'],a,b)
   assert a+at>=last-.01,(x['id'],'overlap');last=b+at;caption_count+=1
  doc.append(f'## {stamp(at)}　{x["title"]}\n\n{x["text"]}');at+=seg['duration']
 assert last<=d+.1
 (B/f'{label}-讲解稿.md').write_text('# '+label+' · v0.15\n\n'+'\n\n'.join(doc),encoding='utf-8')
 results[label]={'file':p.name,'seconds':d,'decodedFrames':frames,'fps':30,'decodeErrors':0,'captionCount':caption_count,'captionTextMatchesScript':True,'lastCaptionSeconds':last,'peakDb':peak,'meanDb':mean,'bytes':p.stat().st_size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()}
(B/'edit/media-check.json').write_text(json.dumps(results,ensure_ascii=False,indent=2),encoding='utf-8');print(json.dumps(results,ensure_ascii=True),flush=True)

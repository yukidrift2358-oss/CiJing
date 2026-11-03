import asyncio,json,re,sys
from pathlib import Path
BASE=Path(__file__).resolve().parent.parent
sys.path.insert(0,str(BASE.parent/'CiJing-Video-v2/tools'))
import edge_tts
def clean(s):return re.sub(r'[^\w]','',s)
def split(text):
 parts=re.findall(r'[^，。；！？]+[，。；！？]?',text);result=[];current=''
 for p in parts:
  if current and len(current+p)>30:result.append(current);current=''
  current+=p
 if current:result.append(current)
 return result
async def generate(item,gate):
 async with gate:
  path=BASE/'voice'/f'{item["id"]}.mp3';events=[]
  timing=path.with_suffix('.json')
  if path.exists() and timing.exists() and json.loads(timing.read_text(encoding='utf-8')).get('text')==item['text']:return
  for attempt in range(3):
   try:
    events=[]
    spoken=re.sub(r'(?<!帘)角','决',item['text']).replace('徵','纸')
    with path.open('wb') as audio:
     async for msg in edge_tts.Communicate(spoken,'zh-CN-YunxiNeural',rate='-3%',boundary='WordBoundary').stream():
      if msg['type']=='audio':audio.write(msg['data'])
      elif msg['type']=='WordBoundary':events.append(msg)
    if events and path.stat().st_size>1000:break
   except Exception:
    if attempt==2:raise
    await asyncio.sleep(2)
  pos=0;tokens=[]
  for e in events:
   n=len(clean(e['text']));tokens.append((pos,pos+n,e['offset']/1e7,(e['offset']+e['duration'])/1e7));pos+=n
  captions=[];cursor=0
  for chunk in split(item['text']):
   n=len(clean(chunk));matched=[t for t in tokens if t[1]>cursor and t[0]<cursor+n]
   if not matched:raise RuntimeError('No speech timing for '+chunk)
   captions.append([matched[0][2],matched[-1][3],chunk]);cursor+=n
  if cursor!=pos:raise RuntimeError('Speech token alignment mismatch '+item['id']+' '+str((cursor,pos)))
  (BASE/'voice'/f'{item["id"]}.json').write_text(json.dumps({'voice':'zh-CN-YunxiNeural','rate':'-3%','text':item['text'],'captions':captions,'events':events},ensure_ascii=False,indent=2),encoding='utf-8')
  print(item['id'],'VOICE_OK',flush=True)
async def main():
 items=json.loads((BASE/'edit/script.json').read_text(encoding='utf-8'))
 gate=asyncio.Semaphore(3)
 await asyncio.gather(*(generate(x,gate) for x in items))
asyncio.run(main())

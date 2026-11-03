from pathlib import Path
import sys,wave,json
sys.path.insert(0,str(Path('CiJing/Tools/AudioLib').resolve()))
import numpy as np
P=Path('CiJing/Assets/Resources/Audio');sr=44100
def read(t):
 with wave.open(str(P/f'harp{t}1.wav')) as f:return np.frombuffer(f.readframes(f.getnframes()),dtype='<i2').astype(np.float32)/32768
notes=[read(t) for t in range(5)];report=[]
for n,melody in enumerate([[3,2,1,0],[2,3,1,0],[4,3,2,0],[0,1,3,0]]):
 out=np.zeros((int(sr*2.2),2),np.float32)
 for i,tone in enumerate(melody):
  a=notes[tone];at=int(sr*.35*i);length=min(len(a),len(out)-at);pan=(-.3+i*.2)
  out[at:at+length,0]+=a[:length]*(1-pan)*(.45 if i<3 else .65);out[at:at+length,1]+=a[:length]*(1+pan)*(.45 if i<3 else .65)
 bass=np.interp(np.arange(0,len(notes[0]),.5),np.arange(len(notes[0])),notes[0]);length=min(len(bass),len(out));out[:length]+=bass[:length,None]*.2
 dry=out.copy();out+=np.roll(dry,int(sr*.17),axis=0)*.08
 out*=np.minimum(np.arange(len(out))/(sr*.005),1)[:,None]*np.minimum((len(out)-np.arange(len(out)))/(sr*.2),1)[:,None];out*=.72/np.max(np.abs(out))
 with wave.open(str(P/f'coda{n}.wav'),'wb') as f:f.setnchannels(2);f.setsampwidth(2);f.setframerate(sr);f.writeframes((out*32767).astype('<i2').tobytes())
 report.append({'clip':f'coda{n}','seconds':2.2,'peak':float(np.max(np.abs(out))),'rms':float(np.sqrt(np.mean(out*out)))})
Path('CiJing/Validation/audio-v015-cadences.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print('Prepared four original sample-based closing phrases')

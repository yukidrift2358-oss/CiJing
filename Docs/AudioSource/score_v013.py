"""Original adaptive arrangement of existing CC0 VSCO recordings.
Run from repository root. No remote music or unknown-licence samples used.
"""
import sys,wave,json
from pathlib import Path
sys.path.insert(0,str(Path('CiJing/Tools/AudioLib').resolve()))
import numpy as np
P=Path('CiJing/Assets/Resources/Audio');SR=44100;N=int(SR*44.8)
def read(name):
 with wave.open(str(P/name)) as f:
  return np.frombuffer(f.readframes(f.getnframes()),dtype='<i2').astype(np.float32)/32768
def pitch(a,ratio):return np.interp(np.arange(0,len(a),ratio),np.arange(len(a)),a).astype(np.float32)
def write(name,a,peak=.67):
 a=a.astype(np.float32);a-=np.mean(a,axis=0);a*=peak/max(float(np.max(np.abs(a))),.001)
 with wave.open(str(P/(name+'.wav')),'wb') as f:
  f.setnchannels(a.shape[1] if a.ndim==2 else 1);f.setsampwidth(2);f.setframerate(SR);f.writeframes((a*32767).astype('<i2').tobytes())
 return {'name':name,'seconds':len(a)/SR,'peak':float(np.max(np.abs(a))),'rms':float(np.sqrt(np.mean(a*a)))}
harps=[read(f'harp{i}1.wav') for i in range(5)];flute=read('flute.wav');semitones=[0,2,4,7,9];report=[]
motifs=[[0,2,3,2,1,0,4,3],[2,1,0,2,4,3,2,1],[0,1,2,4,3,2,1,0]]
for chapter in range(3):
 bed=np.zeros((N,2),np.float32);detail=np.zeros_like(bed)
 def add(out,a,beat,gain,pan=0):
  at=int(beat*.7*SR);n=min(len(a),N-at)
  if n<=0:return
  out[at:at+n,0]+=a[:n]*gain*np.sqrt((1-pan)/2);out[at:at+n,1]+=a[:n]*gain*np.sqrt((1+pan)/2)
 for phrase in range(8):
  root=[0,3,1,0][phrase%4];start=phrase*8
  add(bed,pitch(harps[root],.5),start,.56,-.15)
  # Phrased accompaniment: a space after the rising figure, not endless eighths.
  for offset,degree,gain in [(0,root,.45),(1,2,.29),(2.5,3,.30),(4,root,.38),(5.5,1,.25),(6,2,.24)]:
   add(bed,harps[degree],start+offset,gain,(-.45 if offset<4 else .45))
  if phrase%2==0:
   for k,duration in [(0,1.7),(2,1.1),(4.5,1.65)]:
    degree=motifs[chapter][(phrase+k)%8 if isinstance(k,int) else (phrase+4)%8]
    a=pitch(flute,2**(semitones[degree]/12))[:int(duration*SR)]
    fade=np.minimum(np.arange(len(a))/(SR*.12),1)*np.minimum((len(a)-np.arange(len(a)))/(SR*.26),1)
    add(detail,a*fade,start+k,.29,.3)
  else:
   for k in [0,1.5,3,5,6.5]:
    degree=motifs[chapter][int(phrase+k)%8];add(detail,pitch(harps[degree],2),start+k,.23,-.2)
 # Circular early reflections produce a genuinely seamless phrase boundary.
 for out in [bed,detail]:
  dry=out.copy()
  for delay,gain in [(.19,.12),(.37,.08),(.61,.05)]:out+=np.roll(dry,int(delay*SR),axis=0)*gain
 report.append(write('score'+str(chapter),bed,.72));report.append(write('ornament'+str(chapter),detail,.56))
rng=np.random.default_rng(13013);n=SR*12
# Authored granular rain: filtered noise with many quiet staggered droplets.
white=rng.normal(0,1,n);spectrum=np.fft.rfft(white);freq=np.fft.rfftfreq(n,1/SR)
rain=np.fft.irfft(spectrum*np.minimum(freq/800,1)/(1+(freq/5500)**2),n)*.05
for _ in range(470):
 at=int(rng.integers(0,n-4000));length=int(rng.integers(600,2200));t=np.arange(length)/SR
 grain=rng.normal(0,1,length)*np.exp(-t*150)*.018
 rain[at:at+length]+=grain
wind=np.fft.irfft(spectrum/(1+(freq/450)**2),n)
wind*=.6+.2*np.sin(np.arange(n)*2*np.pi/n*3)+.1*np.sin(np.arange(n)*2*np.pi/n*7)
report.append(write('scene-rain',rain,.32));report.append(write('scene-wind',wind,.28))
Path('CiJing/Validation/audio-v013-analysis.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print(json.dumps(report))

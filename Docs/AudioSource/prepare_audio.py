from pathlib import Path
import subprocess,imageio_ffmpeg,wave,numpy as np,json
p=Path('CiJing/Assets/Resources/Audio');p.mkdir(exist_ok=True);src=Path('CiJing/Docs/AudioSource');ff=imageio_ffmpeg.get_ffmpeg_exe();freq=[261.6256,293.6648,329.6276,391.9954,440.0]
for t,f in enumerate(freq):
 source='harpA4' if t==4 else 'harpD4';base=440 if t==4 else freq[1]
 for rank,dur in enumerate([.66,1.65]):
  filt=f'asetrate={int(44100*f/base)},aresample=44100,atrim=0:{dur},afade=t=in:d=0.004,afade=t=out:st={dur-.24}:d=0.24'
  # Input sample rate is read rather than assumed.
  with wave.open(str(src/(source+'.wav'))) as w:rate=w.getframerate()
  filt=filt.replace(f'asetrate={int(44100*f/base)}',f'asetrate={int(rate*f/base)}')
  subprocess.run([ff,'-y','-i',str(src/(source+'.wav')),'-af',filt,'-ac','1','-ar','44100',str(p/f'harp{t}{rank}.wav')],capture_output=True,check=True)
subprocess.run([ff,'-y','-i',str(src/'fluteC4.wav'),'-ac','1','-ar','44100',str(p/'flute.wav')],capture_output=True,check=True)
def read(name):
 with wave.open(str(p/name)) as w:return np.frombuffer(w.readframes(w.getnframes()),dtype='<i2').astype(np.float32)/32768
for f in p.glob('harp*.wav'):
 a=read(f.name);a*=.65/max(float(np.max(np.abs(a))),.001)
 with wave.open(str(f),'wb') as w:w.setnchannels(1);w.setsampwidth(2);w.setframerate(44100);w.writeframes((a*32767).astype('<i2').tobytes())
notes=[read(f'harp{i}1.wav') for i in range(5)];flute=read('flute.wav');sr=44100;beat=.7;length=64*beat
motifs=[[0,2,3,2,1,0,4,3],[2,1,0,1,2,3,4,2],[0,1,2,1,4,3,1,0]]
report=[]
for chapter in range(3):
 mix=np.zeros((int(length*sr),2),np.float32)
 def add(a,at,gain,pan=0):
  start=int(at*sr);n=min(len(a),len(mix)-start)
  if n>0:mix[start:start+n,0]+=a[:n]*gain*(1-pan*.3);mix[start:start+n,1]+=a[:n]*gain*(1+pan*.3)
 for b in range(64):
  phrase=b//8;idx=motifs[chapter][(b+phrase//2)%8];add(notes[idx],b*beat,.28,(-1 if b%2 else 1)*.4)
  if b%4==0:
   bass=notes[0 if (b//8)%2==0 else 3];low=np.interp(np.arange(len(bass)*2)*.5,np.arange(len(bass)),bass)*.5;add(low,b*beat,.22)
  if b%8==0:
   semitone=[0,2,4,7,9][motifs[chapter][phrase%8]];ratio=2**(semitone/12);a=np.interp(np.arange(0,min(len(flute),int(sr*2.4)),ratio),np.arange(len(flute)),flute);a*=np.minimum(np.arange(len(a))/(sr*.2),1)*np.minimum((len(a)-np.arange(len(a)))/(sr*.35),1);add(a,b*beat,.16)
 fade=np.minimum(np.arange(len(mix))/(sr*.06),1)*np.minimum((len(mix)-np.arange(len(mix)))/(sr*.7),1);mix*=fade[:,None];peak=float(np.max(np.abs(mix)));mix*=.60/max(peak,.001)
 with wave.open(str(p/f'music{chapter}.wav'),'wb') as w:w.setnchannels(2);w.setsampwidth(2);w.setframerate(sr);w.writeframes((mix*32767).astype('<i2').tobytes())
 report.append({'chapter':chapter,'seconds':length,'peak':float(np.max(np.abs(mix))),'rms':float(np.sqrt(np.mean(mix**2)))})
Path('CiJing/Validation/audio-analysis.json').write_text(json.dumps(report,indent=2));print(report)

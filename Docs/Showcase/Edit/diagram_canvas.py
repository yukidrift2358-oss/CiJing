from pathlib import Path
import sys,math,html
BASE=Path(__file__).resolve().parent.parent
sys.path.insert(0,str(BASE.parents[1]/'CiJing/Tools/AudioLib'))
from PIL import Image,ImageDraw,ImageFont
INK='#293e38';RED='#9e4935';MUTE='#69756a';PAPER='#f5f0e6';PALE='#dedfce'
class Canvas:
 def __init__(self,label=True):self.im=Image.new('RGB',(1920,1080),PAPER);self.d=ImageDraw.Draw(self.im);self.svg=['<svg xmlns="http://www.w3.org/2000/svg" width="1920" height="1080" viewBox="0 0 1920 1080"><rect width="1920" height="1080" fill="'+PAPER+'"/>'];self.text(1750,35,'规则示意',26,RED) if label else None
 def text(self,x,y,t,size=38,color=INK):
  font=ImageFont.truetype('C:/Windows/Fonts/msyh.ttc',size);self.d.text((x,y),t,font=font,fill=color,anchor='mm');self.svg.append(f'<text x="{x}" y="{y}" dominant-baseline="central" text-anchor="middle" font-family="Microsoft YaHei,sans-serif" font-size="{size}" fill="{color}">{html.escape(t)}</text>')
 def circle(self,x,y,r,t='',active=True):
  fill='#ece9d9' if active else PAPER;edge=INK if active else '#bac1b1';self.d.ellipse((x-r,y-r,x+r,y+r),fill=fill,outline=edge,width=3 if active else 2);self.svg.append(f'<circle cx="{x}" cy="{y}" r="{r}" fill="{fill}" stroke="{edge}" stroke-width="2"/>')
  if t:self.text(x,y,t,38 if r>75 else 25,INK if active else MUTE)
 def arrow(self,points,color=RED,width=4):
  self.d.line(points,fill=color,width=width);x,y=points[-1];a,b=points[-2];angle=math.atan2(y-b,x-a);tri=[(x,y),(x-18*math.cos(angle-.45),y-18*math.sin(angle-.45)),(x-18*math.cos(angle+.45),y-18*math.sin(angle+.45))];self.d.polygon(tri,fill=color);self.svg.append('<polyline points="'+' '.join(f'{x},{y}' for x,y in points)+f'" fill="none" stroke="{color}" stroke-width="{width}"/><polygon points="'+' '.join(f'{x},{y}' for x,y in tri)+f'" fill="{color}"/>')
 def save(self,name):self.im.save(BASE/'diagrams'/f'{name}.png');(BASE/'diagrams'/f'{name}.svg').write_text(''.join(self.svg)+'</svg>',encoding='utf-8')

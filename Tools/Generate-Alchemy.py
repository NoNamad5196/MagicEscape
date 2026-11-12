"""S10f original alchemical woodcuts. Run after Generate-Art.py; no PD image inputs.
All six symbols use the same vector stroke. Noto cmap lacks U+2609; vector fallback.
"""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFilter, ImageOps
import math, random, shutil, json
from fontTools.ttLib import TTFont
R=random.Random(5196)
B=Path('UnityProject/Assets/Art/S10b'); E=Path('UnityProject/Assets/Art/S10e'); F=Path('UnityProject/Assets/Art/S10f'); F.mkdir(parents=True,exist_ok=True)
C=Path('Tools/asset-cache/s10f')
if not all((C/n).exists() for n in ['Flame02/Flame02_16x4.tga','NotoSansSymbols.ttf','OFL.txt']):
 import runpy
 runpy.run_path('Tools/Fetch-S10f.py')
L=Path('Tools/FontSource/NotoSansSymbols');L.mkdir(parents=True,exist_ok=True)
for n in ['NotoSansSymbols.ttf','OFL.txt']:shutil.copyfile(C/n,L/n)
cmap=TTFont(L/'NotoSansSymbols.ttf').getBestCmap()
(L/'glyph-audit.json').write_text(json.dumps({hex(c):cmap.get(c) for c in [0x1f702,0x1f704,0x1f701,0x1f703,0x2609,0x2644]},indent=2))
G=(177,151,98); INK=(57,43,29)
def glyph(name,size=256,col=G,bg=(33,31,28,255)):
 im=Image.new('RGBA',(256,256),bg);d=ImageDraw.Draw(im)
 def ln(p):d.line(p,fill=col,width=7,joint='curve')
 if name in ('Earth','Water','Fire','Wind'):
  up=name in ('Fire','Wind');p=[(128,45),(211,198),(45,198),(128,45)] if up else [(45,57),(211,57),(128,210),(45,57)];ln(p)
  if name=='Wind':ln([(79,126),(177,126)])
  if name=='Earth':ln([(79,133),(177,133)])
 elif name=='Light':
  d.ellipse((49,49,207,207),outline=col,width=7);d.ellipse((117,117,139,139),fill=col)
 else:
  # Saturn / lead: cross with descending curved sickle, not a crescent moon.
  ln([(99,39),(99,205)]);ln([(66,77),(137,77)])
  d.arc((96,99,194,207),185,365,fill=col,width=7)
  ln([(192,153),(182,206),(195,213)])
 return im.resize((size,size),Image.Resampling.LANCZOS)
for name in ['Earth','Water','Fire','Wind','Light','Shadow']:glyph(name).save(B/(name+'.png'))
def border(d,w,h,col):
 for n in (12,19):d.rectangle((n,n,w-n-1,h-n-1),outline=col,width=2)
 for x,y,sx,sy in [(29,29,1,1),(w-30,29,-1,1),(29,h-30,1,-1),(w-30,h-30,-1,-1)]:
  for j in range(3):d.arc((x-9,y-9,x+9+j*7,y+9+j*7),0,270,fill=col,width=1)
def paper(size,base):
 im=Image.new('RGB',size);im.putdata([tuple(max(0,min(255,c+R.randrange(-7,8))) for c in base) for _ in range(size[0]*size[1])]);return im
for num in range(1,5):
 im=Image.open(B/('Story_'+str(num)+'.png')).convert('L');im=ImageOps.colorize(im,(45,31,20),(201,177,128));d=ImageDraw.Draw(im);border(d,512,512,INK)
 # Dense fine burin hatch along the ground and celestial border.
 for x in range(36,479,9):d.line((x,465,x+12,450),fill=INK,width=1)
 im.save(B/('Story_'+str(num)+'.png'))
def roman(d,n,x,y,c):
 if n<4:
  for j in range(n):d.line((x+j*12,y,x+j*12,y+26),fill=c,width=4)
 else:d.line((x-8,y,x-8,y+26),fill=c,width=4);d.line((x+4,y,x+16,y+26,x+28,y),fill=c,width=4)
names=['Earth','Water','Fire','Light']
im=paper((1024,512),(24,38,31));d=ImageDraw.Draw(im);chalk=(199,204,181);border(d,1024,512,chalk)
for n in range(4):
 x=31+n*250
 # Bold chalk contours with sparse hatch shading; avoid shrinking dense painting detail.
 cx=x+106;ground=345;d.line((x+12,ground,x+206,ground),fill=chalk,width=3)
 for j in range(14):d.line((x+16+j*13,ground+18,x+26+j*13,ground+6),fill=chalk,width=1)
 if n==0:
  d.ellipse((cx-25,ground+32,cx+25,ground+62),outline=chalk,width=4)
  d.line((cx+17,ground+56,cx+38,ground+72,cx+34,ground+88),fill=chalk,width=2)
 else:
  top=227 if n<3 else 209
  d.line((cx,ground+29,cx,top),fill=chalk,width=4)
  for side,yy in [(-1,292),(1,267)]:
   d.line((cx,yy,cx+side*42,yy-23,cx+side*49,yy-8,cx,yy+5),fill=chalk,width=3)
   for j in range(4):d.line((cx+side*(10+j*8),yy-3-j*4,cx+side*(13+j*8),yy-15-j*2),fill=chalk,width=1)
  if n==1:
   for j in range(6):d.line((x+33+j*29,158,x+22+j*29,188),fill=chalk,width=3)
   d.arc((x+51,117,x+160,169),180,355,fill=chalk,width=3)
  else:
   d.ellipse((x+144,141,x+182,179),outline=chalk,width=3)
   for j in range(12):
    angle=j*math.pi/6;d.line((x+163+math.cos(angle)*26,160+math.sin(angle)*26,x+163+math.cos(angle)*35,160+math.sin(angle)*35),fill=chalk,width=2)
  if n==3:
   for j in range(8):
    angle=j*math.pi/4;xx=cx+math.cos(angle)*21;yy=top+math.sin(angle)*21;d.ellipse((xx-9,yy-15,xx+9,yy+15),outline=chalk,width=2)
   d.ellipse((cx-8,top-8,cx+8,top+8),outline=chalk,width=2)
 roman(d,n+1,x+20,54,chalk);g=glyph(names[n],65,chalk,(0,0,0,0));im.paste(g,(x+135,42),g)
# Reproducible chalk dust, not smooth vector UI.
for _ in range(16000):
 x=R.randrange(1024);y=R.randrange(512)
 if R.random()<.3:im.putpixel((x,y),(24,38,31))
im.save(E/'ChalkSequence.png')
im=paper((1024,640),(198,177,130));d=ImageDraw.Draw(im);border(d,1024,640,INK)
for n in range(3):
 x=172+n*336;roman(d,n+1,x-12,42,INK)
 if n==0:
  d.arc((x-37,99,x+37,173),58,302,fill=INK,width=3);d.arc((x-10,96,x+47,176),80,280,fill=INK,width=2)
 else:
  g=glyph(['Light','Fire','Light'][n],96,INK,(0,0,0,0));im.paste(g,(x-48,98),g)
 if n==0:
  d.arc((x-53,207,x+53,313),40,320,fill=INK,width=3)
  for j in range(6):
   a=j*math.pi/3;xx=x+math.cos(a)*32;yy=289+math.sin(a)*32;d.ellipse((xx-14,yy-24,xx+14,yy+24),outline=INK,width=3)
   for k in range(4):d.line((xx-7+k*4,yy+10,xx-3+k*3,yy-11),fill=INK,width=1)
 elif n==1:
  d.polygon([(x,229),(x+54,270),(x+30,340),(x-30,340),(x-54,270)],outline=INK,width=3)
  for j in range(6):d.arc((x-40,249+j*12,x+40,277+j*12),0,180,fill=INK,width=2)
 else:
  d.arc((x-59,280,x+59,343),0,180,fill=INK,width=3)
  for j in range(18):
   xx=x+R.randrange(-52,53);yy=R.randrange(230,295);d.line((xx-3,yy,xx+3,yy),fill=INK,width=2);d.line((xx,yy-3,xx,yy+3),fill=INK,width=2)
 for j in range(9):d.line((x-46+j*10,357,x-36+j*10,345),fill=INK,width=1)
 if n<2:d.line((x+89,275,x+214,275),fill=INK,width=2);d.line((x+202,267,x+214,275,x+202,283),fill=INK,width=2)
# No centre crease, brackets or colored UI fragments.
x=512;d.rectangle((345,420,679,611),outline=INK,width=2);d.rectangle((x-9,507,x+9,566),outline=INK,width=3);d.arc((x-55,453,x+55,552),180,360,fill=INK,width=4);d.line((x-55,502,x+55,502),fill=INK,width=3)
d.line((x-75,445,x+75,587),fill=INK,width=7);d.line((x+75,445,x-75,587),fill=INK,width=7)
im.save(E/'PictureRecipe.png')
# Two sample scales in a single reproducible symbol reference plate.
im=paper((1536,320),(31,29,26))
for i,n in enumerate(['Earth','Water','Fire','Wind','Light','Shadow']):im.paste(glyph(n),(i*256,32))
im.save(F/'SymbolLineup.png')
# Original periodic heightfield -> tangent-space normal, no external water license.
import numpy as np
Y,X=np.mgrid[0:256,0:256]/256*math.tau
h=np.sin(X*5+np.sin(Y*3))*.5+np.sin(Y*7-X*2)*.25+np.sin(X*13+Y*11)*.08
dy,dx=np.gradient(h);v=np.stack([-dx*9,-dy*9,np.ones_like(h)],axis=-1);v/=np.linalg.norm(v,axis=-1,keepdims=True);Image.fromarray(np.uint8((v*.5+.5)*255)).save(F/'WaterNormal.png')
# Unity Labs CC0 Flame02. Preserve straight alpha, 16 columns x 4 rows, 64 frames.
im=Image.open(C/'Flame02/Flame02_16x4.tga');im.save(F/'Flame02.png')
print('S10f: six uniform alchemical vectors, four sepia plates, chalk/recipe, CC0 64-frame fire, original normal.')

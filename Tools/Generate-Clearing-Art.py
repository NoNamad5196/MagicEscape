"""Original stage-3 manuscript plates; deterministic PIL vector/hatch source, no external assets."""
from pathlib import Path
from PIL import Image, ImageDraw
import math, random
R=random.Random(5196)
G=(177,151,98); INK=(57,43,29)
B=Path('UnityProject/Assets/Art/S10b'); E=Path('UnityProject/Assets/Art/S10e')
def glyph(name,size=256,col=G,bg=(33,31,28,255)):
 im=Image.new('RGBA',(256,256),bg);d=ImageDraw.Draw(im)
 def ln(p):d.line(p,fill=col,width=11,joint='curve')
 if name in ('Earth','Water','Fire','Wind'):
  up=name in ('Fire','Wind');p=[(128,45),(211,198),(45,198),(128,45)] if up else [(45,57),(211,57),(128,210),(45,57)];ln(p)
  if name=='Wind':ln([(79,126),(177,126)])
  if name=='Earth':ln([(79,133),(177,133)])
 elif name=='Light':
  d.ellipse((49,49,207,207),outline=col,width=11);d.ellipse((117,117,139,139),fill=col)
 else:
  # Saturn / lead: cross with descending curved sickle, not a crescent moon.
  ln([(99,39),(99,205)]);ln([(66,77),(137,77)])
  d.arc((96,99,194,207),185,365,fill=col,width=11)
  ln([(192,153),(182,206),(195,213)])
 return im.resize((size,size),Image.Resampling.LANCZOS)

for name in ['Earth','Water','Fire','Wind','Light','Shadow']:glyph(name).save(B/(name+'.png'))
def paper(size):
 im=Image.new('RGB',size); im.putdata([tuple(c+R.randrange(-5,6) for c in (205,183,139)) for _ in range(size[0]*size[1])]);return im
def sun(d,x,y,r):
 d.ellipse((x-r,y-r,x+r,y+r),outline=INK,width=4)
 for j in range(16):
  a=j*math.pi/8;d.line((x+math.cos(a)*(r+6),y+math.sin(a)*(r+6),x+math.cos(a)*(r+20),y+math.sin(a)*(r+20)),fill=INK,width=3)
def ground(d,x,y):
 d.line((x-65,y,x+65,y),fill=INK,width=4)
 for j in range(13):d.line((x-60+j*10,y+14,x-48+j*10,y+2),fill=INK,width=1)
# III is drought / heat, not IV's blooming flower.
im=paper((512,512));d=ImageDraw.Draw(im)
for b in (12,19):d.rectangle((b,b,511-b,511-b),outline=INK,width=2)
sun(d,379,114,43)
for yy in (329,352,379,403,430,460):
 d.line([(34,yy),(102,yy-5),(179,yy+7),(245,yy-7),(319,yy+5),(477,yy-3)],fill=INK,width=2)
for x in range(49,475,46):
 d.line([(x,330),(x+12,357),(x-8,392),(x+21,421),(x+7,471)],fill=INK,width=3)
 for j in range(3):d.line((x-13+j*6,448,x+j*6,432),fill=INK,width=1)
d.line([(251,373),(250,299),(240,242),(225,220)],fill=INK,width=5)
for x,y,side in [(245,279,-1),(249,308,1)]:
 d.line([(x,y),(x+side*32,y+11),(x+side*47,y+42),(x+side*10,y+23),(x,y)],fill=INK,width=3)
 for j in range(4):d.line((x+side*(8+j*6),y+8+j*5,x+side*(16+j*7),y+13+j*5),fill=INK,width=1)
for x in (104,166,315,409):
 pts=[(x+math.sin(k*.5)*8,300-k*5) for k in range(22)];d.line(pts,fill=INK,width=2)
g=glyph('Fire',105,INK,(0,0,0,0));im.paste(g,(35,35),g)
im.save(B/'Story_3.png')
print('Stage 3: bold matching brass glyphs, heat woodcut, woven cloth.')

# Woven linen with broad vertical folds (not timber plank grain).
im=Image.new('RGB',(256,256)); px=im.load()
for y in range(256):
 for x in range(256):
  v=int(120+19*math.cos(x*math.tau/43)+R.randrange(-5,6)+(4 if x%4==0 else 0)+(3 if y%4==0 else 0))
  px[x,y]=(v,int(v*.79),int(v*.55))
im.save(E/'ClearingLinen.png')

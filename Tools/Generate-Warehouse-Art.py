from pathlib import Path
from PIL import Image, ImageDraw
import math, random
out=Path('UnityProject/Assets/Art/Warehouse');out.mkdir(exist_ok=True)
# Vector-authored monochrome incision masks, one 11px stroke vocabulary.
def motif(name):
 im=Image.new('RGB',(256,256),(2,2,2));d=ImageDraw.Draw(im);c=(255,255,255)
 def line(p):d.line(p,fill=c,width=11,joint='curve')
 if name=='Earth':
  line([(34,194),(82,91),(123,150),(166,61),(224,194),(34,194)]);line([(152,89),(168,115),(181,97)])
 elif name=='Water':
  p=[(128,32),(116,54),(86,94),(60,134),(57,165),(69,193),(96,211),(128,217),(160,211),(187,193),(199,165),(196,134),(170,94),(140,54),(128,32)];line(p)
  line([(91,108),(78,130)]) # Short upper-left glint, never a smiling arc.
 elif name=='Fire':
  line([(127,29),(153,87),(180,62),(190,119),(213,151),(208,186),(182,213),(78,213),(48,182),(46,147),(70,103),(77,155),(100,123),(127,29)])
  line([(111,208),(98,183),(109,157),(129,133),(145,163),(158,183),(147,208)])
 elif name=='Light':
  d.ellipse((82,82,174,174),outline=c,width=11)
  for i in range(12):
   a=i*math.tau/12;line([(128+math.cos(a)*67,128+math.sin(a)*67),(128+math.cos(a)*100,128+math.sin(a)*100)])
 elif name=='Wind':
  line([(30,91),(157,91),(178,80),(184,60),(175,42),(155,38),(140,48)])
  line([(30,130),(196,130),(217,120),(224,100),(214,83)])
  line([(47,169),(142,169),(162,181),(165,200),(154,215),(133,215)])
 else:
  # Closed crescent outline, no Saturn cross or star.
  p=[]
  for i in range(101):
   a=math.radians(58+244*i/100);p.append((128+92*math.cos(a),128+92*math.sin(a)))
  for i in range(101):
   a=math.radians(270-180*i/100);p.append((177+79*math.cos(a),128+79*math.sin(a)))
  p.append(p[0]);line(p)
 return im
for n in ['Earth','Water','Fire','Light','Wind','Shadow']:motif(n).save(out/('Pictogram_'+n+'.png'))
# Clean, large chalk narrative vignettes: no element motifs/names imported here.
r=random.Random(5196);im=Image.new('RGB',(1536,768));im.putdata([tuple(c+r.randrange(-4,5) for c in (24,38,31)) for _ in range(1536*768)])
d=ImageDraw.Draw(im);c=(211,218,194)
def ln(p,w=6):d.line(p,fill=c,width=w,joint='curve')
for n in range(4):
 x=192+n*384
 if n<3:
  for j in range(n+1):ln([(x-12*n+24*j,62),(x-12*n+24*j,112)])
 else:ln([(x-35,62),(x-35,112)]);ln([(x-6,62),(x+16,112),(x+38,62)])
 # Scene box / baseline, not an empty slot.
 ln([(x-147,565),(x-66,552),(x+26,562),(x+145,550)])
 if n==0:
  # Pointed almond seed, split seam and a single tiny shoot: no head/legs silhouette.
  ln([(x-48,625),(x-32,603),(x-4,593),(x+25,599),(x+45,617),(x+25,639),(x-5,647),(x-32,640),(x-48,625)])
  ln([(x-40,626),(x-10,618),(x+17,619),(x+38,618)],4)
  ln([(x-3,594),(x+1,581),(x+8,570)],4)
  for j in range(7):ln([(x-130+j*41,598),(x-115+j*41,581)],3)
 else:
  top=355 if n==3 else 420
  ln([(x,614),(x-4,517),(x+2,top)])
  if n==2:
   ln([(x-1,479),(x-43,487),(x-65,520),(x-24,501),(x-1,479)],5)
   ln([(x-3,514),(x+42,527),(x+61,554),(x+18,537),(x-3,514)],5)
  else:
   ln([(x,480),(x-64,441),(x-80,455),(x-39,492),(x,480)],5)
   ln([(x,510),(x+57,469),(x+78,479),(x+38,519),(x,510)],5)
 if n==1:
  for j in range(5):d.arc((x-140+j*47,199,x-64+j*47,270),180,350,fill=c,width=5)
  for j in range(7):
   y=305+(j%2)*39;ln([(x-121+j*39,y),(x-137+j*39,y+63)],5)
 if n==2:
  # Heat scene: small sun in sky + cracked earth + wilt, never flame pictogram.
  d.ellipse((x+53,226,x+115,288),outline=c,width=5)
  for j in range(3):ln([(x+55+j*22,312),(x+49+j*22,336),(x+60+j*22,361)],3)
  for j in range(5):
   xx=x-130+j*61;ln([(xx,563),(xx+16,598),(xx-8,633),(xx+21,677),(xx+10,712)],4)
   ln([(xx+10,603),(xx+39,619)],3)
 if n==3:
  # Bloom and broad shafts, not isolated sun / element icon.
  for j in range(8):
   a=j*math.tau/8;xx=x+math.cos(a)*30;yy=355+math.sin(a)*30
   d.ellipse((xx-15,yy-22,xx+15,yy+22),outline=c,width=5)
  d.ellipse((x-12,343,x+12,367),outline=c,width=4)
  for dx in (-120,-72,72,120):ln([(x+dx,210),(x+dx*.38,292)],4)
 if n<3:
  ln([(x+163,407),(x+207,407)],4);ln([(x+196,395),(x+208,407),(x+196,419)],4)
# restrained grain in chalk strokes only
p=im.load()
for _ in range(8000):
 x=r.randrange(1536);y=r.randrange(768)
 if p[x,y]==c:p[x,y]=(151,165,142)
im.save(out/'ChalkScenes.png')
# Warehouse recipe uses narrative ingredient drawings only (no old Fire triangle).
recipe=Image.new('RGB',(1024,640),(205,183,139));rd=ImageDraw.Draw(recipe);ink=(57,43,29)
def rline(p,w=5):rd.line(p,fill=ink,width=w,joint='curve')
for n in range(3):
 x=171+n*341
 # Arabic recipe steps are independent of P1 scene order.
 for j in range(n+1):rline([(x-12*n+j*24,35),(x-12*n+j*24,76)])
 rd.line((x-150,105,x+150,105),fill=ink,width=3)
 if n==0:
  # Moon-grown petals, not the P1 light pictogram.
  rd.arc((x-47,135,x+47,229),60,300,fill=ink,width=5)
  rd.arc((x-12,135,x+58,229),70,290,fill=ink,width=4)
  for j in range(6):
   a=j*math.tau/6;xx=x+math.cos(a)*46;yy=356+math.sin(a)*46
   rd.ellipse((xx-22,yy-33,xx+22,yy+33),outline=ink,width=5)
  rd.ellipse((x-13,343,x+13,369),outline=ink,width=4)
 elif n==1:
  # Overlapping shed scales; flame-shaped heat streaks, never alchemical triangle.
  for yy in range(260,401,45):
   for dx in (-55,0,55):rd.arc((x+dx-31,yy,x+dx+31,yy+73),0,180,fill=ink,width=5)
  for dx in (-65,0,65):rline([(x+dx,221),(x+dx-11,192),(x+dx+4,162),(x+dx,138)],4)
 else:
  rline([(x-55,409),(x-62,285),(x-30,255),(x-30,223),(x+30,223),(x+30,255),(x+62,285),(x+55,409),(x-55,409)])
  for dx,yy in [(-26,305),(22,324),(-12,365),(29,382)]:
   rline([(x+dx-10,yy),(x+dx+10,yy)],3);rline([(x+dx,yy-10),(x+dx,yy+10)],3)
 # Direction under ingredient, no element captions.
 if n<2:rline([(x+135,350),(x+186,350)]);rline([(x+173,338),(x+186,350),(x+173,362)])
# Crossed-out mushroom: unmistakable excluded ingredient.
rd.arc((416,470,608,564),180,360,fill=ink,width=5);rline([(416,517),(608,517)]);rline([(491,520),(491,586),(534,586),(534,520)])
rline([(433,464),(589,600)],7);rline([(589,464),(433,600)],7)
recipe.save(out/'PictureRecipe.png')

# Warehouse-only frame I: retain the etched parchment vocabulary without changing Classroom.
rng=random.Random(5196)
frame=Image.new('RGB',(512,512));frame.putdata([tuple(v+rng.randrange(-6,7) for v in (173,151,110)) for _ in range(512*512)])
fd=ImageDraw.Draw(frame);ink=(45,36,27)
for y in range(40,326,5):fd.line([(28,y),(155,y-2),(310,y+1),(484,y)],fill=(113,99,73),width=1)
for _ in range(280):
 x=rng.randrange(30,480);y=rng.randrange(343,480);fd.line([(x,y),(x+10,y-7)],fill=(80,64,44),width=1)
fd.line([(26,335),(105,318),(220,329),(330,314),(485,338)],fill=ink,width=3)
fd.line([(210,394),(226,373),(252,362),(278,367),(295,385),(278,407),(251,416),(225,409),(210,394)],fill=ink,width=3,joint='curve')
fd.line([(216,394),(243,386),(270,388),(289,385)],fill=ink,width=2)
fd.line([(252,362),(255,348),(262,338)],fill=ink,width=2)
for off in (16,22):fd.rectangle((off,off,511-off,511-off),outline=(79,57,33),width=2)
frame.save(out/'Story_1.png')

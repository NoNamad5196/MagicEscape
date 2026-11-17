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
# Stage 9: compose reviewed image-generated textures; never procedurally redraw P1 art.
# Numbering is separate Unity text, not baked into any generated picture.
im=Image.new('RGB',(1536,768),(20,20,20))
for n in range(4):
 picture=Image.open(out/'Textures'/f'Chalk_{n+1}.png').convert('RGB')
 picture=picture.resize((360,510),Image.Resampling.LANCZOS)
 im.paste(picture,(12+n*384,160))
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

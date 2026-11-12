"""Reproducible original copperplate-style art and worn UI surfaces (Pillow).
No image inputs, AI generation, purchased art or system fonts. Seed 5196.
"""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFilter
import math, random
OUT=Path('UnityProject/Assets/Art/S10b'); OUT.mkdir(parents=True,exist_ok=True)
D=Path('UnityProject/Assets/Art/S10d'); D.mkdir(parents=True,exist_ok=True)
rng=random.Random(5196)
GOLD=(184,143,78); INK=(45,36,27)

def paper(size,base):
 im=Image.new('RGB',size); w,h=size
 im.putdata([tuple(max(0,min(255,c+rng.randint(-8,8)-int(22*((x/w-.5)**2+(y/h-.5)**2)))) for c in base) for y in range(h) for x in range(w)])
 return im

def etch(d,pts,fill=INK,width=2):
 # Slightly drifting parallel burin cuts, never filled emoji silhouettes.
 for k in range(width):
  d.line([(x+rng.uniform(-.7,.7),y+k+rng.uniform(-.5,.5)) for x,y in pts],fill=fill,width=1)

def symbol(d,name,cx=128,cy=128,s=1,col=GOLD):
 def ln(p,w=3):etch(d,[(cx+(x-128)*s,cy+(y-128)*s) for x,y in p],col,w)
 if name=='Earth':
  ln([(49,182),(105,65),(153,179),(183,112),(213,182),(49,182)])
  for i in range(13): ln([(70+i*5,175),(108+i*2,112+i*4)],1)
  for y in (195,203):ln([(58,y),(199,y)],1)
 elif name=='Water':
  ln([(128,47),(111,79),(79,132),(75,162),(91,187),(124,199),(157,188),(179,161),(175,137),(145,87),(128,47)])
  for i in range(8):ln([(91+i*4,145),(92+i*4,171),(109+i*4,187)],1)
  for y in (215,224):ln([(65,y),(95,y-6),(128,y),(161,y-6),(193,y)],1)
 elif name=='Fire':
  ln([(128,40),(153,88),(149,121),(176,96),(187,151),(179,179),(153,200),(110,201),(79,176),(76,144),(99,93),(100,142),(121,119),(128,40)])
  for i in range(9):ln([(98+i*6,181),(112+i*4,143-i*2),(120+i*3,180)],1)
 elif name=='Wind':
  for i in range(3):
   ln([(49,98+i*35),(163,98+i*35),(192,88+i*35),(194,73+i*35),(181,67+i*35),(170,76+i*35)],2)
 elif name=='Light':
  pts=[(128+math.cos(i*math.pi/40)*30,128+math.sin(i*math.pi/40)*30) for i in range(81)];ln(pts)
  for i in range(24):
   a=i*math.pi/12;ln([(128+math.cos(a)*43,128+math.sin(a)*43),(128+math.cos(a)*(88 if i%2==0 else 68),128+math.sin(a)*(88 if i%2==0 else 68))],1)
 else:
  ln([(150,48),(110,60),(83,93),(77,134),(93,174),(125,197),(165,204),(139,180),(122,146),(119,106),(132,72),(150,48)])
  for i in range(10):ln([(85+i*4,123+i*6),(109+i*4,165+i*3)],1)
 # Four angular corner cuts, no circular badge.
 for x,y,sx,sy in [(27,27,1,1),(229,27,-1,1),(27,229,1,-1),(229,229,-1,-1)]:
  ln([(x,y+sy*18),(x,y),(x+sx*18,y)],1)

for name in ['Earth','Water','Fire','Wind','Light','Shadow']:
 im=paper((256,256),(50,43,33));symbol(ImageDraw.Draw(im),name);im.save(OUT/(name+'.png'))

for num in range(1,5):
 im=paper((512,512),(173,151,110));d=ImageDraw.Draw(im)
 # Engraved sky: closely spaced irregular horizontal lines.
 for y in range(40,326,5):
  etch(d,[(28,y),(155,y+rng.randint(-3,3)),(310,y-2),(484,y+1)],(113,99,73),1)
 for i in range(280):
  x=rng.randrange(30,480);y=rng.randrange(330,480)
  etch(d,[(x,y),(x+10,y-7)],(80,64,44),1)
 etch(d,[(26,335),(105,318),(220,329),(330,314),(485,338)],INK,3)
 if num==1:
  # Seed visibly below soil, with fine roots.
  d.ellipse((226,372,287,414),fill=(124,85,44),outline=INK,width=3)
  for i in range(9):etch(d,[(240+i*4,379),(236+i*4,401)],(65,49,31),1)
  etch(d,[(277,400),(293,419),(288,441),(310,457)],INK,2)
  etch(d,[(291,430),(270,442),(268,460)],INK,1)
 else:
  etch(d,[(257,387),(250,315),(259,252 if num<4 else 202)],INK,4)
  for x,y,side in [(252,300,-1),(255,274,1)]:
   etch(d,[(x,y),(x+side*58,y-28),(x+side*75,y-22),(x+side*46,y+4),(x,y)],INK,2)
   for i in range(9):etch(d,[(x+side*i*6,y-i*2),(x+side*(i*6+12),y-i*2-12)],(76,67,40),1)
 if num==2:
  for i in range(24):
   x=rng.randrange(45,470);y=rng.randrange(84,255);etch(d,[(x,y),(x-14,y+40)],(55,66,65),2)
  for i in range(6):
   x=65+i*67; d.arc((x-30,53,x+70,124),180,340,fill=INK,width=2)
 if num>=3:
  # Sun rendered as copperplate disk with many fine rays, not a clipart icon.
  d.ellipse((329,75,404,150),fill=(189,154,81),outline=INK,width=2)
  for i in range(60):
   a=i*math.pi/30;r=57+(i%3)*8
   etch(d,[(366+math.cos(a)*43,112+math.sin(a)*43),(366+math.cos(a)*r,112+math.sin(a)*r)],(103,77,37),1)
 if num==4:
  for i in range(12):
   a=i*math.pi/6;x=258+math.cos(a)*29;y=205+math.sin(a)*29
   d.ellipse((x-12,y-20,x+12,y+20),fill=(185,151,107),outline=INK,width=2)
   etch(d,[(x,y-12),(258,205)],(102,71,47),1)
  d.ellipse((247,194,270,217),fill=(117,78,36),outline=INK,width=2)
 for off in (16,22):d.rectangle((off,off,511-off,511-off),outline=(79,57,33),width=2)
 # Scored varnish/craquelure: restrained hairlines across image.
 for i in range(40):
  x=rng.randrange(512);y=rng.randrange(512);pts=[(x,y)]
  for j in range(rng.randrange(3,8)):x+=rng.randrange(-13,14);y+=rng.randrange(7,22);pts.append((x,y))
  d.line(pts,fill=(107,87,58),width=1)
 # Canvas fibres and darkened edges / yellow varnish.
 p=im.load()
 for y in range(512):
  for x in range(512):
   edge=max(abs(x-256),abs(y-256))/256
   factor=1-.24*edge**6;grain=rng.uniform(-6,6)+(2 if x%3==0 else 0)
   p[x,y]=tuple(int(max(0,min(255,c*factor+grain))) for c in p[x,y])
 im.save(OUT/('Story_'+str(num)+'.png'))

# Quiet cloth panels and wood slots, thin antiqued gold inlay.
for name,size,base in [('ClothPanel',(512,256),(30,27,23)),('WoodSlot',(128,128),(49,36,24)),('SelectedSlot',(128,128),(66,47,28))]:
 im=paper(size,base);d=ImageDraw.Draw(im);w,h=size
 for y in range(0,h,3):d.line((0,y,w,y),fill=tuple(c+3 for c in base))
 for off in (5,9):d.rectangle((off,off,w-1-off,h-1-off),outline=(153,119,65) if name=='SelectedSlot' else (100,79,48),width=1)
 if name=='SelectedSlot':
  for x,y,sx,sy in [(13,13,1,1),(114,13,-1,1),(13,114,1,-1),(114,114,-1,-1)]:d.line((x,y+sy*14,x,y,x+sx*14,y),fill=(208,176,103),width=2)
 if name=='SelectedSlot':
  im=im.convert('RGBA');ImageDraw.Draw(im).rectangle((16,16,w-17,h-17),fill=(0,0,0,0))
 im.save(D/(name+'.png'))
# Material-scale stains rather than ultra-sharp photo detail.
paper((256,256),(148,137,117)).filter(ImageFilter.GaussianBlur(.65)).save(D/'Patina.png')
im=Image.new('RGBA',(256,256));d=ImageDraw.Draw(im)
for i in range(10):
 a=i*math.pi/18;d.line((0,0,math.cos(a)*256,math.sin(a)*256),fill=(163,154,128,85),width=1)
for radius in range(25,340,22):
 pts=[(math.cos(i*math.pi/36)*radius,math.sin(i*math.pi/36)*radius) for i in range(19)]
 d.line(pts,fill=(163,154,128,70),width=1)
im.save(D/'Cobweb.png')
im=Image.new('RGBA',(128,128));d=ImageDraw.Draw(im);d.ellipse((57,57,71,71),fill=(216,199,151,150));im.save(D/'Crosshair.png')
# Retain the existing circle/board filenames but unify their colors and finish.
im=Image.new('RGBA',(512,512));d=ImageDraw.Draw(im)
for r in (245,225,173):d.ellipse((256-r,256-r,256+r,256+r),outline=(154,132,82,180),width=2)
for i in range(12):
 a=i*math.pi/6;x=256+math.cos(a)*201;y=256+math.sin(a)*201;d.line((x-3,y-6,x+3,y+6),fill=GOLD,width=2)
pts=[(256+math.sin(i*math.pi/3)*164,256+math.cos(i*math.pi/3)*164) for i in range(6)]
for indices in ((0,2,4,0),(1,3,5,1)):d.line([pts[i] for i in indices],fill=(164,143,94,200),width=2)
im.save(OUT/'MagicCircle.png')
paper((512,512),(29,43,34)).save(OUT/'Chalkboard.png')
print('S10d: 4 botanical copperplates, 6 engraved glyphs, cloth/wood UI, patina, cobweb, crosshair.')

paper((64,64),(32,16,5)).save(D/'BronzeEmission.png')

# Uneven soot glaze with a completely feathered alpha edge, not an opaque decal rectangle.
im=Image.new('RGBA',(128,256));px=im.load()
for y in range(256):
 for x in range(128):
  radius=((x-64)/63)**2+((y-128)/125)**2
  alpha=int(max(0,1-radius)**2*rng.uniform(12,58))
  px[x,y]=(24,17,11,alpha)
im.save(D/'Soot.png')

# S10e: bold elemental points, original chalk pictograms and particle masks.
E=Path('UnityProject/Assets/Art/S10e');E.mkdir(parents=True,exist_ok=True)
colors={'Earth':(104,197,67),'Water':(65,171,246),'Fire':(246,100,28),'Wind':(152,190,153),'Light':(248,211,87),'Shadow':(169,87,205)}
for name,col in colors.items():
 im=paper((256,256),(36,30,23));d=ImageDraw.Draw(im)
 # Triple, closely offset cuts make the same engraved vocabulary readable at distance.
 for offset in (-2,0,2):symbol(d,name,128+offset,128,1,col)
 symbol(d,name,128,128,.91,GOLD)
 im.save(OUT/(name+'.png'))

# Rework existing story plates without erasing the hand-cut surface character.
from PIL import ImageEnhance, ImageOps
for num in range(1,5):
 im=Image.open(OUT/('Story_'+str(num)+'.png'))
 # Dark strokes are dilated before contrast enhancement; broad soil/plant silhouettes survive mipmapping.
 im=im.filter(ImageFilter.MinFilter(3));im=ImageEnhance.Contrast(im).enhance(1.65)
 im.save(OUT/('Story_'+str(num)+'.png'))

def arrow(d,x,y,length,col,width=6):
 d.line((x,y,x+length,y),fill=col,width=width);d.line((x+length-12,y-12,x+length,y,x+length-12,y+12),fill=col,width=width)
def roman(d,n,x,y,col):
 if n<4:
  for j in range(n):d.line((x+j*12,y,x+j*12,y+30),fill=col,width=5)
 else:
  d.line((x-8,y,x-8,y+30),fill=col,width=5);d.line((x+5,y,x+17,y+30,x+29,y),fill=col,width=5)
def seed(d,x,y,stage,col):
 d.line((x-40,y+35,x+40,y+35),fill=col,width=5)
 if stage==1:d.ellipse((x-11,y+48,x+11,y+62),outline=col,width=5)
 else:
  d.line((x,y+56,x,y-12-(stage-2)*17),fill=col,width=6)
  d.line((x,y+15,x-26,y-3,x-30,y+8,x,y+24,x+29,y-5,x+32,y+7,x,y+28),fill=col,width=4)
 if stage==2:
  for j in range(4):d.line((x-35+j*24,y-60,x-43+j*24,y-40),fill=col,width=4)
 if stage>=3:
  d.ellipse((x+22,y-70,x+49,y-43),outline=col,width=4)
 if stage==4:
  for j in range(6):
   a=j*math.pi/3;xx=x+math.cos(a)*13;yy=y-47+math.sin(a)*13
   d.ellipse((xx-8,yy-8,xx+8,yy+8),outline=col,width=3)
im=paper((1024,512),(24,43,34));d=ImageDraw.Draw(im);chalk=(215,222,190)
for n in range(1,5):
 x=132+(n-1)*252;d.rectangle((x-95,86,x+95,410),outline=chalk,width=5);roman(d,n,x-15,110,chalk);seed(d,x,280,n,chalk)
 if n<4:arrow(d,x+102,247,45,chalk,5)
im.save(E/'ChalkSequence.png')

im=paper((1024,640),(207,186,139));d=ImageDraw.Draw(im)
for x in (12,505,1010):d.line((x,15,x,625),fill=(107,75,42),width=4)
for n in range(1,4):
 x=167+(n-1)*334;roman(d,n,x-10,40,INK)
 if n==1:
  d.ellipse((x-53,113,x+23,189),fill=(66,97,135));d.ellipse((x-25,96,x+44,165),fill=(207,186,139))
  for j in range(5):
   a=j*math.pi*2/5;xx=x+math.cos(a)*28;yy=290+math.sin(a)*28;d.ellipse((xx-16,yy-25,xx+16,yy+25),fill=(107,161,196),outline=INK,width=4)
 elif n==2:
  symbol(d,'Fire',x,196,.6,(203,78,22));d.polygon([(x,265),(x+53,301),(x+28,354),(x-28,354),(x-53,301)],fill=(184,72,28),outline=INK,width=5)
  for j in range(4):d.arc((x-38,279+j*15,x+38,306+j*15),0,180,fill=GOLD,width=3)
 else:
  for j in range(10):
   xx=x+rng.randrange(-62,63);yy=160+rng.randrange(0,185)
   d.line((xx-9,yy,xx+9,yy),fill=(93,123,157),width=4);d.line((xx,yy-10,xx,yy+10),fill=INK,width=3)
  d.arc((x-64,316,x+64,363),0,180,fill=INK,width=5)
 if n<3:arrow(d,x+100,264,90,INK)
# Poison mushroom: pictogram and unequivocal crossed-out candidate, no prose.
x=512;d.rectangle((355,421,669,618),outline=(127,87,51),width=3)
d.rectangle((x-10,514,x+10,568),fill=(149,126,91),outline=INK,width=3)
d.pieslice((x-56,458,x+56,550),180,360,fill=(178,47,35),outline=INK,width=4)
for xx,yy in [(x-30,495),(x,481),(x+30,497)]:d.ellipse((xx-5,yy-5,xx+5,yy+5),fill=(236,216,167))
d.line((x-76,444,x+76,589),fill=(148,27,23),width=12);d.line((x+76,444,x-76,589),fill=(148,27,23),width=12)
im.save(E/'PictureRecipe.png')
for key in ('Soft','Spark','Drop','Leaf','Bubble'):
 im=Image.new('RGBA',(64,64));d=ImageDraw.Draw(im)
 if key=='Soft':
  for y in range(64):
   for x in range(64):
    r=((x-31.5)**2+(y-31.5)**2)**.5/31.5;im.putpixel((x,y),(255,255,255,int(max(0,1-r)**2*220)))
 elif key=='Spark':d.polygon([(32,2),(36,26),(62,32),(36,37),(32,62),(27,37),(2,32),(27,26)],fill=(255,255,255,240))
 elif key=='Drop':d.polygon([(32,2),(48,35),(48,46),(40,57),(24,57),(16,46),(16,35)],fill=(255,255,255,220))
 elif key=='Leaf':d.ellipse((12,6,48,58),fill=(255,255,255,220));d.line((22,51,39,13),fill=(70,100,70,130),width=3)
 else:d.ellipse((7,7,57,57),outline=(255,255,255,180),width=3);d.arc((13,13,51,51),200,270,fill=(255,255,255,220),width=4)
 im.save(E/(key+'.png'))
print('S10e: bold six elements, contrasted plates, picture-only chalk/recipe, five original particle masks.')

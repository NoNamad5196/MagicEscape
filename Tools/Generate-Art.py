"""Original flat/vector-style MagicEscape art. Run from repository root; Pillow only.
All outputs <=512 px. No external image inputs, fonts, or model generation.
"""
from pathlib import Path
from PIL import Image, ImageDraw
import math, random
OUT=Path('UnityProject/Assets/Art/S10b');OUT.mkdir(parents=True,exist_ok=True)
GOLD='#ebc578'; INK='#17273b'
def rune(name,size=256):
 im=Image.new('RGB',(256,256),INK);d=ImageDraw.Draw(im);d.ellipse((8,8,248,248),outline=GOLD,width=7)
 if name=='Earth':
  d.polygon([(50,173),(107,70),(139,130),(174,91),(214,173)],outline=GOLD,width=9);d.line((58,194,202,194),fill=GOLD,width=8)
 elif name=='Water':
  d.polygon([(128,40),(69,124),(65,167),(94,201),(150,208),(189,176),(190,136)],fill='#79cfea');d.arc((89,115,176,190),15,145,fill=INK,width=9)
 elif name=='Fire':
  d.polygon([(133,36),(190,123),(183,181),(149,209),(98,204),(66,164),(94,94),(99,143),(129,115)],fill='#eea362');d.polygon([(131,127),(153,174),(132,196),(111,174)],fill=GOLD)
 elif name=='Wind':
  for y in (88,128,168):d.arc((50,y-20,206,y+27),185,355,fill=GOLD,width=9)
 elif name=='Light':
  d.ellipse((92,92,164,164),fill=GOLD)
  for i in range(8):
   a=i*math.pi/4;d.line((128+math.cos(a)*60,128+math.sin(a)*60,128+math.cos(a)*93,128+math.sin(a)*93),fill=GOLD,width=9)
 else:
  d.ellipse((65,47,190,210),fill='#b8a5e5');d.ellipse((109,35,218,179),fill=INK)
 return im.resize((size,size))
for n in ['Earth','Water','Fire','Wind','Light','Shadow']:rune(n).save(OUT/(n+'.png'))
for num in range(1,5):
 im=Image.new('RGB',(512,512),['#273743','#233e58','#694a39','#385454'][num-1]);d=ImageDraw.Draw(im)
 # A framed storybook landscape, deliberately simplified rather than photoreal.
 d.polygon([(0,315),(95,276),(219,328),(382,285),(512,317),(512,512),(0,512)],fill='#6d6746')
 d.polygon([(0,364),(137,333),(295,372),(512,343),(512,512),(0,512)],fill='#473b31')
 if num==1:
  d.ellipse((223,367,290,417),fill='#c59a66');d.arc((233,374,279,414),205,335,fill='#efd49a',width=6)
 else:
  d.line((256,386,256,264 if num<4 else 219),fill='#a9bd7c',width=13)
  d.ellipse((180,274,258,309),fill='#83a56f');d.ellipse((257,251,334,284),fill='#a4ba7c')
 if num==2:
  for x,y in [(100,110),(175,90),(300,113),(380,90)]:d.ellipse((x-50,y-15,x+70,y+38),fill='#92a7b7')
  for x in range(75,460,58):
   for y in (184,240):d.line((x+14,y,x,y+27),fill='#88cad6',width=6)
 if num>=3:
  d.ellipse((320,65,424,169),fill='#efcb7d')
  for i in range(12):
   a=i*math.pi/6;d.line((372+math.cos(a)*64,117+math.sin(a)*64,372+math.cos(a)*89,117+math.sin(a)*89),fill='#e0ad61',width=5)
 if num==4:
  for i in range(8):
   a=i*math.pi/4;x=256+math.cos(a)*37;y=211+math.sin(a)*37;d.ellipse((x-21,y-21,x+21,y+21),fill='#e4c7d7')
  d.ellipse((237,192,275,230),fill='#f0cd77')
  for x,y in [(88,180),(396,227),(125,265),(350,358)]:d.line((x-8,y,x+8,y),fill='#ead99d',width=3);d.line((x,y-8,x,y+8),fill='#ead99d',width=3)
 d.rectangle((16,16,496,496),outline=GOLD,width=5);im.save(OUT/('Story_'+str(num)+'.png'))
im=Image.new('RGBA',(512,512),(0,0,0,0));d=ImageDraw.Draw(im)
for r in (245,225,173):d.ellipse((256-r,256-r,256+r,256+r),outline='#89d8ed',width=4)
for i in range(12):
 a=i*math.pi/6; x=256+math.cos(a)*200;y=256+math.sin(a)*200;d.ellipse((x-5,y-5,x+5,y+5),fill=GOLD)
pts=[(256+math.sin(i*math.pi/3)*164,256+math.cos(i*math.pi/3)*164) for i in range(6)]
for indices in ((0,2,4,0),(1,3,5,1)):d.line([pts[i] for i in indices],fill='#89d8ed',width=5)
im.save(OUT/'MagicCircle.png')
rng=random.Random(5196);im=Image.new('RGB',(512,512)); im.putdata([(24+rng.randrange(9),54+rng.randrange(12),44+rng.randrange(9)) for _ in range(512*512)]);im.save(OUT/'Chalkboard.png')

"""Original project-native procedural textures. No borrowed icon/font/image inputs."""
from PIL import Image,ImageDraw
from pathlib import Path
import math,random
random.seed(103)
out=Path('UnityProject/Assets/Art/S10c');out.mkdir(exist_ok=True)
im=Image.new('RGB',(256,512));p=im.load()
for y in range(512):
 for x in range(256):
  fold=.78+.17*math.sin(x/15)+random.uniform(-.04,.04)
  p[x,y]=tuple(int(v*fold) for v in (65,13,27))
d=ImageDraw.Draw(im)
for x in [14,23,232,241]:d.line((x,8,x,504),fill=(170,124,47),width=2)
for y in [18,29,480,491]:d.line((14,y,241,y),fill=(170,124,47),width=2)
d.polygon([(128,130),(191,167),(180,266),(128,315),(76,266),(65,167)],fill=(30,30,49),outline=(205,163,69),width=5)
d.ellipse((92,177,164,249),outline=(204,162,65),width=3)
d.polygon([(128,155),(140,207),(164,218),(139,229),(128,277),(116,229),(92,218),(116,207)],fill=(213,174,79))
im.save(out/'AcademyCloth.png')
im=Image.new('RGB',(512,512));p=im.load()
for y in range(512):
 for x in range(512):
  n=random.random()*2;p[x,y]=(int(5+n),int(10+n+y/90),int(25+n+y/30))
d=ImageDraw.Draw(im)
for _ in range(105):
 x=random.randrange(512);y=random.randrange(512);d.ellipse((x,y,x+1,y+1),fill=(80,110,145))
d.ellipse((327,61,430,164),fill=(164,192,210));d.ellipse((342,54,433,149),fill=(13,22,42))
im.save(out/'NightGlass.png')

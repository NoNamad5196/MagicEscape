from pathlib import Path
from PIL import Image, ImageDraw
import random, math
# Warehouse-only plate. Deliberately no imports of glyph/story/name generators.
r=random.Random(5196)
im=Image.new('RGB',(1024,512));im.putdata([tuple(c+r.randrange(-5,6) for c in (24,38,31)) for _ in range(1024*512)])
d=ImageDraw.Draw(im);chalk=(199,204,181)
for n in range(4):
 x=128+n*256
 # Hand-drawn Roman numerals and empty, slightly irregular chalk circles only.
 strokes=[[(x-12+12*j,120),(x-12+12*j,152)] for j in range(n+1)] if n<3 else [[(x-20,120),(x-20,152)],[(x-4,120),(x+10,152),(x+25,120)]]
 for stroke in strokes:d.line(stroke,fill=chalk,width=5)
 pts=[(x+65*math.cos(a)+r.uniform(-1.3,1.3),290+65*math.sin(a)+r.uniform(-1.3,1.3)) for a in [j*math.tau/150 for j in range(151)]]
 d.line(pts,fill=chalk,width=4,joint='curve')
# Sparse broken chalk grain, no decorative motifs.
p=im.load()
for _ in range(2600):
 x=r.randrange(1024);y=r.randrange(512)
 if p[x,y]==chalk:p[x,y]=(103,117,97)
out=Path('UnityProject/Assets/Art/Warehouse');out.mkdir(exist_ok=True)
im.save(out/'ChalkBlanks.png')

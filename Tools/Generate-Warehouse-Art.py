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
im=Image.new('RGB',(1536,768),(0,0,0))
for n in range(4):
 picture=Image.open(out/'Textures'/f'Chalk_{n+1}.png').convert('RGB')
 picture=picture.resize((360,510),Image.Resampling.LANCZOS)
 im.paste(picture,(12+n*384,160))
im.save(out/'ChalkScenes.png')
# Reviewed manuscript ingredient study; numbering is deterministic authored ink, not generated text.
from PIL import ImageFont
recipe=Image.open(out/'Textures'/'RecipeStudy.png').convert('RGB').resize((1024,640),Image.Resampling.LANCZOS)
rd=ImageDraw.Draw(recipe)
font=ImageFont.truetype('UnityProject/Assets/Fonts/GowunBatang-Regular.ttf',42)
for x,caption in [(171,'I'),(512,'II'),(853,'III')]:
 rd.text((x,40),caption,font=font,fill=(65,58,44),anchor='mm')
recipe.save(out/'PictureRecipe.png')
# Warehouse-only name masks keep historical Classroom textures byte-identical.
from PIL import ImageFont
font=ImageFont.truetype('UnityProject/Assets/Fonts/GowunBatang-Regular.ttf',112)
for rune,name in [('Earth','대지'),('Water','물'),('Fire','불'),('Wind','바람'),('Light','빛'),('Shadow','달')]:
 im=Image.new('RGB',(384,128),(2,2,2));d=ImageDraw.Draw(im)
 d.text((192,64),name,font=font,fill='white',anchor='mm',stroke_width=2)
 im.save(out/('RuneName_'+rune+'.png'))

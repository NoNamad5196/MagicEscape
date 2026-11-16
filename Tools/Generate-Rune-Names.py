"""Gowun Batang brass-incision masks, same shader/feedback as the glyphs."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
out=Path('UnityProject/Assets/Art/S10e')
font=ImageFont.truetype('UnityProject/Assets/Fonts/GowunBatang-Regular.ttf',112)
for rune,name in [('Earth','\ub300\uc9c0'),('Water','\ubb3c'),('Fire','\ubd88'),('Wind','\ubc14\ub78c'),('Light','\ube5b'),('Shadow','\uadf8\ub9bc\uc790')]:
    im=Image.new('RGB',(384,128),(2,2,2)); d=ImageDraw.Draw(im)
    d.text((192,64),name,font=font,fill=(255,255,255),anchor='mm',stroke_width=2)
    im.save(out/('RuneName_'+rune+'.png'))

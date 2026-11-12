from pathlib import Path
import pymupdf
out=Path(__file__).resolve().parent
with pymupdf.open(out/'MagicEscape_제안서.pdf') as doc:
 assert len(doc)==12
 (out/'preview').mkdir(exist_ok=True)
 for i,page in enumerate(doc,1):
  page.get_pixmap(matrix=pymupdf.Matrix(1.5,1.5)).save(out/'preview'/f'slide-{i:02}.png')
  print(f'slide {i:02}: rendered 1440x810')

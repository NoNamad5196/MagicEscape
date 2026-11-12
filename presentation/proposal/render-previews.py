import pathlib,pymupdf as f
p=pathlib.Path('presentation/proposal'); d=f.open(p/'MagicEscape_제안서.pdf'); (p/'preview').mkdir(exist_ok=True)
for i,page in enumerate(d):
 page.get_pixmap(matrix=f.Matrix(1.5,1.5)).save(p/'preview'/f'slide-{i+1:02}.png')
 print(i+1,len(page.get_text()),[(x[4].strip()[:20],round(x[3],1)) for x in page.get_text('blocks') if x[3]>498])
print('pages',len(d))

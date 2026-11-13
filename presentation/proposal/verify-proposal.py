from pathlib import Path
import io,json,re,zipfile,hashlib,subprocess,xml.etree.ElementTree as ET
import pymupdf
from PIL import Image,ImageStat
out=Path(__file__).resolve().parent
count=14
pptx=out/'MagicEscape_제안서.pptx'
pdf=out/'MagicEscape_제안서.pdf'
assert pptx.stat().st_size<=15_000_000,('PPTX exceeds 15 MB',pptx.stat().st_size)
agents=(out/'../../AGENTS.md').resolve().read_text(encoding='utf-8')
baseline=subprocess.check_output(['git','show','02cc7f0:AGENTS.md'],cwd=out).decode('utf-8')
assert agents.replace('\r\n','\n')==baseline.replace('\r\n','\n'),'Canonical AGENTS.md changed'
ns={'a':'http://schemas.openxmlformats.org/drawingml/2006/main','p':'http://schemas.openxmlformats.org/presentationml/2006/main'}
manifest=json.loads((out/'art/prompts.json').read_text(encoding='utf-8-sig'))
assert manifest['revision']==7
assert len(manifest['assets'])==14
assets={a['file']:a for a in manifest['assets']}
expected_images=[1,1,0,0,1,1,4,0,4,0,0,0,2,0]
with zipfile.ZipFile(pptx) as z:
 names=[n for n in z.namelist() if re.fullmatch(r'ppt/slides/slide\d+\.xml',n)]
 assert len(names)==count
 slides=[ET.fromstring(z.read(f'ppt/slides/slide{i}.xml')) for i in range(1,count+1)]
 texts=['\n'.join(n.text or '' for n in s.findall('.//a:t',ns)) for s in slides]
 alltext='\n'.join(texts)
 banned=r'\(계획\)|교수|켜고 출구|교실|갇혔다|혁신적|몰입감 극대화|새로운 경험|차별화된|효과적으로|성공적으로|다양한|직관적인|진척|테스트\s*\d|라이선스|에셋|스크린샷|2026[-./]|[☉♄🜃🜄🜂🜁]|\?'
 assert not re.search(banned,alltext),re.findall(banned,alltext)
 assert not any('notesSlides/' in n for n in z.namelist())
 media=[n for n in z.namelist() if n.startswith('ppt/media/')]
 assert len(media)==14
 for name in media:
  filename=Path(name).name
  assert filename.endswith('.jpg') and filename in assets
  source=out/'art'/filename
  assert z.read(name)==source.read_bytes(),name
  with Image.open(io.BytesIO(z.read(name))) as im:
   assert im.format=='JPEG' and max(im.size)<=1600,(filename,im.size)
   assert im.size==(assets[filename]['width'],assets[filename]['height'])
 assert not list((out/'art').glob('*.png')),'Obsolete PNG originals remain'
 fonts=set()
 for i,slide in enumerate(slides):
  assert f'{i+1:02} / {count}' in texts[i]
  assert len(slide.findall('.//p:pic',ns))==expected_images[i]
  for element in slide.findall('.//p:sp',ns)+slide.findall('.//p:pic',ns):
   xf=element.find('./p:spPr/a:xfrm',ns)
   if xf is not None:
    off=xf.find('a:off',ns).attrib;ext=xf.find('a:ext',ns).attrib
    assert min(int(off['x']),int(off['y']))>=0
    assert int(off['x'])+int(ext['cx'])<=960*12700
    assert int(off['y'])+int(ext['cy'])<=540*12700
  for pic in slide.findall('.//p:pic',ns):
   embed=pic.find('.//a:blip',ns).attrib['{http://schemas.openxmlformats.org/officeDocument/2006/relationships}embed']
   rel=ET.fromstring(z.read(f'ppt/slides/_rels/slide{i+1}.xml.rels'))
   target=next(n.attrib['Target'] for n in rel if n.attrib['Id']==embed)
   assert 'ppt/'+target.replace('../','') in media
  for r in slide.findall('.//a:rPr',ns):
   font=r.find('a:latin',ns).attrib['typeface'];fonts.add(font)
   size=int(r.attrib['sz'])/100
   assert size<=12 or size>=20,(i+1,size)
   assert font==('Paperlogy 7 Bold' if r.attrib['b']=='1' else 'Pretendard')
 assert fonts=={'Paperlogy 7 Bold','Pretendard'}
 assert '1인 제작 · 김선우 (202321850)' in texts[0]
 assert not re.search(r'2026[-./]',texts[0])
 assert slides[2].find('.//a:tbl',ns) is not None
 for value in ['I Expect You','To Die','Schell Games (2016)','The Room VR:','A Dark Matter','Fireproof Games (2020)','처음부터 끝까지','글 힌트는 실패 뒤']:
  assert value in texts[2],value
 assert all(v in texts[3] for v in ['9×7m','10~15분','PC VR(OpenXR)','키보드·마우스','봉인등 3개를 모두 끄고'])
 assert all(v in texts[4] for v in ['쓰지 않는 실습 도구','칠판, 제단, 가마솥','쌓아 둔 창고'])
 assert all(v in texts[5] for v in ['처음부터 보이는 것','치워야 드러나는 것','9×7m'])
 assert all(v in texts[6] for v in ['움직일 물건을 알아보는 법','손잡이·밧줄','끌린 자국','장식용 물건은 고정','반드시 단서나 재료','아무 상자나 뒤지는 반복'])
 assert texts[6].count('치우기 전')==texts[6].count('치운 후')==2
 for pic in slides[6].findall('.//p:pic',ns):
  ext=pic.find('./p:spPr/a:xfrm/a:ext',ns).attrib
  assert int(ext['cx'])>=200*12700 and int(ext['cy'])>=140*12700
 # Exact timeline x-center equality in native slide coordinates.
 shapes=slides[7].findall('.//p:sp',ns)
 circles=[s for s in shapes if s.find('./p:spPr/a:prstGeom',ns).attrib.get('prst')=='ellipse']
 assert len(circles)==5
 timeline=[]
 for circle,label in zip(circles,['입장','P1','P2','P3','탈출']):
  def center(e):
   x=e.find('./p:spPr/a:xfrm',ns)
   return int(x.find('a:off',ns).attrib['x'])+int(x.find('a:ext',ns).attrib['cx'])/2
  matches=[s for s in shapes if '\n'.join(n.text or '' for n in s.findall('.//a:t',ns))==label]
  assert len(matches)==1 and center(circle)==center(matches[0])
  description=next(s for s in shapes if s.find('./p:txBody',ns) is not None and center(s)==center(circle) and s.find('./p:spPr/a:xfrm/a:off',ns).attrib['y']==str(310*12700))
  assert description.find('.//a:pPr',ns).attrib['algn']=='ctr'
  timeline.append(center(circle)/12700)
 assert all(v in texts[8] for v in ['시작','P1 후','P2 후','P3 후·탈출','검은 복도','촛불과 주변 빛은 줄이고','단서 조명은 남긴다'])
 assert 'Earth → Water → Fire → Light' in agents and '대지 → 물 → 불 → 빛' in texts[9]
 assert all(v in texts[9] for v in ['액자 하나에 원소 하나','액자 번호 I~IV','칠판에는 I~IV','빈칸 도식만 있다.'])
 assert 'ing_moonpetal → ing_salamander_scale → ing_crystal_dust' in agents
 assert texts[10].index('월광초 꽃잎')<texts[10].index('불도마뱀 비늘')<texts[10].index('수정 가루')
 assert all(v in texts[10] for v in ['창가 화분','동쪽 책장 유리병','교탁 서랍','독버섯','투입 재료 원위치 재생성'])
 assert 'Left=crystal_sapphire, Right=crystal_ruby' in agents
 assert '왼쪽: 푸른 수정' in texts[11] and '오른쪽: 붉은 수정' in texts[11]
 assert '수정은 소모하지 않음' in texts[11]
 assert '3회 실패 후' in texts[9] and all('2회 실패 후' in texts[i] for i in [10,11])
 for i in [9,10,11]: assert 'VR:' in texts[i] and 'PC:' in texts[i]
 assert all(v in texts[12] for v in ['컨트롤러','텔레포트','스냅 회전','WASD','클릭해 들기'])
 assert all(v in texts[13] for v in ['순수 C#','잠김','활성','해결','Rigidbody','XR Device Simulator','데스크톱 모드로 전 구간 플레이','헤드셋을 빌릴 수 있으면'])
 # All revisions of the four brightness stages originate from one fixed-camera input.
 for f in ['darkness-p1.jpg','darkness-p2.jpg','darkness-p3.jpg']:
  assert assets[f]['reference']=='darkness-start.jpg'
 luminance=[]
 for f in ['darkness-start.jpg','darkness-p1.jpg','darkness-p2.jpg','darkness-p3.jpg']:
  with Image.open(out/'art'/f) as im: luminance.append(ImageStat.Stat(im.convert('L')).mean[0])
 assert all(a>b for a,b in zip(luminance,luminance[1:])),luminance
 for name in ['crates','cloth']:
  assert assets[f'clearing-{name}-after.jpg']['reference']==f'clearing-{name}-before.jpg'
  assert (assets[f'clearing-{name}-after.jpg']['width'],assets[f'clearing-{name}-after.jpg']['height'])==(assets[f'clearing-{name}-before.jpg']['width'],assets[f'clearing-{name}-before.jpg']['height'])
 d=pymupdf.open(pdf);assert len(d)==count
 pdf_fonts=set()
 compact=lambda t:re.sub(r'\s+','',t)
 for i,page in enumerate(d):
  pt=page.get_text()
  assert '\ufffd' not in pt and not re.search(banned,pt)
  for t in slides[i].findall('.//a:t',ns):
   assert compact(t.text or '') in compact(pt),(i+1,t.text)
  for b in page.get_text('blocks'):
   assert b[0]>=0 and b[1]>=0 and b[2]<=960 and b[3]<=540,(i+1,b)
  spans=[s for b in page.get_text('dict')['blocks'] if 'lines' in b for l in b['lines'] for s in l['spans'] if s['text'].strip()]
  for j,a in enumerate(spans):
   for b in spans[j+1:]:
    r=pymupdf.Rect(a['bbox']) & pymupdf.Rect(b['bbox'])
    assert r.is_empty or r.width<1 or r.height<1,(i+1,a['text'],b['text'],tuple(r))
  pdf_fonts.update(f[3].split('+')[-1] for f in page.get_fonts())
  with Image.open(out/'preview'/f'slide-{i+1:02}.png') as im:
   assert im.size==(1440,810)
   assert im.convert('RGB').tobytes()==page.get_pixmap(matrix=pymupdf.Matrix(1440/page.rect.width,810/page.rect.height)).samples,(i+1,'stale preview')
 assert len(list((out/'preview').glob('slide-*.png')))==count
 assert all('Paperlogy' in f or 'Pretendard' in f for f in pdf_fonts),pdf_fonts
 report={'revision':7,'slides':count,'pdfPages':count,'pptxBytes':pptx.stat().st_size,'pdfBytes':pdf.stat().st_size,'pptxLimitBytes':15_000_000,'embeddedImages':len(media),'imageInstancesPerSlide':expected_images,'maximumImageEdgePx':1600,'jpegQuality':85,'imageSourceBytesMatched':True,'previewPixelsMatchedPDF':True,'nativeBenchmarkTable':True,'timelineCentersPt':timeline,'timelineTextCentersMatched':True,'darknessMeanLuminance':[round(v,2) for v in luminance],'minimumBodyFontPt':20,'forbiddenTextMatches':0,'fonts':sorted(fonts),'pdfEmbeddedFonts':sorted(pdf_fonts),'allTextWithinPage':True,'renderedTextSpanOverlap':False,'puzzleAnswersAndHintThresholdsMatched':True,'canonicalAGENTSUnchanged':True,'speakerNotes':0,'visualReview':'All final previews and generated originals opened directly. Night tone, lamps, VR controllers, before/after cameras, text and symbol absence inspected. See README.','scope':'Proposal only. VR dual rigs and movable props are design, not implementation claims.','gameTests':'Not run; no game code changed.','powerPointNativeRender':'Not checked; LibreOffice PDF render verified.'}
 (out/'validation.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
 print(json.dumps(report,ensure_ascii=True,indent=2))

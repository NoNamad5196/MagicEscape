from pathlib import Path
import json,re,zipfile,hashlib,xml.etree.ElementTree as ET
import pymupdf
from PIL import Image
out=Path(__file__).resolve().parent
agents=(out/'../../AGENTS.md').resolve().read_text(encoding='utf-8')
ns={'a':'http://schemas.openxmlformats.org/drawingml/2006/main','p':'http://schemas.openxmlformats.org/presentationml/2006/main'}
with zipfile.ZipFile(out/'MagicEscape_제안서.pptx') as z:
 names=[n for n in z.namelist() if re.fullmatch(r'ppt/slides/slide\d+\.xml',n)]
 assert len(names)==12
 slides=[ET.fromstring(z.read(f'ppt/slides/slide{i}.xml')) for i in range(1,13)]
 texts=['\n'.join(n.text or '' for n in s.findall('.//a:t',ns)) for s in slides]
 alltext='\n'.join(texts)
 banned=r'계획|교수|켜고 출구|교실|갇혔다|단서 강조|혁신적|몰입감 극대화|새로운 경험|새로운 가능성|차별화된|효과적으로|성공적으로|다양한|직관적인|합니다|살펴봅니다|소개합니다|개편 예정|씨앗|금박|원소 6종 공통 문양 체계|봉인 해제 5단계|진척|테스트 \d|라이선스|에셋|X=-|GameFlowState|MonoBehaviour|IInteractable|20\d{2}[-./]|[☉♄🜃🜄🜂🜁]|\?'
 assert not re.search(banned,alltext),re.findall(banned,alltext)
 assert not any('notesSlides/' in n for n in z.namelist())
 media=[n for n in z.namelist() if n.startswith('ppt/media/')]
 assert len(media)==13
 for name in media:
  source=out/'art'/Path(name).name
  assert source.is_file() and z.read(name)==source.read_bytes(),name
 expected_images=[1,3,1,1,1,4,0,0,0,0,2,0]
 assert all(len(s.findall('.//p:pic',ns))==expected_images[i] for i,s in enumerate(slides))
 fonts=set();sizes=[]
 for i,s in enumerate(slides):
  assert f'{i+1:02} / 12' in texts[i]
  for pic in s.findall('.//p:pic',ns):
   xf=pic.find('./p:spPr/a:xfrm',ns)
   off=xf.find('a:off',ns).attrib;ext=xf.find('a:ext',ns).attrib
   assert 0<=int(off['x']) and 0<=int(off['y'])
   assert int(off['x'])+int(ext['cx'])<=960*12700
   assert int(off['y'])+int(ext['cy'])<=540*12700
   embed=pic.find('.//a:blip',ns).attrib['{http://schemas.openxmlformats.org/officeDocument/2006/relationships}embed']
   rel=ET.fromstring(z.read(f'ppt/slides/_rels/slide{i+1}.xml.rels'))
   target=next(n.attrib['Target'] for n in rel if n.attrib['Id']==embed)
   assert 'ppt/'+target.replace('../','') in media
  for n in s.findall('.//p:sp',ns):
   xf=n.find('./p:spPr/a:xfrm',ns)
   if xf is not None:
    a=xf.find('a:off',ns).attrib;b=xf.find('a:ext',ns).attrib
    assert 0<=int(a['x']) and 0<=int(a['y'])
    assert int(a['x'])+int(b['cx'])<=960*12700
    assert int(a['y'])+int(b['cy'])<=540*12700
   for r in n.findall('.//a:rPr',ns):
    f=r.find('a:latin',ns).attrib['typeface'];fonts.add(f)
    size=int(r.attrib['sz'])/100;sizes.append(size)
    if size>12: assert size>=20
    assert f==('Paperlogy 7 Bold' if r.attrib['b']=='1' else 'Pretendard')
 assert fonts=={'Paperlogy 7 Bold','Pretendard'}
 assert '1인 제작 · 김선우 (202321850)' in texts[0] and '마법학교 창고 콘셉트의\nVR 방탈출' in texts[0]
 assert not re.search(r'2026[-./]',texts[0])
 assert all(t in texts[2] for t in ['9×7m','10~15분','PC VR(OpenXR)','키보드·마우스','봉인등 3개를 모두 끄고'])
 assert 'Earth → Water → Fire → Light' in agents and '대지 → 물 → 불 → 빛' in texts[7]
 assert 'ing_moonpetal → ing_salamander_scale → ing_crystal_dust' in agents
 assert texts[8].index('월광초 꽃잎')<texts[8].index('불도마뱀 비늘')<texts[8].index('수정 가루')
 assert 'Left=crystal_sapphire, Right=crystal_ruby' in agents
 assert '왼쪽: 푸른 수정' in texts[9] and '오른쪽: 붉은 수정' in texts[9]
 assert '3회 실패 후' in texts[7] and all('2회 실패 후' in texts[i] for i in [8,9])
 assert '투입 재료 원위치 재생성' in texts[8] and '수정은 소모하지 않음' in texts[9]
 for i in [7,8,9]: assert 'VR:' in texts[i] and 'PC:' in texts[i] and '행동 · 조작' in texts[i]
 assert all(v in texts[10] for v in ['헤드셋 연결 시 VR','없으면 데스크톱','Grab / Socket / Poke','XR Device Simulator','텔레포트','스냅 회전','화면 앞에 들기'])
 assert all(v in texts[11] for v in ['순수 C#','잠김','활성','해결'])
 assert all(v in texts[4] for v in ['처음부터 보이는 것','치워야 드러나는 것','9×7m','선반·상자·천·궤짝'])
 assert all(v in texts[5] for v in ['치우기 전','치운 후','가벼운 물건','무거운 물건','고정 장치','VR 조작','키보드·마우스 조작','클릭한 채로 끈다.','잡고 밀기·끌기만 한다.'])
 assert texts[5].count('치우기 전')==2 and texts[5].count('치운 후')==2
 assert all('숨긴 위치:' in texts[i] for i in [7,8,9])
 assert '누군가' in texts[3]
 assert 'Rigidbody 무게 구분' in texts[11] and '상호작용' in texts[11]
 assert sum(n.find('a:bodyPr',ns).attrib.get('anchor')=='ctr' for n in slides[11].findall('.//p:txBody',ns))==6
 d=pymupdf.open(out/'MagicEscape_제안서.pdf');assert len(d)==12
 pdf_fonts=set()
 for i,p in enumerate(d):
  assert '\ufffd' not in p.get_text() and '?' not in p.get_text()
  assert not re.search(banned,p.get_text())
  compact=lambda t: re.sub(r'\s+','',t)
  for t in slides[i].findall('.//a:t',ns):
   assert compact(t.text or '') in compact(p.get_text()),(i+1,t.text)
  # All rendered text, including footers, must be on the page.
  for b in p.get_text('blocks'):
   assert b[0]>=0 and b[1]>=0 and b[2]<=960 and b[3]<=540,(i+1,b)
  # No unrelated text spans may overlap (multi-line blocks are not rectangles of ink).
  spans=[s for b in p.get_text('dict')['blocks'] if 'lines' in b for l in b['lines'] for s in l['spans'] if s['text'].strip()]
  for j,a in enumerate(spans):
   for b in spans[j+1:]:
    r=pymupdf.Rect(a['bbox']) & pymupdf.Rect(b['bbox'])
    assert r.is_empty or r.width<1 or r.height<1,(i+1,a['text'],b['text'])
  pdf_fonts.update(f[3].split('+')[-1] for f in p.get_fonts())
 assert all('Paperlogy' in f or 'Pretendard' in f for f in pdf_fonts),pdf_fonts
 assert len(list((out/'preview').glob('slide-*.png')))==12
 for i,p in enumerate(d,1):
  with Image.open(out/'preview'/f'slide-{i:02}.png') as im:
   assert im.size==(1440,810)
   assert im.convert('RGB').tobytes()==p.get_pixmap(matrix=pymupdf.Matrix(1440/p.rect.width,810/p.rect.height)).samples,(i,'stale preview')
 spans6=[s for b in d[5].get_text('dict')['blocks'] if 'lines' in b for l in b['lines'] for s in l['spans']]
 table_bottom=max(s['bbox'][3] for s in spans6 if '열기' in s['text'])
 note_top=min(s['bbox'][1] for s in spans6 if '단서를 찾을 때도' in s['text'])
 assert note_top-table_bottom>=14,(note_top,table_bottom)
 generation=json.loads((out/'art/prompts.json').read_text(encoding='utf-8-sig'))
 assert len(generation['assets'])==12 and all(a['calls']==1 and a['rerolls']==0 for a in generation['assets'])
 report={'slides':12,'pdfPages':12,'embeddedImages':len(media),'imageInstancesPerSlide':expected_images,'imageSourceBytesMatched':True,'previewPixelsMatchedPDF':True,'clearingTableNoteGapPt':round(note_top-table_bottom,2),'planTagMatches':0,'speakerNotes':0,'fonts':sorted(fonts),'pdfEmbeddedFonts':sorted(pdf_fonts),'minimumBodyFontPt':20,'forbiddenTextMatches':0,'puzzleAnswersAndHintThresholdsMatched':True,'allTextWithinPage':True,'renderedTextSpanOverlap':False,'vrAndDesktop':'Proposed, not implementation claims','designChangesVsCanonicalDocs':['VR/OpenXR, mouse-click rigs, 9x7m storeroom and concealed props are proposal plans only.'],'visualReview':'All twelve final PNGs opened individually; README records the review.','gameTests':'Not run; presentation-only change.'}
 (out/'validation.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
 print(json.dumps(report,ensure_ascii=True,indent=2))

from pathlib import Path
import json,re,zipfile,xml.etree.ElementTree as ET
import pymupdf
root=Path(__file__).resolve().parents[2];out=Path(__file__).resolve().parent
ns={'a':'http://schemas.openxmlformats.org/drawingml/2006/main','p':'http://schemas.openxmlformats.org/presentationml/2006/main'}
agents=(root/'AGENTS.md').read_text(encoding='utf-8')
with zipfile.ZipFile(out/'MagicEscape_제안서.pptx') as z:
 slide_names=[n for n in z.namelist() if re.fullmatch(r'ppt/slides/slide\d+\.xml',n)]
 assert len(slide_names)==11
 slides=[ET.fromstring(z.read(f'ppt/slides/slide{i}.xml')) for i in range(1,12)]
 texts=['\n'.join(n.text or '' for n in s.findall('.//a:t',ns)) for s in slides];alltext='\n'.join(texts)
 assert not any('notesSlides/' in n or n.startswith('ppt/media/') for n in z.namelist())
 assert not any(s.findall('.//p:pic',ns) for s in slides)
 banned=r'KayKit|Quaternius|Poly Haven|Kenney|CC0|OFL|Gowun|고운바탕|Nanum|나눔손글씨|Noto|CREDITS|현재:|완료|44|19개|이전:|이후:|적용|구현됨|자체 제작|Luke\.RUSTLTD|DaGameKnower|Rudmer_Rotteveel|20\d{2}년|\d{1,2}월\s*\d{1,2}일|[☉♄🜃🜄🜂🜁]|녹청|아주라이트|버밀리언|오커|금박|납회색|개편 예정|S10|S\d+~|(?<![\d.])(?:0?[1-9]|1[0-2])/(?:0?[1-9]|[12]\d|3[01])(?![\d.])|\b20\d{2}[-./]\d|SCENE_SETUP|제안서 평가|그림 A|그림 B|\?'
 assert not re.search(banned,alltext,re.I),re.findall(banned,alltext,re.I)
 for i,t in enumerate(texts):
  if i!=10:assert not re.search(r'\b(?:ing|crystal|potion)_\w+',t)
  assert f'{i+1:02} / 11' in t
  assert 'MagicEscape   /   멀티미디어와 가상현실 기말 과제 제안서' in t
 assert not any(0x1F700<=ord(c)<=0x1F77F for c in alltext)
 assert not any(n.attrib.get('prst')=='triangle' for s in slides for n in s.findall('.//a:prstGeom',ns))
 fonts={n.attrib['typeface'] for s in slides for n in s.findall('.//a:latin',ns)}
 assert fonts=={'Paperlogy 7 Bold','Pretendard'},fonts
 for s in slides:
  for n in s.findall('.//p:sp',ns):
   xf=n.find('./p:spPr/a:xfrm',ns)
   if xf is None:continue
   off=xf.find('a:off',ns);ext=xf.find('a:ext',ns)
   assert int(off.attrib['x'])>=0 and int(off.attrib['y'])>=0
   assert int(off.attrib['x'])+int(ext.attrib['cx'])<=960*12700
   assert int(off.attrib['y'])+int(ext.attrib['cy'])<=540*12700
 assert '대지 → 물 → 불 → 빛' in texts[6] and 'Earth → Water → Fire → Light' in agents
 assert [n for n in ['불','III','대지','I','빛','IV','물','II']]==[n.text for n in slides[6].findall('.//a:t',ns)][3:11]
 assert texts[7].index('월광초 꽃잎')<texts[7].index('불도마뱀 비늘')<texts[7].index('수정 가루')
 assert 'ing_moonpetal → ing_salamander_scale → ing_crystal_dust' in agents
 assert '왼쪽 소켓: 푸른 수정 (물)' in texts[8] and '오른쪽 소켓: 붉은 수정 (불)' in texts[8]
 assert 'Left=crystal_sapphire, Right=crystal_ruby' in agents
 assert all(v in texts[2] for v in ['글로 설명하는 방식','이 작품의 방식','P1  액자 그림·칠판 도식','P2  문장 레시피','P2  번호 그림 레시피','청백·주황·금색'])
 for v in ['12×10m','4.5m','10~15분']:assert v in texts[1]
 for v in ['2.5m','8칸','퍼즐 규칙을 엔진과 분리해 자동 테스트로 검증한다']:assert v in texts[10]
 assert all(v in agents for v in ['failCount ≥ 3','failCount ≥ 2','상호작용 거리 2.5m','인벤토리 최대 8칸','이번 시도에서 투입된 모든 재료를 **원래 위치에 재생성**','틀리면 거부(소모 안 함)','ExitTrigger 진입 → Cleared'])
 assert '투입 재료 전부 원위치 재생성' in texts[7]
 assert '오답은 거부·보존' in texts[8]
 assert '출구 통과 시 클리어' in texts[5]
 assert '실패 3회' in texts[6] and '2회 실패' in texts[7] and '2회 실패' in texts[8]
 assert '검정 페이드' in texts[9] and '12초' in texts[9] and '촛불 6개' in texts[9]
 assert '이름 / 학번' in texts[0]
 arrow=next(n for n in slides[6].findall('.//p:sp',ns) if 'I  →  II  →  III  →  IV' in ''.join(t.text or '' for t in n.findall('.//a:t',ns)))
 desc=next(n for n in slides[6].findall('.//p:sp',ns) if '칠판은 채워야 할' in ''.join(t.text or '' for t in n.findall('.//a:t',ns)))
 ay=int(arrow.find('./p:spPr/a:xfrm/a:off',ns).attrib['y']);ah=int(arrow.find('./p:spPr/a:xfrm/a:ext',ns).attrib['cy'])
 dy=int(desc.find('./p:spPr/a:xfrm/a:off',ns).attrib['y']);assert dy-ay-ah>=11*12700
 d=pymupdf.open(out/'MagicEscape_제안서.pdf');assert len(d)==11
 pdf_fonts=set()
 for i,p in enumerate(d):
  assert len(p.get_text())>100 and '\ufffd' not in p.get_text()
  body=[b for b in p.get_text('blocks') if not b[4].startswith('MagicEscape   /')]
  assert all(b[0]>=0 and b[2]<=960 and b[1]>=0 and b[3]<490 for b in body),(i+1,body)
  pdf_fonts.update(f[3].split('+')[-1] for f in p.get_fonts())
 assert all('Paperlogy' in f or 'Pretendard' in f for f in pdf_fonts),pdf_fonts
 report={'slides':11,'pdfPages':11,'embeddedImages':0,'speakerNotes':0,'nativeShapes':sum(len(s.findall('.//p:sp',ns)) for s in slides),'forbiddenTextMatches':0,'itemCodesOutsideTechnicalSlide':0,'pageNumbersAndFooterMatched':True,'alchemyTriangleShapes':0,'fonts':sorted(fonts),'pdfEmbeddedFonts':sorted(pdf_fonts),'fontSubstitution':False,'puzzleAnswersAndHintThresholdsMatched':True,'roomDimensionsAndDistanceAndInventoryMatched':True,'pdfTextBounds':'All body blocks above y=490; footer at y=497','visualReview':'Manual review recorded in README; not evaluated by this script','gameTests':'Not rerun: presentation-only changes','authoring':'Existing JavaScript native OOXML generator retained; artifact-tool and dependency loader unavailable. LibreOffice PDF export and PyMuPDF PNG rendering.'}
 assert len(list((out/'preview').glob('slide-*.png')))==11
 assert not (out/'preview'/'slide-12.png').exists()
 (out/'validation.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
 print(json.dumps(report,ensure_ascii=True,indent=2))

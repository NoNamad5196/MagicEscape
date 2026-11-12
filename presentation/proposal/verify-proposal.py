from pathlib import Path
import json,re,zipfile,xml.etree.ElementTree as ET
import pymupdf
root=Path(__file__).resolve().parents[2];out=Path(__file__).resolve().parent
ns={'a':'http://schemas.openxmlformats.org/drawingml/2006/main','p':'http://schemas.openxmlformats.org/presentationml/2006/main'}
agents=(root/'AGENTS.md').read_text(encoding='utf-8')
with zipfile.ZipFile(out/'MagicEscape_제안서.pptx') as z:
 slide_names=[n for n in z.namelist() if re.fullmatch(r'ppt/slides/slide\d+\.xml',n)]
 assert len(slide_names)==12
 slides=[ET.fromstring(z.read(f'ppt/slides/slide{i}.xml')) for i in range(1,13)]
 texts=['\n'.join(n.text or '' for n in s.findall('.//a:t',ns)) for s in slides];alltext='\n'.join(texts)
 assert not any('notesSlides/' in n or n.startswith('ppt/media/') for n in z.namelist())
 assert not any(s.findall('.//p:pic',ns) for s in slides)
 banned=r'[☉♄🜃🜄🜂🜁]|녹청|아주라이트|버밀리언|오커|금박|납회색|개편 예정|S10|S\d+~|(?<![\d.])(?:0?[1-9]|1[0-2])/(?:0?[1-9]|[12]\d|3[01])(?![\d.])|\b20\d{2}[-.]\d|SCENE_SETUP|ing_\w+|crystal_\w+|potion_\w+|제안서 평가|그림 A|그림 B|\?'
 assert not re.search(banned,alltext),re.findall(banned,alltext)
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
 assert [n for n in ['불','III','대지','I','빛','IV','물','II']]==[n.text for n in slides[6].findall('.//a:t',ns)][4:12]
 assert texts[7].index('월광초 꽃잎')<texts[7].index('불도마뱀 비늘')<texts[7].index('수정 가루')
 assert 'ing_moonpetal → ing_salamander_scale → ing_crystal_dust' in agents
 assert '왼쪽 소켓: 푸른 수정 (물)' in texts[8] and '오른쪽 소켓: 붉은 수정 (불)' in texts[8]
 assert 'Left=crystal_sapphire, Right=crystal_ruby' in agents
 game=(root/'CodeDrafts/Core/GameText.cs').read_text(encoding='utf-8')
 quote='생명은 정해진 순서로 깨어난다. 액자의 숫자를 따르라.'
 assert quote in game and quote in agents and quote in texts[2].replace('\n',' ').replace('“','').replace('”','')
 for v in ['12×10m','4.5m','10~15분']:assert v in texts[1]
 for v in ['2.5m','8칸','44개','19개']:assert v in texts[10]
 assert '실패 3회' in texts[6] and '2회 실패' in texts[7] and '2회 실패' in texts[8]
 assert '검정 페이드' in texts[9] and '12초' in texts[9] and '촛불 6개' in texts[9]
 assert '이름 / 학번' in texts[0]
 assert '퍼즐 그림·문양 개편' in texts[11] and '처음 하는 사람 테스트와 수정' in texts[11]
 count=sum(len(re.findall(r'\[(?:UnityTest|Test)\]',p.read_text(encoding='utf-8-sig'))) for p in (root/'UnityProject/Assets/Tests/PlayMode').glob('*.cs'));assert count==19
 d=pymupdf.open(out/'MagicEscape_제안서.pdf');assert len(d)==12
 pdf_fonts=set()
 for i,p in enumerate(d):
  assert len(p.get_text())>100 and '\ufffd' not in p.get_text()
  body=[b for b in p.get_text('blocks') if not b[4].startswith('MagicEscape   /')]
  assert all(b[0]>=0 and b[2]<=960 and b[1]>=0 and b[3]<490 for b in body),(i+1,body)
  pdf_fonts.update(f[3].split('+')[-1] for f in p.get_fonts())
 assert all('Paperlogy' in f or 'Pretendard' in f for f in pdf_fonts),pdf_fonts
 report={'slides':12,'pdfPages':12,'embeddedImages':0,'speakerNotes':0,'nativeShapes':sum(len(s.findall('.//p:sp',ns)) for s in slides),'forbiddenTextMatches':0,'alchemyTriangleShapes':0,'fonts':sorted(fonts),'pdfEmbeddedFonts':sorted(pdf_fonts),'fontSubstitution':False,'puzzleAnswersAndHintThresholdsMatched':True,'sourceChalkboardQuoteMatched':True,'roomDimensionsAndDistanceAndInventoryMatched':True,'playModeSourceTestCount':count,'playModeRun':'Not rerun; existing project validation record only','coreRun':'dotnet test Tests/MagicEscape.Core.Tests --no-restore: 44 passed, 0 failed, 0 skipped','pdfTextBounds':'All body blocks above y=490; footer at y=497','visualReview':'12 final slide PNGs opened individually; no overflow, unintended overlap or excessive whitespace. Cover intentionally minimal.','authoring':'Existing standalone JavaScript native OOXML generator retained: artifact-tool and dependency loader unavailable. LibreOffice PDF export and PyMuPDF PNG rendering.'}
 (out/'validation.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
 print(json.dumps(report,ensure_ascii=True,indent=2))

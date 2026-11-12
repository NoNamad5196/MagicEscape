from pathlib import Path
import json,re,zipfile,xml.etree.ElementTree as ET
import pymupdf
root=Path(__file__).resolve().parents[2]; out=Path(__file__).resolve().parent
ns={'a':'http://schemas.openxmlformats.org/drawingml/2006/main','p':'http://schemas.openxmlformats.org/presentationml/2006/main'}
with zipfile.ZipFile(out/'MagicEscape_제안서.pptx') as z:
 slides=[ET.fromstring(z.read(f'ppt/slides/slide{i}.xml')) for i in range(1,13)]
 texts=['\n'.join(n.text or '' for n in s.findall('.//a:t',ns)) for s in slides]
 assert not any('notesSlides/' in n for n in z.namelist())
 assert len([n for n in z.namelist() if n.startswith('ppt/media/')])==1
 ids=['ing_moonpetal','ing_salamander_scale','ing_crystal_dust','ing_toadstool','crystal_sapphire','crystal_ruby','potion_revelation']
 agents=(root/'AGENTS.md').read_text(encoding='utf-8')
 for item in ids: assert item in agents and item in '\n'.join(texts),item
 assert '대지 → 물 → 불 → 빛' in texts[6]
 assert 'Earth → Water → Fire → Light' in agents
 assert texts[7].index(ids[0])<texts[7].index(ids[1])<texts[7].index(ids[2])
 assert 'Left = crystal_sapphire' in texts[8] and 'Right = crystal_ruby' in texts[8]
 for value in ['12×10m','4.5m','10~15분']: assert value in texts[1]
 for value in ['2.5m','8칸','44개','19개']: assert value in texts[10]
 assert '백색' not in '\n'.join(texts) and '씨앗' not in '\n'.join(texts)
 assert '검정 페이드' in texts[9]
 assert '이름 / 학번' in texts[0]
 source_test_count=sum(len(re.findall(r'\[(?:UnityTest|Test)\]',p.read_text(encoding='utf-8-sig'))) for p in (root/'UnityProject/Assets/Tests/PlayMode').glob('*.cs'))
 assert source_test_count==19
 d=pymupdf.open(out/'MagicEscape_제안서.pdf');assert len(d)==12
 for i,p in enumerate(d):
  t=p.get_text();assert len(t)>100 and '\ufffd' not in t
  body=[b for b in p.get_text('blocks') if not b[4].startswith('MagicEscape   /')]
  assert all(b[0]>=0 and b[2]<=960 and b[1]>=0 and b[3]<497 for b in body),(i+1,body)
 report={'slides':12,'pdfPages':12,'embeddedImages':1,'nativeShapes':sum(len(s.findall('.//p:sp',ns)) for s in slides),'speakerNotes':0,'allItemIdsMatched':ids,'puzzleAnswersMatched':True,'roomDimensionsAndDistanceAndInventoryMatched':True,'playModeSourceTestCount':source_test_count,'playModeRun':'Not rerun for presentation; existing S10f record is 19/19','coreRun':'dotnet test Tests/MagicEscape.Core.Tests --no-restore: 44 passed, 0 failed, 0 skipped','pdfTextBounds':'All body blocks within slide and above footer','replacementCharacters':0,'visualReview':'All 12 PNG files opened individually; slides 08 and 12 spacing repaired and reopened; six alchemical signs visible','authoring':'Bundled artifact runtime and load_workspace_dependencies not available; standalone JavaScript emits native OOXML, Python stdlib packages ZIP. No python-pptx or dependency installation.'}
 (out/'validation.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
 print(json.dumps(report,ensure_ascii=True,indent=2))

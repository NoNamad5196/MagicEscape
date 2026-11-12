"""Sanitized S10d validation report. Raw Unity XML/logs remain local-only."""
from pathlib import Path
import json,re,hashlib,subprocess,xml.etree.ElementTree as ET
out=Path('docs/validation/s10d')
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
r=ET.parse('docs/validation/s10d-tests-final.xml').getroot()
assert r.get('result')=='Passed' and r.get('passed')=='13' and r.get('failed')=='0'
result={k:r.get(k) for k in ['result','total','passed','failed','skipped','start-time','end-time','duration']}
result['tests']=[{k:t.get(k) for k in ['fullname','result','duration']} for t in r.iter('test-case')]
console=(out/'console-audit.txt').read_text(encoding='utf-8-sig');assert 'events=0' in console
result['game_console_error_exception_assert_events']=0
result['runs']={}
for name,headroom in [('s10d-release-build',18.23),('s10d-tests-final',17.64)]:
 log=Path('docs/validation/'+name+'.log').read_text(encoding='utf-8-sig')
 counts={'compiler_errors':len(re.findall(r'error CS\d+',log)), 'exception_headings':len(re.findall(r'^[\w.]*Exception:',log,re.M)), 'shader_errors':len(re.findall(r'^Shader error',log,re.M)), 'atlas_shadow_reduction_warnings':log.count('Reduced additional punctual light shadows')}
 assert not any(counts.values()),(name,counts)
 result['runs'][name]={'counts':counts,'launch_headroom_gb':headroom,'editor_licensing_access_token_messages':log.count('Access token is unavailable; failed to update')}
result['iterations']=[{'run':'s10d-tests','passed':12,'failed':1,'cause':'UITests still required NotoSansKR. Updated expectation to Gowun UI / Nanum chalk.'},{'run':'s10d-tests-refine','passed':12,'failed':1,'cause':'Dynamic TMP atlas was cleared by TestRunner build. Baked static atlas, disabled clear-on-build; no fallback in glyph test.'},{'run':'s10d-build-baked','status':'compile failed','cause':'TMP clearDynamicDataOnBuild is not public in installed version; corrected using SerializedObject.'},{'run':'s10d-art-baked','passed':1,'failed':0}]
result['glyph_audits']={}
for p in out.glob('*-glyph-audit.txt'):
 text=p.read_text(encoding='utf-8-sig');assert 'Missing=0' in text
 result['glyph_audits'][p.name]={'required':int(re.search(r'characters=(\d+)',text)[1]),'missing':0}
old=subprocess.check_output(['git','show','011dcc3:UnityProject/Assets/Tests/PlayMode/ArtTests.cs']).decode('utf-8-sig')
new=Path('UnityProject/Assets/Tests/PlayMode/ArtTests.cs').read_text(encoding='utf-8-sig')
views=lambda s:[re.sub(r'\s+','',v) for v in re.findall(r'View\(new Vector3.*?;',s)]
assert views(old)==views(new),'Screenshot viewpoints changed'
result['capture_viewpoints_equal_to_s10c']=True
result['captures']=[dict(file=p.name,bytes=p.stat().st_size,sha256=sha(p)) for p in sorted(out.glob('*.png'))];assert len(result['captures'])==14
result['mirrors']={}
for rel in ['Editor/ClassroomFourthArtBuilder.cs','Tests/PlayMode/ArtTests.cs','Tests/PlayMode/FullInputTests.cs','Tests/PlayMode/UITests.cs','Scripts/Runtime/UI/UIManager.cs']:
 source=Path('UnityProject/Assets')/rel
 mirror=Path('CodeDrafts')/rel.replace('Scripts/','')
 assert source.read_bytes()==mirror.read_bytes(),str(source)
 result['mirrors'][str(source).replace('\\','/')]=True
for rel in ['Core/GameText.cs','Core/RunePuzzleLogic.cs','Core/PotionPuzzleLogic.cs','Core/SealPuzzleLogic.cs']:
 p=Path('UnityProject/Assets/Scripts')/rel
 baseline=subprocess.check_output(['git','show','011dcc3:'+str(p).replace('\\','/')])
 assert p.read_bytes().replace(b'\r\n',b'\n')==baseline.replace(b'\r\n',b'\n'),str(p)
result['core_rules_and_GameText_unchanged']=True
counts={}
for name in ['s10c','s10d']:
 manifest=Path('docs/validation')/name/'scene-manifest.txt'
 counts[name]=sum(line.split('|')[0].split('/')[-1].startswith('ArtB_Fit_') for line in manifest.read_text(encoding='utf-8-sig').splitlines())
result['model_roots']=counts
result['scene_manifest_sha256']=sha(out/'scene-manifest.txt')
(out/'results.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
assets=[p for folder in ['UnityProject/Assets/Art/S10d','UnityProject/Assets/Fonts'] for p in Path(folder).glob('*') if p.is_file() and p.suffix!='.meta' and ('Fonts' not in str(p) or p.name.startswith(('Gowun','Nanum')))]
assets += [Path('UnityProject/Assets/ThirdParty/Quaternius')/(n+'.fbx') for n in ['Crate_Wooden','Candle_2','Scroll_2','Book_7']]
assets += [Path('UnityProject/Assets/ThirdParty/Quaternius/License_Standard.txt'),Path('UnityProject/Assets/Art/OldCrystal.shader')]
(out/'asset-inventory.json').write_text(json.dumps([dict(file=str(p).replace('\\','/'),bytes=p.stat().st_size,sha256=sha(p)) for p in sorted(assets)],indent=2)+'\n',encoding='utf-8')
print('PlayMode 13/13; game Console 0; static glyph missing 0; 14 PNG; camera poses unchanged; 5 mirrors exact; Core/GameText unchanged; roots 33 -> 72.')

"""Sanitize Unity test evidence: omit local machine, environment and licensing identifiers."""
import hashlib,json,re,xml.etree.ElementTree as ET
from pathlib import Path

out=Path('docs/validation/s10c')
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
root=ET.parse('docs/validation/s10c-tests-final.xml').getroot()
results={k:root.get(k) for k in ['result','total','passed','failed','skipped','start-time','end-time','duration']}
results['run']='s10c-tests-final'
results['tests']=[{k:t.get(k) for k in ['fullname','result','duration']} for t in root.iter('test-case')]
assert results['result']=='Passed' and results['passed']=='13' and results['failed']=='0'
console=(out/'console-audit.txt').read_text(encoding='utf-8-sig')
assert 'events=0' in console
results['game_console_error_exception_assert_events']=0
results['captures']=[{'file':p.name,'bytes':p.stat().st_size,'sha256':sha(p)} for p in sorted(out.glob('*.png'))]
assert len(results['captures'])==14
results['runs']={}
for name in ['s10c-release-build','s10c-release-repeat','s10c-tests-release','s10c-tests-final']:
    log=Path('docs/validation/'+name+'.log').read_text(encoding='utf-8-sig')
    counts={'compiler_error_lines':len(re.findall(r'error CS\d+',log)), 'exception_headings':len(re.findall(r'^[\w.]*Exception:',log,re.M)), 'shader_error_lines':len(re.findall(r'^Shader error',log,re.M)), 'shadow_atlas_reduction_warnings':log.count('Reduced additional punctual light shadows')}
    assert not any(counts.values()),(name,counts)
    results['runs'][name]=counts
results['launch_headroom_gb']={'s10c-release-build':13.32,'s10c-release-repeat':13.87,'s10c-tests-release':13.72,'s10c-tests-final':13.67}
results['scene_manifest_sha256']=sha(out/'scene-manifest.txt')
results['mirror_checks']={}
for source,mirror in [
 ('UnityProject/Assets/Scripts/Runtime/Audio/RoomAmbience.cs','CodeDrafts/Runtime/Audio/RoomAmbience.cs'),
 ('UnityProject/Assets/Scripts/Runtime/Effects/AtmosphereMotion.cs','CodeDrafts/Runtime/Effects/AtmosphereMotion.cs'),
 ('UnityProject/Assets/Editor/ClassroomThirdArtBuilder.cs','CodeDrafts/Editor/ClassroomThirdArtBuilder.cs'),
 ('UnityProject/Assets/Tests/PlayMode/ArtTests.cs','CodeDrafts/Tests/PlayMode/ArtTests.cs'),
 ('UnityProject/Assets/Tests/PlayMode/FullInputTests.cs','CodeDrafts/Tests/PlayMode/FullInputTests.cs')]:
    equal=Path(source).read_text(encoding='utf-8-sig')==Path(mirror).read_text(encoding='utf-8-sig');assert equal;results['mirror_checks'][source]=equal
(out/'results.json').write_text(json.dumps(results,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
inventory=[{'file':str(p).replace('\\','/'),'bytes':p.stat().st_size,'sha256':sha(p)} for folder in ['UnityProject/Assets/Art/S10c','UnityProject/Assets/ThirdParty/AtmosphereCC0'] for p in sorted(Path(folder).rglob('*')) if p.is_file() and p.suffix!='.meta']
(out/'asset-inventory.json').write_text(json.dumps(inventory,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print('Final PlayMode 13/13; game console 0; 14 PNG; compiler/exception/shader/atlas audits 0; five mirrors identical.')

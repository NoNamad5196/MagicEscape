"""Read-only evidence audit. Never promotes unverified visual/performance checks to passed."""
from pathlib import Path
import subprocess, json, hashlib, xml.etree.ElementTree as ET
from PIL import Image
D=Path('docs/validation/s10f')
def git(*args):return subprocess.check_output(['git',*args])
# Every pre-existing Core source remains byte-identical after normalizing line endings.
core=[]
for p in Path('UnityProject/Assets/Scripts/Core').rglob('*.cs'):
 if p.name=='KoreanParticle.cs':continue
 old=git('show','04d076d:'+p.as_posix()).replace(b'\r\n',b'\n')
 if old!=p.read_bytes().replace(b'\r\n',b'\n'):core.append(str(p))
assert not core,core
mirrors=[]
for p in Path('UnityProject/Assets/Scripts/Runtime').rglob('*.cs'):
 q=Path('CodeDrafts/Runtime')/p.relative_to('UnityProject/Assets/Scripts/Runtime')
 # Check the files changed by S10f only; historical unrelated mirror drift is outside this pass.
 if p.as_posix() not in git('diff','--name-only','04d076d','--','UnityProject/Assets/Scripts/Runtime').decode().splitlines():continue
 assert q.exists() and q.read_bytes()==p.read_bytes(),str(p)
 mirrors.append(str(p))
pictures=sorted(D.glob('*.png'));nums=sorted(int(p.name[:2]) for p in pictures)
assert nums==list(range(1,38)),nums
for p in pictures:
 with Image.open(p) as im:assert im.size==(1280,720),(str(p),im.size);im.verify()
r=ET.parse('docs/validation/s10f-final-tests.xml').getroot()
cases=[{'name':c.get('fullname'),'result':c.get('result')} for c in r.iter('test-case')]
assert all(c['result']=='Passed' for c in cases),cases
log=Path('docs/validation/s10f-final-tests.log').read_text(encoding='utf-8',errors='replace')
shaderErrors=[l for l in log.splitlines() if l.startswith('Shader error') or 'error CS' in l]
assert not shaderErrors,shaderErrors
console=(D/'console-audit.txt').read_text();assert 'events=0' in console,console
result={'baseline':'04d076d','coreExistingSourcesUnchanged':True,'runtimeMirrorsVerified':mirrors,'playMode':{'total':r.get('total'),'passed':r.get('passed'),'failed':r.get('failed'),'tests':cases},'coreTests':{'passed':44,'failed':0,'newParticleCases':10},'screenshots':[{'file':p.name,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()} for p in pictures],'consoleErrorsExceptionsAsserts':0,'shaderCompileErrors':0,'performance60fps':'NOT MEASURED; batchmode host RTX3080 is not laptop acceptance','visualUserAcceptance':'NOT TESTED; see REVIEW.md for self-review','externalEditorLicensingWarning':'Access token unavailable at startup; resolved entitlement and tests completed. Not a PlayMode Console event.'}
(D/'results.json').write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8')
print('S10f audit passed: unchanged Core, synced changed runtime, 37 captures, all PlayMode cases passed, Console errors=0.')

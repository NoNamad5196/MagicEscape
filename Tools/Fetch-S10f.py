"""Fetch only the officially verified free S10f sources; local archive cache is gitignored."""
from pathlib import Path
import requests, zipfile, io, hashlib, json
cache=Path('Tools/asset-cache/s10f');cache.mkdir(parents=True,exist_ok=True)
sources={
 'Flame02.zip':'https://unity3d.com/files/labs/downloads/vfx/assets01/Flame02/Flame02-flipbooks.zip',
 'NotoSansSymbols.ttf':'https://raw.githubusercontent.com/google/fonts/main/ofl/notosanssymbols/NotoSansSymbols%5Bwght%5D.ttf',
 'OFL.txt':'https://raw.githubusercontent.com/google/fonts/main/ofl/notosanssymbols/OFL.txt'}
audit={}
for name,url in sources.items():
 p=cache/name
 if not p.exists():
  r=requests.get(url,timeout=120);r.raise_for_status();p.write_bytes(r.content)
 audit[name]={'url':url,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()}
 if name.endswith('.zip'):
  with zipfile.ZipFile(p) as z:
   for member in z.namelist():
    # Do not permit zip traversal even for official archives.
    out=(cache/'Flame02'/member).resolve()
    if not out.is_relative_to((cache/'Flame02').resolve()):raise ValueError(member)
   z.extractall(cache/'Flame02')
Path('docs/validation/s10f').mkdir(parents=True,exist_ok=True)
Path('docs/validation/s10f/source-hashes.json').write_text(json.dumps(audit,indent=2))
print('Verified-source downloads cached; source SHA-256 manifest written.')

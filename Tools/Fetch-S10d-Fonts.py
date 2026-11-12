"""Fetch original OFL fonts with reviewed-file SHA-256 verification. Run at repo root."""
from pathlib import Path
import hashlib,json,urllib.request
for entry in json.loads(Path('docs/validation/s10d/font-sources.json').read_text(encoding='utf-8')):
 data=urllib.request.urlopen(entry['url']).read()
 if hashlib.sha256(data).hexdigest()!=entry['sha256']:
  raise RuntimeError('Upstream font changed; review source/license before updating: '+entry['url'])
 target=Path(entry['path']);target.parent.mkdir(parents=True,exist_ok=True);target.write_bytes(data)
 print(target.name)

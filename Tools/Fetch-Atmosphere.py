"""Fetch only public CC0 recordings; keep source evidence outside Unity."""
import requests, re, pathlib, subprocess, json,sys
sys.stdout.reconfigure(encoding='utf-8')
from bs4 import BeautifulSoup
cache=pathlib.Path('Tools/asset-cache/atmosphere'); cache.mkdir(parents=True,exist_ok=True)
for key,url in [('wind','https://opengameart.org/content/wind1'),('drone','https://opengameart.org/content/ambient-sound-creepy'),('creak','https://freesound.org/people/Rudmer_Rotteveel/sounds/502505/')]:
    r=requests.get(url);r.raise_for_status();(cache/(key+'.html')).write_text(r.text,encoding='utf8')
    print(key, '\n'.join(sorted(set(re.findall(r'https?[^\s"<>]+?\.(?:wav|ogg|mp3)(?:\?[^\s"<>]*)?',r.text)))))
    soup=BeautifulSoup(r.text,'html.parser')
    # Do not commit login forms/session HTML; store only the factual author/license fields.
    assert 'CC0' in soup.get_text(), 'Source must explicitly offer CC0'

sources=[('wind','https://opengameart.org/sites/default/files/wind1.wav'),('drone','https://opengameart.org/sites/default/files/ambientmusic.mp3'),('creak','https://cdn.freesound.org/previews/502/502505_4921277-hq.mp3')]
out=pathlib.Path('UnityProject/Assets/ThirdParty/AtmosphereCC0');out.mkdir(exist_ok=True)
for key,url in sources:
    r=requests.get(url);r.raise_for_status();src=cache/(key+pathlib.Path(url).suffix);src.write_bytes(r.content)
    # Decode to modest mono PCM. Crossfade bed ends to make a seamless circular loop.
    raw=cache/(key+'-decoded.wav')
    filters=['-af','lowpass=f=220'] if key=='drone' else ['-af','lowpass=f=900'] if key=='wind' else []
    subprocess.run(['ffmpeg','-y','-i',str(src),*filters,'-ar','22050','-ac','1',str(raw)],check=True,stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL)
    import wave,numpy as np
    with wave.open(str(raw),'rb') as w: data=np.frombuffer(w.readframes(w.getnframes()),dtype='<i2').astype(np.float64)/32768
    if key!='creak':
        data=data[:22050*48]; n=min(22050*2,len(data)//5)
        # Overlap tail onto head; rotated result has no hard splice at the loop boundary.
        x=np.linspace(0,1,n,endpoint=False)
        data=np.concatenate([data[n:-n],data[-n:]*(1-x)+data[:n]*x])
    peak=max(abs(data));data=data/max(peak,.001)*.7
    with wave.open(str(out/(key+'.wav')),'wb') as w:w.setnchannels(1);w.setsampwidth(2);w.setframerate(22050);w.writeframes((data*32767).astype('<i2').tobytes())
    print(key,len(data)/22050,'seconds')


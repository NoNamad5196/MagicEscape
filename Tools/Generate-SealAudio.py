"""Original deterministic metal latch sound. No samples, external inputs or dependencies."""
from pathlib import Path
import math
import random
import struct
import wave

out = Path('UnityProject/Assets/Art/S10e/SealLatch.wav')
rng = random.Random(5196)
rate = 44100
samples = []
for n in range(int(rate * .72)):
    t = n / rate
    value = 0.
    for onset, gain in ((0., 1.), (.065, .55), (.18, .42)):
        q = t - onset
        if q < 0:
            continue
        # A sliding latch attack and inharmonic struck-metal tails.
        value += rng.uniform(-1, 1) * math.exp(-q * 110) * .24 * gain
        for hz, decay, level in ((347, 16, .23), (823, 13, .16), (1481, 22, .10), (2713, 30, .065)):
            value += math.sin(2 * math.pi * hz * q) * math.exp(-q * decay) * level * gain
    samples.append(value)
peak = max(abs(s) for s in samples)
out.parent.mkdir(parents=True, exist_ok=True)
with wave.open(str(out), 'wb') as wav:
    wav.setparams((1, 2, rate, 0, 'NONE', 'not compressed'))
    wav.writeframes(b''.join(struct.pack('<h', int(s / peak * 24575)) for s in samples))
print('Original SealLatch.wav: mono, 44.1 kHz, 0.72 seconds, peak -2.5 dBFS.')

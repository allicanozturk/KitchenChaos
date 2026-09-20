#!/usr/bin/env python3
"""Generate a procedural original temporary spatula-impact sound.

Provenance: original synthesized placeholder for KitchenChaos; no third-party
samples, recordings, or downloaded media. May be used and modified with the game.
The deterministic output is mono 44.1 kHz, 16-bit PCM and peaks below 0.65.
Usage: python3 generate_spatula_impact.py /absolute/path/to/new_effect.wav
"""

import argparse
import math
from pathlib import Path
import random
import struct
import wave


def generate(output: Path) -> None:
    sample_rate = 44_100
    duration = 0.15
    count = round(duration * sample_rate)
    rng = random.Random(73641)
    samples = []
    filtered_noise = 0.0
    for index in range(count):
        t = index / sample_rate
        noise = rng.uniform(-1.0, 1.0)
        filtered_noise = 0.55 * filtered_noise + 0.45 * noise
        thud = 0.58 * math.sin(2 * math.pi * 135 * t) * math.exp(-48 * t)
        clack = (
            0.22 * math.sin(2 * math.pi * 1_350 * t)
            + 0.12 * math.sin(2 * math.pi * 2_183 * t)
            + 0.06 * math.sin(2 * math.pi * 3_471 * t)
        ) * math.exp(-43 * t)
        contact = 0.28 * filtered_noise * math.exp(-105 * t)
        fade_in = min(1.0, t / 0.0015)
        fade_out = min(1.0, (count - 1 - index) / (sample_rate * 0.018))
        samples.append((thud + clack + contact) * fade_in * fade_out)

    scale = 0.60 / max(abs(sample) for sample in samples)
    pcm = b"".join(struct.pack("<h", round(sample * scale * 32767)) for sample in samples)

    # Exclusive creation protects existing assets, including under concurrent runs.
    with output.open("xb") as stream:
        with wave.open(stream, "wb") as wav:
            wav.setnchannels(1)
            wav.setsampwidth(2)
            wav.setframerate(sample_rate)
            wav.writeframes(pcm)
    print(f"Created {output.resolve()} (0.15 s, mono PCM16, 44100 Hz, peak <= 0.60)")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("output", type=Path, help="New WAV file; parent directory must exist")
    args = parser.parse_args()
    try:
        generate(args.output)
    except FileExistsError:
        parser.error(f"Refusing to overwrite existing file: {args.output}")
    except OSError as exc:
        parser.error(str(exc))

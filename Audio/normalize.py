import argparse
import os
from time import sleep

import pyaudacity as pa


def init():
    parser = argparse.ArgumentParser(
        prog="Normalizer", description="It will normalize her"
    )
    parser.add_argument("path")
    args = parser.parse_args()

    pa.do("New")

    for root, dirs, files in os.walk(args.path):
        for file in files:
            if not file.endswith(".wav"):
                continue

            filepath = os.path.abspath(os.path.join(root, file))
            pa.do(f'Import2: Filename="{filepath}"')

            pa.do("Select: Track=0")
            pa.do("SelTrackStartToEnd")
            pa.do("Normalize: PeakLevel=-1")

            sleep(0.01)

            pa.do(f'Export2: Filename="{filepath}"')
            pa.do("TrackClose")

            print("Normalizing ", filepath)


if __name__ == "__main__":
    init()

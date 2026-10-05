"""Read actual player PNGs; reject blank frames or incorrect aspect-ratio margins."""
from pathlib import Path
import json
import argparse
from PIL import Image, ImageStat

root=Path(__file__).resolve().parents[2]
parser=argparse.ArgumentParser(); parser.add_argument('--version',choices=['ArtV3','ArtV4','ArtV5','ArtV6','ArtV7','ArtV8','ArtV9','ArtV10'],default='ArtV3'); options=parser.parse_args()
folder=root/'docs/previews'/options.version
checked=[]
for size in [(1280,720),(1702,726),(1024,768)]:
    files=sorted(folder.glob(f'Player_*_{size[0]}x{size[1]}.png'))
    expected=(26 if size==(1280,720) else 6) if options.version in ['ArtV5','ArtV6'] else (30 if size==(1280,720) else 4)
    if options.version=='ArtV7': expected=28 if size==(1280,720) else 8
    if options.version=='ArtV8': expected=30 if size==(1280,720) else 8
    if options.version=='ArtV9': expected=35 if size==(1280,720) else 8
    if options.version=='ArtV10': expected=38 if size==(1280,720) else 8
    assert len(files)==expected, (size,len(files))
    for path in files:
        with Image.open(path) as source:
            image=source.convert('RGB')
            assert image.size==size, (path,image.size)
            assert sum(ImageStat.Stat(image).mean)>30, f'Blank frame: {path}'
            if size==(1702,726):
                assert max(ImageStat.Stat(image.crop((0,0,204,700))).mean)<1
                assert max(ImageStat.Stat(image.crop((1498,0,1702,700))).mean)<1
            if size==(1024,768):
                assert max(ImageStat.Stat(image.crop((0,0,1024,94))).mean)<1
                assert max(ImageStat.Stat(image.crop((0,674,1024,740))).mean)<1
            checked.append({'file':path.name,'size':list(size),'nonblank':True})
(folder/'capture-verification.json').write_text(json.dumps({'result':'PASS','frames':checked},indent=2),encoding='utf-8')
print('PLAYER_FRAME_VERIFICATION_PASS:',len(checked),'nonblank PNGs / 16:9, wide and 4:3 margins')


"""Create a local gallery of actual Unity GPU captures (no generated concept frames)."""
from pathlib import Path
from html import escape
import argparse

root = Path(__file__).resolve().parents[2]
parser=argparse.ArgumentParser(); parser.add_argument('--version',choices=['ArtV3','ArtV4','ArtV5','ArtV6','ArtV7','ArtV8','ArtV9','ArtV10'],default='ArtV3'); options=parser.parse_args()
output = root / "docs/previews" / options.version
chunks = ["CH_Mine_Start", "CH_Mine_Straight_A", "CH_Mine_Straight_B",
          "CH_Transition_MineToPrison", "CH_Prison_Corridor_A", "CH_Prison_Corridor_B",
          "CH_Checkpoint", "CH_Stair", "CH_Laundry", "CH_Kitchen", "CH_Maintenance",
          "CH_Yard", "CH_OuterWall"]
if options.version in ['ArtV5','ArtV6','ArtV7','ArtV8','ArtV9','ArtV10']: chunks=[chunks[i] for i in [0,1,2,3,7,4,5,6,11,12]]

def figure(name):
    if not (output / (name + ".png")).exists():
        return ""
    return f'<figure><a href="{name}.png"><img loading="lazy" src="{name}.png" alt="{escape(name)}"></a><figcaption>{escape(name)}</figcaption></figure>'

flow_names=["Title","WeightSetup","Loading","Idea","Takeover","FirstPerson"] if options.version in ['ArtV5','ArtV6'] else ["Title","Idea","Takeover","FirstPerson"]
if options.version=='ArtV7': flow_names=["Title","WeightSetup","Loading","LoadingReveal","FirstLiveFrame","Idea","Takeover","FirstPerson"]
if options.version=='ArtV8': flow_names=["Title","WeightSetup","Loading","LoadingReveal","FirstLiveFrame","CartRide","CartBank","CartExit","Slip","GameOver"]
if options.version in ['ArtV9','ArtV10']: flow_names=["Title","WeightSetup","Loading","LoadingReveal","FirstLiveFrame","CartRide","CartBank","CartExit","ClosingGate","SlideUnderGate","PrisonEntry","Slip","BananaCatch","RailCrash","GameOver"]
if options.version=='ArtV10': flow_names+=['CellCloseup','CharacterContact','Pause']
frames = "".join(figure("Player_" + name+"_1280x720") for name in flow_names)
if not frames:
    if options.version=='ArtV10': flow_names+=['CellCloseup','CharacterContact','Pause']
frames = "".join(figure(name) for name in ["Title", "Idea", "Takeover", "FirstPerson"])
maps = "".join(f'<section><h2>{escape(chunk)}</h2><div class="pair">{figure("Player_Map_"+str(i).zfill(2)+"_1280x720") or figure(chunk)}{figure("Player_Map_"+str(i).zfill(2)+"_Boundary_1280x720") or figure(chunk + "_Boundary")}</div></section>' for i,chunk in enumerate(chunks))
ratio_flow = "CartRide" if options.version in ["ArtV8","ArtV9","ArtV10"] else "FirstPerson"
ratios = "".join(figure("Player_Title_"+ratio)+figure("Player_"+ratio_flow+"_"+ratio) for ratio in ["1702x726","1024x768"])
html = '''<!doctype html><html lang="ko"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<title>무한옥 · 실제 Unity 아트 검수</title><style>
*{box-sizing:border-box}body{margin:0;background:#111820;color:#eee;font:16px system-ui,sans-serif}main{max-width:1500px;margin:auto;padding:32px}h1{color:#ffc365}h2{font-size:19px;margin-top:34px}p{color:#abb6c4;line-height:1.6}.pair,.flow{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:16px}figure{margin:0;border:1px solid #394555;background:#1b2531;border-radius:8px;overflow:hidden}img{width:100%;display:block}figcaption{padding:12px;font-size:13px;color:#bec8d7}@media(max-width:800px){.pair,.flow{grid-template-columns:1fr}main{padding:16px}}
</style><main><h1>무한옥 · 실제 Unity 아트 검수</h1>
<p>무료 아트 개선 버전 / 2026-10-03. 아래 이미지는 실제 Unity GPU 렌더링입니다. 상단 Player 캡처는 로고와 HUD를 포함합니다. 맵별 캡처는 카메라 렌더만 포함하며 오른쪽은 다음 24m 청크 연결 경계입니다. 이미지를 누르면 원본을 엽니다.</p>
<p>화면 비율은 16:9를 유지하고 남는 공간에는 검은 여백을 표시합니다. Blender 제작 암벽·손·기린·코끼리, Kenney CC0 나무·설비를 실제 적용했습니다. 캐릭터와 연출에는 참고 이미지와 품질 차이가 있습니다. Asset Store 로그인을 확인했으며 Mine은 약관 승인 대기 중으로 미임포트 상태입니다.</p>
<div class="flow">''' + frames + "</div>" + maps + '<h2>실제 창 크기별 화면</h2><div class="flow">'+ratios+"</div></main></html>"
(output / "index.html").write_text(html, encoding="utf-8")
if options.version in ['ArtV4','ArtV5','ArtV6','ArtV7']:
    html=html.replace('Blender 제작 암벽·손·기린·코끼리, Kenney CC0 나무·설비를 실제 적용했습니다.',
        'Blender 제작 스킨 캐릭터·Humanoid 뼈대·자체 동작·암벽·손·동물·수레를 실제 적용했습니다. ProBuilder 13개 경로를 정적 메시로 구워 실제 게임에서 사용합니다. 실제 Mixamo 달리기·점프·숙이기·비틀거림 4개를 다운로드하여 자체 캐릭터에 리타게팅했습니다.')
    html=html.replace('맵별 캡처는 카메라 렌더만 포함하며 오른쪽은 다음 24m 청크 연결 경계입니다.',
        'Player_Map은 실제 실행 파일의 카메라와 HUD를 포함합니다. 맵별 오른쪽 이미지는 다음 24m 청크 연결 경계입니다. 카메라 전용 검수 이미지는 HUD가 없습니다.')
    html=html.replace('</main>', '<h2>Blender 원본 중립 조명 검수 · T-pose</h2>'+figure('BlenderCharacters')+'</main>')
    (output/'index.html').write_text(html,encoding='utf-8')
if options.version in ['ArtV5','ArtV6','ArtV7']:
    html=html.replace('무료 아트 개선 버전 / 2026-10-03.', 'V5 · 광산 → 계단 → 감옥 복도 → 사육장 / 2026-10-03.')
    html=html.replace('캐릭터와 연출에는 참고 이미지와 품질 차이가 있습니다.', '사용자 제공 두 번째 이미지를 로딩에 사용합니다. 선택 몸무게 입력과 몸동작 시간에 따른 예상 칼로리, 바닥까지 닫히는 감옥 문, 불규칙한 광산 암벽과 사육장 바닥/장식을 적용했습니다. 캐릭터 세부 형태와 연출에는 참고 이미지와 품질 차이가 있습니다.')
    (output/'index.html').write_text(html,encoding='utf-8')
if options.version in ['ArtV6','ArtV7']:
    html=html.replace('<h2>Blender 원본 중립 조명 검수 · T-pose</h2>', '')
    html=html.replace('V5 ·','V6 ·').replace('2026-10-03','2026-10-04')
    html=html.replace('Asset Store 로그인을 확인했으며 Mine은 약관 승인 대기 중으로 미임포트 상태입니다.', '무료 BitGem 경찰 상품의 다운로드/임포트 상태는 ART_V6.md에 기록합니다. 유료 원숭이 상품은 구매하지 않았습니다.')
    html=html.replace('캐릭터 세부 형태와 연출에는 참고 이미지와 품질 차이가 있습니다.', '이미지 생성 도구로 만든 재질 atlas를 실제 3D 맵에 투영하고, 두 개의 가까운 광원에 그림자 예산을 배정합니다. 둥근 1인칭 손과 동작 반응을 적용했습니다. 서브웨이 서퍼스·링피트와 동등한 최종 품질로 판정한 상태는 아닙니다.')
    (output/'index.html').write_text(html,encoding='utf-8')
if options.version=='ArtV7':
    html=html.replace('V6 ·','V7 ·')
    html=html.replace('서브웨이 서퍼스·링피트와 동등한 최종 품질로 판정한 상태는 아닙니다.', '로딩 이미지를 실제 광산 장면으로 0.85초간 전환합니다. 갈색 원숭이를 연결된 스킨 메시로 다시 만들고 눈 깜빡임/입 표정, 큰 암벽 단면과 목재 금속 밴드를 적용했습니다. FirstLiveFrame은 그림이 아닌 실제 3D 렌더입니다. 참고 이미지와 동일한 최종 품질로 판정한 상태는 아닙니다.')
    html=html.replace('ART_V6.md','ART_V7.md')
    html=html.replace('</main>','<h2>Blender 실제 제작 원본 · 자체 모델</h2>'+figure('BlenderCharacters')+'</main>')
    (output/'index.html').write_text(html,encoding='utf-8')
if options.version=='ArtV8':
    html=html.replace('무료 아트 개선 버전 / 2026-10-03.','V8 · 로딩 인트로 / 수레 조작 / 즉사와 감속 / 2026-10-04.')
    html=html.replace('맵별 캡처는 카메라 렌더만 포함하며 오른쪽은 다음 24m 청크 연결 경계입니다.','게임 실행 파일의 실제 카메라와 HUD를 촬영했습니다. 오른쪽은 다음 청크와의 연결부입니다. 촬영 중 자동 입력으로 장애물을 피하며 실제 충돌 판정을 유지합니다.')
    html=html.replace('Blender 제작 암벽·손·기린·코끼리, Kenney CC0 나무·설비를 실제 적용했습니다.','Blender 제작 빈 수레·스킨 캐릭터·암벽·동물, Kenney CC0 수목·설비와 ProBuilder 경로를 실제 적용했습니다. 수레 조향·숙이기·하차와 바나나/물웅덩이 감속, 한 번의 장애물 충돌 게임오버를 실제 판정합니다. 로딩 뒤 별도의 8초 인트로는 재생하지 않습니다. 감옥 출구·사육장 진입과 밤하늘을 보강했습니다.')
    html=html.replace('Asset Store 로그인을 확인했으며 Mine은 약관 승인 대기 중으로 미임포트 상태입니다.','무료 경찰 107256은 계정 취득 이후 다운로드 인증 실패로 현재 미임포트입니다. 화면의 경찰은 자체 모델입니다. 서브웨이 서퍼스 수준의 최종 아트 품질로 인증한 결과가 아닙니다. 제작/검증 기록: ART_V8.md, ART_V8_QA.md.')
    (output/'index.html').write_text(html,encoding='utf-8')
print(output / "index.html")
if options.version=='ArtV9':
    html=html.replace('무료 아트 개선 버전 / 2026-10-03.','V9 · 전용 카트 철로 / 운동 장애물 / 바나나 추격 / 2026-10-04.')
    html=html.replace('맵별 캡처는 카메라 렌더만 포함하며 오른쪽은 다음 24m 청크 연결 경계입니다.','Player_Map은 실제 실행 파일의 카메라와 HUD입니다. 오른쪽은 다음 청크와의 연결부입니다. 자동 입력도 실제 충돌/기울이기 판정을 통과합니다.')
    html=html.replace('Blender 제작 암벽·손·기린·코끼리, Kenney CC0 나무·설비를 실제 적용했습니다.','Blender 스킨 캐릭터와 전용 수레 그립 동작, 빈 수레, ProBuilder 단선 절벽 다리와 한쪽 레일 붕괴를 실제 적용했습니다. 카트 길의 일반 장애물은 없으며 좌우 몸 기울이기로 네 개의 붕괴 지점을 통과합니다. 걷는 구간의 점프·숙이기·하이니 장애물과 바나나 두 번 접촉 후 추격을 실제 판정합니다.')
    html=html.replace('Asset Store 로그인을 확인했으며 Mine은 약관 승인 대기 중으로 미임포트 상태입니다.','화면의 경찰은 자체 모델입니다. BitGem 무료 경찰은 정상 다운로드 인증 문제로 미임포트 상태입니다. 상용 게임과 동등한 최종 품질로 판정한 결과가 아닙니다. ART_V9.md / ART_V9_QA.md에 검증과 제한을 기록합니다.')
    (output/'index.html').write_text(html,encoding='utf-8')
if options.version=='ArtV10':
    html=html.replace('무료 아트 개선 버전 / 2026-10-03.', 'V10 · 철창 독방과 캐릭터 접지 / 2026-10-05.')
    html=html.replace('맵별 캡처는 카메라 렌더만 포함하며 오른쪽은 다음 24m 청크 연결 경계입니다.', '모든 Player 이미지는 실제 실행 파일의 화면입니다. CellCloseup과 CharacterContact는 제작 검수용 카메라입니다. 맵 오른쪽 이미지는 다음 청크 연결부입니다.')
    html=html.replace('Blender 제작 암벽·손·기린·코끼리, Kenney CC0 나무·설비를 실제 적용했습니다.', '독방은 바닥·뒷벽·칸막이·잠긴 철창 문으로 구성했습니다. 침대는 방 안에 있고, 각 복도 청크에는 실제 스킨 원숭이 2명이 대기 동작을 재생합니다. 캐릭터에 수감 번호, 손목 밴드와 손발톱을 추가했고 발 높이·경사·짧은 접지 고정 보정을 적용했습니다. 실제 Mixamo 4개 동작과 Blender 자체 동작을 구분해 사용합니다.')
    html=html.replace('Asset Store 로그인을 확인했으며 Mine은 약관 승인 대기 중으로 미임포트 상태입니다.', '현재 경찰은 자체 모델입니다. 무료 BitGem 경찰은 다운로드 인증 문제로 미임포트 상태입니다. 상용 게임과 동등한 최종 품질로 판정한 결과는 아닙니다. 화면별 Word 기획서는 docs/무한옥_화면별_게임기획서_V10.docx입니다.')
    html=html.replace('</main>', '<h2>Blender 원본 모델</h2>'+figure('BlenderCharacters')+'</main>')
    (output/'index.html').write_text(html,encoding='utf-8')


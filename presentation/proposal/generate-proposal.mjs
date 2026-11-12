import fs from 'node:fs';
import path from 'node:path';
import {execFileSync} from 'node:child_process';
const root=path.resolve(import.meta.dirname,'../..');
const out=import.meta.dirname, build=path.join(out,'.build');fs.mkdirSync(build,{recursive:true});
const E=s=>String(s).replaceAll('&','&amp;').replaceAll('<','&lt;').replaceAll('>','&gt;').replaceAll('"','&quot;');
const U=v=>Math.round(v*12700), bg='FFFFFF', gold='2864B4', ink='182638', muted='586576', blue='2864B4', red='2864B4';
let parts={},slides=[],s,id,rels;
const put=(p,t)=>parts[p]=t;
const solid=(c,a=100000)=>c==='none'?'<a:noFill/>':`<a:solidFill><a:srgbClr val="${c}"><a:alpha val="${a}"/></a:srgbClr></a:solidFill>`;
function shape(x,y,w,h,geo='rect',fill='none',stroke=gold,sw=.8,alpha=100000){let n=id++;s+=`<p:sp><p:nvSpPr><p:cNvPr id="${n}" name="shape-${n}"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr><p:spPr><a:xfrm><a:off x="${U(x)}" y="${U(y)}"/><a:ext cx="${U(w)}" cy="${U(h)}"/></a:xfrm><a:prstGeom prst="${geo}"><a:avLst/></a:prstGeom>${solid(fill,alpha)}<a:ln w="${U(sw)}">${solid(stroke)}</a:ln></p:spPr></p:sp>`;}
function text(t,x,y,w,h,size=16,color=ink,bold=false,align='l',font=(size>=20?'Paperlogy 7 Bold':'Pretendard')){let n=id++;s+=`<p:sp><p:nvSpPr><p:cNvPr id="${n}" name="text-${n}"/><p:cNvSpPr txBox="1"/><p:nvPr/></p:nvSpPr><p:spPr><a:xfrm><a:off x="${U(x)}" y="${U(y)}"/><a:ext cx="${U(w)}" cy="${U(h)}"/></a:xfrm><a:prstGeom prst="rect"><a:avLst/></a:prstGeom><a:noFill/><a:ln><a:noFill/></a:ln></p:spPr><p:txBody><a:bodyPr wrap="square" lIns="0" rIns="0" tIns="0" bIns="0" anchor="t"/><a:lstStyle/>${String(t).split('\n').map(line=>`<a:p><a:pPr algn="${align}" marL="0" marR="0"><a:lnSpc><a:spcPct val="118000"/></a:lnSpc><a:spcAft><a:spcPts val="500"/></a:spcAft></a:pPr><a:r><a:rPr lang="ko-KR" sz="${size*100}" b="${bold?1:0}">${solid(color)}<a:latin typeface="${font}"/><a:ea typeface="${font}"/><a:cs typeface="${font}"/></a:rPr><a:t>${E(line)}</a:t></a:r><a:endParaRPr lang="ko-KR" sz="${size*100}"/></a:p>`).join('')}</p:txBody></p:sp>`;}
function line(x,y,x2,y2,c=gold,sw=1){let n=id++;let x0=Math.min(x,x2),y0=Math.min(y,y2);s+=`<p:sp><p:nvSpPr><p:cNvPr id="${n}" name="line-${n}"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr><p:spPr><a:xfrm flipV="${(x2-x)*(y2-y)<0?1:0}"><a:off x="${U(x0)}" y="${U(y0)}"/><a:ext cx="${Math.max(1,U(Math.abs(x2-x)))}" cy="${Math.max(1,U(Math.abs(y2-y)))}"/></a:xfrm><a:prstGeom prst="line"><a:avLst/></a:prstGeom><a:noFill/><a:ln w="${U(sw)}">${solid(c)}</a:ln></p:spPr></p:sp>`;}
function start(title,sub=''){s='';id=2;rels=[{id:'rId1',target:'../slideLayouts/slideLayout1.xml',type:'slideLayout'}];text(String(slides.length+1).padStart(2,'0'),878,32,42,23,14,gold,false,'r');text(title,44,38,808,46,28,ink,true);if(sub)text(sub,46,91,866,35,15,muted);line(44,130,916,130,gold,.6);}
function end(){text('MagicEscape   /   멀티미디어와 가상현실 기말 과제 제안서',44,497,840,18,10,muted);let i=slides.length+1;slides.push(s);put(`ppt/slides/slide${i}.xml`,`<?xml version="1.0" encoding="UTF-8" standalone="yes"?><p:sld xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships" xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main"><p:cSld><p:bg><p:bgPr>${solid(bg)}<a:effectLst/></p:bgPr></p:bg><p:spTree><p:nvGrpSpPr><p:cNvPr id="1" name=""/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr><p:grpSpPr><a:xfrm><a:off x="0" y="0"/><a:ext cx="0" cy="0"/><a:chOff x="0" y="0"/><a:chExt cx="0" cy="0"/></a:xfrm></p:grpSpPr>${s}</p:spTree></p:cSld><p:clrMapOvr><a:masterClrMapping/></p:clrMapOvr></p:sld>`);put(`ppt/slides/_rels/slide${i}.xml.rels`,relationships(rels));}
const relationships=rs=>`<?xml version="1.0" encoding="UTF-8"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">${rs.map(r=>`<Relationship Id="${r.id}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/${r.type||'image'}" Target="${r.target}"/>`).join('')}</Relationships>`;
function row(label,value,y){line(46,y+44,912,y+44,gold,.5);text(label,48,y,165,40,17,gold);text(value,230,y,674,40,18);}
function note(t,y=450){text(t,46,y,867,39,14,muted);}
// 01
s='';id=2;rels=[{id:'rId1',target:'../slideLayouts/slideLayout1.xml',type:'slideLayout'}];
text('멀티미디어와 가상현실 기말 과제 제안서',54,72,840,32,17,muted);
text('MagicEscape',50,176,850,82,56,ink,true);
text('마법학교 연금술 교실 방탈출',54,270,850,50,28,ink,true);
line(54,350,906,350,gold,2);
text('Windows · Unity 6.3 LTS · 1인칭 퍼즐 3종',54,378,850,36,19);
text('1인 제작     이름 / 학번',54,451,820,32,18);end();
//02
start('작품 개요','PC 멀티미디어 작품 · 한 교실 안에서 이어지는 퍼즐 3개');
[['장르','1인칭 3D 퍼즐 방탈출'],['플랫폼 / 엔진','Windows · 키보드·마우스 / Unity 6.3 LTS · URP'],['시간 / 공간','10~15분 목표 / 교실 12×10m · 천장 4.5m'],['최종 목표','문 위 봉인등 3개 소등 후 출구 통과'],['제작 범위','1인 팀 · 교실 1개 · 퍼즐 3개 · 시작/엔딩 연출'],['조작','WASD 이동 · 마우스 시점 · E 조사/사용 · 1~8/휠 선택']].forEach((r,i)=>row(...r,146+i*48));note('Tab 목표/힌트 · ESC 일시정지',451);end();
//03
start('글 단서를 그림 단서로 바꾼 설계','정답과 입력 규칙은 유지하고, 단서를 확인하는 방법을 바꾼다');
text('이전: 칠판 문장',48,156,385,30,21,gold);
text('“생명은 정해진 순서로 깨어난다.\n액자의 숫자를 따르라.”',48,205,382,67,19);
text('이후: 칠판 도식과 액자 점등',496,156,416,30,21,gold);
['I','II','III','IV'].forEach((v,i)=>{shape(497+i*101,205,79,52,'rect','none',gold);text(v,497+i*101,216,79,30,19,ink,false,'ctr');});
text('액자에서 원소와 번호 확인 → 순서대로 입력\n맞힌 액자 점등 · 틀리면 전체 소등',496,283,414,64,18);
line(48,362,912,362,gold,.6);
text('액자',48,390,150,30,21,gold);text('번호별 원소 제공',48,433,230,31,18);
text('칠판',344,390,150,30,21,gold);text('I부터 IV까지 순서의 틀',344,433,260,31,18);
text('피드백',658,390,180,30,21,gold);text('입력한 순서 즉시 확인',658,433,254,31,18);end();
//04
start('원소 실습이 끝난 뒤의 교실','고풍 다크 판타지 · 아르카나 마법학교의 연금술 실습실');
text('방과 후 원소 마법을 연습하던 견습생이 삼중 봉인을 깨운다.\n교수가 남긴 액자와 마법서에는 봉인을 푸는 규칙이 숨어 있다.\n문 위 세 불빛이 꺼지면 실습은 끝난다.',48,155,855,99,19);
line(48,274,908,274,gold,.7);text('공간의 재료',48,301,385,30,22,gold);text('석벽·짙은 목재·무쇠 가마솥\n낡은 책·황동 각인·촛불 6개\n연금술 도안은 낮은 채도의 판화 질감',48,348,410,107,18);text('플레이 중의 밝기',505,301,390,30,22,gold);text('단서 부위의 국소 조명은 끝까지 보존\n봉인 1개 해제마다 촛불·주변광 감소\n차가운 야간 복도에서 검정 페이드',505,348,390,107,18);end();
//05
start('교실 평면과 재료 위치','위에서 본 배치 · 바닥 중심 (0,0,0) · +X 동쪽 / +Z 북쪽 · 단위 m');
const mx=82,my=170,sc=24;const pos=(x,z)=>[mx+(x+6)*sc,my+(5-z)*sc];
shape(mx,my,288,240,'rect','F4F7FB',gold,1.1);line(mx+108,my,mx+180,my,bg,4);text('출구 3m',mx+91,my-30,111,20,14,gold);shape(mx+108,my-4,72,4,'rect','none',gold,.7);text('북 +Z',81,141,80,24,14,gold);text('12m',187,417,80,25,14,muted);text('10m',24,277,54,25,14,muted);
function mapdot(x,z,lab,c=gold,r=6){let [a,b]=pos(x,z);shape(a-r,b-r,r*2,r*2,'ellipse',c,'none');if(lab)text(lab,a-14,b-10,30,22,14,bg,true,'ctr');}
mapdot(-3,3.7,'S',gold,11);mapdot(3,2.5,'C',gold,11);mapdot(4.2,3.6,'B',gold,10);mapdot(0,.5,'',gold,19);let [ax,ay]=pos(0,.5);for(let i=0;i<6;i++)shape(ax+Math.sin(i*Math.PI/3)*13-3,ay-Math.cos(i*Math.PI/3)*13-3,6,6,'ellipse',bg,'none');text('제단',ax-26,ay+24,61,25,14);shape(...pos(5.45,3),11,99,'rect','none',gold,.8);text('책장',385,252,54,25,14);
[-2.4,-.8,.8,2.4].forEach((z,i)=>{let [a,b]=pos(-5.88,z);shape(a-2,b-8,8,16,'rect',gold,'none');});let [bx,by]=pos(-5.88,-4);shape(bx-2,by-8,8,16,'rect',blue,'none');text('액자 4점',8,213,73,24,14);text('칠판',12,374,60,24,14);
let [dx,dy]=pos(-3.5,-3.25);shape(dx-18,dy-8,36,17,'rect','none',gold,.8);mapdot(-3.5,-3.25,'3',ink,8);mapdot(5,-4.2,'1',ink,8);mapdot(5.6,1.5,'2',ink,8);mapdot(-5.4,4.4,'4',ink,8);mapdot(0,-4,'',blue,5);text('시작',222,390,60,24,14,blue);text('창가',318,418,64,23,14);line(315,410,360,410,blue,3);
text('도면 기준 좌표 (X, Z)',471,151,420,30,20,gold);text('제단  (0, 0.5) · 문양석 6개 원형\nC 가마솥  (3, 2.5)\nB 마법서 독서대  (4.2, 3.6)\nS 봉인 장치  (-3, 3.7) · 문 옆\n액자  X=-5.88, Z=-2.4/-0.8/0.8/2.4',471,191,431,150,16);text('1 월광초 꽃잎   창가 (5, -4.2)\n2 불도마뱀 비늘   동쪽 책장 (5.6, 1.5)\n3 수정 가루   교탁 서랍 (-3.5, -3.25)\n4 독버섯   북서쪽 바닥 (-5.4, 4.4)',471,349,431,113,16);note('출구 앞 3×2m는 이동을 위해 비움. 점은 배치 기준, 소품 윤곽은 설명용.',463);end();
//06
start('퍼즐 연결과 봉인등','처음에는 문양 해독만 가능 · 앞 퍼즐을 풀면 다음 장치 활성화');
const xs=[54,365,677];['P1 문양 해독','P2 마법약 제조','P3 최종 봉인'].forEach((t,i)=>{shape(xs[i],167,230,72,'rect','none',gold);text(t,xs[i]+15,187,205,38,21);if(i<2){line(xs[i]+230,202,xs[i+1]-12,202);text('→',xs[i]+246,181,45,35,25,gold);}});
text('푸른 수정\n제단에서 직접 획득',62,268,265,91,17,blue);text('계시의 물약 + 붉은 수정\n인벤토리에 직접 지급',373,268,280,91,17,gold);text('비문 공개 · 수정 2개\n봉인핵 활성화 · 문 개방\n출구 통과 시 클리어',685,268,230,91,17);
line(176,363,790,363,blue);line(176,351,176,363,blue);line(790,363,790,378,blue);text('P1 보상도 P3 소켓에 사용',270,370,430,30,16,blue);
['첫 봉인등 소등 / P2 활성화','둘째 봉인등 소등 / P3 활성화','셋째 봉인등 소등 / 출구 개방'].forEach((t,i)=>text(t,xs[i],420,272,35,17,gold));note('퍼즐 상태: 잠김 / 활성화 / 해결. 재시도 횟수 제한과 게임오버는 두지 않는다.',462);end();
//07
start('P1  문양 해독','액자 4점이 순서와 원소를 함께 보여 준다 · 칠판 도식과 겹쳐 읽는 단서');
text('단서 A  서쪽 벽 액자 4점',48,155,430,30,21,gold);
['III','I','IV','II'].forEach((v,i)=>{shape(49+i*102,201,86,85,'rect','none',gold);text(['불','대지','빛','물'][i],50+i*102,214,82,28,19,ink,false,'ctr');text(v,50+i*102,250,82,28,17,ink,false,'ctr');});text('벽 배치는 섞임 · 번호와 원소 이름을 함께 확인',48,298,430,50,16);
text('단서 B  칠판의 순서 도식',48,365,430,31,21,gold);text('I  →  II  →  III  →  IV',48,409,430,35,22);text('칠판은 채워야 할 네 칸의 순서를 보여 준다.\n각 칸에 넣을 원소의 답은 액자가 준다.',48,439,435,42,14,muted);
line(505,155,505,476,gold,.6);text('입력과 판정',539,155,360,30,21,gold);text('문양석 6개 중 4개를 차례로 E 입력\n정답: 대지 → 물 → 불 → 빛',539,204,363,69,18);text('정답 입력   해당 액자 점등\n오답 입력   전부 소등, 진행 0으로 초기화\n실패 3회    글 힌트 자동 표시',539,294,364,100,17);text('완료: 제단 열림 · 푸른 수정 등장\n첫 봉인등 소등 · 마법서 결계 해제',539,414,364,64,17,gold);end();
//08
start('P2  마법약 제조','번호 그림 3단계 + 독버섯 금지 표시 · 선택한 슬롯의 재료를 E로 투입');
const ing=[['01','월광초 꽃잎','','청백',blue],['02','불도마뱀 비늘','','주황',red],['03','수정 가루','','금색',gold]];
ing.forEach((r,i)=>{let x=49+i*304;text(r[0],x,154,64,42,30,gold);text(r[1],x,205,278,32,22);text('가마솥 액체: '+r[3],x,272,278,31,18);});
line(48,350,909,350,gold,.6);text('후보 4종, 함정 1개',48,368,390,29,21,gold);text('독버섯은 금지 그림으로 제외\n재료가 아닌 아이템은 거부하며 소모하지 않음',48,410,423,62,16);text('오답과 복구',513,368,390,29,21,gold);text('틀린 순서·독버섯: 검은 연기·실패음\n투입 재료 전부 원위치 재생성\n2회 실패 후 글 힌트',513,410,391,76,16);end();
//09
start('P3  최종 봉인 장치','비문판에 계시의 물약 사용 · 왼쪽 물 / 오른쪽 불 각인과 효과 공개');
shape(76,180,424,204,'rect','none',gold);text('물 각인',103,211,102,30,20,gold);text('불 각인',390,211,102,30,20,gold);shape(105,274,88,65,'ellipse','none',blue);shape(392,274,88,65,'ellipse','none',red);shape(265,276,49,49,'ellipse','none',gold,1.8);text('푸른 수정',83,350,148,30,18,blue,false,'ctr');text('봉인핵 E',235,350,113,30,18,gold,false,'ctr');text('붉은 수정',364,350,144,30,18,red,false,'ctr');text('왼쪽 소켓: 푸른 수정 (물)\n오른쪽 소켓: 붉은 수정 (불)',76,414,436,61,16);
text('봉인 해제 5단계',548,152,358,32,22,gold);['잠김: 앞 퍼즐 해결 대기','비문에 계시의 물약 사용','왼쪽·오른쪽 수정 배치','두 수정 고정 후 봉인핵 발광','봉인핵 E: 셋째 등 소등·문 개방'].forEach((t,i)=>{text(String(i+1).padStart(2,'0'),549,202+i*42,35,29,16,gold);text(t,594,202+i*42,313,32,16);});text('정답 수정만 소모·고정, 오답은 거부·보존\n2회 실패 시 글 힌트. 문 밖으로 이동하면 클리어',548,427,358,56,16,muted);end();
//10
start('아트 방향과 탈출 연출','16세기 연금술 사본·목판화의 선화 질감 참고');
text('원소 6종의 공통 문양 체계',48,157,856,32,23,gold);
text('대지·물·불·바람·빛·그림자를 같은 선 굵기와 형식으로 제작한다.\n같은 문양을 액자·제단·비문에 반복해 서로 다른 단서를 잇는다.',48,205,855,65,19);
['액자: 번호와 원소','제단: 문양 입력','비문: 소켓 대응'].forEach((v,i)=>{let x=48+i*300;shape(x,293,261,54,'rect','none',gold);text(v,x+12,306,237,30,19,ink,false,'ctr');if(i<2)text('→',x+270,304,29,30,19,gold);});
text('오프닝·봉인 해제·탈출',48,363,440,30,21,gold);
text('12초 동안 촛불 6개와 봉인등 3개 공개\n봉인 해제마다 주변광·촛불·방 소리 감소\n단서 조명 보존 · 야간 복도 뒤 검정 페이드',48,402,440,76,16);
text('소리는 행동 피드백',535,363,375,30,21,gold);
text('정답 차임 · 오답 둔탁음\n봉인 해제음과 방 소리의 변화\n순서와 원소의 답은 시각 단서로 전달',535,402,375,76,16);end();
//11
start('규칙·표현·씬 구성을 분리한 구조','순수 C# 퍼즐 규칙은 유지하고 입력과 3D 표현을 바꾸는 구성');
[['Core','정답·아이템·진행 규칙\nUnityEngine 참조 없음'],['Runtime','MonoBehaviour\n입력·Raycast·조명·UI·음향'],['Editor','씬 빌더\n배치·참조 연결·재현 검사']].forEach((r,i)=>{let x=48+i*305;shape(x,158,257,103,'rect','none',gold);text(r[0],x+14,171,226,30,22,gold);text(r[1],x+14,211,232,43,15);if(i<2)text('↔',x+264,191,35,35,21,gold);});
text('상태 머신',48,290,409,30,21,gold);text('GameFlowState\nTitle → Playing ↔ Paused → Cleared\nPuzzleState   Locked / Active / Solved',48,334,432,92,16);
text('입력과 상호작용',523,290,390,30,21,gold);text('PlayerInputReader 한 곳에서 입력 수집\n중앙 Raycast 2.5m → IInteractable.Interact\n인벤토리 8칸, 선택 슬롯의 아이템만 사용',523,334,390,95,16);
line(48,439,912,439,gold,.6);text('자동 테스트: 퍼즐 규칙 44개, Unity 플레이 테스트 19개',48,458,864,28,18);end();
//12
start('개발 단계와 사용 에셋','완료한 개발 단계와 남은 검증 단계');
text('지금까지 한 단계',48,153,399,30,21,gold);text('01  교실·시스템·UI·아이템\n02  퍼즐 3종·전체 입력 경로\n03  아트·그림 단서·어두운 탈출 연출',48,197,411,91,17);
line(48,301,456,301,gold,.7);text('남은 단계',48,319,409,28,21,gold);text('04  퍼즐 그림·문양 개편\n05  Windows 빌드\n06  처음 하는 사람 테스트와 수정\n07  최종 발표 시연',48,363,417,106,17);
text('유료 없음 · 외부 에셋은 CC0/OFL',501,153,414,30,20,gold);const asset=[['가구·촛대','KayKit Dungeon / Kay Lousberg','CC0'],['책장·가마솥','Fantasy Props / Quaternius','CC0'],['석벽·목재','Poly Haven','CC0'],['효과음','Kenney Interface Sounds','CC0'],['방 소리','Luke.RUSTLTD / DaGameKnower','CC0'],['나무 소리','Rudmer_Rotteveel','CC0'],['게임 글꼴','Gowun Batang / Nanum Pen / Noto','OFL']];asset.forEach((r,i)=>{let y=193+i*34;text(r[0],501,y,91,28,14,gold);text(r[1],598,y,247,28,14);text(r[2],858,y,54,28,14);line(501,y+29,911,y+29,gold,.4);});text('문양·그림·마스크·셰이더·걸쇠 합성음은 자체 제작\n출처: docs/CREDITS.md · 동봉 라이선스 유지',501,436,411,47,14,muted);end();
// Package
const ns='http://schemas.openxmlformats.org/presentationml/2006/main';
put('[Content_Types].xml',`<?xml version="1.0"?><Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/><Default Extension="png" ContentType="image/png"/>${[['/ppt/presentation.xml','presentation.main'],['/ppt/slideMasters/slideMaster1.xml','slideMaster'],['/ppt/slideLayouts/slideLayout1.xml','slideLayout'],...slides.map((_,i)=>['/ppt/slides/slide'+(i+1)+'.xml','slide'])].map(([p,t])=>`<Override PartName="${p}" ContentType="application/vnd.openxmlformats-officedocument.presentationml.${t}+xml"/>`).join('')}<Override PartName="/ppt/theme/theme1.xml" ContentType="application/vnd.openxmlformats-officedocument.theme+xml"/></Types>`);
put('_rels/.rels',relationships([{id:'rId1',type:'officeDocument',target:'ppt/presentation.xml'}]));
put('ppt/presentation.xml',`<?xml version="1.0"?><p:presentation xmlns:p="${ns}" xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><p:sldMasterIdLst><p:sldMasterId id="2147483648" r:id="rId1"/></p:sldMasterIdLst><p:sldIdLst>${slides.map((_,i)=>`<p:sldId id="${256+i}" r:id="rId${i+2}"/>`).join('')}</p:sldIdLst><p:sldSz cx="12192000" cy="6858000" type="screen16x9"/><p:notesSz cx="6858000" cy="9144000"/><p:defaultTextStyle/></p:presentation>`);
put('ppt/_rels/presentation.xml.rels',relationships([{id:'rId1',type:'slideMaster',target:'slideMasters/slideMaster1.xml'},...slides.map((_,i)=>({id:'rId'+(i+2),type:'slide',target:`slides/slide${i+1}.xml`}))]));
const group='<p:spTree><p:nvGrpSpPr><p:cNvPr id="1" name=""/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr><p:grpSpPr/></p:spTree>';
const cm='<p:clrMap accent1="accent1" accent2="accent2" accent3="accent3" accent4="accent4" accent5="accent5" accent6="accent6" bg1="lt1" bg2="lt2" folHlink="folHlink" hlink="hlink" tx1="dk1" tx2="dk2"/>';
put('ppt/slideMasters/slideMaster1.xml',`<p:sldMaster xmlns:p="${ns}" xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><p:cSld>${group}</p:cSld>${cm}<p:sldLayoutIdLst><p:sldLayoutId id="2147483649" r:id="rId1"/></p:sldLayoutIdLst><p:txStyles><p:titleStyle/><p:bodyStyle/><p:otherStyle/></p:txStyles></p:sldMaster>`);
put('ppt/slideMasters/_rels/slideMaster1.xml.rels',relationships([{id:'rId1',type:'slideLayout',target:'../slideLayouts/slideLayout1.xml'},{id:'rId2',type:'theme',target:'../theme/theme1.xml'}]));
put('ppt/slideLayouts/slideLayout1.xml',`<p:sldLayout xmlns:p="${ns}" xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" type="blank"><p:cSld name="Blank">${group}</p:cSld><p:clrMapOvr><a:masterClrMapping/></p:clrMapOvr></p:sldLayout>`);
put('ppt/slideLayouts/_rels/slideLayout1.xml.rels',relationships([{id:'rId1',type:'slideMaster',target:'../slideMasters/slideMaster1.xml'}]));
put('ppt/theme/theme1.xml',`<a:theme xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" name="Clean"><a:themeElements><a:clrScheme name="Clean">${['dk1','lt1','dk2','lt2','accent1','accent2','accent3','accent4','accent5','accent6','hlink','folHlink'].map((n,i)=>`<a:${n}><a:srgbClr val="${[bg,ink,'202329',muted,gold,blue,red,'586576','AEB8C4','EEF2F6',blue,gold][i]}"/></a:${n}>`).join('')}</a:clrScheme><a:fontScheme name="Presentation"><a:majorFont><a:latin typeface="Paperlogy 7 Bold"/><a:ea typeface="Paperlogy 7 Bold"/><a:cs typeface="Paperlogy 7 Bold"/></a:majorFont><a:minorFont><a:latin typeface="Pretendard"/><a:ea typeface="Pretendard"/><a:cs typeface="Pretendard"/></a:minorFont></a:fontScheme><a:fmtScheme name="Minimal"><a:fillStyleLst>${solid(ink).repeat(3)}</a:fillStyleLst><a:lnStyleLst>${[1,2,3].map(x=>`<a:ln w="${U(x)}">${solid(gold)}<a:prstDash val="solid"/></a:ln>`).join('')}</a:lnStyleLst><a:effectStyleLst><a:effectStyle><a:effectLst/></a:effectStyle><a:effectStyle><a:effectLst/></a:effectStyle><a:effectStyle><a:effectLst/></a:effectStyle></a:effectStyleLst><a:bgFillStyleLst>${solid(bg).repeat(3)}</a:bgFillStyleLst></a:fmtScheme></a:themeElements></a:theme>`);
fs.rmSync(path.join(build,'package'),{recursive:true,force:true});
for(const [p,t] of Object.entries(parts)){let f=path.join(build,'package',p);fs.mkdirSync(path.dirname(f),{recursive:true});fs.writeFileSync(f,t);}

execFileSync('python',['-c',`import pathlib,zipfile; p=pathlib.Path(r'${path.join(build,'package')}'); z=zipfile.ZipFile(r'${path.join(out,'MagicEscape_제안서.pptx')}','w',zipfile.ZIP_DEFLATED); [z.write(f,f.relative_to(p).as_posix()) for f in p.rglob('*') if f.is_file()]; z.close()`]);
console.log('Created 12 slides with editable native DrawingML shapes.');

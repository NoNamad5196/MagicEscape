import fs from 'node:fs';
import path from 'node:path';
import {execFileSync} from 'node:child_process';
const root=path.resolve(import.meta.dirname,'../..');
const out=import.meta.dirname, build=path.join(out,'.build');fs.mkdirSync(build,{recursive:true});
const E=s=>String(s).replaceAll('&','&amp;').replaceAll('<','&lt;').replaceAll('>','&gt;').replaceAll('"','&quot;');
const U=v=>Math.round(v*12700), bg='FFFFFF', gold='2864B4', ink='182638', muted='586576', blue='2864B4', red='2864B4';
const slideCount=12;
let parts={},slides=[],s,id,rels;
const put=(p,t)=>parts[p]=t;
const solid=(c,a=100000)=>c==='none'?'<a:noFill/>':`<a:solidFill><a:srgbClr val="${c}"><a:alpha val="${a}"/></a:srgbClr></a:solidFill>`;
function shape(x,y,w,h,geo='rect',fill='none',stroke=gold,sw=.8,alpha=100000){let n=id++;s+=`<p:sp><p:nvSpPr><p:cNvPr id="${n}" name="shape-${n}"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr><p:spPr><a:xfrm><a:off x="${U(x)}" y="${U(y)}"/><a:ext cx="${U(w)}" cy="${U(h)}"/></a:xfrm><a:prstGeom prst="${geo}"><a:avLst/></a:prstGeom>${solid(fill,alpha)}<a:ln w="${U(sw)}">${solid(stroke)}</a:ln></p:spPr></p:sp>`;}
function text(t,x,y,w,h,size=16,color=ink,bold=false,align='l',font=(bold?'Paperlogy 7 Bold':'Pretendard')){let n=id++;s+=`<p:sp><p:nvSpPr><p:cNvPr id="${n}" name="text-${n}"/><p:cNvSpPr txBox="1"/><p:nvPr/></p:nvSpPr><p:spPr><a:xfrm><a:off x="${U(x)}" y="${U(y)}"/><a:ext cx="${U(w)}" cy="${U(h)}"/></a:xfrm><a:prstGeom prst="rect"><a:avLst/></a:prstGeom><a:noFill/><a:ln><a:noFill/></a:ln></p:spPr><p:txBody><a:bodyPr wrap="square" lIns="0" rIns="0" tIns="0" bIns="0" anchor="t"/><a:lstStyle/>${String(t).split('\n').map(line=>`<a:p><a:pPr algn="${align}" marL="0" marR="0"><a:lnSpc><a:spcPct val="118000"/></a:lnSpc><a:spcAft><a:spcPts val="500"/></a:spcAft></a:pPr><a:r><a:rPr lang="ko-KR" sz="${size*100}" b="${bold?1:0}">${solid(color)}<a:latin typeface="${font}"/><a:ea typeface="${font}"/><a:cs typeface="${font}"/></a:rPr><a:t>${E(line)}</a:t></a:r><a:endParaRPr lang="ko-KR" sz="${size*100}"/></a:p>`).join('')}</p:txBody></p:sp>`;}
function line(x,y,x2,y2,c=gold,sw=1){let n=id++;let x0=Math.min(x,x2),y0=Math.min(y,y2);s+=`<p:sp><p:nvSpPr><p:cNvPr id="${n}" name="line-${n}"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr><p:spPr><a:xfrm flipV="${(x2-x)*(y2-y)<0?1:0}"><a:off x="${U(x0)}" y="${U(y0)}"/><a:ext cx="${Math.max(1,U(Math.abs(x2-x)))}" cy="${Math.max(1,U(Math.abs(y2-y)))}"/></a:xfrm><a:prstGeom prst="line"><a:avLst/></a:prstGeom><a:noFill/><a:ln w="${U(sw)}">${solid(c)}</a:ln></p:spPr></p:sp>`;}
function start(title,sub=''){s='';id=2;rels=[{id:'rId1',target:'../slideLayouts/slideLayout1.xml',type:'slideLayout'}];text(title,44,38,808,46,32,ink,true);if(sub)text(sub,46,91,866,35,20,muted);line(44,130,916,130,gold,.6);}
function end(){text(`${String(slides.length+1).padStart(2,'0')} / ${slideCount}`,854,32,66,23,12,gold,false,'r');if(slides.length!==5)text('MagicEscape   /   멀티미디어와 가상현실 기말 과제 제안서',44,512,840,18,10,muted);let i=slides.length+1;slides.push(s);put(`ppt/slides/slide${i}.xml`,`<?xml version="1.0" encoding="UTF-8" standalone="yes"?><p:sld xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships" xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main"><p:cSld><p:bg><p:bgPr>${solid(bg)}<a:effectLst/></p:bgPr></p:bg><p:spTree><p:nvGrpSpPr><p:cNvPr id="1" name=""/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr><p:grpSpPr><a:xfrm><a:off x="0" y="0"/><a:ext cx="0" cy="0"/><a:chOff x="0" y="0"/><a:chExt cx="0" cy="0"/></a:xfrm></p:grpSpPr>${s}</p:spTree></p:cSld><p:clrMapOvr><a:masterClrMapping/></p:clrMapOvr></p:sld>`);put(`ppt/slides/_rels/slide${i}.xml.rels`,relationships(rels));}
const relationships=rs=>`<?xml version="1.0" encoding="UTF-8"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">${rs.map(r=>`<Relationship Id="${r.id}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/${r.type||'image'}" Target="${r.target}"/>`).join('')}</Relationships>`;
function row(label,value,y){line(46,y+44,912,y+44,gold,.5);text(label,48,y,165,40,17,gold);text(value,230,y,674,40,18);}
function note(t,y=450){text(t,46,y,867,39,14,muted);}
// Revision 5: editable line illustrations and discovery design.
function arrow(x,y,x2,y2){line(x,y,x2,y2,blue,1.5);if(x2===x){line(x2-4,y2-6,x2,y2,blue,1.5);line(x2+4,y2-6,x2,y2,blue,1.5);}else{line(x2-6,y2-4,x2,y2,blue,1.5);line(x2-6,y2+4,x2,y2,blue,1.5);}}
function band(label,body,y=420){line(44,y,916,y,blue,.8);text(label,48,y+15,145,39,22,blue,true);text(body,208,y+12,704,70,22);}
function rect(x,y,w,h,c=ink){shape(x,y,w,h,'rect','none',c,1.3);}
function dashed(x,y,x2,y2){let n=Math.ceil(Math.hypot(x2-x,y2-y)/9);for(let i=0;i<n;i++){let a=i/n,b=Math.min(1,(i+.52)/n);line(x+(x2-x)*a,y+(y2-y)*a,x+(x2-x)*b,y+(y2-y)*b,blue,1.4);}}
function hidden(x,y,w,h){dashed(x,y,x+w,y);dashed(x+w,y,x+w,y+h);dashed(x+w,y+h,x,y+h);dashed(x,y+h,x,y);}
function crate(x,y,w=70,h=55){rect(x,y,w,h);line(x,y,x+w,y+h,ink,1);line(x+w,y,x,y+h,ink,1);line(x+7,y,x+7,y+h,ink,.7);}
function frame(x,y,w=55,h=65,hide=false){if(hide){hidden(x,y,w,h);hidden(x+6,y+6,w-12,h-12);}else{rect(x,y,w,h);rect(x+6,y+6,w-12,h-12);}}
function cloth(x,y,w=85,h=65){line(x+15,y,x+w-15,y,ink,1.3);line(x+15,y,x,y+h,ink,1.3);line(x+w-15,y,x+w,y+h,ink,1.3);line(x,y+h,x+w,y+h,ink,1.3);line(x+30,y+8,x+23,y+h,ink,.8);line(x+w-28,y+10,x+w-17,y+h,ink,.8);}
function chest(x,y,w=90,h=50){rect(x,y,w,h);line(x,y+15,x+w,y+15,ink,1.3);rect(x+w/2-5,y+10,10,14);line(x+12,y,x+12,y+h,ink,.8);line(x+w-12,y,x+w-12,y+h,ink,.8);}
function shelf(x,y,w=120,h=115){rect(x,y,w,h);for(let i=1;i<3;i++)line(x,y+i*h/3,x+w,y+i*h/3,ink,1);for(let i=0;i<4;i++)rect(x+8+i*w/4,y+8,w/8,h/5);}
function books(x,y){rect(x,y,75,10);rect(x+6,y-12,68,10);rect(x-3,y-24,72,10);}
function pot(x,y,w=95,h=65){shape(x,y,w,h,'ellipse','none',ink,1.5);shape(x-4,y-5,w+8,15,'ellipse','FFFFFF',ink,1.5);line(x+15,y+h-4,x+8,y+h+12,ink,2);line(x+w-15,y+h-4,x+w-8,y+h+12,ink,2);}
function door(x,y,w=150,h=210){rect(x,y,w,h);rect(x+8,y+9,w-16,h-9);shape(x+w-25,y+h*.55,6,6,'ellipse','none',ink,1.2);for(let i=0;i<3;i++)shape(x+24+i*(w-48)/2,y-30,13,13,'ellipse','none',blue,1.5);}
start('');
text('멀티미디어와 가상현실 기말 과제 제안서',48,45,840,32,20,muted);
text('MagicEscape',44,146,650,80,58,ink,true);
text('마법학교 창고 콘셉트의 VR 방탈출',48,244,735,46,28,ink,true);
text('PC VR(OpenXR) / 키보드·마우스 지원 계획',48,360,680,32,22);
text('1인 제작 · 김선우 (202321850)',48,462,680,32,22);
frame(770,330,110,125,true);crate(717,395,120,83);line(690,483,914,483,ink,1);end();
start('기획 배경','물건은 보이는 곳에만 있지 않다. 치우고 열어야 찾는다.');
text('문장으로 전달',48,153,260,35,24,ink,true);rect(70,207,138,90);for(let i=0;i<4;i++)line(85,225+i*15,192,225+i*15,muted,1);text('글을 읽고\n순서를 외운다.',48,323,260,75,22);
text('물건에서 읽기',351,153,260,35,24,ink,true);frame(397,201,83,97);text('액자와 마법서를\n살펴본다.',351,323,260,75,22);
text('가림을 걷어 찾기',650,153,265,35,24,ink,true);frame(746,202,68,90,true);crate(691,250,84,52);text('상자를 밀거나\n궤짝을 연다. (계획)',650,323,265,75,22);
arrow(252,252,319,252);arrow(549,252,616,252);
band('실패 뒤 안내','점멸·연기·거부 연출 뒤 다시 시도한다.\n글 힌트는 P1 3회 / P2·P3 2회 실패 후 나온다.',415);end();
start('작품 개요','헤드셋 유무와 관계없이 같은 퍼즐을 끝까지 푸는 작품을 제안한다.');
text('1인칭 방탈출',48,155,400,38,26,ink,true);text('연금술 실습 창고 하나 · 약 9×7m\n목표 플레이 시간 10~15분',48,211,450,76,24);
text('지원 플랫폼 계획',530,155,380,38,26,ink,true);text('PC VR(OpenXR)\n키보드·마우스',530,211,380,76,24);
line(48,317,912,317,blue,.8);text('퍼즐 3개를 차례로 해결한다.',48,344,540,40,26,ink,true);
for(let i=0;i<3;i++){shape(680+i*65,348,24,24,'ellipse','none',ink,1.3);line(678+i*65,375,709+i*65,344,blue,1.8);}
text('목표: 봉인등 3개를 모두 끄고 출구를 연다.',48,430,864,45,28);end();
start('이야기','실습 재료를 가지러 온 견습생이 연금술 창고의 문을 연다.');
text('문이 닫히고,\n문 위 봉인등 3개가 켜진다.',48,166,480,85,27,ink,true);
text('누군가 창고 곳곳에\n봉인을 푸는 단서를 남겼다.',48,290,480,84,25);
text('상자 사이의 액자와 천 아래 비문판을\n찾아 봉인등을 하나씩 끈다. (계획)',48,421,505,68,22);
door(673,235,158,250);shelf(557,188,82,115);chest(841,420,72,65);line(555,487,916,487,ink,1);end();
start('창고 평면과 숨은 단서','공간 배치 계획 · 약 9×7m · 위에서 본 선화');
const mx=65,my=167,mw=480,mh=304;
line(mx,my,mx+190,my,ink,2);line(mx+288,my,mx+mw,my,ink,2);line(mx,my,mx,my+mh,ink,2);line(mx+mw,my,mx+mw,my+mh,ink,2);line(mx,my+mh,mx+mw,my+mh,ink,2);
line(mx+190,my,mx+190,my+75,ink,1.2);line(mx+190,my+75,mx+260,my,muted,.6);
text('출구 / P3',258,139,170,30,20);hidden(334,182,66,24);cloth(335,179,70,36);
frame(78,203,16,28);frame(78,237,16,28);frame(78,276,16,48,true);crate(97,288,63,41);crate(99,250,46,36);frame(78,354,16,48,true);crate(98,367,58,42);
text('액자 / P1',167,242,150,29,20);line(155,278,176,270,blue,1);
shelf(432,204,93,54);books(445,301);hidden(467,275,29,17);text('벽면 선반',417,331,118,29,20);
pot(344,314,61,45);text('P2',355,382,60,28,20);
shape(245,329,67,52,'ellipse','none',ink,1.4);text('제단',250,394,90,28,20);
chest(173,418,85,35);hidden(199,428,26,14);cloth(446,404,68,48);
text('궤짝',175,474,105,27,20);text('창가',457,474,80,27,20);
text('처음부터 보이는 것',607,165,305,34,23,ink,true);line(608,211,688,211,ink,1.5);text('선반·상자·천·궤짝',607,232,305,33,21);
text('치워야 드러나는 것',607,302,305,35,23,blue,true);dashed(608,347,688,347);text('액자·비문판·재료',607,366,305,33,21);
text('이동은 짧게 잡는다.\n몸을 돌려 벽면을 살피고,\n필요한 구간만 텔레포트한다.',607,420,309,83,20);end();
start('치우기 상호작용','상자 뒤 액자와 천 아래 비문판을 발견하는 동작을 설계한다.');
text('치우기 전',49,146,150,27,20,muted);text('치운 후',265,146,150,27,20,blue);
frame(104,184,55,66,true);crate(65,212,75,46);arrow(177,223,227,223);frame(271,184,55,66);crate(347,225,60,33);
text('상자를 밀면 액자가 드러난다.',48,272,405,29,20);
text('치우기 전',514,146,150,27,20,muted);text('치운 후',729,146,150,27,20,blue);
hidden(550,187,65,60);cloth(539,181,87,76);arrow(650,223,700,223);frame(744,187,65,60);cloth(835,233,60,24);
text('천을 걷으면 비문판이 드러난다.',513,272,404,29,20);
line(44,315,916,315,blue,1);text('물건 분류 (계획)',48,325,223,27,20,ink,true);text('VR 조작',295,325,238,27,20,ink,true);text('키보드·마우스 조작',592,325,323,27,20,ink,true);
const table=[['가벼운 물건','한 손으로 집어 든다.','클릭해 화면 앞에 든다.'],['무거운 물건','잡고 밀기·끌기만 한다.','클릭한 채로 끈다.'],['고정 장치','열기·누르기·끼우기','열기·누르기·끼우기']];
table.forEach((r,i)=>{let y=362+i*35;line(44,y-5,916,y-5,'D5E1F0',.7);text(r[0],48,y,225,28,20);text(r[1],295,y,292,28,20);text(r[2],592,y,324,28,20);});
text('탐색이 곧 상호작용이다. 단서를 찾는 동작과\n퍼즐을 푸는 동작이 같은 손 조작이다.',48,462,865,56,20);end();
start('플레이 흐름','P1의 수정과 P2의 물약·수정을 마지막 봉인에 사용한다.');
const stages=[['입장','창고와 출구를\n살핀다.'],['P1','액자를 찾아\n문양석을 누른다.'],['P2','재료를 찾아\n가마솥에 넣는다.'],['P3','천을 걷고\n비문을 드러낸다.'],['탈출','봉인핵을 누르고\n문 밖으로 나간다.']];
line(80,212,878,212,blue,1.5);stages.forEach((r,i)=>{let x=44+i*178;shape(x+46,193,36,36,'ellipse','FFFFFF',blue,1.4);text(r[0],x,253,168,36,26,ink,true);text(r[1],x,310,172,81,21);});
band('연결 보상','P1: 푸른 수정 / P2: 계시의 물약·붉은 수정\n퍼즐 해결마다 봉인등 1개 소등',420);end();
function puzzle(title,sub,where,clue,action,result,failure,hint){
 start(title,sub);text(where,46,143,866,29,20,blue);
 const cols=[44,345,646];['단서 · 보는 것','행동 · 조작 계획','결과'].forEach((t,i)=>{line(cols[i],187,cols[i]+270,187,blue,1);text(t,cols[i]+8,201,262,36,24,ink,true);});
 text(clue,52,254,260,156,22);text(action,353,254,260,156,22);text(result,654,254,260,156,22);arrow(316,307,339,307);arrow(617,307,640,307);
 line(44,422,916,422,blue,.8);text('틀렸을 때',48,437,141,35,22,blue,true);text(failure,203,434,375,66,21);arrow(582,460,608,460);text(hint,626,434,273,66,21);end();
}
puzzle('P1 · 마법 문양 해독','정답: 대지 → 물 → 불 → 빛','가림 계획: 액자 일부는 쌓인 상자 뒤에 기대어 있다.','액자 번호 I~IV가 순서\n그림 속 원소가 답\n\n칠판에 순서 도식','VR: 문양석을\n손으로 누른다.\n\nPC: 조준 후 클릭','입력마다 액자 점등\n푸른 수정 획득\n봉인등 1 소등\nP2 활성','전체 액자 소등\n입력 순서 초기화','3회 실패 후\n번호 순서 글 힌트');
puzzle('P2 · 마법약 제조','정답: 월광초 꽃잎 → 불도마뱀 비늘 → 수정 가루','가림 계획: 선반의 책더미와 궤짝에 재료 일부를 배치한다.','마법서의 번호 그림\n재료 3개의 투입 순서\n\n독버섯 금지 표시','VR: 재료를 집어\n가마솥에 넣는다.\n\nPC: 들고 조준 후 클릭','계시의 물약과\n붉은 수정 획득\n봉인등 2 소등\nP3 활성','검은 연기·실패음\n투입 재료 원위치 재생성','2회 실패 후\n재료 순서 글 힌트');
puzzle('P3 · 최종 봉인 해제','계시의 물약을 비문판에 사용하면 소켓의 단서가 드러난다.','가림 계획: 출구 옆 비문판은 천 아래에 있다.','천을 걷고 물약 사용\n비문판의 원소 각인\n왼쪽: 물\n오른쪽: 불','VR: 수정을\n소켓에 끼운다.\nPC: 들고 소켓 조준 후\n클릭','왼쪽: 푸른 수정\n오른쪽: 붉은 수정\n봉인핵을 누른다.\n봉인등 3 소등·문 개방','잘못 낀 수정 거부\n수정은 소모하지 않음','2회 실패 후\n수정 대응 글 힌트');
start('조작 설계','한 오브젝트, 두 조작 방식으로 설계한다.');
shape(44,157,248,262,'rect','F2F6FC',blue,1);text('물리 오브젝트 계획',60,177,217,39,23,ink,true);text('잡기 · 놓기\n밀기 · 끌기\n끼우기 · 누르기',60,230,215,104,22);text('헤드셋 연결 시 VR\n없으면 데스크톱',60,343,215,62,20);
line(292,285,320,285,blue,2);line(320,214,320,365,blue,2);arrow(320,214,347,214);arrow(320,365,347,365);
shape(351,155,565,127,'rect','F8FAFD','D5E1F0',1);text('VR 리그 계획',368,166,530,34,24,ink,true);text('컨트롤러로 잡기\n텔레포트 이동 / 스냅 회전',368,209,530,63,22);
shape(351,301,565,138,'rect','F8FAFD','D5E1F0',1);text('데스크톱 리그 계획',368,311,530,34,24,ink,true);text('WASD·마우스로 이동 / 클릭해 화면 앞에 들기\n다시 조준 후 클릭해 놓거나 끼우기',368,356,530,68,21);
text('적용 계획: Unity XR Interaction Toolkit의 Grab / Socket / Poke',44,452,872,29,20);text('개발 적용 계획: XR Device Simulator',44,482,872,28,20);end();
start('기술 구조','리그를 바꿔도 퍼즐 규칙은 그대로 유지하도록 설계한다.');
[['퍼즐 규칙','엔진과 분리된 순수 C# / 정답·진행·상태'],['상호작용 계획','물리 오브젝트: Rigidbody 무게 구분\n잡기·밀기·소켓 / VR·데스크톱 공통'],['표현','판정 결과에 맞춰 조명·소리·연출 재생']].forEach((r,i)=>{let y=153+i*88;shape(44,y,872,70,'rect',i===0?'EDF3FC':'F8FAFD','D5E1F0',1);text(r[0],62,y+18,184,38,23,ink,true);text(r[1],251,y+3,648,65,20);if(i<2)arrow(480,y+71,480,y+85);});
['잠김','활성','해결'].forEach((v,i)=>{let x=194+i*215;shape(x,439,157,53,'rect','FFFFFF',blue,1);text(v,x+10,450,137,35,24,ink,false,'ctr');if(i<2)arrow(x+165,465,x+207,465);});text('퍼즐 상태',44,449,145,36,23,ink,true);end();
// Package
if(slides.length!==slideCount)throw new Error(`Expected ${slideCount} slides, got ${slides.length}`);
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
console.log(`Created ${slideCount} slides with editable native DrawingML shapes.`);

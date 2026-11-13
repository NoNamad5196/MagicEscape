import fs from 'node:fs';
import path from 'node:path';
import {execFileSync} from 'node:child_process';
const root=path.resolve(import.meta.dirname,'../..');
const out=import.meta.dirname, build=path.join(out,'.build');fs.mkdirSync(build,{recursive:true});
const E=s=>String(s).replaceAll('&','&amp;').replaceAll('<','&lt;').replaceAll('>','&gt;').replaceAll('"','&quot;');
const U=v=>Math.round(v*12700), bg='FFFFFF', gold='2864B4', ink='182638', muted='586576', blue='2864B4', red='2864B4';
const slideCount=10;
let parts={},slides=[],s,id,rels;
const put=(p,t)=>parts[p]=t;
const solid=(c,a=100000)=>c==='none'?'<a:noFill/>':`<a:solidFill><a:srgbClr val="${c}"><a:alpha val="${a}"/></a:srgbClr></a:solidFill>`;
function shape(x,y,w,h,geo='rect',fill='none',stroke=gold,sw=.8,alpha=100000){let n=id++;s+=`<p:sp><p:nvSpPr><p:cNvPr id="${n}" name="shape-${n}"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr><p:spPr><a:xfrm><a:off x="${U(x)}" y="${U(y)}"/><a:ext cx="${U(w)}" cy="${U(h)}"/></a:xfrm><a:prstGeom prst="${geo}"><a:avLst/></a:prstGeom>${solid(fill,alpha)}<a:ln w="${U(sw)}">${solid(stroke)}</a:ln></p:spPr></p:sp>`;}
function text(t,x,y,w,h,size=16,color=ink,bold=false,align='l',font=(bold?'Paperlogy 7 Bold':'Pretendard')){let n=id++;s+=`<p:sp><p:nvSpPr><p:cNvPr id="${n}" name="text-${n}"/><p:cNvSpPr txBox="1"/><p:nvPr/></p:nvSpPr><p:spPr><a:xfrm><a:off x="${U(x)}" y="${U(y)}"/><a:ext cx="${U(w)}" cy="${U(h)}"/></a:xfrm><a:prstGeom prst="rect"><a:avLst/></a:prstGeom><a:noFill/><a:ln><a:noFill/></a:ln></p:spPr><p:txBody><a:bodyPr wrap="square" lIns="0" rIns="0" tIns="0" bIns="0" anchor="t"/><a:lstStyle/>${String(t).split('\n').map(line=>`<a:p><a:pPr algn="${align}" marL="0" marR="0"><a:lnSpc><a:spcPct val="118000"/></a:lnSpc><a:spcAft><a:spcPts val="500"/></a:spcAft></a:pPr><a:r><a:rPr lang="ko-KR" sz="${size*100}" b="${bold?1:0}">${solid(color)}<a:latin typeface="${font}"/><a:ea typeface="${font}"/><a:cs typeface="${font}"/></a:rPr><a:t>${E(line)}</a:t></a:r><a:endParaRPr lang="ko-KR" sz="${size*100}"/></a:p>`).join('')}</p:txBody></p:sp>`;}
function line(x,y,x2,y2,c=gold,sw=1){let n=id++;let x0=Math.min(x,x2),y0=Math.min(y,y2);s+=`<p:sp><p:nvSpPr><p:cNvPr id="${n}" name="line-${n}"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr><p:spPr><a:xfrm flipV="${(x2-x)*(y2-y)<0?1:0}"><a:off x="${U(x0)}" y="${U(y0)}"/><a:ext cx="${Math.max(1,U(Math.abs(x2-x)))}" cy="${Math.max(1,U(Math.abs(y2-y)))}"/></a:xfrm><a:prstGeom prst="line"><a:avLst/></a:prstGeom><a:noFill/><a:ln w="${U(sw)}">${solid(c)}</a:ln></p:spPr></p:sp>`;}
function start(title,sub=''){s='';id=2;rels=[{id:'rId1',target:'../slideLayouts/slideLayout1.xml',type:'slideLayout'}];text(title,44,38,808,46,32,ink,true);if(sub)text(sub,46,91,866,35,20,muted);line(44,130,916,130,gold,.6);}
function end(){text(`${String(slides.length+1).padStart(2,'0')} / ${slideCount}`,854,32,66,23,12,gold,false,'r');text('MagicEscape   /   멀티미디어와 가상현실 기말 과제 제안서',44,512,840,18,10,muted);let i=slides.length+1;slides.push(s);put(`ppt/slides/slide${i}.xml`,`<?xml version="1.0" encoding="UTF-8" standalone="yes"?><p:sld xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships" xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main"><p:cSld><p:bg><p:bgPr>${solid(bg)}<a:effectLst/></p:bgPr></p:bg><p:spTree><p:nvGrpSpPr><p:cNvPr id="1" name=""/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr><p:grpSpPr><a:xfrm><a:off x="0" y="0"/><a:ext cx="0" cy="0"/><a:chOff x="0" y="0"/><a:chExt cx="0" cy="0"/></a:xfrm></p:grpSpPr>${s}</p:spTree></p:cSld><p:clrMapOvr><a:masterClrMapping/></p:clrMapOvr></p:sld>`);put(`ppt/slides/_rels/slide${i}.xml.rels`,relationships(rels));}
const relationships=rs=>`<?xml version="1.0" encoding="UTF-8"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">${rs.map(r=>`<Relationship Id="${r.id}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/${r.type||'image'}" Target="${r.target}"/>`).join('')}</Relationships>`;
function row(label,value,y){line(46,y+44,912,y+44,gold,.5);text(label,48,y,165,40,17,gold);text(value,230,y,674,40,18);}
function note(t,y=450){text(t,46,y,867,39,14,muted);}
// Revision 4: proposed play experience and interaction design.
function box(title,body,x,y,w,h=180){shape(x,y,w,h,'rect','F2F6FC','D5E1F0',1);text(title,x+18,y+17,w-36,37,24,ink,true);text(body,x+18,y+65,w-36,h-72,22);}
function arrow(x,y,x2,y2){line(x,y,x2,y2,blue,2);if(x2>x){line(x2-7,y2-5,x2,y2,blue,2);line(x2-7,y2+5,x2,y2,blue,2);}else if(x2<x){line(x2+7,y2-5,x2,y2,blue,2);line(x2+7,y2+5,x2,y2,blue,2);}else{let d=y2>y?-7:7;line(x2-5,y2+d,x2,y2,blue,2);line(x2+5,y2+d,x2,y2,blue,2);}}
function band(label,body,y=420){shape(44,y,872,80,'rect','F2F6FC','none');text(label,60,y+14,145,42,22,blue,true);text(body,210,y+10,685,65,22);}
start('');
text('멀티미디어와 가상현실 기말 과제 제안서',48,45,840,32,20,muted);
text('MagicEscape',44,126,870,80,58,ink,true);
text('마법학교 연금술 교실 콘셉트의 VR 방탈출',48,220,866,46,28,ink,true);
['교실 탐색','퍼즐 3개','출구 개방'].forEach((v,i)=>{box(v,'',48+i*302,310,260,76);if(i<2)arrow(310+i*302,348,341+i*302,348);});
text('PC VR(OpenXR) / 키보드·마우스 지원 계획',48,410,860,32,22);
text('이름 / 학번',48,465,830,32,22);end();
start('기획 배경','쪽지와 문장에 치우친 단서를 교실 안 물건으로 옮긴다.');
box('글로 설명하는 방식','쪽지·문장으로 단서 전달\n\n글을 읽고 순서를 외운다.',44,156,405,228);
box('이 작품의 방식','액자·칠판·가마솥·비문판\n\n물건을 살펴보고 조작한다.',511,156,405,228);
arrow(459,270,501,270);band('글 힌트','여러 번 틀렸을 때만 표시한다.\nP1 3회 / P2·P3 2회 실패 기준');end();
start('작품 개요','헤드셋 유무와 관계없이 같은 퍼즐을 끝까지 푸는 작품을 제안한다.');
box('1인칭 방탈출','교실 하나 12×10m\n목표 플레이 시간 10~15분',44,155,410,166);
box('지원 플랫폼 계획','PC VR(OpenXR)\n키보드·마우스',506,155,410,166);
text('퍼즐 해결',50,357,180,38,26,ink,true);arrow(225,376,305,376);
[0,1,2].forEach(i=>{shape(340+i*142,350,110,56,'rect','F2F6FC',blue,1);text('봉인등 '+(i+1),345+i*142,365,100,32,22,ink,false,'ctr');});arrow(752,376,800,376);text('탈출',818,357,100,38,26,ink,true);
text('목표: 문 위 봉인등 3개를 켜고 출구를 연다.',50,443,858,42,26);end();
start('이야기와 공간','원소 실습이 끝난 뒤, 견습 마법사 혼자 교실에 남았다.');
shape(44,150,590,340,'rect','F8FAFD',blue,1.3);
function loc(t,x,y,w=170){shape(x,y,w,48,'rect','FFFFFF','D5E1F0',1);text(t,x+8,y+10,w-16,32,21,ink,false,'ctr');}
loc('출구·비문판 P3',240,164,205);loc('독버섯',58,164,132);loc('마법서',477,164,143);
loc('액자 4점',58,239,151);loc('가마솥 P2',452,239,168);loc('칠판',58,307,151);loc('제단 P1',255,307,156);loc('책장·비늘',452,307,168);
loc('교탁·가루',58,427,151);loc('입장',269,427,132);loc('창가·꽃잎',452,427,168);
arrow(335,418,335,363);line(411,329,433,329,blue,2);line(433,329,433,264,blue,2);arrow(433,264,449,264);arrow(536,230,536,222);arrow(463,188,451,188);
text('단서를 따라\n봉인을 푼다.',668,165,247,80,25,ink,true);
text('교수가 남긴 물건에\n봉인을 푸는 규칙이\n숨어 있다.',668,277,247,108,22);
text('12×10m 교실\n위에서 본 배치',668,409,247,83,22,muted);end();
start('플레이 흐름','P1의 수정과 P2의 물약·수정을 마지막 봉인에 사용한다.');
const stages=[['입장','교실과 출구를\n살핀다.'],['P1','액자 번호대로\n문양석을 누른다.'],['P2','재료를 모아\n가마솥에 넣는다.'],['P3','비문을 드러내고\n수정을 끼운다.'],['탈출','봉인핵을 누르고\n문 밖으로 나간다.']];
stages.forEach((r,i)=>{let x=44+i*178;shape(x,170,160,76,'rect','F2F6FC',blue,1);text(r[0],x+10,188,140,40,28,ink,true,'ctr');text(r[1],x,277,171,100,21);if(i<4)arrow(x+161,208,x+175,208);});
line(220,397,750,397,blue,1.5);line(220,380,220,397,blue,1.5);line(750,380,750,397,blue,1.5);band('연결 보상','P1: 푸른 수정 / P2: 계시의 물약·붉은 수정');end();
function puzzle(title,sub,clue,action,result,failure,hint){
 start(title,sub);box('단서 · 보는 것',clue,44,154,270,231);box('행동 · 조작 계획',action,345,154,270,231);box('결과',result,646,154,270,231);arrow(317,265,341,265);arrow(618,265,642,265);
 line(480,385,480,406,blue,1.5);line(480,406,70,406,blue,1.5);line(70,406,70,424,blue,1.5);
 shape(44,422,872,80,'rect','F2F6FC','none');text('틀렸을 때',60,435,136,35,22,blue,true);text(failure,203,432,375,65,21);arrow(582,460,608,460);text(hint,626,432,273,65,21);end();
}
puzzle('P1 · 마법 문양 해독','정답: 대지 → 물 → 불 → 빛','액자 번호 I~IV가 순서\n그림 속 원소가 답\n\n칠판에 순서 도식','VR: 문양석을\n손으로 누른다.\n\nPC: 조준 후 클릭','입력마다 액자 점등\n\n푸른 수정 획득\nP2 활성','전체 액자 소등\n입력 순서 초기화','3회 실패 후\n번호 순서 글 힌트');
puzzle('P2 · 마법약 제조','정답: 월광초 꽃잎 → 불도마뱀 비늘 → 수정 가루','마법서의 번호 그림\n재료 3개의 투입 순서\n\n독버섯 금지 표시','VR: 재료를 집어\n가마솥에 넣는다.\n\nPC: 들고 조준 후 클릭','계시의 물약과\n붉은 수정 획득\n\nP3 활성','검은 연기·실패음\n투입 재료 원위치 재생성','2회 실패 후\n재료 순서 글 힌트');
puzzle('P3 · 최종 봉인 해제','계시의 물약을 비문판에 사용하면 소켓의 단서가 드러난다.','비문판의 원소 각인\n\n왼쪽: 물\n오른쪽: 불','VR: 수정을\n소켓에 끼운다.\nPC: 들고 소켓 조준 후\n클릭','왼쪽: 푸른 수정\n오른쪽: 붉은 수정\n봉인핵을 누른다.\n출구 개방','잘못 낀 수정 거부\n수정은 소모하지 않음','2회 실패 후\n수정 대응 글 힌트');
start('조작 설계','한 오브젝트, 두 조작 방식으로 설계한다.');
shape(44,157,248,262,'rect','F2F6FC',blue,1);text('공통 물리 오브젝트',60,177,217,39,24,ink,true);text('잡기 · 놓기\n끼우기 · 누르기',60,236,215,76,24);text('헤드셋 연결 시 VR\n없으면 데스크톱',60,343,215,62,20);
line(292,285,320,285,blue,2);line(320,214,320,365,blue,2);arrow(320,214,347,214);arrow(320,365,347,365);
shape(351,155,565,127,'rect','F8FAFD','D5E1F0',1);text('VR 리그 계획',368,166,530,34,24,ink,true);text('컨트롤러로 잡기\n텔레포트 이동 / 스냅 회전',368,209,530,63,22);
shape(351,301,565,138,'rect','F8FAFD','D5E1F0',1);text('데스크톱 리그 계획',368,311,530,34,24,ink,true);text('WASD·마우스로 이동 / 클릭해 화면 앞에 들기\n다시 조준 후 클릭해 놓거나 끼우기',368,356,530,68,21);
text('적용 계획: Unity XR Interaction Toolkit의 Grab / Socket / Poke',44,452,872,29,20);text('헤드셋 없는 개발: XR Device Simulator',44,482,872,28,20);end();
start('기술 구조','리그를 바꿔도 퍼즐 규칙은 그대로 유지하도록 설계한다.');
[['퍼즐 규칙','엔진과 분리된 순수 C# / 정답·진행·상태'],['상호작용','VR·데스크톱 두 리그가 같은 물건을 조작'],['표현','판정 결과에 맞춰 조명·소리·연출 재생']].forEach((r,i)=>{let y=153+i*88;shape(44,y,872,70,'rect',i===0?'EDF3FC':'F8FAFD','D5E1F0',1);text(r[0],62,y+18,175,38,25,ink,true);text(r[1],251,y+21,648,36,23);if(i<2)arrow(480,y+71,480,y+85);});
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

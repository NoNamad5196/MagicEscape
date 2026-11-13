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
function text(t,x,y,w,h,size=16,color=ink,bold=false,align='l',font=(bold?'Paperlogy 7 Bold':'Pretendard'),anchor='t'){let n=id++;s+=`<p:sp><p:nvSpPr><p:cNvPr id="${n}" name="text-${n}"/><p:cNvSpPr txBox="1"/><p:nvPr/></p:nvSpPr><p:spPr><a:xfrm><a:off x="${U(x)}" y="${U(y)}"/><a:ext cx="${U(w)}" cy="${U(h)}"/></a:xfrm><a:prstGeom prst="rect"><a:avLst/></a:prstGeom><a:noFill/><a:ln><a:noFill/></a:ln></p:spPr><p:txBody><a:bodyPr wrap="square" lIns="0" rIns="0" tIns="0" bIns="0" anchor="${anchor}"/><a:lstStyle/>${String(t).split('\n').map(line=>`<a:p><a:pPr algn="${align}" marL="0" marR="0"><a:lnSpc><a:spcPct val="118000"/></a:lnSpc><a:spcAft><a:spcPts val="500"/></a:spcAft></a:pPr><a:r><a:rPr lang="ko-KR" sz="${size*100}" b="${bold?1:0}">${solid(color)}<a:latin typeface="${font}"/><a:ea typeface="${font}"/><a:cs typeface="${font}"/></a:rPr><a:t>${E(line)}</a:t></a:r><a:endParaRPr lang="ko-KR" sz="${size*100}"/></a:p>`).join('')}</p:txBody></p:sp>`;}
function line(x,y,x2,y2,c=gold,sw=1){let n=id++;let x0=Math.min(x,x2),y0=Math.min(y,y2);s+=`<p:sp><p:nvSpPr><p:cNvPr id="${n}" name="line-${n}"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr><p:spPr><a:xfrm flipV="${(x2-x)*(y2-y)<0?1:0}"><a:off x="${U(x0)}" y="${U(y0)}"/><a:ext cx="${Math.max(1,U(Math.abs(x2-x)))}" cy="${Math.max(1,U(Math.abs(y2-y)))}"/></a:xfrm><a:prstGeom prst="line"><a:avLst/></a:prstGeom><a:noFill/><a:ln w="${U(sw)}">${solid(c)}</a:ln></p:spPr></p:sp>`;}
function start(title,sub=''){s='';id=2;rels=[{id:'rId1',target:'../slideLayouts/slideLayout1.xml',type:'slideLayout'}];text(title,44,38,808,46,32,ink,true);if(sub)text(sub,46,91,866,35,20,muted);line(44,130,916,130,gold,.6);}
function end(){text(`${String(slides.length+1).padStart(2,'0')} / ${slideCount}`,854,32,66,23,12,gold,false,'r');if(slides.length!==5)text('MagicEscape   /   멀티미디어와 가상현실 기말 과제 제안서',44,512,840,18,10,muted);let i=slides.length+1;slides.push(s);put(`ppt/slides/slide${i}.xml`,`<?xml version="1.0" encoding="UTF-8" standalone="yes"?><p:sld xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships" xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main"><p:cSld><p:bg><p:bgPr>${solid(bg)}<a:effectLst/></p:bgPr></p:bg><p:spTree><p:nvGrpSpPr><p:cNvPr id="1" name=""/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr><p:grpSpPr><a:xfrm><a:off x="0" y="0"/><a:ext cx="0" cy="0"/><a:chOff x="0" y="0"/><a:chExt cx="0" cy="0"/></a:xfrm></p:grpSpPr>${s}</p:spTree></p:cSld><p:clrMapOvr><a:masterClrMapping/></p:clrMapOvr></p:sld>`);put(`ppt/slides/_rels/slide${i}.xml.rels`,relationships(rels));}
const relationships=rs=>`<?xml version="1.0" encoding="UTF-8"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">${rs.map(r=>`<Relationship Id="${r.id}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/${r.type||'image'}" Target="${r.target}"/>`).join('')}</Relationships>`;
// GPT-generated PNG source assets, with editable labels kept outside the art.
function picture(name,x,y,w,h,fit='contain'){
 const data=fs.readFileSync(path.join(out,'art',name+'.png'));
 const ratio=data.readUInt32BE(16)/data.readUInt32BE(20);let crop='';
 if(fit==='contain'){let ww=Math.min(w,h*ratio),hh=ww/ratio;x+=(w-ww)/2;y+=(h-hh)/2;w=ww;h=hh;}
 else if(ratio>w/h){let c=Math.round((1-(w/h)/ratio)*50000);crop=`<a:srcRect l="${c}" r="${c}"/>`;}
 else {let c=Math.round((1-ratio/(w/h))*50000);crop=`<a:srcRect t="${c}" b="${c}"/>`;}
 const rid='rId'+(rels.length+1),n=id++;
 rels.push({id:rid,target:'../media/'+name+'.png',type:'image'});put('ppt/media/'+name+'.png',data);
 s+=`<p:pic><p:nvPicPr><p:cNvPr id="${n}" name="${E(name)}" descr="GPT-generated sepia ink and pale watercolor concept art"/><p:cNvPicPr><a:picLocks noChangeAspect="1"/></p:cNvPicPr><p:nvPr/></p:nvPicPr><p:blipFill><a:blip r:embed="${rid}"/>${crop}<a:stretch><a:fillRect/></a:stretch></p:blipFill><p:spPr><a:xfrm><a:off x="${U(x)}" y="${U(y)}"/><a:ext cx="${U(w)}" cy="${U(h)}"/></a:xfrm><a:prstGeom prst="rect"><a:avLst/></a:prstGeom></p:spPr></p:pic>`;
}
function row(label,value,y){line(46,y+44,912,y+44,gold,.5);text(label,48,y,165,40,17,gold);text(value,230,y,674,40,18);}
function note(t,y=450){text(t,46,y,867,39,14,muted);}
// Revision 6: generated concept art, native text and diagram annotations.
function arrow(x,y,x2,y2){line(x,y,x2,y2,blue,1.5);if(x2===x){line(x2-4,y2-6,x2,y2,blue,1.5);line(x2+4,y2-6,x2,y2,blue,1.5);}else{line(x2-6,y2-4,x2,y2,blue,1.5);line(x2-6,y2+4,x2,y2,blue,1.5);}}
function band(label,body,y=420){line(44,y,916,y,blue,.8);text(label,48,y+15,145,39,22,blue,true);text(body,208,y+12,704,70,22);}
function rect(x,y,w,h,c=ink){shape(x,y,w,h,'rect','none',c,1.3);}
function dashed(x,y,x2,y2){let n=Math.ceil(Math.hypot(x2-x,y2-y)/9);for(let i=0;i<n;i++){let a=i/n,b=Math.min(1,(i+.52)/n);line(x+(x2-x)*a,y+(y2-y)*a,x+(x2-x)*b,y+(y2-y)*b,blue,1.4);}}
function hidden(x,y,w,h){dashed(x,y,x+w,y);dashed(x+w,y,x+w,y+h);dashed(x+w,y+h,x,y+h);dashed(x,y+h,x,y);}
start('');
picture('cover-warehouse',500,60,416,434,'cover');
text('멀티미디어와 가상현실',48,46,430,30,20,muted);
text('기말 과제 제안서',48,78,430,30,20,muted);
text('MagicEscape',44,166,450,68,48,ink,true);
text('마법학교 창고 콘셉트의\nVR 방탈출',48,256,430,84,28,ink,true);
text('PC VR(OpenXR)\n키보드·마우스 지원',48,370,430,65,22);
text('1인 제작 · 김선우 (202321850)',48,464,440,32,22);end();
start('기획 배경','물건은 보이는 곳에만 있지 않다. 치우고 열어야 찾는다.');
text('문장으로 전달',48,153,260,35,24,ink,true);picture('background-note',48,195,260,118,'cover');text('글을 읽고\n순서를 외운다.',48,323,260,75,22);
text('물건에서 읽기',351,153,260,35,24,ink,true);picture('background-frame-book',351,195,260,118,'cover');text('액자와 마법서를\n살펴본다.',351,323,260,75,22);
text('가림을 걷어 찾기',650,153,265,35,24,ink,true);picture('background-discovery',650,195,265,118,'cover');text('상자를 밀거나\n궤짝을 연다.',650,323,265,75,22);

band('실패 뒤 안내','점멸·연기·거부 연출 뒤 다시 시도한다.\n글 힌트는 P1 3회 / P2·P3 2회 실패 후 나온다.',415);end();
start('작품 개요','헤드셋 유무와 관계없이 같은 퍼즐을 끝까지 푸는 작품을 제안한다.');
text('1인칭 방탈출',48,155,400,38,26,ink,true);text('연금술 실습 창고 하나 · 약 9×7m\n목표 플레이 시간 10~15분',48,211,450,76,24);
text('지원 플랫폼',530,155,380,38,26,ink,true);text('PC VR(OpenXR)\n키보드·마우스',530,211,380,76,24);
line(48,317,912,317,blue,.8);text('퍼즐 3개를 차례로 해결한다.',48,344,540,40,26,ink,true);
picture('seal-lamps',678,328,218,97);
text('목표: 봉인등 3개를 모두 끄고 출구를 연다.',48,430,864,45,28);end();
start('이야기','실습 재료를 가지러 온 견습생이 연금술 창고의 문을 연다.');
text('문이 닫히고,\n문 위 봉인등 3개가 켜진다.',48,166,480,85,27,ink,true);
text('누군가 창고 곳곳에\n봉인을 푸는 단서를 남겼다.',48,290,480,84,25);
text('상자 사이의 액자와 천 아래 비문판을\n찾아 봉인등을 하나씩 끈다.',48,421,505,68,22);
picture('story-door',550,154,366,335,'cover');end();
start('창고 평면과 숨은 단서','공간 배치 · 약 9×7m · 아이소메트릭 개념도');
picture('warehouse-isometric',44,155,574,343);
function mapLabel(t,x,y,w){shape(x-3,y-1,w+6,28,'rect','FFFFFF','none',0,92000);text(t,x,y,w,28,20,ink,true);}
mapLabel('출구 / P3',260,274,143);mapLabel('액자 / P1',51,259,129);
mapLabel('가마솥 / P2',386,329,155);mapLabel('제단',215,381,65);
mapLabel('궤짝',70,457,65);mapLabel('창가',521,458,65);
hidden(340,184,73,74);hidden(82,333,57,58);
text('처음부터 보이는 것',647,165,269,34,23,ink,true);line(648,211,728,211,ink,1.5);text('선반·상자·천·궤짝',647,232,269,33,21);
text('치워야 드러나는 것',647,302,269,35,23,blue,true);dashed(648,347,728,347);text('액자·비문판·재료',647,366,269,33,21);
text('벽면을 따라 살피며\n짧게 텔레포트한다.',647,426,269,65,20);end();
start('치우기 상호작용','상자 뒤 액자와 천 아래 비문판을 발견하는 동작을 설계한다.');
text('치우기 전',49,146,150,27,20,muted);text('치운 후',265,146,150,27,20,blue);
picture('clearing-crates-before',48,176,192,94);arrow(243,222,260,222);picture('clearing-crates-after',265,176,192,94);
text('상자를 밀면 액자가 드러난다.',48,272,405,29,20);
text('치우기 전',514,146,150,27,20,muted);text('치운 후',729,146,150,27,20,blue);
picture('clearing-cloth-before',513,176,192,94);arrow(708,222,725,222);picture('clearing-cloth-after',730,176,186,94);
text('천을 걷으면 비문판이 드러난다.',513,272,404,29,20);
line(44,315,916,315,blue,1);text('물건 분류',48,325,223,27,20,ink,true);text('VR 조작',295,325,238,27,20,ink,true);text('키보드·마우스 조작',592,325,323,27,20,ink,true);
const table=[['가벼운 물건','한 손으로 집어 든다.','클릭해 화면 앞에 든다.'],['무거운 물건','잡고 밀기·끌기만 한다.','클릭한 채로 끈다.'],['고정 장치','열기·누르기·끼우기','열기·누르기·끼우기']];
table.forEach((r,i)=>{let y=357+i*32;line(44,y-5,916,y-5,'D5E1F0',.7);text(r[0],48,y,225,28,20);text(r[1],295,y,292,28,20);text(r[2],592,y,324,28,20);});
text('단서를 찾을 때도 물건을 잡고 옮긴다.\n퍼즐을 풀 때와 같은 손 조작을 사용한다.',48,463,865,56,20);end();
start('플레이 흐름','P1의 수정과 P2의 물약·수정을 마지막 봉인에 사용한다.');
const stages=[['입장','창고와 출구를\n살핀다.'],['P1','액자를 찾아\n문양석을 누른다.'],['P2','재료를 찾아\n가마솥에 넣는다.'],['P3','천을 걷고\n비문을 드러낸다.'],['탈출','봉인핵을 누르고\n문 밖으로 나간다.']];
line(80,212,878,212,blue,1.5);stages.forEach((r,i)=>{let x=44+i*178;shape(x+46,193,36,36,'ellipse','FFFFFF',blue,1.4);text(r[0],x,253,168,36,26,ink,true);text(r[1],x,310,172,81,21);});
band('연결 보상','P1: 푸른 수정 / P2: 계시의 물약·붉은 수정\n퍼즐 해결마다 봉인등 1개 소등',420);end();
function puzzle(title,sub,where,clue,action,result,failure,hint){
 start(title,sub);text(where,46,143,866,29,20,blue);
 const cols=[44,345,646];['단서 · 보는 것','행동 · 조작','결과'].forEach((t,i)=>{line(cols[i],187,cols[i]+270,187,blue,1);text(t,cols[i]+8,201,262,36,24,ink,true);});
 text(clue,52,254,260,156,22);text(action,353,254,260,156,22);text(result,654,254,260,156,22);arrow(316,307,339,307);arrow(617,307,640,307);
 line(44,422,916,422,blue,.8);text('틀렸을 때',48,437,141,35,22,blue,true);text(failure,203,434,375,66,21);arrow(582,460,608,460);text(hint,626,434,273,66,21);end();
}
puzzle('P1 · 마법 문양 해독','정답: 대지 → 물 → 불 → 빛','숨긴 위치: 액자 일부는 쌓인 상자 뒤에 기대어 있다.','액자 번호 I~IV가 순서\n그림 속 원소가 답\n\n칠판에 순서 도식','VR: 문양석을\n손으로 누른다.\n\nPC: 조준 후 클릭','입력마다 액자 점등\n푸른 수정 획득\n봉인등 1 소등\nP2 활성','전체 액자 소등\n입력 순서 초기화','3회 실패 후\n번호 순서 글 힌트');
puzzle('P2 · 마법약 제조','정답: 월광초 꽃잎 → 불도마뱀 비늘 → 수정 가루','숨긴 위치: 선반의 책더미와 궤짝에 재료 일부를 배치한다.','마법서의 번호 그림\n재료 3개의 투입 순서\n\n독버섯 금지 표시','VR: 재료를 집어\n가마솥에 넣는다.\n\nPC: 들고 조준 후 클릭','계시의 물약과\n붉은 수정 획득\n봉인등 2 소등\nP3 활성','검은 연기·실패음\n투입 재료 원위치 재생성','2회 실패 후\n재료 순서 글 힌트');
puzzle('P3 · 최종 봉인 해제','계시의 물약을 비문판에 사용하면 소켓의 단서가 드러난다.','숨긴 위치: 출구 옆 비문판은 천 아래에 있다.','천을 걷고 물약 사용\n비문판의 원소 각인\n왼쪽: 물\n오른쪽: 불','VR: 수정을\n소켓에 끼운다.\nPC: 들고 소켓 조준 후\n클릭','왼쪽: 푸른 수정\n오른쪽: 붉은 수정\n봉인핵을 누른다.\n봉인등 3 소등·문 개방','잘못 낀 수정 거부\n수정은 소모하지 않음','2회 실패 후\n수정 대응 글 힌트');
start('조작 설계','한 오브젝트를 VR과 데스크톱에서 같은 규칙으로 다룬다.');
text('VR 리그',48,149,410,34,24,ink,true);text('데스크톱 리그',503,149,410,34,24,ink,true);
picture('interaction-vr',48,191,409,177,'cover');picture('interaction-desktop',503,191,409,177,'cover');
text('컨트롤러로 잡기\n텔레포트 이동 / 스냅 회전',48,380,409,64,22);
text('WASD·마우스로 이동\n클릭해 화면 앞에 들기',503,380,409,64,22);
text('헤드셋 연결 시 VR / 없으면 데스크톱',48,450,864,28,20);
text('XR Interaction Toolkit: Grab / Socket / Poke · XR Device Simulator',48,478,864,28,20);
end();
start('기술 구조','리그를 바꿔도 퍼즐 규칙은 그대로 유지하도록 설계한다.');
[['퍼즐 규칙','엔진과 분리된 순수 C# / 정답·진행·상태'],['상호작용','물리 오브젝트: Rigidbody 무게 구분\n잡기·밀기·소켓 / VR·데스크톱 공통'],['표현','판정 결과에 맞춰 조명·소리·연출 재생']].forEach((r,i)=>{let y=153+i*88;shape(44,y,872,70,'rect',i===0?'EDF3FC':'F8FAFD','D5E1F0',1);text(r[0],62,y+5,184,60,23,ink,true,'l','Paperlogy 7 Bold','ctr');text(r[1],251,y+5,648,60,20,ink,false,'l','Pretendard','ctr');if(i<2)arrow(480,y+71,480,y+85);});
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
console.log(`Created ${slideCount} slides with embedded GPT concept art and editable labels.`);

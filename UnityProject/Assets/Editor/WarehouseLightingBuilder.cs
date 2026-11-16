using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;
namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        static void ConfigureWarehouseDarkening()
        {
            var p=SceneComponent<RoomPresentation>();
            p.progressiveDarkening=true;
            foreach(var rect in new[]{p.fade.rectTransform,p.clearGroup.GetComponent<RectTransform>()})
            {rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=Vector2.zero;rect.offsetMax=Vector2.zero;}
            var d=p.gameObject.AddComponent<WarehouseDarkening>();
            const string path="Assets/Settings/WarehouseLighting.asset";
            d.profile=AssetDatabase.LoadAssetAtPath<WarehouseLightingProfile>(path);
            if(d.profile==null){d.profile=ScriptableObject.CreateInstance<WarehouseLightingProfile>();AssetDatabase.CreateAsset(d.profile,path);}
            EditorUtility.SetDirty(d.profile);
            d.brewLiquid=Find("Liquid").GetComponent<Renderer>();
            var lights=new List<Light>(p.candles);
            var flames=new List<Renderer>(p.candleFlames.Take(p.candles.Length));
            foreach(var f in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None).Where(r=>r.name=="SconceFlame").OrderBy(r=>r.transform.position.z))
            {
                var l=new GameObject("WarehouseSconceLight").AddComponent<Light>();
                l.transform.SetParent(f.transform.parent,true);l.transform.position=f.transform.position;
                l.intensity=.45f;l.range=1.7f;l.color=new Color(1,.5f,.18f);l.shadows=LightShadows.None;
                lights.Add(l);flames.Add(f);
            }
            d.wickLights=lights.ToArray();d.flames=flames.ToArray();d.extinguishAt=new int[lights.Count];d.smoke=new ParticleSystem[lights.Count];
            // West/solved altar first; east/brew second; north/exit last. One guide candle survives.
            for(int i=0;i<lights.Count;i++)
            {
                var pos=flames[i].transform.position;
                d.extinguishAt[i]=pos.x<0?1:(pos.z<1?2:3);
                if(i==lights.Count-1)d.extinguishAt[i]=4;
                foreach(var motion in flames[i].GetComponents<ElementMotion>())motion.enabled=false;
                var ps=EParticles("WickSmoke_"+i,p.transform,pos,new Color(.3f,.32f,.35f,.22f),"Soft",0,.025f,.018f,.12f);
                var main=ps.main;main.loop=false;main.playOnAwake=false;main.startLifetime=1.2f;main.maxParticles=5;
                var emission=ps.emission;emission.SetBursts(new[]{new ParticleSystem.Burst(0,4)});
                ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);d.smoke[i]=ps;
            }
            var locals=new List<Light>();
            foreach(var pair in new[]{
                ("Cauldron",new Vector3(2.65f,1.05f,.8f)),("Moonpetal",new Vector3(3.75f,1.1f,-2.94f)),
                ("Scale",new Vector3(3.9f,1.30f,1.05f)),("Chest",new Vector3(-3.25f,.7f,-2.3f)),
                ("Inscription",new Vector3(2.1f,1.7f,2.4f)),("Exit",new Vector3(0,1.5f,3.6f))})
            {
                var light=new GameObject("Guide_"+pair.Item1).AddComponent<Light>();
                light.transform.SetParent(p.transform,true);light.transform.position=pair.Item1=="Cauldron"?Find("Liquid").transform.position+Vector3.up*.18f:
                    pair.Item1=="Inscription"?new Vector3(2.1f,1.25f,2.25f):pair.Item2;
                light.intensity=d.profile.localIntensity;light.range=d.profile.localRange;
                light.color=d.profile.localColor;light.shadows=LightShadows.None;locals.Add(light);
            }
            d.localLights=locals.ToArray();
            d.backgroundLights=Object.FindObjectsByType<Light>(FindObjectsSortMode.None)
                .Where(l=>l!=p.dawn && !lights.Contains(l) && !locals.Contains(l) && l!=p.roomFill &&
                    !p.sealLamps.Any(s=>l.transform.IsChildOf(s.transform))).ToArray();
            foreach(var flicker in Object.FindObjectsByType<FireFlicker>(FindObjectsSortMode.None))
                if(d.backgroundLights.Contains(flicker.source))flicker.enabled=false;
        }
    }
}

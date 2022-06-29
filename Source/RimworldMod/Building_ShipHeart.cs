using HarmonyLib;
using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimworldMod;


namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class Building_ShipHeart : Building_ShipBridge
    {
        public ShipBody body = null;
        public String heartId = "NA";
        public bool mutationsDone = false;
        private int aggressionLevel = 1;

        public List<IMutation> mutations = new List<IMutation>() {
        };

        public Dictionary<string, int> mutationThemes = new Dictionary<string, int>()
        {
            {"flesh", 3},
            {"bone", 3},
            {"humors", 3},
            {"misc", 2}
        };
        public Dictionary<string, Dictionary<string, List<IMutation>>> goodMutationOptions = new Dictionary<string, Dictionary<string, List<IMutation>>>()
        {
            {"offense", new Dictionary<string, List<IMutation>>(){
                { "flesh", new List<IMutation>(){

                }},
                { "bone", new List<IMutation>(){
                    new DenseSpines(), new EfficientSpines(),
                }},
                { "humors", new List<IMutation>(){

                }},
                { "misc", new List<IMutation>(){

                }}
            }},
            {"defense", new Dictionary<string, List<IMutation>>(){
                { "flesh", new List<IMutation>(){

                }},
                { "bone", new List<IMutation>(){

                }},
                { "humors", new List<IMutation>(){

                }},
                { "misc", new List<IMutation>(){

                }}
            }},
            {"utility", new Dictionary<string, List<IMutation>>(){
                { "flesh", new List<IMutation>(){

                }},
                { "bone", new List<IMutation>(){

                }},
                { "humors", new List<IMutation>(){

                }},
                { "misc", new List<IMutation>(){

                }}
            }}
        };


        public Dictionary<string, float> statMultipliers = new Dictionary<string, float>();
        public Dictionary<ThingDef, Dictionary<string, float>> specStatMultipliers = new Dictionary<ThingDef, Dictionary<string, float>>();
        public Dictionary<string, List<ThingDef>> organOptions = new Dictionary<string, List<ThingDef>>()
        {
            {"smallTurretOptions", new List<ThingDef>(){
                ThingDef.Named("ShipTurret_Nematocyst")
            }},
            {"mediumTurretOptions", new List<ThingDef>(){
                ThingDef.Named("ShipTurret_BioPlasma"), ThingDef.Named("ShipTurret_BioAcid"),
                ThingDef.Named("ShipTurret_BioPlasma"), ThingDef.Named("ShipTurret_BioAcid"),
            }},
            {"largeTurretOptions", new List<ThingDef>(){
                ThingDef.Named("HeavySpineLauncher"), ThingDef.Named("HeavySpineLauncher"),
                ThingDef.Named("LightSpineLauncher"), ThingDef.Named("LightSpineLauncher"),
            }},
            {"spinalTurretOptions", new List<ThingDef>(){}},
            {"smallMawOptions", new List<ThingDef>(){
                ThingDef.Named("Maw_Small"), ThingDef.Named("Maw_Small"),
            }},
        };
        public Dictionary<ThingDef, List<ThingDef>> defOptions = new Dictionary<ThingDef, List<ThingDef>>()
        {
            {ThingDef.Named("HeavySpineLauncher"), new List<ThingDef>(){
                ThingDef.Named("Spine_Heavy")
            }},
            {ThingDef.Named("LightSpineLauncher"), new List<ThingDef>(){
                ThingDef.Named("Spine_Light")
            }},
        };
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                heartId = Guid.NewGuid().ToString();
                Scribe_Values.Look<String>(ref heartId, "heartId", "NA");
            }
            foreach (IMutation mutation in mutations)
            {
                mutation.Apply(this);
            }
            ((ShipBodyMapComp)Map.components.Where(t => t is ShipBodyMapComp).FirstOrDefault()).Register(this);
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<String>(ref heartId, "heartId", "NA");
        }

        public float getStatMultiplier(string stat, ThingDef src)
        {
            if (src != null)
            {
                return statMultipliers.TryGetValue(stat, 1f) * specStatMultipliers.TryGetValue(src, new Dictionary<string, float>()).TryGetValue(stat, 1f);
            }
            return statMultipliers.TryGetValue(stat, 1f);
        }

        public string GetRandomTheme(Dictionary<string, int> themeOdds, Dictionary<string, List<IMutation>> mutationTables)
        {
            int lower = 0;
            int upper = 0;
            Dictionary<string, Tuple<int, int>> ranges = new Dictionary<string, Tuple<int, int>>();
            foreach(string t in themeOdds.Keys)
            {
                if (mutationTables.TryGetValue(t, new List<IMutation>()).Count > 0)
                {
                    lower = upper + 1;
                    upper = lower + themeOdds[t] + GetChanceModifier(t);
                    ranges.Add(t, new Tuple<int, int>(lower, upper));
                }
            }
            int index = Rand.Range(1, upper);
            foreach(string t in ranges.Keys)
            {
                if (index >= ranges[t].Item1 && index <= ranges[t].Item2)
                {
                    return t;
                }
            }
            return "NA";
        }

        public virtual int GetChanceModifier(string theme)
        {
            return 0;
        }

        public virtual ThingDef GetThingDef(ThingDef def)
        {
            List<ThingDef> defs = defOptions.TryGetValue(def, null);
            if (defs != null)
            {
                return defs[Rand.Range(0, defs.Count)];
            }

            return null;
        }

        public virtual int GetAggressionLevel()
        {
            return aggressionLevel;
        }
    }

}
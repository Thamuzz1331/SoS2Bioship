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

        public Dictionary<string, float> statMultipliers = new Dictionary<string, float>();
        public Dictionary<ThingDef, Dictionary<string, float>> specStatMultipliers = new Dictionary<ThingDef, Dictionary<string, float>>();
        public Dictionary<string, List<ThingDef>> organOptions = new Dictionary<string, List<ThingDef>>()
        {
            {"smallTurretOptions", new List<ThingDef>(){}},
            {"mediumTurretOptions", new List<ThingDef>(){}},
            {"largeTurretOptions", new List<ThingDef>(){}},
            {"spinalTurretOptions", new List<ThingDef>(){}},
            {"smallMawOptions", new List<ThingDef>(){
                ThingDef.Named("Maw_Small"), ThingDef.Named("Maw_Small"),
                ThingDef.Named("Maw_Small"), ThingDef.Named("Maw_Small"),
            }}
        };
  

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
               heartId = Guid.NewGuid().ToString();
               Scribe_Values.Look<String>(ref heartId, "heartId", "NA");
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

    }

}
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



    }

}
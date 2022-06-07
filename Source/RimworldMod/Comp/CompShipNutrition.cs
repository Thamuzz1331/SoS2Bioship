using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompShipNutrition : ThingComp
    {
        public ShipNutritionNet myNet = null;

        public CompProperties_ShipNutrition Props
        {
            get { return props as CompProperties_ShipNutrition; }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            ((ShipNutrientMapComp)this.parent.Map.components.Where(t => t is ShipNutrientMapComp).FirstOrDefault()).Register(this);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using LivingBuildings;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompShipNutritionStore : CompNutritionStore
    {
        public CompProperties_ShipNutritionStore ShipProps => (CompProperties_ShipNutritionStore)props;

        public float efficiency = 1f;
        public float capacityModifier = 1f;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref efficiency, "efficiency", 1f);
            Scribe_Values.Look<float>(ref capacityModifier, "capacityModifier", 1f);
        }
        public override void PostSpawnSetup(bool s)
        {
            base.PostSpawnSetup(s);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (mode == DestroyMode.Deconstruct)
            {
                //TODO: On deconstruct, drop meat based on stored nutrition
            }
        }

        public override float storeNutrition(float qty)
        {
            qty *= efficiency;
            return base.storeNutrition(qty);
        }

        public override float getNutrientCapacity()
        {
            return base.getNutrientCapacity() * capacityModifier;
        }
    }
}
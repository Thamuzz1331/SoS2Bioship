using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    public class CompShipNutritionStore : CompNutritionStore
    {
        public CompProperties_ShipNutritionStore ShipProps => (CompProperties_ShipNutritionStore)props;

        public float efficiency = 1f;

        public override void PostExposeData()
        {
            base.PostExposeData();
        }

        public override void PostSpawnSetup(bool s)
        {
            base.PostSpawnSetup(s);
        }

        public override float storeNutrition(float qty)
        {
            qty *= efficiency;
            float overflow = 0f;
            float toStore = qty;
            currentNutrition += toStore;
            overflow = currentNutrition - getNutrientCapacity();
            if (overflow <= 0)
            {
                return 0;
            }
            currentNutrition = getNutrientCapacity();
            return overflow;
        }

    }
}
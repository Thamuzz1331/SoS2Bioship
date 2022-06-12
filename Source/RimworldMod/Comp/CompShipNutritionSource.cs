using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using SaveOurShip2;

namespace RimWorld
{
    public class CompShipNutritionSource : CompShipNutrition
    {
        private CompProperties_ShipNutritionSource Props => (CompProperties_ShipNutritionSource)props;
        public float getNutritionPerPulse()
        {
            if (parent.TryGetComp<CompRefuelable>() != null && parent.TryGetComp<CompRefuelable>().Fuel <= 0)
            {
                return 0;
            }
            if (body != null && body.heart != null)
            {
                return (Props.nutrientPerPulse * body.heart.getStatMultiplier("nutrientPerPulse", parent.def));
            }
            return Props.nutrientPerPulse;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    public class CompProperties_ShipNutritionConsumer : CompProperties
    {
        public float consumptionPerPulse;
        public CompProperties_ShipNutritionConsumer()
        {
            compClass = typeof(CompShipNutritionConsumer);
        }

    }
}

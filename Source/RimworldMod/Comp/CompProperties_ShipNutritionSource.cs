using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    public class CompProperties_ShipNutritionSource : CompProperties_RefuelableNutritionSource
    {
        public CompProperties_ShipNutritionSource()
        {
            compClass = typeof(CompShipNutritionSource);
        }
    }
}

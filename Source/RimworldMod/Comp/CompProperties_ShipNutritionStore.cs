using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    public class CompProperties_ShipNutritionStore : CompProperties_NutritionStore
    {
        public CompProperties_ShipNutritionStore()
        {
            compClass = typeof(CompShipNutritionStore);
        }
    }
}

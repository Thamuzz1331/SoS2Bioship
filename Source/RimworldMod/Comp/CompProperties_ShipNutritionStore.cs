using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_ShipNutritionStore : CompProperties
    {
        public float nutrientCapacity;
        public float initialNutrition;
        public CompProperties_ShipNutritionStore()
        {
            compClass = typeof(CompShipNutritionStore);
        }
    }
}

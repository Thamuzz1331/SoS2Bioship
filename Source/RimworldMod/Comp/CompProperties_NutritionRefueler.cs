using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    public class CompProperties_NutritionRefueler : CompProperties
    {
        public float refuelInterval = 120f;
        public float refuelCost = 50f;
        public float refuelAmmount = 10f;
        public CompProperties_NutritionRefueler()
        {
            compClass = typeof(CompNutritionRefueler);
        }
    }
}

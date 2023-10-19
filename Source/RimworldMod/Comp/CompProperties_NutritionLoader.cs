using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_NutritionLoader : CompProperties
    {
        public string ammoCat;
        public float reloadInterval;
        public float reloadCost;

        public CompProperties_NutritionLoader()
        {
            compClass = typeof(CompNutritionLoader);
        }

    }
}

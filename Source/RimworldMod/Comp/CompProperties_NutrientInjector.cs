using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_NutrientInjector : CompProperties_Facility
    {
        public CompProperties_NutrientInjector()
        {
            compClass = typeof(CompNutrientInjector);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_EatMe : CompProperties
    {
        public CompProperties_EatMe()
        {
            compClass = typeof(CompEatMe);
        }

    }
}

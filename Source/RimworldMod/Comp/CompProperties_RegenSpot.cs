using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_RegenSpot : CompProperties
    {
        public CompProperties_RegenSpot()
        {
            compClass = typeof(CompRegenSpot);
        }

    }
}

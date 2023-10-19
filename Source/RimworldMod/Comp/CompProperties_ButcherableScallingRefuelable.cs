using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_ButcherableScallingRefuelable : CompProperties_Refuelable
    {
        public CompProperties_ButcherableScallingRefuelable()
        {
            compClass = typeof(CompButcherableScallingRefuelable);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_SalvageMaw : CompProperties_SalvageBay
    {
        public CompProperties_SalvageMaw()
        {
            compClass = typeof(CompSalvageMaw);
        }
    }
}

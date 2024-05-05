using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using SaveOurShip2;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_SalvageMaw : CompProps_ShipBay
    {
        public CompProperties_SalvageMaw()
        {
            compClass = typeof(CompSalvageMaw);
        }
    }
}

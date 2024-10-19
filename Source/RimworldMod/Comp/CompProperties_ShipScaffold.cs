using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_ShipScaffold : CompProperties_Scaffold
    {
        public CompProperties_ShipScaffold()
        {
            compClass = typeof(CompShipScaffold);
        }
    }

}

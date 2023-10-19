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
    public class CompProperties_ShipScaffoldConverter : CompProperties_ScaffoldConverter
    {
        public CompProperties_ShipScaffoldConverter()
        {
            compClass = typeof(CompShipScaffoldConverter);
        }
    }
}

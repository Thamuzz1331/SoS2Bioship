using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    public class CompProperties_ShipScaffoldConverter : CompProperties_ScaffoldConverter
    {
        public CompProperties_ShipScaffoldConverter()
        {
            compClass = typeof(CompShipScaffoldConverter);
        }
    }
}

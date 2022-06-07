using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    class CompProperties_ShipfleshConversion : CompProperties
    {
        public SimpleCurve radiusPerDayCurve;

        public CompProperties_ShipfleshConversion()
        {
            compClass = typeof(CompShipfleshConversion);
        }
    }
}

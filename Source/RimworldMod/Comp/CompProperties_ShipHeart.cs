using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    public class CompProperties_ShipHeart : CompProperties_BuildingCore
    {
        public string species = null;
        public CompProperties_ShipHeart()
        {
            compClass = typeof(CompShipHeart);
        }
    }

}

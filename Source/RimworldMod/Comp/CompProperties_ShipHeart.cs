using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    public class CompProperties_ShipHeart : CompProperties_BuildingCore
    {
        public string shipspecies = null;
        public string geneline;
        public CompProperties_ShipHeart()
        {
            compClass = typeof(CompShipHeart);
        }
    }

}

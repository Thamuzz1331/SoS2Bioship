using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    class CompProperties_ShipBodyPart : CompProperties
    {
        public bool regen = false;
        public string regenDef = "";
        public CompProperties_ShipBodyPart()
        {
            compClass = typeof(CompShipBodyPart);
        }
    }

}

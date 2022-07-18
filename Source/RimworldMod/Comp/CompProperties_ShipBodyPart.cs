using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    public class CompProperties_ShipBodyPart : CompProperties_BuildingBodyPart
    {
        public string regenDef = null;
        public bool isArmor = false;
        public bool growsArmor = false;
        public string whitherTo = null;

        public CompProperties_ShipBodyPart()
        {
            compClass = typeof(CompShipBodyPart);
        }
    }

}

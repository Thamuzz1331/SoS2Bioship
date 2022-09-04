using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    public class CompProperties_FueledFacility : CompProperties_Facility
    {
        public CompProperties_FueledFacility()
        {
            compClass = typeof(CompFueledFacility);
        }

    }
}

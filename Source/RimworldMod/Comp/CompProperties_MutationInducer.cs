using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    public class CompProperties_MutationInducer : CompProperties_Facility
    {
        public CompProperties_MutationInducer()
        {
            compClass = typeof(CompMutationInducer);
        }

    }
}

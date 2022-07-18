using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    public class CompProperties_MutationWorker : CompProperties
    {
        public CompProperties_MutationWorker()
        {
            compClass = typeof(CompMutationWorker);
        }
    }

}

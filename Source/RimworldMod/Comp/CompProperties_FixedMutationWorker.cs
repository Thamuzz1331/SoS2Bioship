using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    public class CompProperties_FixedMutationWorker : CompProperties
    {
        public List<String> startingMutations = new List<String>();
        public CompProperties_FixedMutationWorker()
        {
            compClass = typeof(CompFixedMutationWorker);
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    class CompProperties_RegenWorker : CompProperties
    {
        public CompProperties_RegenWorker()
        {
            compClass = typeof(CompRegenWorker);
        }
    }

}

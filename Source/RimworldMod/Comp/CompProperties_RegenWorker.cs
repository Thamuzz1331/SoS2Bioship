using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_RegenWorker : CompProperties
    {
        public float regenCost = 0;
        public float regenInterval = 0;
        public CompProperties_RegenWorker()
        {
            compClass = typeof(CompRegenWorker);
        }
    }

}

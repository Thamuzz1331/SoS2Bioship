using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_Aggression : CompProperties
    {
        public int baseAggression = 0;
        public CompProperties_Aggression()
        {
            compClass = typeof(CompAggression);
        }
    }

}

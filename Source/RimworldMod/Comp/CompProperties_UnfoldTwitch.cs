using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using SaveOurShip2;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_UnfoldTwitch : CompProps_Unfold
    {
        public CompProperties_UnfoldTwitch()
        {
            compClass = typeof(CompUnfoldTwitch);
        }
    }

}

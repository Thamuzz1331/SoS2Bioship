using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_StaplebleScaffold : CompProperties_Scaffold
    {
        public string stapleDef = null;
        public CompProperties_StaplebleScaffold()
        {
            compClass = typeof(CompStapleableScaffold);
        }
    }

}

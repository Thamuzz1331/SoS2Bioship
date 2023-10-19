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
    public class CompProperties_MutableScaffold : CompProperties_Scaffold
    {
        public string stapleDef = null;
        public CompProperties_MutableScaffold()
        {
            compClass = typeof(CompMutableScaffold);
        }
    }

}

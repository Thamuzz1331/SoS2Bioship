using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    public class CompProperties_MutableScaffold : CompProperties_Scaffold
    {
        public string stapleDef = null;
        public CompProperties_MutableScaffold()
        {
            compClass = typeof(CompMutableScaffold);
        }
    }

}

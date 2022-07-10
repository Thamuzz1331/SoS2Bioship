using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    class CompProperties_Scaffold : CompProperties
    {
        public string transformString = "";
        public bool mutable = false;
        public CompProperties_Scaffold()
        {
            compClass = typeof(CompScaffold);
        }
    }

}

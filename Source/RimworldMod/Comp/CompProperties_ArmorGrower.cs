using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    public class CompProperties_ArmorGrower : CompProperties
    {
        public float growInterval = 120f;
        public float shedInterval = 15f;
        public CompProperties_ArmorGrower()
        {
            compClass = typeof(CompArmorGrower);
        }
    }

}

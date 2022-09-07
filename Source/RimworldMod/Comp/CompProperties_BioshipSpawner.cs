using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    public class CompProperties_BioshipSpawner : CompProperties_NutritionConsumer
    {
        public string creatureDef = null;
        public float gestationTime = 60000f;

        public CompProperties_BioshipSpawner()
        {
            compClass = typeof(CompBioshipSpawner);
        }
    }

}

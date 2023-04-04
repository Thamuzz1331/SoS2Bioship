using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    public class CompProperties_BioshipSpawner : CompProperties_NutritionConsumer
    {
        public string creatureDef = null;
        public int spawnCount = 1;

        public CompProperties_BioshipSpawner()
        {
            compClass = typeof(CompBioshipSpawner);
        }
    }

}

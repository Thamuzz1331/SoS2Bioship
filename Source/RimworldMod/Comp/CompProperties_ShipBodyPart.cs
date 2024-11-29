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
    public class CompProperties_ShipBodyPart : CompProperties_BuildingBodyPart
    {
        public string regenDef = null;
        public bool isArmor = false;
        public bool isCorner = false;
        public bool isFlip = false;
        public bool growsArmor = false;
        public string whitherTo = null;
        public float baseArmor = 0.05f;
        public bool adaptiveArmor = false;
        public bool globalAdaptation = false;
        public float turretNutritionCost = 0;

        public CompProperties_ShipBodyPart()
        {
            compClass = typeof(CompShipBodyPart);
        }
    }

}

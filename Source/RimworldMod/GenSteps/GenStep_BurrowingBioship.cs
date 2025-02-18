using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using System.Text;
using Verse;
using Verse.AI.Group;
using RimWorld.Planet;
using SaveOurShip2;

namespace RimWorld
{
    class GenStep_BurrowingBioship : GenStep_Scatterer
    {
        private static Type shipCombatManagerType = AccessTools.TypeByName("ShipCombatManager");

        public override int SeedPart
        {
            get
            {
                return 564649879;
            }
        }

        protected override bool CanScatterAt(IntVec3 c, Map map)
        {
            return true;
        }

        protected override void ScatterAt(IntVec3 c, Map map, GenStepParams stepparams, int stackCount = 1)
        {
            List<Building> cores = new List<Building>();
            //limited to 100x100 due to unsettable map size, no fleets
            ShipDef ship = DefDatabase<ShipDef>.AllDefs.Where(def => def.core.shapeOrDef == "Ship_Heart_Quest").RandomElement();
            ShipInteriorMod2.GenerateShip(ship, map, null, Faction.OfAncientsHostile, null, out cores, false, true, 0, (map.Size.x - ship.sizeX) / 2, (map.Size.z - ship.sizeZ) / 2);
        }
    }
}

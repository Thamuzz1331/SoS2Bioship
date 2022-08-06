using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using System.Text;
using Verse;
using Verse.AI.Group;
using RimWorld.Planet;

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
            Building core = null;
            ref Building coreRef = ref core;
            try
            {
                EnemyShipDef d = DefDatabase<EnemyShipDef>.AllDefs.Where(def=>def.core.shapeOrDef == "Ship_Heart_Hostile").RandomElement();
                object[] parameters = new object[]{
                    d,
                    map,
                    null,
                    Faction.OfInsects,
                    null,
                    coreRef,
                    true,
                    true
                };
                AccessTools.Method(shipCombatManagerType, "GenerateShip").Invoke(null, parameters);
            }
            catch(Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}

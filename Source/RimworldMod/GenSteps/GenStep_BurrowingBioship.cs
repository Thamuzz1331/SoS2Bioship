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
                Log.Message("! Generating fallen ship");
                Log.Message("!" + DefDatabase<EnemyShipDef>.AllDefs.Where(def=>def.spaceSite).RandomElement());
                Traverse.Create(shipCombatManagerType).Method("GenerateShip", 
                    DefDatabase<EnemyShipDef>.AllDefs.Where(def=>def.spaceSite).RandomElement(), 
                    map, 
                    null, 
                    Faction.OfAncients, 
                    null, 
                    coreRef, 
                    true, 
                    true).GetValue();
                Log.Message("core " + core);
            }
            catch(Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}

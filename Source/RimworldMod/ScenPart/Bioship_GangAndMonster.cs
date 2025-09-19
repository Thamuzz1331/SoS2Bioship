using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using SaveOurShip2;

namespace RimWorld
{
    class ScenPart_GangAndMonster : ScenPart
    {
        public override void GenerateIntoMap(Map map)
        {
            List<Pawn> startingPawns = Find.GameInitData.startingAndOptionalPawns;
            List<Building> cores = new List<Building>();
            try
            {
                ShipInteriorMod2.GenerateShip(DefDatabase<ShipDef>.GetNamed("SleepyBoy"), map, null, Faction.OfPlayer, null, out cores, true, true);
                ((MapCompBuildingTracker)map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).RepairPulse();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
            foreach (Letter letter in Find.LetterStack.LettersListForReading)
                Find.LetterStack.RemoveLetter(letter);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using SaveOurShip2;

namespace RimWorld
{
    public class Projectile_MonsterSpawner : Projectile_ExplosiveShip
    {
        bool hasSpawned = false;

        public override void Tick()
        {
            Map m = this.Map;
            IntVec3 p = this.Position;
            base.Tick();
            if (hasSpawned == false)
            {
                foreach (CompShipHeatShield shield in m.GetComponent<ShipMapComp>().Shields)
                {
                    if (!shield.shutDown && Position.DistanceTo(shield.parent.Position) <= (shield.radius+1))
                    {
                        float age = this.weaponDamageMultiplier;
                        PawnGenerationRequest req = 
                            new PawnGenerationRequest(
                                PawnKindDef.Named("BioShip_VoidJelly"),
                                this.Launcher.Faction, 
                                PawnGenerationContext.NonPlayer, 
                                -1, 
                                true, 
                                false, 
                                false, 
                                false, 
                                true, 
                                0, 
                                allowFood: false, 
                                allowAddictions: false, 
                                forceNoIdeo: true, 
                                forbidAnyTitle: true, 
                                fixedBiologicalAge: age, 
                                fixedChronologicalAge: age, 
                                forceNoBackstory: true);
                        Pawn jelly = PawnGenerator.GeneratePawn(req);
                        IntVec3 targetCell;
                        CellFinder.TryFindRandomSpawnCellForPawnNear(p, m, out targetCell, 3);
                        GenSpawn.Spawn(jelly, targetCell, m, Rot4.North);
                        //                    jelly.mindState?.mentalStateHandler?.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
                        hasSpawned = true;
                        break;
                    }
                }
            }

        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map m = this.Map;
            IntVec3 p = this.Position;
            base.Impact(hitThing, blockedByShield);
            float age = this.weaponDamageMultiplier;
            PawnGenerationRequest req =
                new PawnGenerationRequest(
                    PawnKindDef.Named("BioShip_VoidJelly"),
                    this.Launcher.Faction,
                    PawnGenerationContext.NonPlayer,
                    -1,
                    true,
                    false,
                    false,
                    false,
                    true,
                    0,
                    allowFood: false,
                    allowAddictions: false,
                    forceNoIdeo: true,
                    forbidAnyTitle: true,
                    fixedBiologicalAge: age,
                    fixedChronologicalAge: age,
                    forceNoBackstory: true);
            Pawn jelly = PawnGenerator.GeneratePawn(req);
            IntVec3 targetCell;
            CellFinder.TryFindRandomSpawnCellForPawnNear(p, m, out targetCell, 3);
            GenSpawn.Spawn(jelly, targetCell, m, Rot4.North);
 //           jelly.mindState?.mentalStateHandler?.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
        }
    }
}

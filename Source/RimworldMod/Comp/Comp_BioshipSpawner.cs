using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using LivingBuildings;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompBioshipSpawner : ThingComp
    {
        private CompProperties_BioshipSpawner SpawnerProps => (CompProperties_BioshipSpawner)props;
        private CompRefuelable refuelable = null;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            refuelable = parent.TryGetComp<CompRefuelable>();
            if (!respawningAfterLoad && parent.Faction != Faction.OfPlayer)
            {
                DoSpawn();
            }
        }

        public virtual void DoSpawn()
        {
            for (int i = 0; i < SpawnerProps.spawnCount; i++)
            {
                PawnGenerationRequest req =
                new PawnGenerationRequest(
                    PawnKindDef.Named(SpawnerProps.creatureDef),
                    parent.Faction,
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
                    fixedBiologicalAge: 0,
                    fixedChronologicalAge: 0,
                    forceNoBackstory: true);
                Pawn spawn = PawnGenerator.GeneratePawn(req);
                IntVec3 targetCell;
                CellFinder.TryFindRandomSpawnCellForPawnNear(parent.Position, parent.Map, out targetCell, 3);
                GenSpawn.Spawn(spawn, targetCell, parent.Map, Rot4.North);

                spawn.training.Train(TrainableDefOf.Tameness, null, true);
                spawn.training.Train(TrainableDefOf.Obedience, null, true);
                spawn.training.Train(TrainableDefOf.Release, null, true);
                spawn.training.Train(BSTrainableDefOf.Haul, null, true);
                
            }
            if(refuelable != null)
            {
                refuelable.ConsumeFuel(refuelable.Fuel);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (refuelable != null && refuelable.IsFull)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Spawn",
                    icon = ContentFinder<Texture2D>.Get("U_Elements/Gizmo_SpawnHaulbeast"),
                    action = delegate ()
                    {
                        DoSpawn();
                    }
                };
            }
        }

    }

    [DefOf]
    public static class BSTrainableDefOf
    {
        public static TrainableDef Haul;
    }

}
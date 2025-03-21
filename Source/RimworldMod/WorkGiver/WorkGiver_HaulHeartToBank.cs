using System;
using Verse;
using Verse.AI;
using BioShip;

namespace RimWorld
{
    public class WorkGiver_HaulHeartToBank : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(ResourceBank.BioshipThingDefOf.HeartSeed);
            }
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !ModsConfig.BiotechActive;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return !t.IsForbidden(pawn) && pawn.CanReserve(t, 1, -1, null, forced) && this.FindGeneBank(pawn, t) != null;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Thing thing = this.FindGeneBank(pawn, t);
            if (thing != null)
            {
                Job job = JobMaker.MakeJob(BioShipJobDefs.HaulHeartToBank, t, thing, thing.InteractionCell);
                job.count = t.stackCount;
                return job;
            }
            return null;
        }

        private Thing FindGeneBank(Pawn pawn, Thing genepackThing)
        {
            CompHeartSeed genepack = genepackThing.TryGetComp<CompHeartSeed>();
            if (!genepack.AutoLoad)
            {
                return null;
            }
            if (genepack.targetContainer != null)
            {
                if (genepack.targetContainer.Map == genepack.parent.Map)
                {
                    CompGenepackContainer compGenepackContainer = genepack.targetContainer.TryGetComp<CompGenepackContainer>();
                    if (compGenepackContainer != null && !compGenepackContainer.Full)
                    {
                        return genepack.targetContainer;
                    }
                }
                return null;
            }
            return GenClosest.ClosestThingReachable(
                genepack.parent.Position, 
                genepack.parent.Map, 
                ThingRequest.ForDef(ThingDef.Named("GeneBankShipHeart")), 
                PathEndMode.InteractionCell, 
                TraverseParms.For(pawn, 
                Danger.Deadly, 
                TraverseMode.ByPawn, 
                false, 
                false, 
                false), 9999f, delegate (Thing x)
            {
                if (x.IsForbidden(pawn) || !pawn.CanReserve(x, 1, -1, null, false))
                {
                    return false;
                }
                CompShipGeneContainer compGenepackContainer2 = x.TryGetComp<CompShipGeneContainer>();
                if (compGenepackContainer2 == null || compGenepackContainer2.Full || !compGenepackContainer2.autoLoad)
                {
                    return false;
                }
                Thing targetContainer = genepack.targetContainer;
                return targetContainer == null || targetContainer == compGenepackContainer2.parent;
            }, null, 0, -1, false, RegionType.Set_Passable, false);
        }
    }
}

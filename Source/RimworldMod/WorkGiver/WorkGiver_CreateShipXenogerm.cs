using System;
using UnityEngine;
using Verse;
using Verse.AI;
using BioShip;

namespace RimWorld
{
	public class WorkGiver_CreateShipXenogerm : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForDef(ResourceBank.BioshipThingDefOf.ShipGeneAssembler);
			}
		}

		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return !ModsConfig.BiotechActive;
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Building_ShipGeneAssembler building_GeneAssembler;
			if ((building_GeneAssembler = (t as Building_ShipGeneAssembler)) == null)
			{
				return false;
			}
			if (building_GeneAssembler.ArchitesRequiredNow <= 0)
			{
				return building_GeneAssembler.CanBeWorkedOnNow.Accepted && pawn.CanReserve(t, 1, -1, null, forced) && pawn.CanReserveSittableOrSpot(t.InteractionCell, forced);
			}
			if (this.FindArchiteCapsule(pawn) == null)
			{
				JobFailReason.Is("NoIngredient".Translate(ThingDefOf.ArchiteCapsule), null);
				return false;
			}
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Building_ShipGeneAssembler building_GeneAssembler;
			if ((building_GeneAssembler = (t as Building_ShipGeneAssembler)) == null)
			{
				return null;
			}
			if (building_GeneAssembler.ArchitesRequiredNow > 0)
			{
				Thing thing = this.FindArchiteCapsule(pawn);
				if (thing != null)
				{
					Job job = JobMaker.MakeJob(JobDefOf.HaulToContainer, thing, t);
					job.count = Mathf.Min(building_GeneAssembler.ArchitesRequiredNow, thing.stackCount);
					return job;
				}
			}
			return JobMaker.MakeJob(ResourceBank.BioshipJobDefOf.CreateShipXenogerm, t, 1200, true);
		}

		private Thing FindArchiteCapsule(Pawn pawn)
		{
			return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.ArchiteCapsule), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false), null, 0, -1, false, RegionType.Set_Passable, false);
		}
	}
}

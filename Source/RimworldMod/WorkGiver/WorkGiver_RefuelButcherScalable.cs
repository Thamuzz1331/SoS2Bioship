using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using Verse.AI;

namespace Rimworld
{
	internal class WorkGiver_RefuelButcherScalable : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}

		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return ((CorpseMawTracker)pawn.Map.components.Where(t => t is CorpseMawTracker).FirstOrDefault()).corpseMaws;
		}

		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
			CompButcherableScallingRefuelable butcherableScallingRefuelable = thing.TryGetComp<CompButcherableScallingRefuelable>();
			return butcherableScallingRefuelable != null 
				&& butcherableScallingRefuelable.FuelPercentOfTarget < 1 
				&& (forced || butcherableScallingRefuelable.ShouldAutoRefuelNow) 
				&& !thing.IsForbidden(pawn) 
				&& pawn.CanReserveAndReach(thing, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, false) 
				&& pawn.Map.designationManager.DesignationOn(thing, DesignationDefOf.Deconstruct) == null 
				&& FindFood(pawn, thing) != null
				&& !thing.IsBurning();
		}

		public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
			Thing t = FindFood(pawn, thing);
			if (t == null)
            {
				return null;
            }
			return JobMaker.MakeJob(BioShipJobDefs.RefuelButcherScalable, thing, t);
		}

		private Thing FindFood(Pawn pawn, Thing thing)
        {
			CompButcherableScallingRefuelable refuelable = thing.TryGetComp<CompButcherableScallingRefuelable>();
			ThingFilter butcherableFilter = null;
			if (refuelable != null)
			{
				butcherableFilter = refuelable.Props.fuelFilter;
			}
			Predicate<Thing> validator = (Thing z) => !z.IsForbidden(pawn) && pawn.CanReserve(z, 1, -1, null, false) && butcherableFilter.Allows(z);
			return GenClosest.ClosestThingReachable(pawn.Position,
				pawn.Map,
				butcherableFilter.BestThingRequest,
				PathEndMode.ClosestTouch,
				TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false),
				9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
		}
	}
}

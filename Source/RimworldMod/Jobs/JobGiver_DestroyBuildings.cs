using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200009A RID: 154
	public class JobGiver_DestroyBuildings : ThinkNode_JobGiver
	{
		public override float GetPriority(Pawn pawn)
		{
			return 1f;
        }


		protected override Job TryGiveJob(Pawn pawn)
        {
			Job ret = null;
			if (pawn.TryGetComp<CompBuildingHunter>() != null)
            {
				Thing target = FindTarget(pawn);
				return new Job(JobDefOf.AttackMelee, target)
				{
					maxNumMeleeAttacks = 999,
					expiryInterval = 999999,
					attackDoorIfTargetLost = true
				};
			}

			return ret;
        }

		protected Thing FindTarget(Pawn pawn)
        {
			Thing ret = null;

			ret = GenClosest.ClosestThingReachable(
				pawn.Position, 
				pawn.Map, 
				ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), 
				PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 
				9999f, 
				null, 
				null, 
				0, 
				-1, 
				false, 
				RegionType.Set_Passable, 
				false);

			return ret;
        }
	}
}
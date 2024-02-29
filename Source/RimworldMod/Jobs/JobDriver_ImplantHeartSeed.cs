using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class JobDriver_ImplantHeartSeed : JobDriver
	{

		protected Thing HeartSeed
        {
			get
            {
				return this.job.GetTarget(TargetIndex.A).Thing;
			}
		}

		protected Thing ShipHeart
        {
			get
            {
				return this.job.GetTarget(TargetIndex.B).Thing;
            }
        }

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.HeartSeed, this.job, 1, -1, null, errorOnFailed) 
				&& this.pawn.ReserveSittableOrSpot(this.ShipHeart.InteractionCell, this.job, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			Toil reserveSeed = Toils_Reserve.Reserve(TargetIndex.A, 1, 1, (ReservationLayerDef)null);
			yield return reserveSeed;
			this.FailOnDespawnedNullOrForbidden(TargetIndex.B);

			yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveSeed, TargetIndex.A, TargetIndex.B, true, (Predicate<Thing>)null);
			this.ShipHeart.TryGetComp<CompShipHeart>()?.ApplyHeartSeed(this.HeartSeed.TryGetComp<CompHeartSeed>());
			yield break;
		}

		private const int JobEndInterval = 4000;
	}
}

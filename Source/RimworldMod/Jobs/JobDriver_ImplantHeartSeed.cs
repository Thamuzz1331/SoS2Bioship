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
				return this.job.GetTarget(TargetIndex.B).Thing;
			}
		}

		protected Thing ShipHeart
        {
			get
            {
				return this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.HeartSeed, this.job, 1, -1, null, errorOnFailed) 
				&& this.pawn.ReserveSittableOrSpot(this.ShipHeart.InteractionCell, this.job, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.job.count = 1;
			ToilFailConditions.FailOnDespawnedNullOrForbidden<JobDriver_ImplantHeartSeed>(this, (TargetIndex)1);
			ToilFailConditions.FailOnBurningImmobile<JobDriver_ImplantHeartSeed>(this, (TargetIndex)1);
			yield return Toils_Reserve.Reserve((TargetIndex)1, 1, 1, (ReservationLayerDef)null);
			Toil reserveCorpse = Toils_Reserve.Reserve((TargetIndex)2, 1, 1, (ReservationLayerDef)null);
			yield return reserveCorpse;
			yield return ToilFailConditions.FailOnSomeonePhysicallyInteracting<Toil>(ToilFailConditions.FailOnDespawnedNullOrForbidden<Toil>(Toils_Goto.GotoThing((TargetIndex)2, (PathEndMode)3), (TargetIndex)2), (TargetIndex)2);
			yield return ToilFailConditions.FailOnDestroyedNullOrForbidden<Toil>(Toils_Haul.StartCarryThing((TargetIndex)2, false, true, false), (TargetIndex)2);
			yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveCorpse, (TargetIndex)2, (TargetIndex)0, true, (Predicate<Thing>)null);
			yield return Toils_Goto.GotoThing((TargetIndex)1, (PathEndMode)2);
			yield return ToilEffects.WithProgressBarToilDelay(ToilFailConditions.FailOnDestroyedNullOrForbidden<Toil>(ToilFailConditions.FailOnDestroyedNullOrForbidden<Toil>(Toils_General.Wait(250, (TargetIndex)0), (TargetIndex)2), (TargetIndex)1), (TargetIndex)1, false, -0.5f);
			Toil val = new Toil();
			val.initAction = delegate
			{
				this.ShipHeart.TryGetComp<CompShipHeart>()?.ApplyHeartSeed(this.HeartSeed.TryGetComp<CompHeartSeed>());
				this.HeartSeed.Destroy();
			};
			val.defaultCompleteMode = (ToilCompleteMode)1;
			yield return val;
		}

		private const int JobEndInterval = 4000;
	}
}

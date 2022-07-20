using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace RimWorld
{
    public class JobDriver_RefuleButcherableScaling : JobDriver
    {
        private const int Duration = 400;

        protected Thing Refuelable
        {
            get
            {
                return this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        protected CompButcherableScallingRefuelable RefuelableComp
        {
            get
            {
                return this.Refuelable.TryGetComp<CompButcherableScallingRefuelable>();
            }
        }

        protected Thing fuelCorpse
        {
            get
            {
                LocalTargetInfo target = base.job.GetTarget((TargetIndex)2);
                return (Thing)target;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if(RefuelableComp.FuelPercentOfTarget >= 1)
                return false;
            return ReservationUtility.Reserve(base.pawn, base.job.targetA, base.job, 1, 1, null, true);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.job.count = 1;
            ToilFailConditions.FailOnDespawnedNullOrForbidden<JobDriver_RefuleButcherableScaling>(this, (TargetIndex)1);
            ToilFailConditions.FailOnBurningImmobile<JobDriver_RefuleButcherableScaling>(this, (TargetIndex)1);
            yield return Toils_Reserve.Reserve((TargetIndex)1, 1, 1, (ReservationLayerDef)null);
            Toil reserveCorpse = Toils_Reserve.Reserve((TargetIndex)2, 1, 1, (ReservationLayerDef)null);
            yield return reserveCorpse;
            yield return ToilFailConditions.FailOnSomeonePhysicallyInteracting<Toil>(ToilFailConditions.FailOnDespawnedNullOrForbidden<Toil>(Toils_Goto.GotoThing((TargetIndex)2, (PathEndMode)3), (TargetIndex)2), (TargetIndex)2);
            yield return ToilFailConditions.FailOnDestroyedNullOrForbidden<Toil>(Toils_Haul.StartCarryThing((TargetIndex)2, false, true, false), (TargetIndex)2);
            yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveCorpse, (TargetIndex)2, (TargetIndex)0, true, (Predicate<Thing>)null);
            yield return Toils_Goto.GotoThing((TargetIndex)1, (PathEndMode)2);
            yield return ToilEffects.WithProgressBarToilDelay(ToilFailConditions.FailOnDestroyedNullOrForbidden<Toil>(ToilFailConditions.FailOnDestroyedNullOrForbidden<Toil>(Toils_General.Wait(Duration, (TargetIndex)0), (TargetIndex)2), (TargetIndex)1), (TargetIndex)1, false, -0.5f);
            Toil val = new Toil();
            val.initAction = delegate
            {
                RefuelableComp.Refuel(new List<Thing>() { fuelCorpse });
            };
            val.defaultCompleteMode = (ToilCompleteMode)1;
            yield return val;
        }
    }
}

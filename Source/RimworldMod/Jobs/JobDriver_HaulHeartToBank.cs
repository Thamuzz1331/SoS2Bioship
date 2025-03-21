using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld
{
    public class JobDriver_HaulHeartToBank : JobDriver
    {
        private Thing Container
        {
            get
            {
                return this.job.GetTarget(TargetIndex.B).Thing;
            }
        }

        private CompShipGeneContainer ContainerComp
        {
            get
            {
                return this.Container.TryGetComp<CompShipGeneContainer>();
            }
        }

        private CompHeartSeed HeartSeed
        {
            get
            {
                return this.job.GetTarget(TargetIndex.A).Thing.TryGetComp<CompHeartSeed>();
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.GetTarget(TargetIndex.A), this.job, 1, -1, null, errorOnFailed, false);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(delegate ()
            {
                CompShipGeneContainer containerComp = this.ContainerComp;
                return containerComp == null || containerComp.Full || (!containerComp.autoLoad && (!containerComp.leftToLoad.Contains(this.HeartSeed.parent) || this.HeartSeed.targetContainer != this.Container));
            });
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell, false).FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, true, false, true, false);
            yield return Toils_Goto.Goto(TargetIndex.B, PathEndMode.Touch);
            Toil toil = Toils_General.Wait(30, TargetIndex.B).WithProgressBarToilDelay(TargetIndex.B, false, -0.5f).FailOnDespawnedOrNull(TargetIndex.B);
            toil.handlingFacing = true;
            yield return toil;
            yield return Toils_Haul.DepositHauledThingInContainer(TargetIndex.B, TargetIndex.A, delegate
            {
                this.HeartSeed.parent.def.soundDrop.PlayOneShot(SoundInfo.InMap(this.Container, MaintenanceType.None));
                this.HeartSeed.targetContainer = null;
                CompShipGeneContainer containerComp = this.ContainerComp;
                containerComp.leftToLoad.Remove(this.HeartSeed.parent);
                MoteMaker.ThrowText(this.Container.DrawPos, this.pawn.Map, "InsertedThing".Translate(string.Format("{0} / {1}", containerComp.innerContainer.Count, containerComp.Props.maxCapacity)), -1f);
            });
            yield break;
        }

        private const TargetIndex GenepackInd = TargetIndex.A;

        private const TargetIndex ContainerInd = TargetIndex.B;

        private const int InsertTicks = 30;
    }
}

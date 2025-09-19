using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobDriver_CreateShipXenogerm : JobDriver
	{
		private Building_ShipGeneAssembler Xenogerminator
		{
			get
			{
				return (Building_ShipGeneAssembler)base.TargetThingA;
			}
		}

		// Token: 0x06003A88 RID: 14984 RVA: 0x001598F4 File Offset: 0x00157AF4
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.Xenogerminator, this.job, 1, -1, null, errorOnFailed) && this.pawn.ReserveSittableOrSpot(this.Xenogerminator.InteractionCell, this.job, errorOnFailed);
		}

		// Token: 0x06003A89 RID: 14985 RVA: 0x00159942 File Offset: 0x00157B42
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOn(() => !this.Xenogerminator.CanBeWorkedOnNow.Accepted);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.tickIntervalAction = delegate (int delta)
            {
                float workAmount = this.pawn.GetStatValue(StatDefOf.ResearchSpeed, true, -1) * this.Xenogerminator.GetStatValue(StatDefOf.AssemblySpeedFactor, true, -1) * (float)delta;
                this.Xenogerminator.DoWork(workAmount);
                this.pawn.skills.Learn(SkillDefOf.Intellectual, 0.1f * (float)delta, false, false);
                this.pawn.GainComfortFromCellIfPossible(delta, true);
                if (this.Xenogerminator.ProgressPercent >= 1f)
                {
                    this.Xenogerminator.Finish();
                    this.pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
                    return;
                }
            };
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
			toil.WithEffect(EffecterDefOf.GeneAssembler_Working, TargetIndex.A, null);
			toil.WithProgressBar(TargetIndex.A, () => this.Xenogerminator.ProgressPercent, false, 0f, false);
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.defaultDuration = 4000;
			toil.activeSkill = (() => SkillDefOf.Intellectual);
			yield return toil;
			yield break;
		}

		// Token: 0x040023AE RID: 9134
		private const int JobEndInterval = 4000;
	}
}

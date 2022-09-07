using SaveOurShip2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class CompAggression : ThingComp
	{
		public CompProperties_Aggression Props => (CompProperties_Aggression)props;

		private float ticksToAttack = 0;
		public float attackInterval = 120;
		public int modifiedAggression = 0;
		public float extremeAggressionDetection = 0f;
		public float extremeAggressionInterval = 600f;

		public HashSet<Thing> otherFlesh = new HashSet<Thing>();
		public HashSet<Thing> adjacentMechanicals = new HashSet<Thing>();
		public HashSet<Thing> targets = new HashSet<Thing>();

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref ticksToAttack, "ticksToAttack", 0);
			Scribe_Values.Look(ref extremeAggressionDetection, "extremeAggressionDetection", 0);
			Scribe_Collections.Look(ref targets, "targets", LookMode.Reference);
		}

		public override void CompTick()
        {
			if (!parent.Spawned)
            {
				return;
            }

			if (ticksToAttack <= 0)
            {
				DoAttack();
				ticksToAttack = attackInterval;
            }
			ticksToAttack--;
			if (extremeAggressionDetection <= 0)
            {
				OmniAggressionPulse();
				extremeAggressionDetection = extremeAggressionInterval;
            }
			extremeAggressionDetection--;
		}

		public virtual int GetAggression()
        {
			CompShipHeart heart = parent.TryGetComp<CompShipHeart>();
			if (heart.luciferiumAddiction && !heart.luciferiumSupplied)
            {
				return 3;
            }
			return (modifiedAggression + Props.baseAggression);
        }

		public virtual void DoAttack()
        {
			int numAttack = Rand.RangeInclusive(1, 3);
			for (int i = 0; i < numAttack; i++)
			{
				//I probably should update this to use enums that are set by hediffs or something to that effect.
				switch (GetAggression())
				{
					case 1:
						BasicAggress(adjacentMechanicals);
						break;
					case 2:
						BasicAggress(adjacentMechanicals);
						BasicAggress(otherFlesh);
						break;
					case 3:
						BasicAggress(adjacentMechanicals);
						BasicAggress(otherFlesh);
						BasicAggress(targets);
						break;
					default:
						return;
				};
			}
        }

		private void BasicAggress(HashSet<Thing> targetList)
        {
			if (targetList.Count > 0)
            {
				Thing target = targetList.RandomElement();
				bool stillViable = !target.Destroyed;
				if (stillViable)
                {
					foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(target.Position))
					{
						Thing bbp = c.GetFirstThingWithComp<CompBuildingBodyPart>(parent.Map);
						stillViable = stillViable && !(bbp == null || bbp.TryGetComp<CompBuildingBodyPart>().bodyId != this.parent.TryGetComp<CompShipHeart>().bodyId);
					}
                }
				if (!stillViable)
                {
					targetList.Remove(target);
					BasicAggress(targetList);
					return;
                }
				target.TakeDamage(new DamageInfo(ShipDamageDefOf.ShipAcid, 25f, 0.5f, -1f, this.parent));
            }
        }

		private void OmniAggressionPulse()
        {
			if (GetAggression() == 3 && targets.Count <= 0)
            {

				CompShipHeart heart = parent.TryGetComp<CompShipHeart>();
				if (heart != null && heart.body != null)
				{
					foreach(Thing bodypart in heart.body.bodyParts)
					{
						foreach (Thing adj in bodypart.Position.GetThingList(parent.Map))
						{
							CompBuildingBodyPart bp = adj.TryGetComp<CompBuildingBodyPart>();
							if ((adj is Building || adj is Pawn) && adj.def != ThingDef.Named("LuciferiumInjector") && (bp == null || bp.bodyId != heart.bodyId))
                            {
								targets.Add(adj);
                            }
						}
					}
				}
			}
        }
	}
}
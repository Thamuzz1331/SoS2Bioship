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

		public HashSet<Thing> otherFlesh = new HashSet<Thing>();
		public HashSet<Thing> adjacentMechanicals = new HashSet<Thing>();

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref ticksToAttack, "ticksToAttack", 0);
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
		}

		public virtual void DoAttack()
        {
			int numAttack = Rand.Range(1, 2);
			for (int i = 0; i < numAttack; i++)
			{
				switch ((modifiedAggression + Props.baseAggression))
				{
					case 1:
						BasicAggress(adjacentMechanicals);
						break;
					case 2:
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
				if (target.Destroyed)
                {
					targetList.Remove(target);
					return;
                }
				if (target is Building)
                {
					Building t = ((Building)target);
					t.HitPoints -= 50;
					if (t.HitPoints <= 0)
                    {
						t.Destroy(DestroyMode.KillFinalize);
						targetList.Remove(t);
					}
				}
            }
        }

		private void Aggress()
        {
			if (Rand.Chance(0.5f))
            {
				if (adjacentMechanicals.Count > 0)
                {
					BasicAggress(adjacentMechanicals);
				} else
                {
					BasicAggress(otherFlesh);
                }
            } else
            {
				if (otherFlesh.Count > 0)
				{
					BasicAggress(otherFlesh);
				}
				else
				{
					BasicAggress(adjacentMechanicals);
				}
			}
		}

	}
}
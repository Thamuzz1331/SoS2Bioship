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
	class CompAggression : ThingComp
	{
		protected CompProperties_Aggression Props => (CompProperties_Aggression)props;

		private float ticksToAttack = 0;
		public float attackInterval = 120;

		public override void PostExposeData()
		{
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
				switch (((Building_ShipHeart)parent).GetAggressionLevel())
				{
					case 1:
						BasicAggress(((Building_ShipHeart)parent).body.adjacentMechanicals);
						break;
					case 2:
						Aggress();
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
				Thing target = targetList.ElementAt(Rand.Range(0, targetList.Count));
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
			ShipBody body = ((Building_ShipHeart)parent).body;
			int targetType = Rand.Range(0, 2);
			if (targetType == 1)
            {
				if (body.adjacentMechanicals.Count > 0)
                {
					BasicAggress(body.adjacentMechanicals);
				} else
                {
					BasicAggress(body.otherFlesh);
                }
            } else if (targetType == 0)
            {
				if (body.otherFlesh.Count > 0)
				{
					BasicAggress(body.otherFlesh);
				}
				else
				{
					BasicAggress(body.adjacentMechanicals);
				}
			}
		}

	}
}
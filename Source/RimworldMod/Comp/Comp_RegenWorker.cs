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
	class CompRegenWorker : ThingComp
	{
		protected CompProperties_Aggression Props => (CompProperties_Aggression)props;

		private float ticksToRegen = 0;
		public float regenInterval = 20;

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref ticksToRegen, "ticksToRegen", 0);
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

	}
}
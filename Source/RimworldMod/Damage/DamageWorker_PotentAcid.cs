using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
	class DamageWorker_PotentAcid : DamageWorker_AddInjury
	{
		public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
		{
			base.ExplosionAffectCell(explosion, c, damagedThings, ignoredThings, canThrowMotes);
			AcidGlob g = c.GetFirstThing<AcidGlob>(explosion.Map);
			if (g != null)
            {
				g.damageRemaining += 120;
				return;
            }
				AcidGlob obj = (AcidGlob)ThingMaker.MakeThing(ThingDef.Named("AcidGlob"));
				obj.damageRemaining = 120;
				obj.damageCountdown = Rand.RangeInclusive(1, 9);
				GenSpawn.Spawn(obj, c, explosion.Map, Rot4.North);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
	class DamageWorker_MotileAcid : DamageWorker_AddInjury
	{
		public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
		{
			base.ExplosionAffectCell(explosion, c, damagedThings, ignoredThings, canThrowMotes);
			AcidGlob g = c.GetFirstThing<AcidGlob>(explosion.Map);
			if (g != null)
            {
				g.damageRemaining += 240;
				return;
            }
			MotileAcidGlob obj = (MotileAcidGlob)ThingMaker.MakeThing(ThingDef.Named("MotileAcidGlob"));
			obj.damageRemaining = 240;
			obj.damageAmount = 2f;
			obj.spreadDef = ThingDef.Named("MotileAcidGlob");
			obj.damageCountdown = Rand.RangeInclusive(1, 9);
			obj.spreadCountdown = 30f;
			GenSpawn.Spawn(obj, c, explosion.Map, Rot4.North);
		}
	}
}

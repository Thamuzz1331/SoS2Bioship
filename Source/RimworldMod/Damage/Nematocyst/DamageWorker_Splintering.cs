using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
	class DamageWorker_Splintering : DamageWorker_AddInjury
	{
		public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
		{
			base.ExplosionAffectCell(explosion, c, damagedThings, ignoredThings, canThrowMotes);
			if(damagedThings.Count > 0 && Rand.Chance(0.15f))
            {
				GenExplosion.DoExplosion(c, explosion.Map, 1.9f, ShipDamageDefOf.ShipNematocystSplinter, explosion);
			}
		}
	}
}

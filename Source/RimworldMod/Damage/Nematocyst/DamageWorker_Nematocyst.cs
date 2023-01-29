using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
	class DamageWorker_Nematocyst : DamageWorker_AddInjury
	{
		public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
		{
			base.ExplosionAffectCell(explosion, c, damagedThings, ignoredThings, canThrowMotes);
			foreach(Thing t in damagedThings)
            {
				CompShipBodyPart bp = t.TryGetComp<CompShipBodyPart>();
				if (bp != null && bp.body != null && bp.body.heart != null)
                {
				}
            }
		}
	}
}

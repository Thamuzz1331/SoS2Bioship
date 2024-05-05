using SaveOurShip2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using LivingBuildings;
using BioShip;

namespace RimWorld
{
    [StaticConstructorOnStartup]
	public class CompBioshipShield : CompShipHeatShield
	{
		public CompBuildingBodyPart bp;
		public override void PostSpawnSetup(bool respawningAfterLoad)
        {
			base.PostSpawnSetup(respawningAfterLoad);
			bp = parent.TryGetComp<CompBuildingBodyPart>();
		}

		public override float CalcHeatGenerated(Projectile proj)
        {
			float heat = base.CalcHeatGenerated(proj);
			if (bp.CoreSpawned)
            {
				heat /= bp.Core.GetStat("shieldStrength"); 
				if (bp.Core.hediffs.Any(diff => (diff is Hediff_Reflect)))
				{
					if (Rand.Chance(0.1f))
					{
						BioShip.BioShip.ReflectShot(this, proj);
					}
				}
			}
			return heat;
		}
	}

}
using SaveOurShip2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using LivingBuildings;

namespace RimWorld
{
	public class CompBioshipShield : CompShipCombatShield
	{
		public CompBuildingBodyPart bp;
		public override void PostSpawnSetup(bool respawningAfterLoad)
        {
			base.PostSpawnSetup(respawningAfterLoad);
			bp = parent.TryGetComp<CompBuildingBodyPart>();
		}

		public override float CalcHeatGenerated(Projectile_ExplosiveShipCombat proj)
        {
			float heat = base.CalcHeatGenerated(proj);
			if (bp.CoreSpawned)
            {
				heat /= bp.Core.GetStat("shieldStrength");
			}
			return heat;
		}
	}

}
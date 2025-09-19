using System;
using Verse;
using Verse.AI.Group;

namespace RimWorld
{
	public class DeathActionWorker_JellyBoom : DeathActionWorker
	{
		public override RulePackDef DeathRules
		{
			get
			{
				return RulePackDefOf.Transition_DiedExplosive;
			}
		}

		public override bool DangerousInMelee
		{
			get
			{
				return true;
			}
		}

		public override void PawnDied(Corpse corpse, Lord prevLord)
		{
			float radius;
			int damage = -1;
			if (corpse.InnerPawn.ageTracker.CurLifeStageIndex == 0)
			{
				radius = 2.9f;
				damage = 40;
			}
			else if (corpse.InnerPawn.ageTracker.CurLifeStageIndex == 1)
			{
				radius = 3.9f;
				damage = 60;
			}
			else if (corpse.InnerPawn.ageTracker.CurLifeStageIndex == 2)
			{
				radius = 4.9f;
				damage = 80;
			}
			else if (corpse.InnerPawn.ageTracker.CurLifeStageIndex == 3)
			{
				radius = 5.9f;
				damage = 100;
			}
			else
			{
				radius = 6.9f;
				damage = 120;
			}
			GenExplosion.DoExplosion(
                center:corpse.Position,
                map:corpse.Map,
                radius:radius,
                damType:ShipDamageDefOf.ShipBioAcid,
                instigator:corpse.InnerPawn,
                damAmount:damage,
                armorPenetration:50f,
                explosionSound:null,
                weapon:null,
                projectile:null,
                intendedTarget:null,
                postExplosionSpawnThingDef:null,
                postExplosionSpawnChance:0f,
                postExplosionSpawnThingCount:0,
                postExplosionGasType:null,
                postExplosionGasRadiusOverride:null,
                postExplosionGasAmount: 0,
                false, 
				null, 
				0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld.Planet;
using HarmonyLib;
using RimWorld;
using Vehicles;
using SaveOurShip2.Vehicles;

namespace SaveOurShip2
{
	public class Verb_LaunchBioShipProjectile : Verb_LaunchProjectileShip
	{
		protected override bool TryCastShot()
		{
			ThingDef projectile = Projectile;
			if (projectile == null)
			{
				return true;
			}
			Building_ShipTurret turret = this.caster as Building_ShipTurret;
			if (turret != null)
			{
				if (turret.GroundDefenseMode) //swap projectile for ground
				{
					if (turret.heatComp.Props.groundProjectile != null)
						projectile = turret.heatComp.Props.groundProjectile;
				}
				else if (turret.PointDefenseMode) //remove registered torps/pods in range
				{
					PointDefense(turret);
				}
				else //register projectile on mapComp
				{
					if (turret.torpComp == null)
						RegisterProjectile(turret, this.shipTarget, verbProps.defaultProjectile, turret.SynchronizedBurstLocation);
					else
						RegisterProjectile(turret, this.shipTarget, turret.torpComp.Projectile.interactionCellIcon, turret.SynchronizedBurstLocation); //This is a horrible kludge, but it's a way to attach one projectile's ThingDef to another projectile
				}
			}
			ShootLine resultingLine = new ShootLine(caster.Position, currentTarget.Cell);
			Thing launcher = caster;
			Thing equipment = base.EquipmentSource;
			Vector3 drawPos = caster.DrawPos;
			if (equipment != null)
			{
				base.EquipmentSource.GetComp<CompChangeableProjectile>()?.Notify_ProjectileLaunched();
			}
			Projectile projectile2 = (Projectile)GenSpawn.Spawn(projectile, resultingLine.Source, caster.Map);

			if (turret.GroundDefenseMode && turret.heatComp.Props.groundMissRadius > 0.5f)
			{
				float num = turret.heatComp.Props.groundMissRadius;
				float num2 = VerbUtility.CalculateAdjustedForcedMiss(num, this.currentTarget.Cell - this.caster.Position);
				if (num2 > 0.5f)
				{
					int max = GenRadial.NumCellsInRadius(num2);
					int num3 = Rand.RangeInclusive(0, max);
					if (num3 > 0)
					{
						IntVec3 c = this.currentTarget.Cell + GenRadial.RadialPattern[num3];
						ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
						if (Rand.Chance(0.5f))
						{
							projectileHitFlags = ProjectileHitFlags.All;
						}
						if (!this.canHitNonTargetPawnsNow)
						{
							projectileHitFlags &= ~ProjectileHitFlags.NonTargetPawns;
						}
						projectile2.Launch(launcher, drawPos, c, this.currentTarget, projectileHitFlags, this.preventFriendlyFire, equipment, null);
						return true;
					}
				}
			}

			if (launcher is Building_ShipTurretTorpedo l)
			{
				projectile2.Launch(launcher, drawPos + l.TorpedoTubePos(), currentTarget.Cell, currentTarget.Cell, ProjectileHitFlags.None, false, equipment);
			}
			else
				projectile2.Launch(launcher, currentTarget.Cell, currentTarget.Cell, ProjectileHitFlags.None, false, equipment);

			if (projectile == ResourceBank.ThingDefOf.Bullet_Fake_Laser || projectile == ResourceBank.ThingDefOf.Bullet_Ground_Laser || projectile == ResourceBank.ThingDefOf.Bullet_Fake_Psychic)
			{
				ShipCombatLaserMote obj = (ShipCombatLaserMote)(object)ThingMaker.MakeThing(ResourceBank.ThingDefOf.ShipCombatLaserMote);
				obj.origin = drawPos;
				obj.destination = currentTarget.Cell.ToVector3Shifted();
				obj.large = this.caster.GetStatValue(StatDefOf.RangedWeapon_DamageMultiplier) > 1.0f;
				obj.color = turret.heatComp.Props.laserColor;
				obj.Attach(launcher);
				GenSpawn.Spawn(obj, launcher.Position, launcher.Map, 0);
			}
			projectile2.HitFlags = ProjectileHitFlags.None;
			return true;
		}
	}
}

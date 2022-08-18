using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
	class AcidGlob : Thing
	{
		public float damageRemaining = 0f;
		public int damageCountdown = 0;
		public float damageAmount = 1f;
		private int damageInterval = 9;
		public ThingDef spreadDef = ThingDef.Named("AcidGlob");

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref damageRemaining, "damageRemaining", 0f, false);
		}

		public override void Tick()
        {
			if (damageCountdown <= 0)
            {
				List<Thing> targs = GetValidTarget(this.Position);
				if (targs != null)
                {
					foreach(Thing t in targs)
                    {
						AcidBurn(t);
					}
					damageRemaining -= damageAmount;
					damageCountdown = damageInterval;
					if (damageRemaining <= 0)
					{
						this.Destroy();
						return;
					}
				}
				else
                {
					List<IntVec3> adjacentTargets = new List<IntVec3>();
					List<AcidGlob> adjAcid = new List<AcidGlob>();
					foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(this.Position))
                    {
						AcidGlob g = c.GetFirstThing<AcidGlob>(this.Map);
						if (g != null)
                        {
							adjAcid.Add(g);
                        } 
						else 
						{
							if (GetValidTarget(c) != null)
							{
								adjacentTargets.Add(c);
							}
						}
					}
					int totalAdj = adjacentTargets.Count + adjAcid.Count;
					if (totalAdj > 0) {
						float shareDamage = damageRemaining / (1.15f*totalAdj);
						foreach(AcidGlob g in adjAcid)
                        {
							g.damageRemaining += shareDamage;
                        }
						foreach(IntVec3 c in adjacentTargets)
                        {
							AcidGlob obj = (AcidGlob)ThingMaker.MakeThing(this.spreadDef);
							obj.damageRemaining = shareDamage;
							obj.damageAmount = this.damageAmount;
							obj.damageCountdown = Rand.RangeInclusive(1, 9);
							GenSpawn.Spawn(obj, c, this.Map, Rot4.North);
						}
					}
					damageRemaining = 0f;
					this.Destroy();
					return;
                }
			}
			damageCountdown--;
        }

		public void AcidBurn(Thing targ)
		{
			Pawn pawn = targ as Pawn;
			if (pawn != null)
			{
				BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(pawn, RulePackDefOf.DamageEvent_Fire);
				Find.BattleLog.Add(battleLogEntry_DamageTaken);
				DamageInfo dinfo = new DamageInfo(ShipDamageDefOf.ShipAcid, damageAmount, 0.5f, -1f, this);
				dinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
				targ.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_DamageTaken);
				if (pawn.apparel != null && pawn.apparel.WornApparel.TryRandomElement(out Apparel result))
				{
					result.TakeDamage(new DamageInfo(ShipDamageDefOf.ShipAcid, damageAmount, 0.5f, -1f, this));
				}
			}
			else
			{
				targ.TakeDamage(new DamageInfo(ShipDamageDefOf.ShipAcid, damageAmount, 0.5f, -1f, this));
			}
		}

		public List<Thing> GetValidTarget(IntVec3 pos)
        {
			List<Thing> targets = pos.GetThingList(this.Map);
			List<Thing> rets = new List<Thing>();
			foreach(Thing t in targets)
            {
				if(t.def.useHitPoints && t.def != ThingDef.Named("ChunkSlagSteel"))
                {
					rets.Add(t);
                }
            }
			if (rets.Count > 0)
            {
				return rets;
            }
			return null;
        }

		public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			if (dinfo.Def == ShipDamageDefOf.ShipAcid)
			{
				damageRemaining += totalDamageDealt;
			}
			else
			{
				base.PostApplyDamage(dinfo, totalDamageDealt);
			}
		}
	}
}

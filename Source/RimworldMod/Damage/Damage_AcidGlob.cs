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
		private int damageInterval = 9;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref damageRemaining, "damageRemaining", 0f, false);
		}

		public override void Tick()
        {
			if (damageCountdown <= 0)
            {
				Thing t = GetValidTarget(this.Position);
				if (t != null)
                {
					AcidBurn(t);
					damageRemaining -= 1f;
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
							AcidGlob obj = (AcidGlob)ThingMaker.MakeThing(ThingDef.Named("AcidGlob"));
							obj.damageRemaining = shareDamage;
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
				DamageInfo dinfo = new DamageInfo(ShipDamageDefOf.ShipAcid, 1, 0f, -1f, this);
				dinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
				targ.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_DamageTaken);
				if (pawn.apparel != null && pawn.apparel.WornApparel.TryRandomElement(out Apparel result))
				{
					result.TakeDamage(new DamageInfo(ShipDamageDefOf.ShipAcid, 1, 0f, -1f, this));
				}
			}
			else
			{
				targ.TakeDamage(new DamageInfo(ShipDamageDefOf.ShipAcid, 1, 0f, -1f, this));
			}
		}

		public Thing GetValidTarget(IntVec3 pos)
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
				return rets.RandomElement();
            }
			return null;
        }
	}
}

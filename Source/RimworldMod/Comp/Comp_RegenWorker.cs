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
    [StaticConstructorOnStartup]
	public class CompRegenWorker : ThingComp
	{
		public CompProperties_RegenWorker Props => (CompProperties_RegenWorker)props;

		public BuildingHediff_ToxicBuildup toxicBuildup;

		private float ticksToRegen = 0f;
		
		public HashSet<Building> wounds = new HashSet<Building>();

		public BuildingBody body = null;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref ticksToRegen, "ticksToRegen", 0f);
			Scribe_Collections.Look(ref wounds, "wounds", LookMode.Reference);
		}

		public override void CompTick()
		{
			if (!parent.Spawned || body == null)
				return;
			if (ticksToRegen <= 0)
            {
				HealWounds();
				ticksToRegen = GetRegenInterval() * 5;
			}
			ticksToRegen--;
		}

		public virtual void RegisterWound(Building target)
        {
			wounds.Add(target);
		}

		public virtual void HealWounds()
        {
			if (toxicBuildup.toxLevel > 0)
            {
				toxicBuildup.toxLevel -= (body.bodyParts.Count/500);
            } else if (toxicBuildup.toxLevel < 0)
            {
				toxicBuildup.toxLevel = 0;
            }
			List<Building> healedWounds = new List<Building>();
			if (wounds.Count > 0 && parent.Map.Biome == ResourceBank.BiomeDefOf.OuterSpaceBiome)
            {
				foreach(Building wounded in wounds)
                {
					if (wounded == null || wounded.Destroyed)
                    {
						healedWounds.Add(wounded);
                    } else
                    {
						if (wounded.TryGetComp<CompBreakdownable>() != null &&
							wounded.TryGetComp<CompBreakdownable>().BrokenDown)
                        {
							wounded.TryGetComp<CompBreakdownable>().Notify_Repaired();
                        }
						if (wounded.HitPoints < wounded.MaxHitPoints)
                        {
							wounded.HitPoints += (int)(wounded.MaxHitPoints/50);
                        } else
                        {
							healedWounds.Add(wounded);
                        }
                    }
                }
            }
			foreach(Building s in healedWounds)
            {
				wounds.Remove(s);
            }
        }

		public virtual float GetRegenCost()
        {
			float cost = Props.regenCost / body.heart.GetStat("regenEfficiency");
			if (parent.Map.Biome != ResourceBank.BiomeDefOf.OuterSpaceBiome)
            {
				cost *= 8f;
			}
			return cost;
        }

		public virtual float GetRegenInterval()
        {
			float interval = Props.regenInterval / body.heart.GetStat("regenSpeed");
			if (parent.Map.Biome != ResourceBank.BiomeDefOf.OuterSpaceBiome)
            {
				interval *= 8f;
            }
			interval *= (1 + (float)(Rand.RangeInclusive(-15, 15)/100));
			return interval;
        }
	}
}
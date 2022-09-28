using SaveOurShip2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class CompRegenWorker : ThingComp
	{
		public CompProperties_RegenWorker Props => (CompProperties_RegenWorker)props;

		public float venomOffset = 0f;

		private float ticksToRegen = 0f;
		private float ticksToVenomDec = 0f;
		
		public HashSet<Building> wounds = new HashSet<Building>();

		public BuildingBody body = null;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref ticksToRegen, "ticksToRegen", 0f);
			Scribe_Values.Look(ref ticksToVenomDec, "ticksToVenomDec", 0f);
			Scribe_Values.Look(ref venomOffset, "venomOffset", 0f);
			Scribe_Collections.Look(ref wounds, "wounds", LookMode.Reference);
		}

		public override void CompTick()
		{
			if (!parent.Spawned || body == null)
				return;
			if (ticksToVenomDec <= 0)
            {
				if (venomOffset > 0f)
                {
					venomOffset -= 0.001f;
                }
				ticksToVenomDec = 20f;
            }
			ticksToVenomDec--;
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
			List<Building> healedWounds = new List<Building>();
			if (wounds.Count > 0 && parent.Map.Biome == ShipInteriorMod2.OuterSpaceBiome)
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
			if (parent.Map.Biome != ShipInteriorMod2.OuterSpaceBiome)
            {
				cost *= 8f;
			}
			return cost  * (1f+(venomOffset/4));
        }

		public virtual void RaiseVenom(float inc)
        {
			if (venomOffset < 3.0f)
            {
				venomOffset += inc;
            }
        }

		public virtual float GetRegenInterval()
        {
			float interval = Props.regenInterval / body.heart.GetStat("regenSpeed");
			if (parent.Map.Biome != ShipInteriorMod2.OuterSpaceBiome)
            {
				interval *= 8f;
            }
			interval *= (1 + (float)(Rand.RangeInclusive(-15, 15)/100));
			return interval * (1f + venomOffset);
        }


	    public override string CompInspectStringExtra()
        {
			return "Venom level " + venomOffset;
        }
	}
}
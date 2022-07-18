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

		private float ticksToRegen = 0;
		public BuildingBody body = null;

		public Dictionary<string, Wound> wounds = new Dictionary<string, Wound>();

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref ticksToRegen, "ticksToRegen", 0);
			Scribe_Collections.Look<string, Wound>(ref wounds, "wounds", LookMode.Value, LookMode.Deep);
		}

		public override void CompTick()
		{
			if (ticksToRegen <= 0)
            {
				HealWounds();
				ticksToRegen = GetRegenInterval();
			}
			ticksToRegen--;
		}

		public virtual void RegisterWound(Thing target)
        {
			CompShipBodyPart bp = target.TryGetComp<CompShipBodyPart>();
			if (bp.woundIds.Count <= 0)
            {
				string newWound = Guid.NewGuid().ToString();
				bp.woundIds.Add(newWound);
				wounds.Add(newWound, new Wound());
			}
			wounds[bp.woundIds[0]].wounds.Push(target);
			foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(target.Position))
			{
				foreach(Thing adj in c.GetThingList(body.heart.parent.Map))
                {
					CompShipBodyPart abp = adj.TryGetComp<CompShipBodyPart>();
					if (abp != null)
                    {
						abp.woundIds.Add(bp.woundIds[0]);
                    }
				}
			}
		}

		public virtual void HealWounds()
        {
			List<string> healedWounds = new List<string>();
			if (wounds.Keys.Count > 0)
            {
				foreach(string wId in wounds.Keys)
                {
					if(TryHealWound(wounds[wId]))
                    {
						healedWounds.Add(wId);
					}
                }
            }
			foreach(string s in healedWounds)
            {
				//wounds.Remove(s);
            }
        }

		public virtual bool TryHealWound(Wound wound)
        {
			if (wound.wounds.Count <= 0)
            {
				return true;
            }
			if (body.RequestNutrition(GetRegenCost()))
            {
				Thing w = wound.wounds.Pop();
				CompShipBodyPart bp = w.TryGetComp<CompShipBodyPart>();
				ThingDef scardef = ((CompShipHeart)body.heart).GetThingDef(bp.ShipProps.regenDef);
				if (scardef == null)
                {
					scardef = ThingDef.Named(bp.ShipProps.regenDef);
                }
				Thing replacement = ThingMaker.MakeThing(scardef);
				CompShipBodyPart bodyPart = replacement.TryGetComp<CompShipBodyPart>();
				if (bodyPart != null)
				{
					bodyPart.SetId(bp.bodyId);
					bodyPart.woundIds = bp.woundIds;
				}
				CompNutrition nutrition = replacement.TryGetComp<CompNutrition>();
				if (nutrition != null)
				{
					nutrition.SetId(bp.bodyId);
				}
				replacement.Rotation = w.Rotation;
				replacement.Position = w.Position;
				replacement.SetFaction(Faction.OfPlayer);
				replacement.SpawnSetup(parent.Map, false);
				if (bodyPart != null)
				{
					bodyPart.woundIds = bp.woundIds;
					foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(replacement.Position))
                    {
						foreach (Thing adj in c.GetThingList(replacement.Map))
						{
							CompShipBodyPart abp = adj.TryGetComp<CompShipBodyPart>();
							if (!adj.Destroyed && abp != null)
							{
								abp.woundIds.Remove(bp.woundIds[0]);
							}
						}
					}
				}
			}
			return false;
        }

		public virtual float GetRegenCost()
        {
			float cost = Props.regenCost * body.heart.GetMultiplier("regenCost");
			if (parent.Map.Biome != ShipInteriorMod2.OuterSpaceBiome)
            {
				cost *= 8f;
			}
			return cost;
        }

		public virtual float GetRegenInterval()
        {
			float interval = Props.regenInterval * body.heart.GetMultiplier("regenInterval");
			if (parent.Map.Biome != ShipInteriorMod2.OuterSpaceBiome)
            {
				interval *= 8f;
            }
			return interval;
        }
	}
}
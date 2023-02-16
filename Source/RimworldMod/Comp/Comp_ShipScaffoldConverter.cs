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
	public class CompShipScaffoldConverter : CompScaffoldConverter
	{
		public override float GetConversionCost()
        {
			if (parent.Faction != Faction.OfPlayer)
            {
				return 0;
            }
			return base.GetConversionCost();
        }

		public override float GetConversionWaitLength()
        {
			if (parent.Faction != Faction.OfPlayer)
            {
				return 0;
            }
			return base.GetConversionWaitLength();
        }

		public override List<Thing> ConvertScaffold(bool instant = false, bool free = false)
		{
/*			if (parent.Faction != Faction.OfPlayer && !instant)
            {
				List<Thing> ret = new List<Thing>();
				for (int i = 0; i < 100; i++)
                {
					ret.AddRange(base.ConvertScaffold(true));
                }
				foreach (Thing t in ret)
				{
					if (t.TryGetComp<CompRefuelable>() != null)
					{
						t.TryGetComp<CompRefuelable>().Refuel(t.TryGetComp<CompRefuelable>().Props.fuelCapacity);
					}
					if (t.TryGetComp<CompNutritionStore>() != null) {
						t.TryGetComp<CompNutritionStore>().currentNutrition = t.TryGetComp<CompNutritionStore>().getNutrientCapacity();
					}
				}
				return ret;
            } else
*/
			List<Thing> ret = base.ConvertScaffold(instant);
			if (body.source.Count < 4)
			{
				foreach(Thing t in ret)
				{
					if (t.def == ThingDef.Named("BioShipHullTile") && t.Position.GetThingList(parent.Map).Count < 2 && Rand.Chance(.6f))
					{
						Thing newMaw = ThingMaker.MakeThing(ThingDef.Named("Maw_Small"));
						CompBuildingBodyPart bodyPart = newMaw.TryGetComp<CompBuildingBodyPart>();
						if (bodyPart != null)
						{
							bodyPart.SetId(this.bodyId);
						}
						CompNutrition nutrition = newMaw.TryGetComp<CompNutrition>();
						if (nutrition != null)
						{
							nutrition.SetId(this.bodyId);
						}
						newMaw.Rotation = t.Rotation;
						newMaw.Position = t.Position;
						newMaw.SetFaction(t.Faction);
						newMaw.SpawnSetup(parent.Map, false);
					}
				}
			}
			return ret;


		}

	}

}
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
	public class CompShipScaffoldConverterInstant : CompShipScaffoldConverter
	{
		public override float GetConversionWaitLength()
		{
			if (parent.Faction == Faction.OfPlayer)
            {
				return base.GetConversionWaitLength();
            }
			return 0f;
		}

		public override float GetConversionCost()
		{
			if (parent.Faction == Faction.OfPlayer)
            {
				return base.GetConversionCost();
            }
			return 0f;
		}

		public override List<Thing> ConvertScaffold(bool instant = false)
		{
			if (parent.Faction == Faction.OfPlayer)
            {
				return base.ConvertScaffold();
            }
			List<Thing> ret = new List<Thing>();
			for (int i = 0; i < 100; i++)
			{
				ret.AddRange(base.ConvertScaffold(instant));
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
		}
	}
}
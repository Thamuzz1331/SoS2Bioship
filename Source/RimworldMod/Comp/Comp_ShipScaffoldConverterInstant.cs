using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class CompShipScaffoldConverterInstant : CompShipScaffoldConverter
	{
		public override float GetConversionWaitLength()
		{
			return 0f;
		}

		public override float GetConversionCost()
		{
			return 0f;
		}

		public override List<Thing> ConvertScaffold()
		{
			List<Thing> ret = new List<Thing>();
			for (int i = 0; i < 10; i++)
			{
				ret.AddRange(base.ConvertScaffold());
			}
			foreach (Thing t in ret)
            {
				if (t.TryGetComp<CompRefuelable>() != null)
                {
					t.TryGetComp<CompRefuelable>().Refuel(t.TryGetComp<CompRefuelable>().Props.fuelCapacity);
				}
				if (t.TryGetComp<CompNutritionStore>() != null) {
					t.TryGetComp<CompNutritionStore>().storeNutrition(t.TryGetComp<CompNutritionStore>().getNutrientCapacity());
				}
            }

			return ret;
		}
	}
}
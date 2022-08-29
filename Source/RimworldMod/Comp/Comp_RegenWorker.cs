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

		public BuildingBody body = null;


		public override void PostExposeData()
		{
			base.PostExposeData();
		}

		public virtual float GetRegenCost()
        {
			float cost = Props.regenCost / body.heart.GetStat("regenEfficiency");
			if (parent.Map.Biome != ShipInteriorMod2.OuterSpaceBiome)
            {
				cost *= 8f;
			}
			return cost;
        }

		public virtual float GetRegenInterval()
        {
			float interval = Props.regenInterval / body.heart.GetStat("regenSpeed");
			if (parent.Map.Biome != ShipInteriorMod2.OuterSpaceBiome)
            {
				interval *= 8f;
            }
			return interval;
        }
	}
}
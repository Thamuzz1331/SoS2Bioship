using System;
using Verse;

namespace RimWorld
{
	public class CompProperties_ShipGeneContainer : CompProperties
	{
		public CompProperties_ShipGeneContainer()
		{
			this.compClass = typeof(CompShipGeneContainer);
		}

		public int maxCapacity;
	}
}

using System;
using Verse;

namespace RimWorld
{
	[DefOf]
	public static class BioShipJobDefs
	{
		static BioShipJobDefs()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf));
		}

		public static JobDef RefuelButcherScalable;
		public static JobDef ImplantHeartSeed;
	}
}
using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02001468 RID: 5224
	[DefOf]
	public static class BioShipJobDefs
	{
		// Token: 0x060080D6 RID: 32982 RVA: 0x002E1CB4 File Offset: 0x002DFEB4
		static BioShipJobDefs()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf));
		}

		public static JobDef RefuelButcherScalable;
	}
}
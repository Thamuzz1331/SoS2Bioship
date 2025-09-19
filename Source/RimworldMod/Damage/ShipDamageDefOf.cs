using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02001468 RID: 5224
	[DefOf]
	public static class ShipDamageDefOf
	{
		// Token: 0x060080D6 RID: 32982 RVA: 0x002E1CB4 File Offset: 0x002DFEB4
		static ShipDamageDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(DamageDefOf));
		}

		public static DamageDef ShipBioAcid;
        public static DamageDef ShipAcid;
		public static DamageDef ShipNematocystSplinter;
		public static DamageDef ShipNematocystEnergized;
		public static DamageDef UVEyeLaser;

    }
}
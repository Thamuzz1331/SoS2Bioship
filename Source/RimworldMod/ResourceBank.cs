using Verse;
using RimWorld;
using UnityEngine;

namespace BioShip
{
	[StaticConstructorOnStartup]
	public static class ResourceBank
	{
		static ResourceBank()
		{
		}
		public static Texture2D NutrientTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.5f, 0.5f, 0.1f));
		public static Texture2D MutationBackground = SolidColorMaterials.NewSolidColorTexture(new Color(0.5f, 0.5f, 0.1f));

		[DefOf]
		public static class BioshipThingDefOf
		{
			public static ThingDef ShipGeneAssembler;
		}

		[DefOf]
		public static class BioshipJobDefOf
		{
			public static JobDef CreateShipXenogerm;
		}

	}
}
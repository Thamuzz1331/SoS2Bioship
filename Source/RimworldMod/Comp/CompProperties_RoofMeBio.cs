using System;
using Verse;

namespace RimWorld
{
	public class CompProperties_RoofMeBio : CompProperties_RoofMe
	{
		public string TerrainId = "FakeFloorShipflesh";

		public CompProperties_RoofMeBio()
		{
			this.compClass = typeof(CompRoofMeBio);
		}
	}
}

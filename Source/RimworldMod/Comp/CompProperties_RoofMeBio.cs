using System;
using Verse;

namespace RimWorld
{
	public class CompProperties_RoofMeBio : CompProperties_SoShipPart
	{
		public string TerrainId = "FakeFloorShipflesh";
		public bool isBioTile = false;

		public CompProperties_RoofMeBio()
		{
			this.compClass = typeof(CompRoofMeBio);
		}
	}
}

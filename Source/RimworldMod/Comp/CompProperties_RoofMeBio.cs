using System;
using Verse;
using SaveOurShip2;

namespace RimWorld
{
    [StaticConstructorOnStartup]
	public class CompProperties_RoofMeBio : CompProps_ShipCachePart
	{
		public string TerrainId = "FakeFloorShipflesh";
		public bool isBioTile = false;

		public CompProperties_RoofMeBio()
		{
			this.compClass = typeof(CompRoofMeBio);
		}
	}
}

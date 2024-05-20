using System;
using Verse;
using RimWorld;

namespace RimWorld
{
	public class PlaceWorker_OnFleshOrScaff : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			CellRect occupiedRect = GenAdj.OccupiedRect(loc, rot, def.Size);
			foreach (IntVec3 vec in occupiedRect)
			{
				bool hasPlating = false;
				foreach (Thing t in vec.GetThingList(map))
				{
					if (t is Building b && b.Faction == Faction.OfPlayer)
					{
						var flesh = b.TryGetComp<CompShipBodyPart>();
						var scaff = b.TryGetComp<CompScaffold>();
						if (flesh != null || scaff != null)
						{
							hasPlating = true;
						}
					}
				}
				if (!hasPlating)
					return new AcceptanceReport(TranslatorFormattedStringExtensions.Translate("SoS.PlaceOnShipHull"));
			}
			return true;
		}
	}
}
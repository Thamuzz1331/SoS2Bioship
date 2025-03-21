using System;
using Verse;
using RimWorld;

namespace RimWorld
{
	public class PlaceWorker_OnScaffOnly : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			CellRect occupiedRect = GenAdj.OccupiedRect(loc, rot, def.Size);
			foreach (IntVec3 vec in occupiedRect)
			{
				bool hasScaff = false;
				foreach (Thing t in vec.GetThingList(map))
				{
					if (t is Building b && b.Faction == Faction.OfPlayer)
					{
						var scaff = b.TryGetComp<CompScaffold>();
						if (scaff != null)
						{
                            hasScaff = true;
						}
					}
				}
				if (!hasScaff)
					return new AcceptanceReport(TranslatorFormattedStringExtensions.Translate("Bioship.OnScaff"));
			}
			return true;
		}
	}
}
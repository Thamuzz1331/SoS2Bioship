using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class CompShipScaffoldConverter : CompScaffoldConverter
	{
		public override List<Thing> ConvertScaffold()
		{
			List<Thing> ret = base.ConvertScaffold();
			if (body.source.Count < 4)
            {
				foreach(Thing t in ret)
                {
					if (t.def == ThingDef.Named("BioShipHullTile") && t.Position.GetThingList(parent.Map).Count < 2 && Rand.Chance(.6f))
                    {
						Thing newMaw = ThingMaker.MakeThing(ThingDef.Named("Maw_Small"));
						CompBuildingBodyPart bodyPart = newMaw.TryGetComp<CompBuildingBodyPart>();
						if (bodyPart != null)
						{
							bodyPart.SetId(this.bodyId);
						}
						CompNutrition nutrition = newMaw.TryGetComp<CompNutrition>();
						if (nutrition != null)
						{
							nutrition.SetId(this.bodyId);
						}

						newMaw.Rotation = t.Rotation;
						newMaw.Position = t.Position;
						newMaw.SetFaction(Faction.OfPlayer);
						newMaw.SpawnSetup(parent.Map, false);
					}
				}
            }

			return ret;
		}
	}

}
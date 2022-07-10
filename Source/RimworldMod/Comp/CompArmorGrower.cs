using SaveOurShip2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
	class CompArmorGrower : ThingComp
	{
		protected CompProperties_ArmorGrower Props => (CompProperties_ArmorGrower)props;

		public HashSet<IntVec3> toGrow = new HashSet<IntVec3>();
		public HashSet<Thing> toShed = new HashSet<Thing>();

		private float ticksToGrow = 0f;
		private float ticksToShed = 0f;

		public override void PostExposeData()
		{
			Scribe_Collections.Look<IntVec3>(ref toGrow, "toGrow", LookMode.Deep);
			Scribe_Collections.Look<Thing>(ref toShed, "toShed", LookMode.Deep);
			Scribe_Values.Look(ref ticksToGrow, "ticksToGrow", 0f);
			Scribe_Values.Look(ref ticksToShed, "ticksToShed", 0f);
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostPostMake();
		}

		public override void CompTick()
		{
			if (!parent.Spawned)
            {
				return;
            }
			if (toGrow.Count > 0)
            {
				if (ticksToGrow <= 0)
                {
					GrowArmor();
					ticksToGrow = 10;
				}
				ticksToGrow--;
            }
			if (toShed.Count > 0)
            {
				if (ticksToShed <= 0)
                {
					ShedArmor();
					ticksToShed = 5;
				}
				ticksToShed--;
			}
		}

		public void GrowArmor()
        {
			List<IntVec3> toRemove = new List<IntVec3>();
			IEnumerable<IntVec3> batch = toGrow.Take(Rand.RangeInclusive(1, 3));
			foreach (IntVec3 c in batch)
            {
				if(((Building_ShipHeart)parent).body.RequestNutrition(55 * ((Building_ShipHeart)parent).getStatMultiplier("growthCost", null)))
                {
					Thing armor = ThingMaker.MakeThing(((Building_ShipHeart)parent).armorClass);
					armor.Position = c;
					armor.SetFaction(Faction.OfPlayer);
					armor.TryGetComp<CompShipBodyPart>().SetId(((Building_ShipHeart)parent).heartId);
					armor.SpawnSetup(((Building_ShipHeart)parent).Map, false);
					toRemove.Add(c);
				}
			}
			foreach(IntVec3 c in toRemove)
            {
				toGrow.Remove(c);
            }
		}

		public void ShedArmor()
        {
			List<Thing> toRemove = new List<Thing>();
			IEnumerable<Thing> batch = toShed.Take(Rand.RangeInclusive(1, 3));
			foreach(Thing t in batch)
            {
				t.Destroy();
				toRemove.Add(t);
            }
			foreach(Thing t in toRemove)
            {
				toShed.Remove(t);
            }
		}
	}

}
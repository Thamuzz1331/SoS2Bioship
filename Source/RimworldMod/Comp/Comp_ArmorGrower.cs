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
	public class CompArmorGrower : ThingComp
	{
		public CompProperties_ArmorGrower Props => (CompProperties_ArmorGrower)props;

		public HashSet<IntVec3> toGrow = new HashSet<IntVec3>();
		public HashSet<Thing> toShed = new HashSet<Thing>();
		public BuildingBody body = null;

		public ThingDef armorClass = null;

		private float ticksToGrow = 0f;
		private float ticksToShed = 0f;

		public override void PostExposeData()
		{
			base.PostExposeData();
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
			if (!parent.Spawned || body == null || body.heart == null)
            {
				return;
            }
			if (toGrow.Count > 0)
            {
				if (ticksToGrow <= 0)
                {
					GrowArmor();
					ticksToGrow = Props.growInterval / (body.heart.GetStat("metabolicSpeed") * body.heart.GetStat("armorGrowthSpeed"));
				}
            }
			if (toShed.Count > 0)
            {
				if (ticksToShed <= 0)
                {
					ShedArmor();
					ticksToShed = Props.shedInterval / (body.heart.GetStat("metabolicSpeed") * body.heart.GetStat("armorGrowthSpeed"));
				}
			}
			ticksToGrow--;
			ticksToShed--;
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}
			if (this.armorClass != null)
			{
				yield return new Command_Action
				{
					defaultLabel = "Grow Armor",
					action = delegate ()
					{
						this.ScheduleGrowArmor();
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "Shed Armor",
					action = delegate ()
					{
						this.ScheduleShedArmor();
					}
				};
			}
		}


		public virtual void ScheduleGrowArmor()
        {
			CompShipBodyPart bp = parent.TryGetComp<CompShipBodyPart>();
			if (this.body != null)
            {
				foreach(Thing t in this.body.bodyParts)
                {
					CompShipBodyPart tbp = t.TryGetComp<CompShipBodyPart>();
					if (tbp != null && tbp.ShipProps.growsArmor)
					{
						foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(t.Position))
						{
							if(c.GetThingList(t.Map).Count <= 0)
							{
								toGrow.Add(c);
							}
						}
					}
				}
			}
        }

		public virtual void ScheduleShedArmor()
		{
			if (this.body != null)
			{
				foreach (Thing t in this.body.bodyParts)
				{
					CompShipBodyPart tbp = t.TryGetComp<CompShipBodyPart>();
					if (tbp != null && tbp.ShipProps.isArmor)
                    {
						toShed.Add(t);
                    }
				}
			}
		}

		public void GrowArmor()
        {
			List<IntVec3> toRemove = new List<IntVec3>();
			IEnumerable<IntVec3> batch = toGrow.Take(Rand.RangeInclusive(1, 3));
			foreach (IntVec3 c in batch)
            {
				CompShipHeart heart = parent.TryGetComp<CompShipHeart>();
				if(heart != null && heart.body.RequestNutrition(2 / heart.GetStat("growthEfficiency")))
                {
					Thing armor = ThingMaker.MakeThing(armorClass);
					armor.Position = c;
					armor.SetFaction(parent.Faction);
					armor.TryGetComp<CompShipBodyPart>().SetId(heart.bodyId);
					armor.SpawnSetup(parent.Map, false);
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
using SaveOurShip2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using LivingBuildings;

namespace RimWorld
{
	public class CompArmorGrower : ThingComp
	{
		public CompProperties_ArmorGrower Props => (CompProperties_ArmorGrower)props;

		public HashSet<Tuple<IntVec3, ThingDef, Rot4, float>> toGrow = new HashSet<Tuple<IntVec3, ThingDef, Rot4, float>>();
		public HashSet<Thing> toShed = new HashSet<Thing>();
		public BuildingBody body = null;

		public List<ThingDef> armorClass = new List<ThingDef>();

		private float ticksToGrow = 0f;
		private float ticksToShed = 0f;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Collections.Look<Tuple<IntVec3, ThingDef, Rot4, float>>(ref toGrow, "toGrow", LookMode.Deep);
			Scribe_Collections.Look<Thing>(ref toShed, "toShed", LookMode.Deep);
			Scribe_Collections.Look<ThingDef>(ref armorClass, "armorClass");
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
			if (this.armorClass.Count > 0)
			{
				yield return new Command_Action
				{
					defaultLabel = "Grow Armor",
					action = delegate ()
                    {
						List<FloatMenuOption> options = new List<FloatMenuOption>();
						foreach(ThingDef opt in armorClass)
						{
							options.Add(new FloatMenuOption(opt.label,
								delegate() {
									ScheduleGrowArmor(opt);
								},
								MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
						}
						if (options.Count > 0)
						{
							FloatMenu menu = new FloatMenu(options);
							Find.WindowStack.Add(menu);
						}
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


		public virtual void ScheduleGrowArmor(ThingDef armor)
        {
			CompShipBodyPart bp = parent.TryGetComp<CompShipBodyPart>();
			if (this.body != null)
            {
				foreach(Thing t in this.body.bodyParts)
                {
					CompShipBodyPart tbp = t.TryGetComp<CompShipBodyPart>();
					if (tbp != null && tbp.ShipProps.growsArmor)
					{
						if (tbp.ShipProps.isCorner)
                        {
							IntVec3 v = new Vector3(t.Rotation.AsVector2.x - t.Rotation.AsVector2.y, 0, t.Rotation.AsVector2.y+t.Rotation.AsVector2.x).ToIntVec3();
							if (tbp.ShipProps.isFlip)
                            {
								if (t.Rotation.AsVector2.x > 0)
                                {
									v = new IntVec3(-1, 0, 1);
                                } else if (t.Rotation.AsVector2.x < 0)
                                {
									v = new IntVec3(1, 0, -1);
                                } else if (t.Rotation.AsVector2.y > 0)
                                {
									v = new IntVec3(-1, 0, -1);
                                } else if (t.Rotation.AsVector2.y < 0)
                                {
									v = new IntVec3(1, 0, 1);
								}                            
							}
							IntVec3 cornerLoc = t.Position + v;
							toGrow.Add(new Tuple<IntVec3, ThingDef, Rot4, float>(cornerLoc, t.def, t.Rotation, 1f));

							//TODO: Replace existing corner
						} else
                        {
							foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(t.Position))
							{
								if(c.GetThingList(t.Map).Count <= 0)
								{
									{
										toGrow.Add(new Tuple<IntVec3, ThingDef, Rot4, float>(c, armor, t.Rotation, 1f));
									}
								}
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
			List<Tuple<IntVec3, ThingDef, Rot4, float>> toRemove = new List<Tuple<IntVec3, ThingDef, Rot4, float>>();
			IEnumerable<Tuple<IntVec3, ThingDef, Rot4, float>> batch = toGrow.Take(Rand.RangeInclusive(1, 3));
			foreach (Tuple<IntVec3, ThingDef, Rot4, float> c in batch)
            {
				CompShipHeart heart = parent.TryGetComp<CompShipHeart>();
				if(heart != null && heart.body.RequestNutrition(2 / heart.GetStat("growthEfficiency")))
                {
					Thing armor = ThingMaker.MakeThing(c.Item2);
					armor.Position = c.Item1;
					armor.Rotation = c.Item3;
					armor.SetFaction(parent.Faction);
					armor.TryGetComp<CompShipBodyPart>().SetId(heart.bodyId);
					armor.SpawnSetup(parent.Map, false);
					toRemove.Add(c);
				}
			}
			foreach(Tuple<IntVec3, ThingDef, Rot4, float> c in toRemove)
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
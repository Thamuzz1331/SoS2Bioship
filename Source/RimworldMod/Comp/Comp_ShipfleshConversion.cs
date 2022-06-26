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
	class CompShipfleshConversion : ThingComp
	{
		protected CompProperties_ShipfleshConversion Props => (CompProperties_ShipfleshConversion)props;

		public Queue<Thing> toConvert = new Queue<Thing>();
		public Stack<Thing> toRegen = new Stack<Thing>();

		private ShipBody body = null;

		private int age;

		public int conversionWaitLength = 45;
		public int regenInterval = 45;

		private int ticksToConversion;
		private int ticksToRegen;
		private int ticksToDetectPulse;

		public float AgeDays => (float)age / 60000f;

		Dictionary<ThingDef, Tuple<ThingDef, bool, bool>> Conversions = new Dictionary<ThingDef, Tuple<ThingDef, bool, bool>>();
		Dictionary<ThingDef, Tuple<ThingDef, bool, bool>> Regenerations = new Dictionary<ThingDef, Tuple<ThingDef, bool, bool>>();
		Dictionary<ThingDef, Tuple<string, bool, bool>> MutableConversions = new Dictionary<ThingDef, Tuple<string, bool, bool>>();

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref age, "age", 0);
			Scribe_Values.Look(ref ticksToConversion, "ticksToConversion", 0);
			Scribe_Values.Look(ref ticksToDetectPulse, "ticksToDetectPulse", 0);
			Scribe_Values.Look(ref ticksToRegen, "ticks", 0);
//			Scribe_Collections.Look<Thing>(ref toRegen, "toRegen");
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostPostMake();
			Conversions.Add(ThingDef.Named("Scaffold_Beam"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("Bio_Ship_Beam"), false, false));
			Conversions.Add(ThingDef.Named("Scaffold_Beam_Unpowered"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("Scaffold_Beam_Unpowered"), false, false));
			Conversions.Add(ThingDef.Named("Scaffold_Corner_OneOne"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("Bio_Ship_Corner_OneOne"), false, false));
			Conversions.Add(ThingDef.Named("Scaffold_Corner_OneOneFlip"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("Bio_Ship_Corner_OneOneFlip"), false, false));
			Conversions.Add(ThingDef.Named("Scaffold_Corner_OneTwo"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("Bio_Ship_Corner_OneTwo"), false, false));
			Conversions.Add(ThingDef.Named("Scaffold_Corner_OneTwoFlip"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("Bio_Ship_Corner_OneTwoFlip"), true, true));
			Conversions.Add(ThingDef.Named("Scaffold_Corner_OneThree"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("Bio_Ship_Corner_OneThree"), false, false));
			Conversions.Add(ThingDef.Named("Scaffold_Corner_OneThreeFlip"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("Bio_Ship_Corner_OneThreeFlip"), true, false));
			Conversions.Add(ThingDef.Named("ScaffoldHullTile"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("BioShipHullTile"), false, false));
			Conversions.Add(ThingDef.Named("ScaffoldAirlock"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("BioShipAirlock"), false, false));
			Conversions.Add(ThingDef.Named("Scaffold_Engine_Small"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("BioShip_Engine_Small"), false, false));
			Conversions.Add(ThingDef.Named("Scaffold_Engine"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("BioShip_Engine"), false, false));
			Conversions.Add(ThingDef.Named("Scaffold_Engine_Large"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("BioShip_Engine_Large"), false, false));
			Conversions.Add(ThingDef.Named("FatStoreSmallScaffold"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("FatStoreSmall"), false, false));
			Conversions.Add(ThingDef.Named("ScaffoldInside_PassiveCooler"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("BioShipInside_PassiveCooler"), false, false));
			Conversions.Add(ThingDef.Named("ScaffoldShieldGenerator"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("BioShieldGenerator"), false, false));
			
			Regenerations.Add(ThingDef.Named("Bio_Ship_Beam"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("Scar_Beam"), false, false));
			Regenerations.Add(ThingDef.Named("Bio_Ship_Beam_Unpowered"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("Scar_Beam_Unpowered"), false, false));
			Regenerations.Add(ThingDef.Named("BioShipHullTile"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("ScarHullTile"), false, false));
			Regenerations.Add(ThingDef.Named("Scar_Beam"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("Scar_Beam"), false, false));
			Regenerations.Add(ThingDef.Named("ScarHullTile"), new Tuple<ThingDef, bool, bool>(ThingDef.Named("ScarHullTile"), false, false));

			MutableConversions.Add(ThingDef.Named("Scaffold_Maw_Small"), new Tuple<string, bool, bool>("smallMawOptions", false, false));
			MutableConversions.Add(ThingDef.Named("SmallWeaponScaffold"), new Tuple<string, bool, bool>("smallTurretOptions", false, false));
			MutableConversions.Add(ThingDef.Named("MediumWeaponScaffold"), new Tuple<string, bool, bool>("mediumTurretOptions", false, false));
			MutableConversions.Add(ThingDef.Named("LargeWeaponScaffold"), new Tuple<string, bool, bool>("largeTurretOptions", false, false));
		}

		public override void CompTick()
		{
			if (body == null)
				body = ((Building_ShipHeart)parent).body;
			if (!parent.Spawned || body == null)
			{
				return;
			}
			age++;
			ticksToDetectPulse--;
			ticksToConversion--;
			ticksToRegen--;
			if (ticksToDetectPulse <= 0)
            {
				ticksToDetectPulse = 300;
				DetectionPulse();
			}
			if (ticksToConversion <= 0)
			{
				ticksToConversion = conversionWaitLength;
				ConvertHullTile();
			}
			if (ticksToRegen <= 0)
            {
				ticksToRegen = regenInterval;
				DoRegen();
            }
		}

		private void DetectionPulse()
		{
			if (toConvert.Count > 0)
            {
				return;
            }
			if (body.shipFlesh.Count <= 0)
			{
				int startSpots = Rand.Range(4, 6);
				for (int s = 0; s < startSpots; s++)
                {
					IntVec3 c = parent.Position + (Rand.InsideUnitCircleVec3 * 3).ToIntVec3();
					foreach (Thing t in c.GetThingList(parent.Map))
					{
						if (Conversions.ContainsKey(t.def) || MutableConversions.ContainsKey(t.def))
						{
							toConvert.Enqueue(t);
							EnqueueSpur(t);
						}
					}
				}
			}
			/*
			else
            {
				foreach (Thing t in body.shipFlesh)
                {
					if (toConvert.Count > 250)
		            {
						return;
					}
					RandEnqueue(t);
				}
            }
			*/
		}

		private void RandEnqueue(Thing t)
        {
			if (toConvert.Count > 250)
            {
				return;
            }
			int sel = Rand.Range(1, 6);
			switch(sel)
            {
				case 5:
					EnqueueSpur(t);
					break;
				case 4:
					EnqueueSpur(t);
					break;
				default:
					EnqueueAdjacent(t);
					break;
			}
		}

		private void EnqueueAdjacent(Thing t)
        {

			foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(t.Position))
			{
				foreach (Thing adj in c.GetThingList(parent.Map))
				{
					if (Conversions.ContainsKey(adj.def) || MutableConversions.ContainsKey(adj.def))
					{
						toConvert.Enqueue(adj);
					}
				}
			}
		}

		private void EnqueueSpur(Thing t)
        {
			bool foundSomething = false;
			int numSpur = Rand.Range(1, 3);
			for (int i = 0; i < numSpur; i++)
            {
				int spurDepth = Rand.Range(3, 7);
				Vector3 vec = Rand.InsideUnitCircleVec3;
				bool cont = true;
				bool staple = false;
				for (int j = 2; j < spurDepth && cont && !staple; j++)
                {
					cont = false;
					IntVec3 c = t.Position + (vec * j).ToIntVec3();
					foreach (Thing adj in c.GetThingList(parent.Map))
					{
						if (Conversions.ContainsKey(adj.def))
						{
							toConvert.Enqueue(adj);
							cont = true;
							foundSomething = true;
						}
						if (adj.def == ThingDef.Named("NerveStaple"))
                        {
							staple = true;
                        }
					}
				}
			}
			if (!foundSomething)
            {
				EnqueueAdjacent(t);
			}
        }

		private void ConvertHullTile()
		{
			if (toConvert.Count <= 0)
            {
				return;
            }
			int numSpawn = GetNum();
			for (int i = 0; i < numSpawn; i++)
			{
				if (body.RequestNutrition(50)) { 
					Thing toReplace = null;
					bool searching = true;
					bool mutable = false;
					while (searching)
					{
						if (toConvert.Count <= 0)
							return;
						toReplace = toConvert.Dequeue();
						if ((Conversions.ContainsKey(toReplace.def) || MutableConversions.ContainsKey(toReplace.def)) && !toReplace.Destroyed)
						{
							searching = false;
							mutable = MutableConversions.ContainsKey(toReplace.def);
						}
					}
					bool spaceStapled = false;
					IntVec3 c = toReplace.Position;
					foreach (Thing t in toReplace.Position.GetThingList(parent.Map))
					{
		                if (t.def == ThingDef.Named("NerveStaple")) {
							spaceStapled = true;
							Log.Message("Staple found");
                        }
					}
					ThingDef replacementDef = null;
					bool item2 = false;
					bool item3 = false;
					if (mutable) {
						List<ThingDef> options = ((Building_ShipHeart)parent).organOptions[MutableConversions[toReplace.def].Item1];
						int selIndex = Rand.Range(0, options.Count);
						replacementDef = options[selIndex];
						item2 = MutableConversions[toReplace.def].Item2;
						item3 = MutableConversions[toReplace.def].Item3;
					}
					else {
						replacementDef = Conversions[toReplace.def].Item1;
						item2 = Conversions[toReplace.def].Item2;
						item3 = Conversions[toReplace.def].Item3;
					}

					if (spaceStapled && Building_NerveStaple.Conversions.ContainsKey(replacementDef))
                    {
						replacementDef = Building_NerveStaple.Conversions[replacementDef];
                    }
					Thing replacement = ThingMaker.MakeThing(replacementDef);

					CompShipBodyPart bodyPart = ((ThingWithComps)replacement).GetComp<CompShipBodyPart>();
					if(bodyPart != null)
					{
						bodyPart.SetId(((Building_ShipHeart)parent).heartId);
						body.Register(bodyPart);
					}
					CompShipNutrition nutrition = ((ThingWithComps)replacement).GetComp<CompShipNutrition>();
					if (nutrition != null)
					{
						nutrition.SetId(((Building_ShipHeart)parent).heartId);
						body.Register(nutrition);
					}
					replacement.Rotation = item2 ? toReplace.Rotation.Opposite : toReplace.Rotation;
					replacement.Position = toReplace.Position + (item3 ? IntVec3.South.RotatedBy(replacement.Rotation) : IntVec3.Zero);
					replacement.SetFaction(Faction.OfPlayer);
					TerrainDef terrain = parent.Map.terrainGrid.TerrainAt(c);
					parent.Map.terrainGrid.RemoveTopLayer(c, false);
					toReplace.Destroy();
					replacement.SpawnSetup(parent.Map, false);
					if (!spaceStapled)
                    {
						RandEnqueue(replacement);
                    }
					if (terrain != CompRoofMe.hullTerrain)
						parent.Map.terrainGrid.SetTerrain(c, terrain);
					if (replacement.def == ThingDef.Named("BioShipHullTile") 
						&& ((Building_ShipHeart)parent).body.source.Count < 4
						&& replacement.Position.GetThingList(parent.Map).Count < 2)
                    {
						MawSpawn(replacement);
                    }
				}	
			}

		}

		private void MawSpawn(Thing hull)
        {
			Thing newMaw = ThingMaker.MakeThing(ThingDef.Named("Maw_Small"));
			newMaw.Position = hull.Position;
			newMaw.SetFaction(Faction.OfPlayer);
			CompShipBodyPart bodyPart = ((ThingWithComps)newMaw).GetComp<CompShipBodyPart>();
			CompShipNutrition nutrition = ((ThingWithComps)newMaw).GetComp<CompShipNutrition>();
			bodyPart.SetId(((Building_ShipHeart)parent).heartId);
			nutrition.SetId(((Building_ShipHeart)parent).heartId);
			body.Register(bodyPart);
			body.Register(nutrition);
			newMaw.SpawnSetup(parent.Map, false);
        }

		private void DoRegen()
        {
			if (toRegen.Count > 0)
            {
				int numHeal = GetNum();
				if (numHeal > toRegen.Count)
                {
					numHeal = toRegen.Count;
                }
				for (int i = 0; i < numHeal; i++)
                {
					if (body.RequestNutrition(100)) {
						Thing toReplace = toRegen.Pop();
						if (Regenerations.ContainsKey(toReplace.def))
						{
							IntVec3 c = toReplace.Position;
							Thing replacement = ThingMaker.MakeThing(Regenerations[toReplace.def].Item1);
							CompShipBodyPart bodyPart = ((ThingWithComps)replacement).GetComp<CompShipBodyPart>();
							if(bodyPart != null)
							{
								bodyPart.SetId(((Building_ShipHeart)parent).heartId);
								body.Register(bodyPart);
							}
							replacement.Rotation = Regenerations[toReplace.def].Item2 ? toReplace.Rotation.Opposite : toReplace.Rotation;
							replacement.Position = toReplace.Position + (Regenerations[toReplace.def].Item3 ? IntVec3.South.RotatedBy(replacement.Rotation) : IntVec3.Zero);
							replacement.SetFaction(Faction.OfPlayer);
							TerrainDef terrain = parent.Map.terrainGrid.TerrainAt(c);
												parent.Map.terrainGrid.RemoveTopLayer(c, false);
							replacement.SpawnSetup(parent.Map, false);
							if (terrain != CompRoofMe.hullTerrain)
								parent.Map.terrainGrid.SetTerrain(c, terrain);
						}
                    }
                }
            }
        }

		private int GetNum()
        {
			int ret = Rand.Range(1, 20);
			if (ret > 16)
            {
				ret = 3;
            } else if (ret > 12)
            {
				ret = 2;
            } else
            {
				ret = 1;
            }
			return ret;
        }

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
			List<Gizmo> newList = new List<Gizmo>();
			newList.AddRange(base.CompGetGizmosExtra());
			return newList;
        }
	}
	
}
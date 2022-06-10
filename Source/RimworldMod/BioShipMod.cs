using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HugsLib;
using RimWorld;
using RimWorld.Planet;
using SaveOurShip2;
using Verse;
using Verse.AI;
using Verse.Sound;
using HarmonyLib;
using System.Text;
using UnityEngine;
using HugsLib.Utils;
using Verse.AI.Group;
using HugsLib.Settings;
using RimWorld.QuestGen;
using RimworldMod;
using System.Net;
using System.IO;
using RimworldMod.VacuumIsNotFun;
using System.Collections;
using System.Reflection.Emit;
using UnityEngine.SceneManagement;

namespace BioShip
{
	[StaticConstructorOnStartup]
	public class BioShip : ModBase
	{
		public override void Initialize()
		{
			base.Initialize();
		}
	}


	[HarmonyPatch(typeof(ShipUtility), "LaunchFailReasons")]
	public static class FindLaunchFailReasons
	{
		[HarmonyPostfix]
		public static void FindLaunchFailReasonsReally(Building rootBuilding, ref IEnumerable<string> __result)
		{
			List<string> newResult = new List<string>();
			List<Building> shipParts = ShipUtility.ShipBuildingsAttachedTo(rootBuilding);
			bool huntingEngines = true;
			bool huntingCockpit = true;
			bool huntingSensors = true;
			bool hasPilot = false;
			float fuelNeeded = 0f;
			float fuelHad = 0f;
			using (List<Building>.Enumerator enumerator = shipParts.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Building part = enumerator.Current;
					bool flag = huntingEngines;
					if (flag)
					{
						huntingEngines = !FindLaunchFailReasons.engines.Any((ThingDef d) => d == part.def);
					}
					bool flag2 = huntingCockpit;
					if (flag2)
					{
						huntingCockpit = !FindLaunchFailReasons.pilots.Any((ThingDef d) => d == part.def);
						bool flag3 = !huntingCockpit;
						if (flag3)
						{
							CompMannable manable = part.TryGetComp<CompMannable>();
							bool flag4 = manable != null;
							if (flag4)
							{
								huntingCockpit = (huntingCockpit || !manable.MannedNow);
							}
							CompPowerTrader powerTrader = part.TryGetComp<CompPowerTrader>();
							bool flag5 = powerTrader != null;
							if (flag5)
							{
								huntingCockpit = (huntingCockpit || !powerTrader.PowerOn);
							}
						}
						hasPilot = !huntingCockpit;
					}
					bool flag6 = huntingSensors;
					if (flag6)
					{
						huntingSensors = !FindLaunchFailReasons.engines.Any((ThingDef d) => d == part.def);
					}
					int fuelMult = FindLaunchFailReasons.liftoffPower.TryGetValue(part.def, 0);
					bool flag7 = part.TryGetComp<CompRefuelable>() != null;
					if (flag7)
					{
						fuelHad += part.TryGetComp<CompRefuelable>().Fuel * (float)fuelMult;
					}
					bool flag8 = !FindLaunchFailReasons.hullPlates.Any((ThingDef d) => d == part.def);
					if (flag8)
					{
						fuelNeeded += (float)(part.def.size.x * part.def.size.z) * 3f;
					}
					else
					{
						fuelNeeded += 1f;
					}
				}
			}
			bool flag9 = huntingEngines;
			if (flag9)
			{
				newResult.Add("ShipReportMissingPart".Translate(Array.Empty<NamedArgument>()) + ": " + ThingDefOf.Ship_Engine.label);
			}
			bool flag10 = huntingCockpit;
			if (flag10)
			{
				List<string> list = newResult;
				string str = "ShipReportMissingPart".Translate(Array.Empty<NamedArgument>()) + ": ";
				ThingDef thingDef = ThingDef.Named("ShipPilotSeat");
				list.Add(str + ((thingDef != null) ? thingDef.ToString() : null));
			}
			bool flag11 = huntingSensors;
			if (flag11)
			{
				newResult.Add("ShipReportMissingPart".Translate(Array.Empty<NamedArgument>()) + ": " + ThingDefOf.Ship_SensorCluster.label);
			}
			bool flag12 = fuelHad < fuelNeeded;
			if (flag12)
			{
				newResult.Add("ShipNeedsMoreChemfuel".Translate(fuelHad, fuelNeeded));
			}
			bool flag13 = !hasPilot;
			if (flag13)
			{
				newResult.Add("ShipReportNeedPilot".Translate(Array.Empty<NamedArgument>()));
			}
			__result = newResult;
		}

		private static List<ThingDef> engines = new List<ThingDef>
		{
			ThingDefOf.Ship_Engine,
			ThingDef.Named("Ship_Engine_Small"),
			ThingDef.Named("Ship_Engine_Large"),
			ThingDef.Named("BioShip_Engine"),
			ThingDef.Named("BioShip_Engine_Small"),
			ThingDef.Named("BioShip_Engine_Large")
		};

		private static List<ThingDef> sensors = new List<ThingDef>
		{
			ThingDefOf.Ship_SensorCluster,
			ThingDef.Named("Ship_SensorClusterAdv"),
			ThingDef.Named("BioShip_SensorCluster")
		};

		private static List<ThingDef> pilots = new List<ThingDef>
		{
			ThingDef.Named("ShipPilotSeat"),
			ThingDef.Named("ShipPilotSeatMini"),
			ThingDefOf.Ship_ComputerCore,
			ThingDef.Named("ShipArchotechSpore"),
			ThingDef.Named("Ship_Heart")
		};

		private static List<ThingDef> hullPlates = new List<ThingDef>
		{
			ThingDef.Named("ShipHullTile"),
			ThingDef.Named("ShipHullTileMech"),
			ThingDef.Named("ShipHullTileArchotech"),
			ThingDef.Named("BioShipHullTile")
		};

		private static Dictionary<ThingDef, int> liftoffPower = new Dictionary<ThingDef, int>
		{
			{
				ThingDefOf.Ship_Engine,
				1
			},
			{
				ThingDef.Named("Ship_Engine_Small"),
				1
			},
			{
				ThingDef.Named("BioShip_Engine"),
				1
			},
			{
				ThingDef.Named("BioShip_Engine_Small"),
				1
			},
			{
				ThingDef.Named("Ship_Engine_Large"),
				2
			},
			{
				ThingDef.Named("BioShip_Engine_Large"),
				2
			}
		};

		[HarmonyPatch(typeof(Building_ShipBridge), "InterstellarFailReasons")]
		public static class BioShipInterstellarFail
		{
			[HarmonyPrefix]
			public static void ReasonsNotToFly(ref List<string> __result)
			{
			}
		}
	}
}

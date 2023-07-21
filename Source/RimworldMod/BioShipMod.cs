using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using SaveOurShip2;
using Verse;
using Verse.AI;
using Verse.Sound;
using HarmonyLib;
using System.Text;
using UnityEngine;
using Verse.AI.Group;
using RimWorld.QuestGen;
using RimworldMod;
using System.Net;
using System.IO;
using System.Collections;
using System.Reflection.Emit;
using UnityEngine.SceneManagement;
using LivingBuildings;

namespace BioShip
{
	[StaticConstructorOnStartup]
	static class Setup
	{
		static Setup()
		{
			Harmony pat = new Harmony("SoS2Bioship");

			//Legacy methods. All 3 of these could technically be merged
			BioShip.Initialize(pat);
			BioShip.DefsLoaded();
			pat.PatchAll();
		}
	}

	public class BioShip : Mod
	{

		public BioShip(ModContentPack content) : base(content)
		{
		}

		public static void Initialize(Harmony pat)
		{
		}

		public static void DefsLoaded()
		{
			Log.Message("Bioship Loaded");
		}

		private static Type shipCombatManagerType = AccessTools.TypeByName("ShipCombatManager");

		public static IntVec3 FindBurstLocation(CompShipCombatShield shield, LocalTargetInfo target)
        {
			Map sourceMap = Traverse.Create(shipCombatManagerType).Field("PlayerShip").GetValue<Map>();
			if (shield.parent.Map == sourceMap)
            {
				return (IntVec3)AccessTools.Method(shipCombatManagerType, "FindClosestEdgeCell").Invoke(null, new object[] {
					Traverse.Create(shipCombatManagerType).Field("EnemyShip").GetValue<Map>(),
					(object)target.Cell
				});
			}
			else
            {
				return (IntVec3)AccessTools.Method(shipCombatManagerType, "FindClosestEdgeCell").Invoke(null, new object[] {
					sourceMap,
					(object)target.Cell
				});

			}
		}

		public static void ReflectShot(CompShipCombatShield shield, Projectile_ExplosiveShipCombat proj)
        {
			Vector3 origin = Traverse.Create(proj).Field("origin").GetValue<Vector3>();
			int ticksToImpact = Traverse.Create(proj).Field("ticksToImpact").GetValue<int>();
			Traverse.Create(proj).Field("ticksToImpact").SetValue(ticksToImpact + 2);
			Vector3 returnPoint = proj.ExactPosition;
			LocalTargetInfo localTarget = new LocalTargetInfo(origin.ToIntVec3());

			Building_ShipTurret fakeTurret = (Building_ShipTurret)GenSpawn.Spawn(ThingDef.Named("Phantom_Turret"), returnPoint.ToIntVec3(), proj.Map);
			Projectile returnFire = (Projectile)GenSpawn.Spawn(proj.def, returnPoint.ToIntVec3(), proj.Map);
			returnFire.Launch(proj.Launcher,
				returnPoint,
				localTarget,
				localTarget,
				ProjectileHitFlags.All,
				equipment: proj.Launcher);

			object[] parameters = new object[]{
					fakeTurret,
					new LocalTargetInfo(proj.Launcher.Position),
					proj.def,
					0f,
					FindBurstLocation(shield, new LocalTargetInfo(proj.Launcher.Position))
				};
			AccessTools.Method(shipCombatManagerType, "RegisterProjectile").Invoke(null, parameters);
			fakeTurret.Destroy();
		}
	}

	[HarmonyPatch(typeof(Building_ShipBridge), "InterstellarFailReasons")]
	public static class BioshipInterstellarFailReasons
    {

		[HarmonyPrefix]
		public static bool BioshipFailReasons(Building_ShipBridge __instance, ref List<string> __result)
        {
			__result = new List<string>();
			if (__instance.TryGetComp<CompShipHeart>() != null)
            {
				__result.Add("Bioship FTL Pending");
				return false;
            } 
			return true;
        }
    }

	[HarmonyPatch(typeof(ColonistBar), "ColonistBarOnGUI")]
	public static class BioShipCombatOnGUI
	{
		private static Type shipCombatManagerType = AccessTools.TypeByName("ShipCombatManager");

		[HarmonyPostfix]
		public static void DrawNutritionBars(ColonistBar __instance)
		{
			Map mapPlayer = Find.Maps.Where(m => m.GetComponent<ShipHeatMapComp>().InCombat && !m.GetComponent<ShipHeatMapComp>().ShipCombatMaster).FirstOrDefault();
			if (mapPlayer != null)
            {
				float screenHalf = (float)UI.screenWidth / 2 + SaveOurShip2.ModSettings_SoS.offsetUIx;

				float baseY = __instance.Size.y + SaveOurShip2.ModSettings_SoS.offsetUIy;

				MapCompBuildingTracker bioTracker = mapPlayer.GetComponent<MapCompBuildingTracker>();
				if (bioTracker != null)
                {
					float maxNutrion = 0;
					float curNutrion = 0;
					foreach (BuildingBody body in bioTracker.bodies.Values)
					{
						maxNutrion += body.nutritionCapacity;
						curNutrion += body.currentNutrition;
					}
					baseY += 45;
					Rect rect2 = new Rect(screenHalf - 700, baseY, 205, 35);
					Verse.Widgets.DrawMenuSection(rect2);

					Rect rect3 = new Rect(screenHalf - 1200, baseY, 200, 35);
					Widgets.FillableBar(rect3.ContractedBy(6), curNutrion / maxNutrion,
						ResourceBank.NutrientTex);
					Text.Font = GameFont.Small;
					rect3.y += 7;
					rect3.x = screenHalf - 615;
					rect3.height = Text.LineHeight;
					if (maxNutrion > 0)
						Widgets.Label(rect3, "Nutrition: " + Mathf.Round(curNutrion) + " / " + maxNutrion);
					else
						Widgets.Label(rect3, "<color=red>Nutrition: N/A</color>");
                }
            }
		}	
	}


	[HarmonyPatch(typeof(CompRefuelable), "Refuel", new Type[] {typeof(List<Thing>)})]
	public static class ButcherableScalingRefuel
    {
		[HarmonyPrefix]
		public static bool RefuelPrefix(CompRefuelable __instance, List<Thing> fuelThings)
        {
			if (!(__instance is CompButcherableScallingRefuelable))
            {
				return true;
            }
			foreach(Thing t in fuelThings)
            {
				if(t is Corpse)
                {
					foreach(Thing bt in ((Corpse)t).InnerPawn.ButcherProducts(null, 1.25f))
					{
						__instance.Refuel(bt.stackCount);
					}
				}
				t.Destroy(DestroyMode.Vanish);
            }
			return false;
        }
    }
}

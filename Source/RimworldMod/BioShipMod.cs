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
//using RimworldMod;
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

		public static void ReflectShot(CompShipHeatShield shield, Projectile proj)
        {
			Log.Message("Reflecting");

			Vector3 origin = Traverse.Create(proj).Field("origin").GetValue<Vector3>();
			int ticksToImpact = Traverse.Create(proj).Field("ticksToImpact").GetValue<int>();
			Traverse.Create(proj).Field("ticksToImpact").SetValue(ticksToImpact + 2);
			Vector3 returnPoint = proj.ExactPosition;
			LocalTargetInfo localTarget = new LocalTargetInfo(origin.ToIntVec3());
			Building_ShipTurret sourceTurret = (Building_ShipTurret)proj.Launcher;
			IntVec3 burstLoc = sourceTurret.Map.GetComponent<ShipMapComp>().FindClosestEdgeCell(sourceTurret.Map, sourceTurret.Position);
			ThingDef returnProjSpawnDef = null;
			ThingDef d = proj.Launcher.def.building?.turretGunDef;
			Log.Message("Launcher Type " + d.defName);
			if (d != null)
            {
				foreach(VerbProperties vp in d.Verbs)
                {
					returnProjSpawnDef = vp.spawnDef;
                }
            } else
            {
				return;
            }

			Building_ShipTurret fakeTurret = (Building_ShipTurret)GenSpawn.Spawn(ThingDef.Named("Phantom_Turret"), returnPoint.ToIntVec3(), proj.Map);
			Projectile returnFire = (Projectile)GenSpawn.Spawn(proj.def, returnPoint.ToIntVec3(), proj.Map);
			returnFire.Launch(proj.Launcher,
				returnPoint,
				localTarget,
				localTarget,
				ProjectileHitFlags.All,
				equipment: proj.Launcher);

			ShipCombatProjectile retProj = new ShipCombatProjectile
			{
				turret = fakeTurret,
				target = new LocalTargetInfo(proj.Launcher.Position),
				range = 0,
				spawnProjectile = proj.def,
				missRadius = 0,
				accBoost = 0,
				burstLoc = burstLoc,
				speed = sourceTurret.heatComp.Props.projectileSpeed,
				Map = shield.parent.Map
			};
			ShipMapComp mC = shield.parent.Map.GetComponent<ShipMapComp>();
			mC.Projectiles.Add(retProj);
			fakeTurret.Destroy();
		}
	}
    /*
		[HarmonyPatch(typeof(CompShipBlueprint), "SpawnShipDefBlueprint")]
		public static class BioshipInterstellarFailReasons
		{

			[HarmonyPrefix]
			public static bool BioshipSpawn(ShipDef __shipDef, IntVec3 __pos, Map __map, int __tier)
			{

				return true;
			}
		}
	*/
    /*
        [HarmonyPatch(typeof(SelfDefenseUtility), nameof(SelfDefenseUtility.ShouldStartFleeing))]
        public static class JellyNotFlee
        {
            [HarmonyPostfix]
            public static void IgnoreFlee(Pawn pawn, ref bool __result)
            {
                Log.Message("Testing Should Flee " + pawn.def.defName);
                if (pawn.def.defName == "BioShip_VoidJelly")
                {
                    Log.Message("Jelly detected");
                    __result = false;
                }
            }
        }
    */
    [HarmonyPatch(typeof(ShipUtility), nameof(ShipUtility.LaunchFailReasons))]
    public static class BioshipLaunch
    {

        [HarmonyPostfix]
        public static void Postfix(IEnumerable<string> __result, Building rootBuilding)
        {
			if (rootBuilding is Building_ShipBridge)
			{
				Building_ShipBridge bridge = (Building_ShipBridge)rootBuilding;
                if (bridge.Ship.Sensors.Count > 0)
				{
					__result = __result.Where((s) => !(s.Contains("sensor")));
				}
            }
        }
    }

    [HarmonyPatch(typeof(ColonistBar), "ColonistBarOnGUI")]
	public static class BioShipCombatOnGUI
	{
		private static Type shipCombatManagerType = AccessTools.TypeByName("ShipCombatManager");

		[HarmonyPostfix]
		public static void DrawNutritionBars(ColonistBar __instance)
		{
			/*
			Map mapPlayer = Find.Maps.Where(m => m.GetComponent<ShipHeatMapComp>().InCombat && !m.GetComponent<ShipHeatMapComp>().ShipCombatOrigin).FirstOrDefault();
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
            }*/
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
						float adjustedCount = bt.stackCount * 1.25f;
						__instance.Refuel(adjustedCount);
					}
				}
				t.Destroy(DestroyMode.Vanish);
            }
			return false;
        }
    }
}

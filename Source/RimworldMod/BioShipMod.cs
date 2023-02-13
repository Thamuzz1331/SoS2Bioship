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
using LivingBuildings;


namespace BioShip
{
	[StaticConstructorOnStartup]
	public class BioShip : ModBase
	{
		public static HugsLib.Utils.ModLogger instLogger;

		public static Texture2D NutrientTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.5f, 0.5f, 0.1f));
		public static Texture2D MutationBackground = SolidColorMaterials.NewSolidColorTexture(new Color(0.5f, 0.5f, 0.1f));

		static BioShip() { }

		public override string ModIdentifier
		{
			get { return "BioShip"; }
		}


		public override void Initialize()
		{
			base.Initialize();
		}

		public override void DefsLoaded()
		{
			base.DefsLoaded();
			Log.Message("Bioship Loaded");
		}

/*
		public static List<ThingDef> shipHullDefs = new List<ThingDef>()
        {
			ThingDef.Named("ShipHullTile"),
			ThingDef.Named("ShipHullTileMech"),
			ThingDef.Named("ShipHullTileArchotech"),
			ThingDef.Named("ScarHullTile"),
			ThingDef.Named("ScaffoldHullTile"),
			ThingDef.Named("BioShipHullTile"),
        };
*/

		private static Type shipCombatManagerType = AccessTools.TypeByName("ShipCombatManager");

		public static ThingDef GetAndRegisterProjectile(Building_ShipTurret turret, Verb_Shoot verb)
        {
			if (turret == null)
            {
				return null;
            }
			ThingDef projectileDef = null;
			object[] parameters;
			if (turret.gun.TryGetComp<CompChangeableProjectilePlural>() != null) {
				parameters = new object[]{
					turret,
					Traverse.Create(verb).Field("shipTarget").GetValue<LocalTargetInfo>(),
					turret.gun.TryGetComp<CompChangeableProjectilePlural>().Projectile.interactionCellIcon,
					0f,
					turret.SynchronizedBurstLocation
				};
				projectileDef = turret.gun.TryGetComp<CompChangeableProjectilePlural>().Projectile;
            } else if (turret.TryGetComp<CompMutableAmmo>() != null)
            {
				parameters = new object[]{
					turret,
					Traverse.Create(verb).Field("shipTarget").GetValue<LocalTargetInfo>(),
					turret.TryGetComp<CompMutableAmmo>().GetProjectileDef(),
					1.9f,
					turret.SynchronizedBurstLocation
				};
				projectileDef = turret.TryGetComp<CompMutableAmmo>().GetFakeProjectileDef();
            }
			else
            {
				parameters = new object[]{
					turret,
					Traverse.Create(verb).Field("shipTarget").GetValue<LocalTargetInfo>(),
					verb.verbProps.spawnDef,
					1.9f,
					turret.SynchronizedBurstLocation
				};
				projectileDef = verb.verbProps.defaultProjectile;
            }
			AccessTools.Method(shipCombatManagerType, "RegisterProjectile").Invoke(null, parameters);
			return projectileDef;
        }

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

		public static int GetThrust(Tuple<CompEngineTrail, CompRefuelable, CompFlickable> engine)
        {
			if (engine.Item1.parent.TryGetComp<CompShipBodyPart>() != null && engine.Item1.parent.TryGetComp<CompShipBodyPart>().CoreSpawned)
            {
				return (int)Math.Round(engine.Item1.Props.thrust * engine.Item1.parent.TryGetComp<CompShipBodyPart>().Core.GetStat("movementSpeed"));
            }
			return engine.Item1.Props.thrust;
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

				float baseY = __instance.Size.y + 40 + SaveOurShip2.ModSettings_SoS.offsetUIy;

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
					Rect rect2 = new Rect(screenHalf - 630, baseY, 205, 35);
					Verse.Widgets.DrawMenuSection(rect2);

					Rect rect3 = new Rect(screenHalf - 630, baseY, 200, 35);
					Widgets.FillableBar(rect3.ContractedBy(6), curNutrion / maxNutrion,
						BioShip.NutrientTex);
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
/*
	[HarmonyPatch(typeof(SaveShip), "MoveShip")]
	public static class ExpandFloorListPatch
    {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			bool replacementMade = false;
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
            {
				if (replacementMade)
                {
					if (codes[i].opcode == OpCodes.Ldloc_3)
                    {
						return codes;
                    } else
                    {
						codes[i].opcode = OpCodes.Nop;
                    }
                } else
                {
					LocalBuilder operandString = codes[i].operand as LocalBuilder;
					if(codes.Count > i+8)
					{
						if (codes[i].opcode == OpCodes.Ldloc_S && operandString.LocalType == typeof(Building) && codes[i+7].opcode == OpCodes.Brfalse)
						{
							codes[i+6] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BioShip), "IsShipTerrain", new Type[]{typeof(TerrainDef)}));
							replacementMade = true;
							i = i+7;
						}
					}
				}
            }

			return codes;
		}
    }

	[HarmonyPatch("Verb_LaunchProjectileShip", "TryCastShot")]
	public static class AddVariableLaunchPatch
    {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			bool replacementMade = false;
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
            {
				if (replacementMade)
                {
					if (codes[i].opcode == OpCodes.Stloc_1)
                    {
						return codes;
                    } else
                    {
						codes[i].opcode = OpCodes.Nop;
                    }
                } else
                {
					if (codes[i].opcode == OpCodes.Isinst && (codes[i].operand.ToString() == "RimWorld.Building_ShipTurret")) //&& operandString.LocalType == typeof(Building_ShipTurret))
					{
						codes[i+4] = new CodeInstruction(OpCodes.Ldloc_0);
						codes[i+5] = new CodeInstruction(OpCodes.Ldarg_0);
						codes[i+6] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BioShip), "GetAndRegisterProjectile", new Type[]{typeof(Building_ShipTurret), typeof(Verb_Shoot)}));
						replacementMade = true;
						i=i+6;
					}
				}
            }

			return codes;
		}
    }

	[HarmonyPatch(typeof(CompShipCombatShield), "CompTick")]
	public static class FractionalShieldPatch
    {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			bool replacementMade = false;
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
            {
				if (replacementMade)
                {
					if (codes[i].opcode == OpCodes.Sub)
                    {
						codes[i].opcode = OpCodes.Nop;
						codes[i+1].opcode = OpCodes.Nop;
						return codes;
                    } else
                    {
						codes[i].opcode = OpCodes.Nop;
                    }
                } else
                {

					if (codes[i].opcode == OpCodes.Ble_Un_S)
					{
						codes[i-3] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BioShip), "RoundShield", new Type[]{typeof(CompShipCombatShield)}));
						replacementMade = true;
						i=i-3;
					}
				}
            }

			return codes;
		}
    }

	[HarmonyPatch(typeof(CompShipCombatShield), "HitShield")]
	public static class ShieldThermalMultPatch
    {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
            {
				if (codes[i].opcode == OpCodes.Ldc_R4 && codes[i].operand.ToString() == "0.75")
				{
					codes[i-5].opcode = OpCodes.Ldarg_0;
					codes[i-4].opcode = OpCodes.Ldarg_1;
					codes[i-3] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BioShip), "HeatMultiplier", new Type[]{typeof(CompShipCombatShield), typeof(Projectile_ExplosiveShipCombat)}));
					codes[i-2].opcode = OpCodes.Ldloc_0;
					codes[i-1].opcode = OpCodes.Mul;
					codes[i].opcode = OpCodes.Stloc_0;
					codes[i+1].opcode = OpCodes.Nop;
					codes[i+2].opcode = OpCodes.Nop;
					return codes;
				}
            }

			return codes;
		}
    }

	[HarmonyPatch("ShipCombatManager", "SalvageEverything")]
	public static class SalvageBayPatch
    {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
            {
				if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString() == "Verse.WindowStack get_WindowStack()")
				{
					codes[i] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BioShip), "OpenSalvageWindow"));
					for (int j = i+1; j < codes.Count; j++)
                    {
						if (codes[j].opcode == OpCodes.Ret)
                        {
							return codes;
                        }
						codes[j].opcode = OpCodes.Nop;
                    }
				}
            }

			return codes;
		}
    }

	[HarmonyPatch(typeof(Projectile_ExplosiveShipCombat), "Tick")]
	public static class ProjExplodePatch
    {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
            {
				if (codes[i].opcode == OpCodes.Isinst && codes[i].operand.ToString() == "RimWorld.Projectile_ExplosiveShipCombatPsychic")
				{
					codes[i+3] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BioShip), "ShouldExplode"));
					for (int j = i+6; j < codes.Count; j++)
                    {
						if (codes[j].opcode == OpCodes.Callvirt && j != i+6)
                        {
							return codes;
                        }
						codes[j].opcode = OpCodes.Nop;
                    }
				}
            }

			return codes;
		}
    }

	[HarmonyPatch("ShipCombatManager", "ShipAITick")]
	public static class SpeedPatch
    {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
            {
				if (codes[i].opcode == OpCodes.Ldsfld && (codes[i].operand.ToString() == "System.Int32 playerEngineRot" 
					|| codes[i].operand.ToString() == "System.Int32 enemyEngineRot"))
				{
					codes[i+2] = new CodeInstruction(OpCodes.Call, 
						AccessTools.Method(typeof(BioShip), "EngineFacing", 
							new Type[]{typeof(int), typeof(Tuple<CompEngineTrail, CompRefuelable, CompFlickable>)}));
					codes[i+3].opcode = OpCodes.Nop;
					codes[i+4].opcode = OpCodes.Nop;
					codes[i+5].opcode = OpCodes.Nop;
					codes[i+6].opcode = OpCodes.Nop;
					codes[i+7].opcode = OpCodes.Nop;
					codes[i+8].opcode = OpCodes.Brfalse;

					codes[i+10] = new CodeInstruction(OpCodes.Call, 
						AccessTools.Method(typeof(BioShip), "GetThrust", 
							new Type[]{typeof(Tuple<CompEngineTrail, CompRefuelable, CompFlickable>)}));
					codes[i+11].opcode = OpCodes.Nop;
					codes[i+12].opcode = OpCodes.Nop;
				}
            }

			return codes;
		}
    }

	[HarmonyPatch(typeof(Building_ShipBridge), "RecalcStats")]
	public static class CalcStats
    {
		[HarmonyPrefix]
		public static bool BioshipFailReasons(Building_ShipBridge __instance)
        {
            __instance.ShipThreat = 0;
            __instance.ShipMass = 0;
            __instance.ShipMaxTakeoff = 0;
            __instance.ShipThrust = 0;
            foreach (Building b in (List<Building>)AccessTools.Field(typeof(Building_ShipBridge), "cachedShipParts").GetValue(__instance))
            {
				bool bodyPartWithHeart = (b.TryGetComp<CompShipBodyPart>() != null && b.TryGetComp<CompShipBodyPart>().HeartSpawned);
                if (BioShip.shipHullDefs.Any(bDef => bDef == b.def))
                {
                    __instance.ShipMass += 1;
                } else 
                {
                    __instance.ShipMass += (b.def.size.x * b.def.size.z) * 3;
                    if (b.TryGetComp<CompShipHeat>() != null)
                        __instance.ShipThreat += b.TryGetComp<CompShipHeat>().Props.threat;
                    else if (b.def == ThingDef.Named("ShipSpinalAmplifier"))
                        __instance.ShipThreat += 5;
                    if (b.TryGetComp<CompEngineTrail>() != null)
                    {
						if (bodyPartWithHeart)
                        {
							 __instance.ShipThrust += b.TryGetComp<CompEngineTrail>().Props.thrust * b.TryGetComp<CompShipBodyPart>().body.heart.GetStat("movementSpeed");
                        } else
                        {
	                        __instance.ShipThrust += b.TryGetComp<CompEngineTrail>().Props.thrust;
                        }
                        __instance.ShipMaxTakeoff += b.TryGetComp<CompRefuelable>().Props.fuelCapacity;
                        //nuclear counts x2
                        if (b.TryGetComp<CompRefuelable>().Props.fuelFilter.AllowedThingDefs.Contains(ThingDef.Named("ShuttleFuelPods")))
                        {
                            __instance.ShipMaxTakeoff += b.TryGetComp<CompRefuelable>().Props.fuelCapacity;
                        }
                    }
                }
            }
            __instance.ShipThrust *= 500f / Mathf.Pow(((List<Building>)AccessTools.Field(typeof(Building_ShipBridge), "cachedShipParts").GetValue(__instance)).Count, 1.1f);
            __instance.ShipThreat += __instance.ShipMass / 100;
            if(__instance.TryGetComp<CompShipHeat>()!=null)
            {
                ShipHeatNet net = __instance.TryGetComp<CompShipHeat>().myNet;
                __instance.ShipHeat = net.StorageUsed;
                __instance.ShipHeatCap = net.StorageCapacity;
            }			
			return false;
        }
	}*/
}

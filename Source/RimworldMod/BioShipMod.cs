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
		public static Texture2D NutrientTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.5f, 0.5f, 0.1f));
		public static Texture2D MutationBackground = SolidColorMaterials.NewSolidColorTexture(new Color(0.5f, 0.5f, 0.1f));

		public override string ModIdentifier
		{
			get { return "BioShip"; }
		}


		public override void Initialize()
		{
			base.Initialize();
			//var original = typeof(ShipUtility).GetMethod("LaunchFailReasons");
			//HarmonyInst.Unpatch(original, HarmonyPatchType.All, "ShipInteriorMod2");
		}

		public static List<TerrainDef> shipTerrainDefs = new List<TerrainDef>()
		{
			DefDatabase<TerrainDef>.GetNamed("FakeFloorShipflesh"),
			DefDatabase<TerrainDef>.GetNamed("FakeFloorShipscar"),
			DefDatabase<TerrainDef>.GetNamed("FakeFloorShipwhithered"),
			DefDatabase<TerrainDef>.GetNamed("FakeFloorInsideShip"),
			DefDatabase<TerrainDef>.GetNamed("ShipWreckageTerrain"),
			DefDatabase<TerrainDef>.GetNamed("FakeFloorInsideShipMech"),
			DefDatabase<TerrainDef>.GetNamed("FakeFloorInsideShipArchotech"),
			DefDatabase<TerrainDef>.GetNamed("FakeFloorInsideShipFoam"),
		};

		public static bool IsShipTerrain(TerrainDef tDef)
		{
			return (tDef.layerable && !shipTerrainDefs.Contains(tDef));
		}

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

		public static void RoundShield(CompShipCombatShield shield)
        {
			float absDiff = Math.Abs(shield.radius - shield.radiusSet);
			if (absDiff > 0 && absDiff < 1)
            {
				Log.Message("Rounding shield");
				shield.radius = shield.radiusSet;
            }
			else if (shield.radiusSet > shield.radius)
                shield.radius+=1f;
            else if (shield.radiusSet < shield.radius)
                shield.radius-=1f;
        }

		public static float HeatMultiplier(CompShipCombatShield shield, Projectile_ExplosiveShipCombat proj)
        {
			float ret = 1f;
			if (shield.Props.archotech)
            {
				ret *= 0.75f;
            }
			CompBuildingBodyPart bodyPart = shield.parent.TryGetComp<CompBuildingBodyPart>();
			if (bodyPart != null && bodyPart.body != null && bodyPart.body.heart != null)
            {
				ret = ret/(0.75f * bodyPart.body.heart.GetStat("conciousness"));
            }
			if (proj is Projectile_ShieldBatteringProjectile)
            {
				ret *=  2f;
            }
			return ret;
        }
	}

	[HarmonyPatch(typeof(ShipUtility), "LaunchFailReasons")]
	public static class FindLaunchFailReasonsBioship
	{
		[HarmonyPostfix]
		public static void FindLaunchFailReasonsReallyBioship(Building rootBuilding, ref IEnumerable<string> __result)
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
					CompEngineTrail engineTrail = part.TryGetComp<CompEngineTrail>();
					if (engineTrail != null)
					{
						if (part.TryGetComp<CompRefuelable>() != null)
                        {
							fuelHad += part.TryGetComp<CompRefuelable>().Fuel;
                        }
						huntingEngines = false;
					}
					if (huntingCockpit)
					{
						if (part is Building_ShipBridge)
                        {
							bool functioning = true;
							CompMannable manable = part.TryGetComp<CompMannable>();
							if (manable != null)
                            {
								functioning = functioning && manable.MannedNow;
                            }
							if (part.TryGetComp<CompPowerTrader>() != null)
                            {
								functioning = functioning && part.TryGetComp<CompPowerTrader>().PowerOn;
                            }
							huntingCockpit = !functioning;
							hasPilot = functioning;
                        }
					}
					if (huntingSensors)
					{
						huntingSensors = !FindLaunchFailReasonsBioship.sensors.Any((ThingDef d) => d == part.def);
					}
					if (!FindLaunchFailReasonsBioship.hullPlates.Any((ThingDef d) => d == part.def))
					{
						fuelNeeded += (float)(part.def.size.x * part.def.size.z) * 3f;
					}
					else
					{
						fuelNeeded += 1f;
					}
				}
			}
			if (huntingEngines)
			{
				newResult.Add("ShipReportMissingPart".Translate(Array.Empty<NamedArgument>()) + ": " + ThingDefOf.Ship_Engine.label);
			}
			if (huntingCockpit)
			{
				string str = "ShipReportMissingPart".Translate(Array.Empty<NamedArgument>()) + ": ";
				ThingDef thingDef = ThingDef.Named("ShipPilotSeat");
				newResult.Add(str + ((thingDef != null) ? thingDef.ToString() : null));
			}
			if (huntingSensors)
			{
				newResult.Add("ShipReportMissingPart".Translate(Array.Empty<NamedArgument>()) + ": " + ThingDefOf.Ship_SensorCluster.label);
			}
			if (fuelHad < fuelNeeded)
			{
				newResult.Add("ShipNeedsMoreChemfuel".Translate(fuelHad, fuelNeeded));
			}
			if (!hasPilot)
			{
				newResult.Add("ShipReportNeedPilot".Translate(Array.Empty<NamedArgument>()));
			}
			__result = newResult;
		}

		private static List<ThingDef> sensors = new List<ThingDef>
		{
			ThingDefOf.Ship_SensorCluster,
			ThingDef.Named("Ship_SensorClusterAdv"),
			ThingDef.Named("BioShip_SensorCluster")
		};

		private static List<ThingDef> hullPlates = new List<ThingDef>
		{
			ThingDef.Named("ShipHullTile"),
			ThingDef.Named("ShipHullTileMech"),
			ThingDef.Named("ShipHullTileArchotech"),
			ThingDef.Named("BioShipHullTile")
		};
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

	[HarmonyPatch(typeof(ShipCombatOnGUI), "DrawShipRange")]
	public static class BioShipCombatOnGUI
	{
		private static Type shipCombatManagerType = AccessTools.TypeByName("ShipCombatManager");

		[HarmonyPostfix]
		public static void DrawNutritionBars(ref float baseY)
		{
			if(Traverse.Create(shipCombatManagerType).Field("InCombat").GetValue<bool>())
            {
				Map playerShip = Traverse.Create(shipCombatManagerType).Field("PlayerShip").GetValue<Map>();
				foreach(Thing h in playerShip.listerBuildings.allBuildingsColonist.Where(b => b.TryGetComp<CompShipHeart>() != null)) {
					CompShipHeart heart = h.TryGetComp<CompShipHeart>();
					Rect rect = new Rect(UI.screenWidth - 255, baseY - 40, 250, 40);
					Verse.Widgets.DrawMenuSection(rect);
					Widgets.FillableBar(rect.ContractedBy(6), heart.body.currentNutrition / heart.body.nutritionCapacity,
					BioShip.NutrientTex);

					rect.y += 10;
					rect.x = UI.screenWidth - 200;
					rect.height = Text.LineHeight;

					Widgets.Label(rect, "Nutrition: " + Mathf.Round(heart.body.currentNutrition));

					baseY -= 50;
                    
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
}

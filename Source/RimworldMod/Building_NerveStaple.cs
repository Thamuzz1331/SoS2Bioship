using HarmonyLib;
using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimworldMod;


namespace RimWorld
{
    public class Building_NerveStaple : Building
    {
        public static Dictionary<ThingDef, ThingDef> Conversions = new Dictionary<ThingDef, ThingDef>() {
            {ThingDef.Named("Bio_Ship_Beam"), ThingDef.Named("Bio_Ship_Beam_Insensate")},
            {ThingDef.Named("Bio_Ship_Beam_Unpowered"), ThingDef.Named("Bio_Ship_Beam_Unpowered_Insensate")},
            {ThingDef.Named("Bio_Ship_Corner_OneOne"), ThingDef.Named("Bio_Ship_Corner_OneOne_Insensate")},
            {ThingDef.Named("Bio_Ship_Corner_OneOneFlip"), ThingDef.Named("Bio_Ship_Corner_OneOneFlip_Insensate")},
            {ThingDef.Named("Bio_Ship_Corner_OneTwo"), ThingDef.Named("Bio_Ship_Corner_OneTwo_Insensate")},
            {ThingDef.Named("Bio_Ship_Corner_OneTwoFlip"), ThingDef.Named("Bio_Ship_Corner_OneTwoFlip_Insensate")},
            {ThingDef.Named("Bio_Ship_Corner_OneThree"), ThingDef.Named("Bio_Ship_Corner_OneThree_Insensate")},
            {ThingDef.Named("Bio_Ship_Corner_OneThreeFlip"), ThingDef.Named("Bio_Ship_Corner_OneThreeFlip_Insensate")},
            {ThingDef.Named("BioShipHullTile"), ThingDef.Named("BioShipHullTile_Insensate")},
            {ThingDef.Named("BioShipAirlock"), ThingDef.Named("BioShipAirlock_Insensate")},
        };

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            List<Thing> toDestroy = new List<Thing>();
            foreach (Thing t in this.Position.GetThingList(map))
            {
                if (Conversions.ContainsKey(t.def))
                {
                    toDestroy.Add(t);
                }
            }
            foreach (Thing t in toDestroy)
            {
                Thing replacement = ThingMaker.MakeThing(Conversions[t.def]);
                replacement.Position = t.Position;
                replacement.Rotation = t.Rotation;
                replacement.SetFaction(Faction.OfPlayer);
                TerrainDef terrain = map.terrainGrid.TerrainAt(t.Position);
                map.terrainGrid.RemoveTopLayer(t.Position, false);
                t.Destroy();
                replacement.SpawnSetup(map, false);
                if (terrain != CompRoofMe.hullTerrain)
                    map.terrainGrid.SetTerrain(replacement.Position, terrain);
            }

        }
    }
}

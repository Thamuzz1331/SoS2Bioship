using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using HarmonyLib;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompRoofMeBio : CompRoofMe
    {
        public CompProperties_RoofMeBio BioProps
        {
            get
            {
                return (CompProperties_RoofMeBio)this.props;
            }
        }
        public static GraphicData bioRoofedData = new GraphicData();
        public static Graphic roofTileBio;

        static CompRoofMeBio()
        {
            bioRoofedData.texPath = "Terrain/Surfaces/FleshRoof";
            bioRoofedData.graphicClass = typeof(Graphic_Single);
            bioRoofedData.shaderType = ShaderTypeDefOf.MetaOverlay;
            roofTileBio = new Graphic_256_Bio(bioRoofedData.Graphic);
        }


        private static Type compRoofMeType = AccessTools.TypeByName("CompRoofMe");

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            //base.PostSpawnSetup(respawningAfterLoad);
            
            TerrainDef hullTerrain = DefDatabase<TerrainDef>.GetNamed(BioProps.TerrainId);
            List<IntVec3> positions = new List<IntVec3>();
            foreach (IntVec3 pos in GenAdj.CellsOccupiedBy(parent))
            {
                positions.Add(pos);
            }
            Log.Message("!" + Traverse.Create(this).Field("positions").GetValue<List<IntVec3>>().Count);
            Traverse.Create(this).Field("positions").SetValue(positions);
            Traverse.Create(this).Field("map").SetValue(parent.Map);
            Log.Message("#" + Traverse.Create(this).Field("positions").GetValue<List<IntVec3>>().Count);
            foreach (IntVec3 pos in positions)
            {
                if(Props.roof)
                    parent.Map.roofGrid.SetRoof(pos, CompRoofMe.roof);
                TerrainDef currentTerrain = parent.Map.terrainGrid.TerrainAt(pos);
                if (parent.Map.terrainGrid.TerrainAt(pos) == CompRoofMe.hullTerrain)
                {
                    parent.Map.terrainGrid.RemoveTopLayer(pos, false);
                }
                if (base.Props.roof)
                    parent.Map.roofGrid.SetRoof(pos, roof);
                if (!CompRoofMeBio.IsShipTerrain(currentTerrain) && currentTerrain != hullTerrain)
                {
                    parent.Map.terrainGrid.SetTerrain(pos, hullTerrain);
                }
            }
        }

        public override void PostDestroy(DestroyMode m, Map previousMap)
        {
            base.PostDestroy(m, previousMap);
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if ((Find.PlaySettings.showRoofOverlay || parent.Map.fogGrid.fogGrid[parent.Map.cellIndices.CellToIndex(parent.Position)]) && parent.Position.GetFirstThing<Building_ShipTurret>(parent.Map) == null)
            {
                Graphics.DrawMesh(material: roofTileBio.MatSingleFor(parent), mesh: roofTileBio.MeshAt(parent.Rotation), position: new UnityEngine.Vector3(parent.DrawPos.x, 0, parent.DrawPos.z), rotation: Quaternion.identity, layer: 0);
            }
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
    }
}
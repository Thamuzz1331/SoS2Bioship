using SaveOurShip2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
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

        List<IntVec3> positions = new List<IntVec3>();
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            TerrainDef hullTerrain = DefDatabase<TerrainDef>.GetNamed(BioProps.TerrainId);

            foreach (IntVec3 pos in GenAdj.CellsOccupiedBy(parent))
            {
                positions.Add(pos);
            }
            foreach (IntVec3 pos in positions)
            {
                if (base.Props.roof)
                    parent.Map.roofGrid.SetRoof(pos, roof);
                TerrainDef currentTerrain = parent.Map.terrainGrid.TerrainAt(pos);
                if (currentTerrain != hullTerrain)
                {
                    parent.Map.terrainGrid.SetTerrain(pos, hullTerrain);
                }
            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if ((Find.PlaySettings.showRoofOverlay || parent.Map.fogGrid.fogGrid[parent.Map.cellIndices.CellToIndex(parent.Position)]) && parent.Position.GetFirstThing<Building_ShipTurret>(parent.Map) == null)
            {
                Graphics.DrawMesh(material: roofTileBio.MatSingleFor(parent), mesh: roofTileBio.MeshAt(parent.Rotation), position: new UnityEngine.Vector3(parent.DrawPos.x, 0, parent.DrawPos.z), rotation: Quaternion.identity, layer: 0);
            }
        }

    }
}
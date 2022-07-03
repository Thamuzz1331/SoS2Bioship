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

        public static Graphic roofedGraphicTile;

        static CompRoofMeBio()
        {
            roofedData.texPath = "Terrain/Surfaces/FleshRoof";
            roofedData.graphicClass = typeof(Graphic_Single);
            roofedData.shaderType = ShaderTypeDefOf.MetaOverlay;
            roofedGraphicTile = new Graphic_256(roofedData.Graphic);
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
                parent.Map.terrainGrid.SetTerrain(pos, hullTerrain);
            }
        }
    }
}
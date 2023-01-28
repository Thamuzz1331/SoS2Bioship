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
            base.PostSpawnSetup(true);
            if (respawningAfterLoad)
            {
                return;
            }
            TerrainDef hullTerrain = DefDatabase<TerrainDef>.GetNamed(BioProps.TerrainId);
            foreach (IntVec3 pos in GenAdj.CellsOccupiedBy(parent))
            {
                if(!parent.Map.terrainGrid.TerrainAt(pos).layerable)
                {
                    TerrainDef currentTerrain = parent.Map.terrainGrid.TerrainAt(pos);
                    parent.Map.terrainGrid.SetTerrain(pos, hullTerrain);
                }
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (!Props.roof)
                return;
            if ((Find.PlaySettings.showRoofOverlay || parent.Position.Fogged(parent.Map)) && 
                parent.Position.Roofed(parent.Map))
            {
                Graphics.DrawMesh(material: roofTileBio.MatSingleFor(parent), mesh: roofTileBio.MeshAt(parent.Rotation), position: new UnityEngine.Vector3(parent.DrawPos.x, 0, parent.DrawPos.z), rotation: Quaternion.identity, layer: 0);
            }
        }
    }
}
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
    public class CompRoofMeBio : CompSoShipPart
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

        public override void PostDraw()
        {
            base.PostDraw();
            if (!Props.roof || !BioProps.isBioTile)
                return;
            if ((Find.PlaySettings.showRoofOverlay || parent.Position.Fogged(parent.Map)) && 
                parent.Position.Roofed(parent.Map))
            {
                foreach (Thing t in parent.Position.GetThingList(parent.Map))
                {
                    if (t.TryGetComp<CompShipHeat>() != null && t.def.altitudeLayer == AltitudeLayer.WorldClipper)
                    {
                        return;
                    }
                }
                Graphics.DrawMesh(material: roofTileBio.MatSingleFor(parent), mesh: roofTileBio.MeshAt(parent.Rotation), position: new UnityEngine.Vector3(parent.DrawPos.x, 0, parent.DrawPos.z), rotation: Quaternion.identity, layer: 0);
            }
        }

        public override void SetShipTerrain(IntVec3 v)
        {
            Map map = parent.Map;
            if (!map.terrainGrid.TerrainAt(v).layerable)
            {
                map.terrainGrid.SetTerrain(v, DefDatabase<TerrainDef>.GetNamed(BioProps.TerrainId));
            }
        }
    }
}
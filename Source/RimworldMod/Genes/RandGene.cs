using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using Verse;
using RimWorld;
using LivingBuildings;

namespace Verse
{
    public class RandGene : BuildingGene
    {
        public override void PostAdd(CompBuildingCore core)
        {
            string gene = this.def.props.RandomElement();
            BuildingGene rGene = BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(gene), false);
            core.AddGene(rGene);
            core.RemoveGene(this);
        }
    }
}
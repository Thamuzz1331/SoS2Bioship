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
    public class HediffGiverGene : BuildingGene
    {
        public override void PostAdd(CompBuildingCore core)
        {
            base.PostAdd(core);
            CompShipHeart heart = (CompShipHeart)core;
            
            foreach (String fieldDef in def.props)
            {
                string[] fd = fieldDef.Split(':');
                string[] opts = fd[1].Split(',');
                foreach (String o in opts)
                {
                    BuildingHediffDef diffDef = BuildingHediffDef.Named(o);
                    heart.AddHediff(BuildingHediffMaker.MakeBuildingHediff(diffDef));
                }
            }
        }

        public override void PostRemove(CompBuildingCore core)
        {
            base.PostRemove(core);
            if (!this.Overridden)
            {
                CompShipHeart heart = (CompShipHeart)core;
                foreach (String fieldDef in def.props)
                {
                    string[] fd = fieldDef.Split(':');
                    string[] opts = fd[1].Split(',');
                    foreach (String o in opts)
                    {
                        BuildingHediffDef diffDef = BuildingHediffDef.Named(o);
                        heart.RemoveHediff(BuildingHediffMaker.MakeBuildingHediff(diffDef).DisplayLabel);
                    }
                }


            }
        }

        public override bool OverridesGene(BuildingGene b)
        {
            bool ret = base.OverridesGene(b);
            foreach (string tag in this.def.tags)
            {
                ret = ret || b.def.tags.Contains(tag);
            }
            return ret;
        }

    }
}

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
    public class DefSetterGene : BuildingGene
    {


        public override void PostAdd(CompBuildingCore core)
        {
            base.PostAdd(core);
            CompShipHeart heart = (CompShipHeart)core;
            foreach (String fieldDef in def.props)
            {
                string[] fd = fieldDef.Split(':');
                List<ThingDef> newOpts = new List<ThingDef>(new List<ThingDef>());
                string[] opts = fd[1].Split(',');
                foreach (String o in opts)
                {
                    newOpts.Add(ThingDef.Named(o));
                }
                heart.defs.SetOrAdd(fd[0], new DefOptions(newOpts));
            }
        }

        public override void PostRemove(CompBuildingCore core)
        {
            base.PostRemove(core);
            Log.Message("Gene " + this.label + " " + this.Overridden);
            if (!this.Overridden)
            {
                CompShipHeart heart = (CompShipHeart)core;
                foreach (String fieldDef in def.props)
                {
                    string[] fd = fieldDef.Split(':');
                    heart.defs.SetOrAdd(fd[0], new DefOptions(new List<ThingDef>()));
                }
            }
        }

        public override bool OverridesGene(BuildingGene b)
        {
            bool ret = base.OverridesGene(b);
            if(this.Equals(b))
            {
                return false;
            }
            foreach(string tag in this.def.tags)
            {
                ret = ret || b.def.tags.Contains(tag);
            }
			return ret;
        }

    }
}

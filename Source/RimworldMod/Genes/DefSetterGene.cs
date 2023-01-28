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
            CompShipHeart heart = (CompShipHeart)core;
            foreach (String fieldDef in def.props)
            {
                string[] fd = fieldDef.Split(':');
                heart.defs.SetOrAdd(fd[0], new DefOptions(new List<ThingDef>() { ThingDef.Named(fd[1]) }));
            }
        }

        public override void PostRemove(CompBuildingCore core)
        {
            CompShipHeart heart = (CompShipHeart)core;
            foreach (String fieldDef in def.props)
            {
                string[] fd = fieldDef.Split(':');
                heart.defs.SetOrAdd(fd[0], new DefOptions(new List<ThingDef>()));
            }
        }
    }
}

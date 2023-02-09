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
    public class StatSetterGene : BuildingGene
    {


        public override void PostAdd(CompBuildingCore core)
        {
            CompShipHeart heart = (CompShipHeart)core;
            foreach (String statdDef in def.props)
            {
                string[] fd = statdDef.Split(':');
                float stat = heart.stats.TryGetValue(fd[0], 1f) * float.Parse(fd[1]);
                heart.stats.SetOrAdd(fd[0], stat);
            }
        }

        public override void PostRemove(CompBuildingCore core)
        {
            CompShipHeart heart = (CompShipHeart)core;
            foreach (String statdDef in def.props)
            {
                string[] fd = statdDef.Split(':');
                float stat = heart.stats.TryGetValue(fd[0], 1f) * (1/float.Parse(fd[1]));
                heart.stats.SetOrAdd(fd[0], stat);
            }
        }
    }
}

using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;


namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompButcherableScallingRefuelable : CompRefuelable
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            ((CorpseMawTracker)this.parent.Map.components.Where(t => t is CorpseMawTracker).FirstOrDefault()).corpseMaws.Add(parent);
        }

        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            base.PostDeSpawn(map);
            ((CorpseMawTracker)map.components.Where(t => t is CorpseMawTracker).FirstOrDefault()).corpseMaws.Remove(parent);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

        }
    }
}

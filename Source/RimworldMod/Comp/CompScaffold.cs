using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompScaffold : ThingComp
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(parent.Position))
            {
                foreach (Thing adj in c.GetThingList(parent.Map))
                {
                    if (adj is ThingWithComps)
                    {
                        CompShipBodyPart flesh = ((ThingWithComps)adj).TryGetComp<CompShipBodyPart>();
                        if (flesh != null)
                        {
                            flesh.AddScaff(parent);
                        }
                    }
                }
            }
        }
    }
}
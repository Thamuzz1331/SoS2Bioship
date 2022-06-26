using HarmonyLib;
using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimworldMod;


namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class Building_Scaffold : Building
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(this.Position))
            {
                foreach (Thing adj in c.GetThingList(map))
                {
                    if (adj is ThingWithComps )
                    {
                        CompShipBodyPart flesh = ((ThingWithComps)adj).TryGetComp<CompShipBodyPart>();
                        if (flesh != null)
                        {
                            flesh.AddScaff(this);
                        }
                    }
                }
            }

        }
    }
}

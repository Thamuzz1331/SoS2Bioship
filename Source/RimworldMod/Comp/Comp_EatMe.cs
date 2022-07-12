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
    public class CompEatMe : ThingComp
    {
        private CompProperties_EatMe Props => (CompProperties_EatMe)props;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            CompShipBodyPart bodyPart = ((ThingWithComps)parent).TryGetComp<CompShipBodyPart>();
            if (((ThingWithComps)parent).TryGetComp<CompScaffold>() != null) {
                return;
            }
            foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(parent.Position))
            {
                foreach (Thing adj in c.GetThingList(parent.Map))
                {
                    if (adj is ThingWithComps)
                    {
                        CompShipBodyPart flesh = ((ThingWithComps)adj).TryGetComp<CompShipBodyPart>();
                        if (flesh != null)
                        {
                            ShipBody body = flesh.body;
                            if (bodyPart != null)
                            {
                                if (bodyPart.heartId != flesh.heartId)
                                {
                                    if (body != null)
                                    {
                                        body.otherFlesh.Add(parent);
                                    } else
                                    {
                                        flesh.adjBodypart.Add(parent);
                                    }
                                }
                            } else
                            {
                                if (body != null)
                                {
                                    body.adjacentMechanicals.Add(parent);
                                }
                                else
                                {
                                    flesh.adjMechs.Add(parent);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
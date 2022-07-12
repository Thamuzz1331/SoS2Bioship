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
    public class CompShipBodyPart : CompBuildingBodyPart
    {
        CompProperties_ShipBodyPart ShipProps => (CompProperties_ShipBodyPart)props;
        public HashSet<Thing> adjMechs = new HashSet<Thing>();
        public HashSet<Thing> adjBodypart = new HashSet<Thing>();

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(parent.Position))
            {
                foreach (Thing adj in c.GetThingList(parent.Map))
                {
                    if (adj is ThingWithComps)
                    {
                        CompEatMe eatMe = ((ThingWithComps)adj).TryGetComp<CompEatMe>();
                        if (eatMe != null)
                        {
                            CompShipBodyPart bodyPart = ((ThingWithComps)adj).TryGetComp<CompShipBodyPart>();
                            if (bodyPart != null)
                            {
                                if (bodyPart.bodyId != bodyId)
                                {
                                    body.otherFlesh.Add(adj);
                                }
                            }
                            else
                            {
                                body.adjacentMechanicals.Add(adj);
                            }
                        }
                    }
                }
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (mode == DestroyMode.KillFinalize && Props.regen)
            {
                ((CompShipHeart)body.heart).Regen(parent);
            }
        }

        public override string CompInspectStringExtra()
        {
            return "Flesh of " + heartId;
        }

        public bool IsArmor()
        {
            return Props.isArmor;
        }
    }
}
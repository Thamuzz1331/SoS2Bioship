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
    public class CompEatMe : ThingComp
    {
        private CompProperties_EatMe Props => (CompProperties_EatMe)props;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            CompShipBodyPart bodyPart = parent.TryGetComp<CompShipBodyPart>();
            if (parent.TryGetComp<CompScaffold>() != null) {
                return;
            }
            foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(parent.Position))
            {
                foreach (Thing adj in c.GetThingList(parent.Map))
                {
                    CompShipBodyPart flesh = adj.TryGetComp<CompShipBodyPart>();
                    if (flesh != null)
                    {
                        BuildingBody body = flesh.body;
                        if (bodyPart != null)
                        {
                            if (bodyPart.bodyId != flesh.bodyId)
                            {
                                if (body != null && body.heart != null)
                                {
                                    ((CompShipHeart)body.heart).AggressionTarget(parent, false);
                                } else
                                {
                                    flesh.adjBodypart.Add(parent);
                                }
                            }
                        } else
                        {
                            if (body != null && body.heart != null)
                            {
                                ((CompShipHeart)body.heart).AggressionTarget(parent, true);
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
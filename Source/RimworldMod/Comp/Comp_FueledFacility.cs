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
    public class CompFueledFacility : CompFacility
    {
        private CompProperties_FueledFacility FueledProps => (CompProperties_FueledFacility)props;

        CompRefuelable refuelable;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            refuelable = parent.TryGetComp<CompRefuelable>();

        }

        public override void CompTick()
        {
            base.CompTick();
            float refuelRate = 0f;
            foreach (Thing heart in this.LinkedBuildings)
            {
                refuelRate += heart.TryGetComp<CompShipHeart>().body.bodyParts.Count * 0.01f;
                if (refuelable.HasFuel)
                {
                    heart.TryGetComp<CompShipHeart>().luciferiumAddiction = true;
                    heart.TryGetComp<CompShipHeart>().luciferiumSupplied = true;
                }
                else
                {
                    heart.TryGetComp<CompShipHeart>().luciferiumSupplied = false;
                }
            }
            refuelable.Props.fuelConsumptionRate = refuelRate;
        }
    }
}
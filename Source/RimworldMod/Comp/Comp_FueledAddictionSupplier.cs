using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class CompFueledAddictionSupplier : CompAddictionSupplier
    {
        CompRefuelable refuelable;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            refuelable = parent.TryGetComp<CompRefuelable>();
        }

        public override bool CanSupply()
        {
            return refuelable.HasFuel;
        }

        public override void SetConsumption(float consumptionRate)
        {
            refuelable.Props.fuelConsumptionRate = consumptionRate;
        }

    }
}
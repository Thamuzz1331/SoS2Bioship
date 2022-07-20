using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using SaveOurShip2;

namespace RimWorld
{
    public class CompNutritionRefueler : CompNutritionConsumer
    {
        private CompProperties_NutritionRefueler Props => (CompProperties_NutritionRefueler)props;

        private float ticksTillRefuel = 0;
        private CompRefuelable refuelable = null;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksTillRefuel, "ticksTillRefuel", 0);
        }

        public override float getConsumptionPerPulse()
        {
            if (refuelable != null && refuelable.FuelPercentOfTarget < 1)
            {
                return Props.refuelCost;
            }
            return 0f;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (refuelable == null)
            {
                refuelable = parent.TryGetComp<CompRefuelable>();
            }
            if (refuelable != null && ticksTillRefuel <= 0 && refuelable.FuelPercentOfTarget < 1.0f)
            {
                refuelable.Refuel(Props.refuelAmmount);
                ticksTillRefuel = Props.refuelInterval;
            }
            ticksTillRefuel--;
        }
    }
}
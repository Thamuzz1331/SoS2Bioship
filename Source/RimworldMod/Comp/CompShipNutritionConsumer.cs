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
    public class CompShipNutritionConsumer : CompShipNutrition
    {
        private CompProperties_ShipNutritionConsumer Props => (CompProperties_ShipNutritionConsumer)props;
        public virtual float getConsumptionPerPulse()
        {
            if (body != null && body.heart != null)
            {
                return (Props.consumptionPerPulse * body.heart.getStatMultiplier("consumptionPerPulse", parent.def));
            }
            return Props.consumptionPerPulse;
        }
    }
}
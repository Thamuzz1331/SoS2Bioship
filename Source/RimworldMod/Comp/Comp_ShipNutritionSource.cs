using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using LivingBuildings;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompShipNutritionSource : CompRefuelableNutritionSource
    {
        public float efficiency = 1f;
        public override float getNutritionPerPulse()
        {
            return (base.getNutritionPerPulse() * efficiency);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class Building_BioShipTurret : SaveOurShip2.Building_ShipTurret
    {
        public CompShipBodyPart bodyPart = null;
        public override void BeginBurst()
        {
            Log.Message("!");
            if (bodyPart == null)
            {
                bodyPart = this.TryGetComp<CompShipBodyPart>();
            }
            if (bodyPart.body.currentNutrition > bodyPart.ShipProps.turretNutritionCost)
            {
                base.BeginBurst();
                if (AttackVerb.state == VerbState.Bursting)
                {
                    bodyPart.body.RequestNutrition(bodyPart.ShipProps.turretNutritionCost);
                }
            }
        }

        public override float BurstCooldownTime()
        {
            if (bodyPart == null)
            {
                bodyPart = this.TryGetComp<CompShipBodyPart>();
            }
            Log.Message("Base Cooldown " + base.BurstCooldownTime());
            Log.Message("Metabolic Eff " + bodyPart.body.heart.GetStat("metabolicSpeed"));
            Log.Message("Expected Cooldown " + base.BurstCooldownTime() / bodyPart.body.heart.GetStat("metabolicSpeed"));
            return (base.BurstCooldownTime() /  bodyPart.body.heart.GetStat("metabolicSpeed"));
        }
    }
}
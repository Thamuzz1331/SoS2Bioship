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
    public class CompNutritionLoader : CompShipNutritionConsumer
    {
        private CompProperties_NutritionLoader Props => (CompProperties_NutritionLoader)props;

        private CompChangeableProjectilePlural toReload = null;
        private float ticksTillLoad = 0;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksTillLoad, "ticksTillLoad", 0);
        }

        public override float getConsumptionPerPulse()
        {
            return 0;
        }

        public override void CompTick()
        {
            if (ticksTillLoad <= 0)
            {
                if (toReload == null)
                {
                    toReload = ((Building_ShipTurret)parent).gun.TryGetComp<CompChangeableProjectilePlural>();
                }
                if (toReload != null && !toReload.FullyLoaded)
                {
                    float cost = Props.reloadCost;
                    if (body != null && body.heart != null)
                    {
                        cost = cost * body.heart.getStatMultiplier("spineReloadCost", parent.def);
                        if (body.RequestNutrition(cost))
                        {
                            toReload.LoadShell(ThingDef.Named(Props.ammoCat), 1);
                        }
                        ticksTillLoad = Props.reloadInterval.SecondsToTicks() * body.heart.getStatMultiplier("spineReloadInterval", parent.def);
                    }
                }
            }
            ticksTillLoad--;
        }
    }
}
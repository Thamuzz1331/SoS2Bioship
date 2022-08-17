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
    public class CompNutritionLoader : CompNutritionConsumer
    {
        private CompProperties_NutritionLoader Props => (CompProperties_NutritionLoader)props;

        private CompChangeableProjectilePlural toReload = null;
        private float ticksTillLoad = 0;
        public List<int> torpSpawn = new List<int>() { 
            1,
        };

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
                if (toReload != null && !toReload.FullyLoaded && body != null && body.heart != null)
                {
                    float cost = Props.reloadCost;
                    if (body != null && body.heart != null)
                    {
                        cost = cost / body.heart.GetStat("metabolicEfficiency");
                        if (body.RequestNutrition(cost))
                        {
                            ThingDef torpDef = ((CompShipHeart)body.heart).GetThingDef(parent.def.defName);
                            int loadCount = torpSpawn.RandomElement();
                            for (int i = 0; i < loadCount; i++)
                            {
                                toReload.LoadShell(torpDef, 1);
                            }
                        }
                        ticksTillLoad = Props.reloadInterval.SecondsToTicks() * body.heart.GetMultiplier("spineReloadInterval");
                        ticksTillLoad *= (1.0f + (Rand.Range(-15, 15) / 100.0f));
                    }
                }
            }
            ticksTillLoad--;
        }
    }
}
using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HarmonyLib;


namespace RimWorld
{

    public class CompUnfoldTwitch : UnfoldComponent
    {
        private float ticksToTwitchInterval = 0;
        private float twitchInterval = 180;
        private const float twitchDuration = 15f;
        
        private static Type unfoldType = AccessTools.TypeByName("UnfoldComponent");
        float twitchCountdown = 10f;
        bool isTwtiching = false;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.ticksToTwitchInterval, "ticksToTwitchInterval", 0f);
            Scribe_Values.Look(ref this.twitchCountdown, "twitchCountdown", twitchDuration);
            Scribe_Values.Look(ref this.isTwtiching, "isTwtiching", false);
        }

        public override void CompTick()
        {
            float extension = Traverse.Create(this).Field("extension").GetValue<float>();
            if (isTwtiching)
            {
                if (twitchCountdown >= (twitchDuration-6) && twitchCountdown <= (twitchDuration-4))
                {
                    foreach (IntVec3 c in GenAdjFast.AdjacentCellsCardinal(parent.Position))
					{
                        foreach(Thing t in c.GetThingList(parent.Map)) {
                            if (t.TryGetComp<CompUnfoldTwitch>() != null && !t.TryGetComp<CompUnfoldTwitch>().isTwtiching)
                            {
                                t.TryGetComp<CompUnfoldTwitch>().isTwtiching = true;
                                t.TryGetComp<CompUnfoldTwitch>().twitchCountdown = twitchDuration;
                            }
                        }
                    }
                }
                if (twitchCountdown >= 0)
                {
                    Traverse.Create(this).Field("extension").SetValue((extension - Props.extendRate));
                } else
                {
                    isTwtiching = false;
                    twitchCountdown = 10f;
                }
                twitchCountdown--;
            } else
            {
                base.CompTick();
            } 
            
            if (extension >= 1.0f)
            {
                if (ticksToTwitchInterval <= 0)
                {
                    if (Rand.Chance(0.1f))
                    {
                        isTwtiching = true;
                        twitchCountdown = twitchDuration;
                    }
                    ticksToTwitchInterval = Rand.RangeInclusive(GenMath.RoundRandom(twitchInterval * 0.85f), GenMath.RoundRandom(twitchInterval * 1.15f));
                }
                ticksToTwitchInterval--;
            }
        }

    }
}
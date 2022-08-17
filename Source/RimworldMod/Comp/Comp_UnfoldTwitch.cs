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

        private static Type unfoldType = AccessTools.TypeByName("UnfoldComponent");


        public override void CompTick()
        {
            base.CompTick();
            float extension = Traverse.Create(this).Field("extension").GetValue<float>();
            if (extension >= 1.0f)
            {
                if (ticksToTwitchInterval <= 0)
                {
                    if (Rand.Chance(0.1f))
                    {
                        Traverse.Create(this).Field("extension").SetValue((extension - Rand.RangeInclusive(15, 30)*Props.extendRate));
                    }
                    ticksToTwitchInterval = Rand.RangeInclusive(GenMath.RoundRandom(twitchInterval * 0.85f), GenMath.RoundRandom(twitchInterval * 1.15f));
                }
                ticksToTwitchInterval--;
            }
        }
    }
}
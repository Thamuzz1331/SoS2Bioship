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
    [StaticConstructorOnStartup]
    public class CompHeartHeat : CompShipHeat
    {
        public override int Threat
        {
            get
            {
                CompShipHeart heart = parent.TryGetComp<CompShipHeart>();
                if (heart != null)
                {
                    return heart.Threat;
                }
                return Props.threat;
            }
        }
    }
}
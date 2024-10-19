using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompShipScaffold : CompScaffold
    {
        CompProperties_ShipScaffold ShipProps => (CompProperties_ShipScaffold)props;

        public override Thing Convert(CompScaffoldConverter converter, bool instant = false)
        {
            if (converter.body.heart != null)
            {
                CompShipHeart h = (CompShipHeart)converter.body.heart;
                Building_ShipHeart bh = (Building_ShipHeart)h.parent;
                bh.recentReplace = true;
                Log.Message("Converting");
            }
            return base.Convert(converter, instant);
        }
    }
}
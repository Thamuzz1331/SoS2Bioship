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
    public class CompMutableScaffold : CompScaffold
    {
        CompProperties_MutableScaffold ShipProps => (CompProperties_MutableScaffold)props;

        public override ThingDef GetConversionDef(CompScaffoldConverter converter)
        {
            return ((CompShipHeart)converter.body.heart).GetThingDef(Props.transformString);
        }
    }
}
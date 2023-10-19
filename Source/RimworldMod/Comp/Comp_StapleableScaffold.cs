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
    public class CompStapleableScaffold : CompScaffold
    {
        CompProperties_StaplebleScaffold ShipProps => (CompProperties_StaplebleScaffold)props;

        public override ThingDef GetConversionDef(CompScaffoldConverter converter)
        {
            foreach (Thing t in parent.Position.GetThingList(parent.Map))
            {
                if (t is Building_NerveStaple)
                {
                    MakeReplacement(ThingDef.Named(ShipProps.stapleDef), converter);
                    return null;
                }                
            }
            return base.GetConversionDef(converter);
        }
    }
}
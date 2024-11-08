using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using SaveOurShip2;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class Building_BioTorpTurret : Building_ShipTurret
    {
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                bool isUnload = (gizmo is Command_Action && 
                    ((Command_Action)gizmo).defaultLabel == TranslatorFormattedStringExtensions.Translate("CommandExtractShipTorpedo"));
                if (!isUnload)
                {
                    yield return gizmo;
                }
            }
        }
    }
}
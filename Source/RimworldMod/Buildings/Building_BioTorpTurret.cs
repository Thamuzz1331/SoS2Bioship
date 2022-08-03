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
            /*
            if (CanExtractTorpedo)
            {
                CompChangeableProjectilePlural compChangeableProjectile = gun.TryGetComp<CompChangeableProjectilePlural>();
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = TranslatorFormattedStringExtensions.Translate("CommandExtractShipTorpedo");
                command_Action.defaultDesc = TranslatorFormattedStringExtensions.Translate("CommandExtractShipTorpedoDesc");
                command_Action.icon = compChangeableProjectile.LoadedShells[0].uiIcon;
                command_Action.iconAngle = compChangeableProjectile.LoadedShells[0].uiIconAngle;
                command_Action.iconOffset = compChangeableProjectile.LoadedShells[0].uiIconOffset;
                command_Action.iconDrawScale = GenUI.IconDrawScale(compChangeableProjectile.LoadedShells[0]);
                command_Action.action = delegate
                {
                    ExtractShells();
                };
                yield return command_Action;
            }
            */
        }
    }
}
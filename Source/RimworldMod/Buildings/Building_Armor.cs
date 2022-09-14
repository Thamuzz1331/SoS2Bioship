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
    public class Building_Armor : Building
    {
        CompShipBodyPart bp = null;
        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            if (bp == null)
            {
                bp = this.TryGetComp<CompShipBodyPart>();
            }
            if (bp != null)
            {
                dinfo.SetAmount(dinfo.Amount * bp.GetDamageMult(dinfo.Def));
            }
            base.PreApplyDamage(ref dinfo, out absorbed);
        }
    }
}
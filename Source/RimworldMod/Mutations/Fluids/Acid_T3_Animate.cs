using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class AnimateAcid : AmmoMutation
    {
        public AnimateAcid() : base(new List<AmmoMutationTurretDetails>() {
                    new AmmoMutationTurretDetails(
                            ThingDef.Named("ShipTurret_BioAcid"),
                            new Tuple<string, ThingDef, ThingDef>(
                                "Animate",
                                ThingDef.Named("Bullet_Fake_AnimateAcid"),
                                ThingDef.Named("Proj_AnimateAcid")
                                ),
                            "mediumTurretOptions",
                            2
                        )
                },
                "tier3",
                "Animate Acid",
                "Animate Acid\nSufficiently potent concentrations of acid mucus can animate into ameoba like creatures.",
                "offense",
                "humors",
                null)
        {
        }
    }
}
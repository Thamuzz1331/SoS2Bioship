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
    public class MotileAcid : AmmoMutation
    {
        public MotileAcid() : base(new List<AmmoMutationTurretDetails>() {
                    new AmmoMutationTurretDetails(
                            ThingDef.Named("ShipTurret_BioAcid"),
                            new Tuple<string, ThingDef, ThingDef>(
                                "Motile",
                                ThingDef.Named("Bullet_Fake_MotileAcid"),
                                ThingDef.Named("Proj_MotileAcid")
                                ),
                            "mediumTurretOptions",
                            2
                        )
                },
                "tier2",
                "Semi-Motile Acid",
                "Semi-Motile Acid\nThis heart's acidic mucus is capable of limited mobility, activly seeking to exploit openings to wreak havoc on internal systems.",
                "offense",
                "humors",
                null)
        {
        }

        public override List<Tuple<IMutation, string, string>> UpgradeTier(string tier, List<IMutation> existingMutations) {
            if (tier == "tier3")
            {
                return new List<Tuple<IMutation, string, string>>() {new Tuple<IMutation, string, string>(
                    new AnimateAcid(),
                    "offense",
                    "humors") };
            } else
            {
                return new List<Tuple<IMutation, string, string>>();
            }
        }


    }
}
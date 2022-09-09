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
    public class ShieldbusterPlasma : AmmoMutation
    {
        public ShieldbusterPlasma() : base(new List<AmmoMutationTurretDetails>() {
                    new AmmoMutationTurretDetails(
                            ThingDef.Named("ShipTurret_BioPlasma"),
                            new Tuple<string, ThingDef, ThingDef>(
                                "Energized",
                                ThingDef.Named("Bullet_Fake_BioPlasmaShieldbuster"),
                                ThingDef.Named("Proj_BioPlasmaShieldbuster")
                                ),
                            "mediumTurretOptions",
                            2
                        )
                },
                "tier2",
                "Shieldbuster Plasma",
                "Shieldbuster Plasma\nPlasma Maws significatly increase the damage they do to shields.",
                "offense",
                "humors",
                null)
        {
        }

        public override List<Tuple<IMutation, string, string>> UpgradeTier(string tier, List<IMutation> existingMutations) { 
            if (tier == "tier3")
            {
                return new List<Tuple<IMutation, string, string>>() {new Tuple<IMutation, string, string>(
                    new ShieldcrushingPlasma(),
                    "offense",
                    "humors") };
            } else
            {
                return base.UpgradeTier(tier, existingMutations);
            }
        }

    }
}
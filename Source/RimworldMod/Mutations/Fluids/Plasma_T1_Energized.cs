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
    public class EnergizedPlasma : AmmoMutation
    {
        public EnergizedPlasma() : base(new List<AmmoMutationTurretDetails>() {
                    new AmmoMutationTurretDetails(
                            ThingDef.Named("ShipTurret_BioPlasma"),
                            new Tuple<string, ThingDef, ThingDef>(
                                "Energized",
                                ThingDef.Named("Bullet_Fake_BioPlasmaEnergetic"),
                                ThingDef.Named("Proj_BioPlasmaEnergetic")
                                ),
                            "mediumTurretOptions",
                            2
                        )
                },
                "tier1",
                "Energized Plasma",
                "Energized Plasma\nPlasma Maws deal additional damage.",
                "offense",
                "humors",
                null)
        {
        }

         public override List<Tuple<IMutation, string, string>> UpgradeTier(string tier, List<IMutation> existingMutations) { 
            if (tier == "tier2")
            {
                return new List<Tuple<IMutation, string, string>>() {new Tuple<IMutation, string, string>(
                    new ShieldbusterPlasma(),
                    "offense",
                    "humors") };
            } else
            {
                return base.UpgradeTier(tier, existingMutations);
            }
        }
    }
}
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
    public class EnergizedNematocysts : AmmoMutation
    {
        public EnergizedNematocysts() : base(new List<AmmoMutationTurretDetails>() {
                    new AmmoMutationTurretDetails(
                            ThingDef.Named("ShipTurret_Nematocyst"),
                            new Tuple<string, ThingDef, ThingDef>(
                                "Splintering",
                                ThingDef.Named("Bullet_Fake_NematocystEnergized"),
                                ThingDef.Named("Proj_ShipTurretNematocystEnergized")
                                ),
                            "smallTurretOptions",
                            0
                        ),
                        new AmmoMutationTurretDetails(
                            ThingDef.Named("ShipTurret_ClusteredNematocyst"),
                            new Tuple<string, ThingDef, ThingDef>(
                                "Splintering",
                                ThingDef.Named("Bullet_Fake_NematocystEnergized"),
                                ThingDef.Named("Proj_ShipTurretNematocystEnergized")
                                ),
                            "smallTurretOptions",
                            0
                        )

                },
                "tier2",
                "Energized Nematocysts",
                "Energized Nematocysts\nEnergized Nematocysts replace their venom channels with a resivour of charged plasma.  While less effective against armor, this plasma is highly effective against shields.",
                "offense",
                "flesh",
                null)
        {
        }

        public override List<Tuple<IMutation, string, string>> UpgradeTier(string tier, List<IMutation> existingMutations)
        {
            return base.UpgradeTier(tier, existingMutations);
        }
    }
}
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
    public class BatteringNematocysts : AmmoMutation
    {
        public BatteringNematocysts() : base(new List<AmmoMutationTurretDetails>() {
                    new AmmoMutationTurretDetails(
                            ThingDef.Named("ShipTurret_Nematocyst"),
                            new Tuple<string, ThingDef, ThingDef>(
                                "Battering",
                                ThingDef.Named("Bullet_Fake_NematocystBattering"),
                                ThingDef.Named("Proj_ShipTurretNematocystBattering")
                                ),
                            "smallTurretOptions",
                            0
                        ),
                        new AmmoMutationTurretDetails(
                            ThingDef.Named("ShipTurret_ClusteredNematocyst"),
                            new Tuple<string, ThingDef, ThingDef>(
                                "Battering",
                                ThingDef.Named("Bullet_Fake_NematocystBattering"),
                                ThingDef.Named("Proj_ShipTurretNematocystBattering")
                                ),
                            "smallTurretOptions",
                            0
                        )

                },
                "tier2",
                "Battering Nematocysts",
                "Battering Nematocysts\nBattering Nematocysts deal increased damage, but lack venom.\nBetter suited for taking on inorganic targets.",
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
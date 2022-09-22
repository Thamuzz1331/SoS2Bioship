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
    public class SplinteringNematocysts : AmmoMutation
    {
        public SplinteringNematocysts() : base(new List<AmmoMutationTurretDetails>() {
                    new AmmoMutationTurretDetails(
                            ThingDef.Named("ShipTurret_Nematocyst"),
                            new Tuple<string, ThingDef, ThingDef>(
                                "Splintering",
                                ThingDef.Named("Bullet_Fake_NematocystSplintering"),
                                ThingDef.Named("Proj_ShipTurretNematocystSplintering")
                                ),
                            "smallTurretOptions",
                            0
                        ),
                        new AmmoMutationTurretDetails(
                            ThingDef.Named("ShipTurret_ClusteredNematocyst"),
                            new Tuple<string, ThingDef, ThingDef>(
                                "Splintering",
                                ThingDef.Named("Bullet_Fake_NematocystSplintering"),
                                ThingDef.Named("Proj_ShipTurretNematocystSplintering")
                                ),
                            "smallTurretOptions",
                            0
                        )

                },
                "tier2",
                "Splintering Nematocysts",
                "Splintering Nematocysts\nSplintering Nematocysts shatter on impact, creating a cloud of shrapnel.",
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
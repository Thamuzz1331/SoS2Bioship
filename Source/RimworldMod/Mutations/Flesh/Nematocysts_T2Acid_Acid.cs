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
    public class AcidNematocysts : AmmoMutation
    {
        public AcidNematocysts() : base(new List<AmmoMutationTurretDetails>() {
                    new AmmoMutationTurretDetails(
                            ThingDef.Named("ShipTurret_Nematocyst"),
                            new Tuple<string, ThingDef, ThingDef>(
                                "Acid",
                                ThingDef.Named("Bullet_Fake_NematocystAcid"),
                                ThingDef.Named("Proj_ShipTurretNematocystAcid")
                                ),
                            "smallTurretOptions",
                            0
                        ),
                        new AmmoMutationTurretDetails(
                            ThingDef.Named("ShipTurret_ClusteredNematocyst"),
                            new Tuple<string, ThingDef, ThingDef>(
                                "Acid",
                                ThingDef.Named("Bullet_Fake_NematocystAcid"),
                                ThingDef.Named("Proj_ShipTurretNematocystAcid")
                                ),
                            "smallTurretOptions",
                            0
                        )

                },
                "tier2",
                "Acid Injecting Nematocysts",
                "Acid Injecting Nematocysts\nAcid Nematocysts replace their normal venom with acid, more suitable for inorganic targets.",
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
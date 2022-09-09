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
    public class PotentAcid : AmmoMutation
    {
        public PotentAcid() : base(new List<AmmoMutationTurretDetails>() {
                    new AmmoMutationTurretDetails(
                            ThingDef.Named("ShipTurret_BioAcid"),
                            new Tuple<string, ThingDef, ThingDef>(
                                "Potent",
                                ThingDef.Named("Bullet_Fake_PotentAcid"),
                                ThingDef.Named("Proj_PotentAcid")
                                ),
                            "mediumTurretOptions",
                            2
                        )
                },
                "tier1",
                "Potent Acid",
                "Potent Acid\nLingering acid from acid spitters deal more damage.",
                "offense",
                "humors",
                null)
        {
        }
        public override List<Tuple<IMutation, string, string>> UpgradeTier(string tier, List<IMutation> existingMutations)
        {
            if (tier == "tier2")
            {
                return new List<Tuple<IMutation, string, string>>() {new Tuple<IMutation, string, string>(
                    new MotileAcid(),
                    "offense",
                    "humors") };
            } else
            {
                return base.UpgradeTier(tier, existingMutations);
            }
        }

    }
}
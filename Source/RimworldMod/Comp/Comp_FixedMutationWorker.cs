using SaveOurShip2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class CompFixedMutationWorker : CompMutationWorker
    {
        public CompProperties_FixedMutationWorker FixedProps => (CompProperties_FixedMutationWorker)props;

        Dictionary<String, IMutation> muts = new Dictionary<String, IMutation>()
        {
            {"Bone Armor", new BoneArmor()},
            {"Plasteeel Carapace", new PlasteelArmor()},

            {"Dense Spines", new DenseSpines()},
            {"Penetrator Spines", new PenetratorSpines()},

            {"Efficient Spines", new EfficientSpines()},
            {"Spinestorm", new SpineStorm()},

            {"Clustered Nematocysts", new ClusteredNematocysts()},


            {"Potent Acid", new PotentAcid()},
            {"Semi-Motile Acid", new MotileAcid()},
            {"Animate Acid", new AnimateAcid()},
            {"Energized Plasma", new EnergizedPlasma()},
            {"Shieldbuster Plasma", new ShieldbusterPlasma()},
            {"Shieldcrusher Plasma", new ShieldcrushingPlasma()},
            {"Efficient Regeneration", new EfficientRegeneration()},
            {"Fast Regeneration", new FastRegeneration()},
            {"Undying Beast", new UndyingBeast()},
            {"Efficient Fat Storage", new EfficientFatStorage()},
            {"Dense Fat", new DenseFat()},

            {"Occular Lineage", new OcularPerk()},

            {"Iron Will", new IronWill()},
            {"Reflect", new Reflect()},

        };

        public override void GetInitialMutations(BuildingBody body)
        {
            foreach(string mutId in FixedProps.startingMutations)
            {
                if (muts.ContainsKey(mutId))
                {
                    this.SpreadMutation(body, muts[mutId]);
                }
            }
        }
    }
}
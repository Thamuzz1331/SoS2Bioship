using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public struct AmmoMutationTurretDetails
    {
        public ThingDef turretDef;
        public Tuple<string, ThingDef, ThingDef> ammoDef;
        public string addTable;
        public int addWeight;
    }

    public class AmmoMutation : IMutation
    {
        public List<AmmoMutationTurretDetails> turretsToApplyTo;
        public string tier;
        public string name;
        public string desc;

        bool IHediff.ShouldAddTo(CompBuildingBodyPart target)
        {
            bool ret = false;
            foreach(AmmoMutationTurretDetails det in turretsToApplyTo)
            {
                ret = ret || (target.parent.def == det.turretDef);
            }
            ret = ret || (target.parent.TryGetComp<CompShipHeart>() != null);
            ret = ret || (target.parent.TryGetComp<CompMutationWorker>() != null);
            return ret;
        }

        void IHediff.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                foreach (AmmoMutationTurretDetails det in turretsToApplyTo)
                {
                    for (int i = 0; i < det.addWeight; i++)
                    {
                        target.parent.TryGetComp<CompShipHeart>().defs.TryGetValue(det.addTable, new List<ThingDef>())
                            .Add(det.turretDef);
                    }
                }
            }
            foreach (AmmoMutationTurretDetails det in turretsToApplyTo)
            {
                if (target.parent.def == det.turretDef)
                {
                    target.parent.TryGetComp<CompMutableAmmo>()
                        .ammoTypes.Add(det.ammoDef.Item1,
                            new Tuple<ThingDef, ThingDef>(
                                det.ammoDef.Item2,
                                det.ammoDef.Item3));
                    target.parent.TryGetComp<CompMutableAmmo>()
                        .currentlySelected = det.ammoDef.Item1;
                }
            }
/*
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<PotentAcid>("offense", "humors", true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]++;
            }
*/
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                foreach (AmmoMutationTurretDetails det in turretsToApplyTo)
                {
                    for (int i = 0; i < det.addWeight; i++)
                    {
                        target.parent.TryGetComp<CompShipHeart>().defs.TryGetValue(det.addTable, new List<ThingDef>())
                            .Remove(det.turretDef);
                    }
                }
            }
            foreach (AmmoMutationTurretDetails det in turretsToApplyTo)
            {
                if (target.parent.def == det.turretDef)
                {
                    target.parent.TryGetComp<CompMutableAmmo>()
                        .ammoTypes.Remove(det.ammoDef.Item1);

                }
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("offense", "humors", this, true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]--;
            }

        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations)
        {
            if (tier == "tier2")
            {
                return new List<Tuple<IMutation, string, string>>() {new Tuple<IMutation, string, string>(
                    new MotileAcid(),
                    "offense",
                    "humors") };
            }
            else
            {
                return new List<Tuple<IMutation, string, string>>();
            }
        }
        String IMutation.GetTier()
        {
            return tier;
        }
        String IMutation.GetDescription()
        {
            return desc;
        }
        public override String ToString()
        {
            return name;
        }
        Texture2D IMutation.GetIcon()
        {
            return null;
        }
        void IExposable.ExposeData()
        {

        }
    }
}
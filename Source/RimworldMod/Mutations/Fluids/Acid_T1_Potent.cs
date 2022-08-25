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
    public class PotentAcid : IMutation
    {
        bool IHediff.ShouldAddTo(CompBuildingBodyPart target)
        {
            bool ret = false;
            ret = ret || (target.parent.def == ThingDef.Named("ShipTurret_BioAcid"));
            ret = ret || (target.parent.TryGetComp<CompShipHeart>() != null);
            ret = ret || (target.parent.TryGetComp<CompMutationWorker>() != null);
            return ret;
        }

        void IHediff.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                target.parent.TryGetComp<CompShipHeart>().defs.TryGetValue("mediumTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("ShipTurret_BioAcid"));
                target.parent.TryGetComp<CompShipHeart>().defs.TryGetValue("mediumTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("ShipTurret_BioAcid"));
            }
            if (target.parent.def == ThingDef.Named("ShipTurret_BioAcid"))
            {
                target.parent.TryGetComp<CompMutableAmmo>()
                    .ammoTypes.Add("Potent", 
                    new Tuple<ThingDef, ThingDef>(
                        ThingDef.Named("Proj_PotentAcid"), 
                        ThingDef.Named("Bullet_Fake_PotentAcid")));
                target.parent.TryGetComp<CompMutableAmmo>().currentlySelected = "Potent";
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<PotentAcid>("offense", "humors", true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]++;
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                target.parent.TryGetComp<CompShipHeart>().defs.TryGetValue("mediumTurretOptions", new List<ThingDef>())
                    .Remove(ThingDef.Named("ShipTurret_BioAcid"));
                target.parent.TryGetComp<CompShipHeart>().defs.TryGetValue("mediumTurretOptions", new List<ThingDef>())
                    .Remove(ThingDef.Named("ShipTurret_BioAcid"));
            }
            if (target.parent.def == ThingDef.Named("ShipTurret_BioAcid"))
            {
                target.parent.TryGetComp<CompMutableAmmo>()
                    .ammoTypes.Remove("Potent Acid");
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("offense", "humors", this, true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]--;
            }

        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            if (tier == "tier2")
            {
                return new List<Tuple<IMutation, string, string>>() {new Tuple<IMutation, string, string>(
                    new MotileAcid(),
                    "offense",
                    "humors") };
            } else
            {
                return new List<Tuple<IMutation, string, string>>();
            }
        }
        String IMutation.GetTier() {
            return "tier1";
        }
        String IMutation.GetDescription()
        {
            return "Potent Acid\nLingering acid from acid spitters deal more damage.";
        }
        public override String ToString()
        {
            return "Potent Acid";
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
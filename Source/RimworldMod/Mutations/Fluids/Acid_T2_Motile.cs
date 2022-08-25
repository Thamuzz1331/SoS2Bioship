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
    public class MotileAcid : IMutation
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
                    .ammoTypes.Add("Motile",
                    new Tuple<ThingDef, ThingDef>(
                        ThingDef.Named("Proj_MotileAcid"),
                        ThingDef.Named("Bullet_Fake_MotileAcid")));
                target.parent.TryGetComp<CompMutableAmmo>().currentlySelected = "Motile";
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<MotileAcid>("offense", "humors", true);
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
                    .ammoTypes.Remove("Motile");
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("offense", "humors", this, true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]--;
            }

        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            if (tier == "tier3")
            {
                return new List<Tuple<IMutation, string, string>>() {new Tuple<IMutation, string, string>(
                    new AnimateAcid(),
                    "offense",
                    "humors") };
            } else
            {
                return new List<Tuple<IMutation, string, string>>();
            }
        }
        String IMutation.GetTier() {
            return "tier2";
        }
        String IMutation.GetDescription()
        {
            return "Semi-Motile Acid\nThis heart's acidic is capable of limited mobility, activly seeking to exploit openings to wreak havoc on internal systems.";
        }
        public override String ToString()
        {
            return "Semi-Motile Acid";
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
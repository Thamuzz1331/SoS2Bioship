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
    public class AnimateAcid : IMutation
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
                   .ammoTypes.Add("Animate",
                   new Tuple<ThingDef, ThingDef>(
                       ThingDef.Named("Proj_AnimateAcid"),
                       ThingDef.Named("Bullet_Fake_AnimateAcid")));
                target.parent.TryGetComp<CompMutableAmmo>().currentlySelected = "Animate";

            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<AnimateAcid>("offense", "humors", true);
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
                    .ammoTypes.Remove("Animate");
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("offense", "humors", this, true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]--;
            }

        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            return new List<Tuple<IMutation, string, string>>();
        }
        String IMutation.GetTier() {
            return "tier1";
        }
        String IMutation.GetDescription()
        {
            return "Animate Acid\nSufficiently potent concentrations of acid mucus can animate into ameoba like creatures.";
        }
        public override String ToString()
        {
            return "Animate Acid";
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
using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    public class ShieldcrushingPlasma : IMutation
    {
        bool IMutation.ShouldAddTo(CompBuildingBodyPart target)
        {
            bool ret = false;
            ret = ret || (target.parent.TryGetComp<CompShipHeart>() != null);
            ret = ret || (target.parent.TryGetComp<CompMutationWorker>() != null);
            return ret;
        }

        void IMutation.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                DefOptions blank = new DefOptions(new List<ThingDef>());
                target.parent.TryGetComp<CompShipHeart>().defs.TryGetValue("spinalTurretOptions", blank)
                    .defs.Add(ThingDef.Named("BatteringPlasmaMaw"));
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<ShieldcrushingPlasma>("offense", "humors");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]++;
            }
        }
        void IMutation.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                DefOptions blank = new DefOptions(new List<ThingDef>());
                target.parent.TryGetComp<CompShipHeart>().defs.TryGetValue("spinalTurretOptions", blank)
                    .defs.Remove(ThingDef.Named("BatteringPlasmaMaw"));
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("offense", "humors", this);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]--;
            }

        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            return new List<Tuple<IMutation, string, string>>() { };
        }
        String IMutation.GetTier() {
            return "tier3";
        }
        String IMutation.GetDescription()
        {
            return "Shieldcrusher Plasma\nSpinal weapon scaffolds have a chance to develop into a shieldcrusher maw.  The projectiles of this terrible weapon can force enemy shields to contract under its onslaught.";
        }
        public override String ToString()
        {
            return "Shieldcrusher Plasma";
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
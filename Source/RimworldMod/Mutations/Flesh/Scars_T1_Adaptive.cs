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
    public class AdaptiveScars : IMutation
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
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.maxResistence += 0.10f;

            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<ClusteredNematocysts>("defense", "flesh");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["flesh"]++;
            }
        }
        void IMutation.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.maxResistence -= 0.10f;

            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<ClusteredNematocysts>("defense", "flesh");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["flesh"]--;
            }
        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations)
        {
            if (tier == "tier2")
            {
                return new List<Tuple<IMutation, string, string>>() {new Tuple<IMutation, string, string>(
                    new MoreAdaptiveScars(),
                    "defense",
                    "flesh") };
            }
            else
            {
                return new List<Tuple<IMutation, string, string>>();
            }
        }
        String IMutation.GetTier()
        {
            return "tier1";
        }
        String IMutation.GetDescription()
        {
            return "Adaptive Scars\nThe flesh of the bioship adapts as it heals.  This bease will not suffer full damage from the same weapon twice.  Increases the maximum damage adaptation level.";
        }
        public override String ToString()
        {
            return "Adaptive Scars";
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
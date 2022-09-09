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
    public class FastRegeneration : IMutation
    {
        bool IHediff.ShouldAddTo(CompBuildingBodyPart target)
        {
            bool ret = false;
            ret = ret || (target.parent.TryGetComp<CompShipHeart>() != null);
            ret = ret || (target.parent.TryGetComp<CompMutationWorker>() != null);
            return ret;
        }

        void IHediff.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.stats["regenSpeed"] *= 1.25f;
                heart.stats["metabolicSpeed"] *= 1.05f;
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<FastRegeneration>("defense", "humors");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]++;
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.stats["regenSpeed"] *= 1f/1.25f;
                heart.stats["metabolicSpeed"] *= 1f/1.05f;
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("defense", "humors", new FastRegeneration());
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]--;
            }
        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            if (tier == "tier3")
            {
                return new List<Tuple<IMutation, string, string>>() {new Tuple<IMutation, string, string>(
                    new UndyingBeast(),
                    "defense",
                    "humors") };
            }
            else
            {
                return new List<Tuple<IMutation, string, string>>();
            }
        }
        String IMutation.GetTier() {
            return "tier2";
        }
        String IMutation.GetDescription()
        {
            return "Fast Regeneration\nIncreases the speed with which damaged body parts regenerate.";
        }
        public override string ToString()
        {
            return "Fast Regeneration";
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
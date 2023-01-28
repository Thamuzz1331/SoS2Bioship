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
    public class EfficientRegeneration : IMutation
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
                BuildingHediff toAdd = new BuildingHediff();
                toAdd.label = "Efficient Regeneration";
                toAdd.statMods = new Dictionary<string, float>()
                {
                    {"regenEfficiency", 1.25f},
                    {"metabolicEfficiency", 1.05f}
                };
                target.AddHediff(toAdd);
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<EfficientRegeneration>("defense", "humors");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]++;
            }
        }
        void IMutation.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                target.RemoveHediff("Efficient Regeneration");
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("defense", "humors", this);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]--;
            }
        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            if (tier == "tier2")
            {
                return new List<Tuple<IMutation, string, string>>() {new Tuple<IMutation, string, string>(
                    new FastRegeneration(),
                    "defense",
                    "humors") };
            }
            else
            {
                return new List<Tuple<IMutation, string, string>>();
            }
        }
        String IMutation.GetTier() {
            return "tier1";
        }
        String IMutation.GetDescription()
        {
            return "Efficient Regeneration\nReduces the nutrition cost to regenerate lost bodyparts.";
        }
        public override String ToString()
        {
            return "Efficient Regeneration";
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
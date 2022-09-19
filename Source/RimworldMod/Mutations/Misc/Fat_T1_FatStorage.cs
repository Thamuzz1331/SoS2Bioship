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
    public class EfficientFatStorage : IMutation
    {
        bool IHediff.ShouldAddTo(CompBuildingBodyPart target)
        {
            bool ret = false;
            ret = ret || (target.parent.TryGetComp<CompShipNutritionStore>() != null);
            ret = ret || (target.parent.TryGetComp<CompMutationWorker>() != null);
            return ret;
        }
        void IHediff.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipNutritionStore>() != null)
            {
                target.parent.TryGetComp<CompShipNutritionStore>().efficiency = 1.5f;
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<EfficientFatStorage>("utility", "misc");
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipNutritionStore>() != null)
            {
                target.parent.TryGetComp<CompShipNutritionStore>().efficiency = 1.0f;
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("utility", "misc", this);
            }

        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            if (tier == "tier2")
            {
                return new List<Tuple<IMutation, string, string>>() { new Tuple<IMutation, string, string>(
                    new DenseFat(),
                    "utility",
                    "misc") };
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
            return "Efficient Fat Storage\nIncreases the fat stored per excess nutrition significantly.";
        }
        public override string ToString()
        {
            return "Efficient Fat Storage";
        }
        Texture2D IMutation.GetIcon()
        {
            return null;
        }
        void IExposable.ExposeData()
        {

        }
        float IHediff.StatMult(string stat)
        {
            return 1f;
        }
    }
}
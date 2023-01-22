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
    public class DenseFat : IMutation
    {
        bool IMutation.ShouldAddTo(CompBuildingBodyPart target)
        {
            bool ret = false;
            ret = ret || (target.parent.TryGetComp<CompShipNutritionStore>() != null);
            ret = ret || (target.parent.TryGetComp<CompMutationWorker>() != null);
            return ret;
        }
        void IMutation.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipNutritionStore>() != null)
            {
                target.parent.TryGetComp<CompShipNutritionStore>().capacityModifier = 1.5f;
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<DenseFat>("utility", "misc");
            }
        }
        void IMutation.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipNutritionStore>() != null)
            {
                target.parent.TryGetComp<CompShipNutritionStore>().capacityModifier = 1.0f;
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("utility", "misc", this);
            }

        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            if (tier == "tier3")
            {
                return new List<Tuple<IMutation, string, string>>() { };
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
            return "Dense Fat\nIncreases the storage capacity of fat deposites significantly.";
        }
        public override string ToString()
        {
            return "Dense Fat";
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
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
    public class EfficientGrowth : IMutation
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
                Hediff_Building toAdd = new Hediff_Building();
                toAdd.label = "Efficient Growth";
                toAdd.visible = true;
                toAdd.statMods = new Dictionary<string, float>()
                {
                    {"growthEfficiency", 1.50f},
                    {"metabolicEfficiency", 1.15f},
                };
                target.AddHediff(toAdd);
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<EfficientGrowth>("utility", "misc");
            }
        }
        void IMutation.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                target.RemoveHediff("Efficient Growth");
            }

            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("utility", "misc", new EfficientGrowth());
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
            return "Efficient Growth\nHeart is particularly efficient in converting scaffolds to flesh, and gains a small bonus to universal metobolic efficiency.";
        }
        public override string ToString()
        {
            return "Efficient Growth";
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
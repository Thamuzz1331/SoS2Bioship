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
    public class EfficientFatStorage : IHediff
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
                                Log.Message("!!!!");

                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<EfficientFatStorage>("utility", "misc", true);
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {

        }
        /*
        void IMutation.Apply(Building_ShipHeart target)
        {
            target.statMultipliers.Add("storageEfficiency", 1.25f);

            target.RemoveMutation<EfficientFatStorage>("utility", "misc", true);

            return;
        }
        void IMutation.Apply(Thing target)
        {
            return;
        }
        */
        void IExposable.ExposeData()
        {

        }
    }
}
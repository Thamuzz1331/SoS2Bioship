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
    public class EfficientRegeneration : IHediff
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
                if (!heart.multipliers.ContainsKey("regenCost"))
                {
                    heart.multipliers.Add("regenCost", 0.75f);
                } else
                {
                    heart.multipliers["regenCost"] *= 0.75f;
                }
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<EfficientRegeneration>("defense", "humors", true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]++;
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {

        }
        /*
        void IMutation.Apply(Building_ShipHeart target)
        {
            if (target.statMultipliers.ContainsKey("regenCost"))
            {
                target.statMultipliers["regenCost"] *= 0.75f;
            }
            else
            {
                target.statMultipliers.Add("regenCost", 0.75f);
            }

            target.RemoveMutation<EfficientRegeneration>("defense", "humors", true);

            target.mutationThemes["humors"] += 1;
            return;
        }
        void IMutation.Apply(Thing target)
        {
            return;
        }*/
        void IExposable.ExposeData()
        {

        }
    }
}
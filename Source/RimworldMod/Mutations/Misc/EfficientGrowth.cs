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
    public class EfficientGrowth : IHediff
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
                    heart.multipliers.Add("regenCost", 0.85f);
                    if (!heart.stats.ContainsKey("conversionCost"))
                    {
                        heart.stats.Add("conversionCost", 15f);
                    } 
                    heart.stats["conversionCost"] *= 0.75f;
                } else
                {
                    heart.multipliers["regenCost"] *= 0.85f;
                }
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<EfficientGrowth>("utility", "misc", true);
            }

        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {

        }

        void IExposable.ExposeData()
        {

        }
    }
}
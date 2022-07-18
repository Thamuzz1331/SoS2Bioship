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
    public class FastRegeneration : IHediff
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
                if (!heart.multipliers.ContainsKey("regenInterval"))
                {
                    heart.multipliers.Add("regenInterval", 0.75f);
                } else
                {
                    heart.multipliers["regenInterval"] *= 0.75f;
                }
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<FastRegeneration>("defense", "humors", true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]++;
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {

        }
/*        void Apply(CompShipHeart target)
        {
            target.statMultipliers.Add("regenInterval", 0.85f);

            target.RemoveMutation<FastRegeneration>("defense", "humors", true);

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
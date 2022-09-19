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
    public class Reflect : IMutation
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
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<Reflect>("defense", "psi");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["psi"] += 2;
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("defense", "psi", this);
            }

        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations)
        {
            return new List<Tuple<IMutation, string, string>>();
        }

        String IMutation.GetTier()
        {
            return "tier2";
        }
        String IMutation.GetDescription()
        {
            return "Reflect\nThe bioship's psi shields gain a small chance to reflect attacks back on their source.";
        }
        public override string ToString()
        {
            return "Reflect";
        }
        Texture2D IMutation.GetIcon()
        {
            return null;
        }
        void IExposable.ExposeData()
        {

        }
        Dictionary<string, float> statMults = new Dictionary<string, float>()
        {
            {"conciousness", 1.1f},
        };

        float IHediff.StatMult(string stat)
        {
            return statMults.TryGetValue(stat, 1f);
        }
    }
}
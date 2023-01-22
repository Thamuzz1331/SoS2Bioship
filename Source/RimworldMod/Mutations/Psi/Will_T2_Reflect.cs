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
    public class Reflect : IMutation
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
                Hediff_Reflect toAdd = new Hediff_Reflect();
                toAdd.label = "Reflect";
                toAdd.visible = true;
                toAdd.statMods = new Dictionary<string, float>()
                {
                    {"shieldStrength", 1.1f}
                };
                target.AddHediff(toAdd);
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<Reflect>("defense", "psi");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["psi"] += 2;
            }
        }
        void IMutation.Remove(CompBuildingBodyPart target)
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
    }
}
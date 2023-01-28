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
    public class UndyingBeast : IMutation
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
                toAdd.label = "Undying Beast";
                toAdd.visible = true;
                toAdd.statMods = new Dictionary<string, float>()
                {
                    {"regenSpeed", 1.25f},
                    {"growthSpeed", 1.05f},
                    {"metabolicSpeed", 1.05f},
                    {"regenEfficiency", 1.25f},
                    {"metabolicEfficiency", 1.05f}
                };
                target.AddHediff(toAdd);
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<FastRegeneration>("defense", "humors");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]++;
            }
        }
        void IMutation.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                target.RemoveHediff("Undying Beast");
            }
            if(target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("defense", "humors", new FastRegeneration());
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]--;
            }
        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            return new List<Tuple<IMutation, string, string>>();
        }
        String IMutation.GetTier() {
            return "tier3";
        }
        String IMutation.GetDescription()
        {
            return "Undying Beast\nThe body heals almost faster than it can be injured and with a fraction of the nutrition cost.";
        }
        public override String ToString()
        {
            return "Undying Beast";
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
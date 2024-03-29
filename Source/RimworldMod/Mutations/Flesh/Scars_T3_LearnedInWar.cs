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
    public class LearnedInWar : IMutation
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
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.maxResistence += 0.10f;

                BuildingHediff toAdd = new BuildingHediff();
                toAdd.label = "Learned In War";
                toAdd.statMods = new Dictionary<string, float>()
                {
                    {"regenEfficiency", 1.05f},
                    {"regenSpeed", 1.05f}
                };
                target.AddHediff(toAdd);
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<ClusteredNematocysts>("defense", "flesh");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["flesh"]++;
            }
        }
        void IMutation.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.maxResistence -= 0.10f;

            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<ClusteredNematocysts>("defense", "flesh");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["flesh"]--;
            }
        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations)
        {
            return new List<Tuple<IMutation, string, string>>();
        }
        String IMutation.GetTier()
        {
            return "tier3";
        }
        String IMutation.GetDescription()
        {
            return "Learned in War\nIn life's school of war, that which does not kill me makes me stronger.\nRaises adaptation cap.  Slightly improves regeneration speed and efficiency.";
        }
        public override String ToString()
        {
            return "Learned in War";
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
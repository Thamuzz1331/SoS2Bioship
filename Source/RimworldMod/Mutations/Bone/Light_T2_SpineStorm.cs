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
    public class SpineStorm : IMutation
    {
        bool IHediff.ShouldAddTo(CompBuildingBodyPart target)
        {
            bool ret = false;
            ret = ret || (target.parent.TryGetComp<CompShipHeart>() != null);
            ret = ret || (target.parent.def == ThingDef.Named("LightSpineLauncher"));
            ret = ret || (target.parent.TryGetComp<CompMutationWorker>() != null);
            return ret;
        }

        void IHediff.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.def == ThingDef.Named("LightSpineLauncher") && target.parent.TryGetComp<CompNutritionLoader>() != null)
            {
                target.parent.TryGetComp<CompNutritionLoader>().torpSpawn.Add(1);
                target.parent.TryGetComp<CompNutritionLoader>().torpSpawn.Add(1);
                target.parent.TryGetComp<CompNutritionLoader>().torpSpawn.Add(2);
                target.parent.TryGetComp<CompNutritionLoader>().torpSpawn.Add(2);
                target.parent.TryGetComp<CompNutritionLoader>().torpSpawn.Add(2);
                target.parent.TryGetComp<CompNutritionLoader>().torpSpawn.Add(6);
            }
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();

                heart.defs.TryGetValue("largeTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("LightSpineLauncher"));
                heart.defs.TryGetValue("largeTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("LightSpineLauncher"));
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<SpineStorm>("offense", "bone");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["bone"]++;
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {

        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            return new List<Tuple<IMutation, string, string>>() { };
        }
        String IMutation.GetTier() {
            return "tier2";
        }
        String IMutation.GetDescription()
        {
            return "Spinestorm\nIncreases the chances of a light spine launcher spawning two spines instead of one, and adds a small chance of spawning six instead.";
        }
        public override String ToString()
        {
            return "Spinestorm";
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
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
    public class PenetratorSpines : IMutation
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
                heart.defs.TryGetValue("HeavySpineLauncher", new List<ThingDef>())
                    .Add(ThingDef.Named("Spine_HeavyDense"));
                heart.defs.TryGetValue("HeavySpineLauncher", new List<ThingDef>())
                    .Add(ThingDef.Named("Spine_Penetrator"));

                heart.defs.TryGetValue("largeTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("HeavySpineLauncher"));
                heart.defs.TryGetValue("largeTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("HeavySpineLauncher"));
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<PenetratorSpines>("offense", "bone");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["bone"]++;
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.defs.TryGetValue("HeavySpineLauncher", new List<ThingDef>())
                    .Remove(ThingDef.Named("Spine_HeavyDense"));
                heart.defs.TryGetValue("HeavySpineLauncher", new List<ThingDef>())
                    .Remove(ThingDef.Named("Spine_Penetrator"));

                heart.defs.TryGetValue("largeTurretOptions", new List<ThingDef>())
                    .Remove(ThingDef.Named("HeavySpineLauncher"));
                heart.defs.TryGetValue("largeTurretOptions", new List<ThingDef>())
                    .Remove(ThingDef.Named("HeavySpineLauncher"));
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<DenseSpines>("offense", "bone");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["bone"]--;
            }
        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            return new List<Tuple<IMutation, string, string>>() { };
        }
        String IMutation.GetTier() {
            return "tier2";
        }
        String IMutation.GetDescription()
        {
            return "Penetrator Spines\nHeavy spine launchers have a chance to spawn a penetrator spine, a two stage projectile which can punch through ship armor.";
        }
        public override String ToString()
        {
            return "Penetrator Spines";
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
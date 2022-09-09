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
    public class DenseSpines : IMutation
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
                    .Add(ThingDef.Named("Spine_HeavyDense"));

                heart.defs.TryGetValue("largeTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("HeavySpineLauncher"));
                heart.defs.TryGetValue("largeTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("HeavySpineLauncher"));
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<DenseSpines>("offense", "bone");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["bone"]++;

            }

        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {

        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            if (tier == "tier2")
            {
                return new List<Tuple<IMutation, string, string>>() {new Tuple<IMutation, string, string>(
                    new PenetratorSpines(),
                    "offense",
                    "bone") };
            }
            else
            {
                return new List<Tuple<IMutation, string, string>>();
            }
        }
        String IMutation.GetTier() {
            return "tier1";
        }
        String IMutation.GetDescription()
        {
            return "Dense Spines\nAdds a chance that, when growing a spine, a heavy spine launcher will grow a dense spine that deals double damage.\nIncreases likelyhood of large weapon scaffolds maturing into heavy spine throwers.";
        }
        public override String ToString()
        {
            return "Dense Spines";
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
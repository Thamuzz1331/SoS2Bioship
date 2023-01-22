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
    public class DenseSpines : IMutation
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
                DefOptions blank = new DefOptions(new List<ThingDef>());
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.defs.TryGetValue("HeavySpineLauncher", blank)
                    .defs.Add(ThingDef.Named("Spine_HeavyDense"));
                heart.defs.TryGetValue("HeavySpineLauncher", blank)
                    .defs.Add(ThingDef.Named("Spine_HeavyDense"));

                heart.defs.TryGetValue("largeTurretOptions", blank)
                    .defs.Add(ThingDef.Named("HeavySpineLauncher"));
                heart.defs.TryGetValue("largeTurretOptions", blank)
                    .defs.Add(ThingDef.Named("HeavySpineLauncher"));
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<DenseSpines>("offense", "bone");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["bone"]++;

            }

        }
        void IMutation.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                DefOptions blank = new DefOptions(new List<ThingDef>());
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.defs.TryGetValue("HeavySpineLauncher", blank)
                    .defs.Remove(ThingDef.Named("Spine_HeavyDense"));
                heart.defs.TryGetValue("HeavySpineLauncher", blank)
                    .defs.Remove(ThingDef.Named("Spine_HeavyDense"));

                heart.defs.TryGetValue("largeTurretOptions", blank)
                    .defs.Remove(ThingDef.Named("HeavySpineLauncher"));
                heart.defs.TryGetValue("largeTurretOptions", blank)
                    .defs.Remove(ThingDef.Named("HeavySpineLauncher"));
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("offense", "bone", this);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["bone"]--;

            }
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
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
    public class EfficientSpines : IHediff
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
                target.parent.TryGetComp<CompNutritionLoader>().torpSpawn.Add(2);
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
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<EfficientSpines>("offense", "bone", true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["bone"]++;
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {

        }
        /*
        void Apply(CompShipHeart heart)
        {

            target.organOptions["largeTurretOptions"].Add(ThingDef.Named("LightSpineLauncher"));
            target.organOptions["largeTurretOptions"].Add(ThingDef.Named("LightSpineLauncher"));

            target.RemoveMutation<EfficientSpines>("offense", "bone", true);
            target.mutationThemes["bone"] += 1;
            return;
        }
        void IHediff.Apply(Thing target)
        {
            if (target.def == ThingDef.Named("LightSpineLauncher"))
            {
                CompNutritionLoader loader = ((ThingWithComps)target).TryGetComp<CompNutritionLoader>();
                if (loader != null)
                {
                    loader.torpSpawn.Add(2);
                }
            }
            return;
        }*/
        void IExposable.ExposeData()
        {

        }
    }
}
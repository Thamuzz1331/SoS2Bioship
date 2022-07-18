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
    public class OcularPerk : IHediff
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
                heart.defs.TryGetValue("spinalTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("GiantEyeLaser"));
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                CompMutationWorker mut = target.parent.TryGetComp<CompMutationWorker>();
                mut.quirkPossibilities.Remove(this);
            }

        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {

        }
        /*
        void Apply(CompShipHeart target)
        {
            target.defOptions.TryGetValue(ThingDef.Named("HeavySpineLauncher"), new List<ThingDef>()).Add(ThingDef.Named("Spine_HeavyDense"));

            target.organOptions["largeTurretOptions"].Add(ThingDef.Named("HeavySpineLauncher"));
            target.organOptions["largeTurretOptions"].Add(ThingDef.Named("HeavySpineLauncher"));

            target.RemoveMutation<DenseSpines>("offense", "bone", true);
            target.mutationThemes["bone"] += 1;
            return;
        }*/
        void IExposable.ExposeData()
        {

        }

    }
}
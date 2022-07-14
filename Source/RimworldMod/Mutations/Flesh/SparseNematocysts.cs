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
    public class SparseNematocysts : IHediff
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
                heart.defs.TryGetValue("smallTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("ShipTurret_SparseNematocyst"));
                heart.defs.TryGetValue("smallTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("ShipTurret_SparseNematocyst"));

            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<ClusteredNematocysts>("offense", "flesh", true);
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<SparseNematocysts>("offense", "flesh", false);
            }

        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {

        }
        /*
        void Apply(CompShipHeart target)
        {
            target.organOptions["smallTurretOptions"] = target.organOptions["smallTurretOptions"].FindAll(e => e != ThingDef.Named("ShipTurret_Nematocyst"));
            target.organOptions["smallTurretOptions"].Add(ThingDef.Named("ShipTurret_SparseNematocyst"));

            target.RemoveMutation<SparseNematocysts>("offense", "flesh", true);
            target.RemoveMutation<ClusteredNematocysts>("offense", "flesh", false);

            target.mutationThemes["flesh"] += 1;
            return;
        }*/
        void IExposable.ExposeData()
        {

        }

    }
}
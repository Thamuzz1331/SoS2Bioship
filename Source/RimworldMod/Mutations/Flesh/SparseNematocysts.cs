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
    public class SparseNematocysts : IMutation
    {
        bool IMutation.RunOnBodyParts()
        {
            return false;
        }
        void IMutation.Apply(Building_ShipHeart target)
        {
            target.organOptions["smallTurretOptions"] = target.organOptions["smallTurretOptions"].FindAll(e => e != ThingDef.Named("ShipTurret_Nematocyst"));
            target.organOptions["smallTurretOptions"].Add(ThingDef.Named("ShipTurret_SparseNematocyst"));

            target.RemoveMutation<SparseNematocysts>("offense", "flesh", true);
            target.RemoveMutation<ClusteredNematocysts>("offense", "flesh", false);

            target.mutationThemes["flesh"] += 1;
            return;
        }
        void IMutation.Apply(Thing target)
        {
            return;
        }
        void IExposable.ExposeData()
        {

        }

    }
}
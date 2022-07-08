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
    public class EfficientGrowth : IMutation
    {
        bool IMutation.RunOnBodyParts()
        {
            return false;
        }
        void IMutation.Apply(Building_ShipHeart target)
        {
            target.statMultipliers.Add("growthCost", 0.75f);

            target.RemoveMutation<EfficientGrowth>("utility", "misc", true);

            return;
        }
        void IMutation.Apply(Thing target)
        {
            return;
        }
    }
}
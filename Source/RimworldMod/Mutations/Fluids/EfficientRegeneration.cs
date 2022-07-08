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
    public class EfficientRegeneration : IMutation
    {
        bool IMutation.RunOnBodyParts()
        {
            return false;
        }
        void IMutation.Apply(Building_ShipHeart target)
        {
            target.statMultipliers.Add("regenCost", 0.85f);

            target.RemoveMutation<EfficientRegeneration>("defense", "humors", true);

            target.mutationThemes["humors"] += 1;
            return;
        }
        void IMutation.Apply(Thing target)
        {
            return;
        }
    }
}
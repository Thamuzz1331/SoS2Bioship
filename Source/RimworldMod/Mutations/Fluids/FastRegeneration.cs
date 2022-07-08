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
    public class FastRegeneration : IMutation
    {
        bool IMutation.RunOnBodyParts()
        {
            return false;
        }
        void IMutation.Apply(Building_ShipHeart target)
        {
            target.statMultipliers.Add("regenInterval", 0.85f);

            target.RemoveMutation<FastRegeneration>("defense", "humors", true);

            target.mutationThemes["humors"] += 1;
            return;
        }
        void IMutation.Apply(Thing target)
        {
            return;
        }
    }
}
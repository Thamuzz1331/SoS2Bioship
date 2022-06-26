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
    public class ReinforcedBone : IMutation
    {
        bool IMutation.RunOnBodyParts()
        {
            return true;
        }
        void IMutation.Apply(Building_ShipHeart target)
        {
            List<IMutation> mutations = target.goodMutationOptions.TryGetValue("defense", new Dictionary<string, List<IMutation>>())
                .TryGetValue("bone", new List<IMutation>());
            target.goodMutationOptions
                .TryGetValue("defense", new Dictionary<string, List<IMutation>>())["bone"] = mutations.FindAll(e => !(e is ReinforcedBone));
            target.mutationThemes["bone"] += 1;
            return;
        }
        void IMutation.Apply(Thing target)
        {
            return;
        }
    }
}
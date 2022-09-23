using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
    public interface IMutation : IExposable
    {
        bool ShouldAddTo(CompBuildingBodyPart target);
        void Apply(CompBuildingBodyPart target);
        void Remove(CompBuildingBodyPart target);
        List<Tuple<IMutation, string, string>> GetMutationsForTier(string tier, List<IMutation> existingMutations);
        String GetTier();
        String GetDescription();
        Texture2D GetIcon();
    }
}
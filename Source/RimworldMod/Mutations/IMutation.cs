using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
    public interface IMutation : IHediff
    {
        List<Tuple<IMutation, string, string>> GetMutationsForTier(string tier, List<IMutation> existingMutations);
        String GetTier();
        String GetDescription();
        Texture2D GetIcon();
    }
}
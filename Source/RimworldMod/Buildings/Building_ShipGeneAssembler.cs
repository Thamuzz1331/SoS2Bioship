using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class Building_ShipGeneAssembler : Building
    {
        public List<CompHeartSeed> GetGenepacks(bool a, bool b)
        {
            List<CompHeartSeed> ret = new List<CompHeartSeed>();
            return ret;
        }

        public int MaxComplexity()
        {
            return 6;
        }
    }
}
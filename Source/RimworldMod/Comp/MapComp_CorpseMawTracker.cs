using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld.Planet;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CorpseMawTracker : MapComponent
    {
        public CorpseMawTracker(Map map) : base(map)
        {
        }

        public List<Thing> corpseMaws = new List<Thing>();
        public List<CompHeartSeed> heartSeeds = new List<CompHeartSeed>();
    }
}
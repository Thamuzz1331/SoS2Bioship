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
    public class Luciferium : IHediff
    {
        public CompShipHeart heart = null;

        bool IHediff.ShouldAddTo(CompBuildingBodyPart target) {
            return false;
        }
        void IHediff.Apply(CompBuildingBodyPart target)
        {
            return;
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {
            return;
        }

        Dictionary<string, float> statMults = new Dictionary<string, float>()
        {
            {"metabolicEfficiency", 1.5f},
            {"metabolicSpeed", 1.5f},
            {"conciousness", 1.25f }
        };


        float IHediff.StatMult(string stat)
        {
            if (heart.luciferiumSupplied)
            {
                return statMults.TryGetValue(stat, 1f);
            } else
            {
                return 0.85f;
            }
        }
        void IExposable.ExposeData()
        {
            Scribe_Values.Look(ref heart, "heart");
        }

    }
}
using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    public class Hediff_Luciferium : Hediff_Building
    {
        public bool satisfied = false;

        Dictionary<string, float> statMults = new Dictionary<string, float>()
        {
            {"metabolicEfficiency", 1.5f},
            {"metabolicSpeed", 1.5f},
            {"conciousness", 1.25f }
        };

     	public override float StatMod(string stat)
        {
            if (satisfied)
            {
                return statMults.TryGetValue(stat, 1f);
            } else
            {
                return 0.85f;
            }
        }
    }
}
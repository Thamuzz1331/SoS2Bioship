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
    public class BuildingHediff_LuciferiumDrug : BuildingHediff_Drug
    {
        public bool satisfied = true;

        public override void Tick()
        {
        }


        public override float StatMod(string stat)
        {
            if (satisfied)
            {
                return statMods.TryGetValue(stat, 1f);
            } else
            {
                return 0.85f;
            }
        }
    }
}
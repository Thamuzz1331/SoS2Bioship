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
    public class BuildingHediff_LuciferiumAddiction : Building_Addiction, IAggressionSource
    {
        public override void PostSpawnSetup(bool r)
        {
            base.PostSpawnSetup(r);
            if (r)
            {
                ((CompShipHeart)bp.Core).aggression.aggressionSources.Add(this);
            }
        }

        public override void PostAdd()
        {
            base.PostAdd();
            ((CompShipHeart)bp.Core).aggression.aggressionSources.Add(this);
        }

        public override void PostRemove()
        {
            base.PostRemove();
            ((CompShipHeart)bp.Core).aggression.aggressionSources.Remove(this);
        }

        int IAggressionSource.GetAggressionValue()
        {
            if (this.withdrawl > maxWithdrawl/2)
            {
                return 999;
            }
            return 0;
        }

    }
}
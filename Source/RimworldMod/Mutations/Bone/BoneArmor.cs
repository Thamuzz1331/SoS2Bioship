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
    public class BoneArmor : IHediff
    {
        bool IHediff.RunOnBodyParts()
        {
            return false;
        }
        void IHediff.Apply(Building_ShipHeart target)
        {
            target.armorClass = ThingDef.Named("BoneArmor");

            target.RemoveMutation<BoneArmor>("defense", "bone", true);

            return;
        }
        void IHediff.Apply(Thing target)
        {
            return;
        }
        void IExposable.ExposeData()
        {

        }
    }

}
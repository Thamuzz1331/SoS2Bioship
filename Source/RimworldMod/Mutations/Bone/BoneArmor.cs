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
    public class BoneArmor : IMutation
    {
        bool IMutation.RunOnBodyParts()
        {
            return false;
        }
        void IMutation.Apply(Building_ShipHeart target)
        {
            target.armorClass = ThingDef.Named("BoneArmor");

            target.RemoveMutation<BoneArmor>("defense", "bone", true);

            return;
        }
        void IMutation.Apply(Thing target)
        {
            return;
        }
        void IExposable.ExposeData()
        {

        }
    }

}
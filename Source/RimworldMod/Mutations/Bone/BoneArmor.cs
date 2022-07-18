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
        bool IHediff.ShouldAddTo(CompBuildingBodyPart target)
        {
            bool ret = false;
            ret = ret || (target.parent.TryGetComp<CompArmorGrower>() != null);
            ret = ret || (target.parent.TryGetComp<CompMutationWorker>() != null);
            return ret;
        }
       
        void IHediff.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompArmorGrower>() != null)
            {
                target.parent.TryGetComp<CompArmorGrower>().armorClass = ThingDef.Named("BoneArmor");
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<BoneArmor>("defense", "bone", true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["bone"]++;
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompArmorGrower>() != null && 
                target.parent.TryGetComp<CompArmorGrower>().armorClass == ThingDef.Named("BoneArmor"))
            {
                target.parent.TryGetComp<CompArmorGrower>().armorClass = null;
            }
        }

        void IExposable.ExposeData()
        {

        }
    }

}
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
    public class PlasteelArmor : IMutation
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
                target.parent.TryGetComp<CompArmorGrower>().armorClass = ThingDef.Named("PlasteelArmor");
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<PlasteelArmor>("defense", "bone", true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["bone"]++;
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompArmorGrower>() != null &&
                target.parent.TryGetComp<CompArmorGrower>().armorClass == ThingDef.Named("PlasteelArmor"))
            {
                target.parent.TryGetComp<CompArmorGrower>().armorClass = null;
            }
        }

        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations)
        {
            return new List<Tuple<IMutation, string, string>>() { };
        }
        String IMutation.GetTier()
        {
            return "tier2";
        }
        String IMutation.GetDescription()
        {
            return "Plasteeel Armor\nAllows the bioship to grow a layer of plasteel armor";
        }
        public override String ToString()
        {
            return "Plasteeel Armor";
        }

        Texture2D IMutation.GetIcon()
        {
            return null;
        }
        void IExposable.ExposeData()
        {

        }
    }

}
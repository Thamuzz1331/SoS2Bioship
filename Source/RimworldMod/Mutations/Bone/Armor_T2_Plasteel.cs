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
    public class PlasteelArmor : IMutation
    {
        bool IMutation.ShouldAddTo(CompBuildingBodyPart target)
        {
            bool ret = false;
            ret = ret || (target.parent.TryGetComp<CompArmorGrower>() != null);
            ret = ret || (target.parent.TryGetComp<CompMutationWorker>() != null);
            return ret;
        }

        void IMutation.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompArmorGrower>() != null)
            {
                target.parent.TryGetComp<CompArmorGrower>().armorClass.Add(ThingDef.Named("PlasteelCarapace"));
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<PlasteelArmor>("defense", "bone");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["bone"]++;
            }
        }
        void IMutation.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompArmorGrower>() != null)
            {
                target.parent.TryGetComp<CompArmorGrower>().armorClass.Remove(ThingDef.Named("PlasteelCarapace"));
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("defense", "bone", this);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["bone"]--;
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
            return "Plasteeel Carapace\nAllows the bioship to grow layers of plasteel carapace";
        }
        public override String ToString()
        {
            return "Plasteeel Carapace";
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
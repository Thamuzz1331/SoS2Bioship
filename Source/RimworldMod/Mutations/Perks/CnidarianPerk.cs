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
    public class AstralCniderian : IMutation
    {
        bool IMutation.ShouldAddTo(CompBuildingBodyPart target)
        {
            bool ret = false;
            ret = ret || (target.parent.TryGetComp<CompShipHeart>() != null);
            ret = ret || (target.parent.TryGetComp<CompArmorGrower>() != null);
            ret = ret || (target.parent.TryGetComp<CompMutationWorker>() != null);
            return ret;
        }

        void IMutation.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompArmorGrower>() != null)
            {
                target.parent.TryGetComp<CompArmorGrower>().armorClass.Add(ThingDef.Named("CoralArmor"));
            }
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.defs.TryGetValue("spinalTurretOptions", new DefOptions(new List<ThingDef>()))
                    .defs.Add(ThingDef.Named("GrandNematocyst"));
//                BuildingHediff toAdd = new Building();
//                toAdd.label = "Astral Cniderian";
//                toAdd.visible = true;
//                toAdd.statMods = new Dictionary<string, float>()
//                {
//                };
 //               heart.AddHediff(toAdd);
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<BoneArmor>("defense", "bone");
            }

        }
        void IMutation.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.defs.TryGetValue("spinalTurretOptions", new DefOptions(new List<ThingDef>()))
                    .defs.Remove(ThingDef.Named("GiantEyeLaser"));
                heart.RemoveHediff("Mutant Eyebeast");
            }

        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations)
        {
            return new List<Tuple<IMutation, string, string>>() { };
        }
        String IMutation.GetTier()
        {
            return "quirk";
        }
        String IMutation.GetDescription()
        {
            return "Astral Cniderian Lineage\nThis beast belongs to the cniderian family.  It possesses particularly powerful nematocysts and can grow layers of coral armor.";
        }
        public override string ToString()
        {
            return "Astral Cniderian Lineage";
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
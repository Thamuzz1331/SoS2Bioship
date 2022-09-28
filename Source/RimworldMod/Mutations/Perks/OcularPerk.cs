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
    public class OcularPerk : IMutation
    {
        bool IMutation.ShouldAddTo(CompBuildingBodyPart target)
        {
            bool ret = false;
            ret = ret || (target.parent.TryGetComp<CompShipHeart>() != null);
            return ret;
        }

        void IMutation.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.defs.TryGetValue("spinalTurretOptions", new DefOptions(new List<ThingDef>()))
                    .defs.Add(ThingDef.Named("GiantEyeLaser"));
                Hediff_Building toAdd = new Hediff_Building();
                toAdd.label = "Mutant Eyebeast";
                toAdd.visible = true;
                toAdd.statMods = new Dictionary<string, float>()
                {
                    {"metabolicEfficiency", 1.15f},
                };
                heart.AddHediff(toAdd);
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
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            return new List<Tuple<IMutation, string, string>>() { };
        }
        String IMutation.GetTier() {
            return "quirk";
        }
        String IMutation.GetDescription()
        {
            return "Occular Lineage\nThis beast belongs to the occular family, possessing a powerful eye laser.";
        }
        public override string ToString()
        {
            return "Occular Lineage";
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
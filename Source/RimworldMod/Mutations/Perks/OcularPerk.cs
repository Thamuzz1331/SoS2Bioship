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
            ret = ret || (target.parent.TryGetComp<CompMutationWorker>() != null);
            return ret;
        }

        void IMutation.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.defs.TryGetValue("spinalTurretOptions", new DefOptions(new List<ThingDef>()))
                    .defs.Add(ThingDef.Named("GiantEyeLaser"));
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                CompMutationWorker mut = target.parent.TryGetComp<CompMutationWorker>();
            }

        }
        void IMutation.Remove(CompBuildingBodyPart target)
        {

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
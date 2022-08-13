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
    public class EnergizedPlasma : IMutation
    {
        bool IHediff.ShouldAddTo(CompBuildingBodyPart target)
        {
            bool ret = false;
            ret = ret || (target.parent.TryGetComp<CompShipHeart>() != null);
            ret = ret || (target.parent.TryGetComp<CompMutationWorker>() != null);
            return ret;
        }

        void IHediff.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.defs.TryGetValue("mediumTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("ShipTurret_BioPlasmaEnergetic"));
                heart.defs.TryGetValue("mediumTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("ShipTurret_BioPlasmaEnergetic"));
                heart.defs.TryGetValue("mediumTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("ShipTurret_BioPlasmaEnergetic"));
                heart.defs.TryGetValue("mediumTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("ShipTurret_BioPlasmaEnergetic"));
                heart.defs["mediumTurretOptions"] = heart.defs["mediumTurretOptions"].FindAll(e => !(e == ThingDef.Named("ShipTurret_BioPlasma")));

            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<EnergizedPlasma>("offense", "humors", true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]++;
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {

        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            return new List<Tuple<IMutation, string, string>>() { };
        }
        String IMutation.GetTier() {
            return "tier1";
        }
        String IMutation.GetDescription()
        {
            return "Energized Plasma\nMedium weapon scaffolds now have a chance to grow an energized plasma maw.  Energized plasma does extra damage to shields.";
        }
        public override String ToString()
        {
            return "Energized Plasma";
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
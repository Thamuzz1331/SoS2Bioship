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
            ret = ret || (target.parent.def == ThingDef.Named("ShipTurret_BioPlasma"));
            ret = ret || (target.parent.TryGetComp<CompShipHeart>() != null);
            ret = ret || (target.parent.TryGetComp<CompMutationWorker>() != null);
            return ret;
        }

        void IHediff.Apply(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                target.parent.TryGetComp<CompShipHeart>().defs.TryGetValue("mediumTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("ShipTurret_BioPlasma"));
                target.parent.TryGetComp<CompShipHeart>().defs.TryGetValue("mediumTurretOptions", new List<ThingDef>())
                    .Add(ThingDef.Named("ShipTurret_BioPlasma"));
            }
            if (target.parent.def == ThingDef.Named("ShipTurret_BioPlasma"))
            {
                target.parent.TryGetComp<CompMutableAmmo>().projectileDef = ThingDef.Named("Proj_BioPlasmaEnergetic");
                target.parent.TryGetComp<CompMutableAmmo>().fakeProjectileDef= ThingDef.Named("Bullet_Fake_BioPlasmaEnergetic");
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<EnergizedPlasma>("offense", "humors", true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]++;
            }
        }
        void IHediff.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                target.parent.TryGetComp<CompShipHeart>().defs.TryGetValue("mediumTurretOptions", new List<ThingDef>())
                    .Remove(ThingDef.Named("ShipTurret_BioPlasma"));
                target.parent.TryGetComp<CompShipHeart>().defs.TryGetValue("mediumTurretOptions", new List<ThingDef>())
                    .Remove(ThingDef.Named("ShipTurret_BioPlasma"));
            }
            if (target.parent.def == ThingDef.Named("ShipTurret_BioPlasma"))
            {
                target.parent.TryGetComp<CompMutableAmmo>().projectileDef = null;
                target.parent.TryGetComp<CompMutableAmmo>().fakeProjectileDef = null;
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<EnergizedPlasma>("offense", "humors", true);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"]--;
            }

        }
        List<Tuple<IMutation, string, string>> IMutation.GetMutationsForTier(string tier, List<IMutation> existingMutations) {
            if (tier == "tier2")
            {
                return new List<Tuple<IMutation, string, string>>() {new Tuple<IMutation, string, string>(
                    new ShieldbusterPlasma(),
                    "offense",
                    "humors") };
            } else
            {
                return new List<Tuple<IMutation, string, string>>();
            }
        }
        String IMutation.GetTier() {
            return "tier1";
        }
        String IMutation.GetDescription()
        {
            return "Energized Plasma\nPlasma Maws deal additional damage.";
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
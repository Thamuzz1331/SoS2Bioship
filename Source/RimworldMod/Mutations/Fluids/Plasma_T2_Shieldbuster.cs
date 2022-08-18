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
    public class ShieldbusterPlasma : IMutation
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
            }
            if (target.parent.def == ThingDef.Named("ShipTurret_BioPlasma"))
            {
                target.parent.TryGetComp<CompMutableAmmo>().projectileDef = ThingDef.Named("Proj_BioPlasmaShieldbuster");
                target.parent.TryGetComp<CompMutableAmmo>().fakeProjectileDef= ThingDef.Named("Bullet_Fake_BioPlasmaShieldbuster");
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
            return new List<Tuple<IMutation, string, string>>() { };
        }
        String IMutation.GetTier() {
            return "tier2";
        }
        String IMutation.GetDescription()
        {
            return "Shieldbuster Plasma\nPlasma Maws significatly increase the damage they do to shields.";
        }
        public override String ToString()
        {
            return "Shieldbuster Plasma";
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
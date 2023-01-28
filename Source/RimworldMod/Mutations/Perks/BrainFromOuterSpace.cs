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
    public class GiantBrainFromOuterSpace : IMutation
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
                    .defs.Add(ThingDef.Named("ThirdEye"));
                heart.defs.TryGetValue("shieldEmitter", new DefOptions(new List<ThingDef>())).defs.Clear();
                heart.defs.TryGetValue("shieldEmitter", new DefOptions(new List<ThingDef>()))
                    .defs.Add(ThingDef.Named("BioShieldGeneratorMotive"));

                BuildingHediff toAdd = new BuildingHediff();
                toAdd.visible = true;
                toAdd.statMods = new Dictionary<string, float>()
                {
                    {"conciousness", 1.15f},
                    {"metabolicSpeed", 0.85f},
                    {"metabolicEfficiency", 0.85f},
                };
                heart.AddHediff(toAdd);
            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().RemoveMutation<IronWill>("defense", "psi");
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["psi"] += 4;
            }

        }
        void IMutation.Remove(CompBuildingBodyPart target)
        {
            if (target.parent.TryGetComp<CompShipHeart>() != null)
            {
                CompShipHeart heart = target.parent.TryGetComp<CompShipHeart>();
                heart.defs.TryGetValue("spinalTurretOptions", new DefOptions(new List<ThingDef>()))
                    .defs.Remove(ThingDef.Named("ThirdEye"));
                heart.defs.TryGetValue("shieldEmitter", new DefOptions(new List<ThingDef>())).defs.Clear();
                heart.defs.TryGetValue("shieldEmitter", new DefOptions(new List<ThingDef>()))
                    .defs.Add(ThingDef.Named("BioShieldGenerator"));
                heart.RemoveHediff("Giant Brain From Outer Space");

            }
            if (target.parent.TryGetComp<CompMutationWorker>() != null)
            {
                target.parent.TryGetComp<CompMutationWorker>().AddMutation("defense", "psi", this);
                target.parent.TryGetComp<CompMutationWorker>().mutationThemes["humors"] -= 4;
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
            return "Giant Brain From Outer Space\nA psi focused lineage.  The Third Eye spinal weapon bypasses hull to unleash psychic fury against enemy crews while motive shield ganglia add thrust.";
        }
        public override string ToString()
        {
            return "Giant Brain From Outer Space";
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
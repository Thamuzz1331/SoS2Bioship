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
    public class CompShipHeart : CompBuildingCore
    {
        public CompProperties_ShipHeart HeartProps => (CompProperties_ShipHeart)props;

        public CompRegenWorker regenWorker;
        public CompMutationWorker mutator;
        public CompAggression aggression;
        public CompArmorGrower armorGrower;
        public bool initialized = false;

        public Dictionary<string, List<ThingDef>> defs = new Dictionary<string, List<ThingDef>>()
        {
            {"smallTurretOptions", new List<ThingDef>(){
                ThingDef.Named("ShipTurret_Nematocyst")
            }},
            {"mediumTurretOptions", new List<ThingDef>(){
                ThingDef.Named("ShipTurret_BioPlasma"), ThingDef.Named("ShipTurret_BioAcid"),
                ThingDef.Named("ShipTurret_BioPlasma"), ThingDef.Named("ShipTurret_BioAcid"),
            }},
            {"largeTurretOptions", new List<ThingDef>(){
                ThingDef.Named("HeavySpineLauncher"), ThingDef.Named("HeavySpineLauncher"),
                ThingDef.Named("LightSpineLauncher"), ThingDef.Named("LightSpineLauncher"),
            }},
            {"spinalTurretOptions", new List<ThingDef>(){}},
            {"smallMawOptions", new List<ThingDef>(){
                ThingDef.Named("Maw_Small"), ThingDef.Named("Maw_Small"),
            }},
            {"HeavySpineLauncher", new List<ThingDef>(){
                ThingDef.Named("Spine_Heavy")
            }},
            {"LightSpineLauncher", new List<ThingDef>(){
                ThingDef.Named("Spine_Light")
            }},
        };

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref initialized, "initialized", false);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            Log.Message("!");
            if (!stats.ContainsKey("regenEfficiency"))
                stats.Add("regenEfficiency", 1f);
            if (!stats.ContainsKey("regenSpeed"))
                stats.Add("regenSpeed", 1f);
            regenWorker = parent.TryGetComp<CompRegenWorker>();
            mutator = parent.TryGetComp<CompMutationWorker>();
            aggression = parent.TryGetComp<CompAggression>();
            armorGrower = parent.TryGetComp<CompArmorGrower>();
            base.PostSpawnSetup(respawningAfterLoad);
            Log.Message(respawningAfterLoad + " Spawning heart " + this + " on map " + parent.Map + " body " + body + " bodyId " + bodyId);
            regenWorker.body = this.body;
            mutator.body = this.body;
            armorGrower.body = this.body;
            if (!respawningAfterLoad && !initialized)
            {
                mutator.SpreadMutation(this.body, mutator.quirkPossibilities[Rand.Range(0, mutator.quirkPossibilities.Count)]);
                mutator.SpreadMutation(this.body, mutator.RollMutation("offense", mutator.GetRandomTheme(mutator.mutationThemes, mutator.goodMutationOptions.TryGetValue("offense")), mutator.goodMutationOptions));
                mutator.SpreadMutation(this.body, mutator.RollMutation("defense", mutator.GetRandomTheme(mutator.mutationThemes, mutator.goodMutationOptions.TryGetValue("defense")), mutator.goodMutationOptions));
                mutator.SpreadMutation(this.body, mutator.RollMutation("utility", mutator.GetRandomTheme(mutator.mutationThemes, mutator.goodMutationOptions.TryGetValue("utility")), mutator.goodMutationOptions));
                initialized = true; //When the ship takes off the comps get regenerated.  This ensures that the initial mutations will only proc once.
            }
            if (!respawningAfterLoad)
            {
                if (parent.TryGetComp<CompShipNutritionStore>() != null)
                {
                    parent.TryGetComp<CompShipNutritionStore>().SetId(this.bodyId);
                    ((MapCompBuildingTracker)parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(parent.TryGetComp<CompShipNutritionStore>());
                }
            }
            if (respawningAfterLoad)
            {
                foreach (Thing t in body.bodyParts)
                {
                    CompShipBodyPart bp = t.TryGetComp<CompShipBodyPart>();
                    if (bp != null)
                    {
                        foreach (Thing a in bp.adjMechs)
                        {
                            AggressionTarget(a, true);
                        }
                        bp.adjMechs.Clear();
                        foreach (Thing a in bp.adjBodypart)
                        {
                            AggressionTarget(a, false);
                        }
                        bp.adjBodypart.Clear();
                    }
                }
            } else
            {

            }
        }
        /*
        public override void CompTick()
        {
            base.CompTick();
            if (this.body == null)
            {
                ((MapCompBuildingTracker)this.parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(this);
            }
        }
        */
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (body != null)
            {
                List<Thing> toWhither = new List<Thing>();
                foreach (Thing t in body.bodyParts)
                {
                    toWhither.Add(t);
                }
                foreach (Thing t in toWhither)
                {
                    t.TryGetComp<CompShipBodyPart>().Whither();
                }
            }
            base.PostDestroy(mode, previousMap);
        }


        public override void DoHunger()
        {
            if (this.body.bodyParts.Count > 0)
            {
                this.body.bodyParts.ElementAt(Rand.Range(0, this.body.bodyParts.Count)).TryGetComp<CompShipBodyPart>().Whither();
            }
            this.hungerDuration = 0;
        }

        public override string GetSpecies()
        {
            return HeartProps.shipspecies;
        }

        public virtual ThingDef GetThingDef(string cat)
        {
            List<ThingDef> def = defs.TryGetValue(cat, null);
            if (def != null)
            {
                return def[Rand.Range(0, def.Count)];
            }
            return null;
        }

        public virtual void Regen(Thing toRegen)
        {
            regenWorker.RegisterWound(toRegen);
        }

        public virtual void AggressionTarget(Thing target, bool mechanical)
        {
            if (mechanical)
            {
                aggression.adjacentMechanicals.Add(target);
            } else
            {
                aggression.otherFlesh.Add(target);
            }
        }

        public override float GetStat(string stat)
        {
            switch (stat)
            {
                case "regenEfficiency": 
                    return stats.TryGetValue(stat, 1f) * stats.TryGetValue("metabolicEfficiency", 1f);
                case "regenSpeed":
                    return stats.TryGetValue(stat, 1f) * stats.TryGetValue("metabolicSpeed", 1f);
                default:
                    return base.GetStat(stat);
            }
        }
    }
}
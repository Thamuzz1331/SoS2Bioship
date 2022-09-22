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

        StatDef injectors = StatDef.Named("LuciferInjectors");

        public bool luciferiumAddiction = false;
        public bool luciferiumSupplied = false;
        public CompRegenWorker regenWorker;
        public CompMutationWorker mutator;
        public CompAggression aggression;
        public CompArmorGrower armorGrower;
        public bool initialized = false;
        public float maxResistence = 0.15f;

        public Dictionary<DamageDef, float> resistances = new Dictionary<DamageDef, float>();

        public List<Gizmo> mutationActions = new List<Gizmo>();

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
            Scribe_Values.Look<bool>(ref luciferiumAddiction, "luciferiumAddiction", false);
            Scribe_Values.Look<bool>(ref luciferiumSupplied, "luciferiumSupplied", false);
            Scribe_Collections.Look(ref resistances, "resistences", LookMode.Deep);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!stats.ContainsKey("regenEfficiency"))
                stats.Add("regenEfficiency", 1f);
            if (!stats.ContainsKey("regenSpeed"))
                stats.Add("regenSpeed", 1f);
            if (!stats.ContainsKey("conciousness"))
                stats.Add("conciousness", 1f);
            if (!stats.ContainsKey("movementSpeed"))
                stats.Add("movementSpeed", 1f);
            regenWorker = parent.TryGetComp<CompRegenWorker>();
            mutator = parent.TryGetComp<CompMutationWorker>();
            if (mutator.tier != "tier1")
            {
                mutator.UpgradeMutationTier(mutator.tier);
            }
            aggression = parent.TryGetComp<CompAggression>();
            armorGrower = parent.TryGetComp<CompArmorGrower>();
            base.PostSpawnSetup(respawningAfterLoad);
            regenWorker.body = this.body;
            mutator.body = this.body;
            armorGrower.body = this.body;
            if (!respawningAfterLoad && !initialized)
            {
                mutator.GetInitialMutations(this.body);
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
            }
        }
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
        public override string CompInspectStringExtra()
        {
            return String.Format("Heart of of {0}", this.bodyName);
        }


        public override void DoHunger()
        {
            if (this.body.bodyParts.Count > 0)
            {
                this.body.bodyParts.RandomElement().TryGetComp<CompShipBodyPart>().Whither();
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
                return def.RandomElement();
            }
            return null;
        }

        public virtual void AggressionTarget(Thing target, bool mechanical)
        {
            if (target.TryGetComp<CompRegenSpot>() != null)
            {
                return;
            }
            if (mechanical)
            {
                aggression.adjacentMechanicals.Add(target);
            } else
            {
                aggression.otherFlesh.Add(target);
            }
        }

        public float GetDamageMult(DamageInfo dinfo)
        {
            if (resistances == null)
            {
                resistances = new Dictionary<DamageDef, float>();
            }
            return resistances.TryGetValue(dinfo.Def, 0f);
        }

        public void GainResistance(DamageInfo dinfo)
        {
            if (resistances == null)
            {
                resistances = new Dictionary<DamageDef, float>();
            }
            if (resistances.TryGetValue(dinfo.Def, 0f) < maxResistence)
            {
                if (!resistances.ContainsKey(dinfo.Def))
                {
                    resistances.Add(dinfo.Def, 0f);
                }
                resistances[dinfo.Def] += 0.0025f;
            }
        }

        public override float GetStat(string stat)
        {
            float ret = base.GetStat(stat);
            float luciferMult = 1f;
            if (luciferiumAddiction && luciferiumSupplied)
            {
                luciferMult = 1.5f;
            }
            switch (stat)
            {

                case "metabolicEfficiency":
                    return ret * luciferMult;
                case "metabolicSpeed":
                    return ret * luciferMult;
                case "regenEfficiency": 
                    return ret * GetStat("metabolicEfficiency");
                case "regenSpeed":
                    return ret * GetStat("metabolicSpeed");
                case "movementSpeed":
                    return ret * GetStat("conciousness");
                default:
                    return base.GetStat(stat);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}
            foreach (Gizmo gizmo in mutationActions)
            {
                yield return gizmo;
            }
		}

    }
}
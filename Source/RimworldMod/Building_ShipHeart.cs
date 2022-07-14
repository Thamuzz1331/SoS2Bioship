using HarmonyLib;
using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimworldMod;


namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class Building_ShipHeart : Building_ShipBridge
    {
        public ShipBody body = null;
        public String heartId = "NA";
        public bool mutationsDone = false;
        public float hungerDuration = 0;
        private int aggressionLevel = 1;
        public ThingDef armorClass = null;
        StatDef inducers = StatDef.Named("MutationInducers");


        public List<IMutation> mutations = new List<IMutation>() {
        };




        public Dictionary<string, float> statMultipliers = new Dictionary<string, float>();
        public Dictionary<ThingDef, Dictionary<string, float>> specStatMultipliers = new Dictionary<ThingDef, Dictionary<string, float>>();
        public Dictionary<string, List<ThingDef>> organOptions = new Dictionary<string, List<ThingDef>>()
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
        };
        public Dictionary<ThingDef, List<ThingDef>> defOptions = new Dictionary<ThingDef, List<ThingDef>>()
        {
            {ThingDef.Named("HeavySpineLauncher"), new List<ThingDef>(){
                ThingDef.Named("Spine_Heavy")
            }},
            {ThingDef.Named("LightSpineLauncher"), new List<ThingDef>(){
                ThingDef.Named("Spine_Light")
            }},
        };
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                heartId = Guid.NewGuid().ToString();
                Scribe_Values.Look<String>(ref heartId, "heartId", "NA");
            }
            foreach (IMutation mutation in mutations)
            {
                mutation.Apply(this);
            }
            if (!respawningAfterLoad)
            {
                IMutation offMutation = RollMutation("offense", GetRandomTheme(mutationThemes, goodMutationOptions["offense"]), goodMutationOptions);
                if (offMutation != null)
                {
                    offMutation.Apply(this);
                    mutations.Add(offMutation);
                }
                IMutation defMutation = RollMutation("defense", GetRandomTheme(mutationThemes, goodMutationOptions["defense"]), goodMutationOptions);
                if (defMutation != null)
                {
                    defMutation.Apply(this);
                    mutations.Add(defMutation);
                }
                IMutation utlMutation = RollMutation("utility", GetRandomTheme(mutationThemes, goodMutationOptions["utility"]), goodMutationOptions);
                if (utlMutation != null)
                {
                    utlMutation.Apply(this);
                    mutations.Add(utlMutation);
                }
            }            
            ((ShipBodyMapComp)Map.components.Where(t => t is ShipBodyMapComp).FirstOrDefault()).Register(this);
            if (!respawningAfterLoad)
            {
                if(this.TryGetComp<CompShipNutritionStore>() != null)
                {
                    this.TryGetComp<CompShipNutritionStore>().SetId(this.heartId);
                    ((ShipBodyMapComp)Map.components.Where(t => t is ShipBodyMapComp).FirstOrDefault()).Register(this.TryGetComp<CompShipNutritionStore>());
                }
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<String>(ref heartId, "heartId", "NA");
            Scribe_Values.Look<float>(ref hungerDuration, "hungerDuration", 0f);
            Scribe_Collections.Look<IMutation>(ref mutations, "mutations", LookMode.Deep);
        }

        public float getStatMultiplier(string stat, ThingDef src)
        {
            if (src != null)
            {
                return statMultipliers.TryGetValue(stat, 1f) * specStatMultipliers.TryGetValue(src, new Dictionary<string, float>()).TryGetValue(stat, 1f);
            }
            return statMultipliers.TryGetValue(stat, 1f);
        }

        public virtual int GetAggressionLevel()
        {
            return aggressionLevel;
        }


        public virtual ThingDef GetThingDef(ThingDef def)
        {
            List<ThingDef> defs = defOptions.TryGetValue(def, null);
            if (defs != null)
            {
                return defs[Rand.Range(0, defs.Count)];
            }

            return null;
        }



        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach(Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            if((int)this.GetStatValue(inducers) > 0)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Induce Mutation",
				    action = delegate()
				    {
					    this.InduceMutation();
				    }
                };
            }
            if (this.armorClass != null)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Grow Armor",
                    action = delegate()
                    {
                        body.GrowArmor();
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "Shed Armor",
                    action = delegate()
                    {
                        body.ShedArmor();
                    }
                };
            }
        }

        public override string GetInspectString()
        {
            string ret = base.GetInspectString();
            ret += "\nMutations:";
            foreach (IMutation m in mutations)
            {
                ret += "\n" + m;
            }

            return ret;
        }
    }

}
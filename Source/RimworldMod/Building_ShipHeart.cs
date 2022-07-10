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

        public Dictionary<string, int> mutationThemes = new Dictionary<string, int>()
        {
            {"flesh", 3},
            {"bone", 3},
            {"humors", 3},
            {"misc", 2}
        };
        public Dictionary<string, int> categoryOdds = new Dictionary<string, int>()
        {
            {"offense", 3},
            {"defense", 3},
            {"utility", 2},
            {"quirk", 1}
        };
        public List<IMutation> quirkPossibilities = new List<IMutation>()
        {

        };

        public Dictionary<string, Dictionary<string, List<IMutation>>> goodMutationOptions = new Dictionary<string, Dictionary<string, List<IMutation>>>()
        {
            {"offense", new Dictionary<string, List<IMutation>>(){
                { "flesh", new List<IMutation>(){
                    new ClusteredNematocysts(),
                }},
                { "bone", new List<IMutation>(){
                    new DenseSpines(), new EfficientSpines(),
                }},
                { "humors", new List<IMutation>(){

                }},
                { "misc", new List<IMutation>(){

                }}
            }},
            {"defense", new Dictionary<string, List<IMutation>>(){
                { "flesh", new List<IMutation>(){

                }},
                { "bone", new List<IMutation>(){
                    new BoneArmor(),
                }},
                { "humors", new List<IMutation>(){
                    new FastRegeneration(), new EfficientRegeneration(),
                }},
                { "misc", new List<IMutation>(){

                }}
            }},
            {"utility", new Dictionary<string, List<IMutation>>(){
                { "flesh", new List<IMutation>(){

                }},
                { "bone", new List<IMutation>(){

                }},
                { "humors", new List<IMutation>(){

                }},
                { "misc", new List<IMutation>(){
                    new EfficientFatStorage(), new EfficientGrowth(),
                }}
            }}
        };

        public Dictionary<string, Dictionary<string, List<IMutation>>> badMutationOptions = new Dictionary<string, Dictionary<string, List<IMutation>>>()
        {
            {"offense", new Dictionary<string, List<IMutation>>(){
                { "flesh", new List<IMutation>(){
                    new SparseNematocysts(),
                }},
                { "bone", new List<IMutation>(){
                }},
                { "humors", new List<IMutation>(){

                }},
                { "misc", new List<IMutation>(){

                }}
            }},
            {"defense", new Dictionary<string, List<IMutation>>(){
                { "flesh", new List<IMutation>(){

                }},
                { "bone", new List<IMutation>(){

                }},
                { "humors", new List<IMutation>(){
                }},
                { "misc", new List<IMutation>(){

                }}
            }},
            {"utility", new Dictionary<string, List<IMutation>>(){
                { "flesh", new List<IMutation>(){

                }},
                { "bone", new List<IMutation>(){

                }},
                { "humors", new List<IMutation>(){

                }},
                { "misc", new List<IMutation>(){

                }}
            }}
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

        public virtual int GetChanceModifier(string theme)
        {
            return 0;
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

        public virtual string RollCategory()
        {
            int lower = 0;
            int upper = 0;
            Dictionary<string, Tuple<int, int>> ranges = new Dictionary<string, Tuple<int, int>>();
            foreach(string t in categoryOdds.Keys)
            {
                lower = upper + 1;
                upper = lower + categoryOdds[t] + GetChanceModifier(t);
                ranges.Add(t, new Tuple<int, int>(lower, upper));
            }
            int index = Rand.RangeInclusive(1, upper);
            foreach(string t in ranges.Keys)
            {
                if (index >= ranges[t].Item1 && index <= ranges[t].Item2)
                {
                    return t;
                }
            }
            return null;
        }

        public string GetRandomTheme(Dictionary<string, int> themeOdds, Dictionary<string, List<IMutation>> mutationTables)
        {
            int lower = 0;
            int upper = 0;
            Dictionary<string, Tuple<int, int>> ranges = new Dictionary<string, Tuple<int, int>>();
            foreach(string t in themeOdds.Keys)
            {
                if (mutationTables.TryGetValue(t, new List<IMutation>()).Count > 0)
                {
                    lower = upper + 1;
                    upper = lower + themeOdds[t] + GetChanceModifier(t);
                    ranges.Add(t, new Tuple<int, int>(lower, upper));
                }
            }
            int index = Rand.RangeInclusive(1, upper);
            foreach(string t in ranges.Keys)
            {
                if (index >= ranges[t].Item1 && index <= ranges[t].Item2)
                {
                    return t;
                }
            }
            return null;
        }

        public virtual IMutation RollMutation(string cat, string theme, Dictionary<string, Dictionary<string, List<IMutation>>> mutationOptions)
        {
            List<IMutation> _mutations = mutationOptions[cat][theme];
            if (_mutations.Count > 0)
            {
                return _mutations[Rand.Range(0, _mutations.Count)];
            }
            return null;
        }

        public virtual IMutation RollQuirk()
        {
            return quirkPossibilities[Rand.Range(0, quirkPossibilities.Count)];
        }

        public virtual void AddMutation(string cat, string theme, IMutation toAdd, bool positive)
        {
            if (positive)
            {
                goodMutationOptions[cat][theme].Add(toAdd);
            } else
            {
                badMutationOptions[cat][theme].Add(toAdd);
            }
        }

        public virtual void RemoveMutation<t>(string cat, string theme, bool positive)
        {
            if (positive)
            {
                goodMutationOptions[cat][theme] = goodMutationOptions[cat][theme].FindAll(e => !(e is t));
            } else
            {
                badMutationOptions[cat][theme] = badMutationOptions[cat][theme].FindAll(e => !(e is t));

            }
        }

        public virtual void AdjustThemeChance(string theme, int adj)
        {

        }

        public virtual void InduceMutation()
        {
            string cat = RollCategory();
            if (cat == "quirk")
            {

            } else
            {
                string theme = GetRandomTheme(mutationThemes, goodMutationOptions[cat]);
                if (theme == null)
                {
                    return;
                }
                IMutation mut = RollMutation(cat, theme, goodMutationOptions);
                if (mut != null)
                {
                    mut.Apply(this);
                    if (mut.RunOnBodyParts())
                    {
                        body.ApplyMutationToAll(mut);
                    }
                    mutations.Add(mut);
                }
            }
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
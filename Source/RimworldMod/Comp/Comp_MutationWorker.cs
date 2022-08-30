using SaveOurShip2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class CompMutationWorker : ThingComp
	{
		public CompProperties_MutationWorker Props => (CompProperties_MutationWorker)props;

        StatDef inducers = StatDef.Named("MutationInducers");
        public BuildingBody body = null;

        public List<IMutation> mutations = new List<IMutation>();
        public float mutationCountdown = 0f;
        public bool mutating = false;
        public string tier = "tier1";

        public Dictionary<string, int> mutationThemes = new Dictionary<string, int>()
        {
            {"flesh", 3},
            {"bone", 3},
            {"humors", 3},
            {"psi", 1},
            {"misc", 2}
        };
        public Dictionary<string, int> categoryOdds = new Dictionary<string, int>()
        {
            {"offense", 3},
            {"defense", 3},
            {"utility", 2},
        };
        public List<IMutation> quirkPossibilities = new List<IMutation>()
        {
            new OcularPerk(),
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
                    new PotentAcid(), new EnergizedPlasma(), 
                }},
                { "misc", new List<IMutation>(){

                }},
                { "psi", new List<IMutation>(){

                }}

            }},
            {"defense", new Dictionary<string, List<IMutation>>(){
                { "flesh", new List<IMutation>(){

                }},
                { "bone", new List<IMutation>(){
                    new BoneArmor(),
                }},
                { "humors", new List<IMutation>(){
                    new EfficientRegeneration(),
                }},
                { "misc", new List<IMutation>(){

                }},
                { "psi", new List<IMutation>(){
                    new IronWill()
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
                }},
                { "psi", new List<IMutation>(){

                }}

            }}
        };
        public Dictionary<string, Dictionary<string, List<IMutation>>> badMutationOptions = new Dictionary<string, Dictionary<string, List<IMutation>>>()
        {
            {"offense", new Dictionary<string, List<IMutation>>(){
                { "flesh", new List<IMutation>(){

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

        public override void PostExposeData()
		{
            base.PostExposeData();
            Scribe_Collections.Look<IMutation>(ref mutations, "mutations", LookMode.Deep);
            Scribe_Values.Look(ref mutationCountdown, "mutationCountdown", 0f);
            Scribe_Values.Look(ref mutating, "mutating", false);
            Scribe_Values.Look(ref tier, "tier", "tier1");
		}

        public override void PostSpawnSetup(bool b) {
            base.PostSpawnSetup(b);
            if (tier != "tier1")
            {
                this.UpgradeMutationTier(tier);
            }
        }

        public virtual void GetInitialMutations(BuildingBody body)
        {
            this.SpreadMutation(body, this.quirkPossibilities.RandomElement());
            this.SpreadMutation(body, this.RollMutation("offense", this.GetRandomTheme(this.mutationThemes, this.goodMutationOptions.TryGetValue("offense")), this.goodMutationOptions));
            this.SpreadMutation(body, this.RollMutation("defense", this.GetRandomTheme(this.mutationThemes, this.goodMutationOptions.TryGetValue("defense")), this.goodMutationOptions));
            this.SpreadMutation(body, this.RollMutation("utility", this.GetRandomTheme(this.mutationThemes, this.goodMutationOptions.TryGetValue("utility")), this.goodMutationOptions));
        }

        public override void CompTick()
        {
            base.CompTick();
            if (mutating)
            {
                if (mutationCountdown <= 0)
                {
                    InduceMutation();
                    mutating = false;
                }
                mutationCountdown--;
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if ((int)parent.GetStatValue(inducers) > 0 && !mutating && !(this.tier == "tier3" && this.GetMutationsForTier("tier3").Count >= 2))
            {
                yield return new Command_Action
                {
                    defaultLabel = "Induce Mutation",
                    action = delegate ()
                    {
                        this.mutationCountdown = 60000 * 3;
                        this.mutating = true;
                    }
                };
            }
	        if (Prefs.DevMode)
	        {
                if (mutating)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "DEBUG: Finish Mutating",
                        action = delegate ()
                        {
                            this.mutationCountdown = 0;
                        }
                    };
                }
            }
        }

        public override string CompInspectStringExtra()
        {
            if (mutating)
            {
                return String.Format("Mutating: {0:0.##} days left", (mutationCountdown/60000));
            }
            return "";
        }


        public virtual int GetChanceModifier(string theme)
        {
            return 0;
        }

        public virtual string RollCategory()
        {
            int lower = 0;
            int upper = 0;
            Dictionary<string, Tuple<int, int>> ranges = new Dictionary<string, Tuple<int, int>>();
            foreach (string t in categoryOdds.Keys)
            {
                lower = upper + 1;
                upper = lower + categoryOdds[t] + GetChanceModifier(t);
                ranges.Add(t, new Tuple<int, int>(lower, upper));
            }
            int index = Rand.RangeInclusive(1, upper);
            foreach (string t in ranges.Keys)
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
            foreach (string t in themeOdds.Keys)
            {
                if (mutationTables.TryGetValue(t, new List<IMutation>()).Count > 0)
                {
                    lower = upper + 1;
                    upper = lower + themeOdds[t] + GetChanceModifier(t);
                    ranges.Add(t, new Tuple<int, int>(lower, upper));
                }
            }
            int index = Rand.RangeInclusive(1, upper);
            foreach (string t in ranges.Keys)
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
                return _mutations.RandomElement();
            }
            return null;
        }

        public virtual IMutation RollQuirk()
        {
            return quirkPossibilities.RandomElement();
        }

        public virtual void AddMutation(string cat, string theme, IMutation toAdd, bool positive)
        {
            if (positive)
            {
                goodMutationOptions[cat][theme].Add(toAdd);
            }
            else
            {
                badMutationOptions[cat][theme].Add(toAdd);
            }
        }

        public virtual void RemoveMutation<t>(string cat, string theme, bool positive)
        {
            if (positive)
            {
                goodMutationOptions[cat][theme] = goodMutationOptions[cat][theme].FindAll(e => !(e is t));
            }
            else
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

            }
            else
            {
                string theme = GetRandomTheme(mutationThemes, goodMutationOptions[cat]);
                if (theme == null)
                {
                    return;
                }
                IMutation mut = RollMutation(cat, theme, goodMutationOptions);
                CompShipBodyPart bp = parent.TryGetComp<CompShipBodyPart>();
                if (mut != null && body != null)
                {
                    SpreadMutation(body, mut);
                }
            }
        }
        public virtual void SpreadMutation(BuildingBody b, IMutation mut)
        {
            mutations.Add(mut);
            if (mut.ShouldAddTo(b.heart))
            {
                mut.Apply(b.heart);
                b.heart.hediffs.Add(mut);
            }
            foreach (Thing t in b.bodyParts)
            {
                if (mut.ShouldAddTo(t.TryGetComp<CompShipBodyPart>()))
                {
                    mut.Apply(t.TryGetComp<CompShipBodyPart>());
                    t.TryGetComp<CompShipBodyPart>().hediffs.Add(mut);
                }
            }
            if (this.tier == "tier1" && this.GetMutationsForTier("tier1").Count >= 6)
            {
                UpgradeMutationTier("tier2");
            } else if (this.tier == "tier2" && this.GetMutationsForTier("tier2").Count >= 4)
            {
                UpgradeMutationTier("tier3");
            } else if (this.tier == "tier3" && this.GetMutationsForTier("tier3").Count >= 2)
            {
                return;
            }
        }

        public virtual void RemoveMutationFromOrgan(Building b, IMutation mut)
        {

        }

        public virtual List<IMutation> GetMutationsForTier(String tier)
        {
            return mutations.FindAll((IMutation m) => m.GetTier() == tier);
        }

        public virtual List<IMutation> GetMutationOptionsForTeir(String tier)
        {
            List<IMutation> mutationOptions = new List<IMutation>();
            foreach(Dictionary<String, List<IMutation>> category in goodMutationOptions.Values)
            {
                foreach(List<IMutation> theme in category.Values)
                {
                    foreach(IMutation mut in theme)
                    {
                        if(mut.GetTier() == tier)
                        {
                            mutationOptions.Add(mut);
                        }
                    }
                }
            }
            return mutationOptions;
        }

        public virtual void UpgradeMutationTier(string newTier)
        {
            foreach(String cat in goodMutationOptions.Keys)
            {
                goodMutationOptions[cat].Clear();
            }
            foreach(IMutation mut in mutations)
            {
                foreach(Tuple<IMutation, string, string> newMutation in mut.GetMutationsForTier(newTier, mutations)) {
                    if (!goodMutationOptions.ContainsKey(newMutation.Item2))
                    {
                        goodMutationOptions.Add(newMutation.Item2, new Dictionary<string, List<IMutation>>());
                    }
                    if (!goodMutationOptions
                        .TryGetValue(newMutation.Item2, new Dictionary<string, List<IMutation>>())
                        .ContainsKey(newMutation.Item3))
                    {
                        goodMutationOptions
                            .TryGetValue(newMutation.Item2, new Dictionary<string, List<IMutation>>())
                            .Add(newMutation.Item3, new List<IMutation>());
                    }
                    goodMutationOptions
                            .TryGetValue(newMutation.Item2, new Dictionary<string, List<IMutation>>())
                            .TryGetValue(newMutation.Item3, new List<IMutation>()).Add(newMutation.Item1);
                }
            }
        }
    }
}
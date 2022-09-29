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
    public class MutTier : IExposable
    {
        public Dictionary<string, MutCat> cats = new Dictionary<string, MutCat>();

        public MutTier()
        {

        }

        public MutTier(Dictionary<string, MutCat> _cats)
        {
            cats = _cats;
        }

        void IExposable.ExposeData()
        {
            Scribe_Collections.Look<string, MutCat>(ref cats, "cats", LookMode.Value, LookMode.Deep);
        }
    }

    public class MutCat : IExposable
    {
        public Dictionary<string, MutTheme> themes = new Dictionary<string, MutTheme>();

        public MutCat()
        {

        }

        public MutCat( Dictionary<string, MutTheme> _themes)
        {
            themes = _themes;
        }

        void IExposable.ExposeData()
        {
            Scribe_Collections.Look<string, MutTheme>(ref themes, "themes", LookMode.Value, LookMode.Deep);
        }
    }

    public class MutTheme : IExposable
    {
        public List<IMutation> muts = new List<IMutation>();

        public MutTheme()
        {

        }

        public MutTheme(List<IMutation> _muts)
        {
            muts = _muts;
        }

        void IExposable.ExposeData()
        {
            Scribe_Collections.Look<IMutation>(ref muts, "muts", LookMode.Deep);
        }
    }


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
        public static List<IMutation> quirkPossibilities = new List<IMutation>()
        {
            new OcularPerk(), new GiantBrainFromOuterSpace()
        };
        public MutTier mutationOptions = null;

        public override void PostExposeData()
		{
            base.PostExposeData();
            Scribe_Collections.Look<IMutation>(ref mutations, "mutations", LookMode.Deep);
            Scribe_Collections.Look<string, int>(ref mutationThemes, "mutationThemes", LookMode.Value);
            Scribe_Collections.Look<string, int>(ref categoryOdds, "categoryOdds", LookMode.Value);
            Scribe_Collections.Look<string, MutTier>(ref stockMutations, "stockMutations", LookMode.Value, LookMode.Deep);


            Scribe_Values.Look(ref mutationCountdown, "mutationCountdown", 0f);
            Scribe_Values.Look(ref mutating, "mutating", false);
            Scribe_Values.Look(ref tier, "tier", "tier1");
            mutationOptions = stockMutations[tier];
		}

        public virtual void GetInitialMutations(BuildingBody body)
        {
            if (this.mutationOptions == null)
            {
                mutationOptions = stockMutations["tier1"];
            }
            this.SpreadMutation(body, quirkPossibilities.RandomElement());
            this.SpreadMutation(body, this.RollMutation("offense", this.GetRandomTheme(this.mutationThemes, this.mutationOptions.cats.TryGetValue("offense")), this.mutationOptions));
            this.SpreadMutation(body, this.RollMutation("defense", this.GetRandomTheme(this.mutationThemes, this.mutationOptions.cats.TryGetValue("defense")), this.mutationOptions));
            this.SpreadMutation(body, this.RollMutation("utility", this.GetRandomTheme(this.mutationThemes, this.mutationOptions.cats.TryGetValue("utility")), this.mutationOptions));
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

        public bool CanMutate
        {
            get
            {
                return !mutating && !(this.tier == "tier3" && this.GetMutationsForTier("tier3").Count >= 2);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
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

        public string GetRandomTheme(Dictionary<string, int> themeOdds, MutCat mutationTables)
        {
            int lower = 0;
            int upper = 0;
            Dictionary<string, Tuple<int, int>> ranges = new Dictionary<string, Tuple<int, int>>();
            foreach (string t in themeOdds.Keys)
            {
                if (mutationTables.themes.TryGetValue(t, new MutTheme(new List<IMutation>())).muts.Count > 0)
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

        public virtual IMutation RollMutation(string cat, string theme, MutTier mutationOptions)
        {
            List<IMutation> _mutations = mutationOptions.cats[cat].themes[theme].muts;
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

        public virtual void AddMutation(string cat, string theme, IMutation toAdd)
        {
            mutationOptions.cats[cat].themes[theme].muts.Add(toAdd);
        }

        public virtual void RemoveMutation<t>(string cat, string theme)
        {
            mutationOptions.cats[cat].themes[theme].muts = mutationOptions.cats[cat].themes[theme].muts.FindAll(e => !(e is t));
        }

        public virtual void RemoveMutation(string cat, string theme, IMutation mut)
        {
            mutationOptions.cats[cat].themes[theme].muts.Remove(mut);
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
                string theme = GetRandomTheme(mutationThemes, mutationOptions.cats[cat]);
                if (theme == null)
                {
                    return;
                }
                IMutation mut = RollMutation(cat, theme, mutationOptions);
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
            }
            foreach (Thing t in b.bodyParts)
            {
                if (mut.ShouldAddTo(t.TryGetComp<CompShipBodyPart>()))
                {
                    mut.Apply(t.TryGetComp<CompShipBodyPart>());
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

        public virtual void RemoveMutationFromBody(BuildingBody b, IMutation mut)
        {
            mutations.Remove(mut);
            mut.Remove(b.heart);
            foreach (Thing t in b.bodyParts)
            {
                if (mut.ShouldAddTo(t.TryGetComp<CompShipBodyPart>()))
                {
                    mut.Remove(t.TryGetComp<CompShipBodyPart>());
                }
            }
        }

        public virtual List<IMutation> GetMutationsForTier(String tier)
        {
            return mutations.FindAll((IMutation m) => m.GetTier() == tier);
        }

        public virtual List<IMutation> GetQuirks()
        {
            return quirkPossibilities;
        }

        public virtual List<IMutation> GetMutationOptionsForTeir(String tier)
        {
            List<IMutation> ret = new List<IMutation>();
            foreach(MutCat category in mutationOptions.cats.Values)
            {
                foreach(MutTheme theme in category.themes.Values)
                {
                    foreach(IMutation mut in theme.muts)
                    {
                        if(mut.GetTier() == tier)
                        {
                            ret.Add(mut);
                        }
                    }
                }
            }
            return ret;
        }

        public virtual void UpgradeMutationTier(string newTier)
        {
            this.tier = newTier;
            mutationOptions = stockMutations[newTier];
            foreach(IMutation mut in mutations)
            {
                foreach(Tuple<IMutation, string, string> newMutation in mut.GetMutationsForTier(newTier, mutations)) {
                    if (!mutationOptions.cats.ContainsKey(newMutation.Item2))
                    {
                        mutationOptions.cats.Add(newMutation.Item2, 
                            new MutCat(new Dictionary<string, MutTheme>()));
                    }
                    if (!mutationOptions
                        .cats.TryGetValue(newMutation.Item2, new MutCat(new Dictionary<string, MutTheme>())).themes
                        .ContainsKey(newMutation.Item3))
                    {
                        mutationOptions.cats
                            .TryGetValue(newMutation.Item2, new MutCat()).themes
                            .Add(newMutation.Item3, new MutTheme(new List<IMutation>()));
                    }
                    mutationOptions
                            .cats.TryGetValue(newMutation.Item2, new MutCat(new Dictionary<string, MutTheme>()))
                            .themes.TryGetValue(newMutation.Item3, new MutTheme(new List<IMutation>())).muts.Add(newMutation.Item1);
                }
            }
        }

        public virtual void DowngradeMutationTier(string newTier)
        {
            this.tier = newTier;
            mutationOptions = stockMutations[newTier];
        }

        public Dictionary<string, MutTier> stockMutations = new Dictionary<string, MutTier>()
        {
            {"tier1", new MutTier(new Dictionary<string, MutCat>() {
                {"offense", new MutCat(new Dictionary<string, MutTheme>(){
                    { "flesh", new MutTheme(new List<IMutation>(){
                        new ClusteredNematocysts(),
                        new AdaptiveScars(),
                    })},
                    { "bone", new MutTheme(new List<IMutation>(){
                        new DenseSpines(), new EfficientSpines(),
                    })},
                    { "humors", new MutTheme(new List<IMutation>(){
                        new PotentAcid(), new EnergizedPlasma(), 
                    })},
                    { "misc", new MutTheme(new List<IMutation>(){

                    })},
                    { "psi", new MutTheme(new List<IMutation>(){

                    })}

                })},
                {"defense", new MutCat(new Dictionary<string, MutTheme>(){
                    { "flesh", new MutTheme(new List<IMutation>(){
                        new AdaptiveScars(),
                    })},
                    { "bone", new MutTheme(new List<IMutation>(){
                        new BoneArmor(),
                    })},
                    { "humors", new MutTheme(new List<IMutation>(){
                        new EfficientRegeneration(),
                    })},
                    { "misc", new MutTheme(new List<IMutation>(){

                    })},
                    { "psi", new MutTheme(new List<IMutation>(){
                        new IronWill()
                    })}

                })},
                {"utility", new MutCat(new Dictionary<string, MutTheme>(){
                    { "flesh", new MutTheme(new List<IMutation>(){

                    })},
                    { "bone", new MutTheme(new List<IMutation>(){

                    })},
                    { "humors", new MutTheme(new List<IMutation>(){

                    })},
                    { "misc", new MutTheme(new List<IMutation>(){
                        new EfficientFatStorage(), new EfficientGrowth(),
                    })},
                    { "psi", new MutTheme(new List<IMutation>(){

                    })}
                })}
            })},
            {"tier2",new MutTier(new Dictionary<string, MutCat>() {
                {"offense", new MutCat(new Dictionary<string, MutTheme>(){
                    { "flesh", new MutTheme(new List<IMutation>(){
                    })},
                    { "bone", new MutTheme(new List<IMutation>(){
                    })},
                    { "humors", new MutTheme(new List<IMutation>(){
                    })},
                    { "misc", new MutTheme(new List<IMutation>(){
                    })},
                    { "psi", new MutTheme(new List<IMutation>(){
                    })}

                })},
                {"defense", new MutCat(new Dictionary<string, MutTheme>(){
                    { "flesh", new MutTheme(new List<IMutation>(){
                    })},
                    { "bone", new MutTheme(new List<IMutation>(){
                    })},
                    { "humors", new MutTheme(new List<IMutation>(){
                    })},
                    { "misc", new MutTheme(new List<IMutation>(){
                    })},
                    { "psi", new MutTheme(new List<IMutation>(){
                    })}
                })},
                {"utility", new MutCat(new Dictionary<string, MutTheme>(){
                    { "flesh", new MutTheme(new List<IMutation>(){
                    })},
                    { "bone", new MutTheme(new List<IMutation>(){
                    })},
                    { "humors", new MutTheme(new List<IMutation>(){
                    })},
                    { "misc", new MutTheme(new List<IMutation>(){
                    })},
                    { "psi", new MutTheme(new List<IMutation>(){
                    })}
                })}
            })},
            {"tier3",new MutTier(new Dictionary<string, MutCat>() {
                {"offense", new MutCat(new Dictionary<string, MutTheme>(){
                    { "flesh", new MutTheme(new List<IMutation>(){
                    })},
                    { "bone", new MutTheme(new List<IMutation>(){
                    })},
                    { "humors", new MutTheme(new List<IMutation>(){
                    })},
                    { "misc", new MutTheme(new List<IMutation>(){
                    })},
                    { "psi", new MutTheme(new List<IMutation>(){
                    })}

                })},
                {"defense", new MutCat(new Dictionary<string, MutTheme>(){
                    { "flesh", new MutTheme(new List<IMutation>(){
                    })},
                    { "bone", new MutTheme(new List<IMutation>(){
                    })},
                    { "humors", new MutTheme(new List<IMutation>(){
                    })},
                    { "misc", new MutTheme(new List<IMutation>(){
                    })},
                    { "psi", new MutTheme(new List<IMutation>(){
                    })}
                })},
                {"utility", new MutCat(new Dictionary<string, MutTheme>(){
                    { "flesh", new MutTheme(new List<IMutation>(){
                    })},
                    { "bone", new MutTheme(new List<IMutation>(){
                    })},
                    { "humors", new MutTheme(new List<IMutation>(){
                    })},
                    { "misc", new MutTheme(new List<IMutation>(){
                    })},
                    { "psi", new MutTheme(new List<IMutation>(){
                    })}
                })}
            })},

        };


    }
}
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

        public List<IHediff> mutations = new List<IHediff>();

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
        public List<IHediff> quirkPossibilities = new List<IHediff>()
        {

        };
        public Dictionary<string, Dictionary<string, List<IHediff>>> goodMutationOptions = new Dictionary<string, Dictionary<string, List<IHediff>>>()
        {
            {"offense", new Dictionary<string, List<IHediff>>(){
                { "flesh", new List<IHediff>(){
                    new ClusteredNematocysts(),
                }},
                { "bone", new List<IHediff>(){
                    new DenseSpines(), new EfficientSpines(),
                }},
                { "humors", new List<IHediff>(){

                }},
                { "misc", new List<IHediff>(){

                }}
            }},
            {"defense", new Dictionary<string, List<IHediff>>(){
                { "flesh", new List<IHediff>(){

                }},
                { "bone", new List<IHediff>(){
                    new BoneArmor(),
                }},
                { "humors", new List<IHediff>(){
                    new FastRegeneration(), new EfficientRegeneration(),
                }},
                { "misc", new List<IHediff>(){

                }}
            }},
            {"utility", new Dictionary<string, List<IHediff>>(){
                { "flesh", new List<IHediff>(){

                }},
                { "bone", new List<IHediff>(){

                }},
                { "humors", new List<IHediff>(){

                }},
                { "misc", new List<IHediff>(){
                    new EfficientFatStorage(), new EfficientGrowth(),
                }}
            }}
        };
        public Dictionary<string, Dictionary<string, List<IHediff>>> badMutationOptions = new Dictionary<string, Dictionary<string, List<IHediff>>>()
        {
            {"offense", new Dictionary<string, List<IHediff>>(){
                { "flesh", new List<IHediff>(){
                    new SparseNematocysts(),
                }},
                { "bone", new List<IHediff>(){
                }},
                { "humors", new List<IHediff>(){

                }},
                { "misc", new List<IHediff>(){

                }}
            }},
            {"defense", new Dictionary<string, List<IHediff>>(){
                { "flesh", new List<IHediff>(){

                }},
                { "bone", new List<IHediff>(){

                }},
                { "humors", new List<IHediff>(){
                }},
                { "misc", new List<IHediff>(){

                }}
            }},
            {"utility", new Dictionary<string, List<IHediff>>(){
                { "flesh", new List<IHediff>(){

                }},
                { "bone", new List<IHediff>(){

                }},
                { "humors", new List<IHediff>(){

                }},
                { "misc", new List<IHediff>(){

                }}
            }}
        };

        public override void PostExposeData()
		{
            base.PostExposeData();
            Scribe_Collections.Look<IHediff>(ref mutations, "mutations", LookMode.Deep);
		}

        public override void PostSpawnSetup(bool b) {
            base.PostSpawnSetup(b);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if ((int)parent.GetStatValue(inducers) > 0)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Induce Mutation",
                    action = delegate ()
                    {
                        this.InduceMutation();
                    }
                };
            }

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

        public string GetRandomTheme(Dictionary<string, int> themeOdds, Dictionary<string, List<IHediff>> mutationTables)
        {
            int lower = 0;
            int upper = 0;
            Dictionary<string, Tuple<int, int>> ranges = new Dictionary<string, Tuple<int, int>>();
            foreach (string t in themeOdds.Keys)
            {
                if (mutationTables.TryGetValue(t, new List<IHediff>()).Count > 0)
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

        public virtual IHediff RollMutation(string cat, string theme, Dictionary<string, Dictionary<string, List<IHediff>>> mutationOptions)
        {
            List<IHediff> _mutations = mutationOptions[cat][theme];
            if (_mutations.Count > 0)
            {
                return _mutations[Rand.Range(0, _mutations.Count)];
            }
            return null;
        }

        public virtual IHediff RollQuirk()
        {
            return quirkPossibilities[Rand.Range(0, quirkPossibilities.Count)];
        }

        public virtual void AddMutation(string cat, string theme, IHediff toAdd, bool positive)
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
                IHediff mut = RollMutation(cat, theme, goodMutationOptions);
                CompShipBodyPart bp = parent.TryGetComp<CompShipBodyPart>();
                if (mut != null && body != null)
                {
                    SpreadMutation(body, mut);
                }
            }
        }
        public virtual void SpreadMutation(BuildingBody b, IHediff mut)
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
        }
    }
}
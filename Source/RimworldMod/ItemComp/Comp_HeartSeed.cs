using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    public class CompHeartSeed : ThingComp
    {
        public List<BuildingGeneDef> heartGenes = null;
        public ShipGenelineDef geneline = null;

        public List<BuildingGeneDef> GeneSet
        {
            get
            {
                return heartGenes;
            }
        }

        public List<BuildingGeneDef> GenesListForReading
        {
            get
            {
                return this.heartGenes;
            }
        }

		public int ComplexityTotal
		{
			get
			{
				this.RecacheOverridesIfNeeded();
				int num = 0;
				for (int i = 0; i < this.cachedNonOverriddenGenes.Count; i++)
				{
					num += this.cachedNonOverriddenGenes[i].complexity;
				}
				return num;
			}
		}

		public int MetabolismTotal
		{
			get
			{
				this.RecacheOverridesIfNeeded();
				int num = 0;
				for (int i = 0; i < this.cachedNonOverriddenGenes.Count; i++)
				{
					num += this.cachedNonOverriddenGenes[i].metabolicCost;
				}
				return num;
			}
		}

		public int ArchitesTotal
		{
			get
			{
				this.RecacheOverridesIfNeeded();
				int num = 0;
				for (int i = 0; i < this.cachedNonOverriddenGenes.Count; i++)
				{
					num += this.cachedNonOverriddenGenes[i].architeCost;
				}
				return num;
			}
		}

  		public bool AutoLoad
		{
			get
			{
				return this.autoLoad;
			}
		}

		public string LabelNoCount
		{
			get
			{
				string ret = "";
				foreach (BuildingGeneDef gene in heartGenes)
                {
					ret += " " + gene.label;
                }
				return ret;
			}
		}

		public void RandInit(int metCost = 1)
        {
			geneline = DefDatabase<ShipGenelineDef>.GetRandom();
			heartGenes = new List<BuildingGeneDef>();
			int num = Rand.Range(1,3);
			for(int i = 0; i < num; i++)
            {
				heartGenes.Add(DefDatabase<BuildingGeneDef>.GetRandom());
            }
        }

        private void RecacheOverridesIfNeeded()
		{
			if (this.cachedNonOverriddenGenes == null)
			{
				this.cachedNonOverriddenGenes = new List<BuildingGeneDef>();
				foreach (BuildingGeneDef geneDef in this.heartGenes)
				{
					this.tmpGeneDefWithTypes.Add(new BuildingGeneDefWithType(geneDef, true));
				}
				this.cachedNonOverriddenGenes.AddRange(this.tmpGeneDefWithTypes.NonOverriddenGenes());
				this.tmpGeneDefWithTypes.Clear();
			}
		}

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref geneline, "geneline");
            Scribe_Collections.Look(ref heartGenes, "heartGenes", LookMode.Def);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (mode == DestroyMode.Vanish)
            {
                CompHeartSeed.ExoDefs = this.heartGenes;
                CompHeartSeed.Geneline = this.geneline;
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder b = new StringBuilder(base.CompInspectStringExtra());
            b.Append("Geneline " + geneline.label);
            foreach(BuildingGeneDef gDef in heartGenes)
            {
                b.Append("\n" + gDef.label);
            }
            return b.ToString();
        }

        public static ShipGenelineDef Geneline = null;
        public static List<BuildingGeneDef> ExoDefs = null;


        [Unsaved(false)]
		private List<BuildingGeneDef> cachedNonOverriddenGenes;
        [Unsaved(false)]
        private List<BuildingGeneDefWithType> tmpGeneDefWithTypes = new List<BuildingGeneDefWithType>();
        
        private bool autoLoad = true;

		public Thing targetContainer;
    }
    public static class HeartSeedUtils
    {
        public static List<BuildingGeneDef> NonOverriddenGenes(this List<CompHeartSeed> packs, bool xenogene)
        {
            List<BuildingGeneDef> defs = new List<BuildingGeneDef>();
            foreach (CompHeartSeed seed in packs)
            {
                defs.AddRange(seed.heartGenes);
            }
            return defs.NonOverriddenGenes(xenogene);
        }

        public static void SortGenepacks(this List<CompHeartSeed> genepacks)
		{
			genepacks.SortBy((CompHeartSeed x) => x.LabelNoCount);
			//genepacks.SortBy((CompHeartSeed x) => -x.GenesListForReading[0].displayCategory.displayPriorityInXenotype, (CompHeartSeed x) => x.GenesListForReading[0].displayCategory.label, (CompHeartSeed x) => x.GenesListForReading[0].displayOrderInCategory);
		}
    }
}

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
		public Thing TargetHeart = null;
		public Thing TargetContainer = null;
		public bool isPhantom = false;


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

		public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            ((CorpseMawTracker)this.parent.Map.components.Where(t => t is CorpseMawTracker).FirstOrDefault()).heartSeeds.Add(this);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
			Scribe_Deep.Look(ref TargetHeart, "targetHeart", null);
			Scribe_Deep.Look(ref TargetContainer, "targetContainer", null);
            Scribe_Defs.Look(ref geneline, "geneline");
            Scribe_Collections.Look(ref heartGenes, "heartGenes", LookMode.Def);
        }

        public override void PostDeSpawn(Map map)
        {
            ((CorpseMawTracker)map.components.Where(t => t is CorpseMawTracker).FirstOrDefault()).heartSeeds.Remove(this);
            base.PostDeSpawn(map);
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

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}
			List<Thing> hearts = new List<Thing>();

			foreach(BuildingBody b in ((MapCompBuildingTracker)this.parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).bodies?.Values)
            {
				if (b.heart != null)
                {
					hearts.Add(b.heart.parent);
                }
            }

			if (this.TargetHeart == null)
            {
				yield return new Command_Action
                {
					defaultLabel = "Implant heartseed",//"ImplantHeartseed".Translate(),
					action = delegate ()
					{
						List<FloatMenuOption> options = new List<FloatMenuOption>();
						foreach (Thing heart in hearts)
                        {
							options.Add(new FloatMenuOption(
								heart.TryGetComp<CompShipHeart>()?.bodyName,
								delegate()
                                {
									this.TargetHeart = heart;
                                }));
                        }
						if (options.Count > 0)
                        {
							FloatMenu menu = new FloatMenu(options);
							Find.WindowStack.Add(menu);
                        }
					},
				};
			}
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

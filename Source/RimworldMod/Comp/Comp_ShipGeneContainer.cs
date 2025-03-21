using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;


namespace RimWorld
{
    [StaticConstructorOnStartup]
	public class CompShipGeneContainer : ThingComp, IThingHolder
	{
		public CompProperties_ShipGeneContainer Props
		{
			get
			{
				return (CompProperties_ShipGeneContainer)this.props;
			}
		}

		public bool StorageTabVisible
		{
			get
			{
				return true;
			}
		}

		public bool PowerOn
		{
			get
			{
				return this.parent.TryGetComp<CompPowerTrader>().PowerOn;
			}
		}

		public bool Full
		{
			get
			{
				return this.ContainedGenepacks.Count >= this.Props.maxCapacity;
			}
		}

		public bool CanLoadMore
		{
			get
			{
				return !this.Full && this.ContainedGenepacks.Count + this.leftToLoad.Count < this.Props.maxCapacity;
			}
		}

		public List<CompHeartSeed> ContainedGenepacks
		{
			get
			{
				this.tmpGenepacks.Clear();
				for (int i = 0; i < this.innerContainer.Count; i++)
				{
					CompHeartSeed genepack;
					if ((genepack = (this.innerContainer[i].TryGetComp<CompHeartSeed>())) != null)
					{
						this.tmpGenepacks.Add(genepack);
					}
				}
				return this.tmpGenepacks;
			}
		}

		public override void PostPostMake()
		{
			if (!ModLister.CheckBiotech("Genepack container"))
			{
				this.parent.Destroy(DestroyMode.Vanish);
				return;
			}
			base.PostPostMake();
			this.innerContainer = new ThingOwner<Thing>(this);
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return this.innerContainer;
		}

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            ((CorpseMawTracker)this.parent.Map.components.Where(t => t is CorpseMawTracker).FirstOrDefault()).geneContainers.Add(this);
        }


        public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			this.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
			base.PostDestroy(mode, previousMap);
		}

		public override void PostDeSpawn(Map map)
		{
			this.EjectContents(map);
			for (int i = 0; i < this.leftToLoad.Count; i++)
			{
				this.leftToLoad[i].TryGetComp<CompHeartSeed>().targetContainer = null;
			}
			this.leftToLoad.Clear();
		}

		public override void PostDrawExtraSelectionOverlays()
		{
			for (int i = 0; i < this.leftToLoad.Count; i++)
			{
				if (this.leftToLoad[i].Map == this.parent.Map)
				{
					GenDraw.DrawLineBetween(this.parent.DrawPos, this.leftToLoad[i].DrawPos);
				}
			}
		}

		public override void CompTick()
		{
			this.innerContainer.ThingOwnerTick(true);
		}

		public override void CompTickRare()
		{
			this.innerContainer.ThingOwnerTickRare(true);
		}

		public void EjectContents(Map destMap = null)
		{
			if (destMap == null)
			{
				destMap = this.parent.Map;
			}
			IntVec3 dropLoc = this.parent.def.hasInteractionCell ? this.parent.InteractionCell : this.parent.Position;
			this.innerContainer.TryDropAll(dropLoc, destMap, ThingPlaceMode.Near, null, null, true);
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (this.parent.Faction == Faction.OfPlayer && this.innerContainer.Any)
			{
				yield return new Command_Action
				{
					defaultLabel = "EjectAll".Translate(),
					defaultDesc = "EjectAllDesc".Translate(),
					icon = CompShipGeneContainer.EjectTex.Texture,
					action = delegate ()
					{
						this.EjectContents(this.parent.Map);
					}
				};
			}
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Fill with new hearts",
					action = delegate ()
					{
						this.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
						for (int i = 0; i < this.Props.maxCapacity; i++)
						{
							Thing seed = ThingMaker.MakeThing(ThingDef.Named("HeartSeed"), null);
							seed.TryGetComp<CompHeartSeed>().RandInit();
							this.innerContainer.TryAdd(seed, true);
						}
					}
				};
			}
			yield break;
		}

		public override string CompInspectStringExtra()
		{
			return "GenepacksStored".Translate() + string.Format(": {0} / {1}\n", this.innerContainer.Count, this.Props.maxCapacity) + "CasketContains".Translate() + ": " + this.innerContainer.ContentsString.CapitalizeFirst();
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
			{
				this
			});
			Scribe_Collections.Look<Thing>(ref this.leftToLoad, "leftToLoad", LookMode.Reference, Array.Empty<object>());
			Scribe_Values.Look<bool>(ref this.autoLoad, "autoLoad", true, false);
		}

		public ThingOwner innerContainer;

		public List<Thing> leftToLoad = new List<Thing>();

		public bool autoLoad = true;

		[Unsaved(false)]
		private List<CompHeartSeed> tmpGenepacks = new List<CompHeartSeed>();

		private static readonly CachedTexture EjectTex = new CachedTexture("UI/Gizmos/EjectAll");
	}
}

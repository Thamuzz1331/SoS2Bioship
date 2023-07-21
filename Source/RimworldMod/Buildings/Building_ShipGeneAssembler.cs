using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class Building_ShipGeneAssembler : Building, IThingHolder
    {
		public float ProgressPercent
		{
			get
			{
				return this.workDone / this.totalWorkRequired;
			}
		}

		public bool Working
		{
			get
			{
				return this.workingInt;
			}
		}

		private CompPowerTrader PowerTraderComp
		{
			get
			{
				if (this.cachedPowerComp == null)
				{
					this.cachedPowerComp = this.TryGetComp<CompPowerTrader>();
				}
				return this.cachedPowerComp;
			}
		}

		public bool PowerOn
		{
			get
			{
				return this.PowerTraderComp.PowerOn;
			}
		}

		public List<Thing> ConnectedFacilities
		{
			get
			{
				return this.TryGetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading;
			}
		}

		public int ArchitesCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.innerContainer.Count; i++)
				{
					if (this.innerContainer[i].def == ThingDefOf.ArchiteCapsule)
					{
						num += this.innerContainer[i].stackCount;
					}
				}
				return num;
			}
		}

		public int ArchitesRequiredNow
		{
			get
			{
				return this.architesRequired - this.ArchitesCount;
			}
		}

		private HashSet<Thing> UsedFacilities
		{
			get
			{
				this.tmpUsedFacilities.Clear();
				if (!this.genepacksToRecombine.NullOrEmpty<CompHeartSeed>())
				{
					List<Thing> connectedFacilities = this.ConnectedFacilities;
					for (int i = 0; i < this.genepacksToRecombine.Count; i++)
					{
						for (int j = 0; j < connectedFacilities.Count; j++)
						{
							if (!this.tmpUsedFacilities.Contains(connectedFacilities[j]))
							{
								CompShipGeneContainer compGenepackContainer = connectedFacilities[j].TryGetComp<CompShipGeneContainer>();
								if (compGenepackContainer != null && compGenepackContainer.ContainedGenepacks.Contains(this.genepacksToRecombine[i]))
								{
									this.tmpUsedFacilities.Add(connectedFacilities[j]);
									break;
								}
							}
						}
					}
				}
				return this.tmpUsedFacilities;
			}
		}
		public AcceptanceReport CanBeWorkedOnNow
		{
			get
			{
				if (!this.Working)
				{
					return false;
				}
				if (this.ArchitesRequiredNow > 0)
				{
					return false;
				}
				if (!this.PowerOn)
				{
					return "NoPower".Translate().CapitalizeFirst();
				}
				foreach (Thing thing in this.UsedFacilities)
				{
					CompPowerTrader compPowerTrader = thing.TryGetComp<CompPowerTrader>();
					if (compPowerTrader != null && !compPowerTrader.PowerOn)
					{
						return "GenebankUnpowered".Translate();
					}
				}
				if (this.MaxComplexity() < this.TotalGCX)
				{
					return "GeneProcessorUnpowered".Translate();
				}
				return true;
			}
		}

		private int TotalGCX
		{
			get
			{
				if (!this.Working)
				{
					return 0;
				}
				if (this.cachedComplexity == null)
				{
					this.cachedComplexity = new int?(0);
					if (!this.genepacksToRecombine.NullOrEmpty<CompHeartSeed>())
					{
						List<BuildingGeneDefWithType> list = new List<BuildingGeneDefWithType>();
						for (int i = 0; i < this.genepacksToRecombine.Count; i++)
						{
							if (this.genepacksToRecombine[i].GeneSet != null)
							{
								for (int j = 0; j < this.genepacksToRecombine[i].GenesListForReading.Count; j++)
								{
									list.Add(new BuildingGeneDefWithType(this.genepacksToRecombine[i].GenesListForReading[j], true));
								}
							}
						}
						List<BuildingGeneDef> list2 = list.NonOverriddenGenes();
						for (int k = 0; k < list2.Count; k++)
						{
							this.cachedComplexity += list2[k].complexity;
						}
					}
				}
				return this.cachedComplexity.Value;
			}
		}

		public override void PostPostMake()
		{
			if (!ModLister.CheckBiotech("Gene assembler"))
			{
				this.Destroy(DestroyMode.Vanish);
				return;
			}
			base.PostPostMake();
			this.innerContainer = new ThingOwner<Thing>(this);
		}

		public override void Tick()
		{
			base.Tick();
			this.innerContainer.ThingOwnerTick(true);
			if (this.IsHashIntervalTick(250))
			{
				bool flag = this.lastWorkedTick + 250 + 2 >= Find.TickManager.TicksGame;
				this.PowerTraderComp.PowerOutput = (flag ? (-base.PowerComp.Props.PowerConsumption) : (-base.PowerComp.Props.idlePowerDraw));
			}
			if (this.Working && this.IsHashIntervalTick(180))
			{
				this.CheckAllContainersValid();
			}
		}

		public void Start(List<CompHeartSeed> packs, int architesRequired, string xenotypeName, XenotypeIconDef iconDef)
		{
			this.Reset();
			this.genepacksToRecombine = packs;
			this.architesRequired = architesRequired;
			this.xenotypeName = xenotypeName;
			this.iconDef = iconDef;
			this.workingInt = true;
			this.totalWorkRequired = GeneTuning.ComplexityToCreationHoursCurve.Evaluate((float)this.TotalGCX) * 2500f;
		}

		public void DoWork(float workAmount)
		{
			this.workDone += workAmount;
			this.lastWorkAmount = workAmount;
			this.lastWorkedTick = Find.TickManager.TicksGame;
		}

		public void Finish()
		{
			if (!this.genepacksToRecombine.NullOrEmpty<CompHeartSeed>())
			{
				SoundDefOf.GeneAssembler_Complete.PlayOneShot(SoundInfo.InMap(this, MaintenanceType.None));
				Thing heartSeed = ThingMaker.MakeThing(ThingDef.Named("HeartSeed"));
                heartSeed.Position = this.Position;
                CompHeartSeed seed = heartSeed.TryGetComp<CompHeartSeed>();
                if (seed != null)
                {
					seed.geneline = ShipGenelineDef.Named("AstralCniderianGeneline");
                    seed.heartGenes = this.genepacksToRecombine.NonOverriddenGenes(true);
                }
                heartSeed.SpawnSetup(this.Map, false);
				if (GenPlace.TryPlaceThing(heartSeed, this.InteractionCell, base.Map, ThingPlaceMode.Near, null, null, default(Rot4)))
				{
					Messages.Message("MessageXenogermCompleted".Translate(), heartSeed, MessageTypeDefOf.PositiveEvent, true);
				}
			}
			if (this.architesRequired > 0)
			{
				for (int i = this.innerContainer.Count - 1; i >= 0; i--)
				{
					if (this.innerContainer[i].def == ThingDefOf.ArchiteCapsule)
					{
						Thing thing = this.innerContainer[i].SplitOff(Mathf.Min(this.innerContainer[i].stackCount, this.architesRequired));
						this.architesRequired -= thing.stackCount;
						thing.Destroy(DestroyMode.Vanish);
						if (this.architesRequired <= 0)
						{
							break;
						}
					}
				}
			}
			this.Reset();
		}

		public List<CompHeartSeed> GetGenepacks(bool includePowered, bool includeUnpowered)
		{
			this.tmpGenepacks.Clear();
			List<Thing> connectedFacilities = this.ConnectedFacilities;
			if (connectedFacilities != null)
			{
				foreach (Thing thing in connectedFacilities)
				{
					CompShipGeneContainer compGenepackContainer = thing.TryGetComp<CompShipGeneContainer>();
					if (compGenepackContainer != null)
					{
						CompPowerTrader compPowerTrader = thing.TryGetComp<CompPowerTrader>();
						bool flag = compPowerTrader == null || compPowerTrader.PowerOn;
						if ((includePowered && flag) || (includeUnpowered && !flag))
						{
							this.tmpGenepacks.AddRange(compGenepackContainer.ContainedGenepacks);
						}
					}
				}
			}
			return this.tmpGenepacks;
		}

		public CompShipGeneContainer GetGeneBankHoldingPack(CompHeartSeed pack)
		{
			List<Thing> connectedFacilities = this.ConnectedFacilities;
			if (connectedFacilities != null)
			{
				foreach (Thing thing in connectedFacilities)
				{
					CompShipGeneContainer compGenepackContainer = thing.TryGetComp<CompShipGeneContainer>();
					if (compGenepackContainer != null)
					{
						using (List<CompHeartSeed>.Enumerator enumerator2 = compGenepackContainer.ContainedGenepacks.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current == pack)
								{
									return compGenepackContainer;
								}
							}
						}
					}
				}
			}
			return null;
		}

		public int MaxComplexity()
		{
			int num = 6;
			List<Thing> connectedFacilities = this.ConnectedFacilities;
			if (connectedFacilities != null)
			{
				foreach (Thing thing in connectedFacilities)
				{
					CompPowerTrader compPowerTrader = thing.TryGetComp<CompPowerTrader>();
					if (compPowerTrader == null || compPowerTrader.PowerOn)
					{
						num += (int)thing.GetStatValue(StatDefOf.GeneticComplexityIncrease, true, -1);
					}
				}
			}
			return num;
		}

		private void Reset()
		{
			this.workingInt = false;
			this.genepacksToRecombine = null;
			this.xenotypeName = null;
			this.cachedComplexity = null;
			this.iconDef = XenotypeIconDefOf.Basic;
			this.workDone = 0f;
			this.lastWorkedTick = -999;
			this.architesRequired = 0;
			this.innerContainer.TryDropAll(this.def.hasInteractionCell ? this.InteractionCell : base.Position, base.Map, ThingPlaceMode.Near, null, null, true);
		}

		private void CheckAllContainersValid()
		{
			if (this.genepacksToRecombine.NullOrEmpty<CompHeartSeed>())
			{
				return;
			}
			List<Thing> connectedFacilities = this.ConnectedFacilities;
			for (int i = 0; i < this.genepacksToRecombine.Count; i++)
			{
				bool flag = false;
				for (int j = 0; j < connectedFacilities.Count; j++)
				{
					CompShipGeneContainer compGenepackContainer = connectedFacilities[j].TryGetComp<CompShipGeneContainer>();
					if (compGenepackContainer != null && compGenepackContainer.ContainedGenepacks.Contains(this.genepacksToRecombine[i]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Messages.Message("MessageXenogermCancelledMissingPack".Translate(this), this, MessageTypeDefOf.NegativeEvent, true);
					this.Reset();
					return;
				}
			}
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return this.innerContainer;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			Command_Action command_Action = new Command_Action();
			command_Action.defaultLabel = "Recombine".Translate() + "...";
			command_Action.defaultDesc = "RecombineDesc".Translate();
			command_Action.icon = Building_ShipGeneAssembler.RecombineIcon.Texture;
			command_Action.action = delegate()
			{
				Find.WindowStack.Add(new Dialog_CreateShipXenogerm(this));
			};
			if (!this.def.IsResearchFinished)
			{
				command_Action.Disable("MissingRequiredResearch".Translate() + ": " + (from x in this.def.researchPrerequisites
				where !x.IsFinished
				select x.label).ToCommaList(true, false).CapitalizeFirst());
			}
			else if (!this.PowerOn)
			{
				command_Action.Disable("CannotUseNoPower".Translate());
			}
			else if (!this.GetGenepacks(true, false).Any<CompHeartSeed>())
			{
				command_Action.Disable("CannotUseReason".Translate("NoGenepacksAvailable".Translate().CapitalizeFirst()));
			}
			yield return command_Action;
			if (this.Working)
			{
				yield return new Command_Action
				{
					defaultLabel = "CancelXenogerm".Translate(),
					defaultDesc = "CancelXenogermDesc".Translate(),
					action = delegate()
					{
						this.Reset();
					},
					icon = Building_ShipGeneAssembler.CancelIcon
				};
				if (DebugSettings.ShowDevGizmos)
				{
					yield return new Command_Action
					{
						defaultLabel = "DEV: Finish xenogerm",
						action = delegate()
						{
							this.Finish();
						}
					};
				}
			}
			yield break;
		}

		// Token: 0x06007653 RID: 30291 RVA: 0x0028B4FC File Offset: 0x002896FC
		public override string GetInspectString()
		{
			string text = base.GetInspectString();
			if (this.Working)
			{
				if (!text.NullOrEmpty())
				{
					text += "\n";
				}
				text = text + ("CreatingXenogerm".Translate() + ": " + this.xenotypeName.CapitalizeFirst() + "\n" + "ComplexityTotal".Translate() + ": ") + this.TotalGCX;
				text += "\n" + "Progress".Translate() + ": " + this.ProgressPercent.ToStringPercent();
				int numTicks = Mathf.RoundToInt((this.totalWorkRequired - this.workDone) / ((this.lastWorkAmount > 0f) ? this.lastWorkAmount : this.GetStatValue(StatDefOf.AssemblySpeedFactor, true, -1)));
				text = text + " (" + "DurationLeft".Translate(numTicks.ToStringTicksToPeriod(true, false, true, true, false)).Resolve() + ")";
				AcceptanceReport canBeWorkedOnNow = this.CanBeWorkedOnNow;
				if (!canBeWorkedOnNow.Accepted && !canBeWorkedOnNow.Reason.NullOrEmpty())
				{
					text = text + "\n" + ("AssemblyPaused".Translate() + ": " + canBeWorkedOnNow.Reason).Colorize(ColorLibrary.RedReadable);
				}
				if (this.architesRequired > 0)
				{
					text = string.Concat(new object[]
					{
						text,
						"\n" + "ArchitesRequired".Translate() + ": ",
						this.ArchitesCount,
						" / ",
						this.architesRequired
					});
				}
			}
			return text;
		}

		// Token: 0x06007654 RID: 30292 RVA: 0x0028B6E8 File Offset: 0x002898E8
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
			{
				this
			});
			Scribe_Collections.Look<CompHeartSeed>(ref this.genepacksToRecombine, "genepacksToRecombine", LookMode.Reference, Array.Empty<object>());
			Scribe_Values.Look<bool>(ref this.workingInt, "workingInt", false, false);
			Scribe_Values.Look<float>(ref this.workDone, "workDone", 0f, false);
			Scribe_Values.Look<float>(ref this.totalWorkRequired, "totalWorkRequired", 0f, false);
			Scribe_Values.Look<int>(ref this.lastWorkedTick, "lastWorkedTick", -999, false);
			Scribe_Values.Look<int>(ref this.architesRequired, "architesRequired", 0, false);
			Scribe_Values.Look<string>(ref this.xenotypeName, "xenotypeName", null, false);
			Scribe_Defs.Look<XenotypeIconDef>(ref this.iconDef, "iconDef");
			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.iconDef == null)
			{
				this.iconDef = XenotypeIconDefOf.Basic;
			}
		}

		public ThingOwner innerContainer;

		private List<CompHeartSeed> genepacksToRecombine;

		private int architesRequired;

		private bool workingInt;

		private int lastWorkedTick = -999;

		private float workDone;

		private float totalWorkRequired;

		public string xenotypeName;

		public XenotypeIconDef iconDef;

		[Unsaved(false)]
		private float lastWorkAmount = -1f;

		[Unsaved(false)]
		private CompPowerTrader cachedPowerComp;

		[Unsaved(false)]
		private List<CompHeartSeed> tmpGenepacks = new List<CompHeartSeed>();

		[Unsaved(false)]
		private HashSet<Thing> tmpUsedFacilities = new HashSet<Thing>();

		[Unsaved(false)]
		private int? cachedComplexity;

		private const int CheckContainersInterval = 180;

		private static readonly Texture2D CancelIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel", true);

		private static readonly CachedTexture RecombineIcon = new CachedTexture("UI/Gizmos/RecombineGenes");

    }
}
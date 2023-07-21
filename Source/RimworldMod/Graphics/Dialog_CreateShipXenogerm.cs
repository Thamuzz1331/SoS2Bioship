using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	public class Dialog_CreateShipXenogerm : ShipGeneCreationDialogBase
	{
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(1016f, (float)UI.screenHeight);
			}
		}

		protected override string Header
		{
			get
			{
				return "AssembleGenes".Translate();
			}
		}

		protected override string AcceptButtonLabel
		{
			get
			{
				return "StartCombining".Translate();
			}
		}

		protected override List<BuildingGeneDef> SelectedGenes
		{
			get
			{
				this.tmpGenes.Clear();
				foreach (CompHeartSeed genepack in this.selectedGenepacks)
				{
					foreach (BuildingGeneDef item in genepack.GenesListForReading)
					{
						this.tmpGenes.Add(item);
					}
				}
				return this.tmpGenes;
			}
		}

		public Dialog_CreateShipXenogerm(Building_ShipGeneAssembler geneAssembler)
		{
			this.geneAssembler = geneAssembler;
			this.maxGCX = geneAssembler.MaxComplexity();
			this.libraryGenepacks.AddRange(geneAssembler.GetGenepacks(true, true));
			this.unpoweredGenepacks.AddRange(geneAssembler.GetGenepacks(false, true));
			this.xenotypeName = string.Empty;
			this.closeOnAccept = false;
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.searchWidgetOffsetX = ShipGeneCreationDialogBase.ButSize.x * 2f + 4f;
			this.libraryGenepacks.SortGenepacks();
			this.unpoweredGenepacks.SortGenepacks();
		}

		public override void PostOpen()
		{
			if (!ModLister.CheckBiotech("gene assembly"))
			{
				this.Close(false);
				return;
			}
			base.PostOpen();
		}

		protected override void Accept()
		{
			if (this.geneAssembler.Working)
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmStartNewXenogerm".Translate(this.geneAssembler.xenotypeName.Named("XENOGERMNAME")), new Action(this.StartAssembly), true, null, WindowLayer.Dialog));
				return;
			}
			this.StartAssembly();
		}

		private void StartAssembly()
		{
			Building_ShipGeneAssembler building_GeneAssembler = this.geneAssembler;
			List<CompHeartSeed> packs = this.selectedGenepacks;
			int arc = this.arc;
			string xenotypeName = this.xenotypeName;
			building_GeneAssembler.Start(packs, arc, (xenotypeName != null) ? xenotypeName.Trim() : null, this.iconDef);
			SoundDefOf.StartRecombining.PlayOneShotOnCamera(null);
			this.Close(false);
		}

		protected override void DrawGenes(Rect rect)
		{
			GUI.BeginGroup(rect);
			Rect rect2 = new Rect(0f, 0f, rect.width - 16f, this.scrollHeight);
			float num = 0f;
			Widgets.BeginScrollView(rect.AtZero(), ref this.scrollPosition, rect2, true);
			Rect containingRect = rect2;
			containingRect.y = this.scrollPosition.y;
			containingRect.height = rect.height;
			this.DrawSection(rect, this.selectedGenepacks, "SelectedGenepacks".Translate(), ref num, ref this.selectedHeight, false, containingRect);
			num += 8f;
			this.DrawSection(rect, this.libraryGenepacks, "GenepackLibrary".Translate(), ref num, ref this.unselectedHeight, true, containingRect);
			if (Event.current.type == EventType.Layout)
			{
				this.scrollHeight = num;
			}
			Widgets.EndScrollView();
			GUI.EndGroup();
		}

		private void DrawSection(Rect rect, List<CompHeartSeed> genepacks, string label, ref float curY, ref float sectionHeight, bool adding, Rect containingRect)
		{
			float num = 4f;
			Rect rect2 = new Rect(10f, curY, rect.width - 16f - 10f, Text.LineHeight);
			Widgets.Label(rect2, label);
			if (!adding)
			{
				Text.Anchor = TextAnchor.UpperRight;
				GUI.color = ColoredText.SubtleGrayColor;
				Widgets.Label(rect2, "ClickToAddOrRemove".Translate());
				GUI.color = Color.white;
				Text.Anchor = TextAnchor.UpperLeft;
			}
			curY += Text.LineHeight + 3f;
			float num2 = curY;
			Rect rect3 = new Rect(0f, curY, rect.width, sectionHeight);
			Widgets.DrawRectFast(rect3, Widgets.MenuSectionBGFillColor, null);
			curY += 4f;
			if (!genepacks.Any<CompHeartSeed>())
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				GUI.color = ColoredText.SubtleGrayColor;
				Widgets.Label(rect3, "(" + "NoneLower".Translate() + ")");
				GUI.color = Color.white;
				Text.Anchor = TextAnchor.UpperLeft;
			}
			else
			{
				for (int i = 0; i < genepacks.Count; i++)
				{
					CompHeartSeed genepack = genepacks[i];
					if (!this.quickSearchWidget.filter.Active || (this.matchingGenepacks.Contains(genepack) && (!adding || !this.selectedGenepacks.Contains(genepack))))
					{
						float num3 = 34f + GeneCreationDialogBase.GeneSize.x * (float)genepack.GenesListForReading.Count + 4f * (float)(genepack.GenesListForReading.Count + 2);
						if (num + num3 > rect.width - 16f)
						{
							num = 4f;
							curY += GeneCreationDialogBase.GeneSize.y + 8f + 14f;
						}
						if (adding && this.selectedGenepacks.Contains(genepack))
						{
							Widgets.DrawLightHighlight(new Rect(num, curY, num3, GeneCreationDialogBase.GeneSize.y + 8f));
							num += num3 + 14f;
						}
						else if (this.DrawGenepack(genepack, ref num, curY, num3, containingRect))
						{
							if (adding)
							{
								SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
								this.selectedGenepacks.Add(genepack);
							}
							else
							{
								SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
								this.selectedGenepacks.Remove(genepack);
							}
							if (!this.xenotypeNameLocked)
							{
								this.xenotypeName = "fleshthing".Translate();
							}
							this.OnGenesChanged();
							break;
						}
					}
				}
			}
			curY += GeneCreationDialogBase.GeneSize.y + 12f;
			if (Event.current.type == EventType.Layout)
			{
				sectionHeight = curY - num2;
			}
		}

		private bool DrawGenepack(CompHeartSeed genepack, ref float curX, float curY, float packWidth, Rect containingRect)
		{
			bool result = false;
			if (genepack.GeneSet == null || genepack.GenesListForReading.NullOrEmpty<BuildingGeneDef>())
			{
				return result;
			}
			Rect rect = new Rect(curX, curY, packWidth, GeneCreationDialogBase.GeneSize.y + 8f);
			if (!containingRect.Overlaps(rect))
			{
				curX = rect.xMax + 14f;
				return false;
			}
			Widgets.DrawHighlight(rect);
			GUI.color = ShipGeneCreationDialogBase.OutlineColorUnselected;
			Widgets.DrawBox(rect, 1, null);
			GUI.color = Color.white;
			curX += 4f;
			GeneUIUtility.DrawBiostats(genepack.ComplexityTotal, genepack.MetabolismTotal, genepack.ArchitesTotal, ref curX, curY, 4f);
			List<BuildingGeneDef> genesListForReading = genepack.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				BuildingGeneDef gene = genesListForReading[i];
				bool flag = this.quickSearchWidget.filter.Active && this.matchingGenes.Contains(gene) && this.matchingGenepacks.Contains(genepack);
				bool overridden = this.leftChosenGroups.Any((BuildingGeneLeftChosenGroup x) => x.overriddenGenes.Contains(gene));
				Rect rect2 = new Rect(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y);
				if (flag)
				{
					Widgets.DrawStrongHighlight(rect2.ExpandedBy(6f), null);
				}
				string extraTooltip = null;
				if (this.leftChosenGroups.Any((BuildingGeneLeftChosenGroup x) => x.leftChosen == gene))
				{
					extraTooltip = Dialog_CreateShipXenogerm.DrawGenepack(this.leftChosenGroups.FirstOrDefault((BuildingGeneLeftChosenGroup x) => x.leftChosen == gene));
				}
				else if (this.cachedOverriddenGenes.Contains(gene))
				{
					extraTooltip = Dialog_CreateShipXenogerm.DrawGenepack(this.leftChosenGroups.FirstOrDefault((BuildingGeneLeftChosenGroup x) => x.overriddenGenes.Contains(gene)));
				}
				else if (this.randomChosenGroups.ContainsKey(gene))
				{
					extraTooltip = ("GeneWillBeRandomChosen".Translate() + ":\n" + (from x in this.randomChosenGroups[gene]
																					select x.label).ToLineList("  - ", true)).Colorize(ColoredText.TipSectionTitleColor);
				}
				ITab_ShipGenes.DrawGeneDef_NewTemp(genesListForReading[i], rect2, GeneType.Xenogene, () => extraTooltip, false, false, overridden);
				curX += GeneCreationDialogBase.GeneSize.x + 4f;
			}
			Widgets.InfoCardButton(rect.xMax - 24f, rect.y + 2f, genepack.parent);
			if (this.unpoweredGenepacks.Contains(genepack))
			{
				Widgets.DrawBoxSolid(rect, this.UnpoweredColor);
				TooltipHandler.TipRegion(rect, "GenepackUnusableGenebankUnpowered".Translate().Colorize(ColorLibrary.RedReadable));
			}
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect) && Event.current.button == 1)
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				list.Add(new FloatMenuOption("EjectGenepackFromGeneBank".Translate(), delegate ()
				{
					CompShipGeneContainer geneBankHoldingPack = this.geneAssembler.GetGeneBankHoldingPack(genepack);
					if (geneBankHoldingPack != null)
					{
						ThingWithComps parent = geneBankHoldingPack.parent;
						Thing thing;
						if (geneBankHoldingPack.innerContainer.TryDrop(genepack.parent, parent.def.hasInteractionCell ? parent.InteractionCell : parent.Position, parent.Map, ThingPlaceMode.Near, 1, out thing, null, null))
						{
							if (this.selectedGenepacks.Contains(genepack))
							{
								this.selectedGenepacks.Remove(genepack);
							}
							this.tmpGenes.Clear();
							this.libraryGenepacks.Clear();
							this.unpoweredGenepacks.Clear();
							this.matchingGenepacks.Clear();
							this.libraryGenepacks.AddRange(this.geneAssembler.GetGenepacks(true, true));
							this.unpoweredGenepacks.AddRange(this.geneAssembler.GetGenepacks(false, true));
							this.libraryGenepacks.SortGenepacks();
							this.unpoweredGenepacks.SortGenepacks();
							this.OnGenesChanged();
						}
					}
				}, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
				Find.WindowStack.Add(new FloatMenu(list));
			}
			else if (Widgets.ButtonInvisible(rect, true))
			{
				result = true;
			}
			curX = Mathf.Max(curX, rect.xMax + 14f);
			return result;
		}

		protected override void DrawSearchRect(Rect rect)
		{
			base.DrawSearchRect(rect);
		}

		protected override void DoBottomButtons(Rect rect)
		{
			base.DoBottomButtons(rect);
			if (!this.selectedGenepacks.Any<CompHeartSeed>())
			{
				return;
			}
			int numTicks = Mathf.RoundToInt((float)Mathf.RoundToInt(GeneTuning.ComplexityToCreationHoursCurve.Evaluate((float)this.gcx) * 2500f) / this.geneAssembler.GetStatValue(StatDefOf.AssemblySpeedFactor, true, -1));
			Rect rect2 = new Rect(rect.center.x, rect.y, rect.width / 2f - ShipGeneCreationDialogBase.ButSize.x - 10f, ShipGeneCreationDialogBase.ButSize.y);
			TaggedString label;
			TaggedString str;
			if (this.arc > 0 && !ResearchProjectDefOf.Archogenetics.IsFinished)
			{
				label = ("MissingRequiredResearch".Translate() + ": " + ResearchProjectDefOf.Archogenetics.LabelCap).Colorize(ColorLibrary.RedReadable);
				str = "MustResearchProject".Translate(ResearchProjectDefOf.Archogenetics);
			}
			else
			{
				label = "RecombineDuration".Translate() + ": " + numTicks.ToStringTicksToPeriod(true, false, true, true, false);
				str = "RecombineDurationDesc".Translate();
			}
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect2, label);
			Text.Anchor = TextAnchor.UpperLeft;
			if (Mouse.IsOver(rect2))
			{
				Widgets.DrawHighlight(rect2);
				TooltipHandler.TipRegion(rect2, str);
			}
		}

		// Token: 0x06008B0C RID: 35596 RVA: 0x002FD0FC File Offset: 0x002FB2FC
		protected override bool CanAccept()
		{
			if (!base.CanAccept())
			{
				return false;
			}
			if (!this.selectedGenepacks.Any<CompHeartSeed>())
			{
				Messages.Message("MessageNoSelectedGenepacks".Translate(), null, MessageTypeDefOf.RejectInput, false);
				return false;
			}
			if (this.arc > 0 && !ResearchProjectDefOf.Archogenetics.IsFinished)
			{
				Messages.Message("AssemblingRequiresResearch".Translate(ResearchProjectDefOf.Archogenetics), null, MessageTypeDefOf.RejectInput, false);
				return false;
			}
			if (this.gcx > this.maxGCX)
			{
				Messages.Message("ComplexityTooHighToCreateXenogerm".Translate(this.gcx.Named("AMOUNT"), this.maxGCX.Named("MAX")), null, MessageTypeDefOf.RejectInput, false);
				return false;
			}
			if (!this.ColonyHasEnoughArchites())
			{
				Messages.Message("NotEnoughArchites".Translate(), null, MessageTypeDefOf.RejectInput, false);
				return false;
			}
			return true;
		}

		// Token: 0x06008B0D RID: 35597 RVA: 0x002FD1F4 File Offset: 0x002FB3F4
		private bool ColonyHasEnoughArchites()
		{
			if (this.arc == 0 || this.geneAssembler.MapHeld == null)
			{
				return true;
			}
			List<Thing> list = this.geneAssembler.MapHeld.listerThings.ThingsOfDef(ThingDefOf.ArchiteCapsule);
			int num = 0;
			foreach (Thing thing in list)
			{
				if (!thing.Position.Fogged(this.geneAssembler.MapHeld))
				{
					num += thing.stackCount;
					if (num >= this.arc)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06008B0E RID: 35598 RVA: 0x002FD2A0 File Offset: 0x002FB4A0
		protected override void UpdateSearchResults()
		{
			this.quickSearchWidget.noResultsMatched = false;
			this.matchingGenepacks.Clear();
			this.matchingGenes.Clear();
			if (!this.quickSearchWidget.filter.Active)
			{
				return;
			}
			foreach (CompHeartSeed genepack in this.selectedGenepacks)
			{
				List<BuildingGeneDef> genesListForReading = genepack.GenesListForReading;
				for (int i = 0; i < genesListForReading.Count; i++)
				{
					if (this.quickSearchWidget.filter.Matches(genesListForReading[i].label))
					{
						this.matchingGenepacks.Add(genepack);
						this.matchingGenes.Add(genesListForReading[i]);
					}
				}
			}
			foreach (CompHeartSeed genepack2 in this.libraryGenepacks)
			{
				if (!this.selectedGenepacks.Contains(genepack2))
				{
					List<BuildingGeneDef> genesListForReading2 = genepack2.GenesListForReading;
					for (int j = 0; j < genesListForReading2.Count; j++)
					{
						if (this.quickSearchWidget.filter.Matches(genesListForReading2[j].label))
						{
							this.matchingGenepacks.Add(genepack2);
							this.matchingGenes.Add(genesListForReading2[j]);
						}
					}
				}
			}
			this.quickSearchWidget.noResultsMatched = !this.matchingGenepacks.Any<CompHeartSeed>();
		}

		internal static string DrawGenepack(BuildingGeneLeftChosenGroup group)
		{
			if (group == null)
			{
				return null;
			}
			return ("GeneOneActive".Translate() + ":\n  - " + group.leftChosen.LabelCap + " (" + "Active".Translate() + ")" + "\n" + (from x in @group.overriddenGenes
			select(x.label + " (" + "Suppressed".Translate() + ")").Colorize(ColorLibrary.RedReadable)).ToLineList("  - ", true)).Colorize(ColoredText.TipSectionTitleColor);
		}

		private Building_ShipGeneAssembler geneAssembler;

		private List<CompHeartSeed> libraryGenepacks = new List<CompHeartSeed>();

		private List<CompHeartSeed> unpoweredGenepacks = new List<CompHeartSeed>();

		private List<CompHeartSeed> selectedGenepacks = new List<CompHeartSeed>();

		private HashSet<CompHeartSeed> matchingGenepacks = new HashSet<CompHeartSeed>();

		private readonly Color UnpoweredColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

		private List<BuildingGeneDef> tmpGenes = new List<BuildingGeneDef>();
	}
}

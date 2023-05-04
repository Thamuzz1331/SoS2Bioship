using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	[StaticConstructorOnStartup]
	public abstract class ShipGeneCreationDialogBase : Window
	{
		protected abstract List<BuildingGeneDef> SelectedGenes { get; }

		protected abstract string Header { get; }

		protected abstract string AcceptButtonLabel { get; }

		protected int gcx;

		protected int met;

		protected int arc;

		protected string xenotypeName;

		protected bool xenotypeNameLocked;

		protected float scrollHeight;

		protected Vector2 scrollPosition;

		protected float selectedHeight;

		protected float unselectedHeight;

		protected bool ignoreRestrictions;

		protected float postXenotypeHeight;

		protected bool alwaysUseFullBiostatsTableHeight;

		protected int maxGCX = -1;

		protected float searchWidgetOffsetX;

		protected QuickSearchWidget quickSearchWidget = new QuickSearchWidget();

		protected HashSet<BuildingGeneDef> matchingGenes = new HashSet<BuildingGeneDef>();

		protected Dictionary<BuildingGeneDef, List<BuildingGeneDef>> randomChosenGroups = new Dictionary<BuildingGeneDef, List<BuildingGeneDef>>();

		protected List<GeneLeftChosenGroup> leftChosenGroups = new List<GeneLeftChosenGroup>();

		protected List<BuildingGeneDef> cachedOverriddenGenes = new List<BuildingGeneDef>();

		protected List<BuildingGeneDef> cachedUnoverriddenGenes = new List<BuildingGeneDef>();

		protected List<GeneDefWithType> tmpGenesWithType = new List<GeneDefWithType>();

		protected XenotypeIconDef iconDef;

		protected static readonly Vector2 ButSize = new Vector2(150f, 38f);

		protected const float HeaderHeight = 35f;

		protected const float GeneGap = 4f;

		private const int MaxNameLength = 40;

		private const int NumCharsTypedBeforeAutoLockingName = 3;

		private const int MaxTriesToGenerateUniqueXenotypeNames = 1000;

		private const float TextFieldWidthPct = 0.25f;

		private static readonly Regex ValidSymbolRegex = new Regex("^[\\p{L}0-9 '\\-]*$");

		public static readonly Texture2D UnlockedTex = ContentFinder<Texture2D>.Get("UI/Overlays/LockedMonochrome", true);

		public static readonly Texture2D LockedTex = ContentFinder<Texture2D>.Get("UI/Overlays/Locked", true);

		protected const float BiostatsWidth = 38f;

		public static readonly Vector2 GeneSize = new Vector2(87f, 68f);

		protected static readonly Color OutlineColorUnselected = new Color(1f, 1f, 1f, 0.1f);

		protected const float GenepackGap = 14f;

		protected const float QuickSearchFilterWidth = 300f;

		public override void PreOpen()
		{
			base.PreOpen();
			this.iconDef = XenotypeIconDefOf.Basic;
			this.UpdateSearchResults();
			this.OnGenesChanged();
		}

		public override void DoWindowContents(Rect rect)
		{
			Rect rect2 = rect;
			rect2.yMax -= GeneCreationDialogBase.ButSize.y + 4f;
			Rect rect3 = new Rect(rect2.x, rect2.y, rect2.width, 35f);
			Text.Font = GameFont.Medium;
			Widgets.Label(rect3, this.Header);
			Text.Font = GameFont.Small;
			this.DrawSearchRect(rect);
			rect2.yMin += 39f;
			float num = rect.width * 0.25f - this.Margin - 10f;
			float num2 = num - 24f - 10f;
			float num3 = Mathf.Max(BiostatsTable.HeightForBiostats(this.alwaysUseFullBiostatsTableHeight ? 1 : this.arc), this.postXenotypeHeight);
			Rect rect4 = new Rect(rect2.x + this.Margin, rect2.y, rect2.width - this.Margin * 2f, rect2.height - num3 - 8f);
			this.DrawGenes(rect4);
			float num4 = rect4.yMax + 4f;
			Rect rect5 = new Rect(rect2.x + this.Margin + 10f, num4, rect.width * 0.75f - this.Margin * 3f - 10f, num3)
			{
				yMax = rect4.yMax + num3 + 4f
			};
			BiostatsTable.Draw(rect5, this.gcx, this.met, this.arc, true, this.ignoreRestrictions, this.maxGCX);
			string text = "XenotypeName".Translate().CapitalizeFirst() + ":";
			Rect rect6 = new Rect(rect5.xMax + this.Margin, num4, Text.CalcSize(text).x, Text.LineHeight);
			Widgets.Label(rect6, text);
			Rect rect7 = new Rect(rect6.xMin, rect6.y + Text.LineHeight, num, Text.LineHeight);
			rect7.xMax = rect2.xMax - this.Margin - 17f - num2 * 0.25f;
			string text2 = this.xenotypeName;
			this.xenotypeName = Widgets.TextField(rect7, this.xenotypeName, 40, GeneCreationDialogBase.ValidSymbolRegex);
			if (text2 != this.xenotypeName)
			{
				if (this.xenotypeName.Length > text2.Length && this.xenotypeName.Length > 3)
				{
					this.xenotypeNameLocked = true;
				}
				else if (this.xenotypeName.Length == 0)
				{
					this.xenotypeNameLocked = false;
				}
			}
			Rect rect8 = new Rect(rect7.xMax + 4f, rect7.yMax - 35f, 35f, 35f);
			this.DrawIconSelector(rect8);
			Rect rect9 = new Rect(rect7.x, rect7.yMax + 4f, num2 * 0.75f - 4f, 24f);
			if (Widgets.ButtonText(rect9, "Randomize".Translate(), true, true, true, null))
			{
				if (this.SelectedGenes.Count == 0)
				{
					Messages.Message("SelectAGeneToRandomizeName".Translate(), MessageTypeDefOf.RejectInput, false);
				}
				else
				{
					GUI.FocusControl(null);
					SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
					this.xenotypeName = GeneUtility.GenerateXenotypeNameFromGenes(this.SelectedGenes);
				}
			}
			Rect rect10 = new Rect(rect9.xMax + 4f, rect9.y, num2 * 0.25f, 24f);
			if (Widgets.ButtonText(rect10, "...", true, true, true, null))
			{
				if (this.SelectedGenes.Count > 0)
				{
					List<string> list = new List<string>();
					int num5 = 0;
					while (list.Count < 20)
					{
						string text3 = GeneUtility.GenerateXenotypeNameFromGenes(this.SelectedGenes);
						if (text3.NullOrEmpty())
						{
							break;
						}
						if (list.Contains(text3) || text3 == this.xenotypeName)
						{
							num5++;
							if (num5 >= 1000)
							{
								break;
							}
						}
						else
						{
							list.Add(text3);
						}
					}
					List<FloatMenuOption> list2 = new List<FloatMenuOption>();
					for (int i = 0; i < list.Count; i++)
					{
						string n = list[i];
						list2.Add(new FloatMenuOption(n, delegate ()
						{
							this.xenotypeName = n;
						}, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
					}
					if (list2.Any<FloatMenuOption>())
					{
						Find.WindowStack.Add(new FloatMenu(list2));
					}
				}
				else
				{
					Messages.Message("SelectAGeneToChooseAName".Translate(), MessageTypeDefOf.RejectInput, false);
				}
			}
			Rect rect11 = new Rect(rect10.xMax + 10f, rect9.y, 24f, 24f);
			if (Widgets.ButtonImage(rect11, this.xenotypeNameLocked ? GeneCreationDialogBase.LockedTex : GeneCreationDialogBase.UnlockedTex, true))
			{
				this.xenotypeNameLocked = !this.xenotypeNameLocked;
				if (this.xenotypeNameLocked)
				{
					SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
				}
				else
				{
					SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
				}
			}
			if (Mouse.IsOver(rect11))
			{
				string str = "LockNameButtonDesc".Translate() + "\n\n" + (this.xenotypeNameLocked ? "LockNameOn" : "LockNameOff").Translate();
				TooltipHandler.TipRegion(rect11, str);
			}
			this.postXenotypeHeight = rect11.yMax - num4;
			this.PostXenotypeOnGUI(rect6.xMin, rect9.y + 24f);
			Rect rect12 = rect;
			rect12.yMin = rect12.yMax - GeneCreationDialogBase.ButSize.y;
			this.DoBottomButtons(rect12);
		}

		protected virtual void DrawSearchRect(Rect rect)
		{
			Rect rect2 = new Rect(rect.width - 300f - this.searchWidgetOffsetX, 11f, 300f, 24f);
			this.quickSearchWidget.OnGUI(rect2, new Action(this.UpdateSearchResults));
		}

		protected virtual bool WithinAcceptableBiostatLimits(bool showMessage)
		{
			if (this.ignoreRestrictions)
			{
				return true;
			}
			if (this.met < GeneTuning.BiostatRange.TrueMin)
			{
				if (showMessage)
				{
					Messages.Message("MetabolismTooLowToCreateXenogerm".Translate(this.met.Named("AMOUNT"), GeneTuning.BiostatRange.TrueMin.Named("MIN")), null, MessageTypeDefOf.RejectInput, false);
				}
				return false;
			}
			return true;
		}

		protected virtual bool CanAccept()
		{
			string text = this.xenotypeName;
			if (text != null && text.Trim().Length == 0)
			{
				Messages.Message("XenotypeNameCannotBeEmpty".Translate(), MessageTypeDefOf.RejectInput, false);
				return false;
			}
			if (!this.WithinAcceptableBiostatLimits(true))
			{
				return false;
			}
			List<BuildingGeneDef> selectedGenes = this.SelectedGenes;
			foreach (BuildingGeneDef geneDef in this.SelectedGenes)
			{
				if (geneDef.prerequisite != null && !selectedGenes.Contains(geneDef.prerequisite))
				{
					Messages.Message("MessageGeneMissingPrerequisite".Translate(geneDef.label).CapitalizeFirst() + ": " + geneDef.prerequisite.LabelCap, null, MessageTypeDefOf.RejectInput, false);
					return false;
				}
			}
			return true;
		}

		private void DrawIconSelector(Rect rect)
		{
			Widgets.DrawHighlight(rect);
			if (Widgets.ButtonImage(rect, this.iconDef.Icon, XenotypeDef.IconColor, true))
			{
				Find.WindowStack.Add(new Dialog_SelectXenotypeIcon(this.iconDef, delegate (XenotypeIconDef i)
				{
					this.iconDef = i;
				}));
			}
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegion(rect, "SelectIconDesc".Translate() + "\n\n" + "ClickToEdit".Translate().Colorize(ColoredText.SubtleGrayColor));
			}
		}

		protected virtual void PostXenotypeOnGUI(float curX, float curY)
		{
		}

		protected abstract void Accept();

		protected abstract void DrawGenes(Rect rect);

		protected virtual void OnGenesChanged()
		{
			this.randomChosenGroups.Clear();
			this.leftChosenGroups.Clear();
			this.cachedOverriddenGenes.Clear();
			this.cachedUnoverriddenGenes.Clear();
			this.tmpGenesWithType.Clear();
			this.gcx = 0;
			this.met = 0;
			this.arc = 0;
			List<BuildingGeneDef> selectedGenes = this.SelectedGenes;
			for (int i = 0; i < selectedGenes.Count; i++)
			{
				if (selectedGenes[i].RandomChosen)
				{
					for (int j = i + 1; j < selectedGenes.Count; j++)
					{
						if (selectedGenes[i].ConflictsWith(selectedGenes[j]))
						{
							if (!this.randomChosenGroups.ContainsKey(selectedGenes[i]))
							{
								this.randomChosenGroups.Add(selectedGenes[i], new List<BuildingGeneDef>
								{
									selectedGenes[i]
								});
							}
							this.randomChosenGroups[selectedGenes[i]].Add(selectedGenes[j]);
						}
					}
				}
			}
			for (int k = 0; k < selectedGenes.Count; k++)
			{
				if (!selectedGenes[k].RandomChosen)
				{
					for (int l = k + 1; l < selectedGenes.Count; l++)
					{
						if (!selectedGenes[l].RandomChosen && selectedGenes[k].ConflictsWith(selectedGenes[l]))
						{
							int num = GeneUtility.GenesInOrder.IndexOf(selectedGenes[k]);
							int num2 = GeneUtility.GenesInOrder.IndexOf(selectedGenes[l]);
							BuildingGeneDef leftMost = (num < num2) ? selectedGenes[k] : selectedGenes[l];
							BuildingGeneDef rightMost = (num >= num2) ? selectedGenes[k] : selectedGenes[l];
							GeneLeftChosenGroup geneLeftChosenGroup = this.leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.leftChosen == leftMost);
							GeneLeftChosenGroup geneLeftChosenGroup2 = this.leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.leftChosen == rightMost);
							if (geneLeftChosenGroup == null)
							{
								geneLeftChosenGroup = new GeneLeftChosenGroup(leftMost);
								this.leftChosenGroups.Add(geneLeftChosenGroup);
							}
							if (geneLeftChosenGroup2 != null)
							{
								foreach (GeneDef item in geneLeftChosenGroup2.overriddenGenes)
								{
									if (!geneLeftChosenGroup.overriddenGenes.Contains(item))
									{
										geneLeftChosenGroup.overriddenGenes.Add(item);
									}
									if (!this.cachedOverriddenGenes.Contains(item))
									{
										this.cachedOverriddenGenes.Add(item);
									}
								}
								this.leftChosenGroups.Remove(geneLeftChosenGroup2);
							}
							if (!geneLeftChosenGroup.overriddenGenes.Contains(rightMost))
							{
								geneLeftChosenGroup.overriddenGenes.Add(rightMost);
							}
							if (!this.cachedOverriddenGenes.Contains(rightMost))
							{
								this.cachedOverriddenGenes.Add(rightMost);
							}
						}
					}
				}
			}
			foreach (GeneLeftChosenGroup geneLeftChosenGroup3 in this.leftChosenGroups)
			{
				List<BuildingGeneDef> overriddenGenes = geneLeftChosenGroup3.overriddenGenes;
				Func<BuildingGeneDef, int> selector;
				if (selector == null)
				{
					selector = ((BuildingGeneDef x) => selectedGenes.IndexOf(x));
				}
				overriddenGenes.SortBy(selector);
			}
			this.cachedUnoverriddenGenes.AddRange(this.SelectedGenes);
			foreach (BuildingGeneDef item2 in this.cachedOverriddenGenes)
			{
				this.cachedUnoverriddenGenes.Remove(item2);
			}
			for (int m = 0; m < selectedGenes.Count; m++)
			{
				this.tmpGenesWithType.Add(new BuildingGeneDefWithType(selectedGenes[m], true));
			}
			foreach (BuildingGeneDef buildingGeneDef in this.tmpGenesWithType.NonOverriddenGenes().Distinct<GeneDef>())
			{
				this.gcx += BuildingGeneDef.biostatCpx;
				this.met += geneDef.biostatMet;
				this.arc += geneDef.biostatArc;
			}
		}

		protected abstract void UpdateSearchResults();

		protected virtual void DoBottomButtons(Rect rect)
		{
			if (Widgets.ButtonText(new Rect(rect.xMax - GeneCreationDialogBase.ButSize.x, rect.y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), this.AcceptButtonLabel, true, true, true, null) && this.CanAccept())
			{
				this.Accept();
			}
			if (Widgets.ButtonText(new Rect(rect.x, rect.y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "Close".Translate(), true, true, true, null))
			{
				this.Close(true);
			}
		}


	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class ITab_ContentsHeartseedHolder : ITab_ContentsBase
	{
		public override IList<Thing> container
		{
			get
			{
				return this.ContainerThing.innerContainer;
			}
		}

		public CompShipGeneContainer ContainerThing
		{
			get
			{
				return base.SelThing.TryGetComp<CompShipGeneContainer>();
			}
		}

		public ITab_ContentsHeartseedHolder()
		{
			this.labelKey = "TabCasketContents";
			this.containedItemsKey = "TabCasketContents";
		}

		public override void OnOpen()
		{
			if (!ModLister.CheckBiotech("genepack container"))
			{
				this.CloseTab();
			}
		}

		protected override void DoItemsLists(Rect inRect, ref float curY)
		{
			CompShipGeneContainer containerThing = this.ContainerThing;
			bool autoLoad = containerThing.autoLoad;
			Rect rect = new Rect(inRect.x, inRect.y, inRect.width, 24f);
			Widgets.CheckboxLabeled(rect, "AllowAllGenepacks".Translate(), ref containerThing.autoLoad, false, null, null, false);
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegionByKey(rect, "AllowAllGenepacksDesc");
			}
			if (autoLoad != containerThing.autoLoad)
			{
				containerThing.leftToLoad.Clear();
			}
			curY += 28f;
			this.ListContainedGenepacks(inRect, containerThing, ref curY);
			if (!containerThing.autoLoad)
			{
				this.ListGenepacksToLoad(inRect, containerThing, ref curY);
				this.ListGenepacksOnMap(inRect, containerThing, ref curY);
			}
		}

		// Token: 0x06008FE8 RID: 36840 RVA: 0x0033E504 File Offset: 0x0033C704
		private void ListContainedGenepacks(Rect inRect, CompShipGeneContainer container, ref float curY)
		{
			GUI.BeginGroup(inRect);
			float num = curY;
			Widgets.ListSeparator(ref curY, inRect.width, this.containedItemsKey.Translate());
			Rect rect = new Rect(0f, num, inRect.width, curY - num - 3f);
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegionByKey(rect, "ContainedGenepacksDesc");
			}
			List<CompHeartSeed> containedGenepacks = container.ContainedGenepacks;
			bool flag = false;
			for (int i = 0; i < containedGenepacks.Count; i++)
			{
				CompHeartSeed genepack = containedGenepacks[i];
				if (genepack != null)
				{
					flag = true;
					this.DoRow(genepack, container, inRect.width, ref curY, true);
				}
			}
			if (!flag)
			{
				Widgets.NoneLabel(ref curY, inRect.width, null);
			}
			GUI.EndGroup();
		}

		private void ListGenepacksToLoad(Rect inRect, CompShipGeneContainer container, ref float curY)
		{
			bool flag = false;
			GUI.BeginGroup(inRect);
			float num = curY;
			Widgets.ListSeparator(ref curY, inRect.width, "GenepacksToLoad".Translate());
			Rect rect = new Rect(0f, num, inRect.width, curY - num - 3f);
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegionByKey(rect, "GenepacksToLoadDesc");
			}
			if (container.leftToLoad != null)
			{
				for (int i = container.leftToLoad.Count - 1; i >= 0; i--)
				{
					CompHeartSeed genepack = container.leftToLoad[i].TryGetComp<CompHeartSeed>();
					if (genepack == null || genepack.parent.Destroyed || genepack.parent.MapHeld != container.parent.Map || !genepack.AutoLoad)
					{
						container.leftToLoad.RemoveAt(i);
					}
					else
					{
						this.DoRow(genepack, container, inRect.width, ref curY, false);
						flag = true;
					}
				}
			}
			if (!flag)
			{
				Widgets.NoneLabel(ref curY, inRect.width, null);
			}
			GUI.EndGroup();
		}

		private void ListGenepacksOnMap(Rect inRect, CompShipGeneContainer container, ref float curY)
		{
			bool flag = false;
			GUI.BeginGroup(inRect);
			float num = curY;
			Widgets.ListSeparator(ref curY, inRect.width, "GenepacksToIgnore".Translate());
			Rect rect = new Rect(0f, num, inRect.width, curY - num - 3f);
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegionByKey(rect, "GenepacksIgnoredDesc");
			}
			List<Thing> list = container.parent.Map.listerThings.ThingsOfDef(ThingDefOf.Genepack);
			for (int i = list.Count - 1; i >= 0; i--)
			{
				Thing thing = list[i];
				if (thing != null)
				{
					CompHeartSeed genepack = thing.TryGetComp<CompHeartSeed>();
					if (genepack.targetContainer == null && genepack.AutoLoad)
					{
						this.DoRow(genepack, container, inRect.width, ref curY, false);
						flag = true;
					}
				}
			}
			if (!flag)
			{
				Widgets.NoneLabel(ref curY, inRect.width, null);
			}
			GUI.EndGroup();
		}

		// Token: 0x06008FEB RID: 36843 RVA: 0x0033E7BC File Offset: 0x0033C9BC
		private void DoRow(CompHeartSeed genepack, CompShipGeneContainer container, float width, ref float curY, bool insideContainer)
		{
			bool flag = container.leftToLoad.Contains(genepack.parent);
			bool flag2 = flag;
			Rect rect = new Rect(0f, curY, width, 28f);
			Rect rect2 = new Rect(rect.width - 24f, curY, 24f, 24f);
			if (insideContainer)
			{
				if (Widgets.ButtonImage(rect2, ITab_ContentsHeartseedHolder.DropTex.Texture, true))
				{
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmRemoveGenepack".Translate(genepack.LabelNoCount), delegate
					{
						this.OnDropThing(genepack.parent, genepack.parent.stackCount);
					}, false, null, WindowLayer.Dialog));
				}
				TooltipHandler.TipRegionByKey(rect2, "EjectGenepackDesc");
			}
			else
			{
				Widgets.Checkbox(rect.width - 24f, curY, ref flag, 24f, false, false, null, null);
				string key = flag ? "RemoveFromLoadingListDesc" : "AddToLoadingListDesc";
				TooltipHandler.TipRegionByKey(rect2, key);
			}
			rect.width -= 24f;
			for (int i = Mathf.Min(genepack.GenesListForReading.Count, 5) - 1; i >= 0; i--)
			{
				BuildingGeneDef geneDef = genepack.GenesListForReading[i];
				Rect rect3 = new Rect(rect.xMax - 22f, rect.yMax - rect.height / 2f - 11f, 22f, 22f);
				Widgets.DefIcon(rect3, geneDef, null, 0.75f, null, false, null, null, null);
				Rect rect4 = rect3;
				rect4.yMin = rect.yMin;
				rect4.yMax = rect.yMax;
				if (Mouse.IsOver(rect4))
				{
					Widgets.DrawHighlight(rect4);
					TooltipHandler.TipRegion(rect4, geneDef.LabelCap + "\n\n" + geneDef.DescriptionFull);
				}
				rect.xMax -= 22f;
			}
			Widgets.InfoCardButton(0f, curY, genepack.parent);
			if (Mouse.IsOver(rect))
			{
				GUI.color = ITab_ContentsBase.ThingHighlightColor;
				GUI.DrawTexture(rect, TexUI.HighlightTex);
			}
			Widgets.ThingIcon(new Rect(24f, curY, 28f, 28f), genepack.parent, 1f, null, false);
			Rect rect5 = new Rect(60f, curY, rect.width - 36f, rect.height);
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect5, genepack.parent.LabelCap.Truncate(rect5.width, null));
			Text.Anchor = TextAnchor.UpperLeft;
			if (Mouse.IsOver(rect))
			{
				TargetHighlighter.Highlight(genepack.parent, true, false, false);
				TooltipHandler.TipRegion(rect, genepack.parent.LabelCap);
			}
			curY += 28f;
			if (flag2 != flag)
			{
				if (!flag)
				{
					genepack.targetContainer = null;
					container.leftToLoad.Remove(genepack.parent);
					return;
				}
				if (!container.CanLoadMore)
				{
					Messages.Message("CanOnlyStoreNumGenepacks".Translate(container.parent, container.Props.maxCapacity).CapitalizeFirst(), container.parent, MessageTypeDefOf.RejectInput, false);
					return;
				}
				genepack.targetContainer = container.parent;
				container.leftToLoad.Add(genepack.parent);
			}
		}

		private static readonly CachedTexture DropTex = new CachedTexture("UI/Buttons/Drop");

		private const float MiniGeneIconSize = 22f;
	}
}

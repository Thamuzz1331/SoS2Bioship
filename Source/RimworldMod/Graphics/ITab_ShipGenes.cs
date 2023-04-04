using System;
using UnityEngine;
using Verse;
using LivingBuildings;
using System.Collections.Generic;

namespace RimWorld
{
	// Token: 0x0200168D RID: 5773
	public class ITab_ShipGenes : ITab
	{
		public override bool Hidden
		{
			get
			{
				return false;
			}
		}
		CompShipHeart heart;
		public override bool IsVisible
		{
			get
			{
				CompShipHeart heart = base.SelThing.TryGetComp<CompShipHeart>();
				return (heart != null);
			}
		}

		public ITab_ShipGenes()
		{
			this.size = new Vector2(Mathf.Min(736f, (float)UI.screenWidth), 550f);
			this.labelKey = "Bioship.TabGenes";
		}


		protected override void FillTab()
		{
			heart = base.SelThing.TryGetComp<CompShipHeart>();
			ITab_ShipGenes.DrawGenesInfo(new Rect(0f, 20f, this.size.x, this.size.y - 20f), heart, 550f, ref this.size, ref this.scrollPosition);
		}

		private static void DrawGeneSections(Rect rect, CompShipHeart target, ref Vector2 scrollPosition)
		{
			ITab_ShipGenes.RecacheGenes(target);
			GUI.BeginGroup(rect);
			Rect rect2 = new Rect(0f, 0f, rect.width - 16f, ITab_ShipGenes.scrollHeight);
			float num = 0f;
			Widgets.BeginScrollView(rect.AtZero(), ref scrollPosition, rect2, true);
			Rect containingRect = rect2;
			containingRect.y = scrollPosition.y;
			containingRect.height = rect.height;
			if (Prefs.DevMode)
            {

            }
			if (ITab_ShipGenes.endogenes.Any<BuildingGene>())
			{
				ITab_ShipGenes.DrawSection(rect, false, ITab_ShipGenes.endogenes.Count, ref num, ref ITab_ShipGenes.endoGenesHeight, delegate (int i, Rect r)
				{
					ITab_ShipGenes.DrawGene(ITab_ShipGenes.endogenes[i], r, GeneType.Endogene, true, true);
				}, containingRect);
				num += 12f;
			}
			ITab_ShipGenes.DrawSection(rect, true, ITab_ShipGenes.xenogenes.Count, ref num, ref ITab_ShipGenes.xenoGenesHeight, delegate (int i, Rect r)
			{
				ITab_ShipGenes.DrawGene(ITab_ShipGenes.xenogenes[i], r, GeneType.Xenogene, true, true);
			}, containingRect);

			if (Event.current.type == EventType.Layout)
			{
				ITab_ShipGenes.scrollHeight = num;
			}
			Widgets.EndScrollView();
			GUI.EndGroup();
		}

		public static void DrawGene(BuildingGene gene, Rect geneRect, GeneType geneType, bool doBackground = true, bool clickable = true)
		{
			ITab_ShipGenes.DrawGeneBasics(gene.def, geneRect, geneType, doBackground, clickable, !gene.Active);
			if (Mouse.IsOver(geneRect))
			{
				string text = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + gene.def.Description;
				if (gene.Overridden)
				{
					text += "\n\n";
					if (gene.overriddenByGene.def == gene.def)
					{
						text += ("OverriddenByIdenticalGene".Translate() + ": " + gene.overriddenByGene.LabelCap).Colorize(ColorLibrary.RedReadable);
					}
					else
					{
						text += ("OverriddenByGene".Translate() + ": " + gene.overriddenByGene.LabelCap).Colorize(ColorLibrary.RedReadable);
					}
				}
				if (clickable)
				{
					text = text + "\n\n" + "ClickForMoreInfo".Translate().ToString().Colorize(ColoredText.SubtleGrayColor);
				}
				TooltipHandler.TipRegion(geneRect, text);
			}
		}

		private static void DrawGeneBasics(BuildingGeneDef gene, Rect geneRect, GeneType geneType, bool doBackground, bool clickable, bool overridden)
		{
			GUI.BeginGroup(geneRect);
			Rect rect = geneRect.AtZero();
			if (doBackground)
			{
				Widgets.DrawHighlight(rect);
				GUI.color = new Color(1f, 1f, 1f, 0.05f);
				Widgets.DrawBox(rect, 1, null);
				GUI.color = Color.white;
			}
			float num = rect.width - Text.LineHeight;
			Rect rect2 = new Rect(geneRect.width / 2f - num / 2f, 0f, num, num);
			Color iconColor = gene.IconColor;
			if (overridden)
			{
				iconColor.a = 0.75f;
				GUI.color = ColoredText.SubtleGrayColor;
			}
			CachedTexture cachedTexture = ITab_ShipGenes.GeneBackground_Archite;
			if (gene.architeCost == 0)
			{
				if (geneType != GeneType.Endogene)
				{
					if (geneType == GeneType.Xenogene)
					{
						cachedTexture = ITab_ShipGenes.GeneBackground_Xenogene;
					}
				}
				else
				{
					cachedTexture = ITab_ShipGenes.GeneBackground_Endogene;
				}
			}
			GUI.DrawTexture(rect2, cachedTexture.Texture);
			GUI.DrawTexture(rect2, gene.Icon);
			Widgets.DefIcon(rect2, gene, null, 0.9f, null, false, new Color?(iconColor), null, null);
			Text.Font = GameFont.Tiny;
			float num2 = Text.CalcHeight(gene.LabelCap, rect.width);
			Rect rect3 = new Rect(0f, rect.yMax - num2, rect.width, num2);
			GUI.DrawTexture(new Rect(rect3.x, rect3.yMax - num2, rect3.width, num2), TexUI.GrayTextBG);
			Text.Anchor = TextAnchor.LowerCenter;
			if (overridden)
			{
				GUI.color = ColoredText.SubtleGrayColor;
			}
			if (doBackground && num2 < (Text.LineHeight - 2f) * 2f)
			{
				rect3.y -= 3f;
			}
			Widgets.Label(rect3, gene.LabelCap);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			if (clickable)
			{
				if (Widgets.ButtonInvisible(rect, true))
				{
					Find.WindowStack.Add(new Dialog_InfoCard(gene, null));
				}
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
			}
			GUI.EndGroup();
		}

		public static void DrawGenesInfo(Rect rect, CompShipHeart target, float initialHeight, ref Vector2 size, ref Vector2 scrollPosition)
		{
			Rect rect2 = rect;
			Rect position = rect2.ContractedBy(10f);
			GUI.BeginGroup(position);
			float num = BiostatsTable.HeightForBiostats(ITab_ShipGenes.archoGenes);
			Rect rect3 = new Rect(0f, 0f, position.width, position.height - num - 12f);
			ITab_ShipGenes.DrawGeneSections(rect3, target, ref scrollPosition);
			Rect rect4 = new Rect(0f, rect3.yMax + 6f, position.width - 140f - 4f, num);
			rect4.yMax = rect3.yMax + num + 6f;
			BiostatsTable.Draw(rect4, ITab_ShipGenes.complexity, ITab_ShipGenes.metabolicFactor, ITab_ShipGenes.archoGenes, false, false, -1);
			//ITab_ShipGenes.TryDrawXenotype(target, rect4.xMax + 4f, rect4.y + Text.LineHeight / 2f);
			if (Event.current.type == EventType.Layout)
			{
				float num2 = ITab_ShipGenes.endoGenesHeight + ITab_ShipGenes.xenoGenesHeight + num + 12f + 70f;
				if (num2 > initialHeight)
				{
					size.y = Mathf.Min(num2, (float)(UI.screenHeight - 35) - 165f - 30f);
				}
				else
				{
					size.y = initialHeight;
				}
				ITab_ShipGenes.xenoGenesHeight = 0f;
				ITab_ShipGenes.endoGenesHeight = 0f;
			}
			GUI.EndGroup();
		}

		private static void DrawSection(Rect rect, bool xeno, int count, ref float curY, ref float sectionHeight, Action<int, Rect> drawer, Rect containingRect)
		{
			Widgets.Label(10f, ref curY, rect.width, (xeno ? "Xenogenes" : "Endogenes").Translate().CapitalizeFirst(), (xeno ? "XenogenesDesc" : "EndogenesDesc").Translate());
			float num = curY;
			Rect rect2 = new Rect(rect.x, curY, rect.width, sectionHeight);
			if (xeno && count == 0)
			{
				Text.Anchor = TextAnchor.UpperCenter;
				GUI.color = ColoredText.SubtleGrayColor;
				rect2.height = Text.LineHeight;
				Widgets.Label(rect2, "(" + "NoXenogermImplanted".Translate() + ")");
				GUI.color = Color.white;
				Text.Anchor = TextAnchor.UpperLeft;
				curY += 90f;
			}
			else
			{
				Widgets.DrawMenuSection(rect2);
				float num2 = (rect.width - 12f - 630f - 36f) / 2f;
				curY += num2;
				int num3 = 0;
				int num4 = 0;
				for (int i = 0; i < count; i++)
				{
					if (num4 >= 6)
					{
						num4 = 0;
						num3++;
					}
					else if (i > 0)
					{
						num4++;
					}
					Rect rect3 = new Rect(num2 + (float)num4 * 90f + (float)num4 * 6f, curY + (float)num3 * 90f + (float)num3 * 6f, 90f, 90f);
					if (containingRect.Overlaps(rect3))
					{
						drawer(i, rect3);
					}
				}
				curY += (float)(num3 + 1) * 90f + (float)num3 * 6f + num2;
			}
			if (Event.current.type == EventType.Layout)
			{
				sectionHeight = curY - num;
			}
		}

		private static void RecacheGenes(CompShipHeart heart)
        {
			ITab_ShipGenes.endogenes.Clear();
			ITab_ShipGenes.xenogenes.Clear();
			ITab_ShipGenes.archoGenes = 0;
			ITab_ShipGenes.complexity = 0;
			ITab_ShipGenes.metabolicFactor = 0;
			foreach(BuildingGene g in heart.genes)
            {
				ITab_ShipGenes.complexity += g.def.complexity;
				ITab_ShipGenes.metabolicFactor += g.def.metabolicCost;
				if (g.def.architeCost > 0)
                {
					ITab_ShipGenes.archoGenes += g.def.architeCost;
				}
				if (g.geneLineGene)
                {
					ITab_ShipGenes.endogenes.Add(g);
                } else
                {
					ITab_ShipGenes.xenogenes.Add(g);
                }
            }
		}

		protected static List<BuildingGene> endogenes = new List<BuildingGene>();
		protected static List<BuildingGene> xenogenes = new List<BuildingGene>();
		protected static int archoGenes;
		protected static int complexity;
		protected static int metabolicFactor;
		protected static float endoGenesHeight;
		protected static float xenoGenesHeight;
		protected static float scrollHeight;

		protected Vector2 scrollPosition;

		protected const float TopPadding = 20f;

		public const float GeneSize = 90f;

		public const float GeneGap = 6f;

		public const int MaxGenesHorizontal = 7;

		public const float InitialWidth = 736f;

		protected const float InitialHeight = 550f;

		private static readonly CachedTexture GeneBackground_Archite = new CachedTexture("UI_Elements/Gene_Background");
		private static readonly CachedTexture GeneBackground_Endogene = new CachedTexture("UI_Elements/Gene_Background");
		private static readonly CachedTexture GeneBackground_Xenogene = new CachedTexture("UI_Elements/Gene_Background");
	}
}

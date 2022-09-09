using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using BioShip;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class ITab_Mutations : ITab
	{
//		private Vector2 scrollPos;

		public ITab_Mutations()
		{
			this.labelKey = "Bioship.TabMutations";
			this.size = new Vector2(600f, 520f);
		}

		public override bool IsVisible
		{
			get
			{
				Thing sel = base.SelThing;
				CompMutationWorker mutationWorker = sel.TryGetComp<CompMutationWorker>();
				return (mutationWorker != null);
			}
		}
		private CompMutationWorker mutationWorker;
		protected override void FillTab()
		{
			Thing sel = base.SelThing;
			mutationWorker = sel.TryGetComp<CompMutationWorker>();

			Rect rect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(10f);
			//Widgets.BeginScrollView(rect, ref this.scrollPos, new Rect(0f, 0f, 600f, rect.height - 20f), true);
			ShowMutations(rect.TopHalf().TopHalf(), "", mutationWorker.GetMutationsForTier("quirk"), 1);
			if (mutationWorker.GetMutationsForTier("archo").Count > 0)
            {
				ShowMutations(rect.TopHalf().TopHalf(), "", mutationWorker.GetMutationsForTier("archo"), 2);
            }
			ShowMutations(rect.TopHalf().BottomHalf(), "tier3", mutationWorker.GetMutationsForTier("tier3"), 2);
			ShowMutations(rect.BottomHalf().TopHalf(), "tier2", mutationWorker.GetMutationsForTier("tier2"), 4);
			ShowMutations(rect.BottomHalf().BottomHalf(), "tier1", mutationWorker.GetMutationsForTier("tier1"), 6);
			//Widgets.EndScrollView();
		}

		private void ShowMutations(Rect mutationBar, string label, List<IMutation> muts, int slots)
        {
			Widgets.Label(mutationBar.TopPartPixels(30f), label.Translate());
			float xIncrement = mutationBar.width / slots;
			float initialOffset = (xIncrement/2)-40f;
			for (int i = 0; i < slots; i++)
            {
				Rect mutSlot = new Rect(mutationBar.x + initialOffset + xIncrement * (float)(i), mutationBar.y + 25f, 80f, 80f);
				this.DoEmptyRect(mutSlot, label, i < muts.Count);
				if (i < muts.Count)
                {
					DrawMutationIcon(mutSlot, muts[i], label);
                }
            }
        }

		public void DoEmptyRect(Rect inRect, string tier, bool filled = false)
		{
			GUI.DrawTexture(inRect, BioShip.BioShip.MutationBackground);
			
			if (Prefs.DevMode)
			{
				if (!filled)
                {
					if (Widgets.ButtonInvisible(inRect, true))
					{
						List<FloatMenuOption> options = new List<FloatMenuOption>();
						foreach(IMutation mut in mutationWorker.GetMutationOptionsForTeir(tier))
                        {
							options.Add(new FloatMenuOption(mut.ToString(),
								delegate() {
									mutationWorker.SpreadMutation(mutationWorker.body, mut);
								},
								MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
                        }
						if (options.Count > 0)
                        {
							FloatMenu menu = new FloatMenu(options);
							Find.WindowStack.Add(menu);
                        }
					}
                }
			}
			Widgets.DrawHighlightIfMouseover(inRect);
		}

		public void DrawMutationIcon(Rect inRect, IMutation mut, string tier)
        {
			Widgets.Label(inRect.TopPartPixels(30f), mut.ToString());
			TooltipHandler.TipRegion(inRect, new TipSignal(string.Format("{0}", mut.GetDescription())));
			bool tierAccessible = (tier == mutationWorker.tier) ||
				(tier == "tier1" && mutationWorker.GetMutationsForTier("tier2").Count == 0) ||
				(tier == "tier2" && mutationWorker.GetMutationsForTier("tier3").Count == 0);

			if (Prefs.DevMode && tierAccessible)
            {
				if (Widgets.ButtonInvisible(inRect, true))
				{
					List<FloatMenuOption> options = new List<FloatMenuOption>(){
						new FloatMenuOption("Remove",
							delegate() {
								mutationWorker.RemoveMutationFromBody(mutationWorker.body, mut);
								if (mutationWorker.tier != tier)
                                {
									mutationWorker.DowngradeMutationTier(tier);
                                }
							},
							MenuOptionPriority.Default, null, null, 0f, null, null, true, 0)
					};
					FloatMenu menu = new FloatMenu(options);
					Find.WindowStack.Add(menu);
				}
            }
        }
	}
}
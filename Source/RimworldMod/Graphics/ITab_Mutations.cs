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

		protected override void FillTab()
		{
			Thing sel = base.SelThing;
			CompMutationWorker mutationWorker = sel.TryGetComp<CompMutationWorker>();

			Rect rect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(10f);
			//Widgets.BeginScrollView(rect, ref this.scrollPos, new Rect(0f, 0f, 600f, rect.height - 20f), true);
			ShowMutations(rect.TopHalf().BottomHalf(), "BioShip.TierThree", mutationWorker.GetMutationsForTier("tier3"), 2);
			ShowMutations(rect.BottomHalf().TopHalf(), "BioShip.TierTwo", mutationWorker.GetMutationsForTier("tier2"), 4);
			ShowMutations(rect.BottomHalf().BottomHalf(), "BioShip.TierOne", mutationWorker.GetMutationsForTier("tier1"), 6);
			//Widgets.EndScrollView();
		}

		private void ShowMutations(Rect mutationBar, string label, List<IMutation> muts, int slots)
        {
			Widgets.Label(mutationBar.TopPartPixels(30f), label);
			float xIncrement = mutationBar.width / slots;
			float initialOffset = (xIncrement/2)-40f;
			for (int i = 0; i < slots; i++)
            {
				Rect mutSlot = new Rect(mutationBar.x + initialOffset + xIncrement * (float)(i), mutationBar.y + 25f, 80f, 80f);
				this.DoEmptyRect(mutSlot, i < muts.Count);
				if (i < muts.Count)
                {
					DrawMutationIcon(mutSlot, muts[i]);
                }
            }
        }

		public void DoEmptyRect(Rect inRect, bool filled = false)
		{
			GUI.DrawTexture(inRect, BioShip.BioShip.MutationBackground);
			
/*
			if (!this.EditMode || filled)
			{
				return;
			}
			Widgets.DrawHighlightIfMouseover(inRect);
			if (Widgets.ButtonInvisible(inRect, true))
			{
				Find.WindowStack.Add(new FloatMenu((from power in DefDatabase<PowerDef>.AllDefs
				where power.powerType == type
				select new FloatMenuOption(power.LabelCap, delegate()
				{
					this.SelPowerTracker.AddPower(power);
				}, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0)).ToList<FloatMenuOption>()));
			}
*/
		}

		public void DrawMutationIcon(Rect inRect, IMutation mut)
        {
			Widgets.Label(inRect.TopPartPixels(30f), mut.ToString());
			TooltipHandler.TipRegion(inRect, new TipSignal(string.Format("{0}", mut.GetDescription())));

        }
	}
}
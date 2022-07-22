using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x0200004F RID: 79
	public class ITab_Mutations : ITab
	{
		// Token: 0x0600016F RID: 367 RVA: 0x0000870E File Offset: 0x0000690E
		public ITab_Mutations()
		{
			this.labelKey = "Bioship.TabMutations";
			this.size = new Vector2(550f, 350f);
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

		}

		private void ShowMutations(string label, List<IHediff> muts, int slots)
        {

        }
	}
}
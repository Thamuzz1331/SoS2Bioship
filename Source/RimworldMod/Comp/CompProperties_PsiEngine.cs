using System;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
	public class CompProperties_PsiEngine : CompProperties_EngineTrail
	{
		public CompProperties_PsiEngine()
		{
			this.compClass = typeof(CompPsiEngine);
		}
	}
}


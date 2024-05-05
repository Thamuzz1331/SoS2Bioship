using System;
using Verse;
using SaveOurShip2;

namespace RimWorld
{
    [StaticConstructorOnStartup]
	public class CompProperties_PsiEngine : CompProps_EngineTrail
	{
		public CompProperties_PsiEngine()
		{
			this.compClass = typeof(CompPsiEngine);
		}
	}
}


using System;
using Verse;
using SaveOurShip2;

namespace RimWorld
{
    [StaticConstructorOnStartup]
	public class CompProperties_ReactionlessEngine : CompProps_EngineTrail
	{
		public CompProperties_ReactionlessEngine()
		{
			this.compClass = typeof(CompReactionlessEngine);
		}
	}
}


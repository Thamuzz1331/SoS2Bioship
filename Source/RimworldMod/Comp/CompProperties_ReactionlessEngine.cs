using System;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
	public class CompProperties_ReactionlessEngine : CompProperties_EngineTrail
	{
		public CompProperties_ReactionlessEngine()
		{
			this.compClass = typeof(CompReactionlessEngine);
		}
	}
}


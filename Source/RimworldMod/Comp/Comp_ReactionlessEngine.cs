using SaveOurShip2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompReactionlessEngine : CompPsiEngine
    {
        public override void PostSpawnSetup(bool b)
        {
            base.PostSpawnSetup(b);
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }

        public override void CompTick()
        {
			base.CompTick();
        }
	}
}

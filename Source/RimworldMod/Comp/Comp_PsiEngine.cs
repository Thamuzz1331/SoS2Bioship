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
    public class CompPsiEngine : CompEngineTrail
    {
		protected Mote psiMote;
		protected float emitCounter = 0;
		public float emitInterval = 120;

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
            if (active) {
				if (emitCounter <= 0)
				{
					Emit();
					emitCounter = emitInterval * (1 + (Rand.Range(-15, 15)/100));
				}
				emitCounter--;
			}
        }
		public override int Thrust
        {
            get
            {
				CompBuildingBodyPart bp = parent.TryGetComp<CompBuildingBodyPart>();
				if (bp != null && bp.CoreSpawned)
                {
					return Mathf.RoundToInt(Props.thrust * bp.Core.GetStat("movementSpeed"));
                } 
                return Props.thrust;
            }
        }


		protected void Emit()
		{
			if (!this.parent.Spawned)
			{
				return;
			}
			Vector3 vector = new Vector3(0f, 1f, 0f);
			Vector3 vector2 = this.parent.DrawPos + vector;
			if (vector2.InBounds(this.parent.Map))
			{
				this.psiMote = MoteMaker.MakeStaticMote(vector2, this.parent.Map, ThingDef.Named("Mote_PsychicConditionCauserEffect"), .5f);
			}
		}
	}
}

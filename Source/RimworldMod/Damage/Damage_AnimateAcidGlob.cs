using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
	class AnimateAcidGlob : AcidGlob
	{
		bool canSpawn = true;
		protected override void Tick()
        {
			if (damageCountdown <= 0 && canSpawn && this.damageRemaining > 240f)
            {
			//TODO: Fix this
			/*
				PawnGenerationRequest req = new PawnGenerationRequest(
					PawnKindDef.Named("AcidBlob_Small"), 
					null, 
					PawnGenerationContext.NonPlayer, 
					-1, 
					false, 
					true, 
					false, 
					false, 
					false, true, 0, false, false, false, false, false, false, false, true, 0, 0, forceNoIdeo: true, forbidAnyTitle: true);
				Pawn blob = PawnGenerator.GeneratePawn(req);
				GenSpawn.Spawn(blob, this.Position, this.Map, Rot4.North);
				this.damageRemaining -= 240f;
				canSpawn = false;
			*/
			}
			base.Tick();
		}
	}
}

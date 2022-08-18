using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
	class MotileAcidGlob : AcidGlob
	{
		public static HashSet<MotileAcidGlob> seenGlobs = new HashSet<MotileAcidGlob>();
		public float spreadCountdown = 0;
		private float spreadInterval = 30f;
		public override void Tick()
        {
			if (this.damageRemaining > 30f && spreadCountdown <= 0)
            {
				if (CanSpreadFromLocation(this.Position))
                {
					seenGlobs.Clear();
					seenGlobs.Add(this);
					this.ForceOpening(this);
					seenGlobs.Clear();
					foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(this.Position))
					{
						if (CanSpreadToLocation(c))
                        {
							SpreadTo(c);
							break;
						}
					}
				}
				spreadCountdown = spreadInterval;
            }
			spreadCountdown--;
			base.Tick();
        }

		bool CanSpreadFromLocation(IntVec3 c)
        {
			return !c.Filled(this.Map);
        }

		bool CanSpreadToLocation(IntVec3 c)
        {
			AcidGlob g = c.GetFirstThing<AcidGlob>(this.Map);
			return (c.GetFirstThing<AcidGlob>(this.Map) == null && !c.Filled(this.Map) && GetValidTarget(c).Count > 0);
        }

		public virtual void ForceOpening(MotileAcidGlob origin)
		{
			bool lastInChain = true;
			List<MotileAcidGlob> toForce = new List<MotileAcidGlob>();
			foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(this.Position))
			{
				MotileAcidGlob g = c.GetFirstThing<MotileAcidGlob>(this.Map);
				if (g != null && !seenGlobs.Contains(g))
				{
					seenGlobs.Add(g);
					toForce.Add(g);
					lastInChain = false;
				}
			}
			if (lastInChain && !CanSpreadFromLocation(this.Position))
            {
				if (this.damageRemaining > 1)
                {
					origin.damageRemaining += this.damageRemaining;
				}
				this.damageRemaining = 1;
            }
			if (!lastInChain)
            {
				foreach(MotileAcidGlob m in toForce)
                {
					m.ForceOpening(origin);
                }
            }
		}

		public void SpreadTo(IntVec3 c)
        {
			
			MotileAcidGlob obj = (MotileAcidGlob)ThingMaker.MakeThing(ThingDef.Named("MotileAcidGlob"));
			obj.damageRemaining = (this.damageRemaining)/2f;
			obj.damageAmount = this.damageAmount;
			obj.damageCountdown = Rand.RangeInclusive(1, 9);
			obj.spreadCountdown = this.spreadInterval;
			GenSpawn.Spawn(obj, c, this.Map, Rot4.North);
			this.damageRemaining = (this.damageRemaining)/2;
        }
	}
}

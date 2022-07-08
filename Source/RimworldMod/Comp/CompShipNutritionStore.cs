using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using SaveOurShip2;

namespace RimWorld
{
    public class CompShipNutritionStore : CompShipNutrition
    {
        private CompProperties_ShipNutritionStore Props => (CompProperties_ShipNutritionStore)props;
        public float currentNutrition = 0f;


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                this.currentNutrition = Props.initialNutrition;
            }
        }

        public float getNutrientCapacity()
        {
            if (body != null && body.heart != null)
            {
                return (Props.nutrientCapacity * body.heart.getStatMultiplier("nutrientCapacity", parent.def));
            }
            return Props.nutrientCapacity;
        }
        public float getCurrentNutrition()
        {
            return currentNutrition;
        }
        public float storeNutrition(float qty)
        {
            float overflow = 0f;
            float toStore = qty * 0.5f;
            if (body != null && body.heart != null)
            {
                toStore = toStore * body.heart.getStatMultiplier("storageEfficiency", parent.def);
            }
            currentNutrition += toStore;
            overflow = currentNutrition - getNutrientCapacity();
            if (overflow <= 0)
            {
                return 0;
            }
            currentNutrition = getNutrientCapacity();
                        overflow = overflow / 0.5f;
            if (body != null && body.heart != null)
            {
                toStore = toStore / body.heart.getStatMultiplier("storageEfficiency", parent.def);
            }
            return overflow;
        }
        public float consumeNutrition(float qty)
        {
            currentNutrition -= qty;
            if (currentNutrition < 0)
            {
                float ret = currentNutrition;
                currentNutrition = 0;
                return ret * -1;
            }
            return 0;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref currentNutrition, "currentNutrition", 0f);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
	        foreach (Gizmo gizmo in base.CompGetGizmosExtra())
	        {
		        yield return gizmo;
	        }
	        if (Prefs.DevMode)
	        {
		        if ((this.getNutrientCapacity() - this.getCurrentNutrition()) > 0f)
		        {
			        yield return new Command_Action
			        {
				        defaultLabel = "DEBUG: Fill",
				        action = delegate()
				        {
					        this.currentNutrition = this.getNutrientCapacity();
				        }
			        };
		        }
		        if (this.currentNutrition > 0f)
		        {
			        yield return new Command_Action
			        {
				        defaultLabel = "DEBUG: Empty",
				        action = delegate()
				        {
					        this.consumeNutrition(this.currentNutrition);
				        }
			        };
		        }
	        }
        }
    }
}
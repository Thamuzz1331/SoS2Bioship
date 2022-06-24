using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompShipNutrition : ThingComp
    {
        public String heartId = "NA";
        public ShipBody body = null;

        public void SetId(String _id)
        {
            heartId = _id;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if(respawningAfterLoad)
            {
                ((ShipBodyMapComp)this.parent.Map.components.Where(t => t is ShipBodyMapComp).FirstOrDefault()).Register(this);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<String>(ref heartId, "heartId", "NA");
        }
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            body.DeRegister(this);
        }

        public override string CompInspectStringExtra()
        {
            if (body != null)
            {
                return "Nutrition " + body.nutritionGen + "/" + body.passiveConsumption + "/" + body.tempHunger + "\n" + 
                    body.currentNutrition + "/" + body.nutritionCapacity;
            }
            return "";
        }

    }
}
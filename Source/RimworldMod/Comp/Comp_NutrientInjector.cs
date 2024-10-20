using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;


namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompNutrientInjector : CompFacility
    {
        private CompProperties_NutrientInjector FueledProps => (CompProperties_NutrientInjector)props;

        CompRefuelable refuelable;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            refuelable = parent.TryGetComp<CompRefuelable>();

        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (refuelable.HasFuel)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Inject Nutrients",
                    icon = ContentFinder<Texture2D>.Get("UI_Elements/Gizmo_InjectDrug"),
                    action = delegate ()
                    {
                        foreach (Thing heart in this.LinkedBuildings)
                        {
                            heart.TryGetComp<CompShipNutritionStore>().storeNutrition(50 / this.LinkedBuildings.Count);
                        }
                        refuelable.ConsumeFuel(1.0f);
                    }
                 };
            }
        }
    }
}
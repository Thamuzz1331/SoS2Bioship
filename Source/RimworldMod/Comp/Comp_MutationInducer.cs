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
    public class CompMutationInducer : CompFacility
    {
        private CompProperties_FueledFacility FueledProps => (CompProperties_FueledFacility)props;

        CompRefuelable refuelable;
        CompMutationWorker worker;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            refuelable = parent.TryGetComp<CompRefuelable>();
            foreach(Thing heart in this.LinkedBuildings)
            {
                worker = heart.TryGetComp<CompMutationWorker>();
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            foreach (Thing heart in this.LinkedBuildings)
            {
                if (refuelable.IsFull && heart.TryGetComp<CompMutationWorker>().CanMutate)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "Induce Mutation",
                        action = delegate ()
                        {
                            heart.TryGetComp<CompMutationWorker>().mutationCountdown = 60000 * 3;
                            heart.TryGetComp<CompMutationWorker>().mutating = true;
                        }
                    };


                }

            }
        }
    }
}
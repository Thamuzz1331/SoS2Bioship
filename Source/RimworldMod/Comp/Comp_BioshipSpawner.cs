using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompBioshipSpawner : CompNutritionConsumer
    {
        private CompProperties_BioshipSpawner SpawnerProps => (CompProperties_BioshipSpawner)props;


        Pawn servitor = null;
        bool growing = false;
        bool readyToSpawn = false;
        float gesationCountdown = 0f;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref servitor, "servitor");
            Scribe_Values.Look(ref growing, "growing", false);
            Scribe_Values.Look(ref readyToSpawn, "readyToSpawn", false);
            Scribe_Values.Look(ref gesationCountdown, "gesationCountdown", 0f);
        }


        public override float getConsumptionPerPulse()
        {
            if (growing)
            {
                return SpawnerProps.consumptionPerPulse;
            } else
            {
                return 0f;
            }
        }

        public override void CompTick()
        {
            if (growing)
            {
                if (gesationCountdown <= 0)
                {
                    growing = false;
                    readyToSpawn = true;
                }
                gesationCountdown--;
            }
            if (servitor != null && servitor.Destroyed)
            {
                servitor = null;
            }
        }

        public virtual void DoSpawn()
        {

        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (servitor == null && !growing && !readyToSpawn)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Start Growth",
                    action = delegate ()
                    {
                        growing = true;
                        gesationCountdown = SpawnerProps.gestationTime;
                    }
                };
            }
            if (readyToSpawn)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Start Growth",
                    action = delegate ()
                    {
                        DoSpawn();
                    }
                };
            }
        }

    }
}
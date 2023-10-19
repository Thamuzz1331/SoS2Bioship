using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_DrugInjector : CompProperties_Facility
    {
        public string drugHediff;
        public string addictionHediff;
        public float drugDuration;
        public float addictionChance;
        public float maxWithdrawl;
        public float withdrawlRate;
        public float massDosageMult;
        public float massAddictionMult;
        public CompProperties_DrugInjector()
        {
            compClass = typeof(CompDrugInjector);
        }

    }
}

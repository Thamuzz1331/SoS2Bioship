using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    public class CompHeartSeed : ThingComp
    {
        public List<BuildingGeneDef> heartGenes = null;
        public ShipGenelineDef geneline = null;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref geneline, "geneline");
            Scribe_Collections.Look(ref heartGenes, "heartGenes", LookMode.Def);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (mode == DestroyMode.Vanish)
            {
                CompHeartSeed.ExoDefs = this.heartGenes;
                CompHeartSeed.Geneline = this.geneline;
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder b = new StringBuilder(base.CompInspectStringExtra());
            b.Append("Geneline " + geneline.label);
            foreach(BuildingGeneDef gDef in heartGenes)
            {
                b.Append("\n" + gDef.label);
            }
            return b.ToString();
        }

        public static ShipGenelineDef Geneline = null;
        public static List<BuildingGeneDef> ExoDefs = null;
    }
}

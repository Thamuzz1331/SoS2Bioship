using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    [StaticConstructorOnStartup]
	public class Building_ShipHeart : Building_ShipBridge
	{
        public bool recentReplace = false;

        public override void Tick()
        {
            if (this.recentReplace)
            {
                this.terminate = false;
                this.recentReplace = false;
            }
            base.Tick();
        }

		public override void DeSpawn(DestroyMode mode)
        {
            CompShipHeart heart = this.TryGetComp<CompShipHeart>();
            if (heart != null && mode != DestroyMode.Vanish)
            {
                List<BuildingGeneDef> dropgenes = new List<BuildingGeneDef>();
                List<BuildingGene> exoGenes = new List<BuildingGene>();
                foreach (BuildingGene gene in heart.genes)
                {
                    if (!gene.geneLineGene)
                    {
                        exoGenes.Add(gene);
                    }
                }
                for (int i = 0; exoGenes.Count > 0 && i < 3; i++)
                {
                    BuildingGene d = exoGenes.RandomElement();
                    dropgenes.Add(d.def);
                    exoGenes.Remove(d);
                }
                Thing heartSeed = ThingMaker.MakeThing(ThingDef.Named("HeartSeed"));
                heartSeed.Position = this.Position;
                CompHeartSeed seed = heartSeed.TryGetComp<CompHeartSeed>();
                if (seed != null)
                {
                    seed.geneline = heart.geneline;
                    seed.heartGenes = dropgenes;
                }
                heartSeed.SpawnSetup(this.Map, false);

                for (int i = 0; i < 4; i++)
                {
                    Thing organSeed = ThingMaker.MakeThing(ThingDef.Named("OrganSeed"));
                    organSeed.Position = this.Position;
                    organSeed.SpawnSetup(this.Map, false);
                }
            }
            base.DeSpawn(mode);
        }
    }
}
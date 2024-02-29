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
    public class CompShipHeart : CompBuildingCore, IAggressionSource
    {
        public CompProperties_ShipHeart HeartProps => (CompProperties_ShipHeart)props;

        int IAggressionSource.GetAggressionValue()
        {
            return 2;
        }

        public int Threat
        {
            get
            {
                return 5;
            }
        }

        public CompRegenWorker regenWorker;
        public CompAggression aggression;
        public bool initialized = false;
        public float maxResistence = 0.15f;
        public ShipGenelineDef geneline = null;
        public Dictionary<DamageDef, float> resistances = new Dictionary<DamageDef, float>();

        public Dictionary<string, DefOptions> defs = new Dictionary<string, DefOptions>()
        {
            {"smallTurretOptions", new DefOptions(new List<ThingDef>(){
            })},
            {"mediumTurretOptions", new DefOptions(new List<ThingDef>(){
            })},
            {"largeTurretOptions", new DefOptions(new List<ThingDef>(){
            })},
            {"spinalTurretOptions", new DefOptions(new List<ThingDef>(){})},
            {"armor", new DefOptions(new List<ThingDef>(){
            })},
            {"smallMawOptions", new DefOptions(new List<ThingDef>(){
                ThingDef.Named("Maw_Small"),
            })},
            {"shieldEmitter", new DefOptions(new List<ThingDef>(){
                ThingDef.Named("BioShieldGenerator")
            })},
            {"HeavySpineLauncher", new DefOptions(new List<ThingDef>(){
                ThingDef.Named("Spine_Heavy")
            })},
            {"LightSpineLauncher", new DefOptions(new List<ThingDef>(){
                ThingDef.Named("Spine_Light")
            })},
        };
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref initialized, "initialized", false);
            Scribe_Defs.Look<ShipGenelineDef>(ref geneline, "geneline");
            Scribe_Collections.Look<DamageDef, float>(ref resistances, "resistences", LookMode.Def, LookMode.Value);
            Scribe_Collections.Look<string, DefOptions>(ref defs, "defs", LookMode.Value, LookMode.Deep);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!stats.ContainsKey("regenEfficiency"))
                stats.Add("regenEfficiency", 1f);
            if (!stats.ContainsKey("regenSpeed"))
                stats.Add("regenSpeed", 1f);
            if (!stats.ContainsKey("conciousness"))
                stats.Add("conciousness", 1f);
            if (!stats.ContainsKey("movementSpeed"))
                stats.Add("movementSpeed", 1f);
            if (!stats.ContainsKey("shieldStrength"))
                stats.Add("shieldStrength", 0.75f);
            if (!stats.ContainsKey("toxSensitivity"))
                stats.Add("toxSensitivity", 1f);
            regenWorker = parent.TryGetComp<CompRegenWorker>();
            aggression = parent.TryGetComp<CompAggression>();
            base.PostSpawnSetup(respawningAfterLoad);
            regenWorker.body = this.body;
            if (!respawningAfterLoad && !initialized)
            {
                if (CompHeartSeed.Geneline != null)
                {
                    this.geneline = CompHeartSeed.Geneline;
                    CompHeartSeed.Geneline = null;
                } else
                {
                    this.geneline = ShipGenelineDef.Named(HeartProps.geneline);
                }
                ShipGeneline g = ShipGenelineMaker.MakeShipGeneline(this.geneline);
                this.AddGene(g.smallTurretGene);
                this.AddGene(g.mediumTurretGene);
                this.AddGene(g.largeTurretGene);
                this.AddGene(g.spinalTurretGene);
                this.AddGene(g.armor);
                foreach (BuildingGene b in g.genes)
                {
                    this.AddGene(b);
                }
                foreach (string exoGeneString in this.HeartProps.exoGenes)
                {
                    BuildingGene gene = BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(exoGeneString));
                    this.AddGene(gene);
                }
                if (CompHeartSeed.ExoDefs != null)
                {
                    foreach (BuildingGeneDef gDef in CompHeartSeed.ExoDefs)
                    {
                        BuildingGene gene = BuildingGeneMaker.MakeBuildingGene(gDef);
                        this.AddGene(gene);
                    }
                    CompHeartSeed.ExoDefs = null;
                }
                BuildingHediff toxBuildup = BuildingHediffMaker.MakeBuildingHediff(BuildingHediffDef.Named("ShipToxBuildup"));
                this.AddHediff(toxBuildup);
                initialized = true; //When the ship takes off the comps get regenerated.  This ensures that the initial mutations will only proc once.
            }
            aggression.aggressionSources.Add(this);
            regenWorker.toxicBuildup = (BuildingHediff_ToxicBuildup)hediffs.Where(x => x is BuildingHediff_ToxicBuildup).FirstOrDefault();
            if (!respawningAfterLoad)
            {
                if (parent.TryGetComp<CompShipNutritionStore>() != null)
                {
                    parent.TryGetComp<CompShipNutritionStore>().SetId(this.bodyId);
                    ((MapCompBuildingTracker)parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(parent.TryGetComp<CompShipNutritionStore>());
                }
            }
            if (respawningAfterLoad)
            {
                foreach (Thing t in body.bodyParts)
                {
                    CompShipBodyPart bp = t.TryGetComp<CompShipBodyPart>();
                    if (bp != null)
                    {
                        foreach (Thing a in bp.adjMechs)
                        {
                            AggressionTarget(a, true);
                        }
                        bp.adjMechs.Clear();
                        foreach (Thing a in bp.adjBodypart)
                        {
                            AggressionTarget(a, false);
                        }
                        bp.adjBodypart.Clear();
                    }
                }
            }
        }

        public override string CompInspectStringExtra()
        {
            return String.Format("Heart of {0}", this.bodyName);
        }


        public override void DoHunger()
        {
            if (this.body.bodyParts.Count > 0)
            {
                this.body.bodyParts.RandomElement().TryGetComp<CompShipBodyPart>().Whither();
            }
            this.hungerDuration = 0;
        }

        public override string GetSpecies()
        {
            return HeartProps.shipspecies;
        }

        private DefOptions blankDefOptions = new DefOptions(new List<ThingDef>());

        public virtual ThingDef GetThingDef(string cat)
        {
            List<ThingDef> def = defs.TryGetValue(cat, blankDefOptions).defs;
            if (def != null)
            {
                return def.RandomElement();
            }
            return null;
        }

        public virtual void AggressionTarget(Thing target, bool mechanical)
        {
            if (target.TryGetComp<CompRegenSpot>() != null)
            {
                return;
            }
            if (mechanical)
            {
                aggression.adjacentMechanicals.Add(target);
            } else
            {
                aggression.otherFlesh.Add(target);
            }
        }

        public float GetDamageMult(DamageInfo dinfo)
        {
            if (resistances == null)
            {
                resistances = new Dictionary<DamageDef, float>();
            }
            return resistances.TryGetValue(dinfo.Def, 0f);
        }

        public void GainResistance(DamageInfo dinfo)
        {
            if (resistances == null)
            {
                resistances = new Dictionary<DamageDef, float>();
            }
            if (resistances.TryGetValue(dinfo.Def, 0f) < maxResistence)
            {
                if (!resistances.ContainsKey(dinfo.Def))
                {
                    resistances.Add(dinfo.Def, 0f);
                }
                resistances[dinfo.Def] += 0.0025f;
            }
        }

        public override float GetStat(string stat)
        {
            float ret = base.GetStat(stat);
            switch (stat)
            {
                case "regenEfficiency": 
                    return ret * GetStat("metabolicEfficiency");
                case "regenSpeed":
                    return ret * GetStat("metabolicSpeed");
                case "movementSpeed":
                    return ret * GetStat("conciousness");
                case "shieldStrength":
                    return ret * GetStat("conciousness");
                default:
                    return base.GetStat(stat);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}
            if (Prefs.DevMode)
	        {
				yield return new Command_Action
			    {
				    defaultLabel = "DEBUG: Whither All",
				    action = delegate()
				    {
                        List<Thing> toDestroy = new List<Thing>();
                        foreach(Thing bp in body.bodyParts)
                        {
                            toDestroy.Add(bp);
                        }
                        foreach(Thing bp in toDestroy)
                        {
                            bp.TryGetComp<CompShipBodyPart>().Whither(false);
                        }
				    }
			    };
                List<BuildingGeneDef> bodyDefs = new List<BuildingGeneDef>();
                foreach(BuildingGene gene in this.genes)
                {
                    bodyDefs.Add(gene.def);
                }
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Add Gene",
                    action = delegate()
                    {
                        List<FloatMenuOption> options = new List<FloatMenuOption>();
                        List<BuildingGeneDef> missingGenes = DefDatabase<BuildingGeneDef>.AllDefs.Where(g => !bodyDefs.Contains(g)).ToList();
                        foreach(BuildingGeneDef mg in missingGenes)
                        {
                            options.Add(new FloatMenuOption(
                                mg.label,
                                delegate
                                {
                                    BuildingGene toAdd = BuildingGeneMaker.MakeBuildingGene(mg);
                                    this.AddGene(toAdd);
                                }));
                        }
                        if (options.Count > 0)
                        {
                            FloatMenu menu = new FloatMenu(options);
                            Find.WindowStack.Add(menu);
                        }
                    }
                };
			}
		}

        public void ApplyHeartSeed(CompHeartSeed seed)
        {
            foreach (BuildingGene gene in this.genes.ToArray())
            {
                this.RemoveGene(gene);
            }
            ShipGeneline g = ShipGenelineMaker.MakeShipGeneline(this.geneline);
            this.AddGene(g.smallTurretGene);
            this.AddGene(g.mediumTurretGene);
            this.AddGene(g.largeTurretGene);
            this.AddGene(g.spinalTurretGene);
            this.AddGene(g.armor);
            foreach(BuildingGeneDef gd in seed.heartGenes)
            {
                this.AddGene(BuildingGeneMaker.MakeBuildingGene(gd, false));
            }
        }

        public override string ToString()
        {
            return this.bodyName;
        }
    }

    public class DefOptions : IExposable
    {
        public List<ThingDef> defs = new List<ThingDef>();

        public DefOptions()
        {

        }

        public DefOptions(List<ThingDef> _defs)
        {
            defs = _defs;
        }

        void IExposable.ExposeData() {
            Scribe_Collections.Look<ThingDef>(ref defs, "defs", LookMode.Def);
        }
    }

}
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
    

    public class CompShipHeart : CompBuildingCore
    {
        public CompProperties_ShipHeart HeartProps => (CompProperties_ShipHeart)props;

        StatDef injectors = StatDef.Named("LuciferInjectors");

        public bool luciferiumAddiction = false;
        public bool luciferiumSupplied = false;
        public CompRegenWorker regenWorker;
        public CompAggression aggression;
        public bool initialized = false;
        public float maxResistence = 0.15f;
        public ShipGeneline geneline = null;

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
            Scribe_Values.Look<bool>(ref luciferiumAddiction, "luciferiumAddiction", false);
            Scribe_Values.Look<bool>(ref luciferiumSupplied, "luciferiumSupplied", false);
            Scribe_Deep.Look<ShipGeneline>(ref geneline, "geneline", null);
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
            regenWorker = parent.TryGetComp<CompRegenWorker>();
            aggression = parent.TryGetComp<CompAggression>();
            base.PostSpawnSetup(respawningAfterLoad);
            regenWorker.body = this.body;
            if (!respawningAfterLoad && !initialized)
            {
                Log.Message("Creating geneline");
                geneline = ShipGenelineMaker.MakeShipGeneline(ShipGenelineDef.Named(HeartProps.geneline));
                Log.Message("Adding small turret");
                this.AddGene(geneline.smallTurretGene);
                Log.Message("Adding medium turret");
                this.AddGene(geneline.mediumTurretGene);
                Log.Message("Adding large turret");
                this.AddGene(geneline.largeTurretGene);
                Log.Message("Adding spinal turret");
                this.AddGene(geneline.spinalTurretGene);
                Log.Message("Adding armor");
                this.AddGene(geneline.armor);
                Log.Message("Adding misc");
                foreach (BuildingGene b in geneline.genes)
                {
                    this.AddGene(b);
                }
                initialized = true; //When the ship takes off the comps get regenerated.  This ensures that the initial mutations will only proc once.
            }
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
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (body != null)
            {
                List<Thing> toWhither = new List<Thing>();
                foreach (Thing t in body.bodyParts)
                {
                    toWhither.Add(t);
                }
                foreach (Thing t in toWhither)
                {
                    t.TryGetComp<CompShipBodyPart>().Whither(true);
                }
            }
            base.PostDestroy(mode, previousMap);
        }
        public override string CompInspectStringExtra()
        {
            return String.Format("Heart of of {0}", this.bodyName);
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
            float luciferMult = 1f;
            if (luciferiumAddiction && luciferiumSupplied)
            {
                luciferMult = 1.5f;
            }
            switch (stat)
            {
                case "metabolicEfficiency":
                    return ret * luciferMult;
                case "metabolicSpeed":
                    return ret * luciferMult;
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
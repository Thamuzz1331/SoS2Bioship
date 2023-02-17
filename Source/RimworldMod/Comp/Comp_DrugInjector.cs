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
    public class CompDrugInjector : CompFacility
    {
        private CompProperties_DrugInjector InjectorProps => (CompProperties_DrugInjector)props;

        CompRefuelable refuelable;
        CompFueledAddictionSupplier supplier;
        BuildingHediffDef drugDef;
        BuildingHediffDef addictionDef;
        int curTick = 0;


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            refuelable = parent.TryGetComp<CompRefuelable>();
            supplier = parent.TryGetComp<CompFueledAddictionSupplier>();
            drugDef = BuildingHediffDef.Named(InjectorProps.drugHediff);
            addictionDef = BuildingHediffDef.Named(InjectorProps.addictionHediff);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }

        public override void CompTick()
        {
            base.CompTick();
            if (curTick % 5 == 0)
            {
                curTick = 0;
                foreach (Thing heart in this.LinkedBuildings)
                {
                    CompShipHeart bp = heart.TryGetComp<CompShipHeart>();
                    bp.body.RegisterAddictionSupplier(supplier);
                }
            }
            curTick++;
        }

        public override void PostDeSpawn(Map map)
        {
            foreach (Thing heart in this.LinkedBuildings)
            {
                CompShipHeart bp = heart.TryGetComp<CompShipHeart>();
                bp.body.DeRegisterAddictionSupplier(supplier);
            }

            base.PostDeSpawn(map);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            foreach (Thing heart in this.LinkedBuildings)
            {
                CompShipHeart bp = heart.TryGetComp<CompShipHeart>();
                float requiredDosage = bp.body.bodyParts.Count * InjectorProps.massDosageMult;
                if (requiredDosage <= refuelable.Fuel)
                {
                    options.Add(new FloatMenuOption(bp.bodyName,
                        delegate ()
                        {
                            BuildingHediff_Drug drug = (BuildingHediff_Drug)BuildingHediffMaker.MakeBuildingHediff(drugDef);
                            drug.durationTicks = InjectorProps.drugDuration;
                            bp.AddHediff(drug);
                            refuelable.ConsumeFuel(requiredDosage);
                            if (Rand.Chance(InjectorProps.addictionChance))
                            {
                                Building_Addiction addiction =
                                    (Building_Addiction)BuildingHediffMaker.MakeBuildingHediff(addictionDef);
                                addiction.maxWithdrawl = InjectorProps.maxWithdrawl;
                                addiction.withdrawRate = InjectorProps.withdrawlRate;
                                addiction.massMult = InjectorProps.massAddictionMult;
                                bp.AddHediff(addiction);
                            }
                        },
                        MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
                }
            }
            if (options.Count > 0)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Inject " + drugDef.label + " into ",
                    action = delegate ()
                    {
                        FloatMenu menu = new FloatMenu(options);
                        Find.WindowStack.Add(menu);
                    }
                };
            }
            if (Prefs.DevMode)
            {
                List<FloatMenuOption> devDrugOptions = new List<FloatMenuOption>();
                List<FloatMenuOption> devAddictionOptions = new List<FloatMenuOption>();
                foreach (Thing heart in this.LinkedBuildings)
                {
                    CompShipHeart bp = heart.TryGetComp<CompShipHeart>();
                    devDrugOptions.Add(new FloatMenuOption(bp.bodyName,
                        delegate ()
                        {
                            BuildingHediff_Drug drug = (BuildingHediff_Drug)BuildingHediffMaker.MakeBuildingHediff(drugDef);
                            drug.durationTicks = InjectorProps.drugDuration;
                            bp.AddHediff(drug);
                        },
                        MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
                    devAddictionOptions.Add(new FloatMenuOption(bp.bodyName,
                        delegate ()
                        {
                            Building_Addiction addiction =
                                (Building_Addiction)BuildingHediffMaker.MakeBuildingHediff(addictionDef);
                            addiction.maxWithdrawl = InjectorProps.maxWithdrawl;
                            addiction.withdrawRate = InjectorProps.withdrawlRate;
                            addiction.massMult = InjectorProps.massAddictionMult;
                            bp.AddHediff(addiction);
                        },
                        MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));

                }
                yield return new Command_Action
                {
                    defaultLabel = "Dev: Apply drug " + drugDef.label + " to ",
                    action = delegate ()
                    {
                        FloatMenu menu = new FloatMenu(devDrugOptions);
                        Find.WindowStack.Add(menu);
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "Dev: Apply addiction " + drugDef.label + " to ",
                    action = delegate ()
                    {
                        FloatMenu menu = new FloatMenu(devAddictionOptions);
                        Find.WindowStack.Add(menu);
                    }
                };
            }
        }
    }
}

//

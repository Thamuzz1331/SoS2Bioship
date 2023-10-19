using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using Verse;
using RimWorld;
using LivingBuildings;

namespace Verse
{
    public class PsiSuppressionGene : BuildingGene
    {
        private const int COOLDOWN = 500;
        private int cooldownLeft = 0;
        private Thing core;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref cooldownLeft, "cooldownLeft", 0);
            Scribe_References.Look(ref core, "core", false);
        }

        public override void PostAdd(CompBuildingCore _core)
        {
            base.PostAdd(_core);
            this.core = _core.parent;
        }

        public override void PostRemove(CompBuildingCore core)
        {
            base.PostRemove(core);
        }
        public override void Tick()
        {
            base.Tick();


            if (cooldownLeft > 0)
            {
                cooldownLeft--;
            }
        }
        public override IEnumerable<Gizmo> GeneGetGizmosExtra()
        {
            Command_Action psiSupress = new Command_Action
            {
                action = delegate
                {
                    List<FloatMenuOption> options = new List<FloatMenuOption>();
                    foreach (Map map in Find.Maps)
                    {
                        if (!map.gameConditionManager.ConditionIsActive(GameConditionDefOf.PsychicSuppression))
                        {
                            FloatMenuOption op = new FloatMenuOption(map.Parent.Label, delegate { //Affects both genders, so we have to do this twice
                                GameCondition_PsychicEmanation suppress = (GameCondition_PsychicEmanation)GameConditionMaker.MakeCondition(GameConditionDefOf.PsychicSuppression, 60000);
                                suppress.conditionCauser = this.core;
                                suppress.gender = Gender.Female;
                                map.gameConditionManager.RegisterCondition(suppress);
                                suppress = (GameCondition_PsychicEmanation)GameConditionMaker.MakeCondition(GameConditionDefOf.PsychicSuppression, 60000);
                                suppress.conditionCauser = this.core;
                                suppress.gender = Gender.Male;
                                map.gameConditionManager.RegisterCondition(suppress);

                                cooldownLeft = COOLDOWN;
                            });
                            options.Add(op);
                        }
                    }
                    if (options.Count > 0)
                    {
                        FloatMenu menu = new FloatMenu(options);
                        Find.WindowStack.Add(menu);
                    }
                },
                icon = ContentFinder<Texture2D>.Get("UI/Commands/RenameZone"),
                defaultLabel = TranslatorFormattedStringExtensions.Translate("BodyRename"),
                defaultDesc = TranslatorFormattedStringExtensions.Translate("BeodyRenameDesc")
            };
            yield return psiSupress;

        }


    }
}

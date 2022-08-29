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
    public class CompRegenSpot : ThingComp
    {
        private CompProperties_RegenSpot Props => (CompProperties_RegenSpot)props;
        public ThingDef regenDef;
        public CompRegenWorker regenWorker;

        private float regenCountdown;
        private int lifetimeLeft = 100;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref regenCountdown, "regenCountdown", 0);
            Scribe_Values.Look(ref lifetimeLeft, "lifetimeLeft", 0);
            Scribe_Values.Look(ref regenDef, "regenDef", null);
            Scribe_Values.Look(ref regenWorker, "regenWorker", null);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (regenCountdown <= 0f)
            {
                if (lifetimeLeft <= 0 || regenWorker.parent.Destroyed || regenWorker.body == null || regenWorker.body.heart == null)
                {
                    parent.Destroy();
                    return;
                }
                lifetimeLeft--;
                foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(parent.Position))
                {
                    foreach (Thing adj in c.GetThingList(parent.Map))
                    {
                        if (adj.TryGetComp<CompShipBodyPart>() != null && 
                            adj.TryGetComp<CompShipBodyPart>().bodyId == this.regenWorker.body.heart.bodyId &&
                            regenWorker.body.RequestNutrition(regenWorker.GetRegenCost()))
                        {
                            Thing replacement = ThingMaker.MakeThing(regenDef);
                            CompShipBodyPart bodyPart = replacement.TryGetComp<CompShipBodyPart>();
                            if (bodyPart != null)
                            {
                                bodyPart.SetId(regenWorker.body.heart.bodyId);
                            }
                            CompNutrition nutrition = replacement.TryGetComp<CompNutrition>();
                            if (nutrition != null)
                            {
                                nutrition.SetId(regenWorker.body.heart.bodyId);
                            }
                            replacement.Rotation = parent.Rotation;
                            replacement.Position = parent.Position;
                            replacement.SetFaction(parent.Faction);
                            replacement.SpawnSetup(parent.Map, false);
                            parent.Destroy();
                            return;
                        }
                    }
                }
                regenCountdown = regenWorker.GetRegenInterval();
            }
            regenCountdown--;
        }
    }
}
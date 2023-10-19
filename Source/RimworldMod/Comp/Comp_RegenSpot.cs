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
    public class CompRegenSpot : ThingComp
    {
        private CompProperties_RegenSpot Props => (CompProperties_RegenSpot)props;
        public ThingDef regenDef;
        public Thing heart;
        public Dictionary<DamageDef, float> armorLevels = new Dictionary<DamageDef, float>();

        public float regenCountdown;
        private int lifetimeLeft = 100;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref regenCountdown, "regenCountdown", 0f);
            Scribe_Values.Look(ref lifetimeLeft, "lifetimeLeft", 100);
            Scribe_Defs.Look(ref regenDef, "regenDef");
            Scribe_References.Look(ref heart, "heart", false);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (regenCountdown <= 0f)
            {
                if (lifetimeLeft <= 0 || heart == null || heart.Destroyed)
                {
                    parent.Destroy();
                    return;
                }
                lifetimeLeft--;
                bool didRegen = false;
                List<CompRegenSpot> adjRegen = new List<CompRegenSpot>();
                Thing replacement = null;
                foreach (IntVec3 c in GenAdjFast.AdjacentCellsCardinal(parent.Position))
                {
                    foreach (Thing adj in c.GetThingList(parent.Map))
                    {
                        if (adj.TryGetComp<CompRegenSpot>() != null)
                        {
                            adjRegen.Add(adj.TryGetComp<CompRegenSpot>());
                        }
                        Log.Message("HID" + heart.TryGetComp<CompShipHeart>().bodyId);
                        Log.Message("BPID" + adj.TryGetComp<CompShipBodyPart>()?.bodyId);
                        if (!didRegen && 
                            adj.TryGetComp<CompShipBodyPart>() != null && 
                            adj.TryGetComp<CompShipBodyPart>().bodyId == heart.TryGetComp<CompShipHeart>().bodyId &&
                            heart.TryGetComp<CompShipHeart>().body.RequestNutrition(heart.TryGetComp<CompShipHeart>().regenWorker.GetRegenCost()))
                        {
                            Log.Message("Regenerating");
                            replacement = ThingMaker.MakeThing(regenDef);
                            CompShipBodyPart bodyPart = replacement.TryGetComp<CompShipBodyPart>();
                            if (bodyPart != null)
                            {
                                bodyPart.SetId(heart.TryGetComp<CompShipHeart>().bodyId);
                            }
                            CompNutrition nutrition = replacement.TryGetComp<CompNutrition>();
                            if (nutrition != null)
                            {
                                nutrition.SetId(heart.TryGetComp<CompShipHeart>().bodyId);
                            }
                            replacement.Rotation = parent.Rotation;
                            replacement.Position = parent.Position;
                            replacement.SetFaction(parent.Faction);
                            didRegen = true;
                        }
                    }
                }
                if (didRegen)
                {
                    foreach(CompRegenSpot adj in adjRegen)
                    {
                        adj.regenCountdown = heart.TryGetComp<CompShipHeart>().regenWorker.GetRegenInterval();
                    }
                    Log.Message("Spawning replacement");
                    replacement.SpawnSetup(parent.Map, false);
                    parent.Destroy();
                }
                regenCountdown += (heart.TryGetComp<CompShipHeart>().regenWorker.GetRegenInterval()/2);
            }
            regenCountdown--;
        }
    }
}
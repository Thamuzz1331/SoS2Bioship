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
    public class CompShipBodyPart : CompBuildingBodyPart
    {
        public CompProperties_ShipBodyPart ShipProps => (CompProperties_ShipBodyPart)props;
        public HashSet<Thing> adjMechs = new HashSet<Thing>();
        public HashSet<Thing> adjBodypart = new HashSet<Thing>();
        public float armorGrowthMult = 1f;

        public bool initialized = false;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref initialized, "initialized", false);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad && !initialized && body != null && body.heart != null)
            {
                initialized = true;
            }
            foreach (IntVec3 c in GenAdjFast.AdjacentCellsCardinal(parent.Position))
            {
                foreach (Thing adj in c.GetThingList(parent.Map))
                {
                    CompEatMe eatMe = adj.TryGetComp<CompEatMe>();
                    if (eatMe != null && adj.TryGetComp<CompScaffold>() == null)
                    {
                        CompBuildingBodyPart bodyPart = adj.TryGetComp<CompBuildingBodyPart>();
                        if (bodyPart != null)
                        {
                            if (bodyPart.bodyId != this.bodyId)
                            {
                                if (body.heart != null)
                                {
                                    ((CompShipHeart)body.heart).AggressionTarget(adj, false);
                                }
                                else
                                {
                                    adjBodypart.Add(adj);
                                }
                            }
                        }
                        else
                        {
                            if (body.heart != null)
                            {
                                ((CompShipHeart)body.heart).AggressionTarget(adj, true);
                            }
                            else
                            {
                                adjMechs.Add(adj);
                            }
                        }
                    }
                }
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (mode == DestroyMode.KillFinalize && ShipProps.regenDef != null)
            {
                Thing replacement = ThingMaker.MakeThing(ThingDef.Named("Wound"));
                CompRegenSpot regenDetails = replacement.TryGetComp<CompRegenSpot>();
                regenDetails.regenCountdown = ((CompShipHeart)body.heart).regenWorker.GetRegenInterval();
                regenDetails.regenDef = ThingDef.Named(ShipProps.regenDef);
                ThingDef heartDef = ((CompShipHeart)body.heart).GetThingDef(ShipProps.regenDef);
                if (heartDef != null)
                {
                    regenDetails.regenDef = heartDef;
                }
                regenDetails.heart = body.heart.parent;
                replacement.Position = parent.Position;
                replacement.Rotation = parent.Rotation;
                replacement.SetFaction(parent.Faction);
                replacement.SpawnSetup(previousMap, false);
            }
            base.PostDestroy(mode, previousMap);
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
            ((CompShipHeart)body.heart).regenWorker.RegisterWound((Building)parent);
            ((CompShipHeart)body.heart).GainResistance(dinfo);
        }

        public override string CompInspectStringExtra()
        {
            return "Flesh of " + body.GetName();
        }

        public bool IsArmor()
        {
            return ShipProps.isArmor;
        }

        public override void Detatch(DestroyMode mode, Map previousMap)
        {
            if (mode != DestroyMode.Vanish)
            {
                this.Whither(true);
            }
        }

        public void Whither(bool heartDeath = false)
        {
            if (this.ShipProps.isArmor)
            {
                this.parent.Destroy();
            } else if (!heartDeath) {
                if (parent is Building_Trap)
                {
                    foreach (IntVec3 c in GenAdjFast.AdjacentCellsCardinal(parent.Position))
                    {
                        foreach(Thing adj in c.GetThingList(parent.Map))
                        {
                            if (adj is Pawn)
                            {
                                ((Building_Trap)parent).Spring((Pawn)adj);
                                adj.SetPositionDirect(parent.Position);
                                return;
                            }
                        }
                    }
                } else
                {
                    if (Rand.Chance(0.3f))
                    {
                        List<Thing> onSpace = parent.Position.GetThingList(parent.Map);
                        if (onSpace.Count == 2) { 
                            foreach(Thing loc in onSpace)
                            {
                                if (loc is Pawn)
                                {
                                    Thing replacement = ThingMaker.MakeThing(ThingDef.Named("Maw_Small"));
                                    replacement.Rotation = parent.Rotation;
                                    replacement.Position = parent.Position;
                                    replacement.SetFaction(parent.Faction);
                                    replacement.SpawnSetup(parent.Map, false);
                                    ((Building_Trap)replacement).Spring((Pawn)loc);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            if (this.ShipProps.whitherTo != null)
            {
                Thing replacement = ThingMaker.MakeThing(ThingDef.Named(this.ShipProps.whitherTo));
                replacement.Rotation = parent.Rotation;
                replacement.Position = parent.Position;
                replacement.SetFaction(parent.Faction);
                if (replacement.TryGetComp<CompColorable>() != null)
                {
                    replacement.TryGetComp<CompColorable>().SetColor(Color.gray);
                }
                IntVec3 c = parent.Position;
                TerrainDef terrain = parent.Map.terrainGrid.TerrainAt(c);
                parent.Map.terrainGrid.RemoveTopLayer(c, false);
                Map m = parent.Map;
                replacement.SpawnSetup(m, false);
            }
            parent.Destroy(DestroyMode.Vanish);
        }

        public virtual float GetDamageMult(DamageInfo dinfo)
        {
            float res = ShipProps.baseArmor + ((CompShipHeart)body.heart).GetDamageMult(dinfo);
            return 1f - res;
        }
    }
}
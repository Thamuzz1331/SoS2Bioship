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
                foreach (IHediff mut in ((CompShipHeart)body.heart).mutator.mutations)
                {
                    if (mut.ShouldAddTo(this))
                    {
                        mut.Apply(this);
                        this.hediffs.Add(mut);
                    }
                }
                initialized = true;
            }
            foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(parent.Position))
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
                regenDetails.regenDef = ThingDef.Named(ShipProps.regenDef);
                regenDetails.regenWorker = ((CompShipHeart)body.heart).regenWorker;
                replacement.Position = parent.Position;
                replacement.Rotation = parent.Rotation;
                replacement.SetFaction(parent.Faction);
                replacement.SpawnSetup(previousMap, false);
            }
            base.PostDestroy(mode, previousMap);
        }

        public override string CompInspectStringExtra()
        {
            return "Flesh of " + body.GetName();
        }

        public bool IsArmor()
        {
            return ShipProps.isArmor;
        }

        public void Whither()
        {
            if (this.ShipProps.isArmor)
            {
                this.parent.Destroy();
            } else if 
            (this.ShipProps.whitherTo != null)
            {
                Thing replacement = ThingMaker.MakeThing(ThingDef.Named(this.ShipProps.whitherTo));
                replacement.Rotation = parent.Rotation;
                replacement.Position = parent.Position;
                replacement.SetFaction(parent.Faction);
                if (replacement.TryGetComp<CompColorable>() != null)
                {
                    replacement.TryGetComp<CompColorable>().SetColor(Color.white);
                }
                IntVec3 c = parent.Position;
                TerrainDef terrain = parent.Map.terrainGrid.TerrainAt(c);
                parent.Map.terrainGrid.RemoveTopLayer(c, false);
                Map m = parent.Map;
                parent.Destroy();
                replacement.SpawnSetup(m, false);
            }
        }
    }
}
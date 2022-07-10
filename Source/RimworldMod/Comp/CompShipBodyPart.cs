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
    public class CompShipBodyPart : ThingComp
    {
        CompProperties_ShipBodyPart Props => (CompProperties_ShipBodyPart)props;
        private List<Thing> scaffolds = new List<Thing>();
        public HashSet<Thing> adjMechs = new HashSet<Thing>();
        public HashSet<Thing> adjBodypart = new HashSet<Thing>();

        public String heartId = "NA";
        public ShipBody body = null;

        public void SetId(String _id)
        {
            heartId = _id;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look<String>(ref heartId, "heartId", "NA");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if(respawningAfterLoad)
            {
                ((ShipBodyMapComp)this.parent.Map.components.Where(t => t is ShipBodyMapComp).FirstOrDefault()).Register(this);
            }
            foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(parent.Position))
            {
                foreach (Thing adj in c.GetThingList(parent.Map))
                {
                    if (adj is ThingWithComps)
                    {
                        CompScaffold scaff = ((ThingWithComps)adj).TryGetComp<CompScaffold>();
                        if (scaff != null)
                        {
                            AddScaff(scaff.parent);
                            return;
                        }
                        CompEatMe eatMe = ((ThingWithComps)adj).TryGetComp<CompEatMe>();
                        if (eatMe != null)
                        {
                            CompShipBodyPart bodyPart = ((ThingWithComps)adj).TryGetComp<CompShipBodyPart>();
                            if (bodyPart != null)
                            {
                                if (bodyPart.heartId != heartId)
                                {
                                    body.otherFlesh.Add(adj);
                                }
                            }
                            else
                            {
                                body.adjacentMechanicals.Add(adj);
                            }
                        }
                    }
                }
            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            if (body != null)
                body.DeRegister(this);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (mode == DestroyMode.KillFinalize && Props.regen)
            {
                body.Regen(this);
            }
            body.DeRegister(this);
        }

        public override string CompInspectStringExtra()
        {
            return "Flesh of " + heartId;
        }

        public void AddScaff(Thing scaff)
        {
            if (this.body != null && this.body.heart != null)
            {
                CompShipfleshConversion converter = body.heart.TryGetComp<CompShipfleshConversion>();
                if (converter != null)
                {
                    converter.toConvert.Enqueue(scaff);
                    return;
                }
            }
            scaffolds.Add(scaff);
        }
        public List<Thing> GetScaff()
        {
            return scaffolds;
        }
        public void ClearScaff()
        {
            scaffolds.Clear();
        }
        public bool IsArmor()
        {
            return Props.isArmor;
        }
    }
}
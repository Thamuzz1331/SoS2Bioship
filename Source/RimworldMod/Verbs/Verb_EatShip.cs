using RimWorld;
using SaveOurShip2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class Command_VerbEatShip : Command
    {
        public Building salvageMaw;
        public int salvageMawNum;
        public Map sourceMap;
        public Map targetMap;
        public IntVec3 position;

        public override void ProcessInput(Event ev)
        {
            Building b = null;
            base.ProcessInput(ev);
            if (sourceMap != targetMap)
                CameraJumper.TryJump(targetMap.Center, targetMap);
            Targeter targeter = Find.Targeter;
            TargetingParameters parms = new TargetingParameters();
            parms.canTargetBuildings = true;
            Find.Targeter.BeginTargeting(parms, (Action<LocalTargetInfo>)delegate (LocalTargetInfo x)
            {
                b = x.Cell.GetFirstBuilding(targetMap);
            }, (Pawn)null, delegate { AfterTarget(b); });
        }


        public void AfterTarget(Building b)
        {
            if (b == null)
				return;
            var sourceMapComp = salvageMaw.Map.GetComponent<ShipMapComp>();
			var mapComp = b.Map.GetComponent<ShipMapComp>();
            int shipIndex = mapComp.ShipIndexOnVec(b.Position);
			var ship = mapComp.ShipsOnMap[shipIndex];
            List<CompRefuelable> maws = new List<CompRefuelable>();
            foreach(Thing maw in sourceMap.listerThings.ThingsOfDef(ThingDef.Named("SalvageMaw")))
            {
                CompRefuelable refuelable = ((Building)maw).TryGetComp<CompRefuelable>();
                if (refuelable.FuelPercentOfMax < 1)
                {
                    maws.Add(refuelable);
                }
            }
            List<Building> toRemove = new List<Building>();
            foreach (Building w in ship.Buildings)
            {
                if (!w.Destroyed)
                {
                    if (w.def.costList != null)
                    {
                        foreach (ThingDefCountClass bcomp in w.def.costList)
                        {
                            List<CompRefuelable> full = new List<CompRefuelable>();
                            if (maws.Count > 0)
                            {
                                float fuel = bcomp.count / (maws.Count * 2);
                                foreach (CompRefuelable m in maws)
                                {
                                    m.Refuel(fuel);
                                    if (m.FuelPercentOfTarget >= 1)
                                    {
                                        full.Add(m);
                                    }
                                }
                                foreach (CompRefuelable f in full)
                                {
                                    maws.Remove(f);
                                }
                            }
                        }
                    }
                    toRemove.Add(w);
                }
            }
            foreach (Building w in toRemove)
            {
                w.Destroy(DestroyMode.Vanish);
            }


            mapComp.RemoveShipFromCache(shipIndex);
        }
    }
}

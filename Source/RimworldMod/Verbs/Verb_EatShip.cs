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
    public class Command_VerbEatShip : Command
    {
        public Building salvageMaw;
        public int salvageMawNum;
        public Map sourceMap;
        public Map targetMap;

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
            List<Building> wreck = ShipInteriorMod2.FindBuildingsAttached(b);
            List<CompRefuelable> maws = new List<CompRefuelable>();
            foreach(Thing maw in salvageMaw.Map.listerThings.ThingsOfDef(ThingDef.Named("SalvageMaw")))
            {
                CompRefuelable refuelable = ((Building)maw).TryGetComp<CompRefuelable>();
                if (refuelable.FuelPercentOfMax < 1)
                {
                    maws.Add(refuelable);
                }
            }
            foreach (Building w in wreck)
            {
                if (w.def.costList != null)
                {
                    foreach(ThingDefCountClass bcomp in w.def.costList)
                    {
                        List<CompRefuelable> full = new List<CompRefuelable>();
                        float fuel = bcomp.count / maws.Count;
                        foreach(CompRefuelable m in maws)
                        {
                            m.Refuel(fuel);
                            if(m.FuelPercentOfTarget >= 1)
                            {
                                full.Add(m);
                            }
                        }
                        foreach(CompRefuelable f in full)
                        {
                            maws.Remove(f);
                        }
                    }
                }
                w.Destroy(DestroyMode.Vanish);
            }
        }
    }
}

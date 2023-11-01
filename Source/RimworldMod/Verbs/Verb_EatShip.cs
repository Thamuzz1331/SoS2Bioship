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
            List<Building> wreck = ShipInteriorMod2.FindBuildingsAttached(b);
            List<CompRefuelable> maws = new List<CompRefuelable>();
            foreach(Thing maw in sourceMap.listerThings.ThingsOfDef(ThingDef.Named("SalvageMaw")))
            {
                CompRefuelable refuelable = ((Building)maw).TryGetComp<CompRefuelable>();
                if (refuelable.FuelPercentOfMax < 1)
                {
                    maws.Add(refuelable);
                }
            }
            foreach (Building w in wreck)
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
                                float fuel = bcomp.count / (maws.Count*5);
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
                }
            }
            List<IntVec3> positions = ShipInteriorMod2.FindAreaAttached(b, true).ToList();
            if (positions.Contains(position) || positions.NullOrEmpty())
                return;
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ShipSalvageAbandonConfirm", delegate
            {
                ShipInteriorMod2.RemoveShip(positions, targetMap, false);
            }));

        }
    }
}

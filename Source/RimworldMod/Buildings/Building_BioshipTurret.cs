using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class Building_BioShipTurret : Building_ShipTurret
    {
        private static Type shipCombatManagerType = AccessTools.TypeByName("ShipCombatManager");

        private bool CanExtractTorpedo
        {
            get
            {
                return false;
            }
        }

        public Building_BioShipTurret()
        {
            base();
        }

        public override void OrderAttack(LocalTargetInfo targ)
        {
            if (forcedTarget != targ)
            {
                forcedTarget = targ;
                if (burstCooldownTicksLeft <= 0)
                {
                    TryStartShootSomethingBioShip(canBeginBurstImmediately: false);
                }
            }
            if (holdFire)
            {
                Messages.Message(TranslatorFormattedStringExtensions.Translate("MessageTurretWontFireBecauseHoldFire", def.label), this, MessageTypeDefOf.RejectInput, historical: false);
            }
            if (PointDefenseMode)
            {
                Messages.Message(TranslatorFormattedStringExtensions.Translate("MessageTurretWontFireBecausePointDefense", def.label), this, MessageTypeDefOf.RejectInput, historical: false);
            }
        }

        public override void Tick()
        {
            if (selected && !Find.Selector.IsSelected(this))
            {
                selected = false;
            }
            if (!CanToggleHoldFire)
            {
                holdFire = false;
            }
            bool inCombat = Traverse.Create(shipCombatManagerType).Field("InCombat").GetValue<bool>();
            if (forcedTarget.ThingDestroyed || !inCombat || (this.TryGetComp<CompSpinalMount>() != null && AmplifierCount == -1))
            {
                ResetForcedTarget();
            }
            if (Active && base.Spawned)
            {
                GunCompEq.verbTracker.VerbsTick();
                if ((PointDefenseMode || (!PlayerControlled && GetComp<CompShipHeatSource>().Props.pointDefense)) && inCombat)
                {
                    if (burstCooldownTicksLeft > 0)
                    {
                        burstCooldownTicksLeft--;
                    }
                    if (burstCooldownTicksLeft <= 0 && ShipCombatManager.IncomingTorpedoesInRange(this.Map))
                        BeginBurstBio();
                }
                if (stunner.Stunned || AttackVerb.state == VerbState.Bursting)
                {
                    return;
                }
                if (WarmingUp && !PointDefenseMode)
                {
                    burstWarmupTicksLeft--;
                    if (burstWarmupTicksLeft == 0)
                    {
                        BeginBurstBio();
                    }
                }
                else if (!PointDefenseMode)
                {
                    if (burstCooldownTicksLeft > 0)
                    {
                        burstCooldownTicksLeft--;
                    }
                    if (burstCooldownTicksLeft <= 0 && this.IsHashIntervalTick(10))
                    {
                        TryStartShootSomething(canBeginBurstImmediately: true);
                    }
                }
                if (Active)
                    top.TurretTopTick();
            }
            else
            {
                ResetCurrentTarget();
            }
            base.Tick();
        }

        protected void TryStartShootSomethingBioShip(bool canBeginBurstImmediately)
        {
            if (this.TryGetComp<CompSpinalMount>() != null)
                RecalcStats();
            if (!base.Spawned || (holdFire && CanToggleHoldFire) || !AttackVerb.Available() || PointDefenseMode || !ShipCombatManager.InCombat || (this.TryGetComp<CompSpinalMount>() != null && AmplifierCount == -1))
            {
                ResetCurrentTarget();
                return;
            }

            if (!this.PlayerControlled && this.Map == ShipCombatManager.EnemyShip)
            {
                if (this.TryGetComp<CompSpinalMount>() == null || this.TryGetComp<CompSpinalMount>().Props.destroysHull || ShipCombatManager.PlayerShip.mapPawns.FreeColonistsAndPrisoners.Count == 0)
                    shipTarget = ShipCombatManager.PlayerShip.listerThings.AllThings.RandomElement();
                else //Target pawn with the Psychic Flayer
                    shipTarget = ShipCombatManager.PlayerShip.mapPawns.FreeColonistsAndPrisoners.RandomElement();
            }

            bool isValid = currentTargetInt.IsValid;
            if (shipTarget.IsValid)
            {
                //fire same as engine direction or opposite if retreating
                int rotA;
                int rotB;
                if (this.PlayerControlled)
                {
                    rotA = ShipCombatManager.playerEngineRot;
                    rotB = ShipCombatManager.PlayerHeading;
                }
                else
                {
                    rotA = ShipCombatManager.enemyEngineRot;
                    rotB = ShipCombatManager.EnemyHeading;
                }
                if ((rotA == 0 && rotB != -1) || (rotA == 2 && rotB != 1))
                    currentTargetInt = new LocalTargetInfo(new IntVec3(Rand.RangeInclusive(this.Position.x - 5, this.Position.x + 5), 0, this.Map.Size.z - 1));
                else if ((rotA == 1 && rotB != -1) || (rotA == 3 && rotB != 1))
                    currentTargetInt = new LocalTargetInfo(new IntVec3(this.Map.Size.x - 1, 0, Rand.RangeInclusive(this.Position.z - 5, this.Position.z + 5)));
                else if ((rotA == 2 && rotB != -1) || (rotA == 0 && rotB != 1))
                    currentTargetInt = new LocalTargetInfo(new IntVec3(Rand.RangeInclusive(this.Position.x - 5, this.Position.x + 5), 0, 0));
                else
                    currentTargetInt = new LocalTargetInfo(new IntVec3(0, 0, Rand.RangeInclusive(this.Position.z - 5, this.Position.z + 5)));
            }
            else
            {
                currentTargetInt = TryFindNewTarget();
            }
            if (!isValid && currentTargetInt.IsValid)
            {
                SoundDefOf.TurretAcquireTarget.PlayOneShot(new TargetInfo(base.Position, base.Map));
            }
            if (currentTargetInt.IsValid)
            {
                if (def.building.turretBurstWarmupTime > 0f)
                {
                    burstWarmupTicksLeft = def.building.turretBurstWarmupTime.SecondsToTicks();
                }
                else if (canBeginBurstImmediately)
                {
                    BeginBurstBio();
                }
                else
                {
                    burstWarmupTicksLeft = 1;
                }
            }
            else
            {
                ResetCurrentTarget();
            }
        }

        protected void BeginBurstBio()
        {
            bool incomingTorps = Traverse.Create(shipCombatManagerType).Method("IncomingTorpedoesInRange").GetValue<bool>(this.Map);

            //PD mode
            bool isPointDefBurst = (PointDefenseMode || (!PlayerControlled && this.TryGetComp<CompShipHeatSource>().Props.pointDefense)) && incomingTorps && Find.TickManager.TicksGame > lastPDTick + 15;

            if (isPointDefBurst || shipTarget == null)
                this.shipTarget = LocalTargetInfo.Invalid;
            //sync
            ((Verb_LaunchProjectileShip)AttackVerb).shipTarget = shipTarget;
            if (this.AttackVerb.verbProps.burstShotCount > 0)
            {
                Map playerMap = Traverse.Create(shipCombatManagerType).Field("PlayerShip").GetValue<Map>();
                if (this.Map == playerMap)
                    SynchronizedBurstLocation = ShipCombatManager.FindClosestEdgeCell(ShipCombatManager.EnemyShip, shipTarget.Cell);
                else
                    SynchronizedBurstLocation = ShipCombatManager.FindClosestEdgeCell(ShipCombatManager.PlayerShip, shipTarget.Cell);
            }
            else
                SynchronizedBurstLocation = IntVec3.Invalid;
            //check if we have power to fire
            if (this.TryGetComp<CompPower>() != null && this.TryGetComp<CompShipHeatSource>() != null && this.TryGetComp<CompPower>().PowerNet.CurrentStoredEnergy() < this.TryGetComp<CompShipHeatSource>().Props.energyToFire * (1 + (AmplifierDamageBonus)))
            {
                //Messages.Message(TranslatorFormattedStringExtensions.Translate("CannotFireDueToPower",this.Label), this, MessageTypeDefOf.CautionInput);
                //this.shipTarget = LocalTargetInfo.Invalid;
                return;
            }
            //check if not PD and we are in range
            if (!isPointDefBurst && (200 - this.TryGetComp<CompShipHeatSource>().Props.maxRange > ShipCombatManager.Range))
            {
                return;
            }
            //spinal weapons fire straight
            if (this.TryGetComp<CompSpinalMount>() != null)
            {
                if (this.Rotation.AsByte == 0)
                    currentTargetInt = new LocalTargetInfo(new IntVec3(this.Position.x, 0, this.Map.Size.z - 1));
                else if (this.Rotation.AsByte == 1)
                    currentTargetInt = new LocalTargetInfo(new IntVec3(this.Map.Size.x - 1, 0, this.Position.z));
                else if (this.Rotation.AsByte == 2)
                    currentTargetInt = new LocalTargetInfo(new IntVec3(this.Position.x, 0, 1));
                else
                    currentTargetInt = new LocalTargetInfo(new IntVec3(1, 0, this.Position.z));
            }
            //if we do not have enough heatcap, vent heat to room/fail to fire in vacuum
            if (this.TryGetComp<CompShipHeatSource>() != null && this.TryGetComp<CompShipHeatSource>().AvailableCapacityInNetwork() < this.TryGetComp<CompShipHeatSource>().Props.heatPerPulse * (1 + (AmplifierDamageBonus)))
            {
                if (this.GetRoom() == null || this.GetRoom().OpenRoofCount > 0 || this.GetRoom().TouchesMapEdge)
                {
                    if (!PointDefenseMode && PlayerControlled)
                        Messages.Message(TranslatorFormattedStringExtensions.Translate("CannotFireDueToHeat", this.Label), this, MessageTypeDefOf.CautionInput);
                    this.shipTarget = LocalTargetInfo.Invalid;
                    return;
                }
                GenTemperature.PushHeat(this, this.TryGetComp<CompShipHeatSource>().Props.heatPerPulse * ShipCombatManager.HeatPushMult * (1 + (AmplifierDamageBonus)));
            }
            else
                this.TryGetComp<CompShipHeatSource>().AddHeatToNetwork(this.TryGetComp<CompShipHeatSource>().Props.heatPerPulse * (1 + (AmplifierDamageBonus)));
            //draw the same percentage from each cap: needed*current/currenttotal

            if (this.TryGetComp<CompRefuelable>() != null)
            {
                if (this.TryGetComp<CompRefuelable>().Fuel <= 0)
                {
                    if (PlayerControlled)
                        Messages.Message(TranslatorFormattedStringExtensions.Translate("CannotFireDueToAmmo", this.Label), this, MessageTypeDefOf.CautionInput);
                    this.shipTarget = LocalTargetInfo.Invalid;
                    return;
                }
                this.TryGetComp<CompRefuelable>().ConsumeFuel(1);
            }

            foreach (CompPowerBattery bat in this.TryGetComp<CompPower>().PowerNet.batteryComps)
            {
                bat.DrawPower(Mathf.Min(this.TryGetComp<CompShipHeatSource>().Props.energyToFire * (1 + AmplifierDamageBonus) * bat.StoredEnergy / this.TryGetComp<CompPower>().PowerNet.CurrentStoredEnergy(), bat.StoredEnergy));
            }

            if (GetComp<CompShipHeat>().Props.singleFireSound != null)
            {
                GetComp<CompShipHeat>().Props.singleFireSound.PlayOneShot(this);
            }
            if (PointDefenseMode)
                lastPDTick = Find.TickManager.TicksGame;
            AttackVerb.TryStartCastOn(currentTargetInt);
            OnAttackedTarget(currentTargetInt);
            burstCooldownTicksLeft = BurstCooldownTime().SecondsToTicks(); //Seems to prevent the "turbo railgun" bug. Don't ask me why.
            if (this.TryGetComp<CompSpinalMount>() != null && this.TryGetComp<CompSpinalMount>().Props.destroysHull)
            {
                List<Thing> thingsToDestroy = new List<Thing>();

                if (this.Rotation.AsByte == 0)
                {
                    for (int x = Position.x - 1; x <= Position.x + 1; x++)
                    {
                        for (int z = Position.z + 3; z < Map.Size.z; z++)
                        {
                            IntVec3 vec = new IntVec3(x, 0, z);
                            foreach (Thing thing in vec.GetThingList(Map))
                            {
                                thingsToDestroy.Add(thing);
                            }
                        }
                    }
                }
                else if (this.Rotation.AsByte == 1)
                {
                    for (int x = Position.x + 3; x < Map.Size.x; x++)
                    {
                        for (int z = Position.z - 1; z <= Position.z + 1; z++)
                        {
                            IntVec3 vec = new IntVec3(x, 0, z);
                            foreach (Thing thing in vec.GetThingList(Map))
                            {
                                thingsToDestroy.Add(thing);
                            }
                        }
                    }
                }
                else if (this.Rotation.AsByte == 2)
                {
                    for (int x = Position.x - 1; x <= Position.x + 1; x++)
                    {
                        for (int z = Position.z - 3; z > 0; z--)
                        {
                            IntVec3 vec = new IntVec3(x, 0, z);
                            foreach (Thing thing in vec.GetThingList(Map))
                            {
                                thingsToDestroy.Add(thing);
                            }
                        }
                    }
                }
                else
                {
                    for (int x = 1; x <= Position.x - 3; x++)
                    {
                        for (int z = Position.z - 1; z <= Position.z + 1; z++)
                        {
                            IntVec3 vec = new IntVec3(x, 0, z);
                            foreach (Thing thing in vec.GetThingList(Map))
                            {
                                thingsToDestroy.Add(thing);
                            }
                        }
                    }
                }

                foreach (Thing thing in thingsToDestroy)
                {
                    GenExplosion.DoExplosion(thing.Position, thing.Map, 0.5f, DamageDefOf.Bomb, null);
                    if (!thing.Destroyed)
                        thing.Kill();
                }
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (!selected)
            {
                RecalcStats();
                selected = true;
            }
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                if (gizmo.defaultLabel != TranslatorFormattedStringExtensions.Translate("CommandExtractShipTorpedo"))
                {
                    yield return gizmo;
                }
            }
        }

    }
}
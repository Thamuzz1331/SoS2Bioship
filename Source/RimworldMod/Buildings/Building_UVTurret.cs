using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using SaveOurShip2;
using RimWorld.Planet;
using HarmonyLib;
//using RimworldMod;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class Building_UVTurret : Building_BioShipTurret
    {
        private bool wasFiring = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref wasFiring, "wasFiring", false);
        }

        protected override void Tick()
        {
            base.Tick();
            if (AttackVerb.state == VerbState.Bursting)
            {
                wasFiring = true;
            } else if (wasFiring)
            {
                wasFiring = false;
                
                this.TakeDamage();
            }
        }

        public virtual void TakeDamage()
        {
            this.TakeDamage(new DamageInfo(ShipDamageDefOf.UVEyeLaser, 200, 1.0f, -1f, this));
        }

    }
}
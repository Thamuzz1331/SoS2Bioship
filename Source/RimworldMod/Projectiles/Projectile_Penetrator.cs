using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class Projectile_Penetrator : Projectile_ExplosiveShipCombat
    {
        //TODO: Fix this
        /*
        protected override void Impact(Thing hitThing)
        {
            Projectile secondaryProjectile = (Projectile)GenSpawn.Spawn(ThingDef.Named("Bullet_Spine_Penetrator_StageTwo"), this.Position, this.Map);
            secondaryProjectile.Launch(this.Launcher,
                this.ExactPosition,
                this.DestinationCell,
                this.DestinationCell,
                ProjectileHitFlags.All,
                equipment: this.Launcher);
            base.Impact(hitThing);
        }
        */
    }
}

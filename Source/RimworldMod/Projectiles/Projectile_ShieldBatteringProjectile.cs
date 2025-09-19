using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using SaveOurShip2;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class Projectile_ShieldBatteringProjectile : Projectile_ExplosiveShip
    {
        protected override void Tick()
        {
            Map m = this.Map;
            base.Tick();
            
            foreach (CompShipHeatShield shield in this.Map.GetComponent<ShipMapComp>().Shields)            
            {
                if (!shield.shutDown && Position.DistanceTo(shield.parent.Position) <= shield.radius)
                {
                    if (shield.radius >= 15f)
                    {
                        shield.radius -= 0.2f;
                    }
                    break;
                }
            }
        }
    }
}

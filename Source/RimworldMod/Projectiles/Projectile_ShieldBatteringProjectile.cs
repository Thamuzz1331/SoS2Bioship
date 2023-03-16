using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class Projectile_ShieldBatteringProjectile : Projectile_ExplosiveShipCombat
    {
        public override void Tick()
        {
            Map m = this.Map;
            base.Tick();
            
            foreach (CompShipCombatShield shield in this.Map.GetComponent<ShipHeatMapComp>().Shields)            
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

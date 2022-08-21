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
            
            if (this.Destroyed && CompShipCombatShield.allShieldsOnMap.ContainsKey(m))
            {
                foreach (CompShipCombatShield shield in CompShipCombatShield.allShieldsOnMap[m])
                {
                    if (!shield.shutDown && Position.DistanceTo(shield.parent.Position) <= shield.radius)
                    {
                        if (shield.radius >= 15f)
                        {
                            Log.Message("Shrinking shield");
                            shield.radius -= 0.2f;
                        }
                        break;
                    }
                }
            }
        }
    }
}

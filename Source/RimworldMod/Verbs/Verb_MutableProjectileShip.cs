using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld.Planet;
using HarmonyLib;
using SaveOurShip2;
//using RimworldMod;

namespace RimWorld
{
    public class Verb_MutableProjectileShip : SaveOurShip2.Verb_LaunchProjectileShip
    {
        public override ThingDef Projectile
        {
            get
            {
                if (base.EquipmentSource != null)
                {
                    CompMutableAmmo mutableAmmo = base.EquipmentSource.TryGetComp<CompMutableAmmo>();
                    if (mutableAmmo != null)
                    {
                        return mutableAmmo.GetProjectileDef();
                    }
                }
                return base.Projectile;
            }
        }
    }
}

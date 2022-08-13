using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class CompMutableAmmo : ThingComp
    {
        CompProperties_MutableAmmo Props => (CompProperties_MutableAmmo)props;
        public ThingDef projectileDef = null;
        public ThingDef fakeProjectileDef = null;

        public virtual ThingDef GetProjectileDef()
        {
            if (projectileDef != null)
            {
                return projectileDef;
            } else
            {
                return ThingDef.Named(Props.defaultAmmo);
            }
        }

        public virtual ThingDef GetFakeProjectileDef()
        {
            if (projectileDef != null)
            {
                return fakeProjectileDef;
            }
            else
            {
                return ThingDef.Named(Props.defaultFakeAmmo);
            }
        }
    }
}
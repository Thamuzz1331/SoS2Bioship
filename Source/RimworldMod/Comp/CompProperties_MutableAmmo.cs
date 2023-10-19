using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompProperties_MutableAmmo : CompProperties
    {
        public string defaultAmmo;
        public string defaultFakeAmmo;
        public string defaultAmmoName;
        public CompProperties_MutableAmmo()
        {
            compClass = typeof(CompMutableAmmo);
        }
    }

}

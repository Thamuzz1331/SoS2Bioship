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
    public class AmmoOption : IExposable
    {
        public ThingDef realProj;
        public ThingDef fakeProj;

        public AmmoOption()
        {

        }

        public AmmoOption(ThingDef _realProj, ThingDef _fakeProj)
        {
            realProj = _realProj;
            fakeProj = _fakeProj;
        }

        void IExposable.ExposeData()
        {
            Scribe_Defs.Look(ref realProj, "realProj");
            Scribe_Defs.Look(ref fakeProj, "fakeProj");
        }
    }

    public class CompMutableAmmo : ThingComp
    {
        CompProperties_MutableAmmo Props => (CompProperties_MutableAmmo)props;
        public Dictionary<String, AmmoOption> ammoTypes = new Dictionary<String, AmmoOption>();
        public string currentlySelected = "NA";
        public string setSelected = "NA";

        public override void PostSpawnSetup(bool b)
        {
            base.PostSpawnSetup(b);
            if (!b)
            {
                ammoTypes.Add(Props.defaultAmmoName, new AmmoOption(ThingDef.Named(Props.defaultAmmo), ThingDef.Named(Props.defaultFakeAmmo)));
                currentlySelected = Props.defaultAmmoName;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref setSelected, "setSelected", "NA");
            Scribe_Values.Look(ref currentlySelected, "currentlySelected", "NA");
            Scribe_Collections.Look<string, AmmoOption>(ref ammoTypes, "ammoTypes", LookMode.Value, LookMode.Deep);
        }

        public virtual ThingDef GetProjectileDef()
        {
            if (setSelected != "NA")
            {
                return ammoTypes.TryGetValue(setSelected, ammoTypes[Props.defaultAmmo]).realProj;
            } 
            return ammoTypes.TryGetValue(currentlySelected, ammoTypes[Props.defaultAmmo]).realProj;
        }

        public virtual ThingDef GetFakeProjectileDef()
        {
            if (setSelected != "NA")
            {
                return ammoTypes.TryGetValue(setSelected, ammoTypes[Props.defaultAmmo]).fakeProj;
            } 
            return ammoTypes.TryGetValue(currentlySelected, ammoTypes[Props.defaultAmmo]).fakeProj;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}
			if (this.ammoTypes.Count > 1)
			{
                string label = currentlySelected;
                if (setSelected != "NA")
                {
                    label = setSelected;
                }
                List<FloatMenuOption> options = new List<FloatMenuOption>();
				foreach(String ammoType in ammoTypes.Keys)
                {
					options.Add(new FloatMenuOption(ammoType,
						delegate() {
							this.setSelected = ammoType;
						},
						MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
                }

				yield return new Command_Action
				{
					defaultLabel = label,
					action = delegate ()
					{
						FloatMenu menu = new FloatMenu(options);
						Find.WindowStack.Add(menu);
					}
				};
			}
		}
    }
}
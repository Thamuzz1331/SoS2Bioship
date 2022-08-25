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
        public Dictionary<String, Tuple<ThingDef, ThingDef>> ammoTypes = new Dictionary<String, Tuple<ThingDef, ThingDef>>();
        public string currentlySelected = "NA";
        public string setSelected = "NA";

        public override void PostSpawnSetup(bool b)
        {
            base.PostSpawnSetup(b);
            ammoTypes.Add(Props.defaultAmmoName, new Tuple<ThingDef, ThingDef>(ThingDef.Named(Props.defaultAmmo), ThingDef.Named(Props.defaultFakeAmmo)));
            currentlySelected = Props.defaultAmmoName;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref setSelected, "setSelected", "NA");
        }

        public virtual ThingDef GetProjectileDef()
        {
            if (setSelected != "NA")
            {
                return ammoTypes[setSelected].Item1;
            }
            return ammoTypes[currentlySelected].Item1;
        }

        public virtual ThingDef GetFakeProjectileDef()
        {
            if (setSelected != "NA")
            {
                return ammoTypes[setSelected].Item2;
            }
            return ammoTypes[currentlySelected].Item2;
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
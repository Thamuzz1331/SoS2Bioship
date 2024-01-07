using SaveOurShip2;
using BioShip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using LivingBuildings;


namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompSalvageMaw : CompShipSalvageBay
    {
        private new CompProperties_SalvageMaw Props => (CompProperties_SalvageMaw)props;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            var mapComp = this.parent.Map.GetComponent<ShipHeatMapComp>();

            foreach (Gizmo item in base.CompGetGizmosExtra())
            {
                yield return item;
            }
            if (parent.Faction == Faction.OfPlayer && 
                this.parent.Map.Parent.def == SaveOurShip2.ResourceBank.WorldObjectDefOf.ShipOrbiting || 
                (Prefs.DevMode && ModLister.HasActiveModWithName("Save Our Ship Creation Kit")))
            {
                List<Map> salvagableMaps = new List<Map>();
                foreach (Map map in Find.Maps)
                {
                    if (map.GetComponent<ShipHeatMapComp>().IsGraveyard)
                        salvagableMaps.Add(map);
                }
                foreach (Map map in salvagableMaps)
                {
                    Command_VerbEatShip eatShipEnemy = new Command_VerbEatShip
                    {
                        salvageMaw = (Building)this.parent,
                        sourceMap = this.parent.Map,
                        targetMap = map,
                        icon = ContentFinder<Texture2D>.Get("UI/SalvageShip"),
                        defaultLabel = TranslatorFormattedStringExtensions.Translate("ShipDevourWreckCommand") + " (" + map + ")",
                        defaultDesc = TranslatorFormattedStringExtensions.Translate("ShipDevourWreckCommandDesc") + map
                    };
                    if (mapComp.InCombat)
                        eatShipEnemy.Disable(TranslatorFormattedStringExtensions.Translate("ShipDevourWreckDisabled"));
                    yield return eatShipEnemy;
                }
            }
        }

    }
}
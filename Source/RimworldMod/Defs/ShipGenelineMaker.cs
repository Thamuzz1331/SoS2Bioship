using System;
using LivingBuildings;
using RimWorld;

namespace Verse
{
	public static class ShipGenelineMaker
	{
		public static ShipGeneline MakeShipGeneline(ShipGenelineDef def)
		{
			ShipGeneline geneline = (ShipGeneline)Activator.CreateInstance(def.shipGenelineClass);
			geneline.def = def;


			geneline.smallTurretGene = BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(def.smallTurret));
			geneline.mediumTurretGene = BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(def.mediumTurret));
			geneline.largeTurretGene = BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(def.largeTurret));
			geneline.spinalTurretGene = BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(def.spinalTurret));
			geneline.armor = BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(def.armor));

			foreach(String geneName in def.miscGenes) {
				geneline.genes.Add(BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(geneName)));
			}

			return geneline;
		}
	}
}
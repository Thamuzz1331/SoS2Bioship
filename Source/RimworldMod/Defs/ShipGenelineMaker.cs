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


			geneline.smallTurretGene = BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(def.smallTurret), true);
			geneline.mediumTurretGene = BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(def.mediumTurret), true);
			geneline.largeTurretGene = BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(def.largeTurret), true);
			geneline.spinalTurretGene = BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(def.spinalTurret), true);
			geneline.armor = BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(def.armor), true);

			foreach(String geneName in def.miscGenes) {
				geneline.genes.Add(BuildingGeneMaker.MakeBuildingGene(BuildingGeneDef.Named(geneName), true));
			}

			return geneline;
		}
	}
}
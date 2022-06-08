using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using SaveOurShip2;
using RimWorld.Planet;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
    public class ShipBodyMapComp : MapComponent
    {
        public Building_ShipHeart heart = null;
        public List<Thing> shipFlesh = new List<Thing>();
        public List<CompShipNutritionConsumer> consumers = new List<CompShipNutritionConsumer>();
        public List<CompShipNutritionStore> stores = new List<CompShipNutritionStore>();
        public List<CompShipNutritionSource> source = new List<CompShipNutritionSource>();

        public ShipBodyMapComp(Map map) : base(map)
        {
        }
        public void Register(CompShipNutrition comp)
        {
        }
    }
}
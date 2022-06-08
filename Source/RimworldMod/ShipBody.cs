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
    public class ShipBody
    {
        public Building_ShipHeart heart = null;
        public HashSet<Thing> shipFlesh = new HashSet<Thing>();
        public List<CompShipNutritionConsumer> consumers = new List<CompShipNutritionConsumer>();
        public List<CompShipNutritionStore> stores = new List<CompShipNutritionStore>();
        public List<CompShipNutritionSource> source = new List<CompShipNutritionSource>();

        public void Register(Building_ShipHeart _heart)
        {
            Log.Message("Registering heart" + _heart.heartId);
            heart = _heart;
            heart.body = this;
        }

        public void Register(CompShipBodyPart comp)
        {
            Log.Message("Parent " + comp.parent);
            shipFlesh.Add(comp.parent);
            comp.body = this;
        }

        public void DeRegister(CompShipBodyPart comp)
        {

        }
    }
}
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
    [StaticConstructorOnStartup]
    public class ShipBodyMapComp : MapComponent
    {
        int curTick = 0;
        public Dictionary<String, ShipBody> bodies = new Dictionary<String, ShipBody>();

        public ShipBodyMapComp(Map map) : base(map)
        {
        }
        public void Register(Building_ShipHeart heart)
        {
            ShipBody body = bodies.TryGetValue(heart.heartId);
            if (body == null)
            {
                body = new ShipBody();
                bodies.Add(heart.heartId, body);
            }
            body.Register(heart);
        }
        public void Register(CompShipBodyPart comp)
        {
            ShipBody body = bodies.TryGetValue(comp.heartId);
            if (body == null)
            {
                body = new ShipBody();
                bodies.Add(comp.heartId, body);
            }
            body.Register(comp);
        }
        public void Register(CompShipNutrition comp)
        {
            ShipBody body = bodies.TryGetValue(comp.heartId);
            if (body == null)
            {
                body = new ShipBody();
                bodies.Add(comp.heartId, body);
            }
            body.Register(comp);
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (curTick % 120 == 0) { 
                foreach (string b in bodies.Keys)
                {
                    bodies.TryGetValue(b).runNutrition();
                }
            }
            curTick++;
        }
    }
}
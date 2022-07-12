using LivingBuildingFramework;
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
    public class ShipBody : BuildingBody
    {
        public HashSet<Thing> otherFlesh = new HashSet<Thing>();
        public HashSet<Thing> adjacentMechanicals = new HashSet<Thing>();

        public override void Register(CompBuildingCore _heart)
        {
            base.Register(_heart);
            passiveConsumption += 150;
        }

        public override void Register(CompBuildingBodyPart _comp)
        {
            base.Register(_comp);
            if (_comp is CompShipBodyPart)
            {
                CompShipBodyPart comp = (CompShipBodyPart)_comp; 
                foreach (Thing t in comp.adjBodypart)
                {
                    otherFlesh.Add(t);
                }
                comp.adjBodypart.Clear();
                foreach (Thing t in comp.adjMechs)
                {
                    adjacentMechanicals.Add(t);
                }
                comp.adjMechs.Clear();
            }
        }

        public void ApplyMutationToAll(IMutation mutation)
        {
            foreach (Thing bodypart in shipFlesh)
            {
                mutation.Apply(bodypart);
            }
        }
    }
}
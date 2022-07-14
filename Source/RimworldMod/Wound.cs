using SaveOurShip2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class Wound : IExposable
    {
        public List<Thing> woundList = new List<Thing>();
        public Stack<Thing> wounds = new Stack<Thing>();

        void IExposable.ExposeData()
        {
            woundList = wounds.ToList();
            Scribe_Collections.Look<Thing>(ref woundList, "woundList", LookMode.Deep);
            wounds = new Stack<Thing>();
            woundList.Reverse();
            foreach (Thing t in woundList)
            {
                wounds.Push(t);
            }
            woundList = null;
        }
    }
}
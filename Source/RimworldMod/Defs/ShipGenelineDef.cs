using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using Verse;

namespace Verse
{
    public class ShipGenelineDef : Def
    {
        public Type shipGenelineClass;
        public String smallTurret;
        public String mediumTurret;
        public String largeTurret;
        public String spinalTurret;
        public String armor;
        public List<String> miscGenes;
        public List<String> randomGenes;

        public static ShipGenelineDef Named(string defName)
        {
            return DefDatabase<ShipGenelineDef>.GetNamed(defName, true);
        }
    }

}

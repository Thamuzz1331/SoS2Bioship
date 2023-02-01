using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using Verse.AI.Group;
using LivingBuildings;

namespace RimWorld
{
    public interface IAggressionSource 
    {
        int GetAggressionValue();
    }
}
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
    public enum ToxSeverity
    {
        NONE, INITIAL, MINOR, MODERATE, SERIOUS, EXTREME
    }
    public class BuildingHediff_ToxicBuildup : BuildingHediff
    {
        public float toxLevel = 0;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref toxLevel, "toxLevel", 0);
        }

        public override bool Visible
        {
            get
            {
                return GetSeverityForMass() != ToxSeverity.NONE;
            }
        }

        public override string DisplayLabel
        {
            get
            {
                return LabelBase + ": " + GetSeverityForMass();
            }
        }

        public override string HoverText
        {
            get
            {
                string ret = def.description;
                ret += "\n" + (toxLevel / bp.body.bodyParts.Count);
                foreach(string stat in ToxMults.TryGetValue(GetSeverityForMass(), null).Keys)
                {
                    ret += "\n" + stat.Translate() + ": x" + this.StatMod(stat);
                }
                return ret;
            }
        }

        public override float StatMod(string stat)
        {
            return ToxMults
                .TryGetValue(GetSeverityForMass(), new Dictionary<string, float>())
                .TryGetValue(stat, 1f);
        }

        public ToxSeverity GetSeverityForMass()
        {
            float toxSev = toxLevel / bp.body.bodyParts.Count;
            if (toxSev <= 0f)
                return ToxSeverity.NONE;
            else if (toxSev <= 0.04f)
                return ToxSeverity.INITIAL;
            else if (toxSev <= .2f)
                return ToxSeverity.MINOR;
            else if (toxSev <= .4f)
                return ToxSeverity.MODERATE;
            else if (toxSev <= .6f)
                return ToxSeverity.SERIOUS;
            else if (toxSev <= .8f)
                return ToxSeverity.EXTREME;
            return ToxSeverity.NONE;
        }

        public static Dictionary<ToxSeverity, Dictionary<string, float>> ToxMults = new Dictionary<ToxSeverity, Dictionary<string, float>>
        {
            {
                ToxSeverity.NONE, new Dictionary<string, float>()
            },
            {
                ToxSeverity.INITIAL, new Dictionary<string, float>()
                {
                    { "regenSpeed", 0.95f }
                }
            },
            {
                ToxSeverity.MINOR, new Dictionary<string, float>()
                {
                    { "regenSpeed", 0.85f },
                    { "conciousness", 0.95f }
                }
            },
            {
                ToxSeverity.MODERATE, new Dictionary<string, float>()
                {
                    { "metabolicSpeed", 0.75f },
                    { "regenEfficiency", 0.85f },
                    { "conciousness", 0.85f }
                }
            },
            {
                ToxSeverity.SERIOUS, new Dictionary<string, float>()
                {
                    { "metabolicSpeed", 0.55f },
                    { "regenEfficiency", 0.75f },
                    { "conciousness", 0.65f }
                }
            },
            {
                ToxSeverity.EXTREME, new Dictionary<string, float>()
                {
                    { "metabolicSpeed", 0.25f },
                    { "metabolicEfficiency", 0.55f },
                    { "conciousness", 0.45f }
                }
            },
        };
    }
}
using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using Verse.AI;

namespace RimWorld
{
	internal class WorkGiver_ImplantHeartSeed : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}

		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			List<CompHeartSeed> seeds = ((CorpseMawTracker)pawn.Map.components.Where(t => t is CorpseMawTracker).FirstOrDefault()).heartSeeds;
			List<Thing> ret = new List<Thing>();
			foreach(CompHeartSeed seed in seeds)
            {
				ret.Add(seed.parent);
            }
			return ret;
		}

		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
			CompHeartSeed heartSeed = thing.TryGetComp<CompHeartSeed>();
			bool hasJob = heartSeed != null
				&& !thing.IsForbidden(pawn)
				&& pawn.CanReserveAndReach(thing, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, false)
				&& FindHeart(pawn, thing) != null
				&& !thing.IsBurning();
			Log.Message("hasJob" + hasJob);
			return hasJob;
		}

		public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
			Thing t = FindHeart(pawn, thing);
			if (t == null)
            {
				return null;
            }
			Log.Message("Make implant job");
			return JobMaker.MakeJob(BioShipJobDefs.ImplantHeartSeed, t, thing);
		}

		private Thing FindHeart(Pawn pawn, Thing thing)
        {
			CompHeartSeed seed = thing.TryGetComp<CompHeartSeed>();
			return seed.TargetHeart;
		}
	}
}

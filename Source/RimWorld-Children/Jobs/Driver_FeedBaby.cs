using Verse;
using Verse.AI;
using RimWorld;

namespace RimWorldChildren
{

	public class WorkGiver_FeedBaby : WorkGiver_Scanner
	{
		//
		// Properties
		//
		public override PathEndMode PathEndMode {
			get {
				return PathEndMode.Touch;
			}
		}
		public override ThingRequest PotentialWorkThingRequest {
			get {
				return ThingRequest.ForGroup (ThingRequestGroup.Pawn);
			}
		}

		//
		// Methods
		//
		public override bool HasJobOnThing (Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = t as Pawn;
			if (pawn2 == null || pawn2 == pawn) {
				return false;
			}
			if (!pawn2.RaceProps.Humanlike) {
				return false;
			}
			if (pawn2.needs.food == null || pawn2.needs.food.CurLevelPercentage > pawn2.needs.food.PercentageThreshHungry + 0.02) {
				return false;
			}
			if (!pawn2.InBed()){
				return false;
			}
			if (!FeedPatientUtility.ShouldBeFed (pawn2)) {
				return false;
			}
			if (!pawn.CanReserveAndReach (t, PathEndMode.ClosestTouch, Danger.Deadly, 1, -1, null, forced)) {
				return false;
			}
			Thing thing;
			ThingDef thingDef;
			if (!FoodUtility.TryFindBestFoodSourceFor(pawn, pawn2, pawn2.needs.food.CurCategory == HungerCategory.UrgentlyHungry, out thing, out thingDef, false, true, false, false, false))
			{
				JobFailReason.Is("NoFood".Translate());
				return false;
			}
			return true;
		}
		
		// We just use the FeedPatient Job from the medical branch
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = (Pawn)t;
			Thing t2;
			ThingDef def1;
			if (FoodUtility.TryFindBestFoodSourceFor(pawn, pawn2, pawn2.needs.food.CurCategory == HungerCategory.UrgentlyHungry, out t2, out def1, false, true, false, false, false))
			{
				return new Job(JobDefOf.FeedPatient)
				{
					targetA = t2,
					targetB = pawn2,
					count = FoodUtility.WillIngestStackCountOf(pawn2, def1)
				};
			}
			return null;
		}
	}
}
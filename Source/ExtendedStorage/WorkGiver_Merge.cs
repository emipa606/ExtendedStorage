using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExtendedStorage;

public class WorkGiver_Merge : WorkGiver_Scanner
{
    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return from t in (from b in pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Building_ExtendedStorage>()
                where !b.AtCapacity && !b.IsForbidden(pawn) && b.StoredThingTotal > 0
                select b).SelectMany(b => b.StoredThings)
            where t.TheoreticallyStackable(pawn)
            select t;
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        return !PotentialWorkThingsGlobal(pawn).Any();
    }

    public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
    {
        if (!HaulAIUtility.PawnCanAutomaticallyHaulFast(pawn, thing, forced))
        {
            return null;
        }

        return TryGetTargetCell(pawn, thing, out var storeCell)
            ? HaulAIUtility.HaulToCellStorageJob(pawn, thing, storeCell, true)
            : null;
    }

    public static bool TryGetTargetCell(Pawn pawn, Thing thing, out IntVec3 targetCell)
    {
        var room = thing.GetRoom(RegionType.Normal | RegionType.Portal);
        var source = StorageUtility.GetStoringBuilding(thing);
        foreach (var building_ExtendedStorage in from target in thing.Map.listerBuildings
                     .AllBuildingsColonistOfClass<Building_ExtendedStorage>()
                 where target.GetRoom(RegionType.Normal | RegionType.Portal) == room && target != source
                 orderby target.StoredThingTotal descending
                 select target)
        {
            if (!building_ExtendedStorage.CanBeMergeTargetFor(source, thing, pawn))
            {
                continue;
            }

            targetCell = building_ExtendedStorage.InputSlot;
            return true;
        }

        targetCell = IntVec3.Zero;
        return false;
    }
}
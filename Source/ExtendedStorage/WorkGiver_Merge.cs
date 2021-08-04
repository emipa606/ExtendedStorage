using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace ExtendedStorage
{
    // Token: 0x0200000F RID: 15
    public class WorkGiver_Merge : WorkGiver_Scanner
    {
        // Token: 0x0600004C RID: 76 RVA: 0x00003330 File Offset: 0x00001530
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return from t in (from b in pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Building_ExtendedStorage>()
                    where !b.AtCapacity && !b.IsForbidden(pawn) && b.StoredThingTotal > 0
                    select b).SelectMany(b => b.StoredThings)
                where t.TheoreticallyStackable(pawn)
                select t;
        }

        // Token: 0x0600004D RID: 77 RVA: 0x000033A5 File Offset: 0x000015A5
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !PotentialWorkThingsGlobal(pawn).Any();
        }

        // Token: 0x0600004E RID: 78 RVA: 0x000033B8 File Offset: 0x000015B8
        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            if (!HaulAIUtility.PawnCanAutomaticallyHaulFast(pawn, thing, forced))
            {
                return null;
            }

            if (TryGetTargetCell(pawn, thing, out var storeCell))
            {
                return HaulAIUtility.HaulToCellStorageJob(pawn, thing, storeCell, true);
            }

            return null;
        }

        // Token: 0x0600004F RID: 79 RVA: 0x000033E8 File Offset: 0x000015E8
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
}
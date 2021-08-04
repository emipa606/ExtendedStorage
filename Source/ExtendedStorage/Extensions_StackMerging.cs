using System.Linq;
using RimWorld;
using Verse;

namespace ExtendedStorage
{
    // Token: 0x02000005 RID: 5
    public static class Extensions_StackMerging
    {
        // Token: 0x06000030 RID: 48 RVA: 0x00002D8D File Offset: 0x00000F8D
        public static bool TheoreticallyStackable(this Thing thing, Pawn pawn = null)
        {
            return !thing.IsForbidden(pawn?.Faction ?? Faction.OfPlayer) &&
                   thing.IsInValidBestStorage();
        }

        // Token: 0x06000031 RID: 49 RVA: 0x00002DB4 File Offset: 0x00000FB4
        public static bool CanBeMergeTargetFor(this Building_ExtendedStorage target, Building_ExtendedStorage source,
            Thing thing, Pawn pawn)
        {
            if (thing.def != target.StoredThingDef || target.AtCapacity ||
                source.StoredThingTotal > target.StoredThingTotal || !StoreUtility.IsGoodStoreCell(target.InputSlot,
                    target.Map, thing, pawn, pawn?.Faction ?? Faction.OfPlayer))
            {
                return false;
            }

            var thing2 = target.StoredThings.First();
            return thing2 != null && thing2.IsInValidBestStorage();
        }
    }
}
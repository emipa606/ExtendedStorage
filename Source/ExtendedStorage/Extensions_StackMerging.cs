using System.Linq;
using RimWorld;
using Verse;

namespace ExtendedStorage;

public static class Extensions_StackMerging
{
    public static bool TheoreticallyStackable(this Thing thing, Pawn pawn = null)
    {
        return !thing.IsForbidden(pawn?.Faction ?? Faction.OfPlayer) &&
               thing.IsInValidBestStorage();
    }

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
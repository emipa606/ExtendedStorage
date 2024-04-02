using HarmonyLib;
using RimWorld;
using Verse;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(MinifyUtility), nameof(MinifyUtility.MakeMinified))]
internal class MinifyUtility_MakeMinified
{
    public static void Prefix(Thing thing)
    {
        if (thing is not Building_ExtendedStorage building_ExtendedStorage)
        {
            return;
        }

        building_ExtendedStorage.TrySplurgeStoredItems();
    }
}
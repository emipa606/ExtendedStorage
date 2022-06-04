using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(MinifyUtility), "MakeMinified")]
[UsedImplicitly]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class MinifyUtility_MakeMinified
{
    public static void Prefix(Thing thing)
    {
        if (!(thing is Building_ExtendedStorage building_ExtendedStorage))
        {
            return;
        }

        building_ExtendedStorage.TrySplurgeStoredItems();
    }
}
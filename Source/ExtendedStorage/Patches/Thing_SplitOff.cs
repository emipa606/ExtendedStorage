using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Verse;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(Thing), "SplitOff")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class Thing_SplitOff
{
    public static void Postfix(Thing __instance)
    {
        var storingBuilding = StorageUtility.GetStoringBuilding(__instance);
        if (storingBuilding == null)
        {
            return;
        }

        storingBuilding.UpdateCachedAttributes();
    }
}
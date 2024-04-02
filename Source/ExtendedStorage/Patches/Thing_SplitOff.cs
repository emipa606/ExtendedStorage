using HarmonyLib;
using Verse;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(Thing), nameof(Thing.SplitOff))]
internal class Thing_SplitOff
{
    public static void Postfix(Thing __instance)
    {
        var storingBuilding = StorageUtility.GetStoringBuilding(__instance);

        storingBuilding?.UpdateCachedAttributes();
    }
}
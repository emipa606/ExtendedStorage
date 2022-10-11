using Verse;

namespace ExtendedStorage.Patches;

//[HarmonyPatch(typeof(Thing), "Print")]
internal class Thing_Print
{
    public static bool Prefix(Thing __instance)
    {
        var storingBuilding = StorageUtility.GetStoringBuilding(__instance);
        var intVec = storingBuilding != null ? new IntVec3?(storingBuilding.OutputSlot) : null;
        var position = __instance.Position;
        return intVec == null || !(intVec.GetValueOrDefault() == position);
    }
}
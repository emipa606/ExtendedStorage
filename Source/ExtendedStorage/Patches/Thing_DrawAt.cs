using HarmonyLib;
using Verse;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(Thing), "DrawAt")]
internal class Thing_DrawAt
{
    public static bool Prefix(Thing __instance)
    {
        if (__instance.def.SingleStorableDef())
        {
            return true;
        }

        var storingBuilding = StorageUtility.GetStoringBuilding(__instance);
        var intVec = storingBuilding != null ? new IntVec3?(storingBuilding.OutputSlot) : null;
        var position = __instance.Position;
        return intVec == null || !(intVec.GetValueOrDefault() == position);
    }
}
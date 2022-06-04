using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(Thing), "DrawGUIOverlay")]
[UsedImplicitly]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class Thing_DrawGUIOverlay
{
    public static bool Prefix(Thing __instance)
    {
        var storingBuilding = StorageUtility.GetStoringBuilding(__instance);
        var intVec = storingBuilding != null ? new IntVec3?(storingBuilding.OutputSlot) : null;
        var position = __instance.Position;
        return intVec == null || !(intVec.GetValueOrDefault() == position);
    }
}
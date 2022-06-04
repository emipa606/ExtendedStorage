using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(Thing), "DrawAt")]
[UsedImplicitly]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class Thing_DrawAt
{
    public static bool Prefix(Thing __instance, Vector3 drawLoc, bool flip)
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
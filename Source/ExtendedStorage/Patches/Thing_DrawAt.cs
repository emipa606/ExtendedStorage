using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace ExtendedStorage.Patches
{
    // Token: 0x02000018 RID: 24
    [HarmonyPatch(typeof(Thing), "DrawAt")]
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class Thing_DrawAt
    {
        // Token: 0x06000066 RID: 102 RVA: 0x00003A68 File Offset: 0x00001C68
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
}
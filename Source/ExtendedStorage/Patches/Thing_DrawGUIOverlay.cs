using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace ExtendedStorage.Patches
{
    // Token: 0x0200001B RID: 27
    [HarmonyPatch(typeof(Thing), "DrawGUIOverlay")]
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class Thing_DrawGUIOverlay
    {
        // Token: 0x0600006D RID: 109 RVA: 0x00003BA0 File Offset: 0x00001DA0
        public static bool Prefix(Thing __instance)
        {
            var storingBuilding = StorageUtility.GetStoringBuilding(__instance);
            var intVec = storingBuilding != null ? new IntVec3?(storingBuilding.OutputSlot) : null;
            var position = __instance.Position;
            return intVec == null || !(intVec.GetValueOrDefault() == position);
        }
    }
}
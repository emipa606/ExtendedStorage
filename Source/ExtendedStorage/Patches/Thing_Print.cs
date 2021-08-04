using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace ExtendedStorage.Patches
{
    // Token: 0x02000019 RID: 25
    [HarmonyPatch(typeof(Thing), "Print")]
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class Thing_Print
    {
        // Token: 0x06000068 RID: 104 RVA: 0x00003AD4 File Offset: 0x00001CD4
        public static bool Prefix(Thing __instance)
        {
            var storingBuilding = StorageUtility.GetStoringBuilding(__instance);
            var intVec = storingBuilding != null ? new IntVec3?(storingBuilding.OutputSlot) : null;
            var position = __instance.Position;
            return intVec == null || !(intVec.GetValueOrDefault() == position);
        }
    }
}
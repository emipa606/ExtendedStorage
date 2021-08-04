using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Verse;

namespace ExtendedStorage.Patches
{
    // Token: 0x02000014 RID: 20
    [HarmonyPatch(typeof(Thing), "SplitOff")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class Thing_SplitOff
    {
        // Token: 0x0600005C RID: 92 RVA: 0x0000368D File Offset: 0x0000188D
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
}
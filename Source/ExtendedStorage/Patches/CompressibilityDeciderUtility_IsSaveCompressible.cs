using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Verse;

namespace ExtendedStorage.Patches
{
    // Token: 0x02000010 RID: 16
    [HarmonyPatch(typeof(CompressibilityDeciderUtility), "IsSaveCompressible")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class CompressibilityDeciderUtility_IsSaveCompressible
    {
        // Token: 0x06000051 RID: 81 RVA: 0x000034C4 File Offset: 0x000016C4
        public static void Postfix(ref bool __result, Thing t)
        {
            if (!__result)
            {
                return;
            }

            __result = !t.Map.thingGrid.ThingsListAt(t.Position).Any(thing => thing is Building_ExtendedStorage);
            Log.Message($"{t.Label}: {__result}");
        }
    }
}
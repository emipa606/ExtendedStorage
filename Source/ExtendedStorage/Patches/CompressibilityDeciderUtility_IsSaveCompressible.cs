using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Verse;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(CompressibilityDeciderUtility), "IsSaveCompressible")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class CompressibilityDeciderUtility_IsSaveCompressible
{
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
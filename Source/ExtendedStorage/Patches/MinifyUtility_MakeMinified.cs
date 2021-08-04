using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace ExtendedStorage.Patches
{
    // Token: 0x02000017 RID: 23
    [HarmonyPatch(typeof(MinifyUtility), "MakeMinified")]
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class MinifyUtility_MakeMinified
    {
        // Token: 0x06000064 RID: 100 RVA: 0x00003A54 File Offset: 0x00001C54
        public static void Prefix(Thing thing)
        {
            if (!(thing is Building_ExtendedStorage building_ExtendedStorage))
            {
                return;
            }

            building_ExtendedStorage.TrySplurgeStoredItems();
        }
    }
}
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;

namespace ExtendedStorage.Patches
{
    // Token: 0x02000013 RID: 19
    [HarmonyPatch(typeof(StorageSettings), "set_Priority")]
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class StorageSettings_set_Priority
    {
        // Token: 0x0600005A RID: 90 RVA: 0x0000367B File Offset: 0x0000187B
        public static void Postfix(StorageSettings __instance)
        {
            if (!(__instance is UserSettings userSettings))
            {
                return;
            }

            userSettings.NotifyOwnerSettingsChanged();
        }
    }
}
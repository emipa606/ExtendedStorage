using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(StorageSettings), "set_Priority")]
[UsedImplicitly]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class StorageSettings_set_Priority
{
    public static void Postfix(StorageSettings __instance)
    {
        if (!(__instance is UserSettings userSettings))
        {
            return;
        }

        userSettings.NotifyOwnerSettingsChanged();
    }
}
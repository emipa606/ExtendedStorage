using HarmonyLib;
using RimWorld;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(StorageSettings), nameof(StorageSettings.Priority), MethodType.Setter)]
internal class StorageSettings_set_Priority
{
    public static void Postfix(StorageSettings __instance)
    {
        if (__instance is not UserSettings userSettings)
        {
            return;
        }

        userSettings.NotifyOwnerSettingsChanged();
    }
}
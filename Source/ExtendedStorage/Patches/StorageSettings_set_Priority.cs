using HarmonyLib;
using RimWorld;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(StorageSettings), "set_Priority")]
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
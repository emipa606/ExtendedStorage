using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(ITab_Storage), "FillTab")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class ITab_Storage_FillTab
{
    public static bool showStoreSettings;

    public static void Postfix(ITab_Storage __instance)
    {
        if (!DebugSettings.godMode || __instance == null)
        {
            return;
        }

        var rect = new Rect(175f, 10f, 100f, 29f);
        Text.Font = GameFont.Tiny;
        if (!Widgets.ButtonText(rect, $"[Debug] {(showStoreSettings ? "Store" : "User")}", true, false))
        {
            return;
        }

        var list = new List<FloatMenuOption>
        {
            new FloatMenuOption("User", delegate { showStoreSettings = false; }),
            new FloatMenuOption("Store", delegate { showStoreSettings = true; })
        };
        Find.WindowStack.Add(new FloatMenu(list));
    }
}
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace ExtendedStorage.Patches
{
    // Token: 0x02000016 RID: 22
    [HarmonyPatch(typeof(ITab_Storage), "FillTab")]
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class ITab_Storage_FillTab
    {
        // Token: 0x04000013 RID: 19
        public static bool showStoreSettings;

        // Token: 0x06000061 RID: 97 RVA: 0x00003964 File Offset: 0x00001B64
        public static void Postfix(ITab_Storage __instance)
        {
            if (!DebugSettings.godMode || __instance == null)
            {
                return;
            }

            var rect = new Rect(175f, 10f, 100f, 29f);
            Text.Font = GameFont.Tiny;
            if (!Widgets.ButtonText(rect, "[Debug] " + (showStoreSettings ? "Store" : "User"), true, false))
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
}
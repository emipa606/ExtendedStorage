using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace ExtendedStorage;

internal class ExtendedStorageMod : Mod
{
    public ExtendedStorageMod(ModContentPack content) : base(content)
    {
        try
        {
            new Harmony("com.extendedstorage.patches").PatchAll(Assembly.GetExecutingAssembly());
            Log.Message(
                $"Extended Storage {typeof(ExtendedStorageMod).Assembly.GetName().Version} - Harmony patches successful");
        }
        catch (Exception arg)
        {
            Log.Error($"Extended Storage :: Caught exception: {arg}");
        }
    }
}
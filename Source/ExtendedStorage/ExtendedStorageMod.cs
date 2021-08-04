using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace ExtendedStorage
{
    // Token: 0x02000002 RID: 2
    internal class ExtendedStorageMod : Mod
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
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
                Log.Error("Extended Storage :: Caught exception: " + arg);
            }
        }
    }
}
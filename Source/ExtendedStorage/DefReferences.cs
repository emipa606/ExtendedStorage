using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using RimWorld;
using Verse;

namespace ExtendedStorage
{
    // Token: 0x02000008 RID: 8
    [GeneratedCode("Defs.Generated.tt", "0.1")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class DefReferences
    {
        // Token: 0x0400000C RID: 12
        private static readonly StatDef _Stat_ES_StorageFactor = DefDatabase<StatDef>.GetNamed("ES_StorageFactor");

        // Token: 0x0400000D RID: 13
        private static readonly StatCategoryDef _StatCategory_ExtendedStorage =
            DefDatabase<StatCategoryDef>.GetNamed("ExtendedStorage");

        // Token: 0x1700000C RID: 12
        // (get) Token: 0x06000036 RID: 54 RVA: 0x00002E61 File Offset: 0x00001061
        public static StatDef Stat_ES_StorageFactor => _Stat_ES_StorageFactor;

        // Token: 0x1700000D RID: 13
        // (get) Token: 0x06000037 RID: 55 RVA: 0x00002E68 File Offset: 0x00001068
        public static StatCategoryDef StatCategory_ExtendedStorage => _StatCategory_ExtendedStorage;
    }
}
using System.CodeDom.Compiler;
using RimWorld;
using Verse;

namespace ExtendedStorage;

[GeneratedCode("Defs.Generated.tt", "0.1")]
public class DefReferences
{
    public static StatDef Stat_ES_StorageFactor { get; } = DefDatabase<StatDef>.GetNamed("ES_StorageFactor");

    public static StatCategoryDef StatCategory_ExtendedStorage { get; } =
        DefDatabase<StatCategoryDef>.GetNamed("ExtendedStorage");
}
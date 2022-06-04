using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using RimWorld;
using Verse;

namespace ExtendedStorage;

[GeneratedCode("Defs.Generated.tt", "0.1")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class DefReferences
{
    public static StatDef Stat_ES_StorageFactor { get; } = DefDatabase<StatDef>.GetNamed("ES_StorageFactor");

    public static StatCategoryDef StatCategory_ExtendedStorage { get; } =
        DefDatabase<StatCategoryDef>.GetNamed("ExtendedStorage");
}
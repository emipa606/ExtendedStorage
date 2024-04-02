using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(Map), nameof(Map.FinalizeLoading))]
internal class Map_FinalizeLoading
{
    private static readonly MethodInfo mi =
        typeof(BackCompatibility).GetMethod("PreCheckSpawnBackCompatibleThingAfterLoading");

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
    {
        var list = new List<CodeInstruction>(instr);
        var num = list.FindIndex(ci => ci.opcode == OpCodes.Call && (MethodInfo)ci.operand == mi);
        if (num == -1)
        {
            Log.Warning("Could not find Map_FinalizeLoading transpiler anchor - not patching.");
            return list;
        }

        list.InsertRange(num + 1, new[]
        {
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call,
                typeof(Map_FinalizeLoading).GetMethod("PrioritySpawnStorageBuildings"))
        });
        return list;
    }

    public static void PrioritySpawnStorageBuildings(List<Thing> things, Map map)
    {
        foreach (var building_ExtendedStorage in things.OfType<Building_ExtendedStorage>().ToArray())
        {
            try
            {
                GenSpawn.SpawnBuildingAsPossible(building_ExtendedStorage, map, true);
            }
            catch (Exception ex)
            {
                Log.Error(
                    $"Exception spawning loaded thing {building_ExtendedStorage.ToStringSafe<Building>()}: {ex}");
            }

            things.Remove(building_ExtendedStorage);
        }
    }
}
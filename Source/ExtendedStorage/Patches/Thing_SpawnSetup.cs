using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace ExtendedStorage.Patches;

[HarmonyPatch(typeof(Thing), "SpawnSetup")]
[UsedImplicitly]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class Thing_SpawnSetup
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var instructionsList = instructions.ToList();
        int num;
        for (var i = 0; i < instructionsList.Count; i = num + 1)
        {
            var instruction = instructionsList[i];
            yield return instruction;
            if (instruction.opcode == OpCodes.Ble &&
                (FieldInfo)instructionsList[i - 1].operand == typeof(ThingDef).GetField("stackLimit"))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Call,
                    typeof(Thing_SpawnSetup).GetMethod("VerifyThingShouldNotBeTruncated"));
                yield return new CodeInstruction(OpCodes.Brtrue, instruction.operand);
            }

            num = i;
        }
    }

    public static bool VerifyThingShouldNotBeTruncated(Thing t, Map map)
    {
        return map.thingGrid.ThingsListAt(t.Position).FirstOrDefault(o => o is Building_ExtendedStorage) is
            Building_ExtendedStorage building_ExtendedStorage && building_ExtendedStorage.OutputSlot == t.Position;
    }
}
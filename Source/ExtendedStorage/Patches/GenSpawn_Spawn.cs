using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace ExtendedStorage.Patches
{
    // Token: 0x02000011 RID: 17
    [HarmonyPatch(typeof(GenSpawn), "Spawn", typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4),
        typeof(WipeMode), typeof(bool))]
    public static class GenSpawn_Spawn
    {
        // Token: 0x04000011 RID: 17
        //public static FieldInfo LoadedFullThingsField =
        //    typeof(Map).GetField("loadedFullThings", BindingFlags.Static | BindingFlags.NonPublic);

        // Token: 0x06000053 RID: 83 RVA: 0x000034EC File Offset: 0x000016EC

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = new List<CodeInstruction>(instructions);
            int i;
            for (i = 0; i < code.Count; i++)
            {
                yield return code[i];
                if (code[i].opcode != OpCodes.Ldarg_0 || code[i + 1]?.opcode != OpCodes.Ldfld ||
                    (FieldInfo)code[i + 1]?.operand != typeof(Thing).GetField("def"))
                {
                    continue;
                }

                i++;
                yield return code[i];
                if (code[i + 1]?.opcode != OpCodes.Ldfld ||
                    (FieldInfo)code[i + 1]?.operand != typeof(ThingDef).GetField("category"))
                {
                    continue;
                }

                i++;
                yield return code[i++];
                yield return code[i++];
                var branchLabel = (Label)code[i].operand;
                yield return code[i++];
                yield return new CodeInstruction(OpCodes.Ldarg_S, 5);
                yield return new CodeInstruction(OpCodes.Brtrue, branchLabel);
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                //yield return (CodeInstruction)(object)new CodeInstruction(OpCodes.Call, (object)AccessTools.Method("LWM.DeepStorage.Utils:CanStoreMoreThanOneThingAt", (Type[])null, (Type[])null));
                yield return new CodeInstruction(OpCodes.Call,
                    typeof(GenSpawn_Spawn).GetMethod("ShouldDisplaceOtherItems"));
                yield return new CodeInstruction(OpCodes.Brtrue, branchLabel);
                break;
            }

            for (; i < code.Count; i++)
            {
                yield return code[i];
            }

            //var ldargsSeen = false;
            //var i = il.DefineLabel();
            //var instrList = instructions.ToList();
            //int num;
            //for (var j = 0; j < instrList.Count; j = num + 1)
            //{
            //    //Log.Message($"{instrList[j].opcode} - {instrList[j].operand}");
            //    if (!ldargsSeen && instrList[j].opcode == OpCodes.Ldarg_S && instrList[j].operand.Equals(4))
            //    {
            //        var item = instrList[j].labels[0];
            //        instrList[j].labels.Clear();
            //        var codeInstruction = new CodeInstruction(OpCodes.Ldarg_1)
            //        {
            //            labels = new List<Label>
            //            {
            //                item
            //            }
            //        };
            //        yield return codeInstruction;
            //        yield return new CodeInstruction(OpCodes.Ldarg_2);
            //        yield return new CodeInstruction(OpCodes.Ldarg_S, 5);
            //        yield return new CodeInstruction(OpCodes.Call,
            //            typeof(GenSpawn_Spawn).GetMethod("ShouldDisplaceOtherItems"));
            //        yield return new CodeInstruction(OpCodes.Brfalse, i);
            //        ldargsSeen = true;
            //    }

            //    if (j + 2 < instrList.Count && instrList[j + 2].opcode == OpCodes.Callvirt &&
            //        instrList[j + 2].operand == typeof(Thing).GetProperty("Rotation").GetSetMethod())
            //    {
            //        instrList[j].labels.Add(i);
            //    }

            //    yield return instrList[j];
            //    num = j;
            //}
        }

        // Token: 0x06000054 RID: 84 RVA: 0x00003503 File Offset: 0x00001703
        public static bool ShouldDisplaceOtherItems(Map map, IntVec3 loc)
        {
            //Log.Message("Will not displace items");
            return map == null || !map.thingGrid.ThingsListAtFast(loc).OfType<Building_ExtendedStorage>().Any();
        }
    }
}
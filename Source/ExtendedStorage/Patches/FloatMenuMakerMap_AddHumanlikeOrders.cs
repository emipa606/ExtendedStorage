using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ExtendedStorage.Patches
{
    // Token: 0x02000015 RID: 21
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class FloatMenuMakerMap_AddHumanlikeOrders
    {
        // Token: 0x0600005E RID: 94 RVA: 0x000036A0 File Offset: 0x000018A0
        public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            var c = IntVec3.FromVector3(clickPos);
            var building_ExtendedStorage = pawn.Map.thingGrid.ThingAt<Building_ExtendedStorage>(c);
            if (building_ExtendedStorage?.def.defName != "Storage_Locker")
            {
                return;
            }

            var list = pawn.Map.thingGrid.ThingsAt(c).OfType<Apparel>().ToList();
            if (list.Count <= 1)
            {
                return;
            }

            var baseOption = CreateMenuOption(pawn, list[0]);
            var num = opts.FirstIndexOf(mo => mo.Label == baseOption.Label);
            var collection = from a in list.Skip(1)
                select CreateMenuOption(pawn, a);
            if (num == -1)
            {
                opts.AddRange(collection);
                return;
            }

            opts.InsertRange(num + 1, collection);
        }

        // Token: 0x0600005F RID: 95 RVA: 0x00003788 File Offset: 0x00001988
        private static FloatMenuOption CreateMenuOption(Pawn pawn, Apparel apparel)
        {
            if (!pawn.CanReach(apparel, PathEndMode.ClosestTouch, Danger.Deadly))
            {
                return new FloatMenuOption(
                    "CannotWear".Translate(apparel.Label, apparel) + " (" + "NoPath".Translate() + ")", null);
            }

            if (apparel.IsBurning())
            {
                return new FloatMenuOption(
                    "CannotWear".Translate(apparel.Label, apparel) + " (" + "BurningLower".Translate() + ")", null);
            }

            if (!ApparelUtility.HasPartsToWear(pawn, apparel.def))
            {
                return new FloatMenuOption(
                    "CannotWear".Translate(apparel.Label, apparel) + " (" +
                    "CannotWearBecauseOfMissingBodyParts".Translate() + ")", null);
            }

            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(
                "ForceWear".Translate(apparel.LabelShort, apparel), delegate
                {
                    apparel.SetForbidden(false);
                    var job = new Job(JobDefOf.Wear, apparel);
                    pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                }, MenuOptionPriority.High), pawn, apparel);
        }
    }
}
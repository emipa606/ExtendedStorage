using RimWorld;
using Verse;

namespace ExtendedStorage
{
    // Token: 0x0200000E RID: 14
    internal static class StorageUtility
    {
        // Token: 0x06000049 RID: 73 RVA: 0x00003240 File Offset: 0x00001440
        public static Building_ExtendedStorage GetStoringBuilding(Thing t)
        {
            Building_ExtendedStorage building_ExtendedStorage;
            if (!(t is Building_ExtendedStorage))
            {
                var slotGroup = t.GetSlotGroup();
                building_ExtendedStorage = slotGroup?.parent as Building_ExtendedStorage;
            }
            else
            {
                building_ExtendedStorage = null;
            }

            var building_ExtendedStorage2 = building_ExtendedStorage;
            if (building_ExtendedStorage2?.StoredThingDef != t.def)
            {
                return null;
            }

            return building_ExtendedStorage2;
        }

        // Token: 0x0600004A RID: 74 RVA: 0x00003288 File Offset: 0x00001488
        public static Thing SplitOfStackInto(Thing existingThing, IntVec3 targetLocation)
        {
            var map = existingThing.Map;
            var thing = ThingMaker.MakeThing(existingThing.def, existingThing.Stuff);
            thing.HitPoints = existingThing.HitPoints;
            if (existingThing.stackCount > existingThing.def.stackLimit)
            {
                existingThing.stackCount -= existingThing.def.stackLimit;
                thing.stackCount = existingThing.def.stackLimit;
            }
            else
            {
                thing.stackCount = existingThing.stackCount;
                existingThing.Destroy();
            }

            return GenSpawn.Spawn(thing, targetLocation, map);
        }

        // Token: 0x0600004B RID: 75 RVA: 0x00003314 File Offset: 0x00001514
        public static bool SingleStorableDef(this ThingDef def)
        {
            return def.IsApparel || def.IsCorpse || def.IsWeapon;
        }
    }
}
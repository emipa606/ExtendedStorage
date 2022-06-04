using RimWorld;
using Verse;

namespace ExtendedStorage;

internal static class StorageUtility
{
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

    public static bool SingleStorableDef(this ThingDef def)
    {
        return def.IsApparel || def.IsCorpse || def.IsWeapon;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtendedStorage.Patches;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ExtendedStorage;

public class Building_ExtendedStorage : Building_Storage, IUserSettingsOwner
{
    internal Graphic _gfxStoredThing;

    private string _label;

    private ThingDef _storedThingDef;

    //private Func<IEnumerable<Gizmo>> Building_GetGizmos;


    internal string label;


    private Action queuedTickAction;

    public UserSettings userSettings;

    public bool AtCapacity => StoredThingTotal >= ApparentMaxStorage;

    public int ApparentMaxStorage
    {
        get
        {
            if (StoredThingDef != null)
            {
                return (int)(StoredThingDef.stackLimit * this.GetStatValue(DefReferences.Stat_ES_StorageFactor));
            }

            return int.MaxValue;
        }
    }

    public IntVec3 OutputSlot { get; private set; }

    public IntVec3 InputSlot { get; private set; }

    internal ThingDef StoredThingDef
    {
        get => _storedThingDef;
        set
        {
            _storedThingDef = value;
            if (_storedThingDef != value)
            {
                Notify_StoredThingDefChanged();
            }
        }
    }

    public Thing StoredThingAtInput
    {
        get
        {
            var map = Map;
            if (map == null)
            {
                return null;
            }

            return map.thingGrid.ThingsAt(InputSlot).FirstOrDefault(StoredThingDef != null
                ? t => t.def == StoredThingDef
                : t => slotGroup.Settings.AllowedToAccept(t));
        }
    }

    public IEnumerable<Thing> StoredThings
    {
        get
        {
            if (StoredThingDef == null)
            {
                return Enumerable.Empty<Thing>();
            }

            var map = Map;
            if (map == null)
            {
                return null;
            }

            return from t in map.thingGrid.ThingsAt(OutputSlot)
                where t.def == StoredThingDef
                select t;
        }
    }

    public int StoredThingTotal
    {
        get { return StoredThings.Sum(t => t.stackCount); }
    }

    public override string LabelNoCount
    {
        get
        {
            string result;
            if ((result = label) == null)
            {
                result = label = InitialLabel();
            }

            return result;
        }
    }

    StorageSettings IStoreSettingsParent.GetStoreSettings()
    {
        if (DebugSettings.godMode && ITab_Storage_FillTab.showStoreSettings)
        {
            return base.GetStoreSettings();
        }

        return userSettings;
    }


    public void Notify_UserSettingsChanged()
    {
        if (settings.Priority != userSettings.Priority)
        {
            settings.Priority = userSettings.Priority;
        }

        settings.filter.CopyAllowancesFrom(userSettings.filter);
        if (StoredThingDef != null && settings.filter.Allows(StoredThingDef))
        {
            Notify_StoredThingDefChanged();
            return;
        }

        TryUnstackStoredItems();
        var storedThingDef = StoredThingDef;
        StoredThingDef = null;
        InvalidateThingSection(storedThingDef);
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        TrySplurgeStoredItems();
        base.Destroy(mode);
    }

    public override void DrawGUIOverlay()
    {
        if (Find.CameraDriver.CurrentZoom != CameraZoomRange.Closest)
        {
            return;
        }

        var textColor = new Color(1f, 1f, 0.5f, 0.75f);
        if (string.IsNullOrEmpty(_label))
        {
            return;
        }

        var thing = StoredThings.FirstOrDefault();
        if (thing != null)
        {
            GenMapUI.DrawThingLabel(thing, _label, textColor);
        }
    }

    public override void Draw()
    {
        base.Draw();
        var storedThingDef = StoredThingDef;
        if (storedThingDef != null && storedThingDef.SingleStorableDef())
        {
            return;
        }

        var gfxStoredThing = _gfxStoredThing;
        if (gfxStoredThing == null)
        {
            return;
        }

        gfxStoredThing.DrawFromDef(
            GenThing.TrueCenter(OutputSlot, Rot4.North, IntVec2.One, DrawPos.y + 0.5f),
            Rot4.North, StoredThingDef);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look(ref _storedThingDef, "storedThingDef");
        Scribe_Deep.Look(ref userSettings, "userSettings", this);
        if (Scribe.mode != LoadSaveMode.Saving || label != null)
        {
            Scribe_Values.Look(ref label, "label", def.label);
        }
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        //Log.Message("if (Building_GetGizmos == null)");
        //if (Building_GetGizmos == null)
        //{
        //    var functionPointer = typeof(Building)
        //        .GetMethod("GetGizmos", BindingFlags.Instance | BindingFlags.Public)?.MethodHandle
        //        .GetFunctionPointer();
        //    Building_GetGizmos =
        //        (Func<IEnumerable<Gizmo>>)Activator.CreateInstance(typeof(Func<IEnumerable<Gizmo>>), this,
        //            functionPointer);
        //}

        //Log.Message("var enumerable = Building_GetGizmos();");
        //var enumerable = Building_GetGizmos();
        var enumerable = base.GetGizmos();
        foreach (var gizmo in enumerable)
        {
            yield return gizmo;
        }


        var command_Action = new Command_Action
        {
            icon = ContentFinder<Texture2D>.Get("UI/Icons/Rename"),
            defaultDesc = "ExtendedStorage.Rename".Translate(def.label),
            defaultLabel = "Rename".Translate(),
            activateSound = SoundDef.Named("Click"),
            action = delegate { Find.WindowStack.Add(new ES_Dialog_Rename(this)); },
            groupKey = 942608684
        };
        yield return command_Action;
        foreach (var gizmo2 in StorageSettingsClipboard.CopyPasteGizmosFor(userSettings))
        {
            yield return gizmo2;
        }
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(base.GetInspectString());
        var key = "ExtendedStorage.CurrentlyStoringInspect";
        var storedThingDef = StoredThingDef;
        stringBuilder.Append(key.Translate(storedThingDef?.LabelCap ?? "ExtendedStorage.Nothing".Translate()));
        return stringBuilder.ToString();
    }

    public override void Notify_LostThing(Thing newItem)
    {
        base.Notify_LostThing(newItem);
        Notify_SlotGroupItemsChanged();
    }

    public override void Notify_ReceivedThing(Thing newItem)
    {
        base.Notify_ReceivedThing(newItem);
        Notify_SlotGroupItemsChanged();
    }

    public override void PostMake()
    {
        base.PostMake();
        userSettings = new UserSettings(this);
        if (def.building.defaultStorageSettings != null)
        {
            userSettings.CopyFrom(def.building.defaultStorageSettings);
        }
    }

    public override void PostMapInit()
    {
        base.PostMapInit();
        RecalculateStoredThingDef();
        ChunkifyOutputSlot();
        UpdateCachedAttributes();
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        var list = GenAdj.CellsOccupiedBy(this).ToList();
        InputSlot = list[0];
        OutputSlot = list[1];
        if (label == null || label.Trim().Length == 0)
        {
            label = InitialLabel();
        }
    }

    private string InitialLabel()
    {
        return GenLabel.ThingLabel(this, 1, false);
    }

    public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
    {
        foreach (var statDrawEntry in base.SpecialDisplayStats())
        {
            yield return statDrawEntry;
        }

        var statCategory_ExtendedStorage = DefReferences.StatCategory_ExtendedStorage;
        string text = "ExtendedStorage.CurrentlyStoringStat".Translate();
        var storedThingDef = StoredThingDef;
        string valueString =
            storedThingDef?.LabelCap ?? "ExtendedStorage.Nothing".Translate();
        var storedThingDef2 = StoredThingDef;
        yield return new StatDrawEntry(statCategory_ExtendedStorage, text, valueString,
            storedThingDef2?.DescriptionDetailed, -1);
        yield return new StatDrawEntry(DefReferences.StatCategory_ExtendedStorage,
            "ExtendedStorage.UsageStat".Translate(),
            StoredThingDef != null
                ? "ExtendedStorage.UsageStat.Value".Translate(StoredThingTotal, ApparentMaxStorage)
                : "ExtendedStorage.NA".Translate(), null, -2);
    }

    public override void Tick()
    {
        base.Tick();
        if (!this.IsHashIntervalTick(10))
        {
            return;
        }

        TryGrabOutputItem();
        var action = queuedTickAction;
        action?.Invoke();

        queuedTickAction = null;
        ChunkifyOutputSlot();
        TryMoveItem();
    }

    private void TryGrabOutputItem()
    {
        if (StoredThingDef != null)
        {
            return;
        }

        var map = Map;
        ThingDef storedThingDef;
        if (map == null)
        {
            storedThingDef = null;
        }
        else
        {
            var thing = map.thingGrid.ThingsAt(OutputSlot).Where(userSettings.AllowedToAccept).FirstOrDefault();
            storedThingDef = thing?.def;
        }

        StoredThingDef = storedThingDef;
        InvalidateThingSection(_storedThingDef);
    }

    private void InvalidateThingSection(ThingDef storedDef)
    {
        var drawerType = storedDef != null ? new DrawerType?(storedDef.drawerType) : null;
        if (drawerType == null)
        {
            return;
        }

        var valueOrDefault = drawerType.GetValueOrDefault();
        if (valueOrDefault - DrawerType.MapMeshOnly > 1)
        {
            return;
        }

        var map = Map;
        if (map == null)
        {
            return;
        }

        map.mapDrawer.SectionAt(OutputSlot).RegenerateLayers(MapMeshFlag.Things);
    }

    public void Notify_StoredThingDefChanged()
    {
        if (StoredThingDef != null)
        {
            foreach (var thingDef in new List<ThingDef>(settings.filter.AllowedThingDefs))
            {
                settings.filter.SetAllow(thingDef, false);
            }

            settings.filter.SetAllow(StoredThingDef, true);
        }
        else
        {
            settings.filter.CopyAllowancesFrom(userSettings.filter);
        }

        UpdateCachedAttributes();
    }

    private void Notify_SlotGroupItemsChanged()
    {
        var storedThingDef = _storedThingDef;
        RecalculateStoredThingDef();
        if (_storedThingDef == storedThingDef)
        {
            UpdateCachedAttributes();
        }
    }

    private void ChunkifyOutputSlot()
    {
        if (StoredThingDef == null)
        {
            return;
        }

        var list = (from t in StoredThings
            where t.stackCount != t.def.stackLimit
            select t).ToList();
        if (list.Count == 1 && list[0].stackCount < StoredThingDef.stackLimit)
        {
            return;
        }

        var i = list.Count - 1;
        var num3 = 0;
        while (i > num3)
        {
            var other = list[i];
            if (!list[num3].TryAbsorbStack(other, true))
            {
                num3++;
            }
            else
            {
                i--;
            }
        }

        if (i != num3)
        {
            return;
        }

        var thing = list[i];
        while (list[i].stackCount > StoredThingDef.stackLimit)
        {
            var thing2 = thing.SplitOff(Math.Min(StoredThingDef.stackLimit,
                thing.stackCount - StoredThingDef.stackLimit));
            thing2.Position = thing.Position;
            thing2.SpawnSetup(thing.Map, false);
            var group = OutputSlot.GetSlotGroup(Map);
            if (group == null)
            {
                continue;
            }

            var parent = group.parent;
            if (parent != null)
            {
                parent.Notify_ReceivedThing(thing2);
            }
        }
    }

    private void RecalculateStoredThingDef()
    {
        if (StoredThingDef == null)
        {
            var map = Map;
            ThingDef storedThingDef;
            if (map == null)
            {
                storedThingDef = null;
            }
            else
            {
                var thing = map.thingGrid.ThingsAt(OutputSlot).FirstOrDefault(t => settings.filter.Allows(t));
                storedThingDef = thing?.def;
            }

            StoredThingDef = storedThingDef;
            return;
        }

        if (!StoredThings.Any())
        {
            StoredThingDef = null;
        }
    }

    private void SplurgeThings(IEnumerable<Thing> things, IntVec3 center, bool forceSplurge = false)
    {
        using var enumerator = GenRadial.RadialCellsAround(center, 20f, false).GetEnumerator();
        foreach (var thing in things)
        {
            if (thing.TryGetComp<CompQuality>() != null)
            {
                continue;
            }

            while (!thing.DestroyedOrNull())
            {
                if (!forceSplurge && thing.stackCount <= thing.def.stackLimit)
                {
                    break;
                }

                if (!EnumUtility.TryGetNext(enumerator, c => c.Standable(Map), out var targetLocation))
                {
                    Log.Warning(
                        "Ran out of cells to splurge " + thing.LabelCap +
                        " - there might be issues on save/reload.");
                    return;
                }

                StorageUtility.SplitOfStackInto(thing, targetLocation);
            }
        }
    }

    private void TryMoveItem()
    {
        var storedThingAtInput = StoredThingAtInput;
        if (storedThingAtInput == null)
        {
            return;
        }

        if (!settings.filter.Allows(storedThingAtInput))
        {
            return;
        }

        var storedThingDef = StoredThingDef;
        if (storedThingDef != null &&
            storedThingAtInput.def != storedThingDef)
        {
            return;
        }

        var num = Math.Min(storedThingAtInput.stackCount, ApparentMaxStorage - StoredThingTotal);
        if (num <= 0)
        {
            return;
        }

        var thing = storedThingAtInput.SplitOff(num);
        thing.Position = OutputSlot;
        thing.SpawnSetup(Map, false);
        StoredThingDef = thing.def;
        var group = OutputSlot.GetSlotGroup(Map);
        if (group == null)
        {
            return;
        }

        var parent = group.parent;
        if (parent == null)
        {
            return;
        }

        parent.Notify_ReceivedThing(thing);
    }

    internal void TrySplurgeStoredItems()
    {
        var storedThings = StoredThings;
        if (storedThings == null)
        {
            return;
        }

        SplurgeThings(storedThings, OutputSlot, true);
        SoundDef.Named("DropPodOpen").PlayOneShot(new TargetInfo(OutputSlot, Map));
        StoredThingDef = null;
    }

    private void TryUnstackStoredItems()
    {
        var thingsToSplurge = StoredThings.ToList();
        queuedTickAction = (Action)Delegate.Combine(queuedTickAction, (Action)delegate
        {
            var array = thingsToSplurge.Where(t => t.def != StoredThingDef).ToArray();
            SplurgeThings(array, OutputSlot, true);
            if (array.Length != 0)
            {
                SoundDef.Named("DropPodOpen").PlayOneShot(new TargetInfo(OutputSlot, Map));
            }
        });
    }

    public void UpdateCachedAttributes()
    {
        if (StoredThingDef == null)
        {
            _label = null;
            _gfxStoredThing = null;
            return;
        }

        var array = (from c in (from c in StoredThings.ToList().Select(delegate(Thing t)
                {
                    if (!t.TryGetQuality(out var value))
                    {
                        return null;
                    }

                    return new QualityCategory?(value);
                })
                where c != null
                select c.Value).Distinct()
            orderby c
            select c).ToArray();
        var storedThingTotal = StoredThingTotal;
        if (array.Length != 0)
        {
            _label = array[0].GetLabelShort();
            if (array.Length > 1)
            {
                _label = string.Format("ExtendedStorage.MultipleQualities".Translate(), _label);
            }
        }
        else
        {
            _label = string.Format("ExtendedStorage.TotalCount".Translate(), storedThingTotal);
        }

        if (!StoredThingDef.SingleStorableDef())
        {
            _gfxStoredThing =
                (StoredThingDef.graphic is Graphic_StackCount graphic_StackCount
                    ? graphic_StackCount.SubGraphicForStackCount(
                        Math.Min(storedThingTotal, StoredThingDef.stackLimit), StoredThingDef)
                    : null) ?? StoredThingDef.graphic;
            return;
        }

        _gfxStoredThing = null;
    }
}
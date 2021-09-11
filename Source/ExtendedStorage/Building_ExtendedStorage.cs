using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtendedStorage.Patches;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ExtendedStorage
{
    // Token: 0x02000004 RID: 4
    public class Building_ExtendedStorage : Building_Storage, IUserSettingsOwner
    {
        // Token: 0x04000001 RID: 1
        internal Graphic _gfxStoredThing;

        // Token: 0x04000002 RID: 2
        private string _label;

        // Token: 0x04000003 RID: 3
        private ThingDef _storedThingDef;

        // Token: 0x04000004 RID: 4
        //private Func<IEnumerable<Gizmo>> Building_GetGizmos;

        // Token: 0x04000005 RID: 5

        // Token: 0x04000008 RID: 8
        internal string label;

        // Token: 0x04000006 RID: 6

        // Token: 0x04000007 RID: 7
        private Action queuedTickAction;

        // Token: 0x04000009 RID: 9
        public UserSettings userSettings;

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000003 RID: 3 RVA: 0x000020C8 File Offset: 0x000002C8
        public bool AtCapacity => StoredThingTotal >= ApparentMaxStorage;

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000004 RID: 4 RVA: 0x000020DB File Offset: 0x000002DB
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

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000005 RID: 5 RVA: 0x00002105 File Offset: 0x00000305
        public IntVec3 OutputSlot { get; private set; }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000006 RID: 6 RVA: 0x0000210D File Offset: 0x0000030D
        public IntVec3 InputSlot { get; private set; }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000007 RID: 7 RVA: 0x00002115 File Offset: 0x00000315
        // (set) Token: 0x06000008 RID: 8 RVA: 0x0000211D File Offset: 0x0000031D
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

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000009 RID: 9 RVA: 0x0000213C File Offset: 0x0000033C
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

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x0600000A RID: 10 RVA: 0x0000218C File Offset: 0x0000038C
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

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x0600000B RID: 11 RVA: 0x000021C9 File Offset: 0x000003C9
        public int StoredThingTotal
        {
            get { return StoredThings.Sum(t => t.stackCount); }
        }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x0600000C RID: 12 RVA: 0x000021F8 File Offset: 0x000003F8
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

        // Token: 0x0600001C RID: 28 RVA: 0x00002573 File Offset: 0x00000773
        StorageSettings IStoreSettingsParent.GetStoreSettings()
        {
            if (DebugSettings.godMode && ITab_Storage_FillTab.showStoreSettings)
            {
                return base.GetStoreSettings();
            }

            return userSettings;
        }

        // Token: 0x0600001D RID: 29 RVA: 0x00002590 File Offset: 0x00000790
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

        // Token: 0x0600000D RID: 13 RVA: 0x0000221E File Offset: 0x0000041E
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            TrySplurgeStoredItems();
            base.Destroy(mode);
        }

        // Token: 0x0600000E RID: 14 RVA: 0x00002230 File Offset: 0x00000430
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

        // Token: 0x0600000F RID: 15 RVA: 0x00002290 File Offset: 0x00000490
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

        // Token: 0x06000010 RID: 16 RVA: 0x000022F4 File Offset: 0x000004F4
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

        // Token: 0x06000011 RID: 17 RVA: 0x0000235D File Offset: 0x0000055D
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

        // Token: 0x06000012 RID: 18 RVA: 0x00002370 File Offset: 0x00000570
        public override string GetInspectString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            var key = "ExtendedStorage.CurrentlyStoringInspect";
            var storedThingDef = StoredThingDef;
            stringBuilder.Append(key.Translate(storedThingDef?.LabelCap ?? "ExtendedStorage.Nothing".Translate()));
            return stringBuilder.ToString();
        }

        // Token: 0x06000013 RID: 19 RVA: 0x000023CA File Offset: 0x000005CA
        public override void Notify_LostThing(Thing newItem)
        {
            base.Notify_LostThing(newItem);
            Notify_SlotGroupItemsChanged();
        }

        // Token: 0x06000014 RID: 20 RVA: 0x000023D9 File Offset: 0x000005D9
        public override void Notify_ReceivedThing(Thing newItem)
        {
            base.Notify_ReceivedThing(newItem);
            Notify_SlotGroupItemsChanged();
        }

        // Token: 0x06000015 RID: 21 RVA: 0x000023E8 File Offset: 0x000005E8
        public override void PostMake()
        {
            base.PostMake();
            userSettings = new UserSettings(this);
            if (def.building.defaultStorageSettings != null)
            {
                userSettings.CopyFrom(def.building.defaultStorageSettings);
            }
        }

        // Token: 0x06000016 RID: 22 RVA: 0x00002434 File Offset: 0x00000634
        public override void PostMapInit()
        {
            base.PostMapInit();
            RecalculateStoredThingDef();
            ChunkifyOutputSlot();
            UpdateCachedAttributes();
        }

        // Token: 0x06000017 RID: 23 RVA: 0x00002450 File Offset: 0x00000650
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

        // Token: 0x06000018 RID: 24 RVA: 0x000024B1 File Offset: 0x000006B1
        private string InitialLabel()
        {
            return GenLabel.ThingLabel(this, 1, false);
        }

        // Token: 0x06000019 RID: 25 RVA: 0x000024BB File Offset: 0x000006BB
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

        // Token: 0x0600001A RID: 26 RVA: 0x000024CB File Offset: 0x000006CB
        public override void Tick()
        {
            base.Tick();
            if (!this.IsHashIntervalTick(10))
            {
                return;
            }

            TryGrabOutputItem();
            var action = queuedTickAction;
            if (action != null)
            {
                action();
            }

            queuedTickAction = null;
            ChunkifyOutputSlot();
            TryMoveItem();
        }

        // Token: 0x0600001B RID: 27 RVA: 0x00002508 File Offset: 0x00000708
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

        // Token: 0x0600001E RID: 30 RVA: 0x00002628 File Offset: 0x00000828
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

        // Token: 0x0600001F RID: 31 RVA: 0x00002688 File Offset: 0x00000888
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

        // Token: 0x06000020 RID: 32 RVA: 0x00002738 File Offset: 0x00000938
        private void Notify_SlotGroupItemsChanged()
        {
            var storedThingDef = _storedThingDef;
            RecalculateStoredThingDef();
            if (_storedThingDef == storedThingDef)
            {
                UpdateCachedAttributes();
            }
        }

        // Token: 0x06000021 RID: 33 RVA: 0x00002764 File Offset: 0x00000964
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

        // Token: 0x06000022 RID: 34 RVA: 0x00002900 File Offset: 0x00000B00
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

        // Token: 0x06000023 RID: 35 RVA: 0x0000296C File Offset: 0x00000B6C
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

        // Token: 0x06000024 RID: 36 RVA: 0x00002A48 File Offset: 0x00000C48
        private void TryMoveItem()
        {
            var storedThingAtInput = StoredThingAtInput;
            if (storedThingAtInput == null)
            {
                return;
            }

            var storedThingDef = StoredThingDef;
            if ((storedThingDef != null || !settings.filter.Allows(storedThingAtInput)) &&
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

        // Token: 0x06000025 RID: 37 RVA: 0x00002AF8 File Offset: 0x00000CF8
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

        // Token: 0x06000026 RID: 38 RVA: 0x00002B4C File Offset: 0x00000D4C
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

        // Token: 0x06000027 RID: 39 RVA: 0x00002B9C File Offset: 0x00000D9C
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
}
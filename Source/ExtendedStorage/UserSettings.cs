using System;
using RimWorld;
using Verse;

namespace ExtendedStorage
{
    // Token: 0x0200000B RID: 11
    public class UserSettings : StorageSettings, IExposable
    {
        // Token: 0x0400000F RID: 15
        private static readonly Action<ThingFilter, Action> setThingFilterSettingsChangedCallback =
            Access.GetFieldSetter<ThingFilter, Action>("settingsChangedCallback");

        // Token: 0x04000010 RID: 16
        private readonly IUserSettingsOwner _owner;

        // Token: 0x0600003C RID: 60 RVA: 0x00002EAE File Offset: 0x000010AE
        public UserSettings(IUserSettingsOwner owner) : base(owner)
        {
            _owner = owner;
            FixupFilterChangeCallback();
        }

        // Token: 0x0600003F RID: 63 RVA: 0x00002EEF File Offset: 0x000010EF
        void IExposable.ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                FixupFilterChangeCallback();
            }
        }

        // Token: 0x0600003D RID: 61 RVA: 0x00002EC4 File Offset: 0x000010C4
        private void FixupFilterChangeCallback()
        {
            setThingFilterSettingsChangedCallback(filter, NotifyOwnerSettingsChanged);
        }

        // Token: 0x0600003E RID: 62 RVA: 0x00002EE2 File Offset: 0x000010E2
        public void NotifyOwnerSettingsChanged()
        {
            _owner.Notify_UserSettingsChanged();
        }
    }
}
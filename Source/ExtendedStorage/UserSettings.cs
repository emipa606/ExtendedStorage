using System;
using RimWorld;
using Verse;

namespace ExtendedStorage;

public class UserSettings : StorageSettings, IExposable
{
    private static readonly Action<ThingFilter, Action> setThingFilterSettingsChangedCallback =
        Access.GetFieldSetter<ThingFilter, Action>("settingsChangedCallback");

    private readonly IUserSettingsOwner _owner;

    public UserSettings(IUserSettingsOwner owner) : base(owner)
    {
        _owner = owner;
        FixupFilterChangeCallback();
    }

    void IExposable.ExposeData()
    {
        base.ExposeData();
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            FixupFilterChangeCallback();
        }
    }

    private void FixupFilterChangeCallback()
    {
        setThingFilterSettingsChangedCallback(filter, NotifyOwnerSettingsChanged);
    }

    public void NotifyOwnerSettingsChanged()
    {
        _owner.Notify_UserSettingsChanged();
    }
}
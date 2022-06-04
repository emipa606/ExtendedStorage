using RimWorld;

namespace ExtendedStorage;

public interface IUserSettingsOwner : IStoreSettingsParent
{
    void Notify_UserSettingsChanged();
}
using RimWorld;

namespace ExtendedStorage
{
    // Token: 0x02000003 RID: 3
    public interface IUserSettingsOwner : IStoreSettingsParent
    {
        // Token: 0x06000002 RID: 2
        void Notify_UserSettingsChanged();
    }
}
using ExtendedStorage.Patches;
using RimWorld;
using Verse;

namespace ExtendedStorage
{
    // Token: 0x02000006 RID: 6
    public class ITab_ExtendedStorage : ITab_Storage
    {
        // Token: 0x1700000A RID: 10
        // (get) Token: 0x06000032 RID: 50 RVA: 0x00002E27 File Offset: 0x00001027
        public Building_ExtendedStorage Building => SelThing as Building_ExtendedStorage;

        // Token: 0x1700000B RID: 11
        // (get) Token: 0x06000033 RID: 51 RVA: 0x00002E34 File Offset: 0x00001034
        protected override IStoreSettingsParent SelStoreSettingsParent
        {
            get
            {
                if (DebugSettings.godMode && ITab_Storage_FillTab.showStoreSettings)
                {
                    return base.SelStoreSettingsParent;
                }

                return Building;
            }
        }
    }
}
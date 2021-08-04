using Verse;

namespace ExtendedStorage
{
    // Token: 0x0200000A RID: 10
    public class ES_Dialog_Rename : Dialog_Rename
    {
        // Token: 0x0400000E RID: 14
        private readonly Building_ExtendedStorage building;

        // Token: 0x0600003A RID: 58 RVA: 0x00002E91 File Offset: 0x00001091
        public ES_Dialog_Rename(Building_ExtendedStorage building)
        {
            this.building = building;
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00002EA0 File Offset: 0x000010A0
        protected override void SetName(string name)
        {
            building.label = name;
        }
    }
}
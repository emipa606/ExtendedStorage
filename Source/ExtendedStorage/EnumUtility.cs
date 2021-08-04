using System;
using System.Collections.Generic;

namespace ExtendedStorage
{
    // Token: 0x0200000D RID: 13
    internal class EnumUtility
    {
        // Token: 0x06000047 RID: 71 RVA: 0x00003211 File Offset: 0x00001411
        public static bool TryGetNext<T>(IEnumerator<T> e, Predicate<T> predicate, out T value)
        {
            while (e.MoveNext())
            {
                var t = e.Current;
                value = t;
                if (predicate(value))
                {
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
using System;
using System.Collections.Generic;

namespace ExtendedStorage;

internal class EnumUtility
{
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
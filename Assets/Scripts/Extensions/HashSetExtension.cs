using System.Collections.Generic;

public static class HashSetExtension
{
    public static T Add<T>(this HashSet<T> set, T add)
    {
        if (set.Add(add))
            return add;

        throw new System.Exception("HashSetExtension method Add<T> did <b>not</b> successfully add " + add.ToString() + " to " + set.ToString());
    }
}
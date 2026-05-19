using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static bool Approximately(this Matrix4x4 a, Matrix4x4 b, float eps)
    {
        for (var i = 0; i < 16; i++)
            if (Mathf.Abs(a[i] - b[i]) > eps)
                return false;
        return true;
    }

    public static bool ExistIn(this Matrix4x4 a, IEnumerable<Matrix4x4> space, float eps)
    {
        foreach (var s in space)
        {
            if (s.Approximately(a, eps))
                return true;
        }
        return false;
    }
}

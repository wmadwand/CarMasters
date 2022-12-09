using UnityEngine;

public static class LayerMaskExtensions
{
    public static int ToSingleLayer(this LayerMask mask)
    {
        int value = mask.value;
        if (value == 0) return 0;  // Early out
        for (int l = 1; l < 32; l++)
            if ((value & (1 << l)) != 0) return l;  // Bitwise
        return -1;  // This line won't ever be reached but the compiler needs it
    }
}
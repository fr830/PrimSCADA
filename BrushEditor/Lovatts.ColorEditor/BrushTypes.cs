using System;

namespace BrushEditor
{
    [Flags]
    public enum BrushTypes
    {
        Solid = 1,
        Linear = 2,
        Radial = 4
    }
}
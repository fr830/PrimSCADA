// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
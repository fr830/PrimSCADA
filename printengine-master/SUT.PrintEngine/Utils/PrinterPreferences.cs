// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;

namespace SUT.PrintEngine.Utils
{
    [Serializable]
    public class PrinterPreferences
    {
        public string PrinterName { get; set; }
        public bool IsMarkPageNumbers { get; set; }
    }
}

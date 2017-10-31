// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.Collections.Generic;
using SUT.PrintEngine.Utils;

namespace SUT.PrintEngine.ViewModels
{
    public interface IItemsPrintControlViewModel : IPrintControlViewModel
    {
        List<double> ColumnsWidths { get; set; }
        List<double> RowHeights { get; set; }
        PrintTableDefination PrintTableDefination { get; set; }
    }
}
// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SUT.PrintEngine.Utils
{
    public class UiUtil
    {
        public static void UpdateSize(FrameworkElement element, double availableWidth)
        {
            var vbox = new Viewbox { Child = element };
            vbox.Measure(new Size(availableWidth, 2000));
            vbox.Arrange(new Rect(0, 0, availableWidth, 2000));
            vbox.UpdateLayout();
        }
    }
}

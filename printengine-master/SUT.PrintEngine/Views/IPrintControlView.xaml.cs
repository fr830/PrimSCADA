// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.Windows.Controls;
using System.Windows.Media;

namespace SUT.PrintEngine.Views
{
    public interface IPrintControlView : IView
    {
        DocumentViewer DocumentViewer { get; }
        void PrintingOptionsWaitCurtainVisibility(bool b);
        StackPanel GetPagePreviewContainer();
        void ScalePreviewNode(ScaleTransform scaleTransform);
    }
}
// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Drawing.Printing;
using System.Printing;
using System.Windows.Documents;

namespace SUT.PrintEngine.ViewModels
{
    public interface IFlowDocumentPrintControlViewModel : IViewModel
    {
        PrintQueue CurrentPrinter { get; set; }
        string CurrentPrinterName { get; set; }
        void ReloadPreview(PageOrientation pageOrientation, PaperSize paperSize);
        void ReloadPreview();
        void InitializeProperties();
        Int32 NumberOfPages { get; set; }
        void ShowPrintPreview(FlowDocument flowDocument);
    }
}
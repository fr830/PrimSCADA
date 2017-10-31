// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.Windows;
using System.Windows.Media;
using Microsoft.Practices.Unity;
using SUT.PrintEngine.Paginators;
using SUT.PrintEngine.Views;

namespace SUT.PrintEngine.ViewModels
{
    public class DataTablePrintControlViewModel : ItemsPrintControlViewModel, IDataTablePrintControlViewModel
    {
        public DataTablePrintControlViewModel(PrintControlView view, IUnityContainer unityContainer)
            : base(view, unityContainer)
        {
        }

        protected override void CreatePaginator(DrawingVisual visual, Size printSize)
        {
            Paginator = new DataTablePaginator(visual, printSize, PrintUtility.GetPageMargin(CurrentPrinterName), PrintTableDefination);
        }
    }
}
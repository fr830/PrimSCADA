// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.Windows.Controls;
using SUT.PrintEngine.ViewModels;

namespace SUT.PrintEngine.Controls.ProgressDialog
{
    /// <summary>
    /// Interaction logic for ProgressDialogView.xaml
    /// </summary>
    public partial class ProgressDialogView : UserControl, IProgressDialogView
    {
        public ProgressDialogView()
        {
            InitializeComponent();
        }

        private IProgressDialogViewModel _presenter;
        public IViewModel ViewModel
        {
            set
            {
                _presenter = value as IProgressDialogViewModel;
                DataContext = _presenter;
            }
        }
    }
}
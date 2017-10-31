using System.Windows.Controls;
using BrushEditor;
using System.Windows;

namespace Lovatts.ColorEditor
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class BrushEditor : UserControl
    {
        private BrushEditorViewModel _brushEditorViewModel;
        public Window ParentWindow;

        public BrushEditor()
        {
            InitializeComponent();
            
            BrushEditorViewModel = new BrushEditorViewModel();
            BrushEditorViewModel.ColorEditorViewModel = colorEditor.ColorEditorViewModel;            
        }

        public BrushEditorViewModel BrushEditorViewModel
        {
            get { return _brushEditorViewModel; }
            set { 
                _brushEditorViewModel = value;

                this.DataContext = _brushEditorViewModel;

                // Should probably implement INotifyPropertyChanged
            }
        }



        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ParentWindow.DialogResult = true;
            ParentWindow.Close();
        }
    }
}
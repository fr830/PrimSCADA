using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SCADA
{
    /// <summary>
    /// Interaction logic for WindowUpdate.xaml
    /// </summary>
    public partial class WindowUpdate : Window
    {
        public double VersionNew = 0;

        public WindowUpdate()
        {
            InitializeComponent();
        }

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            LVersionNew.Content = VersionNew;
        }

        private void BClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            e.Handled = true;
        }
        
        private void BDownload_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}

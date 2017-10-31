// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for DialogWindowAbout.xaml
    /// </summary>
    public partial class DialogWindowAbout : Window
    {
        public DialogWindowAbout()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Hyperlink_MailTo(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void MenuItem_CopyMailAbout(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("admin@primscada.com");
            e.Handled = true;
        }

        private void MenuItem_CopySiteAbout(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("www.primscada.com");
            e.Handled = true;
        }

        private void Hyperlink_Site(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo("www.primscada.com"));
            e.Handled = true;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            TextBlockVersion.Text = "Version " + ((MainWindow)((AppWPF)Application.Current).MainWindow).Version;
        }
    }
}
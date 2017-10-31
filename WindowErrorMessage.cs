using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCADA
{   
    public class WindowErrorMessage : Control
    {
        Button BClose;
        public ListBox LBMessageError;

        static WindowErrorMessage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowErrorMessage), new FrameworkPropertyMetadata(typeof(WindowErrorMessage)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BClose = GetTemplateChild("BClose") as Button;
            BClose.Click += BClose_Click;

            LBMessageError = GetTemplateChild("LBMessageError") as ListBox;
        }

        void BClose_Click(object sender, RoutedEventArgs e)
        {
            if (((MainWindow)Application.Current.MainWindow).WindowErrorMessages.IsVisible)
            {
                ((MainWindow)Application.Current.MainWindow).WindowErrorMessages.Visibility = System.Windows.Visibility.Collapsed;

                ((AppWPF)Application.Current).ConfigProgramBin.IsWindowErrorMessage = false;
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).WindowErrorMessages.Visibility = System.Windows.Visibility.Visible;

                ((AppWPF)Application.Current).ConfigProgramBin.IsWindowErrorMessage = true;
            }
          
            e.Handled = true;
        }
    }
}

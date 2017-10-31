using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для DialogWindowPropertiesComControl.xaml
    /// </summary>
    public partial class DialogWindowPropertiesComControl : Window
    {
        GridPropertiesComGeneral PropertiesGeneral;
        Popup Message = new Popup();

        string OldComPort;
        string OldStopBits;
        string OldDescription;
        string OldParity;
        int OldBaudRate;
        int OldReadTimeout;
        int OldWriteTimeout;
        int OldDataBits;

        ComControl ComControl;

        public DialogWindowPropertiesComControl(ComControl comControl)
        {
            InitializeComponent();

            Label l = new Label();
            l.BorderBrush = Brushes.Red;
            l.BorderThickness = new Thickness(3);
            l.Background = Brushes.White;

            Message.Child = l;

            OldComPort = comControl.ComSer.ComPort;
            OldStopBits = comControl.ComSer.StopBits;
            OldDescription = comControl.ComSer.Description;
            OldParity = comControl.ComSer.Parity;
            OldBaudRate = comControl.ComSer.BaudRate;
            OldReadTimeout = comControl.ComSer.ReadTimeout;
            OldWriteTimeout = comControl.ComSer.WriteTimeout;
            OldDataBits = comControl.ComSer.DataBits;

            ComControl = comControl;

            PropertiesGeneral = new GridPropertiesComGeneral(comControl);
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            int readTimeout;
            int writeTimeout;

            int.TryParse(PropertiesGeneral.TBReadTimeout.Text, out readTimeout);
            int.TryParse(PropertiesGeneral.TBWriteTimeout.Text, out writeTimeout);

            if (PropertiesGeneral.TBDescriptionCom.Text.Length > 150)
            {
                PropertiesGeneral.LPopupMessage.Content = "Описание не может быть длинее 150 символов.";

                PropertiesGeneral.PopupMessage.PlacementTarget = PropertiesGeneral.TBDescriptionCom;
                PropertiesGeneral.PopupMessage.IsOpen = true;

                PropertiesGeneral.TBDescriptionCom.SelectAll();
                PropertiesGeneral.TBDescriptionCom.Focus();

                return;
            }

            if (readTimeout < 50 || readTimeout > 5000)
            {
                PropertiesGeneral.LPopupMessage.Content = "Диапазон 50 - 5000";

                PropertiesGeneral.PopupMessage.PlacementTarget = PropertiesGeneral.TBReadTimeout;
                PropertiesGeneral.PopupMessage.IsOpen = true;

                PropertiesGeneral.TBReadTimeout.SelectAll();
                PropertiesGeneral.TBReadTimeout.Focus();

                return;
            }

            if (writeTimeout < 50 || writeTimeout > 5000)
            {
                PropertiesGeneral.LPopupMessage.Content = "Диапазон 50 - 5000";

                PropertiesGeneral.PopupMessage.PlacementTarget = PropertiesGeneral.TBWriteTimeout;
                PropertiesGeneral.PopupMessage.IsOpen = true;

                PropertiesGeneral.TBWriteTimeout.SelectAll();
                PropertiesGeneral.TBWriteTimeout.Focus();

                return;
            }
            
            if (OldComPort != PropertiesGeneral.CBPortName.SelectedItem || OldStopBits != PropertiesGeneral.CBStopBits.SelectedItem || OldDescription != PropertiesGeneral.TBDescriptionCom.Text
                || OldParity != PropertiesGeneral.CBParity.SelectedItem || OldBaudRate != (int)PropertiesGeneral.CBBaudRate.SelectedItem
                || OldReadTimeout != readTimeout || OldWriteTimeout != writeTimeout || OldDataBits != (int)PropertiesGeneral.CBDataBits.SelectedItem)
            {
                ComControl.ComSer.ComPort = (string)PropertiesGeneral.CBPortName.SelectedItem;
                ComControl.ComSer.StopBits = (string)PropertiesGeneral.CBStopBits.SelectedItem;
                ComControl.ComSer.Description = PropertiesGeneral.TBDescriptionCom.Text;
                ComControl.ComSer.Parity = (string)PropertiesGeneral.CBParity.SelectedItem;
                ComControl.ComSer.BaudRate = (int)PropertiesGeneral.CBBaudRate.SelectedItem;
                ComControl.ComSer.ReadTimeout = readTimeout;
                ComControl.ComSer.WriteTimeout = writeTimeout;
                ComControl.ComSer.WriteTimeout = writeTimeout;
                ComControl.ComSer.DataBits = (int)PropertiesGeneral.CBDataBits.SelectedItem;

                ((AppWPF)Application.Current).SaveTabItem(ComControl.CanvasTab.TabItemParent);
            }

            this.Close();
        }       

        private void TreeViewProperties_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem Selected = (TreeViewItem)e.NewValue;
            if ((string)Selected.Header == "Общие")
            {
                PropertiesGeneral.SetValue(Grid.ColumnProperty, 1);

                PropertiesGrid.Children.Add(PropertiesGeneral);
            }

            e.Handled = true;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            TreeViewItemGeneral.IsSelected = true;
            e.Handled = true;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.Close();
        }
    }
}

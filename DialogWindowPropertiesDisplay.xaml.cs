// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
using System.Windows.Shapes;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для DialogWindowPropertiesDisplay.xaml
    /// </summary>
    public partial class DialogWindowPropertiesDisplay : Window
    {
        GridPropertiesDisplayEthernet PropertiesEthernet;

        GridPropertiesDisplayModbus PropertiesModbus;
       
        private Display display;
        public Display Display
        {
            get { return display; }
            set { display = value; }
        }

        public DialogWindowPropertiesDisplay(Display display)
        {
            InitializeComponent();

            PropertiesEthernet = new GridPropertiesDisplayEthernet(display);

            PropertiesModbus = new GridPropertiesDisplayModbus(display);

            Display = display;              
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            TreeViewItem Selected = (TreeViewItem)TreeViewProperties.SelectedItem;

            if ((string)Selected.Header == "Ethernet")
            {
                if (!Display.DisplaySer.IsEthernet)
                {
                    Display.DisplaySer.IsModbus = false;
                    Display.DisplaySer.IsEthernet = true;

                    Display.DisplaySer.ModbusSearch = null;
                    Display.DisplaySer.ItemModbusSearch = null;

                    ((AppWPF)Application.Current).SaveTabItem(Display.CanvasTab.TabItemParent);
                }

                if (Display.DisplaySer.ItemNetSearch == null && PropertiesEthernet.TVItemNet.ItemNetSearch != null)
                {
                    Binding bindingRun = new Binding();
                    bindingRun.Converter = new ValueItemNetConverter(PropertiesEthernet.TVItemNet.ItemNetSearch);
                    bindingRun.Source = PropertiesEthernet.TVItemNet.ItemNetSearch;
                    bindingRun.Path = new PropertyPath("Value");

                    Display.RunBinding.SetBinding(Run.TextProperty, bindingRun);
                    Display.DisplaySer.ItemNetSearch = PropertiesEthernet.TVItemNet.ItemNetSearch;                    
                    Display.DisplaySer.EthernetSearch = PropertiesEthernet.TVItemNet.EthernetSearch;
                                                                                                           
                    ((AppWPF)Application.Current).SaveTabItem(Display.CanvasTab.TabItemParent);
                }
                else if (Display.DisplaySer.ItemNetSearch != null && PropertiesEthernet.TVItemNet.ItemNetSearch == null)
                {
                    Display.DisplaySer.ItemNetSearch = null;
                    Display.DisplaySer.EthernetSearch = null;

                    ((AppWPF)Application.Current).SaveTabItem(Display.CanvasTab.TabItemParent);
                }
                else if (!object.ReferenceEquals(Display.DisplaySer.ItemNetSearch, PropertiesEthernet.TVItemNet.ItemNetSearch))
                {                    
                    Binding bindingRun = new Binding();
                    bindingRun.Converter = new ValueItemNetConverter(PropertiesEthernet.TVItemNet.ItemNetSearch);
                    bindingRun.Source = PropertiesEthernet.TVItemNet.ItemNetSearch;
                    bindingRun.Path = new PropertyPath("Value");

                    Display.RunBinding.SetBinding(Run.TextProperty, bindingRun);

                    Display.DisplaySer.ItemNetSearch = PropertiesEthernet.TVItemNet.ItemNetSearch;
                    
                    
                        Display.DisplaySer.EthernetSearch = PropertiesEthernet.TVItemNet.EthernetSearch;
                                        

                    ((AppWPF)Application.Current).SaveTabItem(Display.CanvasTab.TabItemParent);
                } 
            }
            else if ((string)Selected.Header == "Modbus")
            {
                if (!Display.DisplaySer.IsModbus)
                {
                    Display.DisplaySer.IsModbus = true;
                    Display.DisplaySer.IsEthernet = false;

                    Display.DisplaySer.EthernetSearch = null;
                    Display.DisplaySer.ItemNetSearch = null;

                    ((AppWPF)Application.Current).SaveTabItem(Display.CanvasTab.TabItemParent);
                }

                if (Display.DisplaySer.ItemModbusSearch == null && PropertiesModbus.ItemModbusSearch != null)
                {
                    Binding bindingRun = new Binding();
                    bindingRun.Converter = new ValueItemModbusConverter(PropertiesModbus.ItemModbusSearch);
                    bindingRun.Source = PropertiesModbus.ItemModbusSearch;
                    bindingRun.Path = new PropertyPath("Value");

                    Display.RunBinding.SetBinding(Run.TextProperty, bindingRun);
                    Display.DisplaySer.ItemModbusSearch = PropertiesModbus.ItemModbusSearch;         
                    Display.DisplaySer.ModbusSearch = PropertiesModbus.ModbusSearch;
                                     
                    ((AppWPF)Application.Current).SaveTabItem(Display.CanvasTab.TabItemParent);
                }
                else if (Display.DisplaySer.ItemModbusSearch != null && PropertiesModbus.ItemModbusSearch == null)
                {
                    Display.DisplaySer.ItemModbusSearch = null;
                    Display.DisplaySer.ModbusSearch = null;

                    ((AppWPF)Application.Current).SaveTabItem(Display.CanvasTab.TabItemParent);
                }
                else if (!object.ReferenceEquals(Display.DisplaySer.ItemModbusSearch, PropertiesModbus.ItemModbusSearch))
                {                    
                    Binding bindingRun = new Binding();
                    bindingRun.Converter = new ValueItemModbusConverter(PropertiesModbus.ItemModbusSearch);
                    bindingRun.Source = PropertiesModbus.ItemModbusSearch;
                    bindingRun.Path = new PropertyPath("Value");

                    Display.RunBinding.SetBinding(Run.TextProperty, bindingRun);
                    Display.DisplaySer.ItemModbusSearch = PropertiesModbus.ItemModbusSearch;                                      
                    Display.DisplaySer.ModbusSearch = PropertiesModbus.ModbusSearch;
                    
                    ((AppWPF)Application.Current).SaveTabItem(Display.CanvasTab.TabItemParent);
                }
            }        
                                                                 
            e.Handled = true;
            this.Close();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.Close();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (display.DisplaySer.IsModbus)
            {
                TreeViewItemModbus.IsSelected = true;
            }
            else if (display.DisplaySer.IsEthernet)
            {
                TreeViewItemEthernet.IsSelected = true;
            }
            else
            {
                TreeViewItemEthernet.IsSelected = true;
            }
            
            e.Handled = true;
        }
     
        private void TreeViewProperties_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem Selected = (TreeViewItem)e.NewValue;
            if ((string)Selected.Header == "Ethernet")
            {
                if (PropertiesGrid.Children.Contains(PropertiesModbus))
                {
                    PropertiesGrid.Children.Remove(PropertiesModbus);
                }

                if (PropertiesGrid.Children.Contains(PropertiesEthernet))
                {
                    PropertiesGrid.Children.Remove(PropertiesEthernet);
                }

                PropertiesEthernet.SetValue(Grid.ColumnProperty, 1);

                PropertiesGrid.Children.Add(PropertiesEthernet);
            }
            else if ((string)Selected.Header == "Modbus")
            {
                if (PropertiesGrid.Children.Contains(PropertiesModbus))
                {
                    PropertiesGrid.Children.Remove(PropertiesModbus);
                }

                if (PropertiesGrid.Children.Contains(PropertiesEthernet))
                {
                    PropertiesGrid.Children.Remove(PropertiesEthernet);
                }

                PropertiesModbus.SetValue(Grid.ColumnProperty, 1);

                PropertiesGrid.Children.Add(PropertiesModbus);
            }

            e.Handled = true;
        }       
    }
}

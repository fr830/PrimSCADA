using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Логика взаимодействия для DialogWindowPropertiesModbusControl.xaml
    /// </summary>
    public partial class DialogWindowPropertiesModbusControl : Window
    {
        GridPropertiesModbusGeneral GridPropertiesModbusGeneral;

        Popup Message = new Popup();
      
        ModbusControl ModbusControl;
      
        public DialogWindowPropertiesModbusControl(ModbusControl modbus)
        {
            InitializeComponent();

            GridPropertiesModbusGeneral = new GridPropertiesModbusGeneral(modbus);

            Label l = new Label();
            l.BorderBrush = Brushes.Red;
            l.BorderThickness = new Thickness(3);
            l.Background = Brushes.White;

            Message.Child = l;

            ModbusControl = modbus;
        }

        private void TreeViewProperties_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem Selected = (TreeViewItem)e.NewValue;
            if ((string)Selected.Header == "Общие")
            {
                GridPropertiesModbusGeneral.SetValue(Grid.ColumnProperty, 1);

                PropertiesGrid.Children.Add(GridPropertiesModbusGeneral);
            }

            e.Handled = true;
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            byte slaveAddress;
            int time;            

            e.Handled = true;
                    
            if (byte.TryParse(GridPropertiesModbusGeneral.TBAddressSlave.Text, out slaveAddress))
            {
                if (slaveAddress < 1 || slaveAddress > 255)
                {
                    GridPropertiesModbusGeneral.LPopupMessage.Content = StaticValue.SRange1_255;

                    GridPropertiesModbusGeneral.PopupMessage.PlacementTarget = GridPropertiesModbusGeneral.TBAddressSlave;
                    GridPropertiesModbusGeneral.PopupMessage.IsOpen = true;

                    return;
                }
            }
            else
            {
                GridPropertiesModbusGeneral.LPopupMessage.Content = StaticValue.SRange1_255;

                GridPropertiesModbusGeneral.PopupMessage.PlacementTarget = GridPropertiesModbusGeneral.TBAddressSlave;
                GridPropertiesModbusGeneral.PopupMessage.IsOpen = true;

                return;
            }

            if (int.TryParse(GridPropertiesModbusGeneral.TBTime.Text, out time))
            {
                if (time < 1 || time > 86400)
                {
                    GridPropertiesModbusGeneral.LPopupMessage.Content = StaticValue.SRange1_86400;

                    GridPropertiesModbusGeneral.PopupMessage.PlacementTarget = GridPropertiesModbusGeneral.TBTime;
                    GridPropertiesModbusGeneral.PopupMessage.IsOpen = true;

                    return;
                }               
            }
            else
            {
                GridPropertiesModbusGeneral.LPopupMessage.Content = StaticValue.SRange1_86400;

                GridPropertiesModbusGeneral.PopupMessage.PlacementTarget = GridPropertiesModbusGeneral.TBTime;
                GridPropertiesModbusGeneral.PopupMessage.IsOpen = true;

                return;
            }
           
            if (ModbusControl.ModbusSer != GridPropertiesModbusGeneral.NewModbusSer)
            {
                foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                {
                    foreach (DisplaySer displaySer in page.CollectionDisplay)
                    {
                        if (displaySer.IsModbus)
                        {
                            if (displaySer.ModbusSearch == ModbusControl.ModbusSer.ID)
                            {
                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                            }
                        }
                    }
                }

                ModbusControl.ModbusSer.ComPort = GridPropertiesModbusGeneral.NewModbusSer.ComPort;
                ModbusControl.ModbusSer.Description = GridPropertiesModbusGeneral.NewModbusSer.Description;
                ModbusControl.ModbusSer.Protocol = GridPropertiesModbusGeneral.NewModbusSer.Protocol;
                ModbusControl.ModbusSer.SlaveAddress = GridPropertiesModbusGeneral.NewModbusSer.SlaveAddress;
                ModbusControl.ModbusSer.Time = GridPropertiesModbusGeneral.NewModbusSer.Time;
                ModbusControl.ModbusSer.ReverseRegister = GridPropertiesModbusGeneral.NewModbusSer.ReverseRegister;
                ModbusControl.ModbusSer.IsUS800 = GridPropertiesModbusGeneral.NewModbusSer.IsUS800;

                ((AppWPF)Application.Current).SaveTabItem(ModbusControl.CanvasTab.TabItemParent);               
            }

            if (GridPropertiesModbusGeneral.NewModbusSer.CollectionItemModbus.Count != ModbusControl.ModbusSer.CollectionItemModbus.Count)
            {
                int count = 0;

                if (ModbusControl.ModbusSer.CollectionItemModbus.Count > GridPropertiesModbusGeneral.NewModbusSer.CollectionItemModbus.Count)
                {
                    if (GridPropertiesModbusGeneral.NewModbusSer.CollectionItemModbus.Count == 0)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus.Clear();
                    }
                    else
                    {
                        foreach (ItemModbus itemNew in GridPropertiesModbusGeneral.NewModbusSer.CollectionItemModbus)
                        {
                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                            {
                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                {
                                    if (displaySer.IsModbus)
                                    {
                                        if (object.ReferenceEquals(displaySer.ItemModbusSearch, ModbusControl.ModbusSer.CollectionItemModbus[count]))
                                        {
                                            if (displaySer.ItemModbusSearch != itemNew)
                                            {
                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                            }
                                        }
                                    }
                                }
                            }
                                                   
                            count += 1;
                        }

                        ModbusControl.ModbusSer.CollectionItemModbus = GridPropertiesModbusGeneral.NewModbusSer.CollectionItemModbus;
                    }
                }
                else if (ModbusControl.ModbusSer.CollectionItemModbus.Count < GridPropertiesModbusGeneral.NewModbusSer.CollectionItemModbus.Count)
                {
                    if (ModbusControl.ModbusSer.CollectionItemModbus.Count == 0)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus = GridPropertiesModbusGeneral.NewModbusSer.CollectionItemModbus;
                    }
                    else
                    {
                        foreach (ItemModbus itemNew in ModbusControl.ModbusSer.CollectionItemModbus)
                        {                           
                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                            {
                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                {
                                    if (displaySer.IsModbus)
                                    {
                                        if (object.ReferenceEquals(displaySer.ItemModbusSearch, ModbusControl.ModbusSer.CollectionItemModbus[count]))
                                        {
                                            if (displaySer.ItemModbusSearch != itemNew)
                                            {
                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                            }
                                        }
                                    }
                                }
                            }
                            
                            count += 1;
                        }

                        ModbusControl.ModbusSer.CollectionItemModbus = GridPropertiesModbusGeneral.NewModbusSer.CollectionItemModbus;
                    }
                }

                ((AppWPF)Application.Current).SaveTabItem(ModbusControl.CanvasTab.TabItemParent);
            }
            else
            {
                int count = 0;
                bool isSave = false;

                foreach (ItemModbus itemNew in GridPropertiesModbusGeneral.NewModbusSer.CollectionItemModbus)
                {                   
                    foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                    {
                        foreach (DisplaySer displaySer in page.CollectionDisplay)
                        {
                            if (displaySer.IsModbus)
                            {
                                if (object.ReferenceEquals(displaySer.ItemModbusSearch, ModbusControl.ModbusSer.CollectionItemModbus[count]))
                                {
                                    if (displaySer.ItemModbusSearch != itemNew)
                                    {
                                        ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                    }
                                }
                            }                            
                        }
                    }

                    if (ModbusControl.ModbusSer.CollectionItemModbus[count].Description != itemNew.Description)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].Description = itemNew.Description;
                        isSave = true;
                    }

                    if (ModbusControl.ModbusSer.CollectionItemModbus[count].Function != itemNew.Function)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].Function = itemNew.Function;
                        isSave = true;
                    }

                    if (ModbusControl.ModbusSer.CollectionItemModbus[count].IsSaveDatabase != itemNew.IsSaveDatabase)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].IsSaveDatabase = itemNew.IsSaveDatabase;
                        isSave = true;
                    }

                    if (ModbusControl.ModbusSer.CollectionItemModbus[count].Address != itemNew.Address)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].Address = itemNew.Address;
                        isSave = true;
                    }

                    if (ModbusControl.ModbusSer.CollectionItemModbus[count].TypeValue != itemNew.TypeValue)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].TypeValue = itemNew.TypeValue;
                        isSave = true;
                    }

                    if (ModbusControl.ModbusSer.CollectionItemModbus[count].TableName != itemNew.TableName)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].TableName = itemNew.TableName;
                        isSave = true;
                    }

                    if (ModbusControl.ModbusSer.CollectionItemModbus[count].PeridTimeSaveDB != itemNew.PeridTimeSaveDB)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].PeridTimeSaveDB = itemNew.PeridTimeSaveDB;
                        isSave = true;
                    }

                    if (ModbusControl.ModbusSer.CollectionItemModbus[count].Formula != itemNew.Formula)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].Formula = itemNew.Formula;
                        isSave = true;
                    }

                    if (ModbusControl.ModbusSer.CollectionItemModbus[count].Text != itemNew.Text)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].Text = itemNew.Text;
                        isSave = true;
                    }

                    if (Convert.ToSingle(ModbusControl.ModbusSer.CollectionItemModbus[count].EmergencyUp, CultureInfo.InvariantCulture) != Convert.ToSingle(itemNew.EmergencyUp, CultureInfo.InvariantCulture))
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].EmergencyUp = itemNew.EmergencyUp;
                        isSave = true;
                    }

                    if (Convert.ToSingle(ModbusControl.ModbusSer.CollectionItemModbus[count].EmergencyDown, CultureInfo.InvariantCulture) != Convert.ToSingle(itemNew.EmergencyDown, CultureInfo.InvariantCulture))
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].EmergencyDown = itemNew.EmergencyDown;
                        isSave = true;
                    }

                    if (ModbusControl.ModbusSer.CollectionItemModbus[count].IsEmergencyUp != itemNew.IsEmergencyUp)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].IsEmergencyUp = itemNew.IsEmergencyUp;
                        isSave = true;
                    }

                    if (ModbusControl.ModbusSer.CollectionItemModbus[count].IsEmergencyDown != itemNew.IsEmergencyDown)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].IsEmergencyDown = itemNew.IsEmergencyDown;
                        isSave = true;
                    }

                    if (ModbusControl.ModbusSer.CollectionItemModbus[count].IsEmergencySaveDB != itemNew.IsEmergencySaveDB)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].IsEmergencySaveDB = itemNew.IsEmergencySaveDB;
                        isSave = true;
                    }

                    if (ModbusControl.ModbusSer.CollectionItemModbus[count].PeriodEmergencySaveDB != itemNew.PeriodEmergencySaveDB)
                    {
                        ModbusControl.ModbusSer.CollectionItemModbus[count].PeriodEmergencySaveDB = itemNew.PeriodEmergencySaveDB;
                        isSave = true;
                    }

                    count += 1;
                }

                if (isSave)
                {
                    ((AppWPF)Application.Current).SaveTabItem(ModbusControl.CanvasTab.TabItemParent);
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
            TreeViewItemGeneral.IsSelected = true;
            e.Handled = true;
        }
    }
}

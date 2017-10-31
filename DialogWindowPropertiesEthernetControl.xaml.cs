// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Windows.Threading;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для DialogWindowEthernetControlProperies.xaml
    /// </summary>
    public partial class DialogWindowPropertiesEthernetControl : Window
    {
        GridPropertiesEthernetGeneral PropertiesGeneral;
        Popup Message = new Popup();

        EthernetControl EthernetControl;

        public DialogWindowPropertiesEthernetControl(EthernetControl ethernetControl)
        {
            InitializeComponent();

            Label l = new Label();
            l.BorderBrush = Brushes.Red;
            l.BorderThickness = new Thickness(3);
            l.Background = Brushes.White;

            Message.Child = l;

            PropertiesGeneral = new GridPropertiesEthernetGeneral(ethernetControl);
                                  
            EthernetControl = ethernetControl;
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            byte[] IPclient = null;

            if (PropertiesGeneral.CBLocalIPs.SelectedItem != null)
            {
                IPclient = ((IPAddress)PropertiesGeneral.CBLocalIPs.SelectedItem).GetAddressBytes();
            }
            else
            {
                IPclient = new byte[4];
            }

            PropertiesGeneral.NewEthernetSer.IPAddressClient = IPclient;

            if (EthernetControl.EthernetSer != PropertiesGeneral.NewEthernetSer)
            {
                EthernetControl.EthernetSer.BufferSizeRec = PropertiesGeneral.NewEthernetSer.BufferSizeRec;
                EthernetControl.EthernetSer.EthernetProtocol = PropertiesGeneral.NewEthernetSer.EthernetProtocol;
                EthernetControl.EthernetSer.Time = PropertiesGeneral.NewEthernetSer.Time;              
                EthernetControl.EthernetSer.IPAddressServer[0] = PropertiesGeneral.NewEthernetSer.IPAddressServer[0];
                EthernetControl.EthernetSer.IPAddressServer[1] = PropertiesGeneral.NewEthernetSer.IPAddressServer[1];
                EthernetControl.EthernetSer.IPAddressServer[2] = PropertiesGeneral.NewEthernetSer.IPAddressServer[2];
                EthernetControl.EthernetSer.IPAddressServer[3] = PropertiesGeneral.NewEthernetSer.IPAddressServer[3];                
                EthernetControl.EthernetSer.Description = PropertiesGeneral.NewEthernetSer.Description;
                EthernetControl.EthernetSer.PortClient = PropertiesGeneral.NewEthernetSer.PortClient;
                EthernetControl.EthernetSer.PortServer = PropertiesGeneral.NewEthernetSer.PortServer;             
                EthernetControl.EthernetSer.IPAddressClient = IPclient;                

                ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
            }

            if (PropertiesGeneral.NewEthernetSer.CollectionItemNetRec.Count != EthernetControl.EthernetSer.CollectionItemNetRec.Count)
            {
                int count = 0;

                if (EthernetControl.EthernetSer.CollectionItemNetRec.Count > PropertiesGeneral.NewEthernetSer.CollectionItemNetRec.Count)
                {
                    if (PropertiesGeneral.NewEthernetSer.CollectionItemNetRec.Count == 0)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec.Clear();
                    }
                    else
                    {
                        foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionItemNetRec)
                        {
                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                            {
                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                {
                                    if (displaySer.IsEthernet)
                                    {
                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionItemNetRec[count]))
                                        {
                                            if (displaySer.ItemNetSearch != itemNew)
                                            {
                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                            }
                                        }
                                    }
                                }
                            }

                            count += 1;
                        }

                        EthernetControl.EthernetSer.CollectionItemNetRec = PropertiesGeneral.NewEthernetSer.CollectionItemNetRec;
                    }
                }
                else if (EthernetControl.EthernetSer.CollectionItemNetRec.Count < PropertiesGeneral.NewEthernetSer.CollectionItemNetRec.Count)
                {
                    if (EthernetControl.EthernetSer.CollectionItemNetRec.Count == 0)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec = PropertiesGeneral.NewEthernetSer.CollectionItemNetRec;
                    }
                    else
                    {
                        foreach (ItemNet itemNew in EthernetControl.EthernetSer.CollectionItemNetRec)
                        {
                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                            {
                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                {
                                    if (displaySer.IsEthernet)
                                    {
                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionItemNetRec[count]))
                                        {
                                            if (displaySer.ItemNetSearch != itemNew)
                                            {
                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                            }
                                        }
                                    }
                                }
                            }

                            count += 1;
                        }

                        EthernetControl.EthernetSer.CollectionItemNetRec = PropertiesGeneral.NewEthernetSer.CollectionItemNetRec;
                    }
                }

                ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
            }
            else
            {
                int count = 0;
                bool isSave = false;

                foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionItemNetRec)
                {
                    foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                    {
                        foreach (DisplaySer displaySer in page.CollectionDisplay)
                        {
                            if (displaySer.IsEthernet)
                            {
                                if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionItemNetRec[count]))
                                {
                                    if (displaySer.ItemNetSearch != itemNew)
                                    {
                                        ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                    }
                                }
                            }
                        }
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetRec[count].Description != itemNew.Description)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].Description = itemNew.Description;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetRec[count].Range0 != itemNew.Range0)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].Range0 = itemNew.Range0;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetRec[count].Range1 != itemNew.Range1)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].Range1 = itemNew.Range1;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetRec[count].TypeValue != itemNew.TypeValue)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].TypeValue = itemNew.TypeValue;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetRec[count].IsSaveDatabase != itemNew.IsSaveDatabase)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].IsSaveDatabase = itemNew.IsSaveDatabase;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetRec[count].TableName != itemNew.TableName)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].TableName = itemNew.TableName;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetRec[count].PeridTimeSaveDB != itemNew.PeridTimeSaveDB)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].PeridTimeSaveDB = itemNew.PeridTimeSaveDB;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetRec[count].Formula != itemNew.Formula)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].Formula = itemNew.Formula;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetRec[count].Text != itemNew.Text)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].Text = itemNew.Text;
                        isSave = true;
                    }

                    if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionItemNetRec[count].EmergencyDown) != Convert.ToSingle(itemNew.EmergencyDown))
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].EmergencyDown = itemNew.EmergencyDown;
                        isSave = true;
                    }

                    if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionItemNetRec[count].EmergencyUp) != Convert.ToSingle(itemNew.EmergencyUp))
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].EmergencyUp = itemNew.EmergencyUp;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetRec[count].IsEmergencyDown != itemNew.IsEmergencyDown)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].IsEmergencyDown = itemNew.IsEmergencyDown;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetRec[count].IsEmergencyUp != itemNew.IsEmergencyUp)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].IsEmergencyUp = itemNew.IsEmergencyUp;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetRec[count].IsEmergencySaveDB != itemNew.IsEmergencySaveDB)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].IsEmergencySaveDB = itemNew.IsEmergencySaveDB;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetRec[count].PeriodEmergencySaveDB != itemNew.PeriodEmergencySaveDB)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetRec[count].PeriodEmergencySaveDB = itemNew.PeriodEmergencySaveDB;
                        isSave = true;
                    }

                    count += 1;
                }

                if (isSave)
                {
                    ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                }
            }

            if (PropertiesGeneral.NewEthernetSer.CollectionItemNetSend.Count != EthernetControl.EthernetSer.CollectionItemNetSend.Count)
            {
                int count = 0;

                if (EthernetControl.EthernetSer.CollectionItemNetSend.Count > PropertiesGeneral.NewEthernetSer.CollectionItemNetSend.Count)
                {
                    if (PropertiesGeneral.NewEthernetSer.CollectionItemNetSend.Count == 0)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend.Clear();
                    }
                    else
                    {
                        foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionItemNetSend)
                        {
                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                            {
                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                {
                                    if (displaySer.IsEthernet)
                                    {
                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionItemNetSend[count]))
                                        {
                                            if (displaySer.ItemNetSearch != itemNew)
                                            {
                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                            }
                                        }
                                    }
                                }
                            }

                            count += 1;
                        }

                        EthernetControl.EthernetSer.CollectionItemNetSend = PropertiesGeneral.NewEthernetSer.CollectionItemNetSend;
                    }
                }
                else if (EthernetControl.EthernetSer.CollectionItemNetSend.Count < PropertiesGeneral.NewEthernetSer.CollectionItemNetSend.Count)
                {
                    if (EthernetControl.EthernetSer.CollectionItemNetSend.Count == 0)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend = PropertiesGeneral.NewEthernetSer.CollectionItemNetSend;
                    }
                    else
                    {
                        foreach (ItemNet itemNew in EthernetControl.EthernetSer.CollectionItemNetSend)
                        {
                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                            {
                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                {
                                    if (displaySer.IsEthernet)
                                    {
                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionItemNetSend[count]))
                                        {
                                            if (displaySer.ItemNetSearch != itemNew)
                                            {
                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                            }
                                        }
                                    }
                                }
                            }                           

                            count += 1;
                        }

                        EthernetControl.EthernetSer.CollectionItemNetSend = PropertiesGeneral.NewEthernetSer.CollectionItemNetSend;
                    }
                }

                ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
            }
            else
            {
                int count = 0;
                bool isSave = false;

                foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionItemNetSend)
                {
                    foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                    {
                        foreach (DisplaySer displaySer in page.CollectionDisplay)
                        {
                            if (displaySer.IsEthernet)
                            {
                                if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionItemNetSend[count]))
                                {
                                    if (displaySer.ItemNetSearch != itemNew)
                                    {
                                        ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                    }
                                }
                            }
                        }
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetSend[count].Description != itemNew.Description)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].Description = itemNew.Description;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetSend[count].Range0 != itemNew.Range0)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].Range0 = itemNew.Range0;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetSend[count].Range1 != itemNew.Range1)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].Range1 = itemNew.Range1;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetSend[count].TypeValue != itemNew.TypeValue)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].TypeValue = itemNew.TypeValue;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetSend[count].IsSaveDatabase != itemNew.IsSaveDatabase)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].IsSaveDatabase = itemNew.IsSaveDatabase;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetSend[count].TableName != itemNew.TableName)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].TableName = itemNew.TableName;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetSend[count].PeridTimeSaveDB != itemNew.PeridTimeSaveDB)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].PeridTimeSaveDB = itemNew.PeridTimeSaveDB;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetSend[count].Formula != itemNew.Formula)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].Formula = itemNew.Formula;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetSend[count].Text != itemNew.Text)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].Text = itemNew.Text;
                        isSave = true;
                    }

                    if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionItemNetSend[count].EmergencyDown) != Convert.ToSingle(itemNew.EmergencyDown))
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].EmergencyDown = itemNew.EmergencyDown;
                        isSave = true;
                    }

                    if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionItemNetSend[count].EmergencyUp) != Convert.ToSingle(itemNew.EmergencyUp))
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].EmergencyUp = itemNew.EmergencyUp;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetSend[count].IsEmergencyDown != itemNew.IsEmergencyDown)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].IsEmergencyDown = itemNew.IsEmergencyDown;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetSend[count].IsEmergencyUp != itemNew.IsEmergencyUp)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].IsEmergencyUp = itemNew.IsEmergencyUp;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetSend[count].IsEmergencySaveDB != itemNew.IsEmergencySaveDB)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].IsEmergencySaveDB = itemNew.IsEmergencySaveDB;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionItemNetSend[count].PeriodEmergencySaveDB != itemNew.PeriodEmergencySaveDB)
                    {
                        EthernetControl.EthernetSer.CollectionItemNetSend[count].PeriodEmergencySaveDB = itemNew.PeriodEmergencySaveDB;
                        isSave = true;
                    }

                    count += 1;
                }

                if (isSave)
                {
                    ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                }
            }
           
            if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational.Count != EthernetControl.EthernetSer.CollectionEthernetOperational.Count)
            {
                int count = 0;

                if (EthernetControl.EthernetSer.CollectionEthernetOperational.Count > PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational.Count)
                {
                    if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational.Count == 0)
                    {
                        EthernetControl.EthernetSer.CollectionEthernetOperational.Clear();
                    }
                    else
                    {
                        foreach (EthernetOperational eo in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational)
                        {
                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                            {
                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                {
                                    if (displaySer.IsEthernet)
                                    {
                                        if (object.ReferenceEquals(displaySer.EthernetOperationalSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].EthernetOperationalSearch))
                                        {
                                            if (displaySer.EthernetOperationalSearch != eo.EthernetOperationalSearch)
                                            {
                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                            }
                                        }
                                    }
                                }
                            }
                           
                            if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count != EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count)
                            {
                                int count2 = 0;

                                if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count > PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count)
                                {
                                    if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count == 0)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Clear();
                                    }
                                    else
                                    {
                                        foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec)
                                        {
                                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                            {
                                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                                {
                                                    if (displaySer.IsEthernet)
                                                    {
                                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2]))
                                                        {
                                                            if (displaySer.ItemNetSearch != itemNew)
                                                            {
                                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                            }
                                                        }
                                                    }
                                                }
                                            }                                                  

                                            count2 += 1;
                                        }

                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2] = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2];
                                    }
                                }
                                else if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count < PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count)
                                {
                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count == 0)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec;
                                    }
                                    else
                                    {
                                        foreach (ItemNet itemNew in EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec)
                                        {
                                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                            {
                                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                                {
                                                    if (displaySer.IsEthernet)
                                                    {
                                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2]))
                                                        {
                                                            if (displaySer.ItemNetSearch != itemNew)
                                                            {
                                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                           
                                            count2 += 1;
                                        }

                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2] = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2];
                                    }
                                }

                                ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                            }
                            else
                            {
                                int count2 = 0;
                                bool isSave = false;

                                foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec)
                                {
                                    foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                    {
                                        foreach (DisplaySer displaySer in page.CollectionDisplay)
                                        {
                                            if (displaySer.IsEthernet)
                                            {
                                                if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2]))
                                                {
                                                    if (displaySer.ItemNetSearch != itemNew)
                                                    {
                                                        ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Description != itemNew.Description)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Description = itemNew.Description;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Range0 != itemNew.Range0)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Range0 = itemNew.Range0;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Range1 != itemNew.Range1)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Range1 = itemNew.Range1;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].TypeValue != itemNew.TypeValue)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].TypeValue = itemNew.TypeValue;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsSaveDatabase != itemNew.IsSaveDatabase)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsSaveDatabase = itemNew.IsSaveDatabase;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].TableName != itemNew.TableName)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].TableName = itemNew.TableName;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].PeridTimeSaveDB != itemNew.PeridTimeSaveDB)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].PeridTimeSaveDB = itemNew.PeridTimeSaveDB;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Formula != itemNew.Formula)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Formula = itemNew.Formula;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Text != itemNew.Text)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Text = itemNew.Text;
                                        isSave = true;
                                    }

                                    if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].EmergencyDown) != Convert.ToSingle(itemNew.EmergencyDown))
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].EmergencyDown = itemNew.EmergencyDown;
                                        isSave = true;
                                    }

                                    if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].EmergencyUp) != Convert.ToSingle(itemNew.EmergencyUp))
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].EmergencyUp = itemNew.EmergencyUp;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencyDown != itemNew.IsEmergencyDown)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencyDown = itemNew.IsEmergencyDown;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencyUp != itemNew.IsEmergencyUp)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencyUp = itemNew.IsEmergencyUp;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencySaveDB != itemNew.IsEmergencySaveDB)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencySaveDB = itemNew.IsEmergencySaveDB;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].PeriodEmergencySaveDB != itemNew.PeriodEmergencySaveDB)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].PeriodEmergencySaveDB = itemNew.PeriodEmergencySaveDB;
                                        isSave = true;
                                    }

                                    count2 += 1;
                                }

                                if (isSave)
                                {                            
                                    ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                                }
                            }

                            if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count != EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count)
                            {
                                int count2 = 0;

                                if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count > PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count)
                                {
                                    if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count == 0)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Clear();
                                    }
                                    else
                                    {
                                        foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend)
                                        {
                                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                            {
                                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                                {
                                                    if (displaySer.IsEthernet)
                                                    {
                                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2]))
                                                        {
                                                            if (displaySer.ItemNetSearch != itemNew)
                                                            {
                                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                            }
                                                        }
                                                    }
                                                }
                                            }                                                   

                                            count2 += 1;
                                        }

                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2] = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2];
                                    }
                                }
                                else if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count < PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count)
                                {
                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count == 0)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend;
                                    }
                                    else
                                    {
                                        foreach (ItemNet itemNew in EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend)
                                        {
                                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                            {
                                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                                {
                                                    if (displaySer.IsEthernet)
                                                    {
                                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2]))
                                                        {
                                                            if (displaySer.ItemNetSearch != itemNew)
                                                            {
                                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                            }
                                                        }
                                                    }
                                                }
                                            }                                                 

                                            count2 += 1;
                                        }

                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2] = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2];
                                    }
                                }

                                ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                            }
                            else
                            {
                                int count2 = 0;
                                bool isSave = false;

                                foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend)
                                {
                                    foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                    {
                                        foreach (DisplaySer displaySer in page.CollectionDisplay)
                                        {
                                            if (displaySer.IsEthernet)
                                            {
                                                if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2]))
                                                {
                                                    if (displaySer.ItemNetSearch != itemNew)
                                                    {
                                                        ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Description != itemNew.Description)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Description = itemNew.Description;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Range0 != itemNew.Range0)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Range0 = itemNew.Range0;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Range1 != itemNew.Range1)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Range1 = itemNew.Range1;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].TypeValue != itemNew.TypeValue)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].TypeValue = itemNew.TypeValue;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsSaveDatabase != itemNew.IsSaveDatabase)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsSaveDatabase = itemNew.IsSaveDatabase;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].TableName != itemNew.TableName)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].TableName = itemNew.TableName;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].PeridTimeSaveDB != itemNew.PeridTimeSaveDB)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].PeridTimeSaveDB = itemNew.PeridTimeSaveDB;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Formula != itemNew.Formula)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Formula = itemNew.Formula;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Text != itemNew.Text)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Text = itemNew.Text;
                                        isSave = true;
                                    }

                                    if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].EmergencyDown) != Convert.ToSingle(itemNew.EmergencyDown))
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].EmergencyDown = itemNew.EmergencyDown;
                                        isSave = true;
                                    }

                                    if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].EmergencyUp) != Convert.ToSingle(itemNew.EmergencyUp))
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].EmergencyUp = itemNew.EmergencyUp;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencyDown != itemNew.IsEmergencyDown)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencyDown = itemNew.IsEmergencyDown;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencyUp != itemNew.IsEmergencyUp)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencyUp = itemNew.IsEmergencyUp;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencySaveDB != itemNew.IsEmergencySaveDB)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencySaveDB = itemNew.IsEmergencySaveDB;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].PeriodEmergencySaveDB != itemNew.PeriodEmergencySaveDB)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].PeriodEmergencySaveDB = itemNew.PeriodEmergencySaveDB;
                                        isSave = true;
                                    }

                                    count2 += 1;
                                }

                                if (isSave)
                                {                                 
                                    ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                                }
                            }

                            count += 1;
                        }

                        EthernetControl.EthernetSer.CollectionEthernetOperational = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational;
                    }
                }
                else if (EthernetControl.EthernetSer.CollectionEthernetOperational.Count < PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational.Count)
                {
                    if (EthernetControl.EthernetSer.CollectionEthernetOperational.Count == 0)
                    {
                        EthernetControl.EthernetSer.CollectionEthernetOperational = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational;
                    }
                    else
                    {
                        foreach (EthernetOperational eo in EthernetControl.EthernetSer.CollectionEthernetOperational)
                        {
                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                            {
                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                {
                                    if (displaySer.IsEthernet)
                                    {
                                        if (object.ReferenceEquals(displaySer.EthernetOperationalSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].EthernetOperationalSearch))
                                        {
                                            if (displaySer.EthernetOperationalSearch != eo.EthernetOperationalSearch)
                                            {
                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                            }
                                        }
                                    }
                                }
                            }                                  

                            if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count != EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count)
                            {
                                int count2 = 0;

                                if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count > PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count)
                                {
                                    if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count == 0)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Clear();
                                    }
                                    else
                                    {
                                        foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec)
                                        {
                                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                            {
                                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                                {
                                                    if (displaySer.IsEthernet)
                                                    {
                                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2]))
                                                        {
                                                            if (displaySer.ItemNetSearch != itemNew)
                                                            {
                                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                            }
                                                        }
                                                    }
                                                }
                                            }                                                   

                                            count2 += 1;
                                        }

                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec;
                                    }
                                }
                                else if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count < PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count)
                                {
                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count == 0)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec;
                                    }
                                    else
                                    {
                                        foreach (ItemNet itemNew in EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec)
                                        {
                                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                            {
                                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                                {
                                                    if (displaySer.IsEthernet)
                                                    {
                                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2]))
                                                        {
                                                            if (displaySer.ItemNetSearch != itemNew)
                                                            {
                                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                            }
                                                        }
                                                    }
                                                }
                                            }                                 

                                            count2 += 1;
                                        }

                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec;
                                    }
                                }

                                ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                            }
                            else
                            {
                                int count2 = 0;
                                bool isSave = false;

                                foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec)
                                {
                                    foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                    {
                                        foreach (DisplaySer displaySer in page.CollectionDisplay)
                                        {
                                            if (displaySer.IsEthernet)
                                            {
                                                if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2]))
                                                {
                                                    if (displaySer.ItemNetSearch != itemNew)
                                                    {
                                                        ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Description != itemNew.Description)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Description = itemNew.Description;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Range0 != itemNew.Range0)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Range0 = itemNew.Range0;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Range1 != itemNew.Range1)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Range1 = itemNew.Range1;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].TypeValue != itemNew.TypeValue)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].TypeValue = itemNew.TypeValue;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsSaveDatabase != itemNew.IsSaveDatabase)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsSaveDatabase = itemNew.IsSaveDatabase;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].TableName != itemNew.TableName)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].TableName = itemNew.TableName;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].PeridTimeSaveDB != itemNew.PeridTimeSaveDB)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].PeridTimeSaveDB = itemNew.PeridTimeSaveDB;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Formula != itemNew.Formula)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Formula = itemNew.Formula;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Text != itemNew.Text)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Text = itemNew.Text;
                                        isSave = true;
                                    }

                                    if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].EmergencyDown) != Convert.ToSingle(itemNew.EmergencyDown))
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].EmergencyDown = itemNew.EmergencyDown;
                                        isSave = true;
                                    }

                                    if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].EmergencyUp) != Convert.ToSingle(itemNew.EmergencyUp))
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].EmergencyUp = itemNew.EmergencyUp;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencyDown != itemNew.IsEmergencyDown)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencyDown = itemNew.IsEmergencyDown;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencyUp != itemNew.IsEmergencyUp)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencyUp = itemNew.IsEmergencyUp;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencySaveDB != itemNew.IsEmergencySaveDB)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencySaveDB = itemNew.IsEmergencySaveDB;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].PeriodEmergencySaveDB != itemNew.PeriodEmergencySaveDB)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].PeriodEmergencySaveDB = itemNew.PeriodEmergencySaveDB;
                                        isSave = true;
                                    }

                                    count2 += 1;
                                }

                                if (isSave)
                                { 
                                    ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                                }
                            }

                            if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count != EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count)
                            {
                                int count2 = 0;

                                if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count > PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count)
                                {
                                    if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count == 0)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Clear();
                                    }
                                    else
                                    {
                                        foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend)
                                        {
                                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                            {
                                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                                {
                                                    if (displaySer.IsEthernet)
                                                    {
                                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2]))
                                                        {
                                                            if (displaySer.ItemNetSearch != itemNew)
                                                            {
                                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                            }
                                                        }
                                                    }
                                                }
                                            }                                                    

                                            count2 += 1;
                                        }

                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend;
                                    }
                                }
                                else if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count < PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count)
                                {
                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count == 0)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend;
                                    }
                                    else
                                    {
                                        foreach (ItemNet itemNew in EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend)
                                        {
                                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                            {
                                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                                {
                                                    if (displaySer.IsEthernet)
                                                    {
                                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2]))
                                                        {
                                                            if (displaySer.ItemNetSearch != itemNew)
                                                            {
                                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                            }
                                                        }
                                                    }
                                                }
                                            }                                                   

                                            count2 += 1;
                                        }

                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend;
                                    }
                                }

                                ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                            }
                            else
                            {
                                int count2 = 0;
                                bool isSave = false;

                                foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend)
                                {
                                    foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                    {
                                        foreach (DisplaySer displaySer in page.CollectionDisplay)
                                        {
                                            if (displaySer.IsEthernet)
                                            {
                                                if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2]))
                                                {
                                                    if (displaySer.ItemNetSearch != itemNew)
                                                    {
                                                        ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Description != itemNew.Description)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Description = itemNew.Description;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Range0 != itemNew.Range0)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Range0 = itemNew.Range0;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Range1 != itemNew.Range1)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Range1 = itemNew.Range1;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].TypeValue != itemNew.TypeValue)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].TypeValue = itemNew.TypeValue;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsSaveDatabase != itemNew.IsSaveDatabase)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsSaveDatabase = itemNew.IsSaveDatabase;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].TableName != itemNew.TableName)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].TableName = itemNew.TableName;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].PeridTimeSaveDB != itemNew.PeridTimeSaveDB)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].PeridTimeSaveDB = itemNew.PeridTimeSaveDB;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Formula != itemNew.Formula)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Formula = itemNew.Formula;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Text != itemNew.Text)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Text = itemNew.Text;
                                        isSave = true;
                                    }

                                    if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].EmergencyDown) != Convert.ToSingle(itemNew.EmergencyDown))
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].EmergencyDown = itemNew.EmergencyDown;
                                        isSave = true;
                                    }

                                    if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].EmergencyUp) != Convert.ToSingle(itemNew.EmergencyUp))
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].EmergencyUp = itemNew.EmergencyUp;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencyDown != itemNew.IsEmergencyDown)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencyDown = itemNew.IsEmergencyDown;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencyUp != itemNew.IsEmergencyUp)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencyUp = itemNew.IsEmergencyUp;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencySaveDB != itemNew.IsEmergencySaveDB)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencySaveDB = itemNew.IsEmergencySaveDB;
                                        isSave = true;
                                    }

                                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].PeriodEmergencySaveDB != itemNew.PeriodEmergencySaveDB)
                                    {
                                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].PeriodEmergencySaveDB = itemNew.PeriodEmergencySaveDB;
                                        isSave = true;
                                    }

                                    count2 += 1;
                                }

                                if (isSave)
                                {                                    
                                    ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                                }
                            }

                            count += 1;
                        }

                        EthernetControl.EthernetSer.CollectionEthernetOperational = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational;
                    }
                }                        

                ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
            }
            else
            {
                int count = 0;
                bool isSave = false;

                foreach (EthernetOperational eo in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational)
                {
                    foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                    {
                        foreach (DisplaySer displaySer in page.CollectionDisplay)
                        {
                            if (displaySer.IsEthernet)
                            {
                                if (object.ReferenceEquals(displaySer.EthernetOperationalSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].EthernetOperationalSearch))
                                {
                                    if (displaySer.EthernetOperationalSearch != eo.EthernetOperationalSearch)
                                    {
                                        ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                    }
                                }
                            }
                        }
                    }

                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].Description != eo.Description)
                    {
                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].Description = eo.Description;
                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].EthernetOperationalSearch.Description = eo.Description;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].BufferSizeRec != eo.BufferSizeRec)
                    {
                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].BufferSizeRec = eo.BufferSizeRec;
                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].EthernetOperationalSearch.BufferSizeRec = eo.BufferSizeRec;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].BufferSizeSend != eo.BufferSizeSend)
                    {
                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].BufferSizeSend = eo.BufferSizeSend;
                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].EthernetOperationalSearch.BufferSizeSend = eo.BufferSizeSend;
                        isSave = true;
                    }

                    if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].Port != eo.Port)
                    {
                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].Port = eo.Port;
                        EthernetControl.EthernetSer.CollectionEthernetOperational[count].EthernetOperationalSearch.Port = eo.Port;
                        isSave = true;
                    }
                   
                    if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count != EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count)
                    {
                        int count2 = 0;

                        if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count > PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count)
                        {
                            if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count == 0)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Clear();
                            }
                            else
                            {
                                foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec)
                                {
                                    foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                    {
                                        foreach (DisplaySer displaySer in page.CollectionDisplay)
                                        {
                                            if (displaySer.IsEthernet)
                                            {
                                                if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2]))
                                                {
                                                    if (displaySer.ItemNetSearch != itemNew)
                                                    {
                                                        ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                    }
                                                }
                                            }
                                        }
                                    }                                         

                                    count2 += 1;
                                }

                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec;
                            }
                        }
                        else if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count < PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count)
                        {
                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec.Count == 0)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec;
                            }
                            else
                            {
                                foreach (ItemNet itemNew in EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec)
                                {
                                    foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                    {
                                        foreach (DisplaySer displaySer in page.CollectionDisplay)
                                        {
                                            if (displaySer.IsEthernet)
                                            {
                                                if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2]))
                                                {
                                                    if (displaySer.ItemNetSearch != itemNew)
                                                    {
                                                        ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                  
                                    count2 += 1;
                                }

                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec;
                            }
                        }

                        ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                    }
                    else
                    {
                        int count2 = 0;
                        bool isSave2 = false;

                        foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec)
                        {
                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                            {
                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                {
                                    if (displaySer.IsEthernet)
                                    {
                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2]))
                                        {
                                            if (displaySer.ItemNetSearch != itemNew)
                                            {
                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                            }
                                        }
                                    }
                                }
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Description != itemNew.Description)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Description = itemNew.Description;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Range0 != itemNew.Range0)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Range0 = itemNew.Range0;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Range1 != itemNew.Range1)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Range1 = itemNew.Range1;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].TypeValue != itemNew.TypeValue)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].TypeValue = itemNew.TypeValue;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsSaveDatabase != itemNew.IsSaveDatabase)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsSaveDatabase = itemNew.IsSaveDatabase;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].TableName != itemNew.TableName)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].TableName = itemNew.TableName;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].PeridTimeSaveDB != itemNew.PeridTimeSaveDB)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].PeridTimeSaveDB = itemNew.PeridTimeSaveDB;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Formula != itemNew.Formula)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Formula = itemNew.Formula;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Text != itemNew.Text)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].Text = itemNew.Text;
                                isSave2 = true;
                            }

                            if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].EmergencyDown) != Convert.ToSingle(itemNew.EmergencyDown))
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].EmergencyDown = itemNew.EmergencyDown;
                                isSave2 = true;
                            }

                            if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].EmergencyUp) != Convert.ToSingle(itemNew.EmergencyUp))
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].EmergencyUp = itemNew.EmergencyUp;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencyDown != itemNew.IsEmergencyDown)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencyDown = itemNew.IsEmergencyDown;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencyUp != itemNew.IsEmergencyUp)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencyUp = itemNew.IsEmergencyUp;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencySaveDB != itemNew.IsEmergencySaveDB)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].IsEmergencySaveDB = itemNew.IsEmergencySaveDB;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].PeriodEmergencySaveDB != itemNew.PeriodEmergencySaveDB)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetRec[count2].PeriodEmergencySaveDB = itemNew.PeriodEmergencySaveDB;
                                isSave2 = true;
                            }

                            count2 += 1;
                        }

                        if (isSave2)
                        {
                            ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                        }
                    }

                    if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count != EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count)
                    {
                        int count2 = 0;

                        if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count > PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count)
                        {
                            if (PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count == 0)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Clear();
                            }
                            else
                            {
                                foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend)
                                {
                                    foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                    {
                                        foreach (DisplaySer displaySer in page.CollectionDisplay)
                                        {
                                            if (displaySer.IsEthernet)
                                            {
                                                if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2]))
                                                {
                                                    if (displaySer.ItemNetSearch != itemNew)
                                                    {
                                                        ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                    }
                                                }
                                            }
                                        }
                                    }                                           

                                    count2 += 1;
                                }

                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend;
                            }
                        }
                        else if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count < PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count)
                        {
                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend.Count == 0)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend;
                            }
                            else
                            {
                                foreach (ItemNet itemNew in EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend)
                                {
                                    foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                                    {
                                        foreach (DisplaySer displaySer in page.CollectionDisplay)
                                        {
                                            if (displaySer.IsEthernet)
                                            {
                                                if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2]))
                                                {
                                                    if (displaySer.ItemNetSearch != itemNew)
                                                    {
                                                        ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                                    }
                                                }
                                            }
                                        }
                                    }                                            

                                    count2 += 1;
                                }

                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend = PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend;
                            }
                        }

                        ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                    }
                    else
                    {
                        int count2 = 0;
                        bool isSave2 = false;

                        foreach (ItemNet itemNew in PropertiesGeneral.NewEthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend)
                        {
                            foreach (Page page in ((AppWPF)Application.Current).CollectionPage.Values)
                            {
                                foreach (DisplaySer displaySer in page.CollectionDisplay)
                                {
                                    if (displaySer.IsEthernet)
                                    {
                                        if (object.ReferenceEquals(displaySer.ItemNetSearch, EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2]))
                                        {
                                            if (displaySer.ItemNetSearch != itemNew)
                                            {
                                                ((AppWPF)Application.Current).SaveTabItem(displaySer.ControlItem.CanvasTab.TabItemParent);
                                            }
                                        }
                                    }
                                }
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Description != itemNew.Description)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Description = itemNew.Description;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Range0 != itemNew.Range0)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Range0 = itemNew.Range0;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Range1 != itemNew.Range1)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Range1 = itemNew.Range1;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].TypeValue != itemNew.TypeValue)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].TypeValue = itemNew.TypeValue;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsSaveDatabase != itemNew.IsSaveDatabase)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsSaveDatabase = itemNew.IsSaveDatabase;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].TableName != itemNew.TableName)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].TableName = itemNew.TableName;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].PeridTimeSaveDB != itemNew.PeridTimeSaveDB)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].PeridTimeSaveDB = itemNew.PeridTimeSaveDB;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Formula != itemNew.Formula)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Formula = itemNew.Formula;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Text != itemNew.Text)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].Text = itemNew.Text;
                                isSave2 = true;
                            }

                            if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].EmergencyDown) != Convert.ToSingle(itemNew.EmergencyDown))
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].EmergencyDown = itemNew.EmergencyDown;
                                isSave2 = true;
                            }

                            if (Convert.ToSingle(EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].EmergencyUp) != Convert.ToSingle(itemNew.EmergencyUp))
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].EmergencyUp = itemNew.EmergencyUp;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencyDown != itemNew.IsEmergencyDown)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencyDown = itemNew.IsEmergencyDown;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencyUp != itemNew.IsEmergencyUp)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencyUp = itemNew.IsEmergencyUp;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencySaveDB != itemNew.IsEmergencySaveDB)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].IsEmergencySaveDB = itemNew.IsEmergencySaveDB;
                                isSave2 = true;
                            }

                            if (EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].PeriodEmergencySaveDB != itemNew.PeriodEmergencySaveDB)
                            {
                                EthernetControl.EthernetSer.CollectionEthernetOperational[count].CollectionItemNetSend[count2].PeriodEmergencySaveDB = itemNew.PeriodEmergencySaveDB;
                                isSave2 = true;
                            }

                            count2 += 1;
                        }

                        if (isSave2)
                        {
                            ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
                        }
                    }

                    count += 1;
                }

                if (isSave)
                {
                    ((AppWPF)Application.Current).SaveTabItem(EthernetControl.CanvasTab.TabItemParent);
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
    }
}

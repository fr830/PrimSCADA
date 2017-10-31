// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using Microsoft.Win32;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SCADA
{
    static class StaticValue
    {
        public static string SPeriodSaveDB = "Период сохранения в БД";
        public static string SPeriodSaveSetDB = "Период сохранения уставки";
        public static string SRange1_86400 = "Диапазон 1 - 86400";
        public static string SRangePort = "Диапазон порта 0 - 65535";
        public static string SRange1_102400000 = "Диапазон буфера 1 - 102400000";
        public static string SRange0 = "Диапазон 0";
        public static string SRange1 = "Диапазон 1";
        public static string SSetUp = "Значение уставки верх";
        public static string SSetDown = "Значение уставки низ";
        public static string SSetMessage = "Уставка вверх не может быть меньше низа или равно";
        public static string SDescription = "Описание";
        public static string STableName = "Имя таблицы в БД";
        public static string SFormula = "Формула";
        public static string SText = "Текст";
        public static string SDescriptionMessage = "Описание не может быть длинее 200 символов.";
        public static string STableNameDescription = "Имя таблицы не может быть длинее 100 символов.";
        public static string SFormulaDescription = "Формула не может быть длинее 300 символов.";
        public static string STextDescription = "Текст не может быть длинее 100 символов.";
        public static string SAddress = "Адрес регистра";
        public static string SRange1_255 = "Диапазон 1 - 255";
        public static string SRange0_65535 = "Диапазон 0 - 65535";
        public static string SRange_32768_32767 = "Диапазон -32768 - 32767";
        public static string SRange_2147483648_2147483647 = "Диапазон -2147483648 - 2147483647";
        public static string SIsSaveBD = "Сохранять в БД";
        public static string SIsSaveSetDB = "Сохранять уставки в БД";
        public static string SIsSetUp = "Уставка верх";
        public static string SIsSetDown = "Уставка низ";

        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null)
            {
                return null;
            }
            if (element.GetType() == type)
            {
                return element;
            }
            Visual foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }
    }

    class ModbusSendConnect
    {
        public string ComPort;
        public byte Slave;
        public bool IsReverse;
        public bool IsUS800;
        public string Protocol;
    }

    class GridPropertiesModbusGeneral : Grid
    {        
        public static readonly DependencyProperty IsBindingStartProperty =
          DependencyProperty.Register(
          "IsBindingStart",
          typeof(bool),
          typeof(GridPropertiesModbusGeneral),
           new FrameworkPropertyMetadata(false));

        public bool IsBindingStart
        {
            get { return (bool)GetValue(GridPropertiesModbusGeneral.IsBindingStartProperty); }
            set { SetValue(GridPropertiesModbusGeneral.IsBindingStartProperty, value); }
        }

        public Popup PopupMessage = new Popup();
        public Label LPopupMessage = new Label();

        SerialPort SerialPort;
        public TextBox TBIDModbus = new TextBox();
        public TextBox TBDescriptionModbus = new TextBox();
        public TextBox TBTime = new TextBox();
        public TextBox TBAddressSlave = new TextBox();
        public DataGrid DG = new DataGrid();
        public ComboBox CBProtocol = new ComboBox();
        public ComboBox CBSerialPort = new ComboBox();
        public CheckBox CHReverseRegister = new CheckBox();
        public CheckBox CHUS800 = new CheckBox();
        Button RemoveNetButton;
        Button AddNetButton;
        public Button Start;
        Button Stop;
        ModbusControl ModbusControl;
        public ModbusSer NewModbusSer;
        TextBox TBStatus;
        TextBox TBTableName;
        TextBox TBDescriptionItemNet;
        TextBox TBPeriodTimeSaveDB;
        TextBox TBAddress;
        TextBox TBFormula;
        TextBox TBText;
        TextBox TBEmergencyUp;
        TextBox TBEmergencyDown;
        TextBox TBEmergencySaveBD;

        ItemModbus Item;

        int PeriodTime;
        volatile bool IsStop;

        string EscText;
        float EscDigital;

        public GridPropertiesModbusGeneral(ModbusControl modbusControl)
        {
            LPopupMessage = new Label();
            LPopupMessage.BorderThickness = new Thickness(1);
            LPopupMessage.BorderBrush = Brushes.Red;
            LPopupMessage.Background = Brushes.White;

            PopupMessage.AllowsTransparency = true;
            PopupMessage.Child = LPopupMessage;
            PopupMessage.PopupAnimation = PopupAnimation.Fade;
            PopupMessage.StaysOpen = false;

            this.Unloaded += GridPropertiesEthernetGeneral_Unloaded;

            this.MaxWidth = 1000;
            this.MaxHeight = 800;
            
            ModbusControl = modbusControl;

            ModbusSer copyModbusSer = null;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, modbusControl.ModbusSer);

                ms.Position = 0;
                copyModbusSer = (ModbusSer)bf.Deserialize(ms);
            }

            NewModbusSer = copyModbusSer;
            
            foreach(ComSer comSer in ((AppWPF)Application.Current).CollectionComSers)
            {
                if(comSer.ComPort != null)
                {
                    if(CBSerialPort.Items.Count != 0)
                    {
                        foreach(string s in CBSerialPort.Items)
                        {
                            if(s != comSer.ComPort)
                            {
                                CBSerialPort.Items.Add(comSer.ComPort);

                                break;
                            }
                        }
                    }
                    else
                    {
                        CBSerialPort.Items.Add(comSer.ComPort);
                    }                   
                }
            }

            CBSerialPort.MinWidth = 100;

            CHReverseRegister.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            CHUS800.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TBIDModbus.MinWidth = 150;
            TBIDModbus.Margin = new Thickness(3);
            TBIDModbus.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            TBIDModbus.MaxWidth = 300;
            TBIDModbus.IsReadOnly = true;

            TBDescriptionModbus.MinWidth = 150;
            TBDescriptionModbus.Margin = new Thickness(3);
            TBDescriptionModbus.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            TBDescriptionModbus.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            TBDescriptionModbus.MaxWidth = 300;
            TBDescriptionModbus.MaxHeight = 200;
            TBDescriptionModbus.AcceptsReturn = true;

            TBAddressSlave.MinWidth = 50;
            TBAddressSlave.Margin = new Thickness(3);
            TBAddressSlave.MaxWidth = 50;            
            
            TBTime.MinWidth = 40;

            CBProtocol.Items.Add("RTU");
            CBProtocol.Items.Add("ASCII");

            #region DG
            Binding bindingCollectionNet = new Binding();
            bindingCollectionNet.NotifyOnSourceUpdated = true;
            bindingCollectionNet.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingCollectionNet.Source = NewModbusSer.CollectionItemModbus;

            DG.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            DG.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            DG.BorderThickness = new Thickness(3);
            DG.BorderBrush = Brushes.Black;
            DG.SetValue(Grid.RowProperty, 10);
            DG.AutoGenerateColumns = false;
            DG.CellEditEnding += DG_CellEditEnding;
            DG.PreparingCellForEdit += DG_PreparingCellForEdit;
            DG.MaxHeight = 600;
            DG.MaxWidth = 800;
            DG.CanUserAddRows = false;
            DG.SetBinding(DataGrid.ItemsSourceProperty, bindingCollectionNet);                                   

            Binding bindingType = new Binding();
            bindingType.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingType.Path = new PropertyPath("TypeValue");

            Binding bindingFunction = new Binding();
            bindingFunction.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingFunction.Path = new PropertyPath("Function");

            Binding bindingValue = new Binding();
            bindingValue.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingValue.Path = new PropertyPath("Value");

            Binding bindingID = new Binding();
            bindingID.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingID.Path = new PropertyPath("ID");

            Binding bindingDescription = new Binding();
            bindingDescription.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingDescription.Path = new PropertyPath("Description");

            Binding bindingIsSaveDatabase = new Binding();
            bindingIsSaveDatabase.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsSaveDatabase.Path = new PropertyPath("IsSaveDatabase");

            Binding bindingTableName = new Binding();
            bindingTableName.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingTableName.Path = new PropertyPath("TableName");

            Binding bindingPeriodSaveDB = new Binding();
            bindingPeriodSaveDB.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingPeriodSaveDB.Path = new PropertyPath("PeridTimeSaveDB");

            Binding bindingAddress = new Binding();
            bindingAddress.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingAddress.Path = new PropertyPath("Address");

            Binding bindingFormula = new Binding();
            bindingFormula.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingFormula.Path = new PropertyPath("Formula");

            Binding bindingText = new Binding();
            bindingText.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingText.Path = new PropertyPath("Text");

            Binding bindingEmergencyUp = new Binding();
            bindingEmergencyUp.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingEmergencyUp.Path = new PropertyPath("EmergencyUp");

            Binding bindingEmergencyDown = new Binding();
            bindingEmergencyDown.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingEmergencyDown.Path = new PropertyPath("EmergencyDown");

            Binding bindingIsEmergencySaveDB = new Binding();
            bindingIsEmergencySaveDB.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsEmergencySaveDB.Path = new PropertyPath("IsEmergencySaveDB");

            Binding bindingPeriodEmergencySaveDB = new Binding();
            bindingPeriodEmergencySaveDB.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingPeriodEmergencySaveDB.Path = new PropertyPath("PeriodEmergencySaveDB");

            Binding bindingIsEmergencyUp = new Binding();
            bindingIsEmergencyUp.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsEmergencyUp.Path = new PropertyPath("IsEmergencyUpDG");

            Binding bindingIsEmergencyDown = new Binding();
            bindingIsEmergencyDown.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsEmergencyDown.Path = new PropertyPath("IsEmergencyDownDG");

            Binding bindingEmergencyUpEnable = new Binding();
            bindingEmergencyUpEnable.Path = new PropertyPath("IsEmergencyUpDG");

            Binding bindingEmergencyDownEnable = new Binding();
            bindingEmergencyDownEnable.Path = new PropertyPath("IsEmergencyDownDG");

            Binding bindingIsUp = new Binding();
            bindingIsUp.Path = new PropertyPath("IsEmergencyUpDG");

            Binding bindingIsDown = new Binding();
            bindingIsDown.Path = new PropertyPath("IsEmergencyDownDG");

            MultiBinding bindingIsEmergencyDB = new MultiBinding();
            bindingIsEmergencyDB.Converter = new CheckBoxEmergencySaveDBValueConverter();
            bindingIsEmergencyDB.Bindings.Add(bindingIsUp);
            bindingIsEmergencyDB.Bindings.Add(bindingIsDown);

            Binding bindingEnableTBPeriodEmergencySaveDB1 = new Binding();
            bindingEnableTBPeriodEmergencySaveDB1.Path = new PropertyPath("IsEmergencyUpDG");

            Binding bindingEnableTBPeriodEmergencySaveDB2 = new Binding();
            bindingEnableTBPeriodEmergencySaveDB2.Path = new PropertyPath("IsEmergencyDownDG");

            Binding bindingEnableTBPeriodEmergencySaveDB3 = new Binding();
            bindingEnableTBPeriodEmergencySaveDB3.Path = new PropertyPath("IsEmergencySaveDB");

            MultiBinding bindingEnableTBPeriodEmergencySaveDB4 = new MultiBinding();
            bindingEnableTBPeriodEmergencySaveDB4.Converter = new DGTBEmergencyEnabledValueConverter();
            bindingEnableTBPeriodEmergencySaveDB4.Bindings.Add(bindingEnableTBPeriodEmergencySaveDB1);
            bindingEnableTBPeriodEmergencySaveDB4.Bindings.Add(bindingEnableTBPeriodEmergencySaveDB2);
            bindingEnableTBPeriodEmergencySaveDB4.Bindings.Add(bindingEnableTBPeriodEmergencySaveDB3);

            FrameworkElementFactory lTypeEditable = new FrameworkElementFactory(typeof(ComboBox));
            lTypeEditable.Name = "ComboBoxType";

            FrameworkElementFactory lFunctionEditable = new FrameworkElementFactory(typeof(ComboBox));
            lFunctionEditable.Name = "ComboBoxFunction";

            FrameworkElementFactory lType = new FrameworkElementFactory(typeof(Label));
            lType.AddHandler(ComboBox.LoadedEvent, new RoutedEventHandler(LoadedType));
            lType.SetBinding(Label.ContentProperty, bindingType);

            FrameworkElementFactory lFunction = new FrameworkElementFactory(typeof(Label));
            lFunction.AddHandler(ComboBox.LoadedEvent, new RoutedEventHandler(LoadedFunction));
            lFunction.SetBinding(Label.ContentProperty, bindingFunction);

            FrameworkElementFactory lValue = new FrameworkElementFactory(typeof(Label));
            lValue.SetBinding(Label.ContentProperty, bindingValue);

            FrameworkElementFactory lAddress = new FrameworkElementFactory(typeof(Label));
            lAddress.SetBinding(Label.ContentProperty, bindingAddress);

            FrameworkElementFactory lFormula = new FrameworkElementFactory(typeof(Label));
            lFormula.SetBinding(Label.ContentProperty, bindingFormula);

            FrameworkElementFactory lText = new FrameworkElementFactory(typeof(Label));
            lText.SetBinding(Label.ContentProperty, bindingText);

            FrameworkElementFactory lEmergencyUp = new FrameworkElementFactory(typeof(Label));
            lEmergencyUp.SetBinding(Label.ContentProperty, bindingEmergencyUp);

            FrameworkElementFactory lEmergencyDown = new FrameworkElementFactory(typeof(Label));
            lEmergencyDown.SetBinding(Label.ContentProperty, bindingEmergencyDown);
            
            FrameworkElementFactory lPeriodEmergencySaveDB = new FrameworkElementFactory(typeof(Label));
            lPeriodEmergencySaveDB.SetBinding(Label.ContentProperty, bindingPeriodEmergencySaveDB);           

            FrameworkElementFactory tbAddressEditable = new FrameworkElementFactory(typeof(TextBox));
            tbAddressEditable.Name = "TextBoxAddress";
            tbAddressEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            tbAddressEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            tbAddressEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            tbAddressEditable.SetBinding(TextBox.TextProperty, bindingAddress);
                      
            FrameworkElementFactory lEthernetSerDescription = new FrameworkElementFactory(typeof(Label));
            lEthernetSerDescription.SetBinding(Label.ContentProperty, bindingDescription);

            FrameworkElementFactory lID = new FrameworkElementFactory(typeof(TextBox));
            lID.SetBinding(TextBox.TextProperty, bindingID);

            FrameworkElementFactory lDescriptionEditable = new FrameworkElementFactory(typeof(TextBox));
            lDescriptionEditable.Name = "TextBoxDescription";
            lDescriptionEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(Text_PreviewTextInput));
            lDescriptionEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lDescriptionEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            lDescriptionEditable.SetBinding(TextBox.TextProperty, bindingDescription);

            FrameworkElementFactory lEthernetSerIsSaveDatabase = new FrameworkElementFactory(typeof(CheckBox));
            lEthernetSerIsSaveDatabase.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            lEthernetSerIsSaveDatabase.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            lEthernetSerIsSaveDatabase.SetBinding(CheckBox.IsCheckedProperty, bindingIsSaveDatabase);

            FrameworkElementFactory lEthernetSerTableName = new FrameworkElementFactory(typeof(Label));
            lEthernetSerTableName.SetBinding(Label.ContentProperty, bindingTableName);

            FrameworkElementFactory lTableNameEditable = new FrameworkElementFactory(typeof(TextBox));
            lTableNameEditable.Name = "TextBoxTableName";
            lTableNameEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(Text_PreviewTextInput));
            lTableNameEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lTableNameEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            lTableNameEditable.SetBinding(TextBox.TextProperty, bindingTableName);

            FrameworkElementFactory lFormulaEditable = new FrameworkElementFactory(typeof(TextBox));
            lFormulaEditable.Name = "TextBoxFormula";
            lFormulaEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(Text_PreviewTextInput));
            lFormulaEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lFormulaEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            lFormulaEditable.SetBinding(TextBox.TextProperty, bindingFormula);

            FrameworkElementFactory lTextEditable = new FrameworkElementFactory(typeof(TextBox));
            lTextEditable.Name = "TextBoxText";
            lTextEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(Text_PreviewTextInput));
            lTextEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lTextEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            lTextEditable.SetBinding(TextBox.TextProperty, bindingText);

            FrameworkElementFactory lEthernetSerPeriodSaveDB = new FrameworkElementFactory(typeof(Label));
            lEthernetSerPeriodSaveDB.SetBinding(Label.ContentProperty, bindingPeriodSaveDB);

            FrameworkElementFactory lPeriodSaveDBEditable = new FrameworkElementFactory(typeof(TextBox));
            lPeriodSaveDBEditable.Name = "TextBoxPeriodSaveDB";
            lPeriodSaveDBEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            lPeriodSaveDBEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lPeriodSaveDBEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));            
            lPeriodSaveDBEditable.SetBinding(TextBox.TextProperty, bindingPeriodSaveDB);                 

            FrameworkElementFactory lEmergencyUpEditable = new FrameworkElementFactory(typeof(TextBox));
            lEmergencyUpEditable.Name = "TextBoxEmergencyUp";
            lEmergencyUpEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            lEmergencyUpEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lEmergencyUpEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));            
            lEmergencyUpEditable.SetBinding(TextBox.TextProperty, bindingEmergencyUp);
            lEmergencyUpEditable.SetBinding(TextBox.IsEnabledProperty, bindingEmergencyUpEnable);  

            FrameworkElementFactory lEmergencyDownEditable = new FrameworkElementFactory(typeof(TextBox));
            lEmergencyDownEditable.Name = "TextBoxEmergencyDown";
            lEmergencyDownEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            lEmergencyDownEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lEmergencyDownEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));           
            lEmergencyDownEditable.SetBinding(TextBox.TextProperty, bindingEmergencyDown);
            lEmergencyDownEditable.SetBinding(TextBox.IsEnabledProperty, bindingEmergencyDownEnable);            

            FrameworkElementFactory fIsEmergencySaveDB = new FrameworkElementFactory(typeof(CheckBox));
            fIsEmergencySaveDB.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            fIsEmergencySaveDB.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            fIsEmergencySaveDB.SetBinding(CheckBox.IsCheckedProperty, bindingIsEmergencySaveDB);
            fIsEmergencySaveDB.SetBinding(CheckBox.IsEnabledProperty, bindingIsEmergencyDB);           

            FrameworkElementFactory fPeriodEmergencySaveDBEditable = new FrameworkElementFactory(typeof(TextBox));
            fPeriodEmergencySaveDBEditable.Name = "TextBoxPeriodEmergencySaveDB";
            fPeriodEmergencySaveDBEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            fPeriodEmergencySaveDBEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            fPeriodEmergencySaveDBEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));           
            fPeriodEmergencySaveDBEditable.SetBinding(TextBox.TextProperty, bindingPeriodEmergencySaveDB);
            fPeriodEmergencySaveDBEditable.SetBinding(TextBox.IsEnabledProperty, bindingEnableTBPeriodEmergencySaveDB4);
            
            FrameworkElementFactory fIsEmergencyUp = new FrameworkElementFactory(typeof(CheckBox));
            fIsEmergencyUp.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            fIsEmergencyUp.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            fIsEmergencyUp.SetBinding(CheckBox.IsCheckedProperty, bindingIsEmergencyUp);

            FrameworkElementFactory fIsEmergencyDown = new FrameworkElementFactory(typeof(CheckBox));
            fIsEmergencyDown.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            fIsEmergencyDown.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            fIsEmergencyDown.SetBinding(CheckBox.IsCheckedProperty, bindingIsEmergencyDown);

            DataTemplate dataTemplateType = new DataTemplate();
            dataTemplateType.VisualTree = lType;

            DataTemplate dataTemplateFunction = new DataTemplate();
            dataTemplateFunction.VisualTree = lFunction;

            DataTemplate dataTemplateTypeEditable = new DataTemplate();
            dataTemplateTypeEditable.VisualTree = lTypeEditable;

            DataTemplate dataTemplateFunctionEditable = new DataTemplate();
            dataTemplateFunctionEditable.VisualTree = lFunctionEditable;

            DataTemplate dataTemplateValue = new DataTemplate();
            dataTemplateValue.VisualTree = lValue;

            DataTemplate dataTemplateAddress = new DataTemplate();
            dataTemplateAddress.VisualTree = lAddress;

            DataTemplate dataTemplateAddressEditable = new DataTemplate();
            dataTemplateAddressEditable.VisualTree = tbAddressEditable;

            DataTemplate dataTemplateID = new DataTemplate();
            dataTemplateID.VisualTree = lID;

            DataTemplate dataTemplateDescription = new DataTemplate();
            dataTemplateDescription.VisualTree = lEthernetSerDescription;

            DataTemplate dataTemplateDescriptionEditable = new DataTemplate();
            dataTemplateDescriptionEditable.VisualTree = lDescriptionEditable;

            DataTemplate dataTemplateIsSaveDatabase = new DataTemplate();
            dataTemplateIsSaveDatabase.VisualTree = lEthernetSerIsSaveDatabase;

            DataTemplate dataTemplateTableName = new DataTemplate();
            dataTemplateTableName.VisualTree = lEthernetSerTableName;

            DataTemplate dataTemplateTableNameEditable = new DataTemplate();
            dataTemplateTableNameEditable.VisualTree = lTableNameEditable;

            DataTemplate dataTemplatePeriodSaveDB = new DataTemplate();
            dataTemplatePeriodSaveDB.VisualTree = lEthernetSerPeriodSaveDB;

            DataTemplate dataTemplatePeriodSaveDBEditable = new DataTemplate();
            dataTemplatePeriodSaveDBEditable.VisualTree = lPeriodSaveDBEditable;

            DataTemplate dataTemplateFormula = new DataTemplate();
            dataTemplateFormula.VisualTree = lFormula;

            DataTemplate dataTemplateFormulaEditable = new DataTemplate();
            dataTemplateFormulaEditable.VisualTree = lFormulaEditable;

            DataTemplate dataTemplateText = new DataTemplate();
            dataTemplateText.VisualTree = lText;

            DataTemplate dataTemplateTextEditable = new DataTemplate();
            dataTemplateTextEditable.VisualTree = lTextEditable;

            DataTemplate dataTemplateEmergencyUp = new DataTemplate();
            dataTemplateEmergencyUp.VisualTree = lEmergencyUp;

            DataTemplate dataTemplateEmergencyUpEditable = new DataTemplate();
            dataTemplateEmergencyUpEditable.VisualTree = lEmergencyUpEditable;

            DataTemplate dataTemplateEmergencyDown = new DataTemplate();
            dataTemplateEmergencyDown.VisualTree = lEmergencyDown;

            DataTemplate dataTemplateEmergencyDownEditable = new DataTemplate();
            dataTemplateEmergencyDownEditable.VisualTree = lEmergencyDownEditable;

            DataTemplate dataTemplatePeriodEmergencySaveDB = new DataTemplate();
            dataTemplatePeriodEmergencySaveDB.VisualTree = lPeriodEmergencySaveDB;

            DataTemplate dataTemplatePeriodEmergencySaveDBEditable = new DataTemplate();
            dataTemplatePeriodEmergencySaveDBEditable.VisualTree = fPeriodEmergencySaveDBEditable;

            DataTemplate dataTemplateIsEmergencySaveDBEditable = new DataTemplate();
            dataTemplateIsEmergencySaveDBEditable.VisualTree = fIsEmergencySaveDB;

            DataTemplate dataTemplateIsEmergencyUpEditable = new DataTemplate();
            dataTemplateIsEmergencyUpEditable.VisualTree = fIsEmergencyUp;

            DataTemplate dataTemplateIsEmergencyDownEditable = new DataTemplate();
            dataTemplateIsEmergencyDownEditable.VisualTree = fIsEmergencyDown;         

            DataGridTemplateColumn type = new DataGridTemplateColumn();
            type.Header = "Тип";
            type.CellTemplate = dataTemplateType;
            type.CellEditingTemplate = dataTemplateTypeEditable;

            DataGridTemplateColumn function = new DataGridTemplateColumn();
            function.Header = "Функция";
            function.CellTemplate = dataTemplateFunction;
            function.CellEditingTemplate = dataTemplateFunctionEditable;

            DataGridTemplateColumn value = new DataGridTemplateColumn();
            value.Header = "Значение";
            value.CellTemplate = dataTemplateValue;
            value.IsReadOnly = true;

            DataGridTemplateColumn address = new DataGridTemplateColumn();
            address.Header = StaticValue.SAddress;
            address.CellTemplate = dataTemplateAddress;
            address.CellEditingTemplate = dataTemplateAddressEditable;

            DataGridTemplateColumn id = new DataGridTemplateColumn();
            id.Header = "ID";
            id.CellTemplate = dataTemplateID;

            DataGridTemplateColumn description = new DataGridTemplateColumn();
            description.Header = StaticValue.SDescription;
            description.CellTemplate = dataTemplateDescription;
            description.CellEditingTemplate = dataTemplateDescriptionEditable;

            DataGridTemplateColumn isSaveDatabase = new DataGridTemplateColumn();
            isSaveDatabase.Header = StaticValue.SIsSaveBD;
            isSaveDatabase.CellTemplate = dataTemplateIsSaveDatabase;

            DataGridTemplateColumn tableName = new DataGridTemplateColumn();
            tableName.Header = StaticValue.STableName;
            tableName.CellTemplate = dataTemplateTableName;
            tableName.CellEditingTemplate = dataTemplateTableNameEditable;

            DataGridTemplateColumn periodSaveDB = new DataGridTemplateColumn();
            periodSaveDB.Header = StaticValue.SPeriodSaveDB; 
            periodSaveDB.CellTemplate = dataTemplatePeriodSaveDB;
            periodSaveDB.CellEditingTemplate = dataTemplatePeriodSaveDBEditable;

            DataGridTemplateColumn formula = new DataGridTemplateColumn();
            formula.Header = StaticValue.SFormula;
            formula.CellTemplate = dataTemplateFormula;
            formula.CellEditingTemplate = dataTemplateFormulaEditable;

            DataGridTemplateColumn text = new DataGridTemplateColumn();
            text.Header = StaticValue.SText;
            text.CellTemplate = dataTemplateText;
            text.CellEditingTemplate = dataTemplateTextEditable;

            DataGridTemplateColumn emergencyUp = new DataGridTemplateColumn();
            emergencyUp.Header = StaticValue.SSetUp;
            emergencyUp.CellTemplate = dataTemplateEmergencyUp;
            emergencyUp.CellEditingTemplate = dataTemplateEmergencyUpEditable;

            DataGridTemplateColumn emergencyDown = new DataGridTemplateColumn();
            emergencyDown.Header = StaticValue.SSetDown;
            emergencyDown.CellTemplate = dataTemplateEmergencyDown;
            emergencyDown.CellEditingTemplate = dataTemplateEmergencyDownEditable;

            DataGridTemplateColumn periodEmergencySaveDB = new DataGridTemplateColumn();
            periodEmergencySaveDB.Header = StaticValue.SPeriodSaveSetDB;
            periodEmergencySaveDB.CellTemplate = dataTemplatePeriodEmergencySaveDB;
            periodEmergencySaveDB.CellEditingTemplate = dataTemplatePeriodEmergencySaveDBEditable;

            DataGridTemplateColumn isEmergencySaveDB = new DataGridTemplateColumn();
            isEmergencySaveDB.Header = StaticValue.SIsSaveSetDB;
            isEmergencySaveDB.CellTemplate = dataTemplateIsEmergencySaveDBEditable;

            DataGridTemplateColumn isEmergencyUp = new DataGridTemplateColumn();
            isEmergencyUp.Header = StaticValue.SIsSetUp;
            isEmergencyUp.CellTemplate = dataTemplateIsEmergencyUpEditable;

            DataGridTemplateColumn isEmergencyDown = new DataGridTemplateColumn();
            isEmergencyDown.Header = StaticValue.SIsSetDown;
            isEmergencyDown.CellTemplate = dataTemplateIsEmergencyDownEditable;

            DG.Columns.Add(type);
            DG.Columns.Add(value);
            DG.Columns.Add(function);
            DG.Columns.Add(address);
            DG.Columns.Add(formula);
            DG.Columns.Add(text);
            DG.Columns.Add(description);
            DG.Columns.Add(isSaveDatabase);
            DG.Columns.Add(tableName);
            DG.Columns.Add(periodSaveDB);
            DG.Columns.Add(isEmergencyUp);
            DG.Columns.Add(emergencyUp);
            DG.Columns.Add(isEmergencyDown);
            DG.Columns.Add(emergencyDown);
            DG.Columns.Add(isEmergencySaveDB);
            DG.Columns.Add(periodEmergencySaveDB);
            DG.Columns.Add(id);
            #endregion          

            RowDefinition rowID = new RowDefinition();
            rowID.Height = GridLength.Auto;

            RowDefinition rowDescription = new RowDefinition();
            rowDescription.Height = GridLength.Auto;

            RowDefinition rowSerialPort = new RowDefinition();
            rowSerialPort.Height = GridLength.Auto;

            RowDefinition rowAddressSlave = new RowDefinition();
            rowAddressSlave.Height = GridLength.Auto;

            RowDefinition rowTime = new RowDefinition();
            rowTime.Height = GridLength.Auto;

            RowDefinition rowNet = new RowDefinition();
            rowNet.Height = new GridLength(30);

            RowDefinition rowStatus = new RowDefinition();
            rowStatus.Height = GridLength.Auto;

            RowDefinition rowProtocols = new RowDefinition();
            rowProtocols.Height = GridLength.Auto;

            RowDefinition rowReverseRegister = new RowDefinition();
            rowReverseRegister.Height = GridLength.Auto;

            RowDefinition rowUS800 = new RowDefinition();
            rowUS800.Height = GridLength.Auto;

            RowDefinition rowItemNets = new RowDefinition();
            rowItemNets.Height = GridLength.Auto;

            Image addImage = new Image();
            addImage.Source = new BitmapImage(new Uri("Images/AddNet.png", UriKind.Relative));

            Image startImage = new Image();
            startImage.Source = new BitmapImage(new Uri("Images/StartEthernet.png", UriKind.Relative));

            Image stopImage = new Image();
            stopImage.Source = new BitmapImage(new Uri("Images/StopEthernet.png", UriKind.Relative));

            Binding bindingStartIsEnable = new Binding();
            bindingStartIsEnable.Converter = new DGItemsToBoolConverter();
            bindingStartIsEnable.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingStartIsEnable.Source = DG;
            bindingStartIsEnable.Path = new PropertyPath("Items.Count");

            Binding bindingStart2IsEnable = new Binding();
            bindingStart2IsEnable.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingStart2IsEnable.Source = this;
            bindingStart2IsEnable.Path = new PropertyPath("IsBindingStart");

            Binding bindingRemoveButtonIsEnable = new Binding();
            bindingRemoveButtonIsEnable.Converter = new RemoveButtonConverter();
            bindingRemoveButtonIsEnable.Source = DG;
            bindingRemoveButtonIsEnable.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingRemoveButtonIsEnable.Path = new PropertyPath("SelectedItem");

            Binding bindingRemoveButton2IsEnable = new Binding();
            bindingRemoveButton2IsEnable.Source = this;
            bindingRemoveButton2IsEnable.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingRemoveButton2IsEnable.Path = new PropertyPath("IsBindingStart");

            MultiBinding multiBindingStart = new MultiBinding();
            multiBindingStart.Converter = new ButtonStartInterfaceValueConverter();
            multiBindingStart.Bindings.Add(bindingStartIsEnable);
            multiBindingStart.Bindings.Add(bindingStart2IsEnable);

            MultiBinding RemoveButtonIsEnable = new MultiBinding();
            RemoveButtonIsEnable.Converter = new ButtonStartInterfaceValueConverter();
            RemoveButtonIsEnable.Bindings.Add(bindingRemoveButtonIsEnable);
            RemoveButtonIsEnable.Bindings.Add(bindingRemoveButton2IsEnable);

            Start = new Button();
            Start.SetBinding(Button.IsEnabledProperty, multiBindingStart);
            Start.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            Start.ToolTip = "Начать опрос сетевых параметров";
            Start.Margin = new Thickness(3);
            Start.Content = startImage;
            Start.Click += Start_Click;

            Stop = new Button();
            Stop.IsEnabled = false;
            Stop.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            Stop.ToolTip = "Остановить опрос сетевых параметров";
            Stop.Margin = new Thickness(3);
            Stop.Content = stopImage;
            Stop.Click += Stop_Click;

            AddNetButton = new Button();
            AddNetButton.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            AddNetButton.ToolTip = "Добавить параметр";
            AddNetButton.Margin = new Thickness(3);
            AddNetButton.Content = addImage;
            AddNetButton.PreviewMouseDown += AddNetButton_PreviewMouseDown;

            Image removeImage = new Image();
            removeImage.Source = new BitmapImage(new Uri("Images/DeleteNet.png", UriKind.Relative));

            RemoveNetButton = new Button();
            RemoveNetButton.IsEnabled = false;
            RemoveNetButton.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            RemoveNetButton.ToolTip = "Удалить параметр";
            RemoveNetButton.Margin = new Thickness(3);
            RemoveNetButton.Content = removeImage;
            RemoveNetButton.SetBinding(Button.IsEnabledProperty, RemoveButtonIsEnable);
            RemoveNetButton.Click += RemoveNetButton_Click;

            StackPanel panelID = new StackPanel();
            panelID.Orientation = Orientation.Horizontal;
            panelID.SetValue(Grid.RowProperty, 0);

            StackPanel panelDescription = new StackPanel();
            panelDescription.Orientation = Orientation.Horizontal;
            panelDescription.SetValue(Grid.RowProperty, 1);

            StackPanel panelSerialPort = new StackPanel();
            panelSerialPort.Orientation = Orientation.Horizontal;
            panelSerialPort.SetValue(Grid.RowProperty, 2);

            StackPanel panelAddressSlave = new StackPanel();
            panelAddressSlave.Orientation = Orientation.Horizontal;
            panelAddressSlave.SetValue(Grid.RowProperty, 3);

            StackPanel panelTime = new StackPanel();
            panelTime.Orientation = Orientation.Horizontal;
            panelTime.SetValue(Grid.RowProperty, 4);

            StackPanel panelProtocol = new StackPanel();
            panelProtocol.Orientation = Orientation.Horizontal;
            panelProtocol.SetValue(Grid.RowProperty, 5);

            StackPanel panelReverseRegister = new StackPanel();
            panelReverseRegister.Orientation = Orientation.Horizontal;
            panelReverseRegister.SetValue(Grid.RowProperty, 6);

            StackPanel panelUS800 = new StackPanel();
            panelUS800.Orientation = Orientation.Horizontal;
            panelUS800.SetValue(Grid.RowProperty, 7);

            StackPanel panelNetButton = new StackPanel();
            panelNetButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            panelNetButton.Orientation = Orientation.Horizontal;
            panelNetButton.SetValue(Grid.RowProperty, 8);

            StackPanel panelStatus = new StackPanel();
            panelStatus.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            panelStatus.Orientation = Orientation.Horizontal;
            panelStatus.SetValue(Grid.RowProperty, 9);
           
            TBStatus = new TextBox();
            TBStatus.TextWrapping = TextWrapping.Wrap;
            TBStatus.MaxWidth = 700;
            TBStatus.MaxLines = 5;
            TBStatus.IsReadOnly = true;
            TBStatus.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            TBStatus.Text = "Статус: Опрос остановлен";

            Label labelIDModbus = new Label();
            labelIDModbus.Content = "ID: ";

            Label labelDescriptionModbus = new Label();
            labelDescriptionModbus.Content = "Описание Modbus: ";

            Label LabelSerialPort = new Label();
            LabelSerialPort.Content = "Com-порт:";

            Label LabelAddressSlave = new Label();
            LabelAddressSlave.Content = "Адрес slave:";

            Label protocolLabel = new Label();
            protocolLabel.Content = "Протокол:";

            Label reverseLabel = new Label();
            reverseLabel.Content = "Реверс регистров:";

            Label us800Label = new Label();
            us800Label.Content = "Modbus US800:";

            Label timeLabel = new Label();
            timeLabel.Content = "Время опроса (с):";

            Label collectionNetLabel = new Label();
            collectionNetLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            collectionNetLabel.Content = "Список сетевых параметров";

            panelID.Children.Add(labelIDModbus);
            panelID.Children.Add(TBIDModbus);

            panelDescription.Children.Add(labelDescriptionModbus);
            panelDescription.Children.Add(TBDescriptionModbus);

            panelSerialPort.Children.Add(LabelSerialPort);
            panelSerialPort.Children.Add(CBSerialPort);

            panelAddressSlave.Children.Add(LabelAddressSlave);
            panelAddressSlave.Children.Add(TBAddressSlave);

            panelProtocol.Children.Add(protocolLabel);
            panelProtocol.Children.Add(CBProtocol);

            panelReverseRegister.Children.Add(reverseLabel);
            panelReverseRegister.Children.Add(CHReverseRegister);

            panelUS800.Children.Add(us800Label);
            panelUS800.Children.Add(CHUS800);

            panelTime.Children.Add(timeLabel);
            panelTime.Children.Add(TBTime);

            panelNetButton.Children.Add(Start);
            panelNetButton.Children.Add(Stop);
            panelNetButton.Children.Add(collectionNetLabel);
            panelNetButton.Children.Add(AddNetButton);
            panelNetButton.Children.Add(RemoveNetButton);

            panelStatus.Children.Add(TBStatus);

            RowDefinitions.Add(rowID);
            RowDefinitions.Add(rowDescription);
            RowDefinitions.Add(rowSerialPort);
            RowDefinitions.Add(rowAddressSlave);
            RowDefinitions.Add(rowTime);
            RowDefinitions.Add(rowProtocols);
            RowDefinitions.Add(rowReverseRegister);
            RowDefinitions.Add(rowUS800);
            RowDefinitions.Add(rowNet);
            RowDefinitions.Add(rowStatus);
            RowDefinitions.Add(rowItemNets);

            MenuItem menuItemPasteBufferSize = new MenuItem();
            menuItemPasteBufferSize.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyBufferSize = new MenuItem();
            menuItemCopyBufferSize.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuBufferSize = new System.Windows.Controls.ContextMenu();
            ContextMenuBufferSize.Items.Add(menuItemPasteBufferSize);
            ContextMenuBufferSize.Items.Add(menuItemCopyBufferSize);

            MenuItem menuItemPasteDescription = new MenuItem();
            menuItemPasteDescription.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyDescription = new MenuItem();
            menuItemCopyDescription.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuDescription = new System.Windows.Controls.ContextMenu();
            ContextMenuDescription.Items.Add(menuItemPasteDescription);
            ContextMenuDescription.Items.Add(menuItemCopyDescription);
                                   
            MenuItem menuItemPasteTime = new MenuItem();
            menuItemPasteTime.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyTime = new MenuItem();
            menuItemCopyTime.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuTime = new System.Windows.Controls.ContextMenu();
            ContextMenuTime.Items.Add(menuItemPasteTime);
            ContextMenuTime.Items.Add(menuItemCopyTime);

            TBDescriptionModbus.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBDescriptionModbus.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextTextBoxPaste));
            TBDescriptionModbus.ContextMenu = ContextMenuDescription;

            TBDescriptionModbus.PreviewTextInput += Text_PreviewTextInput;
            TBDescriptionModbus.PreviewKeyDown += BackspacePreviewTextKeyDown;
            TBDescriptionModbus.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBDescriptionModbus.GotFocus += TextBoxFocus;                

            TBTime.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBTime.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
            TBTime.ContextMenu = ContextMenuTime;

            TBAddressSlave.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBAddressSlave.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
            TBAddressSlave.ContextMenu = ContextMenuTime;

            TBAddressSlave.PreviewTextInput += DigitalTextBox_PreviewTextInput;
            TBAddressSlave.PreviewKeyDown += BackspacePreviewTextKeyDown;
            TBAddressSlave.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBAddressSlave.GotFocus += TextBoxFocus;

            TBTime.PreviewTextInput += DigitalTextBox_PreviewTextInput;
            TBTime.PreviewKeyDown += BackspacePreviewTextKeyDown;
            TBTime.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBTime.GotFocus += TextBoxFocus;

            TBTime.Margin = new Thickness(3);

            Binding bindingDescriptionModbus = new Binding();
            bindingDescriptionModbus.Source = NewModbusSer;
            bindingDescriptionModbus.Path = new PropertyPath("Description");

            Binding bindingTimeModbus = new Binding();
            bindingTimeModbus.Source = NewModbusSer;
            bindingTimeModbus.Path = new PropertyPath("Time");

            Binding bindingSlaveAddressModbus = new Binding();
            bindingSlaveAddressModbus.Source = NewModbusSer;
            bindingSlaveAddressModbus.Path = new PropertyPath("SlaveAddress");

            Binding bindingReverseRegisterModbus = new Binding();
            bindingReverseRegisterModbus.Source = NewModbusSer;
            bindingReverseRegisterModbus.Path = new PropertyPath("ReverseRegister");

            Binding bindingIsUS800Modbus = new Binding();
            bindingIsUS800Modbus.Source = NewModbusSer;
            bindingIsUS800Modbus.Path = new PropertyPath("IsUS800");

            Binding bindingProtocolModbus = new Binding();
            bindingProtocolModbus.Source = NewModbusSer;
            bindingProtocolModbus.Path = new PropertyPath("Protocol");

            Binding bindingComPortModbus = new Binding();
            bindingComPortModbus.Source = NewModbusSer;
            bindingComPortModbus.Path = new PropertyPath("ComPort");

            CHUS800.SetBinding(CheckBox.IsCheckedProperty, bindingIsUS800Modbus);
            CHReverseRegister.SetBinding(CheckBox.IsCheckedProperty, bindingReverseRegisterModbus);
            TBTime.SetBinding(TextBox.TextProperty, bindingTimeModbus);
            TBDescriptionModbus.SetBinding(TextBox.TextProperty, bindingDescriptionModbus);
            TBAddressSlave.SetBinding(TextBox.TextProperty, bindingSlaveAddressModbus);
            CBProtocol.SetBinding(ComboBox.SelectedItemProperty, bindingProtocolModbus);
            CBSerialPort.SetBinding(ComboBox.SelectedItemProperty, bindingComPortModbus);

            TBIDModbus.Text = ModbusControl.ModbusSer.ID;

            this.Children.Add(panelID);
            this.Children.Add(panelDescription);
            this.Children.Add(panelSerialPort);
            this.Children.Add(panelAddressSlave);
            this.Children.Add(panelTime);
            this.Children.Add(panelProtocol);
            this.Children.Add(panelReverseRegister);
            this.Children.Add(panelUS800);
            this.Children.Add(panelStatus);
            this.Children.Add(DG);
            this.Children.Add(panelNetButton);
        }        

        void BackspacePreviewTextKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (e.Key == Key.Back)
            {
                PopupMessage.IsOpen = false;
            }
            else if (e.Key == Key.Escape)
            {                
                if (tb == TBTime || tb == TBAddressSlave)
                {
                    e.Handled = true;

                    PopupMessage.IsOpen = false;

                    tb.Text = EscDigital.ToString(CultureInfo.InvariantCulture);
                }
                else if (tb == TBDescriptionModbus)
                {
                    e.Handled = true;

                    PopupMessage.IsOpen = false;

                    tb.Text = EscText;
                }
            }
        }

        void PopupMessageShow(string columnName, TextBox tb, string message, RoutedEventArgs e)
        {
            if (e != null)
            {
                e.Handled = true;
            }

            int i = 0;
            double offset = 0;
           
            foreach (DataGridColumn col in DG.Columns)
            {
                if ((string)col.Header == columnName)
                {
                    break;
                }

                i += 1;
                offset += col.Width.DisplayValue;
            }

            ScrollViewer scrollViewer = StaticValue.GetDescendantByType(DG, typeof(ScrollViewer)) as ScrollViewer;

            PopupMessage.HorizontalOffset = scrollViewer.HorizontalOffset - offset;
            PopupMessage.PlacementTarget = tb;
            LPopupMessage.Content = message;
            PopupMessage.IsOpen = true;
        }

        void Text_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PopupMessage.IsOpen = false;

            TextBox tb = (TextBox)sender;

            if (tb == TBDescriptionModbus)
            {
                if (tb.Text.Length + 1 > 200)
                {
                    if (tb.SelectionLength > 0)
                    {
                        if ((tb.Text.Length + 1) - tb.SelectionLength <= 200)
                        {
                            return;
                        }
                    }

                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SDescriptionMessage;
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }
            }
            else if (tb == TBDescriptionItemNet)
            {
                if (tb.Text.Length + 1 > 200)
                {
                    if (tb.SelectionLength > 0)
                    {
                        if ((tb.Text.Length + 1) - tb.SelectionLength <= 200)
                        {
                            return;
                        }
                    }

                    PopupMessageShow(StaticValue.SDescription, tb, StaticValue.SDescriptionMessage, e);
                }
            }
            else if (tb == TBTableName)
            {
                if (tb.Text.Length + 1 > 100)
                {
                    if (tb.SelectionLength > 0)
                    {
                        if ((tb.Text.Length + 1) - tb.SelectionLength <= 100)
                        {
                            return;
                        }
                    }

                    PopupMessageShow(StaticValue.STableName, tb, StaticValue.STableNameDescription, e);
                }
            }
            else if (tb == TBFormula)
            {
                if (tb.Text.Length + 1 > 300)
                {
                    if (tb.SelectionLength > 0)
                    {
                        if ((tb.Text.Length + 1) - tb.SelectionLength <= 300)
                        {
                            return;
                        }
                    }

                    PopupMessageShow(StaticValue.SFormula, tb, StaticValue.SFormulaDescription, e);
                }
            }
            else if (tb == TBText)
            {
                if (tb.Text.Length + 1 > 100)
                {
                    if (tb.SelectionLength > 0)
                    {
                        if ((tb.Text.Length + 1) - tb.SelectionLength <= 100)
                        {
                            return;
                        }
                    }

                    PopupMessageShow(StaticValue.SText, tb, StaticValue.STextDescription, e);
                }
            }
        }
       
        void TextBoxFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb == TBTime || tb == TBAddressSlave)
            {
                float.TryParse(tb.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out EscDigital);
            }
            else if (tb == TBDescriptionModbus)
            {
                EscText = tb.Text;
            }

            tb.SelectAll();

            e.Handled = true;
        }


        void TextTextBoxPaste(object sender, ExecutedRoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            string s;

            if (tb.SelectionLength > 0)
            {
                s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                s = s.Insert(tb.SelectionStart, Clipboard.GetText());
            }
            else
            {
                s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
            }

            if (tb == TBDescriptionModbus)
            {
                if (s.Length > 200)
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SDescriptionMessage;
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }
                else
                {
                    tb.Paste();
                }
            }
            else if (tb == TBDescriptionItemNet)
            {
                if (s.Length > 200)
                {
                    PopupMessageShow(StaticValue.SDescription, tb, StaticValue.SDescriptionMessage, e);
                }
                else
                {
                    tb.Paste();
                }
            }
            else if (tb == TBTableName)
            {
                if (s.Length > 100)
                {
                    PopupMessageShow(StaticValue.STableName, tb, StaticValue.STableNameDescription, e);
                }
                else
                {
                    tb.Paste();
                }
            }
            else if (tb == TBFormula)
            {
                if (s.Length > 300)
                {
                    PopupMessageShow(StaticValue.SFormula, tb, StaticValue.SFormulaDescription, e);
                }
                else
                {
                    tb.Paste();
                }
            }
            else if (tb == TBText)
            {
                if (s.Length > 100)
                {
                    PopupMessageShow(StaticValue.SText, tb, StaticValue.STextDescription, e);
                }
                else
                {
                    tb.Paste();
                }
            }
        }

        void RemoveNetButton_Click(object sender, RoutedEventArgs e)
        {
            NewModbusSer.CollectionItemModbus.Remove((ItemModbus)DG.SelectedItem);

            e.Handled = true;
        }

        void GridPropertiesEthernetGeneral_Unloaded(object sender, RoutedEventArgs e)
        {            
            IsStop = true;

            e.Handled = true;
        }

        void CorrectSelectionAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tbCorrectSelectionAll = (TextBox)sender;

            if (e.ClickCount > 1 && tbCorrectSelectionAll.SelectionLength > 0)
            {
                e.Handled = true;
                tbCorrectSelectionAll.CaretIndex = tbCorrectSelectionAll.GetCharacterIndexFromPoint(Mouse.GetPosition(tbCorrectSelectionAll), true);
            }

            if (!tbCorrectSelectionAll.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                Keyboard.Focus(tbCorrectSelectionAll);
            }
        }

        void Stop_Click(object sender, RoutedEventArgs e)
        {
            IsStop = true;

            Stop.IsEnabled = false;

            e.Handled = true;
        }

        void Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsStop)
                {
                    IsBindingStart = true;
                    Stop.IsEnabled = true;
                    AddNetButton.IsEnabled = false;
                    CBSerialPort.IsEnabled = true;
                    TBTime.IsReadOnly = true;
                    CBProtocol.IsEnabled = false;

                    foreach (DataGridColumn column in DG.Columns)
                    {
                        if (column.Header == StaticValue.SRange0)
                        {
                            column.IsReadOnly = true;
                        }
                        else if (column.Header == StaticValue.SRange1)
                        {
                            column.IsReadOnly = true;
                        }
                        else if (column.Header == "Тип")
                        {
                            column.IsReadOnly = true;
                        }
                    }                  

                    TBStatus.Text = "Статус: подключение к " + CBSerialPort.SelectedItem;

                    ModbusSendConnect param = new ModbusSendConnect();
                    param.IsReverse = (bool)CHReverseRegister.IsChecked;
                    param.ComPort = (string)CBSerialPort.SelectedItem;
                    param.Slave = byte.Parse(TBAddressSlave.Text);
                    param.IsUS800 = (bool)CHUS800.IsChecked;
                    param.Protocol = (string)CBProtocol.SelectedItem;

                    PeriodTime = int.Parse(TBTime.Text);

                    Thread threadConnect = new Thread(Connect);
                    threadConnect.Start(param);
                }
                else
                {
                    TBStatus.Text = "Статус: com-порт еще не закрыт, повторите попытку позже";
                }
            }
            catch (SystemException ex)
            {
                TBStatus.Text = "Статус: " + ex;

                IsBindingStart = false;
                Stop.IsEnabled = false;
                AddNetButton.IsEnabled = true;
                CBSerialPort.IsEnabled = false;
                TBTime.IsReadOnly = false;               
                CBProtocol.IsEnabled = true;

                foreach (DataGridColumn column in DG.Columns)
                {
                    if (column.Header == StaticValue.SRange0)
                    {
                        column.IsReadOnly = false;
                    }
                    else if (column.Header == StaticValue.SRange1)
                    {
                        column.IsReadOnly = false;
                    }
                    else if (column.Header == "Тип")
                    {
                        column.IsReadOnly = false;
                    }
                }
            }
            finally
            {
                e.Handled = true;
            }
        }

        int Digital(ItemModbus Item, int countDigital)
        {
            if ((countDigital - Item.Formula.Length) != 0)
            {
                if (char.IsDigit(Item.Formula, countDigital))
                {
                    countDigital++;

                    countDigital = Digital(Item, countDigital);
                }
            }

            return countDigital;
        }

        

        void Connect(object SelectSerialPort)
        {
            if (!IsStop)
            {
                try
                {                   
                    SerialPort = new SerialPort();

                    ModbusSendConnect param = (ModbusSendConnect)SelectSerialPort;

                    foreach (ComSer comSer in ((AppWPF)Application.Current).CollectionComSers)
                    {
                        if (comSer.ComPort != null && comSer.ComPort == param.ComPort)
                        {
                            SerialPort.PortName = comSer.ComPort;
                            SerialPort.BaudRate = comSer.BaudRate;
                            SerialPort.DataBits = comSer.DataBits;
                            SerialPort.ReadTimeout = comSer.ReadTimeout;
                            SerialPort.WriteTimeout = comSer.WriteTimeout;

                            if (comSer.StopBits == "None")
                            {
                                SerialPort.StopBits = StopBits.None;
                            }
                            else if (comSer.StopBits == "One")
                            {
                                SerialPort.StopBits = StopBits.One;
                            }
                            else if (comSer.StopBits == "OnePointFive")
                            {
                                SerialPort.StopBits = StopBits.OnePointFive;
                            }
                            else if (comSer.StopBits == "Two")
                            {
                                SerialPort.StopBits = StopBits.Two;
                            }

                            if (comSer.Parity == "Even")
                            {
                                SerialPort.Parity = Parity.Even;
                            }
                            else if (comSer.Parity == "Mark")
                            {
                                SerialPort.Parity = Parity.Mark;
                            }
                            else if (comSer.Parity == "None")
                            {
                                SerialPort.Parity = Parity.None;
                            }
                            else if (comSer.Parity == "Odd")
                            {
                                SerialPort.Parity = Parity.Odd;
                            }
                            else if (comSer.Parity == "Space")
                            {
                                SerialPort.Parity = Parity.Space;
                            }

                            break;
                        }
                    }

                    SerialPort.Open();                 

                    Stopwatch timeCheckExit = new Stopwatch();
                    ushort[] data = new ushort[2];
                    byte[] buffer = new byte[8];

                    ModbusSerialMaster modbus = null;

                    if(param.Protocol == "RTU")
                    {
                        modbus = ModbusSerialMaster.CreateRtu(SerialPort);
                    }
                    else
                    {
                        modbus = ModbusSerialMaster.CreateAscii(SerialPort);
                    }
                   
                    modbus.Transport.Retries = 1;

                    PModbus pModbus = null;

                    if (param.IsUS800)
                    {
                        pModbus = new PModbus();
                    }

                    ushort temp;

                    while (true)
                    {
                        timeCheckExit.Start();
             
                        foreach (ItemModbus item in NewModbusSer.CollectionItemModbus)
                        {
                            if (item.TypeValue == "float")
                            {
                                if (param.IsUS800)
                                {
                                    if (item.Function == 3)
                                    {
                                        item.Value = pModbus.SendFc3(SerialPort, ((ModbusSendConnect)SelectSerialPort).Slave, item.Address, 2, ref data, ref buffer);
                                    }
                                    else if (item.Function == 4)
                                    {
                                        item.Value = pModbus.SendFc4(SerialPort, ((ModbusSendConnect)SelectSerialPort).Slave, item.Address, 2, ref data, ref buffer);
                                    }                                  
                                }
                                else
                                {
                                    if(item.Function == 3)
                                    {
                                        data = modbus.ReadHoldingRegisters(((ModbusSendConnect)SelectSerialPort).Slave, item.Address, 2);
                                    }
                                    else if (item.Function == 4)
                                    {
                                        data = modbus.ReadInputRegisters(((ModbusSendConnect)SelectSerialPort).Slave, item.Address, 2);
                                    }
                                    
                                    if (data != null)
                                    {
                                        temp = data[0];

                                        data[0] = data[1];
                                        data[1] = temp;

                                        buffer[0] = BitConverter.GetBytes(data[0])[0];
                                        buffer[1] = BitConverter.GetBytes(data[0])[1];
                                        buffer[2] = BitConverter.GetBytes(data[1])[0];
                                        buffer[3] = BitConverter.GetBytes(data[1])[1];

                                        if (item.Formula.Length != 0)
                                        {
                                            List<float> collectionDigital = new List<float>();

                                            int countDigital = 0;
                                            
                                            if (item.Formula[0] == '/')
                                            {
                                                countDigital++;

                                                if ((countDigital - item.Formula.Length) != 0)
                                                {
                                                    countDigital = Digital(item, countDigital);

                                                    collectionDigital.Add(float.Parse(item.Formula.Substring(1, countDigital - 1)));

                                                    item.Value = BitConverter.ToSingle(buffer, 0) / collectionDigital[0];
                                                }
                                                else
                                                {
                                                    item.Value = BitConverter.ToSingle(buffer, 0);
                                                }
                                            }
                                            else if (item.Formula.IndexOf("CSF") != -1)
                                            {
                                                if (item.Formula[3] == '/')
                                                {
                                                    countDigital = 3;

                                                    countDigital++;

                                                    if ((countDigital - item.Formula.Length) != 0)
                                                    {
                                                        countDigital = Digital(item, countDigital);

                                                        collectionDigital.Add(float.Parse(item.Formula.Substring(4, countDigital - 4)));

                                                        item.Value = BitConverter.ToInt16(buffer, 2) / collectionDigital[0];
                                                    }
                                                    else
                                                    {
                                                        item.Value = BitConverter.ToInt16(buffer, 2);
                                                    }
                                                }
                                                else
                                                {
                                                    item.Value = BitConverter.ToInt16(buffer, 2);
                                                }
                                            }
                                            else
                                            {
                                                item.Value = BitConverter.ToSingle(buffer, 0);
                                            }
                                        }
                                        else
                                        {
                                            item.Value = BitConverter.ToSingle(buffer, 0);
                                        }
                                    }
                                }                              
                            }
                            else if (item.TypeValue == "short")
                            {
                                if (item.Function == 3)
                                {
                                    data = modbus.ReadHoldingRegisters(((ModbusSendConnect)SelectSerialPort).Slave, item.Address, 1);
                                }
                                else if (item.Function == 4)
                                {
                                    data = modbus.ReadInputRegisters(((ModbusSendConnect)SelectSerialPort).Slave, item.Address, 1);
                                }                              

                                if (data != null)
                                {
                                    buffer[0] = BitConverter.GetBytes(data[0])[0];
                                    buffer[1] = BitConverter.GetBytes(data[0])[1];

                                    item.Value = BitConverter.ToInt16(buffer, 0);
                                }                        
                            }
                            else if (item.TypeValue == "ushort")
                            {
                                if (item.Function == 3)
                                {
                                    data = modbus.ReadHoldingRegisters(((ModbusSendConnect)SelectSerialPort).Slave, item.Address, 1);
                                }
                                else if (item.Function == 4)
                                {
                                    data = modbus.ReadInputRegisters(((ModbusSendConnect)SelectSerialPort).Slave, item.Address, 1);
                                }
                               
                                if (data != null)
                                {
                                    buffer[0] = BitConverter.GetBytes(data[0])[0];
                                    buffer[1] = BitConverter.GetBytes(data[0])[1];

                                    item.Value = BitConverter.ToUInt16(buffer, 0);
                                } 
                            }
                            else if (item.TypeValue == "int")
                            {
                                if (item.Function == 3)
                                {
                                    data = modbus.ReadHoldingRegisters(((ModbusSendConnect)SelectSerialPort).Slave, item.Address, 2);
                                }
                                else if (item.Function == 4)
                                {
                                    data = modbus.ReadInputRegisters(((ModbusSendConnect)SelectSerialPort).Slave, item.Address, 2);
                                }
                                
                                if (data != null)
                                {
                                    temp = data[0];

                                    data[0] = data[1];
                                    data[1] = temp;

                                    buffer[0] = BitConverter.GetBytes(data[0])[0];
                                    buffer[1] = BitConverter.GetBytes(data[0])[1];
                                    buffer[2] = BitConverter.GetBytes(data[1])[0];
                                    buffer[3] = BitConverter.GetBytes(data[1])[1];

                                    if (item.Formula.Length != 0)
                                    {
                                        List<int> collectionDigital = new List<int>();

                                        int countDigital = 0;
                                        
                                        if (item.Formula[0] == '/')
                                        {
                                            countDigital++;

                                            if ((countDigital - item.Formula.Length) != 0)
                                            {
                                                countDigital = Digital(item, countDigital);

                                                collectionDigital.Add(int.Parse(item.Formula.Substring(1, countDigital - 1)));

                                                item.Value = BitConverter.ToInt32(buffer, 0) / collectionDigital[0];
                                            }
                                            else
                                            {
                                                item.Value = BitConverter.ToInt32(buffer, 0);
                                            }
                                        }                                        
                                    }
                                    else
                                    {
                                        item.Value = BitConverter.ToInt32(buffer, 0);
                                    }
                                }
                            }
                            else if (item.TypeValue == "uint")
                            {
                                if (item.Function == 3)
                                {
                                    data = modbus.ReadHoldingRegisters(((ModbusSendConnect)SelectSerialPort).Slave, item.Address, 2);
                                }
                                else if (item.Function == 4)
                                {
                                    data = modbus.ReadInputRegisters(((ModbusSendConnect)SelectSerialPort).Slave, item.Address, 2);
                                }
                              
                                if (data != null)
                                {
                                    temp = data[0];

                                    data[0] = data[1];
                                    data[1] = temp;

                                    buffer[0] = BitConverter.GetBytes(data[0])[0];
                                    buffer[1] = BitConverter.GetBytes(data[0])[1];
                                    buffer[2] = BitConverter.GetBytes(data[1])[0];
                                    buffer[3] = BitConverter.GetBytes(data[1])[1];

                                    item.Value = BitConverter.ToUInt32(buffer, 0);
                                }
                            }                            
                        }

                        while (true)
                        {
                            Thread.Sleep(StaticValues.TimeSleep);

                            if (IsStop)
                            {                                
                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    TBStatus.Text = "Статус: Опрос остановлен.";
                                }));

                                return;
                            }
                           
                            if (timeCheckExit.ElapsedMilliseconds >= (PeriodTime * 1000))
                            {
                                timeCheckExit.Reset();
                                break;
                            }
                        }                                              
                    }
                }                
                catch (SystemException ex)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        TBStatus.Text = "Статус: " + ex;
                    }));
                }
                finally
                {
                    if (SerialPort != null)
                    {
                        SerialPort.Close();
                    }

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        IsBindingStart = false;
                        Stop.IsEnabled = false;
                        AddNetButton.IsEnabled = true;                        
                        TBTime.IsReadOnly = false;                      
                        CBProtocol.IsEnabled = true;

                        foreach (DataGridColumn column in DG.Columns)
                        {
                            if (column.Header == StaticValue.SRange0)
                            {
                                column.IsReadOnly = false;
                            }
                            else if (column.Header == StaticValue.SRange1)
                            {
                                column.IsReadOnly = false;
                            }
                            else if (column.Header == "Тип")
                            {
                                column.IsReadOnly = false;
                            }
                        }
                    }));

                    IsStop = false;
                }
            }
        }

        void DG_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            Item = (ItemModbus)e.Row.Item;

            ContentPresenter element = (ContentPresenter)e.EditingElement;

            if (element.ContentTemplate.FindName("ComboBoxType", element) != null)
            {
                ComboBox cbType = (ComboBox)element.ContentTemplate.FindName("ComboBoxType", element);

                ComboBoxItem cbFloat = new ComboBoxItem();
                cbFloat.ToolTip = "Число одинарной точности, 4 байта.";
                cbFloat.Content = "float";

                ComboBoxItem cbDouble = new ComboBoxItem();
                cbDouble.ToolTip = "Число двойной точности, 8 байт.";
                cbDouble.Content = "double";

                ComboBoxItem cbDecimal = new ComboBoxItem();
                cbDecimal.ToolTip = "Десятичное число, 12 байт.";
                cbDecimal.Content = "decimal";

                ComboBoxItem cbByte = new ComboBoxItem();
                cbByte.ToolTip = "Целочисленный тип: 0-255, 1 байт.";
                cbByte.Content = "byte";

                ComboBoxItem cbSByte = new ComboBoxItem();
                cbSByte.ToolTip = "Целочисленный тип: -128-127, 1 байт.";
                cbSByte.Content = "sbyte";

                ComboBoxItem cbShort = new ComboBoxItem();
                cbShort.ToolTip = "Целочисленный тип: -32768-32767, 2 байта.";
                cbShort.Content = "short";

                ComboBoxItem cbUShort = new ComboBoxItem();
                cbUShort.ToolTip = "Целочисленный тип: 0-65535, 2 байта.";
                cbUShort.Content = "ushort";

                ComboBoxItem cbInt = new ComboBoxItem();
                cbInt.ToolTip = "Целочисленный тип: -2147483648-2147483647, 4 байта.";
                cbInt.Content = "int";

                ComboBoxItem cbUInt = new ComboBoxItem();
                cbUInt.ToolTip = "Целочисленный тип: 0-4294967295, 4 байта.";
                cbUInt.Content = "uint";

                ComboBoxItem cbLong = new ComboBoxItem();
                cbLong.ToolTip = "Целочисленный тип: -9223372036854775808-9223372036854775807, 8 байт.";
                cbLong.Content = "long";

                ComboBoxItem cbULong = new ComboBoxItem();
                cbULong.ToolTip = "Целочисленный тип: 0-18446744073709551615, 8 байт.";
                cbULong.Content = "ulong";

                //ComboBoxItem cbBool = new ComboBoxItem();
                //cbBool.ToolTip = "Логический тип, 1 байт.";
                //cbBool.Content = "bool";

                ComboBoxItem cbChar = new ComboBoxItem();
                cbChar.ToolTip = "Символьный тип, 2 байта.";
                cbChar.Content = "char";

                ComboBoxItem cbString = new ComboBoxItem();
                cbString.ToolTip = "Строковый тип.";
                cbString.Content = "string";

                List<ComboBoxItem> cbItem = new List<ComboBoxItem>();
                cbItem.Add(cbFloat);
                cbItem.Add(cbDouble);
                cbItem.Add(cbDecimal);
                cbItem.Add(cbByte);
                cbItem.Add(cbSByte);
                cbItem.Add(cbShort);
                cbItem.Add(cbUShort);
                cbItem.Add(cbInt);
                cbItem.Add(cbUInt);
                cbItem.Add(cbLong);
                cbItem.Add(cbULong);
                //cbItem.Add(cbBool);
                cbItem.Add(cbChar);
                cbItem.Add(cbString);

                Binding bindingType = new Binding();
                bindingType.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                bindingType.Path = new PropertyPath("TypeValue");

                cbType.SetValue(ComboBox.ItemsSourceProperty, cbItem);
                cbType.SetValue(ComboBox.SelectedValuePathProperty, "Content");
                cbType.SetBinding(ComboBox.SelectedValueProperty, bindingType);

                EscText = (string)((ComboBoxItem)cbType.SelectedItem).Content;
            }
            else if (element.ContentTemplate.FindName("ComboBoxFunction", element) != null)
            {
                ComboBox cbType = (ComboBox)element.ContentTemplate.FindName("ComboBoxFunction", element);

                ComboBoxItem cbFloat = new ComboBoxItem();
                cbFloat.ToolTip = "0x03 - Read Holding Registers";
                cbFloat.Content = 3;

                ComboBoxItem cbDouble = new ComboBoxItem();
                cbDouble.ToolTip = "0x04 - Read Input Registers";
                cbDouble.Content = 4;
                
                List<ComboBoxItem> cbItem = new List<ComboBoxItem>();
                cbItem.Add(cbFloat);
                cbItem.Add(cbDouble);                

                Binding bindingFunction = new Binding();
                bindingFunction.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                bindingFunction.Path = new PropertyPath("Function");

                cbType.SetValue(ComboBox.ItemsSourceProperty, cbItem);
                cbType.SetValue(ComboBox.SelectedValuePathProperty, "Content");
                cbType.SetBinding(ComboBox.SelectedValueProperty, bindingFunction);

                EscDigital = (int)((ComboBoxItem)cbType.SelectedItem).Content;
            }
            else if (element.ContentTemplate.FindName("TextBoxDescription", element) != null)
            {
                TBDescriptionItemNet = (TextBox)element.ContentTemplate.FindName("TextBoxDescription", element);               
                TBDescriptionItemNet.AcceptsReturn = true;
                TBDescriptionItemNet.MaxLines = 3;

                EscText = TBDescriptionItemNet.Text;

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBDescriptionItemNet.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBDescriptionItemNet.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextTextBoxPaste));
                TBDescriptionItemNet.ContextMenu = ContextMenu;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBDescriptionItemNet.Focus();
                }));
            }
            else if (element.ContentTemplate.FindName("TextBoxTableName", element) != null)
            {
                TBTableName = (TextBox)element.ContentTemplate.FindName("TextBoxTableName", element);
                TBTableName.PreviewTextInput += Text_PreviewTextInput;
                EscText = TBTableName.Text;

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBTableName.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBTableName.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextTextBoxPaste));
                TBTableName.ContextMenu = ContextMenu;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBTableName.Focus();
                }));
            }
            else if (element.ContentTemplate.FindName("TextBoxPeriodSaveDB", element) != null)
            {
                TBPeriodTimeSaveDB = (TextBox)element.ContentTemplate.FindName("TextBoxPeriodSaveDB", element);
                TBPeriodTimeSaveDB.PreviewTextInput += DigitalTextBox_PreviewTextInput;

                float.TryParse(TBPeriodTimeSaveDB.Text, out EscDigital);

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBPeriodTimeSaveDB.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBPeriodTimeSaveDB.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
                TBPeriodTimeSaveDB.ContextMenu = ContextMenu;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBPeriodTimeSaveDB.Focus();
                }));
            }
            else if (element.ContentTemplate.FindName("TextBoxEmergencyUp", element) != null)
            {
                TBEmergencyUp = (TextBox)element.ContentTemplate.FindName("TextBoxEmergencyUp", element);
                TBEmergencyUp.PreviewTextInput += DigitalTextBox_PreviewTextInput;
                TBEmergencyUp.Text = TBEmergencyUp.Text.Replace(',', '.');

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBEmergencyUp.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBEmergencyUp.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
                TBEmergencyUp.ContextMenu = ContextMenu;

                if (Item.TypeValue == "bool")
                {
                    TBEmergencyUp.IsReadOnly = true;
                }
                else
                {
                    float.TryParse(TBEmergencyUp.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out EscDigital);

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        TBEmergencyUp.Focus();
                    }));
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxEmergencyDown", element) != null)
            {
                TBEmergencyDown = (TextBox)element.ContentTemplate.FindName("TextBoxEmergencyDown", element);
                TBEmergencyDown.PreviewTextInput += DigitalTextBox_PreviewTextInput;
                TBEmergencyDown.Text = TBEmergencyDown.Text.Replace(',', '.');

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBEmergencyDown.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBEmergencyDown.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
                TBEmergencyDown.ContextMenu = ContextMenu;

                if (Item.TypeValue == "bool")
                {
                    TBEmergencyDown.IsReadOnly = true;
                }
                else
                {
                    float.TryParse(TBEmergencyDown.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out EscDigital);

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        TBEmergencyDown.Focus();
                    }));
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxPeriodEmergencySaveDB", element) != null)
            {
                TBEmergencySaveBD = (TextBox)element.ContentTemplate.FindName("TextBoxPeriodEmergencySaveDB", element);
                TBEmergencySaveBD.PreviewTextInput += DigitalTextBox_PreviewTextInput;

                float.TryParse(TBEmergencySaveBD.Text, out EscDigital);

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBEmergencySaveBD.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBEmergencySaveBD.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
                TBEmergencySaveBD.ContextMenu = ContextMenu;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBEmergencySaveBD.Focus();
                }));
            }
            else if (element.ContentTemplate.FindName("TextBoxAddress", element) != null)
            {
                TBAddress = (TextBox)element.ContentTemplate.FindName("TextBoxAddress", element);
                TBAddress.PreviewTextInput += DigitalTextBox_PreviewTextInput;

                float.TryParse(TBAddress.Text, out EscDigital);

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBAddress.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBAddress.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
                TBAddress.ContextMenu = ContextMenu;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBAddress.Focus();
                }));
            }
            else if (element.ContentTemplate.FindName("TextBoxFormula", element) != null)
            {
                TBFormula = (TextBox)element.ContentTemplate.FindName("TextBoxFormula", element);
                TBFormula.PreviewTextInput += Text_PreviewTextInput;
                TBFormula.AcceptsReturn = true;
                TBFormula.MaxLines = 3;

                EscText = TBFormula.Text;

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBFormula.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBFormula.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextTextBoxPaste));
                TBFormula.ContextMenu = ContextMenu;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBFormula.Focus();
                }));
            }
            else if (element.ContentTemplate.FindName("TextBoxText", element) != null)
            {
                TBText = (TextBox)element.ContentTemplate.FindName("TextBoxText", element);
                TBText.PreviewTextInput += Text_PreviewTextInput;
                TBText.AcceptsReturn = true;
                TBText.MaxLines = 3;

                EscText = TBText.Text;

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBText.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBText.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextTextBoxPaste));
                TBText.ContextMenu = ContextMenu;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBText.Focus();
                }));
            }      
        }

        void DG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ContentPresenter element = (ContentPresenter)e.EditingElement;
                        
            if (element.ContentTemplate.FindName("TextBoxFormula", element) != null)
            {
                TextBox tbFormula = (TextBox)element.ContentTemplate.FindName("TextBoxFormula", element);

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {                    
                    tbFormula.Text = EscText;
                }
            }      
            else if (element.ContentTemplate.FindName("TextBoxText", element) != null)
            {
                TextBox tbText = (TextBox)element.ContentTemplate.FindName("TextBoxText", element);

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {                    
                    tbText.Text = EscText;
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxDescription", element) != null)
            {
                TextBox tbDescription = (TextBox)element.ContentTemplate.FindName("TextBoxDescription", element);

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {                    
                    tbDescription.Text = EscText;
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxTableName", element) != null)
            {
                TextBox tbTableName = (TextBox)element.ContentTemplate.FindName("TextBoxTableName", element);

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    tbTableName.Text = EscText;
                }
            }
            else if (element.ContentTemplate.FindName("ComboBoxType", element) != null)
            {
                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    ComboBox cbType = (ComboBox)element.ContentTemplate.FindName("ComboBoxType", element);
                    cbType.SelectedValue = EscText;
                }
            }
            else if (element.ContentTemplate.FindName("ComboBoxFunction", element) != null)
            {
                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    ComboBox cbFunction = (ComboBox)element.ContentTemplate.FindName("ComboBoxFunction", element);
                    cbFunction.SelectedValue = EscDigital;
                }
            }           
            else if (element.ContentTemplate.FindName("TextBoxPeriodSaveDB", element) != null)
            {
                TextBox periodTextBox = (TextBox)element.ContentTemplate.FindName("TextBoxPeriodSaveDB", element);

                if (periodTextBox.Text.Length == 0)
                {
                    periodTextBox.Text = EscDigital.ToString();
                }

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    periodTextBox.Text = EscDigital.ToString();
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxPeriodEmergencySaveDB", element) != null)
            {
                TextBox emergencySaveTextBox = (TextBox)element.ContentTemplate.FindName("TextBoxPeriodEmergencySaveDB", element);

                if (emergencySaveTextBox.Text.Length == 0)
                {
                    emergencySaveTextBox.Text = EscDigital.ToString();
                }

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    emergencySaveTextBox.Text = EscDigital.ToString();
                }
            }            
            else if (element.ContentTemplate.FindName("TextBoxAddress", element) != null)
            {
                TextBox addressTextBox = (TextBox)element.ContentTemplate.FindName("TextBoxAddress", element);

                if (addressTextBox.Text.Length == 0)
                {
                    addressTextBox.Text = EscDigital.ToString();
                }

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    addressTextBox.Text = EscDigital.ToString();
                }
            }           
            else if (element.ContentTemplate.FindName("TextBoxEmergencyDown", element) != null)
            {
                TextBox EmergencyDownTextBox = (TextBox)element.ContentTemplate.FindName("TextBoxEmergencyDown", element);

                if (EmergencyDownTextBox.Text.Length == 0)
                {
                    EmergencyDownTextBox.Text = EscDigital.ToString();
                }

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    EmergencyDownTextBox.Text = EscDigital.ToString(CultureInfo.InvariantCulture);
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxEmergencyUp", element) != null)
            {                
                TextBox EmergencyUpTextBox = (TextBox)element.ContentTemplate.FindName("TextBoxEmergencyUp", element);

                if (EmergencyUpTextBox.Text.Length == 0)
                {
                    EmergencyUpTextBox.Text = EscDigital.ToString();
                }

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    EmergencyUpTextBox.Text = EscDigital.ToString(CultureInfo.InvariantCulture);
                }
            }           
        }        

        void LoadedType(Object sender, RoutedEventArgs e)
        {
            Label l = (Label)sender;

            if (l.Content == "float")
            {
                l.ToolTip = "Число одинарной точности, 4 байта.";
            }
            else if (l.Content == "double")
            {
                l.ToolTip = "Число двойной точности, 8 байт.";
            }
            else if (l.Content == "decimal")
            {
                l.ToolTip = "Десятичное число, 12 байт.";
            }
            else if (l.Content == "byte")
            {
                l.ToolTip = "Целочисленный тип: 0-255, 1 байт.";
            }
            else if (l.Content == "sbyte")
            {
                l.ToolTip = "Целочисленный тип: -128-127, 1 байт.";
            }
            else if (l.Content == "short")
            {
                l.ToolTip = "Целочисленный тип: -32768-32767, 2 байта.";
            }
            else if (l.Content == "ushort")
            {
                l.ToolTip = "Целочисленный тип: 0-65535, 2 байта.";
            }
            else if (l.Content == "int")
            {
                l.ToolTip = "Целочисленный тип: -2147483648-2147483647, 4 байта.";
            }
            else if (l.Content == "uint")
            {
                l.ToolTip = "Целочисленный тип: 0-4294967295, 4 байта.";
            }
            else if (l.Content == "long")
            {
                l.ToolTip = "Целочисленный тип: -9223372036854775808-9223372036854775807, 8 байт.";
            }
            else if (l.Content == "ulong")
            {
                l.ToolTip = "Целочисленный тип: 0-18446744073709551615, 8 байт.";
            }
            else if (l.Content == "bool")
            {
                l.ToolTip = "Логический тип, 1 байт.";
            }
            else if (l.Content == "char")
            {
                l.ToolTip = "Символьный тип, 2 байта.";
            }
            else if (l.Content == "string")
            {
                l.ToolTip = "Строковый тип.";
            }

            e.Handled = true;
        }

        void LoadedFunction(Object sender, RoutedEventArgs e)
        {
            Label l = (Label)sender;

            if (l.Content != null)
            {
                if ((int)l.Content == 3)
                {
                    l.ToolTip = "0x03 - Read Holding Registers";
                }
                else if ((int)l.Content == 4)
                {
                    l.ToolTip = "0x04 - Read Input Registers";
                }
            }
                     
            e.Handled = true;
        }

        void AddNetButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddItemModbusWindow addItemNetWindow = new AddItemModbusWindow(NewModbusSer.CollectionItemModbus);

            addItemNetWindow.ShowDialog();
        }

        private void IgnoreCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }
       
        #region DigitalTextBox
        private void DigitalTextBoxPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            TextBox tb = (TextBox)sender;
            string s;

            if (tb == TBTime || tb == TBPeriodTimeSaveDB || tb == TBEmergencySaveBD)
            {
                string pattern = @"^\d{1,5}$";               

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    if (tb == TBTime)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = StaticValue.SRange1_86400;
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;
                    }
                    else if (tb == TBPeriodTimeSaveDB)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e);
                    }
                    else if (tb == TBEmergencySaveBD)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e);
                    }

                    return;
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 1 || d > 86400)
                    {
                        if (tb == TBTime)
                        {
                            PopupMessage.HorizontalOffset = 0;
                            LPopupMessage.Content = StaticValue.SRange1_86400;
                            PopupMessage.PlacementTarget = tb;
                            PopupMessage.IsOpen = true;
                        }
                        else if (tb == TBPeriodTimeSaveDB)
                        {
                            PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e);
                        }
                        else if (tb == TBEmergencySaveBD)
                        {
                            PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e);
                        }
                    }
                    else
                    {
                        PopupMessage.IsOpen = false;

                        tb.Paste();
                    }
                }
                else
                {
                    if (tb == TBTime)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = StaticValue.SRange1_86400;
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;
                    }
                    else if (tb == TBPeriodTimeSaveDB)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e);
                    }
                    else if (tb == TBEmergencySaveBD)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e);
                    }
                }
            }
            else if (tb == TBAddressSlave)
            {
                string pattern = @"^\d{1,3}$";

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SRange1_255;
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 1 || d > 255)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = StaticValue.SRange1_255;
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;
                    }
                    else
                    {
                        PopupMessage.IsOpen = false;

                        tb.Paste();
                    }
                }
                else
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SRange1_255;
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;
                }
            }
            else if(tb == TBAddress)
            {
                string pattern = @"^\d{1,5}$";

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    PopupMessageShow(StaticValue.SAddress, tb, StaticValue.SRange0_65535, e);
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > 65535)
                    {
                        PopupMessageShow(StaticValue.SAddress, tb, StaticValue.SRange0_65535, e);
                    }
                    else
                    {
                        PopupMessage.IsOpen = false;

                        tb.Paste();
                    }
                }
                else
                {
                    PopupMessageShow(StaticValue.SAddress, tb, StaticValue.SRange0_65535, e);
                }
            }
            else if (tb == TBEmergencyUp || tb == TBEmergencyDown)
            {                
                if (Item.TypeValue == "float")
                {
                    string pattern = @"^\d{1,8}(?:\.\d{0,6})?$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Неверный формат", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Неверный формат", e);
                        }
                    }
                    else
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            float up = 0;
                            float down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                float.TryParse(s, out up);
                                down = Convert.ToSingle(((ItemModbus)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                float.TryParse(s, out down);
                                up = Convert.ToSingle(((ItemModbus)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e);
                                }
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                }
                else if (Item.TypeValue == "short")
                {
                    string pattern = @"^\d{1,5}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    short d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -32768-32767", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -32768-32767", e);
                        }
                    }
                    else if (short.TryParse(s, out d))
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            short up = 0;
                            short down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                short.TryParse(s, out up);
                                down = Convert.ToInt16(((ItemModbus)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                short.TryParse(s, out down);
                                up = Convert.ToInt16(((ItemModbus)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e);
                                }
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -32768-32767", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -32768-32767", e);
                        }
                    }
                }
                else if (Item.TypeValue == "ushort")
                {
                    string pattern = @"^\d{1,5}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    ushort d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange0_65535, e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange0_65535, e);
                        }
                    }
                    else if (ushort.TryParse(s, out d))
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            ushort up = 0;
                            ushort down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                ushort.TryParse(s, out up);
                                down = Convert.ToUInt16(((ItemModbus)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                ushort.TryParse(s, out down);
                                up = Convert.ToUInt16(((ItemModbus)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e);
                                }
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange0_65535, e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange0_65535, e);
                        }
                    }
                }
                else if (Item.TypeValue == "int")
                {
                    string pattern = @"^\d{1,10}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    int d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange_2147483648_2147483647, e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange_2147483648_2147483647, e);
                        }
                    }
                    else if (int.TryParse(s, out d))
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            int up = 0;
                            int down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                int.TryParse(s, out up);
                                down = Convert.ToInt32(((ItemModbus)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                int.TryParse(s, out down);
                                up = Convert.ToInt32(((ItemModbus)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e);
                                }
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange_2147483648_2147483647, e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange_2147483648_2147483647, e);
                        }
                    }
                }
                else if (Item.TypeValue == "uint")
                {
                    string pattern = @"^\d{1,10}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    uint d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-4294967295", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-4294967295", e);
                        }
                    }
                    else if (uint.TryParse(s, out d))
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            uint up = 0;
                            uint down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                uint.TryParse(s, out up);
                                down = Convert.ToUInt32(((ItemModbus)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                uint.TryParse(s, out down);
                                up = Convert.ToUInt32(((ItemModbus)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e);
                                }
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-4294967295", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-4294967295", e);
                        }
                    }
                }
            }
        }

        void DigitalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PopupMessage.IsOpen = false;

            TextBox tb = (TextBox)sender;
            string s;

            if (tb == TBTime || tb == TBPeriodTimeSaveDB || tb == TBEmergencySaveBD)
            {
                string pattern = @"^\d{1,5}$";

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, e.Text);
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, e.Text);
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    if (tb == TBTime)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = StaticValue.SRange1_86400;
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;

                        e.Handled = true;
                    }
                    else if (tb == TBPeriodTimeSaveDB)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e);
                    }
                    else if (tb == TBEmergencySaveBD)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e);
                    }                    

                    return;
                }
                if (double.TryParse(s, out d))
                {
                    if (d < 1 || d > 86400)
                    {
                        if (tb == TBTime)
                        {
                            PopupMessage.HorizontalOffset = 0;
                            LPopupMessage.Content = StaticValue.SRange1_86400;
                            PopupMessage.PlacementTarget = tb;
                            PopupMessage.IsOpen = true;

                            e.Handled = true;
                        }
                        else if (tb == TBPeriodTimeSaveDB)
                        {
                            PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e);
                        }
                        else if (tb == TBEmergencySaveBD)
                        {
                            PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e);
                        }
                    }
                }
                else
                {
                    if (tb == TBTime)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = StaticValue.SRange1_86400;
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;

                        e.Handled = true;
                    }
                    else if (tb == TBPeriodTimeSaveDB)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e);
                    }
                    else if (tb == TBEmergencySaveBD)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e);
                    }
                }
            }
            else if (tb == TBAddressSlave)
            {
                string pattern = @"^\d{1,3}$";

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, e.Text);
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, e.Text);
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SRange1_255;
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }

                if (double.TryParse(s, out d))
                {
                    if (d < 1 || d > 255)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = StaticValue.SRange1_255;
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;

                        e.Handled = true;
                    }
                }
                else
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SRange1_255;
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }
            }
            else if(tb == TBAddress)
            {
                string pattern = @"^\d{1,5}$";                

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, e.Text);
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, e.Text);
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    PopupMessageShow(StaticValue.SAddress, tb, StaticValue.SRange0_65535, e);
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > 65535)
                    {
                        PopupMessageShow(StaticValue.SAddress, tb, StaticValue.SRange0_65535, e);
                    }
                }
                else
                {
                    PopupMessageShow(StaticValue.SAddress, tb, StaticValue.SRange0_65535, e);
                }
            }
            else if (tb == TBEmergencyUp || tb == TBEmergencyDown)
            {
                if (Item.TypeValue == "float")
                {
                    string pattern = @"^\d{1,8}(?:\.\d{0,6})?$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Неверный формат", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Неверный формат", e);
                        }
                    }
                    else
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            float up = 0;
                            float down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                float.TryParse(s, out up);
                                down = Convert.ToSingle(((ItemModbus)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                float.TryParse(s, out down);
                                up = Convert.ToSingle(((ItemModbus)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e);
                                }
                            }
                        }
                    }
                }
                else if (Item.TypeValue == "short")
                {
                    string pattern = @"^\d{1,5}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    short d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange_32768_32767, e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange_32768_32767, e);
                        }
                    }
                    else if (short.TryParse(s, out d))
                    {
                        short up = 0;
                        short down = 0;

                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            if (tb == TBEmergencyUp)
                            {
                                short.TryParse(s, out up);
                                down = Convert.ToInt16(((ItemModbus)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                short.TryParse(s, out down);
                                up = Convert.ToInt16(((ItemModbus)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange_32768_32767, e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange_32768_32767, e);
                        }
                    }
                }
                else if (Item.TypeValue == "ushort")
                {
                    string pattern = @"^\d{1,5}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    ushort d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange0_65535, e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange0_65535, e);
                        }
                    }
                    else if (ushort.TryParse(s, out d))
                    {
                        ushort up = 0;
                        ushort down = 0;

                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            if (tb == TBEmergencyUp)
                            {
                                ushort.TryParse(s, out up);
                                down = Convert.ToUInt16(((ItemModbus)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                ushort.TryParse(s, out down);
                                up = Convert.ToUInt16(((ItemModbus)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange0_65535, e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange0_65535, e);
                        }
                    }
                }
                else if (Item.TypeValue == "int")
                {
                    string pattern = @"^\d{1,10}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    int d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange_2147483648_2147483647, e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange_2147483648_2147483647, e);
                        }
                    }
                    else if (int.TryParse(s, out d))
                    {
                        int up = 0;
                        int down = 0;

                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            if (tb == TBEmergencyUp)
                            {
                                int.TryParse(s, out up);
                                down = Convert.ToInt32(((ItemModbus)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                int.TryParse(s, out down);
                                up = Convert.ToInt32(((ItemModbus)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange_2147483648_2147483647, e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange_2147483648_2147483647, e);
                        }
                    }
                }
                else if (Item.TypeValue == "uint")
                {
                    string pattern = @"^\d{1,10}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    uint d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-4294967295", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-4294967295", e);
                        }
                    }
                    else if (uint.TryParse(s, out d))
                    {
                        uint up = 0;
                        uint down = 0;

                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            if (tb == TBEmergencyUp)
                            {
                                uint.TryParse(s, out up);
                                down = Convert.ToUInt32(((ItemModbus)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                uint.TryParse(s, out down);
                                up = Convert.ToUInt32(((ItemModbus)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-4294967295", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-4294967295", e);
                        }
                    }
                }
            }
        }
       
        #endregion                  
    }

    class GridPropertiesEthernetGeneral : Grid
    {
        public static readonly DependencyProperty IsBindingStartProperty =
          DependencyProperty.Register(
          "IsBindingStart",
          typeof(bool),
          typeof(GridPropertiesEthernetGeneral),
           new FrameworkPropertyMetadata(false));

        public bool IsBindingStart
        {
            get { return (bool)GetValue(GridPropertiesEthernetGeneral.IsBindingStartProperty); }
            set { SetValue(GridPropertiesEthernetGeneral.IsBindingStartProperty, value); }
        }

        public Popup PopupMessage = new Popup();
        public Label LPopupMessage = new Label();

        public bool IsEthernetOperational { get; set; }

        ItemNet Item;

        public ListBox ListEthernets = new ListBox();
        Button AddEthernet = new Button();
        Button RemoveEthernet = new Button();

        public EthernetSer NewEthernetSer;

        TcpClient TcpClient;
        UdpClient UdpClient;
        public Grid GridMain = new Grid();
        public ScrollViewer SVMain = new ScrollViewer();
        public TextBox TBIDEthernet = new TextBox();
        public TextBox TBDescriptionEthernet = new TextBox();
        public TextBox TBBufferSizeRec = new TextBox();
        public TextBox TBTime = new TextBox();
        public TextBox TBBufferSizeSend = new TextBox();
        public DataGrid DGRec = new DataGrid();
        public DataGrid DGSend = new DataGrid();
        public ComboBox CBProtocol = new ComboBox();
        public TextBox TBIPAdress1 = new TextBox();
        public TextBox TBIPAdress2 = new TextBox();
        public TextBox TBIPAdress3 = new TextBox();
        public TextBox TBIPAdress4 = new TextBox();
        public TextBox TBPortServer = new TextBox();
        public TextBox TBPortClient = new TextBox();
        public ComboBox CBLocalIPs = new ComboBox();
        Button RemoveNetButton;
        Button AddNetButton;
        Button RemoveNetButtonSend;
        Button AddNetButtonSend;
        public Button Start;
        Button Stop;
        EthernetControl EthernetControl;
        TextBox TBStatus;
        TextBox TBTableName;
        TextBox TBDescriptionItemNet;
        TextBox TBPeriodTimeSaveDB;
        TextBox TBFormula;
        TextBox TBText;
        TextBox TBEmergencyUp;
        TextBox TBEmergencyDown;
        TextBox TBEmergencySaveBD;
        TextBox TBRange0;
        TextBox TBRange1;

        int MaxDigit;

        string IPAddressServer;
        IPAddress IPAddressClient;
        int PortServer;
        int PortClient;
        int BufferSize;
        int BufferSizeSend;
        int PeriodTime;
        volatile bool IsStop; 

        string EscText;
        decimal EscDigital;

        StackPanel PanelButtonEthernets;

        public GridPropertiesEthernetGeneral(EthernetControl ethernetControl)
        {
            EthernetControl = ethernetControl;

            EthernetSer copyEthernetSer = null;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, ethernetControl.EthernetSer);

                ms.Position = 0;
                copyEthernetSer = (EthernetSer)bf.Deserialize(ms);
            }

            NewEthernetSer = copyEthernetSer;            
     
            LPopupMessage = new Label();
            LPopupMessage.BorderThickness = new Thickness(1);
            LPopupMessage.BorderBrush = Brushes.Red;
            LPopupMessage.Background = Brushes.White;

            PopupMessage.AllowsTransparency = true;
            PopupMessage.Child = LPopupMessage;
            PopupMessage.PopupAnimation = PopupAnimation.Fade;
            PopupMessage.StaysOpen = false;

            ListBoxItem lbEthernet = new ListBoxItem();
            lbEthernet.Tag = "1";
            lbEthernet.Content = ethernetControl.EthernetSer.Description;

            ListEthernets.MinWidth = 200;
            ListEthernets.MaxWidth = 300;
            ListEthernets.MinHeight = 50;
            ListEthernets.Items.Add(lbEthernet);            
            ListEthernets.Margin = new Thickness(5);
            ListEthernets.SetValue(Grid.RowProperty, 0);
            ListEthernets.SelectedItem = lbEthernet;

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                ListEthernets.SelectionChanged += ListEthernets_SelectionChanged;
            }));

            foreach(EthernetOperational ethernetOperational in NewEthernetSer.CollectionEthernetOperational)
            {
                ListBoxItem lbEthernetOperational = new ListBoxItem();
                lbEthernetOperational.Tag = ethernetOperational;
                lbEthernetOperational.Content = ethernetOperational.Description;

                ListEthernets.Items.Add(lbEthernetOperational);
            }

            Image removeEthernetImage = new Image();
            removeEthernetImage.Source = new BitmapImage(new Uri("Images/DeleteNet.png", UriKind.Relative));

            Image addEthernetImage = new Image();
            addEthernetImage.Source = new BitmapImage(new Uri("Images/AddNet.png", UriKind.Relative));

            Binding bindingEthernetDelete = new Binding();
            bindingEthernetDelete.Converter = new EthernetDeleteButtonConverter();
            bindingEthernetDelete.Source = ListEthernets;
            bindingEthernetDelete.Path = new PropertyPath("Items.Count");

            Binding bindingEthernetDelete2 = new Binding();
            bindingEthernetDelete2.Converter = new EthernetDeleteButtonConverter2();
            bindingEthernetDelete2.Source = ListEthernets;
            bindingEthernetDelete2.Path = new PropertyPath("SelectedItem");

            MultiBinding bindingEthernetDelete3 = new MultiBinding();
            bindingEthernetDelete3.Converter = new StartAddEthernetConverter();
            bindingEthernetDelete3.Bindings.Add(bindingEthernetDelete);
            bindingEthernetDelete3.Bindings.Add(bindingEthernetDelete2);

            RemoveEthernet.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            RemoveEthernet.SetBinding(Button.IsEnabledProperty, bindingEthernetDelete3);
            RemoveEthernet.Click += RemoveEthernet_Click;
            RemoveEthernet.Margin = new Thickness(3);
            RemoveEthernet.ToolTip = "Удалить Ethernet";
            RemoveEthernet.Content = removeEthernetImage;

            AddEthernet.Margin = new Thickness(3);
            AddEthernet.Click += AddEthernet_Click;
            AddEthernet.ToolTip = "Добавить Ethernet";
            AddEthernet.Content = addEthernetImage;

            PanelButtonEthernets = new StackPanel();
            PanelButtonEthernets.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            PanelButtonEthernets.Orientation = Orientation.Horizontal;
            PanelButtonEthernets.SetValue(Grid.RowProperty, 1);
            PanelButtonEthernets.Children.Add(AddEthernet);
            PanelButtonEthernets.Children.Add(RemoveEthernet);

            this.Unloaded += GridPropertiesEthernetGeneral_Unloaded;

            this.MaxWidth = 1000;
            this.MaxHeight = 800;           
                                                                                                                    
            List<IPAddress> Addresses = new List<IPAddress>();

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();

                    foreach (IPAddressInformation uniCast in properties.UnicastAddresses)
                    {
                        if (!IPAddress.IsLoopback(uniCast.Address) && uniCast.Address.AddressFamily != AddressFamily.InterNetworkV6)
                        {
                            Addresses.Add(uniCast.Address);
                        }
                    }
                }               
            }

            IPAddress ipClient = null;

            ipClient = new IPAddress(ethernetControl.EthernetSer.IPAddressClient);

            TBIDEthernet.MinWidth = 150;
            TBIDEthernet.Margin = new Thickness(3);
            TBIDEthernet.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            TBIDEthernet.MaxWidth = 300;
            TBIDEthernet.IsReadOnly = true;
       
            TBDescriptionEthernet.MinWidth = 150;            
            TBDescriptionEthernet.Margin = new Thickness(3);
            TBDescriptionEthernet.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            TBDescriptionEthernet.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            TBDescriptionEthernet.MaxWidth = 300;
            TBDescriptionEthernet.MaxHeight = 200;
            TBDescriptionEthernet.AcceptsReturn = true;

            CBLocalIPs.MinWidth = 150;
            CBLocalIPs.ItemsSource = Addresses;
            CBLocalIPs.SelectedItem = ipClient;

            TBIPAdress1.MinWidth = 40;
            TBIPAdress1.Margin = new Thickness(3);

            TBIPAdress2.MinWidth = 40;
            TBIPAdress2.Margin = new Thickness(3);

            TBIPAdress3.MinWidth = 40;
            TBIPAdress3.Margin = new Thickness(3);

            TBIPAdress4.MinWidth = 40;
            TBIPAdress4.Margin = new Thickness(3);

            TBPortServer.MinWidth = 40;
            TBPortServer.Margin = new Thickness(3);

            TBPortClient.MinWidth = 40;
            TBPortClient.Margin = new Thickness(3);

            TBBufferSizeRec.MinWidth = 40;
            TBBufferSizeRec.Margin = new Thickness(3);

            TBTime.MinWidth = 40;
            TBTime.Margin = new Thickness(3);

            TBBufferSizeSend.MinWidth = 40;
            TBBufferSizeSend.Margin = new Thickness(3);

            CBProtocol.Items.Add("TCP");
            CBProtocol.Items.Add("UDP");                                 

            #region DGRec
            Binding bindingCollectionNet = new Binding();
            bindingCollectionNet.NotifyOnSourceUpdated = true;
            bindingCollectionNet.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingCollectionNet.Source = NewEthernetSer.CollectionItemNetRec;

            DGRec.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            DGRec.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            DGRec.Margin = new Thickness(0, 0, 0, 5);
            DGRec.BorderThickness = new Thickness(3);
            DGRec.BorderBrush = Brushes.Black;
            DGRec.SetValue(Grid.RowProperty, 12);
            DGRec.AutoGenerateColumns = false;
            DGRec.CellEditEnding += DG_CellEditEnding;
            DGRec.PreparingCellForEdit += DG_PreparingCellForEdit;
            DGRec.MaxHeight = 250;
            DGRec.MaxWidth = 800;
            DGRec.SetBinding(DataGrid.ItemsSourceProperty, bindingCollectionNet);
                      
            Binding bindingType = new Binding();
            bindingType.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingType.Path = new PropertyPath("TypeValue");

            Binding bindingValue = new Binding();
            bindingValue.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingValue.Path = new PropertyPath("Value");

            Binding bindingRange0 = new Binding();
            bindingRange0.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingRange0.Path = new PropertyPath("Range0");

            Binding bindingRange1 = new Binding();
            bindingRange1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingRange1.Path = new PropertyPath("Range1");

            Binding bindingID = new Binding();
            bindingID.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingID.Path = new PropertyPath("ID");

            Binding bindingDescription = new Binding();
            bindingDescription.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingDescription.Path = new PropertyPath("Description");

            Binding bindingIsSaveDatabase = new Binding();
            bindingIsSaveDatabase.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsSaveDatabase.Path = new PropertyPath("IsSaveDatabase");

            Binding bindingTableName = new Binding();
            bindingTableName.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingTableName.Path = new PropertyPath("TableName");

            Binding bindingPeriodSaveDB = new Binding();
            bindingPeriodSaveDB.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingPeriodSaveDB.Path = new PropertyPath("PeridTimeSaveDB");

            Binding bindingFormula = new Binding();
            bindingFormula.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingFormula.Path = new PropertyPath("Formula");

            Binding bindingText = new Binding();
            bindingText.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingText.Path = new PropertyPath("Text");

            Binding bindingEmergencyUp = new Binding();
            bindingEmergencyUp.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingEmergencyUp.Path = new PropertyPath("EmergencyUp");

            Binding bindingEmergencyDown = new Binding();
            bindingEmergencyDown.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingEmergencyDown.Path = new PropertyPath("EmergencyDown");

            Binding bindingIsEmergencySaveDB = new Binding();
            bindingIsEmergencySaveDB.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsEmergencySaveDB.Path = new PropertyPath("IsEmergencySaveDB");

            Binding bindingPeriodEmergencySaveDB = new Binding();
            bindingPeriodEmergencySaveDB.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingPeriodEmergencySaveDB.Path = new PropertyPath("PeriodEmergencySaveDB");

            Binding bindingIsEmergencyUp = new Binding();
            bindingIsEmergencyUp.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsEmergencyUp.Path = new PropertyPath("IsEmergencyUpDG");

            Binding bindingIsEmergencyDown = new Binding();
            bindingIsEmergencyDown.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsEmergencyDown.Path = new PropertyPath("IsEmergencyDownDG");

            Binding bindingEmergencyUpEnable = new Binding();
            bindingEmergencyUpEnable.Path = new PropertyPath("IsEmergencyUpDG");

            Binding bindingEmergencyDownEnable = new Binding();
            bindingEmergencyDownEnable.Path = new PropertyPath("IsEmergencyDownDG");

            Binding bindingIsUp = new Binding();
            bindingIsUp.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsUp.Path = new PropertyPath("IsEmergencyUpDG");

            Binding bindingIsDown = new Binding();
            bindingIsDown.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsDown.Path = new PropertyPath("IsEmergencyDownDG");

            MultiBinding bindingIsEmergencyDB = new MultiBinding();
            bindingIsEmergencyDB.Converter = new CheckBoxEmergencySaveDBValueConverter();
            bindingIsEmergencyDB.Bindings.Add(bindingIsUp);
            bindingIsEmergencyDB.Bindings.Add(bindingIsDown);

            Binding bindingEnableTBPeriodEmergencySaveDB1 = new Binding();
            bindingEnableTBPeriodEmergencySaveDB1.Path = new PropertyPath("IsEmergencyUpDG");

            Binding bindingEnableTBPeriodEmergencySaveDB2 = new Binding();
            bindingEnableTBPeriodEmergencySaveDB2.Path = new PropertyPath("IsEmergencyDownDG");

            Binding bindingEnableTBPeriodEmergencySaveDB3 = new Binding();
            bindingEnableTBPeriodEmergencySaveDB3.Path = new PropertyPath("IsEmergencySaveDB");

            MultiBinding bindingEnableTBPeriodEmergencySaveDB4 = new MultiBinding();
            bindingEnableTBPeriodEmergencySaveDB4.Converter = new DGTBEmergencyEnabledValueConverter();
            bindingEnableTBPeriodEmergencySaveDB4.Bindings.Add(bindingEnableTBPeriodEmergencySaveDB1);
            bindingEnableTBPeriodEmergencySaveDB4.Bindings.Add(bindingEnableTBPeriodEmergencySaveDB2);
            bindingEnableTBPeriodEmergencySaveDB4.Bindings.Add(bindingEnableTBPeriodEmergencySaveDB3);
            
            FrameworkElementFactory lTypeEditable = new FrameworkElementFactory(typeof(ComboBox));
            lTypeEditable.Name = "ComboBoxType";

            FrameworkElementFactory lType = new FrameworkElementFactory(typeof(Label));
            lType.AddHandler(ComboBox.LoadedEvent, new RoutedEventHandler(LoadedType));
            lType.SetBinding(Label.ContentProperty, bindingType);

            FrameworkElementFactory lValue = new FrameworkElementFactory(typeof(Label));
            lValue.SetBinding(Label.ContentProperty, bindingValue);

            FrameworkElementFactory lRange0 = new FrameworkElementFactory(typeof(Label));
            lRange0.SetBinding(Label.ContentProperty, bindingRange0);

            FrameworkElementFactory lFormula = new FrameworkElementFactory(typeof(Label));
            lFormula.SetBinding(Label.ContentProperty, bindingFormula);

            FrameworkElementFactory lText = new FrameworkElementFactory(typeof(Label));
            lText.SetBinding(Label.ContentProperty, bindingText);

            FrameworkElementFactory lEmergencyUp = new FrameworkElementFactory(typeof(Label));
            lEmergencyUp.SetBinding(Label.ContentProperty, bindingEmergencyUp);

            FrameworkElementFactory lEmergencyDown = new FrameworkElementFactory(typeof(Label));
            lEmergencyDown.SetBinding(Label.ContentProperty, bindingEmergencyDown);

            FrameworkElementFactory lPeriodEmergencySaveDB = new FrameworkElementFactory(typeof(Label));
            lPeriodEmergencySaveDB.SetBinding(Label.ContentProperty, bindingPeriodEmergencySaveDB); 
           
            FrameworkElementFactory tbRange0Editable = new FrameworkElementFactory(typeof(TextBox));
            tbRange0Editable.Name = "TextBoxRange0";
            tbRange0Editable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            tbRange0Editable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            tbRange0Editable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            tbRange0Editable.SetBinding(TextBox.TextProperty, bindingRange0);

            FrameworkElementFactory lRange1 = new FrameworkElementFactory(typeof(Label));
            lRange1.SetBinding(Label.ContentProperty, bindingRange1);

            FrameworkElementFactory lFormulaEditable = new FrameworkElementFactory(typeof(TextBox));
            lFormulaEditable.Name = "TextBoxFormula";
            lFormulaEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            lFormulaEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(Text_PreviewTextInput));
            lFormulaEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lFormulaEditable.SetBinding(TextBox.TextProperty, bindingFormula);

            FrameworkElementFactory lTextEditable = new FrameworkElementFactory(typeof(TextBox));
            lTextEditable.Name = "TextBoxText";
            lTextEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(Text_PreviewTextInput));
            lTextEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lTextEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            lTextEditable.SetBinding(TextBox.TextProperty, bindingText);

            FrameworkElementFactory tbRange1Editable = new FrameworkElementFactory(typeof(TextBox));
            tbRange1Editable.Name = "TextBoxRange1";
            tbRange1Editable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            tbRange1Editable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            tbRange1Editable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            tbRange1Editable.SetBinding(TextBox.TextProperty, bindingRange1);
           
            FrameworkElementFactory lID = new FrameworkElementFactory(typeof(TextBox));
            lID.SetValue(TextBox.IsReadOnlyProperty, true);
            lID.SetBinding(TextBox.TextProperty, bindingID);

            FrameworkElementFactory lEthernetSerDescription = new FrameworkElementFactory(typeof(Label));
            lEthernetSerDescription.SetBinding(Label.ContentProperty, bindingDescription);
           
            FrameworkElementFactory lDescriptionEditable = new FrameworkElementFactory(typeof(TextBox));           
            lDescriptionEditable.Name = "TextBoxDescription";
            lDescriptionEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(Text_PreviewTextInput));
            lDescriptionEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lDescriptionEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            lDescriptionEditable.SetBinding(TextBox.TextProperty, bindingDescription);

            FrameworkElementFactory lEthernetSerIsSaveDatabase = new FrameworkElementFactory(typeof(CheckBox));
            lEthernetSerIsSaveDatabase.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            lEthernetSerIsSaveDatabase.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            lEthernetSerIsSaveDatabase.SetBinding(CheckBox.IsCheckedProperty, bindingIsSaveDatabase);

            FrameworkElementFactory lEthernetSerTableName = new FrameworkElementFactory(typeof(Label));
            lEthernetSerTableName.SetBinding(Label.ContentProperty, bindingTableName);

            FrameworkElementFactory lTableNameEditable = new FrameworkElementFactory(typeof(TextBox));
            lTableNameEditable.Name = "TextBoxTableName";
            lTableNameEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(Text_PreviewTextInput));
            lTableNameEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lTableNameEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            lTableNameEditable.SetBinding(TextBox.TextProperty, bindingTableName);

            FrameworkElementFactory lEthernetSerPeriodSaveDB = new FrameworkElementFactory(typeof(Label));
            lEthernetSerPeriodSaveDB.SetBinding(Label.ContentProperty, bindingPeriodSaveDB);

            FrameworkElementFactory lPeriodSaveDBEditable = new FrameworkElementFactory(typeof(TextBox));
            lPeriodSaveDBEditable.Name = "TextBoxPeriodSaveDB";
            lPeriodSaveDBEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            lPeriodSaveDBEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lPeriodSaveDBEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));           
            lPeriodSaveDBEditable.SetBinding(TextBox.TextProperty, bindingPeriodSaveDB);

            FrameworkElementFactory lEmergencyUpEditable = new FrameworkElementFactory(typeof(TextBox));
            lEmergencyUpEditable.Name = "TextBoxEmergencyUp";
            lEmergencyUpEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            lEmergencyUpEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lEmergencyUpEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));            
            lEmergencyUpEditable.SetBinding(TextBox.TextProperty, bindingEmergencyUp);
            lEmergencyUpEditable.SetBinding(TextBox.IsEnabledProperty, bindingEmergencyUpEnable);

            FrameworkElementFactory lEmergencyDownEditable = new FrameworkElementFactory(typeof(TextBox));
            lEmergencyDownEditable.Name = "TextBoxEmergencyDown";
            lEmergencyDownEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            lEmergencyDownEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lEmergencyDownEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));           
            lEmergencyDownEditable.SetBinding(TextBox.TextProperty, bindingEmergencyDown);
            lEmergencyDownEditable.SetBinding(TextBox.IsEnabledProperty, bindingEmergencyDownEnable);

            FrameworkElementFactory fIsEmergencySaveDB = new FrameworkElementFactory(typeof(CheckBox));
            fIsEmergencySaveDB.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            fIsEmergencySaveDB.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            fIsEmergencySaveDB.SetBinding(CheckBox.IsCheckedProperty, bindingIsEmergencySaveDB);
            fIsEmergencySaveDB.SetBinding(CheckBox.IsEnabledProperty, bindingIsEmergencyDB);

            FrameworkElementFactory fPeriodEmergencySaveDBEditable = new FrameworkElementFactory(typeof(TextBox));
            fPeriodEmergencySaveDBEditable.Name = "TextBoxPeriodEmergencySaveDB";
            fPeriodEmergencySaveDBEditable.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            fPeriodEmergencySaveDBEditable.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            fPeriodEmergencySaveDBEditable.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));            
            fPeriodEmergencySaveDBEditable.SetBinding(TextBox.TextProperty, bindingPeriodEmergencySaveDB);
            fPeriodEmergencySaveDBEditable.SetBinding(TextBox.IsEnabledProperty, bindingEnableTBPeriodEmergencySaveDB4);

            FrameworkElementFactory fIsEmergencyUp = new FrameworkElementFactory(typeof(CheckBox));
            fIsEmergencyUp.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            fIsEmergencyUp.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            fIsEmergencyUp.SetBinding(CheckBox.IsCheckedProperty, bindingIsEmergencyUp);

            FrameworkElementFactory fIsEmergencyDown = new FrameworkElementFactory(typeof(CheckBox));
            fIsEmergencyDown.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            fIsEmergencyDown.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            fIsEmergencyDown.SetBinding(CheckBox.IsCheckedProperty, bindingIsEmergencyDown);
          
            DataTemplate dataTemplateType = new DataTemplate();
            dataTemplateType.VisualTree = lType;

            DataTemplate dataTemplateTypeEditable = new DataTemplate();
            dataTemplateTypeEditable.VisualTree = lTypeEditable;

            DataTemplate dataTemplateValue = new DataTemplate();
            dataTemplateValue.VisualTree = lValue;

            DataTemplate dataTemplateRange0 = new DataTemplate();
            dataTemplateRange0.VisualTree = lRange0;

            DataTemplate dataTemplateRange0Editable = new DataTemplate();           
            dataTemplateRange0Editable.VisualTree = tbRange0Editable;
           
            DataTemplate dataTemplateRange1 = new DataTemplate();
            dataTemplateRange1.VisualTree = lRange1;

            DataTemplate dataTemplateRange1Editable = new DataTemplate();
            dataTemplateRange1Editable.VisualTree = tbRange1Editable;

            DataTemplate dataTemplateID = new DataTemplate();
            dataTemplateID.VisualTree = lID;

            DataTemplate dataTemplateDescription = new DataTemplate();
            dataTemplateDescription.VisualTree = lEthernetSerDescription;
           
            DataTemplate dataTemplateDescriptionEditable = new DataTemplate();
            dataTemplateDescriptionEditable.VisualTree = lDescriptionEditable;

            DataTemplate dataTemplateIsSaveDatabase = new DataTemplate();
            dataTemplateIsSaveDatabase.VisualTree = lEthernetSerIsSaveDatabase;

            DataTemplate dataTemplateTableName = new DataTemplate();
            dataTemplateTableName.VisualTree = lEthernetSerTableName;

            DataTemplate dataTemplateTableNameEditable = new DataTemplate();
            dataTemplateTableNameEditable.VisualTree = lTableNameEditable;

            DataTemplate dataTemplatePeriodSaveDB = new DataTemplate();
            dataTemplatePeriodSaveDB.VisualTree = lEthernetSerPeriodSaveDB;

            DataTemplate dataTemplatePeriodSaveDBEditable = new DataTemplate();
            dataTemplatePeriodSaveDBEditable.VisualTree = lPeriodSaveDBEditable;

            DataTemplate dataTemplateFormula = new DataTemplate();
            dataTemplateFormula.VisualTree = lFormula;

            DataTemplate dataTemplateFormulaEditable = new DataTemplate();
            dataTemplateFormulaEditable.VisualTree = lFormulaEditable;

            DataTemplate dataTemplateText = new DataTemplate();
            dataTemplateText.VisualTree = lText;

            DataTemplate dataTemplateTextEditable = new DataTemplate();
            dataTemplateTextEditable.VisualTree = lTextEditable;

            DataTemplate dataTemplateEmergencyUp = new DataTemplate();
            dataTemplateEmergencyUp.VisualTree = lEmergencyUp;

            DataTemplate dataTemplateEmergencyUpEditable = new DataTemplate();
            dataTemplateEmergencyUpEditable.VisualTree = lEmergencyUpEditable;

            DataTemplate dataTemplateEmergencyDown = new DataTemplate();
            dataTemplateEmergencyDown.VisualTree = lEmergencyDown;

            DataTemplate dataTemplateEmergencyDownEditable = new DataTemplate();
            dataTemplateEmergencyDownEditable.VisualTree = lEmergencyDownEditable;

            DataTemplate dataTemplatePeriodEmergencySaveDB = new DataTemplate();
            dataTemplatePeriodEmergencySaveDB.VisualTree = lPeriodEmergencySaveDB;

            DataTemplate dataTemplatePeriodEmergencySaveDBEditable = new DataTemplate();
            dataTemplatePeriodEmergencySaveDBEditable.VisualTree = fPeriodEmergencySaveDBEditable;

            DataTemplate dataTemplateIsEmergencySaveDBEditable = new DataTemplate();
            dataTemplateIsEmergencySaveDBEditable.VisualTree = fIsEmergencySaveDB;

            DataTemplate dataTemplateIsEmergencyUpEditable = new DataTemplate();
            dataTemplateIsEmergencyUpEditable.VisualTree = fIsEmergencyUp;

            DataTemplate dataTemplateIsEmergencyDownEditable = new DataTemplate();
            dataTemplateIsEmergencyDownEditable.VisualTree = fIsEmergencyDown;           

            DataGridTemplateColumn type = new DataGridTemplateColumn();
            type.Header = "Тип";
            type.CellTemplate = dataTemplateType;
            type.CellEditingTemplate = dataTemplateTypeEditable;

            DataGridTemplateColumn value = new DataGridTemplateColumn();
            value.Header = "Значение";
            value.CellTemplate = dataTemplateValue;
            value.IsReadOnly = true;

            DataGridTemplateColumn range0 = new DataGridTemplateColumn();
            range0.Header = StaticValue.SRange0;
            range0.CellTemplate = dataTemplateRange0;
            range0.CellEditingTemplate = dataTemplateRange0Editable;

            DataGridTemplateColumn range1 = new DataGridTemplateColumn();
            range1.Header = StaticValue.SRange1;
            range1.CellTemplate = dataTemplateRange1;
            range1.CellEditingTemplate = dataTemplateRange1Editable;

            DataGridTemplateColumn id = new DataGridTemplateColumn();
            id.Header = "ID";
            id.CellTemplate = dataTemplateID;

            DataGridTemplateColumn description = new DataGridTemplateColumn();
            description.Header = StaticValue.SDescription;
            description.CellTemplate = dataTemplateDescription;
            description.CellEditingTemplate = dataTemplateDescriptionEditable;

            DataGridTemplateColumn isSaveDatabase = new DataGridTemplateColumn();
            isSaveDatabase.Header = StaticValue.SIsSaveBD;
            isSaveDatabase.CellTemplate = dataTemplateIsSaveDatabase;

            DataGridTemplateColumn tableName = new DataGridTemplateColumn();
            tableName.Header = StaticValue.STableName;
            tableName.CellTemplate = dataTemplateTableName;
            tableName.CellEditingTemplate = dataTemplateTableNameEditable;

            DataGridTemplateColumn periodSaveDB = new DataGridTemplateColumn();
            periodSaveDB.Header = StaticValue.SPeriodSaveDB;
            periodSaveDB.CellTemplate = dataTemplatePeriodSaveDB;
            periodSaveDB.CellEditingTemplate = dataTemplatePeriodSaveDBEditable;

            DataGridTemplateColumn formula = new DataGridTemplateColumn();
            formula.Header = StaticValue.SFormula;
            formula.CellTemplate = dataTemplateFormula;
            formula.CellEditingTemplate = dataTemplateFormulaEditable;

            DataGridTemplateColumn text = new DataGridTemplateColumn();
            text.Header = StaticValue.SText;
            text.CellTemplate = dataTemplateText;
            text.CellEditingTemplate = dataTemplateTextEditable;

            DataGridTemplateColumn emergencyUp = new DataGridTemplateColumn();
            emergencyUp.Header = StaticValue.SSetUp;
            emergencyUp.CellTemplate = dataTemplateEmergencyUp;
            emergencyUp.CellEditingTemplate = dataTemplateEmergencyUpEditable;

            DataGridTemplateColumn emergencyDown = new DataGridTemplateColumn();
            emergencyDown.Header = StaticValue.SSetDown;
            emergencyDown.CellTemplate = dataTemplateEmergencyDown;
            emergencyDown.CellEditingTemplate = dataTemplateEmergencyDownEditable;

            DataGridTemplateColumn periodEmergencySaveDB = new DataGridTemplateColumn();
            periodEmergencySaveDB.Header = StaticValue.SPeriodSaveSetDB;
            periodEmergencySaveDB.CellTemplate = dataTemplatePeriodEmergencySaveDB;
            periodEmergencySaveDB.CellEditingTemplate = dataTemplatePeriodEmergencySaveDBEditable;

            DataGridTemplateColumn isEmergencySaveDB = new DataGridTemplateColumn();
            isEmergencySaveDB.Header = StaticValue.SIsSaveSetDB;
            isEmergencySaveDB.CellTemplate = dataTemplateIsEmergencySaveDBEditable;

            DataGridTemplateColumn isEmergencyUp = new DataGridTemplateColumn();
            isEmergencyUp.Header = StaticValue.SIsSetUp;
            isEmergencyUp.CellTemplate = dataTemplateIsEmergencyUpEditable;

            DataGridTemplateColumn isEmergencyDown = new DataGridTemplateColumn();
            isEmergencyDown.Header = StaticValue.SIsSetDown;
            isEmergencyDown.CellTemplate = dataTemplateIsEmergencyDownEditable;

            DGRec.Columns.Add(type);
            DGRec.Columns.Add(value);
            DGRec.Columns.Add(range0);
            DGRec.Columns.Add(range1);
            DGRec.Columns.Add(formula);
            DGRec.Columns.Add(text);
            DGRec.Columns.Add(description);
            DGRec.Columns.Add(isSaveDatabase);
            DGRec.Columns.Add(tableName);
            DGRec.Columns.Add(periodSaveDB);
            DGRec.Columns.Add(isEmergencyUp);
            DGRec.Columns.Add(emergencyUp);
            DGRec.Columns.Add(isEmergencyDown);
            DGRec.Columns.Add(emergencyDown);
            DGRec.Columns.Add(isEmergencySaveDB);
            DGRec.Columns.Add(periodEmergencySaveDB);
            DGRec.Columns.Add(id);

            DGRec.CanUserAddRows = false;

            #endregion

            #region DGSend       
            Binding bindingCollectionSend = new Binding();
            bindingCollectionSend.NotifyOnSourceUpdated = true;
            bindingCollectionSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingCollectionSend.Source = NewEthernetSer.CollectionItemNetSend;

            DGSend.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            DGSend.Margin = new Thickness(0, 5, 0, 0);
            DGSend.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            DGSend.BorderThickness = new Thickness(3);
            DGSend.BorderBrush = Brushes.Black;
            DGSend.SetValue(Grid.RowProperty, 14);
            DGSend.AutoGenerateColumns = false;
            DGSend.CellEditEnding += DG_CellEditEnding;
            DGSend.PreparingCellForEdit += DG_PreparingCellForEdit;
            DGSend.MaxHeight = 250;
            DGSend.MaxWidth = 800;
            DGSend.SetBinding(DataGrid.ItemsSourceProperty, bindingCollectionSend);

            Binding bindingTypeSend = new Binding();
            bindingTypeSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingTypeSend.Path = new PropertyPath("TypeValue");

            Binding bindingValueSend = new Binding();
            bindingValueSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingValueSend.Path = new PropertyPath("Value");

            Binding bindingRange0Send = new Binding();
            bindingRange0Send.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingRange0Send.Path = new PropertyPath("Range0");

            Binding bindingRange1Send = new Binding();
            bindingRange1Send.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingRange1Send.Path = new PropertyPath("Range1");

            Binding bindingIDSend = new Binding();
            bindingIDSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIDSend.Path = new PropertyPath("ID");

            Binding bindingDescriptionSend = new Binding();
            bindingDescriptionSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingDescriptionSend.Path = new PropertyPath("Description");

            Binding bindingIsSaveDatabaseSend = new Binding();
            bindingIsSaveDatabaseSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsSaveDatabaseSend.Path = new PropertyPath("IsSaveDatabase");

            Binding bindingTableNameSend = new Binding();
            bindingTableNameSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingTableNameSend.Path = new PropertyPath("TableName");

            Binding bindingPeriodSaveDBSend = new Binding();
            bindingPeriodSaveDBSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingPeriodSaveDBSend.Path = new PropertyPath("PeridTimeSaveDB");

            Binding bindingFormulaSend = new Binding();
            bindingFormulaSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingFormulaSend.Path = new PropertyPath("Formula");

            Binding bindingTextSend = new Binding();
            bindingTextSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingTextSend.Path = new PropertyPath("Text");

            Binding bindingEmergencyUpSend = new Binding();
            bindingEmergencyUpSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingEmergencyUpSend.Path = new PropertyPath("EmergencyUp");

            Binding bindingEmergencyDownSend = new Binding();
            bindingEmergencyDownSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingEmergencyDownSend.Path = new PropertyPath("EmergencyDown");

            Binding bindingIsEmergencySaveDBSend = new Binding();
            bindingIsEmergencySaveDBSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsEmergencySaveDBSend.Path = new PropertyPath("IsEmergencySaveDB");

            Binding bindingPeriodEmergencySaveDBSend = new Binding();
            bindingPeriodEmergencySaveDBSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingPeriodEmergencySaveDBSend.Path = new PropertyPath("PeriodEmergencySaveDB");

            Binding bindingIsEmergencyUpSend = new Binding();
            bindingIsEmergencyUpSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsEmergencyUpSend.Path = new PropertyPath("IsEmergencyUpDG");

            Binding bindingIsEmergencyDownSend = new Binding();
            bindingIsEmergencyDownSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingIsEmergencyDownSend.Path = new PropertyPath("IsEmergencyDownDG");

            Binding bindingEmergencyUpEnableSend = new Binding();
            bindingEmergencyUpEnableSend.Path = new PropertyPath("IsEmergencyUpDG");

            Binding bindingEmergencyDownEnableSend = new Binding();
            bindingEmergencyDownEnableSend.Path = new PropertyPath("IsEmergencyDownDG");

            Binding bindingIsUpSend = new Binding();
            bindingIsUpSend.Path = new PropertyPath("IsEmergencyUpDG");

            Binding bindingIsDownSend = new Binding();
            bindingIsDownSend.Path = new PropertyPath("IsEmergencyDownDG");

            MultiBinding bindingIsEmergencyDBSend = new MultiBinding();
            bindingIsEmergencyDBSend.Converter = new CheckBoxEmergencySaveDBValueConverter();
            bindingIsEmergencyDBSend.Bindings.Add(bindingIsUpSend);
            bindingIsEmergencyDBSend.Bindings.Add(bindingIsDownSend);

            Binding bindingEnableTBPeriodEmergencySaveDB1Send = new Binding();
            bindingEnableTBPeriodEmergencySaveDB1Send.Path = new PropertyPath("IsEmergencyUpDG");

            Binding bindingEnableTBPeriodEmergencySaveDB2Send = new Binding();
            bindingEnableTBPeriodEmergencySaveDB2Send.Path = new PropertyPath("IsEmergencyDownDG");

            Binding bindingEnableTBPeriodEmergencySaveDB3Send = new Binding();
            bindingEnableTBPeriodEmergencySaveDB3Send.Path = new PropertyPath("IsEmergencySaveDB");

            MultiBinding bindingEnableTBPeriodEmergencySaveDB4Send = new MultiBinding();
            bindingEnableTBPeriodEmergencySaveDB4Send.Converter = new DGTBEmergencyEnabledValueConverter();
            bindingEnableTBPeriodEmergencySaveDB4Send.Bindings.Add(bindingEnableTBPeriodEmergencySaveDB1Send);
            bindingEnableTBPeriodEmergencySaveDB4Send.Bindings.Add(bindingEnableTBPeriodEmergencySaveDB2Send);
            bindingEnableTBPeriodEmergencySaveDB4Send.Bindings.Add(bindingEnableTBPeriodEmergencySaveDB3Send);

            FrameworkElementFactory lTypeEditableSend = new FrameworkElementFactory(typeof(ComboBox));
            lTypeEditableSend.Name = "ComboBoxType";

            FrameworkElementFactory lTypeSend = new FrameworkElementFactory(typeof(Label));
            lTypeSend.AddHandler(ComboBox.LoadedEvent, new RoutedEventHandler(LoadedType));
            lTypeSend.SetBinding(Label.ContentProperty, bindingTypeSend);

            FrameworkElementFactory lValueSend = new FrameworkElementFactory(typeof(Label));
            lValueSend.SetBinding(Label.ContentProperty, bindingValueSend);

            FrameworkElementFactory lRange0Send = new FrameworkElementFactory(typeof(Label));
            lRange0Send.SetBinding(Label.ContentProperty, bindingRange0Send);

            FrameworkElementFactory lFormulaSend = new FrameworkElementFactory(typeof(Label));
            lFormulaSend.SetBinding(Label.ContentProperty, bindingFormulaSend);

            FrameworkElementFactory lTextSend = new FrameworkElementFactory(typeof(Label));
            lTextSend.SetBinding(Label.ContentProperty, bindingTextSend);

            FrameworkElementFactory lEmergencyUpSend = new FrameworkElementFactory(typeof(Label));
            lEmergencyUpSend.SetBinding(Label.ContentProperty, bindingEmergencyUpSend);

            FrameworkElementFactory lEmergencyDownSend = new FrameworkElementFactory(typeof(Label));
            lEmergencyDownSend.SetBinding(Label.ContentProperty, bindingEmergencyDownSend);

            FrameworkElementFactory lPeriodEmergencySaveDBSend = new FrameworkElementFactory(typeof(Label));
            lPeriodEmergencySaveDBSend.SetBinding(Label.ContentProperty, bindingPeriodEmergencySaveDBSend);

            FrameworkElementFactory tbRange0EditableSend = new FrameworkElementFactory(typeof(TextBox));
            tbRange0EditableSend.Name = "TextBoxRange0";
            tbRange0EditableSend.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            tbRange0EditableSend.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            tbRange0EditableSend.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            tbRange0EditableSend.SetBinding(TextBox.TextProperty, bindingRange0Send);

            FrameworkElementFactory lRange1Send = new FrameworkElementFactory(typeof(Label));
            lRange1Send.SetBinding(Label.ContentProperty, bindingRange1Send);

            FrameworkElementFactory lFormulaEditableSend = new FrameworkElementFactory(typeof(TextBox));
            lFormulaEditableSend.Name = "TextBoxFormula";
            lFormulaEditableSend.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            lFormulaEditableSend.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(Text_PreviewTextInput));
            lFormulaEditableSend.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lFormulaEditableSend.SetBinding(TextBox.TextProperty, bindingFormulaSend);

            FrameworkElementFactory lTextEditableSend = new FrameworkElementFactory(typeof(TextBox));
            lTextEditableSend.Name = "TextBoxText";
            lTextEditableSend.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(Text_PreviewTextInput));
            lTextEditableSend.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lTextEditableSend.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            lTextEditableSend.SetBinding(TextBox.TextProperty, bindingTextSend);

            FrameworkElementFactory tbRange1EditableSend = new FrameworkElementFactory(typeof(TextBox));
            tbRange1EditableSend.Name = "TextBoxRange1";
            tbRange1EditableSend.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            tbRange1EditableSend.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));           
            tbRange1EditableSend.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            tbRange1EditableSend.SetBinding(TextBox.TextProperty, bindingRange1Send);

            FrameworkElementFactory lEthernetSerDescriptionSend = new FrameworkElementFactory(typeof(Label));
            lEthernetSerDescriptionSend.SetBinding(Label.ContentProperty, bindingDescriptionSend);

            FrameworkElementFactory lIDSend = new FrameworkElementFactory(typeof(TextBox));
            lIDSend.SetValue(TextBox.IsReadOnlyProperty, true);
            lIDSend.SetBinding(TextBox.TextProperty, bindingIDSend);

            FrameworkElementFactory lDescriptionEditableSend = new FrameworkElementFactory(typeof(TextBox));
            lDescriptionEditableSend.Name = "TextBoxDescription";
            lDescriptionEditableSend.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(Text_PreviewTextInput));
            lDescriptionEditableSend.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown)); 
            lDescriptionEditableSend.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            lDescriptionEditableSend.SetBinding(TextBox.TextProperty, bindingDescriptionSend);

            FrameworkElementFactory lEthernetSerIsSaveDatabaseSend = new FrameworkElementFactory(typeof(CheckBox));
            lEthernetSerIsSaveDatabaseSend.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            lEthernetSerIsSaveDatabaseSend.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            lEthernetSerIsSaveDatabaseSend.SetBinding(CheckBox.IsCheckedProperty, bindingIsSaveDatabaseSend);

            FrameworkElementFactory lEthernetSerTableNameSend = new FrameworkElementFactory(typeof(Label));
            lEthernetSerTableNameSend.SetBinding(Label.ContentProperty, bindingTableNameSend);

            FrameworkElementFactory lTableNameEditableSend = new FrameworkElementFactory(typeof(TextBox));
            lTableNameEditableSend.Name = "TextBoxTableName";
            lTableNameEditableSend.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(Text_PreviewTextInput));
            lTableNameEditableSend.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lTableNameEditableSend.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));
            lTableNameEditableSend.SetBinding(TextBox.TextProperty, bindingTableNameSend);

            FrameworkElementFactory lEthernetSerPeriodSaveDBSend = new FrameworkElementFactory(typeof(Label));
            lEthernetSerPeriodSaveDBSend.SetBinding(Label.ContentProperty, bindingPeriodSaveDBSend);

            FrameworkElementFactory lPeriodSaveDBEditableSend = new FrameworkElementFactory(typeof(TextBox));
            lPeriodSaveDBEditableSend.Name = "TextBoxPeriodSaveDB";
            lPeriodSaveDBEditableSend.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            lPeriodSaveDBEditableSend.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lPeriodSaveDBEditableSend.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));         
            lPeriodSaveDBEditableSend.SetBinding(TextBox.TextProperty, bindingPeriodSaveDBSend);

            FrameworkElementFactory lEmergencyUpEditableSend = new FrameworkElementFactory(typeof(TextBox));
            lEmergencyUpEditableSend.Name = "TextBoxEmergencyUp";
            lEmergencyUpEditableSend.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            lEmergencyUpEditableSend.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lEmergencyUpEditableSend.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));           
            lEmergencyUpEditableSend.SetBinding(TextBox.TextProperty, bindingEmergencyUpSend);
            lEmergencyUpEditableSend.SetBinding(TextBox.IsEnabledProperty, bindingEmergencyUpEnableSend);

            FrameworkElementFactory lEmergencyDownEditableSend = new FrameworkElementFactory(typeof(TextBox));
            lEmergencyDownEditableSend.Name = "TextBoxEmergencyDown";
            lEmergencyDownEditableSend.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            lEmergencyDownEditableSend.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            lEmergencyDownEditableSend.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));            
            lEmergencyDownEditableSend.SetBinding(TextBox.TextProperty, bindingEmergencyDownSend);
            lEmergencyDownEditableSend.SetBinding(TextBox.IsEnabledProperty, bindingEmergencyDownEnableSend);

            FrameworkElementFactory fIsEmergencySaveDBSend = new FrameworkElementFactory(typeof(CheckBox));
            fIsEmergencySaveDBSend.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            fIsEmergencySaveDBSend.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            fIsEmergencySaveDBSend.SetBinding(CheckBox.IsCheckedProperty, bindingIsEmergencySaveDBSend);
            fIsEmergencySaveDBSend.SetBinding(CheckBox.IsEnabledProperty, bindingIsEmergencyDBSend);

            FrameworkElementFactory fPeriodEmergencySaveDBEditableSend = new FrameworkElementFactory(typeof(TextBox));
            fPeriodEmergencySaveDBEditableSend.Name = "TextBoxPeriodEmergencySaveDB";
            fPeriodEmergencySaveDBEditableSend.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(DigitalTextBox_PreviewTextInput));
            fPeriodEmergencySaveDBEditableSend.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(BackspacePreviewTextKeyDown));
            fPeriodEmergencySaveDBEditableSend.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxFocus));           
            fPeriodEmergencySaveDBEditableSend.SetBinding(TextBox.TextProperty, bindingPeriodEmergencySaveDBSend);
            fPeriodEmergencySaveDBEditableSend.SetBinding(TextBox.IsEnabledProperty, bindingEnableTBPeriodEmergencySaveDB4Send);

            FrameworkElementFactory fIsEmergencyUpSend = new FrameworkElementFactory(typeof(CheckBox));
            fIsEmergencyUpSend.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            fIsEmergencyUpSend.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            fIsEmergencyUpSend.SetBinding(CheckBox.IsCheckedProperty, bindingIsEmergencyUpSend);

            FrameworkElementFactory fIsEmergencyDownSend = new FrameworkElementFactory(typeof(CheckBox));
            fIsEmergencyDownSend.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            fIsEmergencyDownSend.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            fIsEmergencyDownSend.SetBinding(CheckBox.IsCheckedProperty, bindingIsEmergencyDownSend);

            DataTemplate dataTemplateTypeSend = new DataTemplate();
            dataTemplateTypeSend.VisualTree = lTypeSend;

            DataTemplate dataTemplateTypeEditableSend = new DataTemplate();
            dataTemplateTypeEditableSend.VisualTree = lTypeEditableSend;

            DataTemplate dataTemplateValueSend = new DataTemplate();
            dataTemplateValueSend.VisualTree = lValueSend;

            DataTemplate dataTemplateRange0Send = new DataTemplate();
            dataTemplateRange0Send.VisualTree = lRange0Send;

            DataTemplate dataTemplateRange0EditableSend = new DataTemplate();
            dataTemplateRange0EditableSend.VisualTree = tbRange0EditableSend;

            DataTemplate dataTemplateRange1Send = new DataTemplate();
            dataTemplateRange1Send.VisualTree = lRange1Send;

            DataTemplate dataTemplateRange1EditableSend = new DataTemplate();
            dataTemplateRange1EditableSend.VisualTree = tbRange1EditableSend;

            DataTemplate dataTemplateIDSend = new DataTemplate();
            dataTemplateIDSend.VisualTree = lIDSend;

            DataTemplate dataTemplateDescriptionSend = new DataTemplate();
            dataTemplateDescriptionSend.VisualTree = lEthernetSerDescriptionSend;

            DataTemplate dataTemplateDescriptionEditableSend = new DataTemplate();
            dataTemplateDescriptionEditableSend.VisualTree = lDescriptionEditableSend;

            DataTemplate dataTemplateIsSaveDatabaseSend = new DataTemplate();
            dataTemplateIsSaveDatabaseSend.VisualTree = lEthernetSerIsSaveDatabaseSend;

            DataTemplate dataTemplateTableNameSend = new DataTemplate();
            dataTemplateTableNameSend.VisualTree = lEthernetSerTableNameSend;

            DataTemplate dataTemplateTableNameEditableSend = new DataTemplate();
            dataTemplateTableNameEditableSend.VisualTree = lTableNameEditableSend;

            DataTemplate dataTemplatePeriodSaveDBSend = new DataTemplate();
            dataTemplatePeriodSaveDBSend.VisualTree = lEthernetSerPeriodSaveDBSend;

            DataTemplate dataTemplatePeriodSaveDBEditableSend = new DataTemplate();
            dataTemplatePeriodSaveDBEditableSend.VisualTree = lPeriodSaveDBEditableSend;

            DataTemplate dataTemplateFormulaSend = new DataTemplate();
            dataTemplateFormulaSend.VisualTree = lFormulaSend;

            DataTemplate dataTemplateFormulaEditableSend = new DataTemplate();
            dataTemplateFormulaEditableSend.VisualTree = lFormulaEditableSend;

            DataTemplate dataTemplateTextSend = new DataTemplate();
            dataTemplateTextSend.VisualTree = lTextSend;

            DataTemplate dataTemplateTextEditableSend = new DataTemplate();
            dataTemplateTextEditableSend.VisualTree = lTextEditableSend;

            DataTemplate dataTemplateEmergencyUpSend = new DataTemplate();
            dataTemplateEmergencyUpSend.VisualTree = lEmergencyUpSend;

            DataTemplate dataTemplateEmergencyUpEditableSend = new DataTemplate();
            dataTemplateEmergencyUpEditableSend.VisualTree = lEmergencyUpEditableSend;

            DataTemplate dataTemplateEmergencyDownSend = new DataTemplate();
            dataTemplateEmergencyDownSend.VisualTree = lEmergencyDownSend;

            DataTemplate dataTemplateEmergencyDownEditableSend = new DataTemplate();
            dataTemplateEmergencyDownEditableSend.VisualTree = lEmergencyDownEditableSend;

            DataTemplate dataTemplatePeriodEmergencySaveDBSend = new DataTemplate();
            dataTemplatePeriodEmergencySaveDBSend.VisualTree = lPeriodEmergencySaveDBSend;

            DataTemplate dataTemplatePeriodEmergencySaveDBEditableSend = new DataTemplate();
            dataTemplatePeriodEmergencySaveDBEditableSend.VisualTree = fPeriodEmergencySaveDBEditableSend;

            DataTemplate dataTemplateIsEmergencySaveDBEditableSend = new DataTemplate();
            dataTemplateIsEmergencySaveDBEditableSend.VisualTree = fIsEmergencySaveDBSend;

            DataTemplate dataTemplateIsEmergencyUpEditableSend = new DataTemplate();
            dataTemplateIsEmergencyUpEditableSend.VisualTree = fIsEmergencyUpSend;

            DataTemplate dataTemplateIsEmergencyDownEditableSend = new DataTemplate();
            dataTemplateIsEmergencyDownEditableSend.VisualTree = fIsEmergencyDownSend;           

            DataGridTemplateColumn typeSend = new DataGridTemplateColumn();
            typeSend.Header = "Тип";
            typeSend.CellTemplate = dataTemplateTypeSend;
            typeSend.CellEditingTemplate = dataTemplateTypeEditableSend;

            DataGridTemplateColumn valueSend = new DataGridTemplateColumn();
            valueSend.Header = "Значение";
            valueSend.CellTemplate = dataTemplateValueSend;
            valueSend.IsReadOnly = true;

            DataGridTemplateColumn range0Send = new DataGridTemplateColumn();
            range0Send.Header = StaticValue.SRange0;
            range0Send.CellTemplate = dataTemplateRange0Send;
            range0Send.CellEditingTemplate = dataTemplateRange0EditableSend;

            DataGridTemplateColumn range1Send = new DataGridTemplateColumn();
            range1Send.Header = StaticValue.SRange1;
            range1Send.CellTemplate = dataTemplateRange1Send;
            range1Send.CellEditingTemplate = dataTemplateRange1EditableSend;

            DataGridTemplateColumn idSend = new DataGridTemplateColumn();
            idSend.Header = "ID";
            idSend.CellTemplate = dataTemplateIDSend;

            DataGridTemplateColumn descriptionSend = new DataGridTemplateColumn();
            descriptionSend.Header = StaticValue.SDescription;
            descriptionSend.CellTemplate = dataTemplateDescriptionSend;
            descriptionSend.CellEditingTemplate = dataTemplateDescriptionEditableSend;

            DataGridTemplateColumn isSaveDatabaseSend = new DataGridTemplateColumn();
            isSaveDatabaseSend.Header = StaticValue.SIsSaveBD;
            isSaveDatabaseSend.CellTemplate = dataTemplateIsSaveDatabaseSend;

            DataGridTemplateColumn tableNameSend = new DataGridTemplateColumn();
            tableNameSend.Header = StaticValue.STableName;
            tableNameSend.CellTemplate = dataTemplateTableNameSend;
            tableNameSend.CellEditingTemplate = dataTemplateTableNameEditableSend;

            DataGridTemplateColumn periodSaveDBSend = new DataGridTemplateColumn();
            periodSaveDBSend.Header = StaticValue.SPeriodSaveDB;
            periodSaveDBSend.CellTemplate = dataTemplatePeriodSaveDBSend;
            periodSaveDBSend.CellEditingTemplate = dataTemplatePeriodSaveDBEditableSend;

            DataGridTemplateColumn formulaSend = new DataGridTemplateColumn();
            formulaSend.Header = StaticValue.SFormula;
            formulaSend.CellTemplate = dataTemplateFormulaSend;
            formulaSend.CellEditingTemplate = dataTemplateFormulaEditableSend;

            DataGridTemplateColumn textSend = new DataGridTemplateColumn();
            textSend.Header = StaticValue.SText;
            textSend.CellTemplate = dataTemplateTextSend;
            textSend.CellEditingTemplate = dataTemplateTextEditableSend;

            DataGridTemplateColumn emergencyUpSend = new DataGridTemplateColumn();
            emergencyUpSend.Header = StaticValue.SSetUp;
            emergencyUpSend.CellTemplate = dataTemplateEmergencyUpSend;
            emergencyUpSend.CellEditingTemplate = dataTemplateEmergencyUpEditableSend;

            DataGridTemplateColumn emergencyDownSend = new DataGridTemplateColumn();
            emergencyDownSend.Header = StaticValue.SSetDown;
            emergencyDownSend.CellTemplate = dataTemplateEmergencyDownSend;
            emergencyDownSend.CellEditingTemplate = dataTemplateEmergencyDownEditableSend;

            DataGridTemplateColumn periodEmergencySaveDBSend = new DataGridTemplateColumn();
            periodEmergencySaveDBSend.Header = StaticValue.SPeriodSaveSetDB;
            periodEmergencySaveDBSend.CellTemplate = dataTemplatePeriodEmergencySaveDBSend;
            periodEmergencySaveDBSend.CellEditingTemplate = dataTemplatePeriodEmergencySaveDBEditableSend;

            DataGridTemplateColumn isEmergencySaveDBSend = new DataGridTemplateColumn();
            isEmergencySaveDBSend.Header = StaticValue.SIsSaveSetDB;
            isEmergencySaveDBSend.CellTemplate = dataTemplateIsEmergencySaveDBEditableSend;

            DataGridTemplateColumn isEmergencyUpSend = new DataGridTemplateColumn();
            isEmergencyUpSend.Header = StaticValue.SIsSetUp;
            isEmergencyUpSend.CellTemplate = dataTemplateIsEmergencyUpEditableSend;

            DataGridTemplateColumn isEmergencyDownSend = new DataGridTemplateColumn();
            isEmergencyDownSend.Header = StaticValue.SIsSetDown;
            isEmergencyDownSend.CellTemplate = dataTemplateIsEmergencyDownEditableSend;

            DGSend.Columns.Add(typeSend);
            DGSend.Columns.Add(valueSend);
            DGSend.Columns.Add(range0Send);
            DGSend.Columns.Add(range1Send);
            DGSend.Columns.Add(formulaSend);
            DGSend.Columns.Add(textSend);
            DGSend.Columns.Add(descriptionSend);
            DGSend.Columns.Add(isSaveDatabaseSend);
            DGSend.Columns.Add(tableNameSend);
            DGSend.Columns.Add(periodSaveDBSend);
            DGSend.Columns.Add(isEmergencyUpSend);
            DGSend.Columns.Add(emergencyUpSend);
            DGSend.Columns.Add(isEmergencyDownSend);
            DGSend.Columns.Add(emergencyDownSend);
            DGSend.Columns.Add(isEmergencySaveDBSend);
            DGSend.Columns.Add(periodEmergencySaveDBSend);
            DGSend.Columns.Add(idSend);

            DGSend.CanUserAddRows = false;

            #endregion

            RowDefinition rowID = new RowDefinition();
            rowID.Height = GridLength.Auto;

            RowDefinition rowListEthernets = new RowDefinition();
            rowListEthernets.Height = GridLength.Auto;

            RowDefinition rowButtonEthernets = new RowDefinition();
            rowButtonEthernets.Height = new GridLength(30);

            RowDefinition rowDescription = new RowDefinition();
            rowDescription.Height = GridLength.Auto;

            RowDefinition rowIP = new RowDefinition();
            rowIP.Height = GridLength.Auto;

            RowDefinition rowIPClient = new RowDefinition();
            rowIPClient.Height = GridLength.Auto;

            RowDefinition rowTime = new RowDefinition();
            rowTime.Height = GridLength.Auto;

            RowDefinition rowNet = new RowDefinition();
            rowNet.Height = new GridLength(30);

            RowDefinition rowStatus = new RowDefinition();
            rowStatus.Height = GridLength.Auto;

            RowDefinition rowBufferSize = new RowDefinition();
            rowBufferSize.Height = GridLength.Auto;

            RowDefinition rowBufferSizeSend = new RowDefinition();
            rowBufferSizeSend.Height = GridLength.Auto;

            RowDefinition rowProtocols = new RowDefinition();
            rowProtocols.Height = GridLength.Auto;

            RowDefinition rowItemNets = new RowDefinition();
            rowItemNets.Height = GridLength.Auto;

            RowDefinition rowDGSend = new RowDefinition();
            rowDGSend.Height = GridLength.Auto;

            RowDefinition rowButtonSend = new RowDefinition();
            rowButtonSend.Height = new GridLength(30);

            Image addImage = new Image();
            addImage.Source = new BitmapImage(new Uri("Images/AddNet.png", UriKind.Relative));

            Image addImageSend = new Image();
            addImageSend.Source = new BitmapImage(new Uri("Images/AddNet.png", UriKind.Relative));

            Image startImage = new Image();
            startImage.Source = new BitmapImage(new Uri("Images/StartEthernet.png", UriKind.Relative));

            Image stopImage = new Image();
            stopImage.Source = new BitmapImage(new Uri("Images/StopEthernet.png", UriKind.Relative));
           
            Binding bindingStartIsEnable = new Binding();
            bindingStartIsEnable.Converter = new DGItemsToBoolConverter();
            bindingStartIsEnable.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingStartIsEnable.Source = DGRec;
            bindingStartIsEnable.Path = new PropertyPath("Items.Count");

            Binding bindingStartIsEnable2 = new Binding();
            bindingStartIsEnable2.Converter = new DGItemsToBoolConverter();
            bindingStartIsEnable2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingStartIsEnable2.Source = DGSend;
            bindingStartIsEnable2.Path = new PropertyPath("Items.Count");

            Binding bindingStartIsEnable3 = new Binding();
            bindingStartIsEnable3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingStartIsEnable3.Source = this;
            bindingStartIsEnable3.Path = new PropertyPath("IsBindingStart");

            MultiBinding bindingsStartIsEnable = new MultiBinding();
            bindingsStartIsEnable.Converter = new StartDGEnableValueConverter();
            bindingsStartIsEnable.Bindings.Add(bindingStartIsEnable);
            bindingsStartIsEnable.Bindings.Add(bindingStartIsEnable2);
            bindingsStartIsEnable.Bindings.Add(bindingStartIsEnable3);

            Binding bindingRemoveButtonIsEnable = new Binding();
            bindingRemoveButtonIsEnable.Converter = new RemoveButtonConverter();
            bindingRemoveButtonIsEnable.Source = DGRec;
            bindingRemoveButtonIsEnable.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingRemoveButtonIsEnable.Path = new PropertyPath("SelectedItem");

            Binding bindingRemoveButton2IsEnable = new Binding();
            bindingRemoveButton2IsEnable.Converter = new RemoveButtonConverter();
            bindingRemoveButton2IsEnable.Source = DGSend;
            bindingRemoveButton2IsEnable.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingRemoveButton2IsEnable.Path = new PropertyPath("SelectedItem");

            Binding bindingRemoveButtonIsEnable2 = new Binding();
            bindingRemoveButtonIsEnable2.Source = this;
            bindingRemoveButtonIsEnable2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingRemoveButtonIsEnable2.Path = new PropertyPath("IsBindingStart");
          
            MultiBinding RemoveButtonIsEnable = new MultiBinding();
            RemoveButtonIsEnable.Converter = new ButtonStartInterfaceValueConverter();
            RemoveButtonIsEnable.Bindings.Add(bindingRemoveButtonIsEnable);
            RemoveButtonIsEnable.Bindings.Add(bindingRemoveButtonIsEnable2);

            MultiBinding RemoveButton2IsEnable = new MultiBinding();
            RemoveButton2IsEnable.Converter = new ButtonStartInterfaceValueConverter();
            RemoveButton2IsEnable.Bindings.Add(bindingRemoveButton2IsEnable);
            RemoveButton2IsEnable.Bindings.Add(bindingRemoveButtonIsEnable2);

            Start = new Button();
            Start.SetBinding(Button.IsEnabledProperty, bindingsStartIsEnable);
            Start.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            Start.ToolTip = "Начать опрос сетевых параметров";
            Start.Margin = new Thickness(3);
            Start.Content = startImage;
            Start.Click += Start_Click;
            
            Stop = new Button();
            Stop.IsEnabled = false;
            Stop.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            Stop.ToolTip = "Остановить опрос сетевых параметров";
            Stop.Margin = new Thickness(3);
            Stop.Content = stopImage;
            Stop.Click += Stop_Click; 

            AddNetButton = new Button();
            AddNetButton.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            AddNetButton.ToolTip = "Добавить параметр";
            AddNetButton.Margin = new Thickness(3);
            AddNetButton.Content = addImage;
            AddNetButton.PreviewMouseDown += AddNetButton_PreviewMouseDown;

            Image removeImage = new Image();
            removeImage.Source = new BitmapImage(new Uri("Images/DeleteNet.png", UriKind.Relative));

            AddNetButtonSend = new Button();
            AddNetButtonSend.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            AddNetButtonSend.ToolTip = "Добавить параметр";
            AddNetButtonSend.Margin = new Thickness(3);
            AddNetButtonSend.Content = addImageSend;
            AddNetButtonSend.PreviewMouseDown += AddNetButtonSend_PreviewMouseDown;

            Image removeImageSend = new Image();
            removeImageSend.Source = new BitmapImage(new Uri("Images/DeleteNet.png", UriKind.Relative));
           
            RemoveNetButton = new Button();
            RemoveNetButton.IsEnabled = false;
            RemoveNetButton.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            RemoveNetButton.ToolTip = "Удалить параметр";
            RemoveNetButton.Margin = new Thickness(3);
            RemoveNetButton.Content = removeImage;
            RemoveNetButton.SetBinding(Button.IsEnabledProperty, RemoveButtonIsEnable);
            RemoveNetButton.Click += RemoveNetButton_Click;

            RemoveNetButtonSend = new Button();
            RemoveNetButtonSend.IsEnabled = false;
            RemoveNetButtonSend.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            RemoveNetButtonSend.ToolTip = "Удалить параметр";
            RemoveNetButtonSend.Margin = new Thickness(3);
            RemoveNetButtonSend.Content = removeImageSend;
            RemoveNetButtonSend.SetBinding(Button.IsEnabledProperty, RemoveButton2IsEnable);
            RemoveNetButtonSend.Click += RemoveNetButtonSend_Click;

            StackPanel panelID= new StackPanel();
            panelID.Orientation = Orientation.Horizontal;
            panelID.SetValue(Grid.RowProperty, 2);

            StackPanel panelDescription = new StackPanel();
            panelDescription.Orientation = Orientation.Horizontal;
            panelDescription.SetValue(Grid.RowProperty, 3);

            StackPanel panelIP = new StackPanel();
            panelIP.Orientation = Orientation.Horizontal;
            panelIP.SetValue(Grid.RowProperty, 4);

            StackPanel panelIPClient = new StackPanel();
            panelIPClient.Orientation = Orientation.Horizontal;
            panelIPClient.SetValue(Grid.RowProperty, 5);

            StackPanel panelProtocol = new StackPanel();
            panelProtocol.Orientation = Orientation.Horizontal;
            panelProtocol.SetValue(Grid.RowProperty, 9);

            StackPanel panelNetButton = new StackPanel();
            panelNetButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            panelNetButton.Orientation = Orientation.Horizontal;
            panelNetButton.SetValue(Grid.RowProperty, 10);

            StackPanel panelStatus = new StackPanel();
            panelStatus.Margin = new Thickness(0, 0, 0, 5);
            panelStatus.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            panelStatus.Orientation = Orientation.Horizontal;
            panelStatus.SetValue(Grid.RowProperty, 11);

            StackPanel panelBuffer = new StackPanel();
            panelBuffer.Orientation = Orientation.Horizontal;
            panelBuffer.SetValue(Grid.RowProperty, 7);

            StackPanel panelTime = new StackPanel();
            panelTime.Orientation = Orientation.Horizontal;
            panelTime.SetValue(Grid.RowProperty, 6);

            StackPanel panelBufferSend = new StackPanel();
            panelBufferSend.Orientation = Orientation.Horizontal;
            panelBufferSend.SetValue(Grid.RowProperty, 8);

            StackPanel panelButtonSend = new StackPanel();
            panelButtonSend.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            panelButtonSend.Orientation = Orientation.Horizontal;
            panelButtonSend.SetValue(Grid.RowProperty, 13);

            TBStatus = new TextBox();
            TBStatus.TextWrapping = TextWrapping.Wrap;
            TBStatus.MaxWidth = 700;
            TBStatus.MaxLines = 5;
            TBStatus.IsReadOnly = true;
            TBStatus.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            TBStatus.Text = "Статус: Опрос остановлен";

            Label labelIDEthernet = new Label();
            labelIDEthernet.Content = "ID: ";

            Label labelDescriptionEthernet = new Label();
            labelDescriptionEthernet.Content = "Описание Ethernet: ";

            Label ipLabelServer = new Label();
            ipLabelServer.Content = "IP-адрес сервера:";

            Label ipLabelClient = new Label();
            ipLabelClient.Content = "IP-адрес клиента:";

            Label lPortServer = new Label();
            lPortServer.Content = "Порт сервера:";

            Label lPortClient = new Label();
            lPortClient.Content = "Порт клиента:";

            Label protocolLabel = new Label();
            protocolLabel.Content = "Протокол:";

            Label bufferLabel = new Label();
            bufferLabel.Content = "Размер приемного буфера (байт):";

            Label timeLabel = new Label();
            timeLabel.Content = "Время опроса (с):";

            Label bufferLabelSend = new Label();
            bufferLabelSend.Content = "Размер отправочного буфера (байт):";

            Label collectionNetLabel = new Label();
            collectionNetLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            collectionNetLabel.Content = "Список сетевых параметров(Прием данных)";

            Label collectionNetSendLabel = new Label();
            collectionNetSendLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            collectionNetSendLabel.Content = "Список сетевых параметров(Отправка данных)";

            panelID.Children.Add(labelIDEthernet);
            panelID.Children.Add(TBIDEthernet);

            panelDescription.Children.Add(labelDescriptionEthernet);
            panelDescription.Children.Add(TBDescriptionEthernet);

            panelIP.Children.Add(ipLabelServer);
            panelIP.Children.Add(TBIPAdress1);
            panelIP.Children.Add(TBIPAdress2);
            panelIP.Children.Add(TBIPAdress3);
            panelIP.Children.Add(TBIPAdress4);
            panelIP.Children.Add(lPortServer);
            panelIP.Children.Add(TBPortServer);

            panelIPClient.Children.Add(ipLabelClient);
            panelIPClient.Children.Add(CBLocalIPs);
            panelIPClient.Children.Add(lPortClient);
            panelIPClient.Children.Add(TBPortClient);

            panelProtocol.Children.Add(protocolLabel);
            panelProtocol.Children.Add(CBProtocol);

            panelBuffer.Children.Add(bufferLabel);
            panelBuffer.Children.Add(TBBufferSizeRec);

            panelTime.Children.Add(timeLabel);
            panelTime.Children.Add(TBTime);

            panelBufferSend.Children.Add(bufferLabelSend);
            panelBufferSend.Children.Add(TBBufferSizeSend);

            panelNetButton.Children.Add(Start);
            panelNetButton.Children.Add(Stop);
            panelNetButton.Children.Add(collectionNetLabel);
            panelNetButton.Children.Add(AddNetButton);
            panelNetButton.Children.Add(RemoveNetButton);

            panelButtonSend.Children.Add(collectionNetSendLabel);
            panelButtonSend.Children.Add(AddNetButtonSend);
            panelButtonSend.Children.Add(RemoveNetButtonSend);

            panelStatus.Children.Add(TBStatus);

            GridMain.RowDefinitions.Add(rowListEthernets);
            GridMain.RowDefinitions.Add(rowButtonEthernets);
            GridMain.RowDefinitions.Add(rowID);
            GridMain.RowDefinitions.Add(rowDescription);
            GridMain.RowDefinitions.Add(rowIP);
            GridMain.RowDefinitions.Add(rowIPClient);
            GridMain.RowDefinitions.Add(rowTime);
            GridMain.RowDefinitions.Add(rowBufferSize);
            GridMain.RowDefinitions.Add(rowBufferSizeSend);
            GridMain.RowDefinitions.Add(rowProtocols);
            GridMain.RowDefinitions.Add(rowNet);
            GridMain.RowDefinitions.Add(rowStatus);
            GridMain.RowDefinitions.Add(rowItemNets);
            GridMain.RowDefinitions.Add(rowButtonSend);
            GridMain.RowDefinitions.Add(rowDGSend);

            MenuItem menuItemPasteBufferSize = new MenuItem();
            menuItemPasteBufferSize.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyBufferSize = new MenuItem();
            menuItemCopyBufferSize.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuBufferSize = new System.Windows.Controls.ContextMenu();
            ContextMenuBufferSize.Items.Add(menuItemPasteBufferSize);
            ContextMenuBufferSize.Items.Add(menuItemCopyBufferSize);

            MenuItem menuItemPasteBufferSizeSend = new MenuItem();
            menuItemPasteBufferSizeSend.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyBufferSizeSend = new MenuItem();
            menuItemCopyBufferSizeSend.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuBufferSizeSend = new System.Windows.Controls.ContextMenu();
            ContextMenuBufferSizeSend.Items.Add(menuItemPasteBufferSizeSend);
            ContextMenuBufferSizeSend.Items.Add(menuItemCopyBufferSizeSend);

            MenuItem menuItemPasteDescription = new MenuItem();
            menuItemPasteDescription.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyDescription = new MenuItem();
            menuItemCopyDescription.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuDescription = new System.Windows.Controls.ContextMenu();
            ContextMenuDescription.Items.Add(menuItemPasteDescription);
            ContextMenuDescription.Items.Add(menuItemCopyDescription);

            MenuItem miPortPaste = new MenuItem();
            miPortPaste.Command = ApplicationCommands.Paste;

            MenuItem miPortCopy = new MenuItem();
            miPortCopy.Command = ApplicationCommands.Copy;

            ContextMenu cmPort = new System.Windows.Controls.ContextMenu();
            cmPort.Items.Add(miPortPaste);
            cmPort.Items.Add(miPortCopy);

            MenuItem menuItemPasteIP = new MenuItem();
            menuItemPasteIP.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyIP = new MenuItem();
            menuItemCopyIP.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuIP = new System.Windows.Controls.ContextMenu();
            ContextMenuIP.Items.Add(menuItemPasteIP);
            ContextMenuIP.Items.Add(menuItemCopyIP);

            MenuItem menuItemPasteTime = new MenuItem();
            menuItemPasteTime.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyTime = new MenuItem();
            menuItemCopyTime.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuTime = new System.Windows.Controls.ContextMenu();
            ContextMenuTime.Items.Add(menuItemPasteTime);
            ContextMenuTime.Items.Add(menuItemCopyTime);

            MenuItem menuItemPasteTimeSend = new MenuItem();
            menuItemPasteTimeSend.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyTimeSend = new MenuItem();
            menuItemCopyTimeSend.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuTimeSend = new System.Windows.Controls.ContextMenu();
            ContextMenuTimeSend.Items.Add(menuItemPasteTimeSend);
            ContextMenuTimeSend.Items.Add(menuItemCopyTimeSend);
           
            TBDescriptionEthernet.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBDescriptionEthernet.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextTextBoxPaste));
            TBDescriptionEthernet.ContextMenu = ContextMenuDescription;

            TBDescriptionEthernet.PreviewTextInput += Text_PreviewTextInput;
            TBDescriptionEthernet.PreviewKeyDown += BackspacePreviewTextKeyDown;
            TBDescriptionEthernet.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBDescriptionEthernet.GotFocus += TextBoxFocus;
                              
            TBPortServer.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBPortServer.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
            TBPortServer.ContextMenu = cmPort;

            TBPortServer.PreviewTextInput += DigitalTextBox_PreviewTextInput;
            TBPortServer.PreviewKeyDown += BackspacePreviewTextKeyDown;
            TBPortServer.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBPortServer.GotFocus += TextBoxFocus;

            TBPortClient.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBPortClient.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
            TBPortClient.ContextMenu = cmPort;

            TBPortClient.PreviewTextInput += DigitalTextBox_PreviewTextInput;
            TBPortClient.PreviewKeyDown += BackspacePreviewTextKeyDown;
            TBPortClient.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBPortClient.GotFocus += TextBoxFocus;

            TBBufferSizeRec.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBBufferSizeRec.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
            TBBufferSizeRec.ContextMenu = ContextMenuBufferSize;

            TBBufferSizeRec.PreviewTextInput += DigitalTextBox_PreviewTextInput;
            TBBufferSizeRec.PreviewKeyDown += BackspacePreviewTextKeyDown;
            TBBufferSizeRec.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBBufferSizeRec.GotFocus += TextBoxFocus;

            TBBufferSizeSend.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBBufferSizeSend.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
            TBBufferSizeSend.ContextMenu = ContextMenuBufferSizeSend;

            TBBufferSizeSend.PreviewTextInput += DigitalTextBox_PreviewTextInput;
            TBBufferSizeSend.PreviewKeyDown += BackspacePreviewTextKeyDown;
            TBBufferSizeSend.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBBufferSizeSend.GotFocus += TextBoxFocus;

            #region IPAddrssServerBinding
            TBIPAdress1.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBIPAdress1.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
            TBIPAdress1.ContextMenu = ContextMenuIP;

            TBIPAdress1.PreviewTextInput += DigitalTextBox_PreviewTextInput;
            TBIPAdress1.PreviewKeyDown += BackspacePreviewTextKeyDown;
            TBIPAdress1.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBIPAdress1.GotFocus += TextBoxFocus;

            TBIPAdress2.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBIPAdress2.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
            TBIPAdress2.ContextMenu = ContextMenuIP;

            TBIPAdress2.PreviewTextInput += DigitalTextBox_PreviewTextInput;
            TBIPAdress2.PreviewKeyDown += BackspacePreviewTextKeyDown;
            TBIPAdress2.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBIPAdress2.GotFocus += TextBoxFocus;

            TBIPAdress3.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBIPAdress3.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
            TBIPAdress3.ContextMenu = ContextMenuIP;

            TBIPAdress3.PreviewTextInput += DigitalTextBox_PreviewTextInput;
            TBIPAdress3.PreviewKeyDown += BackspacePreviewTextKeyDown;
            TBIPAdress3.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBIPAdress3.GotFocus += TextBoxFocus;

            TBIPAdress4.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBIPAdress4.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
            TBIPAdress4.ContextMenu = ContextMenuIP;

            TBIPAdress4.PreviewTextInput += DigitalTextBox_PreviewTextInput;
            TBIPAdress4.PreviewKeyDown += BackspacePreviewTextKeyDown;
            TBIPAdress4.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBIPAdress4.GotFocus += TextBoxFocus;
            #endregion
            
            TBTime.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBTime.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
            TBTime.ContextMenu = ContextMenuTime;

            TBTime.PreviewTextInput += DigitalTextBox_PreviewTextInput;
            TBTime.PreviewKeyDown += BackspacePreviewTextKeyDown;
            TBTime.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBTime.GotFocus += TextBoxFocus;          

            TBBufferSizeRec.Margin = new Thickness(3);
            TBTime.Margin = new Thickness(3);
            TBBufferSizeSend.Margin = new Thickness(3);

            Binding bindingBufferSizeRec = new Binding();
            bindingBufferSizeRec.Source = NewEthernetSer;
            bindingBufferSizeRec.Path = new PropertyPath("BufferSizeRec");

            Binding bindingBufferSizeSend = new Binding();
            bindingBufferSizeSend.Source = NewEthernetSer;
            bindingBufferSizeSend.Path = new PropertyPath("BufferSizeSend");

            Binding bindingDescriptionEthernet = new Binding();
            bindingDescriptionEthernet.Source = NewEthernetSer;
            bindingDescriptionEthernet.Path = new PropertyPath("Description");

            Binding bindingTime = new Binding();
            bindingTime.Source = NewEthernetSer;
            bindingTime.Path = new PropertyPath("Time");

            Binding bindingPortServer = new Binding();
            bindingPortServer.Source = NewEthernetSer;
            bindingPortServer.Path = new PropertyPath("PortServer");

            Binding bindingPortClient = new Binding();
            bindingPortClient.Source = NewEthernetSer;
            bindingPortClient.Path = new PropertyPath("PortClient");

            Binding bindingTBIPAdress1 = new Binding();
            bindingTBIPAdress1.Source = NewEthernetSer;
            bindingTBIPAdress1.Path = new PropertyPath("IPAddressServer[0]");

            Binding bindingTBIPAdress2 = new Binding();
            bindingTBIPAdress2.Source = NewEthernetSer;
            bindingTBIPAdress2.Path = new PropertyPath("IPAddressServer[1]");

            Binding bindingTBIPAdress3 = new Binding();
            bindingTBIPAdress3.Source = NewEthernetSer;
            bindingTBIPAdress3.Path = new PropertyPath("IPAddressServer[2]");

            Binding bindingTBIPAdress4 = new Binding();
            bindingTBIPAdress4.Source = NewEthernetSer;
            bindingTBIPAdress4.Path = new PropertyPath("IPAddressServer[3]");

            TBBufferSizeRec.SetBinding(TextBox.TextProperty, bindingBufferSizeRec);
            TBBufferSizeSend.SetBinding(TextBox.TextProperty, bindingBufferSizeSend);
            TBDescriptionEthernet.SetBinding(TextBox.TextProperty, bindingDescriptionEthernet);
            TBTime.SetBinding(TextBox.TextProperty, bindingTime);
            TBPortServer.SetBinding(TextBox.TextProperty, bindingPortServer);
            TBPortClient.SetBinding(TextBox.TextProperty, bindingPortClient);
            TBIPAdress1.SetBinding(TextBox.TextProperty, bindingTBIPAdress1);
            TBIPAdress2.SetBinding(TextBox.TextProperty, bindingTBIPAdress2);
            TBIPAdress3.SetBinding(TextBox.TextProperty, bindingTBIPAdress3);
            TBIPAdress4.SetBinding(TextBox.TextProperty, bindingTBIPAdress4); 
            
            TBIDEthernet.Text = ethernetControl.EthernetSer.ID;

            CBProtocol.SelectedItem = ethernetControl.EthernetSer.EthernetProtocol;
            CBProtocol.SelectionChanged += CBProtocol_SelectionChanged;

            SVMain.Content = GridMain;

            GridMain.Children.Add(panelID);
            GridMain.Children.Add(panelDescription);
            GridMain.Children.Add(panelIP);
            GridMain.Children.Add(panelIPClient);
            GridMain.Children.Add(panelTime);
            GridMain.Children.Add(panelBuffer);
            GridMain.Children.Add(panelBufferSend);
            GridMain.Children.Add(panelProtocol);
            GridMain.Children.Add(panelStatus);
            GridMain.Children.Add(DGRec);
            GridMain.Children.Add(panelNetButton);
            GridMain.Children.Add(panelButtonSend);
            GridMain.Children.Add(DGSend);
            GridMain.Children.Add(ListEthernets);
            GridMain.Children.Add(PanelButtonEthernets);
            this.Children.Add(SVMain);  
        }
        
        void CBProtocol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NewEthernetSer.EthernetProtocol = (string)e.AddedItems[0];

            e.Handled = true;
        }

        void RemoveEthernet_Click(object sender, RoutedEventArgs e)
        {
            NewEthernetSer.CollectionEthernetOperational.Remove((EthernetOperational)((ListBoxItem)ListEthernets.SelectedItem).Tag);

            ListEthernets.Items.Remove(ListEthernets.SelectedItem);

            ListEthernets.SelectedItem = ListEthernets.Items[0];
           
            e.Handled = true;
        }

        void ListEthernets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count >= 1)
            {
                ListBoxItem lbItem = (ListBoxItem)e.AddedItems[0];

                if (lbItem.Tag == "1")
                {
                    IsEthernetOperational = false;

                    BindingOperations.ClearBinding(DGRec, DataGrid.ItemsSourceProperty);

                    BindingOperations.ClearBinding(DGSend, DataGrid.ItemsSourceProperty);

                    Binding bindingCollectionNet = new Binding();
                    bindingCollectionNet.NotifyOnSourceUpdated = true;
                    bindingCollectionNet.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bindingCollectionNet.Source = NewEthernetSer.CollectionItemNetRec;

                    DGRec.SetBinding(DataGrid.ItemsSourceProperty, bindingCollectionNet);

                    Binding bindingCollectionSend = new Binding();
                    bindingCollectionSend.NotifyOnSourceUpdated = true;
                    bindingCollectionSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bindingCollectionSend.Source = NewEthernetSer.CollectionItemNetSend;

                    DGSend.SetBinding(DataGrid.ItemsSourceProperty, bindingCollectionSend);

                    TBDescriptionEthernet.Text = NewEthernetSer.Description;
                    TBBufferSizeRec.Text = NewEthernetSer.BufferSizeRec.ToString();
                    TBBufferSizeSend.Text = NewEthernetSer.BufferSizeSend.ToString();
                    TBPortServer.Text = NewEthernetSer.PortServer.ToString();
                }
                else
                {
                    IsEthernetOperational = true;

                    EthernetOperational ethernetOperational = (EthernetOperational)lbItem.Tag;

                    BindingOperations.ClearBinding(DGRec, DataGrid.ItemsSourceProperty);

                    BindingOperations.ClearBinding(DGSend, DataGrid.ItemsSourceProperty);

                    Binding bindingCollectionNet = new Binding();
                    bindingCollectionNet.NotifyOnSourceUpdated = true;
                    bindingCollectionNet.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bindingCollectionNet.Source = ethernetOperational.CollectionItemNetRec;

                    DGRec.SetBinding(DataGrid.ItemsSourceProperty, bindingCollectionNet);

                    Binding bindingCollectionSend = new Binding();
                    bindingCollectionSend.NotifyOnSourceUpdated = true;
                    bindingCollectionSend.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bindingCollectionSend.Source = ethernetOperational.CollectionItemNetSend;

                    DGSend.SetBinding(DataGrid.ItemsSourceProperty, bindingCollectionSend);

                    TBDescriptionEthernet.Text = ethernetOperational.Description;
                    TBBufferSizeRec.Text = ethernetOperational.BufferSizeRec.ToString();
                    TBBufferSizeSend.Text = ethernetOperational.BufferSizeSend.ToString();
                    TBPortServer.Text = ethernetOperational.Port.ToString();
                }
            }
          
            e.Handled = true;
        }

        void AddEthernet_Click(object sender, RoutedEventArgs e)
        {           
            EthernetOperational ethernetOperational = new EthernetOperational();

            ListBoxItem lbItem = new ListBoxItem();
            lbItem.Tag = ethernetOperational;
            lbItem.Content = ethernetOperational.Description;

            ListEthernets.Items.Add(lbItem);
            ListEthernets.SelectedItem = lbItem;

            NewEthernetSer.CollectionEthernetOperational.Add(ethernetOperational);

            e.Handled = true;
        }       

        private void DigitalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PopupMessage.IsOpen = false;

            TextBox tb = (TextBox)sender;

            string s;

            if (tb == TBPeriodTimeSaveDB || tb == TBEmergencySaveBD || tb == TBTime)
            {
                string pattern = @"^\d{1,5}$";
                
                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, e.Text);
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, e.Text);
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    if (tb == TBTime)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = StaticValue.SRange1_86400;
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;

                        e.Handled = true;
                    }
                    else if (tb == TBPeriodTimeSaveDB)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e, Item);
                    }
                    else if (tb == TBEmergencySaveBD)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e, Item);
                    }

                    return;
                }               

                if (double.TryParse(s, out d))
                {
                    if (d < 1 || d > 86400)
                    {
                        if (tb == TBTime)
                        {
                            PopupMessage.HorizontalOffset = 0;
                            LPopupMessage.Content = StaticValue.SRange1_86400;
                            PopupMessage.PlacementTarget = tb;
                            PopupMessage.IsOpen = true;

                            e.Handled = true;
                        }
                        else if (tb == TBPeriodTimeSaveDB)
                        {
                            PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e, Item);
                        }
                        else if (tb == TBEmergencySaveBD)
                        {
                            PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e, Item);
                        }
                    }
                    else
                    {
                        PopupMessage.IsOpen = false;
                    }
                }
                else
                {
                    if (tb == TBTime)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = StaticValue.SRange1_86400;
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;

                        e.Handled = true;
                    }
                    else if (tb == TBPeriodTimeSaveDB)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e, Item);
                    }
                    else if (tb == TBEmergencySaveBD)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e, Item);
                    } 
                }
            }
            else if (tb == TBIPAdress1 || tb == TBIPAdress2 || tb == TBIPAdress3 || tb == TBIPAdress4)
            {
                string pattern = @"^\d{1,3}$";

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, e.Text);
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, e.Text);
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = "Диапазон 0 - 255";
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }

                if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > 255)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = "Диапазон 0 - 255";
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;

                        e.Handled = true;
                    }
                }
            }
            else if(tb == TBPortClient || tb == TBPortServer)
            {
                string pattern = @"^\d{1,5}$";

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, e.Text);
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, e.Text);
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SRangePort;
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > 65535)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = "Диапазон буфера 0 - 65535";
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;

                        e.Handled = true;
                    }                    
                }
                else
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = "Диапазон буфера 0 - 65535";
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }
            }
            else if (tb == TBBufferSizeRec || tb == TBBufferSizeSend)
            {
                string pattern = @"^\d{1,9}$";

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, e.Text);
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, e.Text);
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SRange1_102400000;
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 1 || d > 102400000)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = StaticValue.SRange1_102400000;
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;

                        e.Handled = true;
                    }                    
                }
                else
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SRange1_102400000;
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }
            }
            else if (tb == TBRange0 || tb == TBRange1)
            {
                string pattern = @"^\d{1," + MaxDigit + "}$";

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, e.Text);
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, e.Text);
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    if (tb == TBRange0)
                    {
                        PopupMessageShow(StaticValue.SRange0, tb, "Диапазон буфера 0 -  " + BufferSize, e, Item);
                    }
                    else if (tb == TBRange1)
                    {
                        PopupMessageShow(StaticValue.SRange1, tb, "Диапазон буфера 0 -  " + BufferSize, e, Item);
                    } 
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > BufferSize)
                    {
                        if (tb == TBRange0)
                        {
                            PopupMessageShow(StaticValue.SRange0, tb, "Диапазон буфера 0 -  " + BufferSize, e, Item);
                        }
                        else if (tb == TBRange1)
                        {
                            PopupMessageShow(StaticValue.SRange1, tb, "Диапазон буфера 0 -  " + BufferSize, e, Item);
                        } 
                    }
                }
                else
                {
                    if (tb == TBRange0)
                    {
                        PopupMessageShow(StaticValue.SRange0, tb, "Диапазон буфера 0 -  " + BufferSize, e, Item);
                    }
                    else if (tb == TBRange1)
                    {
                        PopupMessageShow(StaticValue.SRange1, tb, "Диапазон буфера 0 -  " + BufferSize, e, Item);
                    }
                }
            }
            else if (tb == TBEmergencyUp || tb == TBEmergencyDown)
            {
                if (Item.TypeValue == "float")
                {
                    string pattern = @"^\d{1,8}(?:\.\d{0,6})?$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Неверный формат", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Неверный формат", e, Item);
                        }
                    }
                    else
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            float up = 0;
                            float down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                float.TryParse(s, out up);
                                down = Convert.ToSingle(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                float.TryParse(s, out down);
                                up = Convert.ToSingle(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }                                
                            }
                        }
                    }
                }
                else if (Item.TypeValue == "double")
                {
                    string pattern = @"^\d{1,15}(?:\.\d{0,12})?$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Неверный формат", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Неверный формат", e, Item);
                        }
                    }
                    else
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            double up = 0;
                            double down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                double.TryParse(s, out up);
                                down = Convert.ToDouble(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                double.TryParse(s, out down);
                                up = Convert.ToDouble(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                } 
                            }
                        }
                    }
                }
                else if (Item.TypeValue == "decimal")
                {
                    string pattern = @"^\d{1,29}(?:\.\d{0,27})?$";
                    decimal d;

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e, Item);
                        }
                    }
                    else if (decimal.TryParse(tb.Text, out d))
                    {
                        decimal up = 0;
                        decimal down = 0;

                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            if (tb == TBEmergencyUp)
                            {
                                decimal.TryParse(s, out up);
                                down = Convert.ToDecimal(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                decimal.TryParse(s, out down);
                                up = Convert.ToDecimal(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }
                            }
                        }                       
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e, Item);
                        }
                    }
                }
                else if (Item.TypeValue == "short")
                {
                    string pattern = @"^\d{1,5}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    short d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange_32768_32767, e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange_32768_32767, e, Item);
                        }                       
                    }
                    else if (short.TryParse(s, out d))
                    {
                        short up = 0;
                        short down = 0;

                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            if (tb == TBEmergencyUp)
                            {
                                short.TryParse(s, out up);
                                down = Convert.ToInt16(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                short.TryParse(s, out down);
                                up= Convert.ToInt16(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange_32768_32767, e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange_32768_32767, e, Item);
                        }  
                    }
                }
                else if (Item.TypeValue == "ushort")
                {
                    string pattern = @"^\d{1,5}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    ushort d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange0_65535, e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange0_65535, e, Item);
                        }
                    }
                    else if (ushort.TryParse(s, out d))
                    {
                        ushort up = 0;
                        ushort down = 0;

                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            if (tb == TBEmergencyUp)
                            {
                                ushort.TryParse(s, out up);
                                down = Convert.ToUInt16(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                ushort.TryParse(s, out down);
                                up = Convert.ToUInt16(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange0_65535, e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange0_65535, e, Item);
                        }
                    }
                }
                else if (Item.TypeValue == "int")
                {
                    string pattern = @"^\d{1,10}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    int d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange_2147483648_2147483647, e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange_2147483648_2147483647, e, Item);
                        }                       
                    }
                    else if (int.TryParse(s, out d))
                    {
                        int up = 0;
                        int down = 0;

                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            if (tb == TBEmergencyUp)
                            {
                                int.TryParse(s, out up);
                                down = Convert.ToInt32(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                int.TryParse(s, out down);
                                up = Convert.ToInt32(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange_2147483648_2147483647, e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange_2147483648_2147483647, e, Item);
                        }
                    }
                }
                else if (Item.TypeValue == "uint")
                {
                    string pattern = @"^\d{1,10}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    uint d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-4294967295", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-4294967295", e, Item);
                        }                       
                    }
                    else if (uint.TryParse(s, out d))
                    {
                        uint up = 0;
                        uint down = 0;

                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            if (tb == TBEmergencyUp)
                            {
                                uint.TryParse(s, out up);
                                down = Convert.ToUInt32(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                uint.TryParse(s, out down);
                                up = Convert.ToUInt32(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-4294967295", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-4294967295", e, Item);
                        } 
                    }
                }
                else if (Item.TypeValue == "long")
                {
                    string pattern = @"^\d{1,19}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    long d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -9223372036854775808-9223372036854775807", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -9223372036854775808-9223372036854775807", e, Item);
                        } 
                    }
                    else if (long.TryParse(s, out d))
                    {
                        long up = 0;
                        long down = 0;

                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            if (tb == TBEmergencyUp)
                            {
                                long.TryParse(s, out up);
                                down = Convert.ToInt64(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                long.TryParse(s, out down);
                                up = Convert.ToInt64(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -9223372036854775808-9223372036854775807", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -9223372036854775808-9223372036854775807", e, Item);
                        }
                    }
                }
                else if (Item.TypeValue == "ulong")
                {
                    string pattern = @"^\d{1,20}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, e.Text);
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, e.Text);
                    }

                    ulong d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-18446744073709551615", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-18446744073709551615", e, Item);
                        }
                    }
                    else if (ulong.TryParse(s, out d))
                    {
                        ulong up = 0;
                        ulong down = 0;

                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            if (tb == TBEmergencyUp)
                            {
                                ulong.TryParse(s, out up);
                                down = Convert.ToUInt64(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                ulong.TryParse(s, out down);
                                up = Convert.ToUInt64(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-18446744073709551615", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-18446744073709551615", e, Item);
                        }
                    }
                }
            }
        }
        
        void RemoveNetButton_Click(object sender, RoutedEventArgs e)
        {
            if (((ListBoxItem)ListEthernets.SelectedItem).Tag == "1")
            {
                NewEthernetSer.CollectionItemNetRec.Remove((ItemNet)DGRec.SelectedItem);
            }
            else
            {
                EthernetOperational ethernetOperational = (EthernetOperational)((ListBoxItem)ListEthernets.SelectedItem).Tag;

                ethernetOperational.CollectionItemNetRec.Remove((ItemNet)DGRec.SelectedItem);
            }  
            
            e.Handled = true;
        }

        void RemoveNetButtonSend_Click(object sender, RoutedEventArgs e)
        {
            if (((ListBoxItem)ListEthernets.SelectedItem).Tag == "1")
            {
                NewEthernetSer.CollectionItemNetSend.Remove((ItemNet)DGSend.SelectedItem);
            }
            else
            {
                EthernetOperational ethernetOperational = (EthernetOperational)((ListBoxItem)ListEthernets.SelectedItem).Tag;

                ethernetOperational.CollectionItemNetSend.Remove((ItemNet)DGSend.SelectedItem);
            }   

            e.Handled = true;
        }

        void GridPropertiesEthernetGeneral_Unloaded(object sender, RoutedEventArgs e)
        {
            IsStop = true;

            e.Handled = true;
        }

        void CorrectSelectionAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tbCorrectSelectionAll = (TextBox)sender;

            if (e.ClickCount > 1 && tbCorrectSelectionAll.SelectionLength > 0)
            {
                e.Handled = true;
                tbCorrectSelectionAll.CaretIndex = tbCorrectSelectionAll.GetCharacterIndexFromPoint(Mouse.GetPosition(tbCorrectSelectionAll), true);
            }

            if (!tbCorrectSelectionAll.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                Keyboard.Focus(tbCorrectSelectionAll);           
            }
        }
                        
        void Stop_Click(object sender, RoutedEventArgs e)
        {            
            IsStop = true;

            Stop.IsEnabled = false;
                      
            e.Handled = true;
        }

        void Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsStop)
                {
                    IsBindingStart = true;
                    Stop.IsEnabled = true;
                    AddNetButton.IsEnabled = false;
                    TBBufferSizeRec.IsReadOnly = true;
                    TBPortClient.IsReadOnly = true;
                    TBPortServer.IsReadOnly = true;
                    TBTime.IsReadOnly = true;
                    TBIPAdress1.IsReadOnly = true;
                    TBIPAdress2.IsReadOnly = true;
                    TBIPAdress3.IsReadOnly = true;
                    TBIPAdress4.IsReadOnly = true;
                    CBLocalIPs.IsEnabled = false;
                    CBProtocol.IsEnabled = false;

                    foreach (DataGridColumn column in DGRec.Columns)
                    {
                        if (column.Header == StaticValue.SRange0)
                        {
                            column.IsReadOnly = true;
                        }
                        else if (column.Header == StaticValue.SRange1)
                        {
                            column.IsReadOnly = true;
                        }
                        else if (column.Header == "Тип")
                        {
                            column.IsReadOnly = true;
                        }
                    }

                    IPAddressServer = TBIPAdress1.Text + "." + TBIPAdress2.Text + "." + TBIPAdress3.Text + "." + TBIPAdress4.Text;

                    if (CBLocalIPs.SelectedItem == null)
                    {
                        IPAddressClient = new IPAddress(EthernetControl.EthernetSer.IPAddressClient);
                    }
                    else
                    {
                        IPAddressClient = (IPAddress)CBLocalIPs.SelectedItem;
                    }
                  
                    PortServer = int.Parse(TBPortServer.Text);
                    PortClient = int.Parse(TBPortClient.Text);
                    BufferSize = ushort.Parse(TBBufferSizeRec.Text);
                    BufferSizeSend = ushort.Parse(TBBufferSizeSend.Text);
                    PeriodTime = int.Parse(TBTime.Text);
                    
                    if(CBProtocol.SelectedItem == "TCP")
                    {
                        TBStatus.Text = "Статус: подключение к " + IPAddressServer;

                        Thread threadConnect = new Thread(ConnectTCP);
                        threadConnect.IsBackground = true;
                        threadConnect.Start();
                    }
                    else
                    {
                        Thread threadConnect = new Thread(ConnectUDP);
                        threadConnect.IsBackground = true;
                        threadConnect.Start();
                    }
                    
                    foreach (EthernetOperational ethernetOperational in NewEthernetSer.CollectionEthernetOperational)
                    {
                        Thread threadConnectEthernetOperation = new Thread(ConnectEthernetOperation);
                        threadConnectEthernetOperation.IsBackground = true;
                        threadConnectEthernetOperation.Start(ethernetOperational);
                    }                   
                }
                else
                {
                    TBStatus.Text = "Статус: сокет еще не закрыт, повторите попытку позже.";
                }
            }
            catch (SystemException ex)
            {                
                TBStatus.Text = "Статус: " + ex;

                IsBindingStart = false;
                Stop.IsEnabled = false;
                AddNetButton.IsEnabled = true;
                TBBufferSizeRec.IsReadOnly = false;
                TBPortClient.IsReadOnly = false;
                TBPortServer.IsReadOnly = false;
                TBTime.IsReadOnly = false;
                TBIPAdress1.IsReadOnly = false;
                TBIPAdress2.IsReadOnly = false;
                TBIPAdress3.IsReadOnly = false;
                TBIPAdress4.IsReadOnly = false;
                CBLocalIPs.IsEnabled = true;
                CBProtocol.IsEnabled = true;

                foreach (DataGridColumn column in DGRec.Columns)
                {
                    if (column.Header == StaticValue.SRange0)
                    {
                        column.IsReadOnly = false;
                    }
                    else if (column.Header == StaticValue.SRange1)
                    {
                        column.IsReadOnly = false;
                    }
                    else if (column.Header == "Тип")
                    {
                        column.IsReadOnly = false;
                    }
                }                       
            }
            finally
            {               
                e.Handled = true;
            }            
        }

        byte[] formula(ItemNet item, int periodTime)
        {
            string modbusID;
            string itemModbusID;

            byte[] bWrite;

            if (item.ItemModbus != null)
            {
                if (item.ItemModbus.TypeValue == "float")
                {
                    bWrite = BitConverter.GetBytes((float)item.ItemModbus.Value);

                    return bWrite;
                }
                else if (item.ItemModbus.TypeValue == "int")
                {
                    bWrite = BitConverter.GetBytes((int)item.ItemModbus.Value);

                    return bWrite;
                }
                else if (item.ItemModbus.TypeValue == "uint")
                {
                    bWrite = bWrite = BitConverter.GetBytes((uint)item.ItemModbus.Value);

                    return bWrite;
                }
                else if (item.ItemModbus.TypeValue == "short")
                {
                    bWrite = BitConverter.GetBytes((short)item.ItemModbus.Value);

                    return bWrite;
                }
                else if (item.ItemModbus.TypeValue == "ushort")
                {
                    bWrite = BitConverter.GetBytes((ushort)item.ItemModbus.Value);

                    return bWrite;
                }
                else if (item.ItemModbus.TypeValue == "byte")
                {
                    bWrite = BitConverter.GetBytes((byte)item.ItemModbus.Value);

                    return bWrite;
                }
                else if (item.ItemModbus.TypeValue == "sbyte")
                {
                    bWrite = BitConverter.GetBytes((sbyte)item.ItemModbus.Value);

                    return bWrite;
                }
            }
            else if (item.Formula.IndexOf("ItemModbusID") != -1)
            {
                int modbusIDIndex = item.Text.IndexOf("ItemModbusID");
                    
                modbusID = item.Text.Substring(modbusIDIndex, 32);

                itemModbusID = item.Text.Substring(modbusIDIndex + 33, 32);

                foreach (ModbusSer mb in ((AppWPF)Application.Current).CollectionModbusSers)
                {
                    if (mb.ID == modbusID)
                    {
                        foreach(ItemModbus im in mb.CollectionItemModbus)
                        {
                            if (im.ID == itemModbusID)
                            {
                                item.ItemModbus = im;

                                if (im.TypeValue == "float")
                                {
                                    bWrite = BitConverter.GetBytes((float)im.Value);

                                    return bWrite;
                                }
                                else if (im.TypeValue == "int")
                                {
                                    bWrite = BitConverter.GetBytes((int)im.Value);

                                    return bWrite;
                                }
                                else if (im.TypeValue == "uint")
                                {
                                    bWrite = bWrite = BitConverter.GetBytes((uint)im.Value);

                                    return bWrite;
                                }
                                else if (im.TypeValue == "short")
                                {
                                    bWrite = BitConverter.GetBytes((short)im.Value);

                                    return bWrite;
                                }
                                else if (im.TypeValue == "ushort")
                                {
                                    bWrite = BitConverter.GetBytes((ushort)im.Value);

                                    return bWrite;
                                }
                                else if (im.TypeValue == "byte")
                                {
                                    bWrite = BitConverter.GetBytes((byte)im.Value);

                                    return bWrite;
                                }
                                else if (im.TypeValue == "sbyte")
                                {
                                    bWrite = BitConverter.GetBytes((sbyte)im.Value);

                                    return bWrite;
                                }
                            }
                        }
                    }
                }
            }
            else if (item.Formula.IndexOf("Period") != -1)
            {
                return bWrite = BitConverter.GetBytes(periodTime);
            }

            return null;
        }

        void ConnectEthernetOperation(object eo)
        {
            if (!IsStop)
            {
                try
                {
                    EthernetOperational ethernetOperational = (EthernetOperational)eo;

                    IPEndPoint localPoint = new IPEndPoint(IPAddressClient, PortClient);

                    TcpClient = new TcpClient(localPoint);

                    BufferSize = ethernetOperational.BufferSizeRec + 1;
                    BufferSizeSend = ethernetOperational.BufferSizeSend + 1;

                    byte[] bRead = new byte[BufferSize];
                    byte[] bWrite = new byte[BufferSizeSend];

                    TcpClient.Connect(IPAddress.Parse(IPAddressServer), ethernetOperational.Port);

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        TBStatus.Text = "Статус: соединение с сервером " + IPAddressServer + " порт " + ethernetOperational.Port + " выполнено.";
                    }));

                    NetworkStream stream = TcpClient.GetStream();

                    int[] aDecimal = new int[3];

                    Stopwatch timeCheckExit = new Stopwatch();

                    while (true)
                    {
                        if (ethernetOperational.CollectionItemNetRec.Count > 0)
                        {
                            if (TcpClient.Available == BufferSize)
                            {
                                stream.Read(bRead, 0, bRead.Length);

                                foreach (ItemNet item in NewEthernetSer.CollectionItemNetRec)
                                {
                                    if (item.TypeValue == "floatOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            item.Value = BitConverter.ToSingle(bRead, item.Range0);
                                        }                                       
                                    }
                                    else if (item.TypeValue == "doubleOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            item.Value = BitConverter.ToDouble(bRead, item.Range0);
                                        }                                       
                                    }
                                    else if (item.TypeValue == "decimalOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            aDecimal[0] = BitConverter.ToInt32(bRead, item.Range0);
                                            aDecimal[1] = BitConverter.ToInt32(bRead, item.Range0 + 4);
                                            aDecimal[2] = BitConverter.ToInt32(bRead, item.Range0 + 8);

                                            item.Value = new Decimal(aDecimal);
                                        }                                      
                                    }
                                    else if (item.TypeValue == "byteOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            item.Value = bRead[item.Range0];
                                        }                                        
                                    }
                                    else if (item.TypeValue == "sbyteOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            item.Value = (sbyte)bRead[item.Range0];
                                        }                                        
                                    }
                                    else if (item.TypeValue == "shortOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            item.Value = BitConverter.ToInt16(bRead, item.Range0);
                                        }                                       
                                    }
                                    else if (item.TypeValue == "ushortOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            item.Value = BitConverter.ToUInt16(bRead, item.Range0);
                                        }                                      
                                    }
                                    else if (item.TypeValue == "intOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            item.Value = BitConverter.ToInt32(bRead, item.Range0);
                                        }                                       
                                    }
                                    else if (item.TypeValue == "uintOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            item.Value = BitConverter.ToUInt32(bRead, item.Range0);
                                        }                                     
                                    }
                                    else if (item.TypeValue == "longOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            item.Value = BitConverter.ToInt64(bRead, item.Range0);
                                        }                                       
                                    }
                                    else if (item.TypeValue == "ulongOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            item.Value = BitConverter.ToUInt64(bRead, item.Range0);
                                        }                                       
                                    }
                                    else if (item.TypeValue == "boolOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            item.Value = BitConverter.ToBoolean(bRead, item.Range0);
                                        }                                      
                                    }
                                    else if (item.TypeValue == "charOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            item.Value = BitConverter.ToChar(bRead, item.Range0);
                                        }                                        
                                    }
                                    else if (item.TypeValue == "stringOperational")
                                    {
                                        if (BitConverter.ToBoolean(bRead, item.Range1 - 1))
                                        {
                                            item.Value = BitConverter.ToString(bRead, item.Range0);
                                        }                                       
                                    }
                                }
                            }
                        }
                        
                        while (true)
                        {
                            Thread.Sleep(StaticValues.TimeSleep);

                            if (IsStop)
                            {
                                IsStop = false;

                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    TBStatus.Text = "Статус: Опрос остановлен.";
                                }));

                                return;
                            }

                            timeCheckExit.Start();

                            if ((PeriodTime * 1000) >= timeCheckExit.ElapsedMilliseconds)
                            {
                                timeCheckExit.Reset();
                                break;
                            }
                        }
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10048)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            TBStatus.Text = "Статус: Порт: " + PortClient + " еще не освободился, повторите попытку позже.";
                        }));
                    }
                    else
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            TBStatus.Text = "Статус: " + ex;
                        }));
                    }
                }
                catch (SystemException ex)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        TBStatus.Text = "Статус: " + ex;
                    }));
                }
                finally
                {
                    if (TcpClient != null)
                    {
                        TcpClient.Close();
                    }

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        IsBindingStart = false;
                        Stop.IsEnabled = false;
                        AddNetButton.IsEnabled = true;
                        TBBufferSizeRec.IsReadOnly = false;
                        TBPortClient.IsReadOnly = false;
                        TBPortServer.IsReadOnly = false;
                        TBTime.IsReadOnly = false;
                        TBIPAdress1.IsReadOnly = false;
                        TBIPAdress2.IsReadOnly = false;
                        TBIPAdress3.IsReadOnly = false;
                        TBIPAdress4.IsReadOnly = false;
                        CBLocalIPs.IsEnabled = true;
                        CBProtocol.IsEnabled = true;

                        foreach (DataGridColumn column in DGRec.Columns)
                        {
                            if (column.Header == StaticValue.SRange0)
                            {
                                column.IsReadOnly = false;
                            }
                            else if (column.Header == StaticValue.SRange1)
                            {
                                column.IsReadOnly = false;
                            }
                            else if (column.Header == "Тип")
                            {
                                column.IsReadOnly = false;
                            }
                        }
                    }));
                }
            }
        }

        private void ConnectUDP()
        {
            try
            {
                Stopwatch timePeriod = new Stopwatch();
                Stopwatch timeCheckConnect = new Stopwatch();

                IPEndPoint remoteIP = new IPEndPoint(IPAddress.Parse(IPAddressServer), PortServer);
                IPEndPoint localIP = new IPEndPoint(IPAddressClient, PortClient);
                IPEndPoint remoteIPRec = null;

                UdpClient = new UdpClient(localIP);

                UdpClient.Client.ReceiveTimeout = PeriodTime * 2000;
                UdpClient.Client.SendTimeout = PeriodTime * 2000;

                byte[] bRead = new byte[BufferSize];
                byte[] bWrite = new byte[BufferSizeSend];

                int[] aDecimal = new int[3];

                byte[] formulaBuff;

                while(true)
                {
                    timePeriod.Start();
    
                    if (NewEthernetSer.CollectionItemNetSend.Count > 0)
                    {
                        foreach (ItemNet item in NewEthernetSer.CollectionItemNetSend)
                        {
                            if (item.TypeValue == "float")
                            {
                                formulaBuff = formula(item, 0);

                                if (formulaBuff != null)
                                {
                                    bWrite[item.Range0] = formulaBuff[0];
                                    bWrite[item.Range0 + 1] = formulaBuff[1];
                                    bWrite[item.Range0 + 2] = formulaBuff[2];
                                    bWrite[item.Range0 + 3] = formulaBuff[3];
                                }

                                item.Value = BitConverter.ToSingle(bWrite, item.Range0);
                            }
                            else if (item.TypeValue == "double")
                            {
                                item.Value = BitConverter.ToDouble(formula(item, 0), 0);
                            }
                            else if (item.TypeValue == "decimal")
                            {
                                aDecimal[0] = BitConverter.ToInt32(bRead, item.Range0);
                                aDecimal[1] = BitConverter.ToInt32(bRead, item.Range0 + 4);
                                aDecimal[2] = BitConverter.ToInt32(bRead, item.Range0 + 8);

                                item.Value = new Decimal(aDecimal);
                            }
                            else if (item.TypeValue == "byte")
                            {
                                formulaBuff = formula(item, 0);

                                if (formulaBuff != null)
                                {
                                    bWrite[item.Range0] = formulaBuff[0];
                                }

                                item.Value = bWrite[item.Range0];
                            }
                            else if (item.TypeValue == "sbyte")
                            {
                                formulaBuff = formula(item, 0);

                                if (formulaBuff != null)
                                {
                                    bWrite[item.Range0] = formulaBuff[0];
                                }

                                item.Value = (sbyte)bRead[item.Range0];
                            }
                            else if (item.TypeValue == "short")
                            {
                                formulaBuff = formula(item, 0);

                                if (formulaBuff != null)
                                {
                                    bWrite[item.Range0] = formulaBuff[0];
                                    bWrite[item.Range0 + 1] = formulaBuff[1];
                                }

                                item.Value = BitConverter.ToInt16(bWrite, item.Range0);
                            }
                            else if (item.TypeValue == "ushort")
                            {
                                formulaBuff = formula(item, 0);

                                if (formulaBuff != null)
                                {
                                    bWrite[item.Range0] = formulaBuff[0];
                                    bWrite[item.Range0 + 1] = formulaBuff[1];
                                }

                                item.Value = BitConverter.ToUInt16(bWrite, item.Range0);
                            }
                            else if (item.TypeValue == "int")
                            {
                                formulaBuff = formula(item, PeriodTime);

                                if (formulaBuff != null)
                                {
                                    bWrite[item.Range0] = formulaBuff[0];
                                    bWrite[item.Range0 + 1] = formulaBuff[1];
                                    bWrite[item.Range0 + 2] = formulaBuff[2];
                                    bWrite[item.Range0 + 3] = formulaBuff[3];
                                }

                                item.Value = BitConverter.ToInt32(bWrite, item.Range0);
                            }
                            else if (item.TypeValue == "uint")
                            {
                                formulaBuff = formula(item, 0);

                                if (formulaBuff != null)
                                {
                                    bWrite[item.Range0] = formulaBuff[0];
                                    bWrite[item.Range0 + 1] = formulaBuff[1];
                                    bWrite[item.Range0 + 2] = formulaBuff[2];
                                    bWrite[item.Range0 + 3] = formulaBuff[3];
                                }

                                item.Value = BitConverter.ToUInt32(bWrite, item.Range0);
                            }
                            else if (item.TypeValue == "long")
                            {
                                item.Value = BitConverter.ToInt64(bWrite, item.Range0);
                            }
                            else if (item.TypeValue == "ulong")
                            {
                                item.Value = BitConverter.ToUInt64(bWrite, item.Range0);
                            }
                            else if (item.TypeValue == "bool")
                            {
                                item.Value = BitConverter.ToBoolean(bWrite, item.Range0);
                            }
                            else if (item.TypeValue == "char")
                            {
                                item.Value = BitConverter.ToChar(bWrite, item.Range0);
                            }
                            else if (item.TypeValue == "string")
                            {
                                item.Value = BitConverter.ToString(bWrite, item.Range0);
                            }
                        }

                        UdpClient.Send(bWrite, bWrite.Length, remoteIP);                       
                    }

                    while (true)
                    {
                        if (IsStop)
                        {
                            IsStop = false;

                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                TBStatus.Text = "Статус: Опрос остановлен.";
                            }));

                            return;
                        }

                        if (UdpClient.Available != BufferSize)
                        {
                            timeCheckConnect.Start();

                            if ((PeriodTime * 2000) <= timeCheckConnect.ElapsedMilliseconds)
                            {
                                throw new Exception("Таймаут приема.");
                            }
                        }

                        if (UdpClient.Available == BufferSize)
                        {
                            timeCheckConnect.Reset();

                            bRead = UdpClient.Receive(ref remoteIPRec);

                            foreach (ItemNet item in NewEthernetSer.CollectionItemNetRec)
                            {
                                if (item.TypeValue == "float")
                                {
                                    item.Value = BitConverter.ToSingle(bRead, item.Range0);
                                }
                                else if (item.TypeValue == "double")
                                {
                                    item.Value = BitConverter.ToDouble(bRead, item.Range0);
                                }
                                else if (item.TypeValue == "decimal")
                                {
                                    aDecimal[0] = BitConverter.ToInt32(bRead, item.Range0);
                                    aDecimal[1] = BitConverter.ToInt32(bRead, item.Range0 + 4);
                                    aDecimal[2] = BitConverter.ToInt32(bRead, item.Range0 + 8);

                                    item.Value = new Decimal(aDecimal);
                                }
                                else if (item.TypeValue == "byte")
                                {
                                    item.Value = bRead[item.Range0];
                                }
                                else if (item.TypeValue == "sbyte")
                                {
                                    item.Value = (sbyte)bRead[item.Range0];
                                }
                                else if (item.TypeValue == "short")
                                {
                                    item.Value = BitConverter.ToInt16(bRead, item.Range0);
                                }
                                else if (item.TypeValue == "ushort")
                                {
                                    item.Value = BitConverter.ToUInt16(bRead, item.Range0);
                                }
                                else if (item.TypeValue == "int")
                                {
                                    item.Value = BitConverter.ToInt32(bRead, item.Range0);
                                }
                                else if (item.TypeValue == "uint")
                                {
                                    item.Value = BitConverter.ToUInt32(bRead, item.Range0);
                                }
                                else if (item.TypeValue == "long")
                                {
                                    item.Value = BitConverter.ToInt64(bRead, item.Range0);
                                }
                                else if (item.TypeValue == "ulong")
                                {
                                    item.Value = BitConverter.ToUInt64(bRead, item.Range0);
                                }
                                else if (item.TypeValue == "bool")
                                {
                                    item.Value = BitConverter.ToBoolean(bRead, item.Range0);
                                }
                                else if (item.TypeValue == "char")
                                {
                                    item.Value = BitConverter.ToChar(bRead, item.Range0);
                                }
                                else if (item.TypeValue == "string")
                                {
                                    item.Value = BitConverter.ToString(bRead, item.Range0);
                                }
                            }

                            break;
                        }

                        Thread.Sleep(StaticValues.TimeSleep);
                    }
                                      
                    while (true)
                    {                                           
                        if (IsStop)
                        {                            
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                TBStatus.Text = "Статус: Опрос остановлен.";
                            }));

                            return;
                        }

                        if ((PeriodTime * 1000) <= timePeriod.ElapsedMilliseconds)
                        {
                            timePeriod.Reset();
                            break;
                        }

                        Thread.Sleep(StaticValues.TimeSleep);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBStatus.Text = "Статус: " + ex;
                }));
            }
            finally
            {
                if (UdpClient != null)
                {
                    UdpClient.Close();
                }

                foreach (ItemNet item in NewEthernetSer.CollectionItemNetSend)
                {
                    item.ItemModbus = null;
                }

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    IsBindingStart = false;
                    Stop.IsEnabled = false;
                    AddNetButton.IsEnabled = true;
                    TBBufferSizeRec.IsReadOnly = false;
                    TBPortClient.IsReadOnly = false;
                    TBPortServer.IsReadOnly = false;
                    TBTime.IsReadOnly = false;
                    TBIPAdress1.IsReadOnly = false;
                    TBIPAdress2.IsReadOnly = false;
                    TBIPAdress3.IsReadOnly = false;
                    TBIPAdress4.IsReadOnly = false;
                    CBLocalIPs.IsEnabled = true;
                    CBProtocol.IsEnabled = true;

                    foreach (DataGridColumn column in DGRec.Columns)
                    {
                        if (column.Header == StaticValue.SRange0)
                        {
                            column.IsReadOnly = false;
                        }
                        else if (column.Header == StaticValue.SRange1)
                        {
                            column.IsReadOnly = false;
                        }
                        else if (column.Header == "Тип")
                        {
                            column.IsReadOnly = false;
                        }
                    }
                }));

                IsStop = false;
            }
        }

        void ConnectTCP()
        {
            if (!IsStop)
            {
                try
                {
                    Stopwatch timePeriod = new Stopwatch();
                    Stopwatch timeCheckConnect = new Stopwatch();

                    IPEndPoint localPoint = new IPEndPoint(IPAddressClient, PortClient);

                    TcpClient = new TcpClient(localPoint);
                    TcpClient.ReceiveTimeout = PeriodTime * 2000;
                    TcpClient.SendTimeout = PeriodTime * 2000; ;

                    byte[] bRead = new byte[BufferSize];
                    byte[] bWrite = new byte[BufferSizeSend];

                    TcpClient.BeginConnect(IPAddress.Parse(IPAddressServer), PortServer, null, null);

                    while (true)
                    {
                        timeCheckConnect.Start();

                        if ((PeriodTime * 2000) <= timeCheckConnect.ElapsedMilliseconds)
                        {
                            throw new Exception("Не удалось подключится к серверу.");
                        }

                        if (TcpClient.Connected)
                        {
                            timeCheckConnect.Reset();
                            break;
                        }

                        if(IsStop)
                        {
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                TBStatus.Text = "Статус: Опрос остановлен.";
                            }));                        

                            return;
                        }

                        Thread.Sleep(StaticValues.TimeSleep);
                    }

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        TBStatus.Text = "Статус: соединение с сервером " + IPAddressServer + " выполнено.";
                    }));

                    NetworkStream stream = TcpClient.GetStream();

                    int[] aDecimal = new int[3];                    

                    byte[] formulaBuff;

                    while (true)
                    {
                        timePeriod.Start();

                        if (NewEthernetSer.CollectionItemNetSend.Count > 0)
                        {
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                TBStatus.Text = "Статус: передача данных.";
                            }));

                            foreach (ItemNet item in NewEthernetSer.CollectionItemNetSend)
                            {
                                if (item.TypeValue == "float")
                                {
                                    formulaBuff = formula(item, 0);

                                    if (formulaBuff != null)
                                    {
                                        bWrite[item.Range0] = formulaBuff[0];
                                        bWrite[item.Range0 + 1] = formulaBuff[1];
                                        bWrite[item.Range0 + 2] = formulaBuff[2];
                                        bWrite[item.Range0 + 3] = formulaBuff[3];
                                    }

                                    item.Value = BitConverter.ToSingle(bWrite, item.Range0);
                                }
                                else if (item.TypeValue == "double")
                                {
                                    item.Value = BitConverter.ToDouble(formula(item, 0), 0);
                                }
                                else if (item.TypeValue == "decimal")
                                {
                                    aDecimal[0] = BitConverter.ToInt32(bRead, item.Range0);
                                    aDecimal[1] = BitConverter.ToInt32(bRead, item.Range0 + 4);
                                    aDecimal[2] = BitConverter.ToInt32(bRead, item.Range0 + 8);

                                    item.Value = new Decimal(aDecimal);
                                }
                                else if (item.TypeValue == "byte")
                                {
                                    formulaBuff = formula(item, 0);

                                    if (formulaBuff != null)
                                    {
                                        bWrite[item.Range0] = formulaBuff[0];
                                    }

                                    item.Value = bWrite[item.Range0];
                                }
                                else if (item.TypeValue == "sbyte")
                                {
                                    formulaBuff = formula(item, 0);

                                    if (formulaBuff != null)
                                    {
                                        bWrite[item.Range0] = formulaBuff[0];
                                    }

                                    item.Value = (sbyte)bRead[item.Range0];
                                }
                                else if (item.TypeValue == "short")
                                {
                                    formulaBuff = formula(item, 0);

                                    if (formulaBuff != null)
                                    {
                                        bWrite[item.Range0] = formulaBuff[0];
                                        bWrite[item.Range0 + 1] = formulaBuff[1];
                                    }

                                    item.Value = BitConverter.ToInt16(bWrite, item.Range0);
                                }
                                else if (item.TypeValue == "ushort")
                                {
                                    formulaBuff = formula(item, 0);

                                    if (formulaBuff != null)
                                    {
                                        bWrite[item.Range0] = formulaBuff[0];
                                        bWrite[item.Range0 + 1] = formulaBuff[1];
                                    }

                                    item.Value = BitConverter.ToUInt16(bWrite, item.Range0);
                                }
                                else if (item.TypeValue == "int")
                                {
                                    formulaBuff = formula(item, PeriodTime);

                                    if (formulaBuff != null)
                                    {
                                        bWrite[item.Range0] = formulaBuff[0];
                                        bWrite[item.Range0 + 1] = formulaBuff[1];
                                        bWrite[item.Range0 + 2] = formulaBuff[2];
                                        bWrite[item.Range0 + 3] = formulaBuff[3];
                                    }

                                    item.Value = BitConverter.ToInt32(bWrite, item.Range0);
                                }
                                else if (item.TypeValue == "uint")
                                {
                                    formulaBuff = formula(item, 0);

                                    if (formulaBuff != null)
                                    {
                                        bWrite[item.Range0] = formulaBuff[0];
                                        bWrite[item.Range0 + 1] = formulaBuff[1];
                                        bWrite[item.Range0 + 2] = formulaBuff[2];
                                        bWrite[item.Range0 + 3] = formulaBuff[3];
                                    }

                                    item.Value = BitConverter.ToUInt32(bWrite, item.Range0);
                                }
                                else if (item.TypeValue == "long")
                                {
                                    item.Value = BitConverter.ToInt64(bWrite, item.Range0);
                                }
                                else if (item.TypeValue == "ulong")
                                {
                                    item.Value = BitConverter.ToUInt64(bWrite, item.Range0);
                                }
                                else if (item.TypeValue == "bool")
                                {
                                    item.Value = BitConverter.ToBoolean(bWrite, item.Range0);
                                }
                                else if (item.TypeValue == "char")
                                {
                                    item.Value = BitConverter.ToChar(bWrite, item.Range0);
                                }
                                else if (item.TypeValue == "string")
                                {
                                    item.Value = BitConverter.ToString(bWrite, item.Range0);
                                }
                            }

                            stream.Write(bWrite, 0, bWrite.Length);

                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                TBStatus.Text = "Статус: передача данных выполнена " + bWrite.Length + " " + "байт.";
                            }));
                        }

                        while (true)
                        {
                            if (IsStop)
                            {                               
                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    TBStatus.Text = "Статус: Опрос остановлен.";
                                }));

                                return;
                            } 

                            if (TcpClient.Available != BufferSize)
                            {
                                timeCheckConnect.Start();

                                if ((PeriodTime * 2000) <= timeCheckConnect.ElapsedMilliseconds)
                                {
                                    throw new Exception("Таймаут приема, разрываем соединение.");
                                }
                            }

                            if (TcpClient.Available == BufferSize)
                            {
                                timeCheckConnect.Reset();

                                stream.Read(bRead, 0, bRead.Length);

                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    TBStatus.Text = "Статус: прием данных.";
                                }));

                                foreach (ItemNet item in NewEthernetSer.CollectionItemNetRec)
                                {
                                    if (item.TypeValue == "float")
                                    {
                                        item.Value = BitConverter.ToSingle(bRead, item.Range0);
                                    }
                                    else if (item.TypeValue == "double")
                                    {
                                        item.Value = BitConverter.ToDouble(bRead, item.Range0);
                                    }
                                    else if (item.TypeValue == "decimal")
                                    {
                                        aDecimal[0] = BitConverter.ToInt32(bRead, item.Range0);
                                        aDecimal[1] = BitConverter.ToInt32(bRead, item.Range0 + 4);
                                        aDecimal[2] = BitConverter.ToInt32(bRead, item.Range0 + 8);

                                        item.Value = new Decimal(aDecimal);
                                    }
                                    else if (item.TypeValue == "byte")
                                    {
                                        item.Value = bRead[item.Range0];
                                    }
                                    else if (item.TypeValue == "sbyte")
                                    {
                                        item.Value = (sbyte)bRead[item.Range0];
                                    }
                                    else if (item.TypeValue == "short")
                                    {
                                        item.Value = BitConverter.ToInt16(bRead, item.Range0);
                                    }
                                    else if (item.TypeValue == "ushort")
                                    {
                                        item.Value = BitConverter.ToUInt16(bRead, item.Range0);
                                    }
                                    else if (item.TypeValue == "int")
                                    {
                                        item.Value = BitConverter.ToInt32(bRead, item.Range0);
                                    }
                                    else if (item.TypeValue == "uint")
                                    {
                                        item.Value = BitConverter.ToUInt32(bRead, item.Range0);
                                    }
                                    else if (item.TypeValue == "long")
                                    {
                                        item.Value = BitConverter.ToInt64(bRead, item.Range0);
                                    }
                                    else if (item.TypeValue == "ulong")
                                    {
                                        item.Value = BitConverter.ToUInt64(bRead, item.Range0);
                                    }
                                    else if (item.TypeValue == "bool")
                                    {
                                        item.Value = BitConverter.ToBoolean(bRead, item.Range0);
                                    }
                                    else if (item.TypeValue == "char")
                                    {
                                        item.Value = BitConverter.ToChar(bRead, item.Range0);
                                    }
                                    else if (item.TypeValue == "string")
                                    {
                                        item.Value = BitConverter.ToString(bRead, item.Range0);
                                    }
                                }

                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    TBStatus.Text = "Статус: прием данных выполнен " + bRead.Length + " " + "байт.";
                                }));

                                break;
                            }

                            Thread.Sleep(StaticValues.TimeSleep);
                        }                      
                        
                        while(true)
                        {                                                       
                            if (IsStop)
                            {
                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    TBStatus.Text = "Статус: Опрос остановлен.";
                                }));

                                return;
                            } 

                            if ((PeriodTime * 1000) <= timePeriod.ElapsedMilliseconds)
                            {
                                timePeriod.Reset();
                                break;
                            }

                            Thread.Sleep(StaticValues.TimeSleep);
                        }                                                                                                               
                    }
                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        TBStatus.Text = "Статус: " + ex;
                    }));
                }
                finally
                {
                    if (TcpClient != null)
                    {
                        TcpClient.Close();
                    }

                    foreach (ItemNet item in NewEthernetSer.CollectionItemNetSend)
                    {
                        item.ItemModbus = null;
                    }

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        IsBindingStart = false;
                        Stop.IsEnabled = false;
                        AddNetButton.IsEnabled = true;
                        TBBufferSizeRec.IsReadOnly = false;
                        TBPortClient.IsReadOnly = false;
                        TBPortServer.IsReadOnly = false;
                        TBTime.IsReadOnly = false;
                        TBIPAdress1.IsReadOnly = false;
                        TBIPAdress2.IsReadOnly = false;
                        TBIPAdress3.IsReadOnly = false;
                        TBIPAdress4.IsReadOnly = false;
                        CBLocalIPs.IsEnabled = true;
                        CBProtocol.IsEnabled = true;

                        foreach (DataGridColumn column in DGRec.Columns)
                        {
                            if (column.Header == StaticValue.SRange0)
                            {
                                column.IsReadOnly = false;
                            }
                            else if (column.Header == StaticValue.SRange1)
                            {
                                column.IsReadOnly = false;
                            }
                            else if (column.Header == "Тип")
                            {
                                column.IsReadOnly = false;
                            }
                        }
                    }));

                    IsStop = false;
                }
            }
        }

        void DG_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {           
            ContentPresenter element = (ContentPresenter)e.EditingElement;

            Item = (ItemNet)e.Row.DataContext;

            int.TryParse(TBBufferSizeRec.Text, out BufferSize);

            BufferSize = BufferSize - 1;

            MaxDigit = TBBufferSizeRec.Text.Length;

            if (element.ContentTemplate.FindName("ComboBoxType", element) != null)
            {
                ComboBox cbType = (ComboBox)element.ContentTemplate.FindName("ComboBoxType", element);

                if (!IsEthernetOperational)
                {
                    ComboBoxItem cbFloat = new ComboBoxItem();
                    cbFloat.ToolTip = "Число одинарной точности, 4 байта.";
                    cbFloat.Content = "float";

                    ComboBoxItem cbDouble = new ComboBoxItem();
                    cbDouble.ToolTip = "Число двойной точности, 8 байт.";
                    cbDouble.Content = "double";

                    ComboBoxItem cbDecimal = new ComboBoxItem();
                    cbDecimal.ToolTip = "Десятичное число, 12 байт.";
                    cbDecimal.Content = "decimal";

                    ComboBoxItem cbByte = new ComboBoxItem();
                    cbByte.ToolTip = "Целочисленный тип: 0-255, 1 байт.";
                    cbByte.Content = "byte";

                    ComboBoxItem cbSByte = new ComboBoxItem();
                    cbSByte.ToolTip = "Целочисленный тип: -128-127, 1 байт.";
                    cbSByte.Content = "sbyte";

                    ComboBoxItem cbShort = new ComboBoxItem();
                    cbShort.ToolTip = "Целочисленный тип: -32768-32767, 2 байта.";
                    cbShort.Content = "short";

                    ComboBoxItem cbUShort = new ComboBoxItem();
                    cbUShort.ToolTip = "Целочисленный тип: 0-65535, 2 байта.";
                    cbUShort.Content = "ushort";

                    ComboBoxItem cbInt = new ComboBoxItem();
                    cbInt.ToolTip = "Целочисленный тип: -2147483648-2147483647, 4 байта.";
                    cbInt.Content = "int";

                    ComboBoxItem cbUInt = new ComboBoxItem();
                    cbUInt.ToolTip = "Целочисленный тип: 0-4294967295, 4 байта.";
                    cbUInt.Content = "uint";

                    ComboBoxItem cbLong = new ComboBoxItem();
                    cbLong.ToolTip = "Целочисленный тип: -9223372036854775808-9223372036854775807, 8 байт.";
                    cbLong.Content = "long";

                    ComboBoxItem cbULong = new ComboBoxItem();
                    cbULong.ToolTip = "Целочисленный тип: 0-18446744073709551615, 8 байт.";
                    cbULong.Content = "ulong";

                    //ComboBoxItem cbBool = new ComboBoxItem();
                    //cbBool.ToolTip = "Логический тип, 1 байт.";
                    //cbBool.Content = "bool";

                    ComboBoxItem cbChar = new ComboBoxItem();
                    cbChar.ToolTip = "Символьный тип, 2 байта.";
                    cbChar.Content = "char";

                    ComboBoxItem cbString = new ComboBoxItem();
                    cbString.ToolTip = "Строковый тип.";
                    cbString.Content = "string";

                    List<ComboBoxItem> cbItem = new List<ComboBoxItem>();
                    cbItem.Add(cbFloat);
                    cbItem.Add(cbDouble);
                    cbItem.Add(cbDecimal);
                    cbItem.Add(cbByte);
                    cbItem.Add(cbSByte);
                    cbItem.Add(cbShort);
                    cbItem.Add(cbUShort);
                    cbItem.Add(cbInt);
                    cbItem.Add(cbUInt);
                    cbItem.Add(cbLong);
                    cbItem.Add(cbULong);
                    //cbItem.Add(cbBool);
                    cbItem.Add(cbChar);
                    cbItem.Add(cbString);

                    Binding bindingType = new Binding();
                    bindingType.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bindingType.Path = new PropertyPath("TypeValue");

                    cbType.SetValue(ComboBox.ItemsSourceProperty, cbItem);
                    cbType.SetValue(ComboBox.SelectedValuePathProperty, "Content");
                    cbType.SetBinding(ComboBox.SelectedValueProperty, bindingType);
                }
                else
                {
                    ComboBoxItem cbFloat = new ComboBoxItem();
                    cbFloat.ToolTip = "Число одинарной точности, 5 байта.";
                    cbFloat.Content = "floatOperational";

                    ComboBoxItem cbDouble = new ComboBoxItem();
                    cbDouble.ToolTip = "Число двойной точности, 9 байт.";
                    cbDouble.Content = "doubleOperational";

                    ComboBoxItem cbDecimal = new ComboBoxItem();
                    cbDecimal.ToolTip = "Десятичное число, 13 байт.";
                    cbDecimal.Content = "decimalOperational";

                    ComboBoxItem cbByte = new ComboBoxItem();
                    cbByte.ToolTip = "Целочисленный тип: 0-255, 2 байта.";
                    cbByte.Content = "byteOperational";

                    ComboBoxItem cbSByte = new ComboBoxItem();
                    cbSByte.ToolTip = "Целочисленный тип: -128-127, 2 байта.";
                    cbSByte.Content = "sbyteOperational";

                    ComboBoxItem cbShort = new ComboBoxItem();
                    cbShort.ToolTip = "Целочисленный тип: -32768-32767, 3 байта.";
                    cbShort.Content = "shortOperational";

                    ComboBoxItem cbUShort = new ComboBoxItem();
                    cbUShort.ToolTip = "Целочисленный тип: 0-65535, 3 байта.";
                    cbUShort.Content = "ushortOperational";

                    ComboBoxItem cbInt = new ComboBoxItem();
                    cbInt.ToolTip = "Целочисленный тип: -2147483648-2147483647, 5 байта.";
                    cbInt.Content = "intOperational";

                    ComboBoxItem cbUInt = new ComboBoxItem();
                    cbUInt.ToolTip = "Целочисленный тип: 0-4294967295, 5 байта.";
                    cbUInt.Content = "uintOperational";

                    ComboBoxItem cbLong = new ComboBoxItem();
                    cbLong.ToolTip = "Целочисленный тип: -9223372036854775808-9223372036854775807, 9 байт.";
                    cbLong.Content = "longOperational";

                    ComboBoxItem cbULong = new ComboBoxItem();
                    cbULong.ToolTip = "Целочисленный тип: 0-18446744073709551615, 9 байт.";
                    cbULong.Content = "ulongOperational";

                    ComboBoxItem cbBool = new ComboBoxItem();
                    cbBool.ToolTip = "Логический тип, 2 байта.";
                    cbBool.Content = "boolOperational";

                    ComboBoxItem cbChar = new ComboBoxItem();
                    cbChar.ToolTip = "Символьный тип, 3 байта.";
                    cbChar.Content = "charOperational";

                    ComboBoxItem cbString = new ComboBoxItem();
                    cbString.ToolTip = "Строковый тип.";
                    cbString.Content = "stringOperational";

                    List<ComboBoxItem> cbItem = new List<ComboBoxItem>();
                    cbItem.Add(cbFloat);
                    cbItem.Add(cbDouble);
                    cbItem.Add(cbDecimal);
                    cbItem.Add(cbByte);
                    cbItem.Add(cbSByte);
                    cbItem.Add(cbShort);
                    cbItem.Add(cbUShort);
                    cbItem.Add(cbInt);
                    cbItem.Add(cbUInt);
                    cbItem.Add(cbLong);
                    cbItem.Add(cbULong);
                    cbItem.Add(cbBool);
                    cbItem.Add(cbChar);
                    cbItem.Add(cbString);

                    Binding bindingType = new Binding();
                    bindingType.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bindingType.Path = new PropertyPath("TypeValue");

                    cbType.SetValue(ComboBox.ItemsSourceProperty, cbItem);
                    cbType.SetValue(ComboBox.SelectedValuePathProperty, "Content");
                    cbType.SetBinding(ComboBox.SelectedValueProperty, bindingType);
                }                                            

                EscText = (string)((ComboBoxItem)cbType.SelectedItem).Content;
            }
            else if (element.ContentTemplate.FindName("TextBoxRange0", element) != null)
            {
                TBRange0 = (TextBox)element.ContentTemplate.FindName("TextBoxRange0", element);

                if (NewEthernetSer.CollectionItemNetSend.Contains(Item))
                {
                    TBRange0.Tag = "S";
                }
                else
                {
                    TBRange0.Tag = "R";
                }

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBRange0.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBRange0.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
                TBRange0.ContextMenu = ContextMenu;

                decimal.TryParse(TBRange0.Text, out EscDigital);

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBRange0.Focus();
                }));
            }
            else if (element.ContentTemplate.FindName("TextBoxRange1", element) != null)
            {
                TBRange1 = (TextBox)element.ContentTemplate.FindName("TextBoxRange1", element);

                if (NewEthernetSer.CollectionItemNetSend.Contains(Item))
                {
                    TBRange1.Tag = "S";
                }
                else
                {
                    TBRange1.Tag = "R";
                }

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBRange1.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBRange1.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
                TBRange1.ContextMenu = ContextMenu;

                decimal.TryParse(TBRange1.Text, out EscDigital);

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBRange1.Focus();
                }));
            }
            else if (element.ContentTemplate.FindName("TextBoxEmergencyUp", element) != null)
            {
                TBEmergencyUp = (TextBox)element.ContentTemplate.FindName("TextBoxEmergencyUp", element);
                TBEmergencyUp.Text = TBEmergencyUp.Text.Replace(',', '.');

                if (NewEthernetSer.CollectionItemNetSend.Contains(Item))
                {
                    TBEmergencyUp.Tag = "S";
                }
                else
                {
                    TBEmergencyUp.Tag = "R";
                }

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBEmergencyUp.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBEmergencyUp.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
                TBEmergencyUp.ContextMenu = ContextMenu;                

                if (Item.TypeValue == "bool")
                {
                    TBEmergencyUp.IsReadOnly = true;
                }
                else
                {
                    decimal.TryParse(TBEmergencyUp.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out EscDigital);

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        TBEmergencyUp.Focus();
                    }));
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxEmergencyDown", element) != null)
            {
                TBEmergencyDown = (TextBox)element.ContentTemplate.FindName("TextBoxEmergencyDown", element);                
                TBEmergencyDown.Text = TBEmergencyDown.Text.Replace(',', '.');

                if (NewEthernetSer.CollectionItemNetSend.Contains(Item))
                {
                    TBEmergencyDown.Tag = "S";
                }
                else
                {
                    TBEmergencyDown.Tag = "R";
                }

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBEmergencyDown.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBEmergencyDown.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
                TBEmergencyDown.ContextMenu = ContextMenu;                            

                if (Item.TypeValue == "bool")
                {
                    TBEmergencyDown.IsReadOnly = true;
                }
                else
                {
                    decimal.TryParse(TBEmergencyDown.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out EscDigital);

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        TBEmergencyDown.Focus();
                    }));
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxPeriodEmergencySaveDB", element) != null)
            {
                TBEmergencySaveBD = (TextBox)element.ContentTemplate.FindName("TextBoxPeriodEmergencySaveDB", element);

                if (NewEthernetSer.CollectionItemNetSend.Contains(Item))
                {
                    TBEmergencySaveBD.Tag = "S";
                }
                else
                {
                    TBEmergencySaveBD.Tag = "R";
                }

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBEmergencySaveBD.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBEmergencySaveBD.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
                TBEmergencySaveBD.ContextMenu = ContextMenu;

                decimal.TryParse(TBEmergencySaveBD.Text, out EscDigital);

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBEmergencySaveBD.Focus();
                }));
            }
            else if (element.ContentTemplate.FindName("TextBoxDescription", element) != null)
            {
                TBDescriptionItemNet = (TextBox)element.ContentTemplate.FindName("TextBoxDescription", element);                
                TBDescriptionItemNet.AcceptsReturn = true;
                TBDescriptionItemNet.MaxLines = 3;

                if (NewEthernetSer.CollectionItemNetSend.Contains(Item))
                {
                    TBDescriptionItemNet.Tag = "S";
                }
                else
                {
                    TBDescriptionItemNet.Tag = "R";
                }

                EscText = TBDescriptionItemNet.Text;

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBDescriptionItemNet.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBDescriptionItemNet.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextTextBoxPaste));
                TBDescriptionItemNet.ContextMenu = ContextMenu;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBDescriptionItemNet.Focus();
                }));
            }
            else if (element.ContentTemplate.FindName("TextBoxText", element) != null)
            {
                TBText = (TextBox)element.ContentTemplate.FindName("TextBoxText", element);

                if (NewEthernetSer.CollectionItemNetSend.Contains(Item))
                {
                    TBText.Tag = "S";
                }
                else
                {
                    TBText.Tag = "R";
                }

                EscText = TBText.Text;

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBText.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBText.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextTextBoxPaste));
                TBText.ContextMenu = ContextMenu;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBText.Focus();
                }));
            }
            else if (element.ContentTemplate.FindName("TextBoxFormula", element) != null)
            {
                TBFormula = (TextBox)element.ContentTemplate.FindName("TextBoxFormula", element);

                if (NewEthernetSer.CollectionItemNetSend.Contains(Item))
                {
                    TBFormula.Tag = "S";
                }
                else
                {
                    TBFormula.Tag = "R";
                }

                EscText = TBFormula.Text;

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBFormula.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBFormula.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextTextBoxPaste));
                TBFormula.ContextMenu = ContextMenu;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBFormula.Focus();
                }));
            }
            else if (element.ContentTemplate.FindName("TextBoxTableName", element) != null)
            {
                TBTableName = (TextBox)element.ContentTemplate.FindName("TextBoxTableName", element);

                if (NewEthernetSer.CollectionItemNetSend.Contains(Item))
                {
                    TBTableName.Tag = "S";
                }
                else
                {
                    TBTableName.Tag = "R";
                }

                EscText = TBTableName.Text;

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBTableName.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBTableName.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextTextBoxPaste));
                TBTableName.ContextMenu = ContextMenu;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBTableName.Focus();
                }));
            }
            else if (element.ContentTemplate.FindName("TextBoxPeriodSaveDB", element) != null)
            {
                TBPeriodTimeSaveDB = (TextBox)element.ContentTemplate.FindName("TextBoxPeriodSaveDB", element);

                if (NewEthernetSer.CollectionItemNetSend.Contains(Item))
                {
                    TBPeriodTimeSaveDB.Tag = "S";
                }
                else
                {
                    TBPeriodTimeSaveDB.Tag = "R";
                }

                decimal.TryParse(TBPeriodTimeSaveDB.Text, out EscDigital);

                MenuItem menuItemPaste = new MenuItem();
                menuItemPaste.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenu = new System.Windows.Controls.ContextMenu();
                ContextMenu.Items.Add(menuItemPaste);
                ContextMenu.Items.Add(menuItemCopy);

                TBPeriodTimeSaveDB.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                TBPeriodTimeSaveDB.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DigitalTextBoxPaste));
                TBPeriodTimeSaveDB.ContextMenu = ContextMenu;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TBPeriodTimeSaveDB.Focus();
                }));
            }
        }
               
        void DG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ContentPresenter element = (ContentPresenter)e.EditingElement;

            if (element.ContentTemplate.FindName("TextBoxRange0", element) != null)
            {
                TextBox rangeTextBox = (TextBox)element.ContentTemplate.FindName("TextBoxRange0", element);

                if (rangeTextBox.Text.Length == 0)
                {
                    rangeTextBox.Text = EscDigital.ToString();
                }

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    rangeTextBox.Text = EscDigital.ToString();
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxRange1", element) != null)
            {
                TextBox rangeTextBox = (TextBox)element.ContentTemplate.FindName("TextBoxRange1", element);

                if (rangeTextBox.Text.Length == 0)
                {
                    rangeTextBox.Text = EscDigital.ToString();
                }

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    rangeTextBox.Text = EscDigital.ToString();
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxDescription", element) != null)
            {
                TextBox tbDescription = (TextBox)element.ContentTemplate.FindName("TextBoxDescription", element);

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {                    
                    tbDescription.Text = EscText;
                }
            }
            else if (element.ContentTemplate.FindName("ComboBoxType", element) != null)
            {
                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    ComboBox cbType = (ComboBox)element.ContentTemplate.FindName("ComboBoxType", element);
                    cbType.SelectedValue = EscText;
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxTableName", element) != null)
            {
                TextBox tbTableName = (TextBox)element.ContentTemplate.FindName("TextBoxTableName", element);

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {                   
                    tbTableName.Text = EscText;
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxText", element) != null)
            {
                TextBox tbTableName = (TextBox)element.ContentTemplate.FindName("TextBoxText", element);

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {                    
                    tbTableName.Text = EscText;
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxFormula", element) != null)
            {
                TextBox tb = (TextBox)element.ContentTemplate.FindName("TextBoxFormula", element);

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    tb.Text = EscText;
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxPeriodSaveDB", element) != null)
            {
                TextBox periodTextBox = (TextBox)element.ContentTemplate.FindName("TextBoxPeriodSaveDB", element);

                if (periodTextBox.Text.Length == 0)
                {
                    periodTextBox.Text = EscDigital.ToString();
                }

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    periodTextBox.Text = EscDigital.ToString();
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxEmergencyUp", element) != null)
            {
                TextBox tb = (TextBox)element.ContentTemplate.FindName("TextBoxEmergencyUp", element);

                PopupMessage.IsOpen = false;

                if (tb.Text.Length == 0)
                {
                    tb.Text = EscDigital.ToString();
                }
           
                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    if (Item.TypeValue != "bool")
                    {
                        tb.Text = EscDigital.ToString(CultureInfo.InvariantCulture);
                    }                   
                }                                
            }
            else if (element.ContentTemplate.FindName("TextBoxEmergencyDown", element) != null)
            {
                TextBox tb = (TextBox)element.ContentTemplate.FindName("TextBoxEmergencyDown", element);

                PopupMessage.IsOpen = false;

                if (tb.Text.Length == 0)
                {
                    tb.Text = EscDigital.ToString();
                }

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    if (Item.TypeValue != "bool")
                    {
                        tb.Text = EscDigital.ToString(CultureInfo.InvariantCulture);
                    }                  
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxPeriodEmergencySaveDB", element) != null)
            {
                TextBox tb = (TextBox)element.ContentTemplate.FindName("TextBoxPeriodEmergencySaveDB", element);

                PopupMessage.IsOpen = false;

                if (tb.Text.Length == 0)
                {
                    tb.Text = EscDigital.ToString();
                }

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    tb.Text = EscDigital.ToString();
                }
            }
        }
                            
        private void DigitalTextBoxPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            TextBox tb = (TextBox)sender;
            string s;

            if (tb == TBPeriodTimeSaveDB || tb == TBEmergencySaveBD || tb == TBTime)
            {
                string pattern = @"^\d{1,5}$";

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    if (tb == TBTime)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = StaticValue.SRange1_86400;
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;
                    }
                    else if (tb == TBPeriodTimeSaveDB)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e, Item);
                    }
                    else if (tb == TBEmergencySaveBD)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e, Item);
                    }
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 1 || d > 86400)
                    {
                        if (tb == TBTime)
                        {
                            PopupMessage.HorizontalOffset = 0;
                            LPopupMessage.Content = StaticValue.SRange1_86400;
                            PopupMessage.PlacementTarget = tb;
                            PopupMessage.IsOpen = true;
                        }
                        else if (tb == TBPeriodTimeSaveDB)
                        {
                            PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e, Item);
                        }
                        else if (tb == TBEmergencySaveBD)
                        {
                            PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e, Item);
                        }
                    }
                    else
                    {
                        PopupMessage.IsOpen = false;

                        tb.Paste();
                    }
                }
                else
                {
                    if (tb == TBTime)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = StaticValue.SRange1_86400;
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;
                    }
                    else if (tb == TBPeriodTimeSaveDB)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e, Item);
                    }
                    else if (tb == TBEmergencySaveBD)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e, Item);
                    }
                }
            }
            else if(tb == TBIPAdress1 || tb == TBIPAdress2 || tb == TBIPAdress3 || tb == TBIPAdress4)
            {
                string pattern = @"^\d{1,3}$";

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = "Диапазон буфера 0 - 255";
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > 255)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = "Диапазон буфера 0 -  " + BufferSize;
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;

                        e.Handled = true;
                    }
                    else
                    {
                        PopupMessage.IsOpen = false;
                        tb.Paste();
                    }
                }
            }
            else if (tb == TBPortClient || tb == TBPortServer)
            {
                string pattern = @"^\d{1,5}$";

                TextBox tbPort = (TextBox)sender;

                if (tbPort.SelectionLength > 0)
                {
                    s = tbPort.Text.Remove(tbPort.SelectionStart, tbPort.SelectionLength);
                    s = s.Insert(tbPort.SelectionStart, Clipboard.GetText());
                }
                else
                {
                    s = tbPort.Text.Insert(tbPort.Text.Length, Clipboard.GetText());
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SRangePort;
                    PopupMessage.PlacementTarget = tbPort;
                    PopupMessage.IsOpen = true;
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > 65535)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = StaticValue.SRangePort;
                        PopupMessage.PlacementTarget = tbPort;
                        PopupMessage.IsOpen = true;
                    }
                    else
                    {
                        PopupMessage.IsOpen = false;

                        tbPort.Paste();                        
                    }
                }
                else
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SRangePort;
                    PopupMessage.PlacementTarget = tbPort;
                    PopupMessage.IsOpen = true;
                }
            }
            else if (tb == TBBufferSizeRec || tb == TBBufferSizeSend)
            {
                string pattern = @"^\d{1,9}$";

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SRange1_102400000;
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 1 || d > 102400000)
                    {
                        PopupMessage.HorizontalOffset = 0;
                        LPopupMessage.Content = StaticValue.SRange1_102400000;
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;
                    }
                    else
                    {
                        PopupMessage.IsOpen = false;

                        tb.Paste();                        
                    }
                }
                else
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SRange1_102400000;
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;
                }
            }
            else if (tb == TBRange0 || tb == TBRange1)
            {
                string pattern = @"^\d{1," + MaxDigit + "}$";

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    if (tb == TBRange0)
                    {
                        PopupMessageShow(StaticValue.SRange0, tb, "Диапазон буфера 0 -  " + BufferSize, e, Item);
                    }
                    else if (tb == TBRange1)
                    {
                        PopupMessageShow(StaticValue.SRange1, tb, "Диапазон буфера 0 -  " + BufferSize, e, Item);
                    } 
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > BufferSize)
                    {
                        if (tb == TBRange0)
                        {
                            PopupMessageShow(StaticValue.SRange0, tb, "Диапазон буфера 0 -  " + BufferSize, e, Item);
                        }
                        else if (tb == TBRange1)
                        {
                            PopupMessageShow(StaticValue.SRange1, tb, "Диапазон буфера 0 -  " + BufferSize, e, Item);
                        }
                    }
                    else
                    {
                        PopupMessage.IsOpen = false;

                        tb.Paste();
                    }
                }
                else
                {
                    if (tb == TBRange0)
                    {
                        PopupMessageShow(StaticValue.SRange0, tb, "Диапазон буфера 0 -  " + BufferSize, e, Item);
                    }
                    else if (tb == TBRange1)
                    {
                        PopupMessageShow(StaticValue.SRange1, tb, "Диапазон буфера 0 -  " + BufferSize, e, Item);
                    }
                }
            }
            else if (tb == TBEmergencyUp || tb == TBEmergencyDown)
            {
                if (Item.TypeValue == "double")
                {
                    string pattern = @"^\d{1,15}(?:\.\d{0,12})?$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Неверный формат", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Неверный формат", e, Item);
                        }
                    }
                    else
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            double up = 0;
                            double down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                double.TryParse(s, out up);
                                down = Convert.ToDouble(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                double.TryParse(s, out down);
                                up = Convert.ToDouble(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }  
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                }
                else if (Item.TypeValue == "float")
                {
                    string pattern = @"^\d{1,8}(?:\.\d{0,6})?$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Неверный формат", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Неверный формат", e, Item);
                        }
                    }
                    else
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            float up = 0;
                            float down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                float.TryParse(s, out up);
                                down = Convert.ToSingle(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                float.TryParse(s, out down);
                                up = Convert.ToSingle(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                }
                else if (Item.TypeValue == "decimal")
                {
                    string pattern = @"^\d{1,29}(?:\.\d{0,27})?$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    decimal d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e, Item);
                        }
                    }
                    else if (decimal.TryParse(s, out d))
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            decimal up = 0;
                            decimal down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                decimal.TryParse(s, out up);
                                down = Convert.ToDecimal(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                decimal.TryParse(s, out down);
                                up = Convert.ToDecimal(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }                              
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e, Item);
                        }
                    }
                }
                else if (Item.TypeValue == "short")
                {
                    string pattern = @"^\d{1,5}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    short d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -32768-32767", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -32768-32767", e, Item);
                        }                        
                    }
                    else if (short.TryParse(s, out d))
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            short up = 0;
                            short down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                short.TryParse(s, out up);
                                down = Convert.ToInt16(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                short.TryParse(s, out down);
                                up = Convert.ToInt16(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }                               
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -32768-32767", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -32768-32767", e, Item);
                        }
                    }
                }
                else if (Item.TypeValue == "ushort")
                {
                    string pattern = @"^\d{1,5}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    ushort d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange0_65535, e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange0_65535, e, Item);
                        }                       
                    }
                    else if (ushort.TryParse(s, out d))
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            ushort up = 0;
                            ushort down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                ushort.TryParse(s, out up);
                                down = Convert.ToUInt16(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                ushort.TryParse(s, out down);
                                up = Convert.ToUInt16(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }                               
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange0_65535, e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange0_65535, e, Item);
                        }
                    }
                }
                else if (Item.TypeValue == "int")
                {
                    string pattern = @"^\d{1,10}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    int d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange_2147483648_2147483647, e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange_2147483648_2147483647, e, Item);
                        }                     
                    }
                    else if (int.TryParse(s, out d))
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            int up = 0;
                            int down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                int.TryParse(s, out up);
                                down = Convert.ToInt32(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                int.TryParse(s, out down);
                                up = Convert.ToInt32(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }                               
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SRange_2147483648_2147483647, e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SRange_2147483648_2147483647, e, Item);
                        }
                    }
                }
                else if (Item.TypeValue == "uint")
                {
                    string pattern = @"^\d{1,10}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    uint d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-4294967295", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-4294967295", e, Item);
                        }                    
                    }
                    else if (uint.TryParse(s, out d))
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            uint up = 0;
                            uint down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                uint.TryParse(s, out up);
                                down = Convert.ToUInt32(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                uint.TryParse(s, out down);
                                up = Convert.ToUInt32(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }                                  
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-4294967295", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-4294967295", e, Item);
                        }  
                    }
                }
                else if (Item.TypeValue == "long")
                {
                    string pattern = @"^\d{1,19}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    long d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -9223372036854775808-9223372036854775807", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -9223372036854775808-9223372036854775807", e, Item);
                        }  
                    }
                    else if (long.TryParse(s, out d))
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            long up = 0;
                            long down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                long.TryParse(s, out up);
                                down = Convert.ToInt64(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                long.TryParse(s, out down);
                                up = Convert.ToInt64(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }                               
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -9223372036854775808-9223372036854775807", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -9223372036854775808-9223372036854775807", e, Item);
                        } 
                    }
                }
                else if (Item.TypeValue == "ulong")
                {
                    string pattern = @"^\d{1,20}$";

                    if (tb.SelectionLength > 0)
                    {
                        s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                        s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                    }
                    else
                    {
                        s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                    }

                    ulong d = 0;

                    if (!Regex.IsMatch(s, pattern))
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-18446744073709551615", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-18446744073709551615", e, Item);
                        }                        
                    }
                    else if (ulong.TryParse(s, out d))
                    {
                        if (Item.IsEmergencyUp && Item.IsEmergencyDown)
                        {
                            ulong up = 0;
                            ulong down = 0;

                            if (tb == TBEmergencyUp)
                            {
                                ulong.TryParse(s, out up);
                                down = Convert.ToUInt64(((ItemNet)tb.DataContext).EmergencyDown);
                            }
                            else if (tb == TBEmergencyDown)
                            {
                                ulong.TryParse(s, out down);
                                up = Convert.ToUInt64(((ItemNet)tb.DataContext).EmergencyUp);
                            }

                            if (up <= down)
                            {
                                if (tb == TBEmergencyUp)
                                {
                                    PopupMessageShow(StaticValue.SSetUp, tb, StaticValue.SSetMessage, e, Item);
                                }
                                else if (tb == TBEmergencyDown)
                                {
                                    PopupMessageShow(StaticValue.SSetDown, tb, StaticValue.SSetMessage, e, Item);
                                }                                 
                            }
                            else
                            {
                                PopupMessage.IsOpen = false;

                                tb.Paste();
                            }
                        }
                        else
                        {
                            PopupMessage.IsOpen = false;

                            tb.Paste();
                        }
                    }
                    else
                    {
                        if (tb == TBEmergencyUp)
                        {
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-18446744073709551615", e, Item);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-18446744073709551615", e, Item);
                        }  
                    }
                }
            }
        }

        void BackspacePreviewTextKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (e.Key == Key.Back)
            {                
                PopupMessage.IsOpen = false;
            }
            else if (e.Key == Key.Escape)
            {                
                if (tb == TBTime || tb == TBIPAdress1 || tb == TBIPAdress2 || tb == TBIPAdress3 || tb == TBIPAdress4 || tb == TBBufferSizeRec || tb == TBBufferSizeSend || tb == TBPortClient || tb == TBPortServer)
                {
                    e.Handled = true;

                    PopupMessage.IsOpen = false;

                    tb.Text = EscDigital.ToString(CultureInfo.InvariantCulture);
                }
                else if (tb == TBDescriptionEthernet)
                {
                    e.Handled = true;

                    PopupMessage.IsOpen = false;

                    tb.Text = EscText;
                }
            }
        }

        void Text_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PopupMessage.IsOpen = false;

            TextBox tb = (TextBox)sender;

            if (tb == TBDescriptionItemNet)
            {
                if (tb.Text.Length + 1 > 200)
                {
                    if (tb.SelectionLength > 0)
                    {
                        if ((tb.Text.Length + 1) - tb.SelectionLength <= 200)
                        {
                            return;
                        }
                    }

                    PopupMessageShow(StaticValue.SDescription, tb, StaticValue.SDescriptionMessage, e, Item);
                }
                else
                {
                    PopupMessage.IsOpen = false;
                }
            }
            else if (tb == TBDescriptionEthernet)
            {
                if (tb.Text.Length + 1 > 200)
                {
                    if (tb.SelectionLength > 0)
                    {
                        if ((tb.Text.Length + 1) - tb.SelectionLength <= 200)
                        {
                            PopupMessage.IsOpen = false;
                                                 
                            return;
                        }
                    }

                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SDescriptionMessage;
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }
                else
                {
                    PopupMessage.IsOpen = false;
                }
            }
            else if (tb == TBFormula)
            {
                if (tb.Text.Length + 1 > 300)
                {
                    if (tb.SelectionLength > 0)
                    {
                        if ((tb.Text.Length + 1) - tb.SelectionLength <= 300)
                        {
                            PopupMessage.IsOpen = false;

                            return;
                        }
                    }

                    PopupMessageShow(StaticValue.SFormula, tb, StaticValue.SFormulaDescription, e, Item);
                }
                else
                {
                    PopupMessage.IsOpen = false;
                }
            }
            else if (tb == TBText)
            {
                if (tb.Text.Length + 1 > 100)
                {
                    if (tb.SelectionLength > 0)
                    {
                        if ((tb.Text.Length + 1) - tb.SelectionLength <= 100)
                        {
                            PopupMessage.IsOpen = false;

                            return;
                        }
                    }

                    PopupMessageShow(StaticValue.SText, tb, StaticValue.STextDescription, e, Item);
                }
                else
                {
                    PopupMessage.IsOpen = false;
                }
            }
            else if (tb == TBTableName)
            {
                if (tb.Text.Length + 1 > 100)
                {
                    if (tb.SelectionLength > 0)
                    {
                        if ((tb.Text.Length + 1) - tb.SelectionLength <= 100)
                        {
                            PopupMessage.IsOpen = false;

                            return;
                        }
                    }

                    PopupMessageShow(StaticValue.STableName, tb, StaticValue.STableNameDescription, e, Item);
                }
                else
                {
                    PopupMessage.IsOpen = false;
                }
            }
        }

        void PopupMessageShow(string columnName, TextBox tb, string message, RoutedEventArgs e, ItemNet item)
        {
            if (e != null)
            {
                e.Handled = true;
            }

            if (NewEthernetSer.CollectionItemNetSend.Contains(item))
            {
                tb.Tag = "S";
            }
            else
            {
                tb.Tag = "R";
            }
            
            int i = 0;
            double offset = 0;

            DataGrid dg = null;

            if (tb.Tag == "S")
            {
                dg = DGSend;
            }
            else if (tb.Tag == "R")
            {
                dg = DGRec;
            }

            foreach (DataGridColumn col in dg.Columns)
            {
                if ((string)col.Header == columnName)
                {
                    break;
                }

                i += 1;
                offset += col.Width.DisplayValue;
            }

            ScrollViewer scrollViewer = StaticValue.GetDescendantByType(dg, typeof(ScrollViewer)) as ScrollViewer;

            PopupMessage.HorizontalOffset = scrollViewer.HorizontalOffset - offset;
            PopupMessage.PlacementTarget = tb;
            LPopupMessage.Content = message;
            PopupMessage.IsOpen = true;
        }

        void TextTextBoxPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            TextBox tb = (TextBox)sender;

            string s;

            if (tb.SelectionLength > 0)
            {
                s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                s = s.Insert(tb.SelectionStart, Clipboard.GetText());
            }
            else
            {
                s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
            }

            if (tb == TBDescriptionItemNet)
            {
                if (s.Length > 200)
                {
                    PopupMessageShow(StaticValue.SDescription, tb, StaticValue.SDescriptionMessage, e, Item);
                }
                else
                {
                    tb.Paste();
                }
            }
            else if (tb == TBTableName)
            {
                if (s.Length > 100)
                {
                    PopupMessageShow(StaticValue.STableName, tb, StaticValue.STableNameDescription, e, Item);                    
                }
                else
                {
                    tb.Paste();
                }
            }
            else if (tb == TBDescriptionEthernet)
            {
                if (s.Length > 200)
                {
                    PopupMessage.HorizontalOffset = 0;
                    LPopupMessage.Content = StaticValue.SDescriptionMessage;
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;                   
                }
                else
                {
                    tb.Paste();                    
                }
            }
            else if (tb == TBFormula)
            {
                if (s.Length > 300)
                {
                    PopupMessageShow(StaticValue.SFormula, tb, StaticValue.SFormulaDescription, e, Item);
                }
                else
                {
                    tb.Paste();
                }
            }
            else if (tb == TBText)
            {
                if (s.Length > 100)
                {
                    PopupMessageShow(StaticValue.SText, tb, StaticValue.STextDescription, e, Item);
                }
                else
                {
                    tb.Paste();
                }
            }
        }

        void LoadedType(Object sender, RoutedEventArgs e)
        {
            Label l = (Label)sender;

            if (l.Content == "float")
            {
                l.ToolTip = "Число одинарной точности, 4 байта.";
            }
            else if (l.Content == "double")
            {
                l.ToolTip = "Число двойной точности, 8 байт.";
            }
            else if (l.Content == "decimal")
            {
                l.ToolTip = "Десятичное число, 12 байт.";
            }
            else if (l.Content == "byte")
            {
                l.ToolTip = "Целочисленный тип: 0-255, 1 байт.";
            }
            else if (l.Content == "sbyte")
            {
                l.ToolTip = "Целочисленный тип: -128-127, 1 байт.";
            }
            else if (l.Content == "short")
            {
                l.ToolTip = "Целочисленный тип: -32768-32767, 2 байта.";
            }
            else if (l.Content == "ushort")
            {
                l.ToolTip = "Целочисленный тип: 0-65535, 2 байта.";
            }
            else if (l.Content == "int")
            {
                l.ToolTip = "Целочисленный тип: -2147483648-2147483647, 4 байта.";
            }
            else if (l.Content == "uint")
            {
                l.ToolTip = "Целочисленный тип: 0-4294967295, 4 байта.";
            }
            else if (l.Content == "long")
            {
                l.ToolTip = "Целочисленный тип: -9223372036854775808-9223372036854775807, 8 байт.";
            }
            else if (l.Content == "ulong")
            {
                l.ToolTip = "Целочисленный тип: 0-18446744073709551615, 8 байт.";
            }
            else if (l.Content == "bool")
            {
                l.ToolTip = "Логический тип, 1 байт.";
            }
            else if (l.Content == "char")
            {
                l.ToolTip = "Символьный тип, 2 байта.";
            }
            else if (l.Content == "string")
            {
                l.ToolTip = "Строковый тип.";
            }
            else if (l.Content == "floatOperational")
            {
                l.ToolTip = "Число одинарной точности, 4 байта.";
            }
            else if (l.Content == "doubleOperational")
            {
                l.ToolTip = "Число двойной точности, 8 байт.";
            }
            else if (l.Content == "decimalOperational")
            {
                l.ToolTip = "Десятичное число, 12 байт.";
            }
            else if (l.Content == "byteOperational")
            {
                l.ToolTip = "Целочисленный тип: 0-255, 2 байта.";
            }
            else if (l.Content == "sbyteOperational")
            {
                l.ToolTip = "Целочисленный тип: -128-127, 2 байта.";
            }
            else if (l.Content == "shortOperational")
            {
                l.ToolTip = "Целочисленный тип: -32768-32767, 3 байта.";
            }
            else if (l.Content == "ushortOperational")
            {
                l.ToolTip = "Целочисленный тип: 0-65535, 3 байта.";
            }
            else if (l.Content == "intOperational")
            {
                l.ToolTip = "Целочисленный тип: -2147483648-2147483647, 5 байт.";
            }
            else if (l.Content == "uintOperational")
            {
                l.ToolTip = "Целочисленный тип: 0-4294967295, 5 байт.";
            }
            else if (l.Content == "longOperational")
            {
                l.ToolTip = "Целочисленный тип: -9223372036854775808-9223372036854775807, 9 байт.";
            }
            else if (l.Content == "ulongOperational")
            {
                l.ToolTip = "Целочисленный тип: 0-18446744073709551615, 9 байт.";
            }
            else if (l.Content == "boolOperational")
            {
                l.ToolTip = "Логический тип, 2 байта.";
            }
            else if (l.Content == "charOperational")
            {
                l.ToolTip = "Символьный тип, 3 байта.";
            }
            else if (l.Content == "stringOperational")
            {
                l.ToolTip = "Строковый тип.";
            }

            e.Handled = true;
        }

        void AddNetButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int bufferSize;

            if(!int.TryParse(TBBufferSizeRec.Text, out bufferSize))
            {
                LPopupMessage.Content = StaticValue.SRange1_102400000;
                PopupMessage.PlacementTarget = TBBufferSizeRec;
                PopupMessage.IsOpen = true;

                e.Handled = true;
            }            

            if (((ListBoxItem)ListEthernets.SelectedItem).Tag == "1")
            {                
                AddItemNetWindow addItemNetWindow = new AddItemNetWindow(bufferSize - 1, NewEthernetSer.CollectionItemNetRec, null);

                addItemNetWindow.ShowDialog();
            }
            else
            {
                EthernetOperational ethernetOperational = (EthernetOperational)((ListBoxItem)ListEthernets.SelectedItem).Tag;

                AddItemNetWindow addItemNetWindow = new AddItemNetWindow(bufferSize - 1, ethernetOperational.CollectionItemNetRec, ethernetOperational);

                addItemNetWindow.ShowDialog();
            }                        
        }

        void AddNetButtonSend_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int bufferSize;

            if (!int.TryParse(TBBufferSizeSend.Text, out bufferSize))
            {
                LPopupMessage.Content = StaticValue.SRange1_102400000;
                PopupMessage.PlacementTarget = TBBufferSizeSend;
                PopupMessage.IsOpen = true;

                e.Handled = true;
            }           
           
            if (((ListBoxItem)ListEthernets.SelectedItem).Tag == "1")
            {
                AddItemNetWindow addItemNetWindow = new AddItemNetWindow(bufferSize - 1, NewEthernetSer.CollectionItemNetSend, null);

                addItemNetWindow.ShowDialog();
            }
            else
            {
                EthernetOperational ethernetOperational = (EthernetOperational)((ListBoxItem)ListEthernets.SelectedItem).Tag;

                AddItemNetWindow addItemNetWindow = new AddItemNetWindow(bufferSize, ethernetOperational.CollectionItemNetSend, ethernetOperational);

                addItemNetWindow.ShowDialog();
            }           
        }              

        private void IgnoreCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }        
               
        void TextBoxFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb == TBTime || tb == TBIPAdress1 || tb == TBIPAdress2 || tb == TBIPAdress3 || tb == TBIPAdress4 || tb == TBBufferSizeRec || tb == TBBufferSizeSend ||  tb == TBPortClient || tb == TBPortServer)
            {
                decimal.TryParse(tb.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out EscDigital);
            }
            else if (tb == TBDescriptionEthernet)
            {
                EscText = tb.Text;
            }

            tb.SelectAll();
           
            e.Handled = true;
        }                      
    }

    class GridPropertiesComGeneral : Grid
    {
        public Popup PopupMessage = new Popup();
        public Label LPopupMessage = new Label();

        public TextBox TBDescriptionCom = new TextBox();
        public ComboBox CBPortName = new ComboBox();
        public ComboBox CBBaudRate = new ComboBox();
        public ComboBox CBParity = new ComboBox();
        public ComboBox CBDataBits = new ComboBox();
        public ComboBox CBStopBits = new ComboBox();
        public TextBox TBReadTimeout = new TextBox();
        public TextBox TBWriteTimeout = new TextBox();
        int EscDigital;
        string EscText;

        ComControl ComControl;

        public GridPropertiesComGeneral(ComControl comControl)
        {
            LPopupMessage = new Label();
            LPopupMessage.BorderThickness = new Thickness(1);
            LPopupMessage.BorderBrush = Brushes.Red;
            LPopupMessage.Background = Brushes.White;

            PopupMessage.AllowsTransparency = true;
            PopupMessage.Child = LPopupMessage;
            PopupMessage.PopupAnimation = PopupAnimation.Fade;
            PopupMessage.StaysOpen = false;

            ComControl = comControl;
                  
            if (comControl.ComSer.Description == null || comControl.ComSer.Description.Length > 150)
            {
                TBDescriptionCom.Text = "";
            }
            else
            {
                TBDescriptionCom.Text = comControl.ComSer.Description;
            }

            if(comControl.ComSer.ComPort != null)
            {
                CBPortName.Items.Add(comControl.ComSer.ComPort);               
            }

            foreach (string s in SerialPort.GetPortNames())
            {
                if (s != comControl.ComSer.ComPort)
                {
                    CBPortName.Items.Add(s);
                }
            }

            if (comControl.ComSer.ComPort != null)
            {
                CBPortName.SelectedItem = comControl.ComSer.ComPort;
            }
            
            TBDescriptionCom.MinWidth = 150;
            TBDescriptionCom.Margin = new Thickness(3);
            TBDescriptionCom.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            TBDescriptionCom.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            TBDescriptionCom.MaxWidth = 300;
            TBDescriptionCom.MaxHeight = 200;
            TBDescriptionCom.AcceptsReturn = true;

            CBPortName.MinWidth = 100;
            CBPortName.Margin = new Thickness(3);

            CBBaudRate.MinWidth = 50;
            CBBaudRate.Margin = new Thickness(3);
            CBBaudRate.Items.Add(2400);
            CBBaudRate.Items.Add(4800);
            CBBaudRate.Items.Add(9600);
            CBBaudRate.Items.Add(14400);
            CBBaudRate.Items.Add(19200);
            CBBaudRate.Items.Add(28800);
            CBBaudRate.Items.Add(38400);
            CBBaudRate.Items.Add(57600);
            CBBaudRate.Items.Add(115200);
            CBBaudRate.SelectedItem = comControl.ComSer.BaudRate;

            CBParity.MinWidth = 50;
            CBParity.Margin = new Thickness(3);
            CBParity.Items.Add("None");
            CBParity.Items.Add("Even");
            CBParity.Items.Add("Mark");
            CBParity.Items.Add("Odd");
            CBParity.Items.Add("Space");
            CBParity.SelectedItem = comControl.ComSer.Parity;

            CBDataBits.MinWidth = 50;
            CBDataBits.Margin = new Thickness(3);
            CBDataBits.Items.Add(5);
            CBDataBits.Items.Add(6);
            CBDataBits.Items.Add(7);
            CBDataBits.Items.Add(8);
            CBDataBits.SelectedItem = comControl.ComSer.DataBits;

            CBStopBits.MinWidth = 50;
            CBStopBits.Margin = new Thickness(3);
            CBStopBits.Items.Add("None");
            CBStopBits.Items.Add("One");
            CBStopBits.Items.Add("OnePointFive");
            CBStopBits.Items.Add("Two");
            CBStopBits.SelectedItem = comControl.ComSer.StopBits;

            TBReadTimeout.MinWidth = 50;
            TBReadTimeout.Margin = new Thickness(3);
            TBReadTimeout.Text = comControl.ComSer.ReadTimeout.ToString();

            TBWriteTimeout.MinWidth = 50;
            TBWriteTimeout.Margin = new Thickness(3);
            TBWriteTimeout.Text = comControl.ComSer.WriteTimeout.ToString();
            
            RowDefinition rowDescription = new RowDefinition();
            rowDescription.Height = GridLength.Auto;

            RowDefinition rowPortName = new RowDefinition();
            rowPortName.Height = GridLength.Auto;

            RowDefinition rowBaudRate = new RowDefinition();
            rowBaudRate.Height = GridLength.Auto;

            RowDefinition rowParity = new RowDefinition();
            rowParity.Height = GridLength.Auto;

            RowDefinition rowDataBits = new RowDefinition();
            rowDataBits.Height = new GridLength(30);

            RowDefinition rowStopBits = new RowDefinition();
            rowStopBits.Height = GridLength.Auto;

            RowDefinition rowReadTimeout = new RowDefinition();
            rowReadTimeout.Height = GridLength.Auto;

            RowDefinition rowWriteTimeout = new RowDefinition();
            rowWriteTimeout.Height = GridLength.Auto;
                     
            StackPanel panelDescription = new StackPanel();
            panelDescription.Orientation = Orientation.Horizontal;
            panelDescription.SetValue(Grid.RowProperty, 0);

            StackPanel panelPortName = new StackPanel();
            panelPortName.Orientation = Orientation.Horizontal;
            panelPortName.SetValue(Grid.RowProperty, 1);

            StackPanel panelBaudRate = new StackPanel();
            panelBaudRate.Orientation = Orientation.Horizontal;
            panelBaudRate.SetValue(Grid.RowProperty, 2);           

            StackPanel panelParity = new StackPanel();
            panelParity.Orientation = Orientation.Horizontal;
            panelParity.SetValue(Grid.RowProperty, 3);

            StackPanel panelDataBits = new StackPanel();
            panelDataBits.Orientation = Orientation.Horizontal;
            panelDataBits.SetValue(Grid.RowProperty, 4);

            StackPanel panelStopBits = new StackPanel();
            panelStopBits.Orientation = Orientation.Horizontal;
            panelStopBits.SetValue(Grid.RowProperty, 5);

            StackPanel panelReadTimeout = new StackPanel();
            panelReadTimeout.Orientation = Orientation.Horizontal;
            panelReadTimeout.SetValue(Grid.RowProperty, 6);

            StackPanel panelWriteTimeout = new StackPanel();
            panelWriteTimeout.Orientation = Orientation.Horizontal;
            panelWriteTimeout.SetValue(Grid.RowProperty, 7);
                        
            Label labelDescriptionCom = new Label();
            labelDescriptionCom.Content = "Описание Com-порта: ";

            Label lNamePort = new Label();
            lNamePort.Content = "Имя Com-порта: ";

            Label lBaudRate = new Label();
            lBaudRate.Content = "Скорость обмена: ";

            Label lParity = new Label();
            lParity.Content = "Четность: ";

            Label lDataBits = new Label();
            lDataBits.Content = "Длина слова данных: ";

            Label lStopBits = new Label();
            lStopBits.Content = "Количесто стоп-бит: ";

            Label lReadTimeout = new Label();
            lReadTimeout.Content = "Таймаут чтения (мс): ";

            Label lWriteTimeout = new Label();
            lWriteTimeout.Content = "Таймаут записи (мс): ";

            panelDescription.Children.Add(labelDescriptionCom);
            panelDescription.Children.Add(TBDescriptionCom);

            panelPortName.Children.Add(lNamePort);
            panelPortName.Children.Add(CBPortName);

            panelBaudRate.Children.Add(lBaudRate);
            panelBaudRate.Children.Add(CBBaudRate);

            panelParity.Children.Add(lParity);
            panelParity.Children.Add(CBParity);

            panelDataBits.Children.Add(lDataBits);
            panelDataBits.Children.Add(CBDataBits);

            panelStopBits.Children.Add(lStopBits);
            panelStopBits.Children.Add(CBStopBits);

            panelReadTimeout.Children.Add(lReadTimeout);
            panelReadTimeout.Children.Add(TBReadTimeout);

            panelWriteTimeout.Children.Add(lWriteTimeout);
            panelWriteTimeout.Children.Add(TBWriteTimeout);
           
            RowDefinitions.Add(rowDescription);
            RowDefinitions.Add(rowPortName);
            RowDefinitions.Add(rowBaudRate);
            RowDefinitions.Add(rowParity);
            RowDefinitions.Add(rowReadTimeout);
            RowDefinitions.Add(rowWriteTimeout);
            RowDefinitions.Add(rowDataBits);
            RowDefinitions.Add(rowStopBits);
           
            MenuItem menuItemPasteDescription = new MenuItem();
            menuItemPasteDescription.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyDescription = new MenuItem();
            menuItemCopyDescription.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuDescription = new System.Windows.Controls.ContextMenu();
            ContextMenuDescription.Items.Add(menuItemPasteDescription);
            ContextMenuDescription.Items.Add(menuItemCopyDescription);
            
            TBDescriptionCom.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBDescriptionCom.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DescriptionTextBoxPaste));
            TBDescriptionCom.ContextMenu = ContextMenuDescription;

            TBDescriptionCom.PreviewKeyDown += Description_PreviewKeyDown;
            TBDescriptionCom.PreviewTextInput += Description_PreviewTextInput;
            TBDescriptionCom.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBDescriptionCom.GotKeyboardFocus += TextBox_GotKeyboardFocus;

            MenuItem menuItemPasteReadTimeout = new MenuItem();
            menuItemPasteReadTimeout.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyReadTimeout = new MenuItem();
            menuItemCopyReadTimeout.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuReadTimeout = new System.Windows.Controls.ContextMenu();
            ContextMenuReadTimeout.Items.Add(menuItemPasteReadTimeout);
            ContextMenuReadTimeout.Items.Add(menuItemCopyReadTimeout);

            TBReadTimeout.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBReadTimeout.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DescriptionTextBoxPaste));
            TBReadTimeout.ContextMenu = ContextMenuReadTimeout;

            TBReadTimeout.PreviewKeyDown += Description_PreviewKeyDown;
            TBReadTimeout.PreviewTextInput += Description_PreviewTextInput;
            TBReadTimeout.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBReadTimeout.GotKeyboardFocus += TextBox_GotKeyboardFocus;

            MenuItem menuItemPasteWriteTimeout = new MenuItem();
            menuItemPasteWriteTimeout.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyWriteTimeout = new MenuItem();
            menuItemCopyWriteTimeout.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuWriteTimeout = new System.Windows.Controls.ContextMenu();
            ContextMenuWriteTimeout.Items.Add(menuItemPasteWriteTimeout);
            ContextMenuWriteTimeout.Items.Add(menuItemCopyWriteTimeout);

            TBWriteTimeout.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBWriteTimeout.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DescriptionTextBoxPaste));
            TBWriteTimeout.ContextMenu = ContextMenuWriteTimeout;

            TBWriteTimeout.PreviewKeyDown += Description_PreviewKeyDown;
            TBWriteTimeout.PreviewTextInput += Description_PreviewTextInput;
            TBWriteTimeout.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            TBWriteTimeout.GotKeyboardFocus += TextBox_GotKeyboardFocus;
           
            this.Children.Add(panelDescription);
            this.Children.Add(panelPortName);
            this.Children.Add(panelBaudRate);
            this.Children.Add(panelParity);
            this.Children.Add(panelDataBits);
            this.Children.Add(panelStopBits);
            this.Children.Add(panelReadTimeout);
            this.Children.Add(panelWriteTimeout);     
        }

        void Description_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PopupMessage.IsOpen = false;

            TextBox tb = (TextBox)sender;

            if (tb == TBDescriptionCom)
            {
                if (tb.Text.Length + 1 > 150)
                {
                    if (tb.SelectionLength > 0)
                    {
                        if ((tb.Text.Length + 1) - tb.SelectionLength <= 150)
                        {
                            return;
                        }
                    }

                    LPopupMessage.Content = "Описание не может быть длинее 150 символов.";

                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }
            }
            else if (tb == TBReadTimeout || tb == TBWriteTimeout)
            {
                string pattern = @"^\d{1,5}$";
                string s;

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, e.Text);
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, e.Text);
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    LPopupMessage.Content = "Диапазон таймаута 50 -  5000";

                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > 5000)
                    {
                        LPopupMessage.Content = "Диапазон буфера 50 -  5000";
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;

                        e.Handled = true;
                    }
                }
            }    
        }

        void Description_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (e.Key == Key.Back)
            {
                PopupMessage.IsOpen = false;
            }
            else if (e.Key == Key.Escape)
            {
                if (tb == TBReadTimeout || tb == TBWriteTimeout)
                {
                    e.Handled = true;

                    PopupMessage.IsOpen = false;

                    tb.Text = EscDigital.ToString(CultureInfo.InvariantCulture);
                }
                else if (tb == TBDescriptionCom)
                {
                    e.Handled = true;

                    PopupMessage.IsOpen = false;

                    tb.Text = EscText;
                }
            }
        }

        void DescriptionTextBoxPaste(object sender, ExecutedRoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb == TBDescriptionCom)
            {
                string s;

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                }

                if (tb == TBDescriptionCom)
                {
                    if (s.Length > 150)
                    {
                        LPopupMessage.Content = "Описание не может быть длинее 150 символов.";
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;

                        e.Handled = true;
                    }
                    else
                    {
                        PopupMessage.IsOpen = false;

                        tb.Paste();
                    }
                }
            }
            else if (tb == TBReadTimeout || tb == TBWriteTimeout)
            {
                string pattern = @"^\d{1,5}$";
                string s;

                if (tb.SelectionLength > 0)
                {
                    s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                    s = s.Insert(tb.SelectionStart, Clipboard.GetText());
                }
                else
                {
                    s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
                }

                double d = 0;

                if (!Regex.IsMatch(s, pattern))
                {
                    LPopupMessage.Content = "Диапазон буфера 50 - 5000";
                    PopupMessage.PlacementTarget = tb;
                    PopupMessage.IsOpen = true;

                    e.Handled = true;
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > 5000)
                    {
                        LPopupMessage.Content = "Диапазон буфера 50 - 5000";
                        PopupMessage.PlacementTarget = tb;
                        PopupMessage.IsOpen = true;

                        e.Handled = true;
                    }
                    else
                    {
                        PopupMessage.IsOpen = false;

                        tb.Paste();
                    }
                }
            }           
        }

        void CorrectSelectionAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tbCorrectSelectionAll = (TextBox)sender;

            if (e.ClickCount > 1 && tbCorrectSelectionAll.SelectionLength > 0)
            {
                e.Handled = true;
                tbCorrectSelectionAll.CaretIndex = tbCorrectSelectionAll.GetCharacterIndexFromPoint(Mouse.GetPosition(tbCorrectSelectionAll), true);
            }

            if (!tbCorrectSelectionAll.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                Keyboard.Focus(tbCorrectSelectionAll);
            }
        }

        void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb == TBReadTimeout || tb == TBWriteTimeout)
            {
                int.TryParse(tb.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out EscDigital);
            }
            else if (tb == TBDescriptionCom)
            {
                EscText = tb.Text;
            }

            tb.SelectAll();

            e.Handled = true;
        }

        private void IgnoreCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }
    }

    class GridPropertiesDisplayModbus : Grid
    {
        ListBox lbModbus;
        ListBox lbModbusItems;

        int CorrectTime;
        public TextBox tbPeriodTime;

        public ItemModbus ItemModbusSearch;
        public Display Display;
        public string ModbusSearch;

        public GridPropertiesDisplayModbus(Display display)
        {
            Display = display;

            ColumnDefinition columnModbus = new ColumnDefinition();
            columnModbus.Width = GridLength.Auto;

            ColumnDefinition columnModbusItems = new ColumnDefinition();
            columnModbusItems.Width = GridLength.Auto;

            RowDefinition row0 = new RowDefinition();
            row0.Height = GridLength.Auto;

            RowDefinition row1 = new RowDefinition();
            row1.Height = GridLength.Auto;

            RowDefinition rowPeriodTime = new RowDefinition();
            rowPeriodTime.Height = GridLength.Auto;

            ObservableCollection<ModbusSer> collectionModbusSers = ((AppWPF)Application.Current).CollectionModbusSers;

            Label lModbusSers = new Label();
            lModbusSers.Content = "Список Modbus.";

            Label lModbusItems = new Label();
            lModbusItems.Content = "Список объектов Modbus.";
            lModbusItems.SetValue(Grid.ColumnProperty, 1);

            Label lPeriodTime = new Label();
            lPeriodTime.Content = "Время опроса мс: ";

            MenuItem menuItemPasteTime = new MenuItem();
            menuItemPasteTime.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyTime = new MenuItem();
            menuItemCopyTime.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuTime = new System.Windows.Controls.ContextMenu();
            ContextMenuTime.Items.Add(menuItemPasteTime);
            ContextMenuTime.Items.Add(menuItemCopyTime);

            tbPeriodTime = new TextBox();
            tbPeriodTime.MinWidth = 50;
            tbPeriodTime.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            tbPeriodTime.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TimeTextBoxPaste));
            tbPeriodTime.ContextMenu = ContextMenuTime;

            tbPeriodTime.PreviewTextInput += TimeTextBox_PreviewTextInput;
            tbPeriodTime.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            tbPeriodTime.GotKeyboardFocus += TimeTextBox_GotKeyboardFocus;
            tbPeriodTime.PreviewKeyDown += TimePreviewKeyDown;

            Binding bindingDescription = new Binding();
            bindingDescription.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingDescription.Path = new PropertyPath("Description");

            Binding bindingSlaveAddress = new Binding();
            bindingSlaveAddress.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingSlaveAddress.Path = new PropertyPath("SlaveAddress");

            Binding bindingComPort = new Binding();
            bindingComPort.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingComPort.Path = new PropertyPath("ComPort");

            MultiBinding bindingDescriptionIPPort = new MultiBinding();
            bindingDescriptionIPPort.Converter = new EthernetSerSlaveAddressPortConverter();
            bindingDescriptionIPPort.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingDescriptionIPPort.Bindings.Add(bindingDescription);
            bindingDescriptionIPPort.Bindings.Add(bindingSlaveAddress);
            bindingDescriptionIPPort.Bindings.Add(bindingComPort);

            FrameworkElementFactory lEthernetIPPort = new FrameworkElementFactory(typeof(Label));
            lEthernetIPPort.SetBinding(Label.ContentProperty, bindingDescriptionIPPort);

            DataTemplate dataTemplateEthernetSer = new DataTemplate();
            dataTemplateEthernetSer.VisualTree = lEthernetIPPort;

            lbModbus = new ListBox();
            lbModbus.MinWidth = 200;
            lbModbus.SelectionChanged += lbModbus_SelectionChanged;
            lbModbus.MaxWidth = 300;
            lbModbus.MinHeight = 400;
            lbModbus.MaxHeight = 500;
            lbModbus.ItemTemplate = dataTemplateEthernetSer;
            lbModbus.ItemsSource = collectionModbusSers;
            lbModbus.SetValue(Grid.ColumnProperty, 0);
            lbModbus.SetValue(Grid.RowProperty, 1);

            Binding isBinding = new Binding();
            isBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            isBinding.Path = new PropertyPath("IsBinding");

            FrameworkElementFactory checkBoxModbusItem = new FrameworkElementFactory(typeof(CheckBox));
            checkBoxModbusItem.SetBinding(CheckBox.IsCheckedProperty, isBinding);
            checkBoxModbusItem.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(Checked));
            checkBoxModbusItem.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(Unchecked));

            Binding bindingValueType = new Binding();
            bindingValueType.Path = new PropertyPath("TypeValue");

            Binding bindingAddress = new Binding();
            bindingAddress.Path = new PropertyPath("Address");
           
            Binding bindingDescriptionItemNet = new Binding();
            bindingDescriptionItemNet.Path = new PropertyPath("Description");

            FrameworkElementFactory lValueType = new FrameworkElementFactory(typeof(Label));
            lValueType.SetBinding(Label.ContentProperty, bindingValueType);

            FrameworkElementFactory lAddress = new FrameworkElementFactory(typeof(Label));
            lAddress.SetBinding(Label.ContentProperty, bindingAddress);

            FrameworkElementFactory lDescription = new FrameworkElementFactory(typeof(Label));
            lDescription.SetBinding(Label.ContentProperty, bindingDescriptionItemNet);

            FrameworkElementFactory panelModbusItem = new FrameworkElementFactory(typeof(StackPanel));
            panelModbusItem.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            panelModbusItem.AppendChild(checkBoxModbusItem);
            panelModbusItem.AppendChild(lValueType);
            panelModbusItem.AppendChild(lAddress);
            panelModbusItem.AppendChild(lDescription);

            Binding bindingModbusSelected = new Binding();
            bindingModbusSelected.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingModbusSelected.Source = lbModbus;
            bindingModbusSelected.Path = new PropertyPath("SelectedItem.CollectionItemModbus");

            DataTemplate dataTemplateItemModbus = new DataTemplate();
            dataTemplateItemModbus.VisualTree = panelModbusItem;

            lbModbusItems = new ListBox();
            lbModbusItems.ItemTemplate = dataTemplateItemModbus;
            lbModbusItems.SetBinding(ListBox.ItemsSourceProperty, bindingModbusSelected);
            lbModbusItems.MinWidth = 300;
            lbModbusItems.MaxWidth = 400;
            lbModbusItems.MinHeight = 400;
            lbModbusItems.MaxHeight = 500;
            lbModbusItems.SetValue(Grid.ColumnProperty, 1);
            lbModbusItems.SetValue(Grid.RowProperty, 1);

            this.ColumnDefinitions.Add(columnModbus);
            this.ColumnDefinitions.Add(columnModbusItems);

            this.RowDefinitions.Add(row0);
            this.RowDefinitions.Add(row1);
            this.RowDefinitions.Add(rowPeriodTime);

            this.Children.Add(lbModbus);
            this.Children.Add(lbModbusItems);
            this.Children.Add(lModbusSers);
            this.Children.Add(lModbusItems);

            if (display.DisplaySer.PeriodTime < 1000 || display.DisplaySer.PeriodTime > 86400000)
            {
                CorrectTime = 1000;
                tbPeriodTime.Text = CorrectTime.ToString();
            }
            else
            {
                tbPeriodTime.Text = display.DisplaySer.PeriodTime.ToString();
            }

            foreach (ModbusSer modbusSer in ((AppWPF)Application.Current).CollectionModbusSers)
            {
                if (modbusSer.ID == display.DisplaySer.ModbusSearch)
                {
                    lbModbus.SelectedItem = modbusSer;
                }

                foreach (ItemModbus itemModbus in modbusSer.CollectionItemModbus)
                {
                    if (System.Object.ReferenceEquals(display.DisplaySer.ItemModbusSearch, itemModbus))
                    {
                        itemModbus.IsBinding = true;

                        lbModbusItems.SelectedItem = itemModbus;
                    }
                    else
                    {
                        if (itemModbus.IsBinding)
                        {
                            itemModbus.IsBinding = false;
                        }
                    }
                }
            }
        }

        void lbModbus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                foreach (ItemModbus itemModbus in ((ModbusSer)e.AddedItems[0]).CollectionItemModbus)
                {
                    if (System.Object.ReferenceEquals(Display.DisplaySer.ItemModbusSearch, itemModbus))
                    {
                        lbModbusItems.SelectedItem = itemModbus;

                        break;
                    }
                    else
                    {
                        lbModbusItems.SelectedItem = null;
                    }
                }
            }

            e.Handled = true;
        }

        void Checked(Object sender, RoutedEventArgs e)
        {
            ItemModbusSearch = (ItemModbus)((CheckBox)sender).DataContext;

            ModbusSer modbusSerBinding = (ModbusSer)lbModbus.SelectedItem;

            ModbusSearch = modbusSerBinding.ID;

            foreach (ModbusSer modbusSer in lbModbus.Items)
            {
                foreach (ItemModbus itemModbus in modbusSer.CollectionItemModbus)
                {
                    if (!System.Object.ReferenceEquals(ItemModbusSearch, itemModbus))
                    {
                        itemModbus.IsBinding = false;
                    }
                }
            }

            ItemModbusSearch.IsBinding = true;

            e.Handled = true;
        }

        void Unchecked(Object sender, RoutedEventArgs e)
        {
            ItemModbus uncheckedItemModbus = (ItemModbus)((CheckBox)sender).DataContext;

            if (System.Object.ReferenceEquals(ItemModbusSearch, uncheckedItemModbus))
            {
                ItemModbusSearch = null;
                ModbusSearch = null;
            }

            e.Handled = true;
        }

        void CorrectSelectionAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tbCorrectSelectionAll = (TextBox)sender;

            if (e.ClickCount > 1 && tbCorrectSelectionAll.SelectionLength > 0)
            {
                e.Handled = true;
                tbCorrectSelectionAll.CaretIndex = tbCorrectSelectionAll.GetCharacterIndexFromPoint(Mouse.GetPosition(tbCorrectSelectionAll), true);
            }

            if (!tbCorrectSelectionAll.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                Keyboard.Focus(tbCorrectSelectionAll);
            }
        }

        private void IgnoreCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        #region TimeTextBox
        private void TimeTextBoxPaste(object sender, ExecutedRoutedEventArgs e)
        {
            string pattern = @"^\d{4,8}$";
            string s;

            if (tbPeriodTime.SelectionLength > 0)
            {
                s = tbPeriodTime.Text.Remove(tbPeriodTime.SelectionStart, tbPeriodTime.SelectionLength);
                s = s.Insert(tbPeriodTime.SelectionStart, Clipboard.GetText());
            }
            else
            {
                s = tbPeriodTime.Text.Insert(tbPeriodTime.Text.Length, Clipboard.GetText());
            }

            double d = 0;

            if (!Regex.IsMatch(s, pattern))
            {
                e.Handled = true;
            }
            else if (double.TryParse(s, out d))
            {
                if (d < 1000 || d > 86400000)
                {
                    e.Handled = true;
                }
                else
                {
                    tbPeriodTime.Paste();
                }
            }
        }

        void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string pattern = @"^\d{4,8}$";
            string s;

            if (tbPeriodTime.SelectionLength > 0)
            {
                s = tbPeriodTime.Text.Remove(tbPeriodTime.SelectionStart, tbPeriodTime.SelectionLength);
                s = s.Insert(tbPeriodTime.SelectionStart, e.Text);
            }
            else
            {
                s = tbPeriodTime.Text.Insert(tbPeriodTime.Text.Length, e.Text);
            }

            double d = 0;

            if (!Regex.IsMatch(s, pattern))
            {
                e.Handled = true;
            }

            if (double.TryParse(s, out d))
            {
                if (d < 1 || d > 86400)
                {
                    e.Handled = true;
                }
            }
        }

        private void TimePreviewKeyDown(object sender, RoutedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Escape))
            {
                e.Handled = true;

                tbPeriodTime.Text = CorrectTime.ToString();
            }
            else if (Keyboard.IsKeyDown(Key.Enter))
            {
                e.Handled = true;

                double d;

                if (double.TryParse(tbPeriodTime.Text, out d))
                {
                    if (d > 86400)
                    {
                        tbPeriodTime.Text = CorrectTime.ToString();

                        MessageBox.Show("Время опроса не может быть больше 86400 секунд", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                        return;
                    }
                    else if (d < 1)
                    {
                        tbPeriodTime.Text = CorrectTime.ToString();

                        MessageBox.Show("Время опроса не может быть меньше 1 секунды", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                        return;
                    }
                }
                else
                {
                    tbPeriodTime.Text = CorrectTime.ToString();

                    MessageBox.Show("Неверный формат числа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }
            }
        }

        void TimeTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            int d;

            if (int.TryParse(tbPeriodTime.Text, out d))
            {
                if (d < 1000 || d > 86400000)
                {
                    CorrectTime = 5;

                    tbPeriodTime.Text = "1000";
                }
                else
                {
                    CorrectTime = d;
                }
            }
            else
            {
                CorrectTime = 1000;

                tbPeriodTime.Text = "1000";
            }

            tbPeriodTime.SelectAll();

            e.Handled = true;
        }
        #endregion
    }

    class GridPropertiesDisplayEthernet : Grid
    {
        public TVItemNet TVItemNet;

        int CorrectTime;
        public TextBox tbPeriodTime;

        public Display Display;

        public GridPropertiesDisplayEthernet(Display display)
        {
            Display = display;

            ColumnDefinition columnEthernets = new ColumnDefinition();
            columnEthernets.Width = GridLength.Auto;
                   
            ColumnDefinition columnEthernetItems = new ColumnDefinition();
            columnEthernetItems.Width = GridLength.Auto;

            RowDefinition row0 = new RowDefinition();
            row0.Height = GridLength.Auto;

            RowDefinition row1 = new RowDefinition();
            row1.Height = GridLength.Auto;

            RowDefinition rowPeriodTime = new RowDefinition();
            rowPeriodTime.Height = GridLength.Auto;

            ObservableCollection<EthernetSer> collectionEthernetSers = ((AppWPF)Application.Current).CollectionEthernetSers;
            ObservableCollection<ModbusSer> collectionModbusSers = ((AppWPF)Application.Current).CollectionModbusSers;

            Label lEthernetSers = new Label();
            lEthernetSers.Content = "Список Ethernet интерфейсов.";

            Label lEthernetItems = new Label();
            lEthernetItems.Content = "Список объектов интерфеса.";
            lEthernetItems.SetValue(Grid.ColumnProperty, 1);

            Label lPeriodTime = new Label();
            lPeriodTime.Content = "Время опроса мс: ";

            MenuItem menuItemPasteTime = new MenuItem();
            menuItemPasteTime.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyTime = new MenuItem();
            menuItemCopyTime.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuTime = new System.Windows.Controls.ContextMenu();
            ContextMenuTime.Items.Add(menuItemPasteTime);
            ContextMenuTime.Items.Add(menuItemCopyTime);

            tbPeriodTime = new TextBox();
            tbPeriodTime.MinWidth = 50;
            tbPeriodTime.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            tbPeriodTime.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TimeTextBoxPaste));
            tbPeriodTime.ContextMenu = ContextMenuTime;

            tbPeriodTime.PreviewTextInput += TimeTextBox_PreviewTextInput;
            tbPeriodTime.PreviewMouseDown += CorrectSelectionAll_PreviewMouseDown;
            tbPeriodTime.GotKeyboardFocus += TimeTextBox_GotKeyboardFocus;
            tbPeriodTime.PreviewKeyDown += TimePreviewKeyDown;

            TVItemNet = new SCADA.TVItemNet(Display.DisplaySer);

            TVItemNet.TVEthernets = new SCADA.TVEthernets();
            TVItemNet.TVEthernets.TVEthernet.MinWidth = 200;
            TVItemNet.TVEthernets.TVEthernet.MaxWidth = 300;
            TVItemNet.TVEthernets.TVEthernet.MinHeight = 400;
            TVItemNet.TVEthernets.TVEthernet.MaxHeight = 500;
            TVItemNet.TVEthernets.TVEthernet.ItemsSource = collectionEthernetSers;
            TVItemNet.TVEthernets.SetValue(Grid.ColumnProperty, 0);
            TVItemNet.TVEthernets.SetValue(Grid.RowProperty, 1);
            
            Binding bindingEthernetSelected = new Binding();
            bindingEthernetSelected.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingEthernetSelected.Source = TVItemNet.TVEthernets.TVEthernet;
            bindingEthernetSelected.Path = new PropertyPath("SelectedItem");

            TVItemNet.TVItemNets.SetBinding(TreeViewItem.DataContextProperty, bindingEthernetSelected);
            TVItemNet.MinWidth = 300;
            TVItemNet.MaxWidth = 400;
            TVItemNet.MinHeight = 400;
            TVItemNet.MaxHeight = 500;
            TVItemNet.SetValue(Grid.ColumnProperty, 1);
            TVItemNet.SetValue(Grid.RowProperty, 1);

            this.ColumnDefinitions.Add(columnEthernets);
            this.ColumnDefinitions.Add(columnEthernetItems);

            this.RowDefinitions.Add(row0);
            this.RowDefinitions.Add(row1);
            this.RowDefinitions.Add(rowPeriodTime);

            this.Children.Add(TVItemNet.TVEthernets);
            this.Children.Add(TVItemNet);
            this.Children.Add(lEthernetSers);
            this.Children.Add(lEthernetItems);
            
            tbPeriodTime.Text = display.DisplaySer.PeriodTime.ToString();           

            this.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
            {
                if (display.DisplaySer.EthernetSearch != null)
                {
                    foreach (EthernetSer ethernetSer in ((AppWPF)Application.Current).CollectionEthernetSers)
                    {
                        if (ethernetSer.ID == display.DisplaySer.EthernetSearch)
                        {
                            foreach (var item in TVItemNet.TVEthernets.TVEthernet.Items)
                            {
                                if (item == ethernetSer)
                                {
                                    var treeviewItem = TVItemNet.TVEthernets.TVEthernet.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                                    treeviewItem.IsSelected = true;

                                    if (display.DisplaySer.EthernetOperationalSearch != null)
                                    {
                                        TreeViewItem treeviewItemOperational = null;

                                        foreach (EthernetOperational ethernetOperational in ethernetSer.CollectionEthernetOperational)
                                        {
                                            if (ethernetOperational.EthernetOperationalSearch == display.DisplaySer.EthernetOperationalSearch)
                                            {
                                                foreach (var itemOperational in treeviewItem.Items)
                                                {
                                                    if (itemOperational == ethernetOperational)
                                                    {
                                                        treeviewItem.IsExpanded = true;

                                                        this.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
                                                        {
                                                            treeviewItemOperational = treeviewItem.ItemContainerGenerator.ContainerFromItem(itemOperational) as TreeViewItem;

                                                            treeviewItemOperational.IsSelected = true;

                                                        }));

                                                        break;
                                                    }
                                                }
                                            }

                                            if (treeviewItemOperational != null)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    break;
                                }
                            }

                            this.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
                            {
                                TreeViewItem tvItemNet2;

                                foreach (TreeViewItem tvItemNet in TVItemNet.TVItemNets.Items)
                                {
                                    foreach (ItemNet itemNet in tvItemNet.Items)
                                    {
                                        if (System.Object.ReferenceEquals(display.DisplaySer.ItemNetSearch, itemNet))
                                        {
                                            tvItemNet.IsExpanded = true;

                                            this.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
                                            {
                                                tvItemNet2 = tvItemNet.ItemContainerGenerator.ContainerFromItem(itemNet) as TreeViewItem;
                                                tvItemNet2.IsSelected = true;
                                                tvItemNet2.IsExpanded = true;
                                            }));

                                            itemNet.IsBinding = true;
                                        }
                                        else
                                        {
                                            itemNet.IsBinding = false;
                                        }
                                    }
                                }
                            }));
                        }
                        else
                        {
                            foreach (EthernetOperational ethernetOperational in ethernetSer.CollectionEthernetOperational)
                            {
                                foreach (ItemNet itemNet in ethernetOperational.CollectionItemNetRec)
                                {
                                    itemNet.IsBinding = false;
                                }

                                foreach (ItemNet itemNet in ethernetOperational.CollectionItemNetSend)
                                {
                                    itemNet.IsBinding = false;
                                }
                            }

                            foreach (ItemNet itemNet in ethernetSer.CollectionItemNetRec)
                            {
                                itemNet.IsBinding = false;
                            }

                            foreach (ItemNet itemNet in ethernetSer.CollectionItemNetSend)
                            {
                                itemNet.IsBinding = false;
                            }
                        }
                    }
                }
                else
                {
                    foreach (EthernetSer ethernetSer in ((AppWPF)Application.Current).CollectionEthernetSers)
                    {
                        foreach (EthernetOperational ethernetOperational in ethernetSer.CollectionEthernetOperational)
                        {
                            foreach (ItemNet itemNet in ethernetOperational.CollectionItemNetRec)
                            {
                                itemNet.IsBinding = false;
                            }

                            foreach (ItemNet itemNet in ethernetOperational.CollectionItemNetSend)
                            {
                                itemNet.IsBinding = false;
                            }
                        }

                        foreach (ItemNet itemNet in ethernetSer.CollectionItemNetRec)
                        {
                            itemNet.IsBinding = false;
                        }

                        foreach (ItemNet itemNet in ethernetSer.CollectionItemNetSend)
                        {
                            itemNet.IsBinding = false;
                        }
                    }
                }
            }));                     
        }
                               
        void CorrectSelectionAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tbCorrectSelectionAll = (TextBox)sender;

            if (e.ClickCount > 1 && tbCorrectSelectionAll.SelectionLength > 0)
            {
                e.Handled = true;
                tbCorrectSelectionAll.CaretIndex = tbCorrectSelectionAll.GetCharacterIndexFromPoint(Mouse.GetPosition(tbCorrectSelectionAll), true);
            }

            if (!tbCorrectSelectionAll.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                Keyboard.Focus(tbCorrectSelectionAll);
            }
        }

        private void IgnoreCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        #region TimeTextBox
        private void TimeTextBoxPaste(object sender, ExecutedRoutedEventArgs e)
        {
            string pattern = @"^\d{4,8}$";
            string s;

            if (tbPeriodTime.SelectionLength > 0)
            {
                s = tbPeriodTime.Text.Remove(tbPeriodTime.SelectionStart, tbPeriodTime.SelectionLength);
                s = s.Insert(tbPeriodTime.SelectionStart, Clipboard.GetText());
            }
            else
            {
                s = tbPeriodTime.Text.Insert(tbPeriodTime.Text.Length, Clipboard.GetText());
            }

            double d = 0;

            if (!Regex.IsMatch(s, pattern))
            {
                e.Handled = true;
            }
            else if (double.TryParse(s, out d))
            {
                if (d < 1000 || d > 86400000)
                {
                    e.Handled = true;
                }
                else
                {
                    tbPeriodTime.Paste();
                }
            }
        }

        void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string pattern = @"^\d{4,8}$";
            string s;

            if (tbPeriodTime.SelectionLength > 0)
            {
                s = tbPeriodTime.Text.Remove(tbPeriodTime.SelectionStart, tbPeriodTime.SelectionLength);
                s = s.Insert(tbPeriodTime.SelectionStart, e.Text);
            }
            else
            {
                s = tbPeriodTime.Text.Insert(tbPeriodTime.Text.Length, e.Text);
            }

            double d = 0;

            if (!Regex.IsMatch(s, pattern))
            {
                e.Handled = true;
            }

            if (double.TryParse(s, out d))
            {
                if (d < 1 || d > 86400)
                {
                    e.Handled = true;
                }
            }
        }

        private void TimePreviewKeyDown(object sender, RoutedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Escape))
            {
                e.Handled = true;

                tbPeriodTime.Text = CorrectTime.ToString();
            }
            else if (Keyboard.IsKeyDown(Key.Enter))
            {
                e.Handled = true;

                double d;

                if (double.TryParse(tbPeriodTime.Text, out d))
                {
                    if (d > 86400)
                    {
                        tbPeriodTime.Text = CorrectTime.ToString();

                        MessageBox.Show("Время опроса не может быть больше 86400 секунд", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                        return;
                    }
                    else if (d < 1)
                    {
                        tbPeriodTime.Text = CorrectTime.ToString();

                        MessageBox.Show("Время опроса не может быть меньше 1 секунды", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                        return;
                    }
                }
                else
                {
                    tbPeriodTime.Text = CorrectTime.ToString();

                    MessageBox.Show("Неверный формат числа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }
            }
        }

        void TimeTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            int d;

            if (int.TryParse(tbPeriodTime.Text, out d))
            {
                if (d < 1000 || d > 86400000)
                {
                    CorrectTime = 5;

                    tbPeriodTime.Text = "1000";
                }
                else
                {
                    CorrectTime = d;
                }
            }
            else
            {
                CorrectTime = 1000;

                tbPeriodTime.Text = "1000";
            }

            tbPeriodTime.SelectAll();

            e.Handled = true;
        }
        #endregion
    }

    public class GridPropertiesImageGeneral : Grid
    {
        public TextBox TBPathImage;
        public ComboBox CBStretch;
        public CheckBox CHBImageToFull;
        public Brush NewBrush;
        public ComboBox CBLibrary;
        public Image Img;
        public RadioButton RBPathImage;
        public RadioButton RBLibraryImage;
        Button BPath;
        ImageSer ImageSer;

        public GridPropertiesImageGeneral(ImageControl imageControl)
        {
            ImageSer = imageControl.ImageSer;

            ColumnDefinition column0 = new ColumnDefinition();
            column0.Width = GridLength.Auto;

            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = GridLength.Auto;

            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = GridLength.Auto;

            RowDefinition row0 = new RowDefinition();
            row0.Height = GridLength.Auto;

            RowDefinition row1 = new RowDefinition();
            row1.Height = GridLength.Auto;

            RowDefinition row2 = new RowDefinition();
            row2.Height = GridLength.Auto;

            RowDefinition row3 = new RowDefinition();
            row3.Height = GridLength.Auto;

            RowDefinition row4 = new RowDefinition();
            row4.Height = GridLength.Auto;

            RowDefinition row5 = new RowDefinition();
            row5.Height = GridLength.Auto;

            RowDefinition row6 = new RowDefinition();
            row6.Height = GridLength.Auto;

            RowDefinition row7 = new RowDefinition();
            row7.Height = GridLength.Auto;
                        
            Label lImageLibrary = new Label();
            lImageLibrary.SetValue(Grid.RowProperty, 4);
            lImageLibrary.Content = "Библиотека изображений: ";

            Label lImage = new Label();
            lImage.SetValue(Grid.RowProperty, 3);
            lImage.Content = "Представление изображения: ";

            Label lColorBorder = new Label();
            lColorBorder.Content = "Цвет рамки: ";
            lColorBorder.SetValue(Grid.RowProperty, 7);

            Label lImageToFull = new Label();
            lImageToFull.Content = "Подогнать размер: ";
            lImageToFull.SetValue(Grid.RowProperty, 6);           

            Label lImageStretch = new Label();
            lImageStretch.Content = "Заполнение изображения: ";
            lImageStretch.SetValue(Grid.RowProperty, 5);

            Img = new Image();
            Img.SetValue(Grid.RowProperty, 4);
            Img.SetValue(Grid.ColumnProperty, 1);
            Img.SetValue(Grid.RowProperty, 3);
            Img.SetValue(Grid.ColumnProperty, 1);

            String[] collectionLibrary = new string[] { "Дымосос", "Насос", "Клапан", "Отсекатель", "Котел Е-1_9", "Котел КВ-ГМ-30" };

            CBLibrary = new ComboBox();
            CBLibrary.SelectionChanged += CBLibrary_SelectionChanged;
            CBLibrary.MinWidth = 100;
            CBLibrary.MaxWidth = 300;
            CBLibrary.ItemsSource = collectionLibrary;
            CBLibrary.SetValue(Grid.RowProperty, 4);
            CBLibrary.SetValue(Grid.ColumnProperty, 1);           

            Image imageColor = new Image();
            imageColor.Source = new BitmapImage(new Uri("Images/FontColor24.png", UriKind.Relative));

            Xceed.Wpf.Toolkit.DropDownButton fontColorConteiner = new Xceed.Wpf.Toolkit.DropDownButton();
            fontColorConteiner.SetValue(Grid.RowProperty, 7);
            fontColorConteiner.SetValue(Grid.ColumnProperty, 1);

            Rectangle rectangleColor = new Rectangle();
            rectangleColor.Width = 24;
            rectangleColor.Height = 5;
            rectangleColor.Fill = imageControl.ImageSer.ColorBorder;

            Border fontColor = new Border();
            fontColor.BorderThickness = new Thickness(0);
            fontColor.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            fontColor.ToolTip = "Цвет шрифта (Ctrl + J)";
            fontColor.Child = imageColor;

            StackPanel panelColor = new StackPanel();
            panelColor.Children.Add(fontColor);
            panelColor.Children.Add(rectangleColor);

            fontColorConteiner.Content = panelColor;
            fontColorConteiner.DropDownContent = new ColorBrushPickerBorder(fontColorConteiner, this, rectangleColor);

            CHBImageToFull = new CheckBox();
            CHBImageToFull.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            CHBImageToFull.SetValue(Grid.RowProperty, 6);
            CHBImageToFull.SetValue(Grid.ColumnProperty, 1);

            ComboBoxItem cbItemNone = new ComboBoxItem();
            cbItemNone.Content = "None";
            cbItemNone.ToolTip = "Фигура не растягивается";

            ComboBoxItem cbItemFill = new ComboBoxItem();
            cbItemFill.Content = "Fill";
            cbItemFill.ToolTip = "Фигура растягивается по ширине и высоте для полного заполнения контейнера";

            ComboBoxItem cbItemUniformToFill = new ComboBoxItem();
            cbItemUniformToFill.Content = "UniformToFill";
            cbItemUniformToFill.ToolTip = "Ширина и высота устанавливаются пропорционально, пока фигура не заполнит всю доступную высоту и ширину";

            ComboBoxItem cbItemUniform = new ComboBoxItem();
            cbItemUniform.Content = "Uniform";
            cbItemUniform.ToolTip = "Ширина и высота устанавливаются пропорционально, пока фигура не достигнет границ контейнера";

            CBStretch = new ComboBox();
            CBStretch.MinWidth = 100;
            CBStretch.ItemsSource = new ComboBoxItem[] { cbItemNone, cbItemFill, cbItemUniformToFill, cbItemUniform };
            CBStretch.SelectedValuePath = "Content";
            CBStretch.SelectedValue = imageControl.ImageSer.StretchImage;
            CBStretch.SetValue(Grid.RowProperty, 5);
            CBStretch.SetValue(Grid.ColumnProperty, 1);

            Label lPathImage = new Label();
            lPathImage.SetValue(Grid.ColumnProperty, 1);
            lPathImage.Content = "Путь к изображению: ";

            TBPathImage = new TextBox();
            TBPathImage.SetValue(Grid.RowProperty, 1);
            TBPathImage.MinWidth = 300;
            TBPathImage.SetValue(Grid.ColumnProperty, 1);
            TBPathImage.IsReadOnly = true;
            TBPathImage.Text = imageControl.ImageSer.PathImage;

            BPath = new Button();
            BPath.SetValue(Grid.RowProperty, 1);
            BPath.Content = "Путь...";
            BPath.SetValue(Grid.ColumnProperty, 2);
            BPath.Click += bPath_Click;

            RBPathImage = new RadioButton();
            RBPathImage.GroupName = "Image";
            RBPathImage.SetValue(Grid.RowProperty, 0);
            RBPathImage.Content = "Использовать путь к изображению";
            RBPathImage.Checked += rbPathImage_Checked;

            RBLibraryImage = new RadioButton();
            RBLibraryImage.GroupName = "Image";
            RBLibraryImage.SetValue(Grid.RowProperty, 2);
            RBLibraryImage.Content = "Использовать библиотеку изображений";
            RBLibraryImage.Checked += rbLibraryImage_Checked;

            if (imageControl.ImageSer.IsPathImage)
            {
                RBPathImage.IsChecked = true;                                            
            }
            else
            {
                CBLibrary.SelectedItem = ImageSer.LibraryImage;

                RBLibraryImage.IsChecked = true;                
            }

            ColumnDefinitions.Add(column0);
            ColumnDefinitions.Add(column1);
            ColumnDefinitions.Add(column2);

            RowDefinitions.Add(row0);
            RowDefinitions.Add(row1);
            RowDefinitions.Add(row2);
            RowDefinitions.Add(row3);
            RowDefinitions.Add(row4);
            RowDefinitions.Add(row5);
            RowDefinitions.Add(row6);
            RowDefinitions.Add(row7);

            this.Children.Add(lImageStretch);
            this.Children.Add(CBStretch);
            this.Children.Add(lPathImage);
            this.Children.Add(TBPathImage);
            this.Children.Add(BPath);
            this.Children.Add(lImageToFull);
            this.Children.Add(CHBImageToFull);
            this.Children.Add(fontColorConteiner);
            this.Children.Add(lColorBorder);
            this.Children.Add(RBPathImage);
            this.Children.Add(RBLibraryImage);
            this.Children.Add(lImageLibrary);
            this.Children.Add(lImage);
            this.Children.Add(Img);
            this.Children.Add(CBLibrary); 
        }

        void CBLibrary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                string s = (string)e.AddedItems[0];

                if (s == "Дымосос")
                {
                    BitmapImage bi = new BitmapImage(new Uri("Images/Дымосос.png", UriKind.RelativeOrAbsolute));

                    Img.Source = bi;
                }
                else if (s == "Насос")
                {
                    BitmapImage bi = new BitmapImage(new Uri("Images/Насос.png", UriKind.RelativeOrAbsolute));

                    Img.Source = bi;
                }
                else if (s == "Клапан")
                {
                    BitmapImage bi = new BitmapImage(new Uri("Images/Клапан.png", UriKind.RelativeOrAbsolute));

                    Img.Source = bi;
                }
                else if (s == "Отсекатель")
                {
                    BitmapImage bi = new BitmapImage(new Uri("Images/Отсекатель.png", UriKind.RelativeOrAbsolute));

                    Img.Source = bi;
                }
                else if (s == "Котел Е-1_9")
                {
                    BitmapImage bi = new BitmapImage(new Uri("Images/Котел Е-1_9.png", UriKind.RelativeOrAbsolute));

                    Img.Source = bi;
                }
                else if (s == "Котел КВ-ГМ-30")
                {
                    BitmapImage bi = new BitmapImage(new Uri("Images/Котел КВ-ГМ-30.png", UriKind.RelativeOrAbsolute));

                    Img.Source = bi;
                }
            }
           
            e.Handled = true;
        }        

        void rbLibraryImage_Checked(object sender, RoutedEventArgs e)
        {
            string s = (string)CBLibrary.SelectedItem;

            BPath.IsEnabled = false;
            BPath.Opacity = 30;

            TBPathImage.IsEnabled = false;

            CBLibrary.IsEnabled = true;
            CBLibrary.SelectedItem = null;
            CBLibrary.SelectedItem = s;

            e.Handled = true;
        }

        void rbPathImage_Checked(object sender, RoutedEventArgs e)
        {
            BPath.IsEnabled = true;
            BPath.Opacity = 100;

            TBPathImage.IsEnabled = true;

            CBLibrary.IsEnabled = false;

            try
            {
                BitmapImage bi = new BitmapImage(new Uri(@TBPathImage.Text, UriKind.RelativeOrAbsolute));

                Img.Source = bi;
            }
            catch
            {

            } 

            e.Handled = true;
        }

        void bPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenImageWindow = new OpenFileDialog();
            OpenImageWindow.Filter = "Изображение|*.bmp; *.ico; *.gif; *.jpg; *.wdp; *.png; *.tiff";
            if (OpenImageWindow.ShowDialog() == true)
            {
                TBPathImage.Text = OpenImageWindow.FileName;

                BitmapImage bi = new BitmapImage(new Uri(@TBPathImage.Text, UriKind.RelativeOrAbsolute));

                Img.Source = bi;
            }

            e.Handled = true;
        }
    }

    class GridPropertiesImageLibrary : Grid
    {                
    }

    class GridOptionsGeneralProgramm : Grid
    {
        public TextBox TextBoxBrowseProject = new TextBox();
        public CheckBox CheckBoxCreateFolder;
        public CheckBox CheckBoxUpdate;

        public GridOptionsGeneralProgramm()
        {
            RowDefinition Row0 = new RowDefinition();
            Row0.Height = GridLength.Auto;

            RowDefinition Row1 = new RowDefinition();
            Row1.Height = GridLength.Auto;

            RowDefinition Row2 = new RowDefinition();
            Row2.Height = GridLength.Auto;

            RowDefinition Row3 = new RowDefinition();
            Row3.Height = GridLength.Auto;

            ColumnDefinition Column0 = new ColumnDefinition();
            Column0.Width = GridLength.Auto;

            ColumnDefinition Column1 = new ColumnDefinition();
            Column1.Width = GridLength.Auto;
          
            CheckBoxCreateFolder = new CheckBox();
            CheckBoxCreateFolder.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            CheckBoxCreateFolder.IsChecked = ((AppWPF)Application.Current).ConfigProgramBin.CreateFolder;
            CheckBoxCreateFolder.SetValue(Grid.RowProperty, 2);
            CheckBoxCreateFolder.SetValue(Grid.ColumnProperty, 1);

            CheckBoxUpdate = new CheckBox();
            CheckBoxUpdate.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            CheckBoxUpdate.IsChecked = ((AppWPF)Application.Current).ConfigProgramBin.IsWindowUpdate;
            CheckBoxUpdate.SetValue(Grid.RowProperty, 3);
            CheckBoxUpdate.SetValue(Grid.ColumnProperty, 1);

            Label LebelBrowseProject = new Label();
            LebelBrowseProject.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            LebelBrowseProject.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            LebelBrowseProject.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            LebelBrowseProject.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
            LebelBrowseProject.Content = "Размещение проектов: ";
            LebelBrowseProject.SetValue(Grid.RowProperty, 0);
            LebelBrowseProject.SetValue(Grid.ColumnProperty, 0);

            Label LebelOptionCreateFolder = new Label();
            LebelOptionCreateFolder.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            LebelOptionCreateFolder.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            LebelOptionCreateFolder.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            LebelOptionCreateFolder.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
            LebelOptionCreateFolder.Content = "Автоматически создавать папку под проект: ";
            LebelOptionCreateFolder.SetValue(Grid.RowProperty, 2);
            LebelOptionCreateFolder.SetValue(Grid.ColumnProperty, 0);

            Button Buttonbrowse = new Button();
            Buttonbrowse.Content = "Обзор...";
            Buttonbrowse.Click += BrowseDialog;
            Buttonbrowse.SetValue(Grid.RowProperty, 1);
            Buttonbrowse.SetValue(Grid.ColumnProperty, 1);

            RowDefinitions.Add(Row0);
            RowDefinitions.Add(Row1);
            RowDefinitions.Add(Row2);
            RowDefinitions.Add(Row3);
            ColumnDefinitions.Add(Column0);
            ColumnDefinitions.Add(Column1);

            MenuItem menuItemPasteBrowseProject = new MenuItem();
            menuItemPasteBrowseProject.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyBrowseProject = new MenuItem();
            menuItemCopyBrowseProject.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuBrowseProject = new System.Windows.Controls.ContextMenu();
            ContextMenuBrowseProject.Items.Add(menuItemPasteBrowseProject);
            ContextMenuBrowseProject.Items.Add(menuItemCopyBrowseProject);

            TextBoxBrowseProject.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TextBoxBrowseProject.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextBoxPaste));
            TextBoxBrowseProject.ContextMenu = ContextMenuBrowseProject;
            TextBoxBrowseProject.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            TextBoxBrowseProject.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            TextBoxBrowseProject.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            TextBoxBrowseProject.PreviewKeyDown += TextBoxBrowseProject_PreviewKeyDown;
            TextBoxBrowseProject.MaxLines = 1;
            TextBoxBrowseProject.FontSize = 14;
            TextBoxBrowseProject.IsReadOnly = true;
            TextBoxBrowseProject.SetValue(Grid.RowProperty, 1);
            TextBoxBrowseProject.SetValue(Grid.ColumnProperty, 0);

            if (((AppWPF)Application.Current).ConfigProgramBin.PathBrowseProject != null)
            {
                if (((AppWPF)Application.Current).ConfigProgramBin.PathBrowseProject.Length > 80)
                {
                    TextBoxBrowseProject.Text = ((AppWPF)Application.Current).ConfigProgramBin.PathBrowseProject = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SCADA\\Projects";
                }
                else
                {
                    TextBoxBrowseProject.Text = ((AppWPF)Application.Current).ConfigProgramBin.PathBrowseProject;
                }
            }
           
            Children.Add(LebelBrowseProject);
            Children.Add(LebelOptionCreateFolder);
            Children.Add(TextBoxBrowseProject);
            Children.Add(Buttonbrowse);
            Children.Add(CheckBoxCreateFolder);
            Children.Add(CheckBoxUpdate);
        }

        void TextBoxBrowseProject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb.Text.Length > 80)
            {
                e.Handled = true;
            }
        }

        private void IgnoreCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void TextBoxPaste(object sender, ExecutedRoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            string s;

            if (tb.SelectionLength > 0)
            {
                s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                s = s.Insert(tb.SelectionStart, Clipboard.GetText());
            }
            else
            {
                s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
            }

            if (s.Length > 80)
            {
                e.Handled = true;
            }
            else
            {
                tb.Paste();
            }
        }

        void BrowseDialog(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog BrowseDialog = new System.Windows.Forms.FolderBrowserDialog();

            if (BrowseDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxBrowseProject.Text = BrowseDialog.SelectedPath;
            }
            e.Handled = true;
        }
    }

    class GridOptionsDatabaseProgramm : Grid
    {
        public Popup PopupMessage = new Popup();
        public Label LPopupMessage = new Label();
        public TextBox TBServerName = new TextBox();
        public TextBox TBDatabaseName = new TextBox();
        public TextBox TBUserName = new TextBox();
        public TextBox TBUserPassword = new TextBox();
        public CheckBox chbUseDatabase;
        public RadioButton RBSecuritySSPI;

        public GridOptionsDatabaseProgramm()
        {
            LPopupMessage = new Label();
            LPopupMessage.BorderThickness = new Thickness(1);
            LPopupMessage.BorderBrush = Brushes.Red;
            LPopupMessage.Background = Brushes.White;

            PopupMessage.AllowsTransparency = true;
            PopupMessage.Child = LPopupMessage;
            PopupMessage.PopupAnimation = PopupAnimation.Fade;
            PopupMessage.StaysOpen = false;
            
            Label lAddDatabase = new Label();
            lAddDatabase.Content = "Добавить базу данных";

            Button bAddDatabase = new Button();
            bAddDatabase.MaxHeight = 25;
            bAddDatabase.Content = "Добавить...";         
            bAddDatabase.Click += bAddDatabase_Click;
           
            StackPanel pannelAddDatabase = new StackPanel();
            pannelAddDatabase.SetValue(Grid.RowProperty, 1);
            pannelAddDatabase.Orientation = Orientation.Horizontal;
            pannelAddDatabase.Children.Add(lAddDatabase);
            pannelAddDatabase.Children.Add(bAddDatabase);

            AppWPF application = ((AppWPF)Application.Current);

            ColumnDefinition column0 = new ColumnDefinition();
            column0.Width = GridLength.Auto;

            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = GridLength.Auto;

            RowDefinition row0 = new RowDefinition();
            row0.Height = GridLength.Auto;

            RowDefinition row1 = new RowDefinition();
            row1.Height = GridLength.Auto;

            RowDefinition row2 = new RowDefinition();
            row2.Height = GridLength.Auto;

            RowDefinition row3 = new RowDefinition();
            row3.Height = GridLength.Auto;

            RowDefinition row4 = new RowDefinition();
            row4.Height = GridLength.Auto;

            RowDefinition row5 = new RowDefinition();
            row5.Height = GridLength.Auto;

            RowDefinition row6 = new RowDefinition();
            row6.Height = GridLength.Auto;

            RowDefinition row7 = new RowDefinition();
            row7.Height = GridLength.Auto;

            RowDefinition row8 = new RowDefinition();
            row8.Height = GridLength.Auto;

            RowDefinition row9 = new RowDefinition();
            row9.Height = GridLength.Auto;

            RowDefinition row10 = new RowDefinition();
            row10.Height = GridLength.Auto;

            RowDefinition row11 = new RowDefinition();
            row11.Height = GridLength.Auto;

            MenuItem menuItemPasteUserPassword = new MenuItem();
            menuItemPasteUserPassword.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyUserPassword = new MenuItem();
            menuItemCopyUserPassword.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuUserPassword = new System.Windows.Controls.ContextMenu();
            ContextMenuUserPassword.Items.Add(menuItemPasteUserPassword);
            ContextMenuUserPassword.Items.Add(menuItemCopyUserPassword);

            MenuItem menuItemPasteUserName = new MenuItem();
            menuItemPasteUserName.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyUserName = new MenuItem();
            menuItemCopyUserName.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuUserName = new System.Windows.Controls.ContextMenu();
            ContextMenuUserName.Items.Add(menuItemPasteUserName);
            ContextMenuUserName.Items.Add(menuItemCopyUserName);

            MenuItem menuItemPasteServerName = new MenuItem();
            menuItemPasteServerName.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyServerName = new MenuItem();
            menuItemCopyServerName.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuServerName = new System.Windows.Controls.ContextMenu();
            ContextMenuServerName.Items.Add(menuItemPasteServerName);
            ContextMenuServerName.Items.Add(menuItemCopyServerName);

            MenuItem menuItemPasteDatabaseName = new MenuItem();
            menuItemPasteDatabaseName.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyDatabaseName = new MenuItem();
            menuItemCopyDatabaseName.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuDatabaseName = new System.Windows.Controls.ContextMenu();
            ContextMenuDatabaseName.Items.Add(menuItemPasteDatabaseName);
            ContextMenuDatabaseName.Items.Add(menuItemCopyDatabaseName);

            TBUserName.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBUserName.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextBoxPaste));
            TBUserName.ContextMenu = ContextMenuUserPassword;
            TBUserName.MinWidth = 150;
            TBUserName.PreviewKeyDown += TextBox_PreviewKeyDown;
            TBUserName.PreviewTextInput += TextBox_PreviewTextInput;

            TBUserPassword.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBUserPassword.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextBoxPaste));
            TBUserPassword.ContextMenu = ContextMenuUserName;
            TBUserPassword.MinWidth = 150;
            TBUserPassword.PreviewKeyDown += TextBox_PreviewKeyDown;
            TBUserPassword.PreviewTextInput += TextBox_PreviewTextInput;

            Label lUserName = new Label();
            lUserName.Content = "Имя пользователя: ";

            Label lUseDatabase = new Label();
            lUseDatabase.Content = "Использовать базу данных:";

            Label lUserPassword = new Label();
            lUserPassword.Content = "Пароль: ";

            chbUseDatabase = new CheckBox();
            chbUseDatabase.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            StackPanel panelUseDatabase = new StackPanel();
            panelUseDatabase.Orientation = Orientation.Horizontal;
            panelUseDatabase.Margin = new Thickness(30, 0, 0, 0);
            panelUseDatabase.Children.Add(lUseDatabase);
            panelUseDatabase.Children.Add(chbUseDatabase);
            panelUseDatabase.SetValue(Grid.RowProperty, 11);

            StackPanel panelUserName = new StackPanel();
            panelUserName.Margin = new Thickness(30, 0, 0, 0);
            panelUserName.Children.Add(lUserName);
            panelUserName.Children.Add(TBUserName);

            StackPanel panelUserPassword = new StackPanel();
            panelUserPassword.Margin = new Thickness(30, 0, 0, 0);
            panelUserPassword.Children.Add(lUserPassword);
            panelUserPassword.Children.Add(TBUserPassword);

            TBServerName.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBServerName.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextBoxPaste));
            TBServerName.ContextMenu = ContextMenuServerName;
            TBServerName.MinWidth = 150;
            TBServerName.PreviewKeyDown += TextBox_PreviewKeyDown;
            TBServerName.Margin = new Thickness(30, 0, 0, 0);
            TBServerName.SetValue(Grid.RowProperty, 7);
            TBServerName.PreviewTextInput += TextBox_PreviewTextInput;

            TBDatabaseName.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBDatabaseName.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextBoxPaste));
            TBDatabaseName.ContextMenu = ContextMenuDatabaseName;
            TBDatabaseName.MinWidth = 150;
            TBDatabaseName.PreviewKeyDown += TextBox_PreviewKeyDown;
            TBDatabaseName.Margin = new Thickness(30, 0, 0, 0);
            TBDatabaseName.SetValue(Grid.RowProperty, 9);
            TBDatabaseName.PreviewTextInput += TextBox_PreviewTextInput;

            Label lManualServerName = new Label();
            lManualServerName.Margin = new Thickness(30, 0, 0, 0);
            lManualServerName.SetValue(Grid.RowProperty, 6);
            lManualServerName.Content = "Имя сервера";

            Label lManualDatabaseName = new Label();
            lManualDatabaseName.Margin = new Thickness(30, 0, 0, 0);
            lManualDatabaseName.SetValue(Grid.RowProperty, 8);
            lManualDatabaseName.Content = "Имя базы данных";

            RBSecuritySSPI = new RadioButton();
            RBSecuritySSPI.Checked += RBSecuritySSPI_Checked;
            RBSecuritySSPI.Unchecked += RBSecuritySSPI_Unchecked;
            RBSecuritySSPI.Content = "Проверка подлинности Windows";
            RBSecuritySSPI.GroupName = "authentication";

            RadioButton rbNameAndPassword = new RadioButton();
            rbNameAndPassword.Content = "Проверка подлинности SQL Server";
            rbNameAndPassword.GroupName = "authentication";
           
            StackPanel panelAuthentication = new StackPanel();
            panelAuthentication.Children.Add(RBSecuritySSPI);            
            panelAuthentication.Children.Add(rbNameAndPassword);
            panelAuthentication.Children.Add(panelUserName);
            panelAuthentication.Children.Add(panelUserPassword);

            GroupBox gpAuthentication = new GroupBox();
            gpAuthentication.Header = "Авторизация.";
            gpAuthentication.SetValue(Grid.ColumnSpanProperty, 2);
            gpAuthentication.SetValue(Grid.RowProperty, 10);
            gpAuthentication.Content = panelAuthentication;
                       
            chbUseDatabase.IsChecked = application.ConfigProgramBin.UseDatabase;

            if (application.ConfigProgramBin.SQLDatabaseName == null || application.ConfigProgramBin.SQLDatabaseName.Length > 40)
            {
                application.ConfigProgramBin.SQLDatabaseName = "";
            }
                      
            if (application.ConfigProgramBin.SQLPassword == null || application.ConfigProgramBin.SQLPassword.Length > 40)
            {
                application.ConfigProgramBin.SQLPassword = "";
            }
                      
            if (application.ConfigProgramBin.SQLServerName == null || application.ConfigProgramBin.SQLServerName.Length > 40)
            {
                application.ConfigProgramBin.SQLServerName = "";
            }
                      
            if (application.ConfigProgramBin.SQLUserName == null || application.ConfigProgramBin.SQLUserName.Length > 40)
            {
                application.ConfigProgramBin.SQLUserName = "";
            }            
                     
            TBServerName.IsEnabled = true;
            TBServerName.Text = application.ConfigProgramBin.SQLServerName;

            TBDatabaseName.IsEnabled = true;
            TBDatabaseName.Text = application.ConfigProgramBin.SQLDatabaseName;
            
            if (application.ConfigProgramBin.SQLSecuritySSPI)
            {
                TBUserName.IsEnabled = false;

                TBUserPassword.IsEnabled = false;

                RBSecuritySSPI.IsChecked = true;
            }
            else
            {
                rbNameAndPassword.IsChecked = true;

                TBUserName.IsEnabled = true;
                TBUserName.Text = application.ConfigProgramBin.SQLUserName;

                TBUserPassword.IsEnabled = true;
                TBUserPassword.Text = application.ConfigProgramBin.SQLPassword;
            }

            ColumnDefinitions.Add(column0);
            ColumnDefinitions.Add(column1);

            RowDefinitions.Add(row0);
            RowDefinitions.Add(row1);
            RowDefinitions.Add(row2);
            RowDefinitions.Add(row3);
            RowDefinitions.Add(row4);
            RowDefinitions.Add(row5);
            RowDefinitions.Add(row6);
            RowDefinitions.Add(row7);
            RowDefinitions.Add(row8);
            RowDefinitions.Add(row9);
            RowDefinitions.Add(row10);
            RowDefinitions.Add(row11);

            Children.Add(pannelAddDatabase);
            Children.Add(TBServerName);
            Children.Add(TBDatabaseName);
            Children.Add(lManualServerName);
            Children.Add(lManualDatabaseName);
            Children.Add(gpAuthentication);
            Children.Add(panelUseDatabase);
        }
        
        void bAddDatabase_Click(object sender, RoutedEventArgs e)
        {
            DialogWindowAddDatabase windowAddDatabase = new DialogWindowAddDatabase();
            windowAddDatabase.SSPI = (bool)RBSecuritySSPI.IsChecked;
            windowAddDatabase.UserName = TBUserName.Text;
            windowAddDatabase.Password = TBUserPassword.Text;
            windowAddDatabase.Owner = Application.Current.MainWindow;         
            windowAddDatabase.ShowDialog();

            e.Handled = true;
        }

        void RBSecuritySSPI_Unchecked(object sender, RoutedEventArgs e)
        {
            TBUserName.IsEnabled = true;

            TBUserPassword.IsEnabled = true;

            e.Handled = true;
        }

        void RBSecuritySSPI_Checked(object sender, RoutedEventArgs e)
        {
            TBUserName.IsEnabled = false;

            TBUserPassword.IsEnabled = false;

            e.Handled = true;
        }

        void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb.Text.Length + 1 > 40)
            {
                if (tb.SelectionLength > 0)
                {
                    if ((tb.Text.Length + 1) - tb.SelectionLength <= 40)
                    {
                        return;
                    }
                }

                if (tb == TBServerName)
                {
                    LPopupMessage.Content = "Имя сервера не может быть длинее 40 символов.";
                }
                else if (tb == TBDatabaseName)
                {
                    LPopupMessage.Content = "Имя базы данных не может быть длинее 40 символов.";
                }
                else if (tb == TBUserName)
                {
                    LPopupMessage.Content = "Имя пользователя не может быть длинее 40 символов.";
                }
                else if (tb == TBUserPassword)
                {
                    LPopupMessage.Content = "Пароль не может быть длинее 40 символов.";
                }

                PopupMessage.PlacementTarget = tb;
                PopupMessage.IsOpen = true;

                e.Handled = true;
            }
        }

        private void IgnoreCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void TextBoxPaste(object sender, ExecutedRoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            string s;

            if (tb.SelectionLength > 0)
            {
                s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                s = s.Insert(tb.SelectionStart, Clipboard.GetText());
            }
            else
            {
                s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
            }

            if (s.Length > 40)
            {
                if (tb == TBServerName)
                {
                    LPopupMessage.Content = "Имя сервера не может быть длинее 40 символов.";
                }
                else if (tb == TBDatabaseName)
                {
                    LPopupMessage.Content = "Имя базы данных не может быть длинее 40 символов.";
                }
                else if (tb == TBUserName)
                {
                    LPopupMessage.Content = "Имя пользователя не может быть длинее 40 символов.";
                }
                else if (tb == TBUserPassword)
                {
                    LPopupMessage.Content = "Пароль не может быть длинее 40 символов.";
                }
               
                PopupMessage.PlacementTarget = tb;
                PopupMessage.IsOpen = true;

                e.Handled = true;   
            }
            else
            {
                tb.Paste();               
            }
        }

        void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb.Text.Length > 40 && e.Key == Key.Enter)
            {
                if (tb == TBServerName)
                {
                    LPopupMessage.Content = "Имя сервера не может быть длинее 40 символов.";
                }
                else if (tb == TBDatabaseName)
                {
                    LPopupMessage.Content = "Имя базы данных не может быть длинее 40 символов.";
                }
                else if (tb == TBUserName)
                {
                    LPopupMessage.Content = "Имя пользователя не может быть длинее 40 символов.";
                }
                else if (tb == TBUserPassword)
                {
                    LPopupMessage.Content = "Пароль не может быть длинее 40 символов.";
                }

                PopupMessage.PlacementTarget = tb;
                PopupMessage.IsOpen = true;

                e.Handled = true;
            }
            else
            {
                PopupMessage.IsOpen = false;
            }
        }                         
    }
}

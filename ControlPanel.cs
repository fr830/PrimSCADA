// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SCADA
{
    [Serializable]
    public class ControlPanel
    {
        public CollectionsEthernet CollectionEthernet { get; set; }

        public CollectionsCom CollectionCom { get; set; }

        public CollectionsModbus CollectionModbus { get; set; }

        public ControlPanel()
        {
            CollectionEthernet = new CollectionsEthernet();

            CollectionCom = new CollectionsCom();
            
            CollectionModbus = new CollectionsModbus();
        }
    }

    public class CollectionsEthernet : List<EthernetSer>
    { }

    public class CollectionsCom : List<ComSer>
    { }

    public class CollectionsModbus : List<ModbusSer>
    { }
    
    [Serializable]
    public class EthernetOperational
    {
        public ObservableCollection<ItemNet> CollectionItemNetRec { get; set; }
        public ObservableCollection<ItemNet> CollectionItemNetSend { get; set; }
        public ushort Port { get; set; }
        public int BufferSizeRec { get; set; }
        public int BufferSizeSend { get; set; }
        public string Description { get; set; }

        [NonSerialized]
        public EthernetOperationalSearch EthernetOperationalSearch = new EthernetOperationalSearch();

        public EthernetOperational()
        {
            CollectionItemNetRec = new CollectionNets();
            CollectionItemNetSend = new CollectionNets();
            BufferSizeRec = 4;
            BufferSizeSend = 4;
            Port = 1000;
            Description = "Описание " + ((AppWPF)Application.Current).GenerateTextName.Next();

            EthernetOperationalSearch.Description = Description;
            EthernetOperationalSearch.BufferSizeRec = BufferSizeRec;
            EthernetOperationalSearch.BufferSizeSend = BufferSizeSend;
            EthernetOperationalSearch.Port = Port;
        }
        
    }

    [Serializable]
    public class EthernetSer : ControlOnCanvasSer
    {
        public string ColorBackGround { get; set; }
        public string ColorBorder { get; set; }
        public double BorderThickness { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int BufferSizeRec { get; set; }
        public int BufferSizeSend { get; set; }
        public string EthernetProtocol { get; set; }
        public int Time { get; set; }
        public string[] IPAddressServer { get; set; }
        public byte[] IPAddressClient { get; set; }
        public int PortServer { get; set; }
        public int PortClient { get; set; }
        public string ID { get; set; }
        public string Description { get; set; }

        public ObservableCollection<ItemNet> CollectionItemNetRec { get; set; }

        public ObservableCollection<ItemNet> CollectionItemNetSend { get; set; }        

        public CollectionEthernetOperational CollectionEthernetOperational { get; set; }

        public TwoPointSegment RightSize { get; set; }
        public TwoPointSegment LeftSize { get; set; }
        public TwoPointSegment TopSize { get; set; }
        public TwoPointSegment DownSize { get; set; }
        public FivePointSegment Border { get; set; }

        public EthernetSer(int ZIndex, int transform, Point point)
            : base(ZIndex, transform)
        {
            IPAddressClient = new byte[4];

            PortServer = 1000;
            PortClient = 0;

            Description = "Описание " + ((AppWPF)Application.Current).GenerateTextName.Next();

            IPAddressServer = new string[4]{"192", "168", "0", "1"};

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

            if (Addresses.Count > 0)
            {
                IPAddressClient = Addresses[0].GetAddressBytes();
            }

            EthernetProtocol = "TCP";

            Time = 5;

            RightSize = new TwoPointSegment();
            LeftSize = new TwoPointSegment();
            TopSize = new TwoPointSegment();
            DownSize = new TwoPointSegment();
            Border = new FivePointSegment();

            Width = 84;
            Height = 22;
            Сoordinates = point;
            BorderThickness = 2;
            ColorBorder = new ColorConverter().ConvertToString(Colors.Black);
            ColorBackGround = new ColorConverter().ConvertToString(Colors.Transparent);
            BufferSizeRec = 4;
            BufferSizeSend = 4;

            ID = ((AppWPF)Application.Current).GenerateID();

            RightSize.point = new Point[2] { new Point(90, 3), new Point(90, 24) };
            LeftSize.point = new Point[2] { new Point(3, 3), new Point(3, 24) };
            TopSize.point = new Point[2] { new Point(3, 3), new Point(84, 3) };
            DownSize.point = new Point[2] { new Point(3, 27), new Point(84, 27) };
            Border.point = new Point[5] { new Point(0, 0), new Point(90, 0), new Point(90, 30), new Point(0, 30), new Point(0, 0) };

            CollectionItemNetRec = new CollectionNets();

            CollectionItemNetSend = new CollectionNets();

            CollectionEthernetOperational = new CollectionEthernetOperational();          
        }

        public EthernetSer()
        { }

        public static bool operator ==(EthernetSer op1, EthernetSer op2)
        {
            if ((object)op1 == null)
            {
                return (object)op2 == null;
            }

            return op1.Equals(op2);
        }

        public static bool operator !=(EthernetSer op1, EthernetSer op2)
        {
            if ((object)op1 != null)
            {
                if ((object)op2 == null)
                {
                    return true;
                }
                else
                {
                    return !op1.Equals(op2);
                }
            }
            else
            {
                return (object)op2 != null;
            }
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to EthernetSer return false.
            EthernetSer ethernetSer = obj as EthernetSer;
            if ((System.Object)ethernetSer == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (ID == ethernetSer.ID && BufferSizeRec == ethernetSer.BufferSizeRec && EthernetProtocol == ethernetSer.EthernetProtocol && Time == ethernetSer.Time
                && IPAddressServer[0] == ethernetSer.IPAddressServer[0] && IPAddressServer[1] == ethernetSer.IPAddressServer[1] && IPAddressServer[2] == ethernetSer.IPAddressServer[2]
                && IPAddressServer[3] == ethernetSer.IPAddressServer[3] && Description == ethernetSer.Description && PortClient == ethernetSer.PortClient && PortServer == ethernetSer.PortServer
                && IPAddressClient[0] == ethernetSer.IPAddressClient[0] && IPAddressClient[1] == ethernetSer.IPAddressClient[1] && IPAddressClient[2] == ethernetSer.IPAddressClient[2]
                && IPAddressClient[3] == ethernetSer.IPAddressClient[3] && BufferSizeSend == ethernetSer.BufferSizeSend)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(EthernetSer ethernetSer)
        {
            // If parameter is null return false:
            if ((object)ethernetSer == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (ID == ethernetSer.ID && BufferSizeRec == ethernetSer.BufferSizeRec && EthernetProtocol == ethernetSer.EthernetProtocol && Time == ethernetSer.Time
                && IPAddressServer[0] == ethernetSer.IPAddressServer[0] && IPAddressServer[1] == ethernetSer.IPAddressServer[1] && IPAddressServer[2] == ethernetSer.IPAddressServer[2]
                && IPAddressServer[3] == ethernetSer.IPAddressServer[3] && Description == ethernetSer.Description && PortClient == ethernetSer.PortClient && PortServer == ethernetSer.PortServer
                && IPAddressClient[0] == ethernetSer.IPAddressClient[0] && IPAddressClient[1] == ethernetSer.IPAddressClient[1] && IPAddressClient[2] == ethernetSer.IPAddressClient[2]
                && IPAddressClient[3] == ethernetSer.IPAddressClient[3] && BufferSizeSend == ethernetSer.BufferSizeSend)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Serializable]
    public class ComSer : ControlOnCanvasSer
    {
        public string ComPort { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public string StopBits { get; set; }
        public string Parity { get; set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }

        public string ColorBackGround { get; set; }
        public string ColorBorder { get; set; }
        public double BorderThickness { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public string Description { get; set; }

        public TwoPointSegment RightSize { get; set; }
        public TwoPointSegment LeftSize { get; set; }
        public TwoPointSegment TopSize { get; set; }
        public TwoPointSegment DownSize { get; set; }
        public FivePointSegment Border { get; set; }       

        public ComSer(int ZIndex, int transform, Point point)
            : base(ZIndex, transform)
        {
            ReadTimeout = 100;

            WriteTimeout = 100;

            Description = "Описание " + ((AppWPF)Application.Current).GenerateTextName.Next();

            Parity = "None";

            BaudRate = 115200;

            StopBits = "One";

            DataBits = 8;

            RightSize = new TwoPointSegment();
            LeftSize = new TwoPointSegment();
            TopSize = new TwoPointSegment();
            DownSize = new TwoPointSegment();
            Border = new FivePointSegment();

            Width = 84;
            Height = 22;
            Сoordinates = point;
            BorderThickness = 2;
            ColorBorder = new ColorConverter().ConvertToString(Colors.Black);
            ColorBackGround = new ColorConverter().ConvertToString(Colors.Transparent);

            RightSize.point = new Point[2] { new Point(90, 3), new Point(90, 24) };
            LeftSize.point = new Point[2] { new Point(3, 3), new Point(3, 24) };
            TopSize.point = new Point[2] { new Point(3, 3), new Point(84, 3) };
            DownSize.point = new Point[2] { new Point(3, 27), new Point(84, 27) };
            Border.point = new Point[5] { new Point(0, 0), new Point(90, 0), new Point(90, 30), new Point(0, 30), new Point(0, 0) };
        }

        public ComSer()
        { }
    }

    [Serializable]
    public class ModbusSer : ControlOnCanvasSer
    {
        public string ComPort { get; set; }
        public int Time { get; set; }
        public string Protocol { get; set; }
        public byte Command { get; set; }
        public byte SlaveAddress { get; set; }
        public ObservableCollection<ItemModbus> CollectionItemModbus { get; set; }
        public bool ReverseRegister { get; set; }
        public bool IsUS800 { get; set; }
        public string ID { get; set; }

        public string ColorBackGround { get; set; }
        public string ColorBorder { get; set; }
        public double BorderThickness { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public string Description { get; set; }

        public TwoPointSegment RightSize { get; set; }
        public TwoPointSegment LeftSize { get; set; }
        public TwoPointSegment TopSize { get; set; }
        public TwoPointSegment DownSize { get; set; }
        public FivePointSegment Border { get; set; }

        public ModbusSer(int ZIndex, int transform, Point point)
            : base(ZIndex, transform)
        {
            Time = 5;
            Description = "Описание " + ((AppWPF)Application.Current).GenerateTextName.Next();
            Protocol = "RTU";
            Command = 4;
            SlaveAddress = 1;

            ID = ((AppWPF)Application.Current).GenerateID();

            CollectionItemModbus = new CollectionModbusNets();

            RightSize = new TwoPointSegment();
            LeftSize = new TwoPointSegment();
            TopSize = new TwoPointSegment();
            DownSize = new TwoPointSegment();
            Border = new FivePointSegment();

            Width = 84;
            Height = 22;
            Сoordinates = point;
            BorderThickness = 2;
            ColorBorder = new ColorConverter().ConvertToString(Colors.Black);
            ColorBackGround = new ColorConverter().ConvertToString(Colors.Transparent);

            RightSize.point = new Point[2] { new Point(90, 3), new Point(90, 24) };
            LeftSize.point = new Point[2] { new Point(3, 3), new Point(3, 24) };
            TopSize.point = new Point[2] { new Point(3, 3), new Point(84, 3) };
            DownSize.point = new Point[2] { new Point(3, 27), new Point(84, 27) };
            Border.point = new Point[5] { new Point(0, 0), new Point(90, 0), new Point(90, 30), new Point(0, 30), new Point(0, 0) };
        }

        public ModbusSer()
        { }

        public static bool operator ==(ModbusSer op1, ModbusSer op2)
        {
            if ((object)op1 == null)
            {
                return (object)op2 == null;
            }

            return op1.Equals(op2);
        }

        public static bool operator !=(ModbusSer op1, ModbusSer op2)
        {
            if ((object)op1 != null)
            {
                if ((object)op2 == null)
                {
                    return true;
                }
                else
                {
                    return !op1.Equals(op2);
                }
            }
            else
            {
                return (object)op2 != null;
            }
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to EthernetSer return false.
            ModbusSer modbusSer = obj as ModbusSer;
            if ((System.Object)modbusSer == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (ID == modbusSer.ID && Protocol == modbusSer.Protocol && Time == modbusSer.Time && ComPort == modbusSer.ComPort && Description == modbusSer.Description && SlaveAddress == modbusSer.SlaveAddress && IsUS800 == modbusSer.IsUS800 && ReverseRegister == modbusSer.ReverseRegister && Command == modbusSer.Command)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(ModbusSer modbusSer)
        {
            // If parameter is null return false:
            if ((object)modbusSer == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (ID == modbusSer.ID && Protocol == modbusSer.Protocol && Time == modbusSer.Time && ComPort == modbusSer.ComPort && Description == modbusSer.Description && SlaveAddress == modbusSer.SlaveAddress && IsUS800 == modbusSer.IsUS800 && ReverseRegister == modbusSer.ReverseRegister && Command == modbusSer.Command)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Serializable]
    public class CollectionEthernetOperational : List<EthernetOperational>
    { }

    [Serializable]
    public class CollectionNets : ObservableCollection<ItemNet>
    { }

    [Serializable]
    public class CollectionModbusNets : ObservableCollection<ItemModbus>
    { }

    [Serializable]
    public class Item : INotifyPropertyChanged
    {
        [field: NonSerialized()]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        [field: NonSerializedAttribute()]
        private object valueItem;
        public object Value
        {
            get { return valueItem; }
            set
            {
                valueItem = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Value"));
            }
        }

        [field: NonSerializedAttribute()]
        private bool isBinding;
        public bool IsBinding
        {
            get { return isBinding; }
            set
            {
                isBinding = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsBinding"));
            }
        }

        public bool IsEmergencyUpDG
        {
            get { return IsEmergencyUp; }
            set
            {
                IsEmergencyUp = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsEmergencyUpDG"));
            }
        }

        public bool IsEmergencyDownDG
        {
            get { return IsEmergencyDown; }
            set
            {
                IsEmergencyDown = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsEmergencyDownDG"));
            }
        }

        public string TypeValue { get; set; }
        public string Description { get; set; }
        public bool IsSaveDatabase { get; set; }
        public string TableName { get; set; }
        public int PeridTimeSaveDB { get; set; }
        public string FormulaText { get; set; }
        public string Text { get; set; }
        public bool IsEmergencySaveDB { get; set; }
        public int PeriodEmergencySaveDB { get; set; }
        public bool IsEmergencyUp { get; set; }
        public bool IsEmergencyDown { get; set; }
        public object EmergencyUp { get; set; }
        public object EmergencyDown { get; set; }
        public string ID { get; set; }

        public Item()
        {
            TypeValue = "float";
            Value = "###";
            Description = "Описание " + ((AppWPF)Application.Current).GenerateTextName.Next();
            IsSaveDatabase = false;
            TableName = "";
            PeridTimeSaveDB = 120;
            FormulaText = "";
            Text = "";
            EmergencyUp = 0;
            EmergencyDown = 0;
            PeriodEmergencySaveDB = 60;
            ID = ((AppWPF)Application.Current).GenerateID();
        }
    }

    [Serializable]
    public class ItemModbus : Item
    {                                      
        public ushort Address { get; set; }               
        
        public ushort Function { get; set; }
                    
        [field: NonSerializedAttribute()]
        private object valueItem;
              
        public static bool operator ==(ItemModbus op1, ItemModbus op2)
        {
            if ((object)op1 == null)
            {
                return (object)op2 == null;
            }

            return op1.Equals(op2);
        }

        public static bool operator !=(ItemModbus op1, ItemModbus op2)
        {
            if ((object)op1 != null)
            {
                if ((object)op2 == null)
                {
                    return true;
                }
                else
                {
                    return !op1.Equals(op2);
                }
            }
            else
            {
                return (object)op2 != null;
            }
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to ItemNet return false.
            ItemModbus itemModbus = obj as ItemModbus;
            if ((System.Object)itemModbus == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (ID == itemModbus.ID && Description == itemModbus.Description && Function == itemModbus.Function && TypeValue == itemModbus.TypeValue && IsSaveDatabase == itemModbus.IsSaveDatabase
                 && TableName == itemModbus.TableName && PeridTimeSaveDB == itemModbus.PeridTimeSaveDB && Address == itemModbus.Address
                && FormulaText == itemModbus.FormulaText && Text == itemModbus.Text && Convert.ToSingle(EmergencyUp, CultureInfo.InvariantCulture) == Convert.ToSingle(itemModbus.EmergencyUp, CultureInfo.InvariantCulture)
                && Convert.ToSingle(EmergencyDown, CultureInfo.InvariantCulture) == Convert.ToSingle(itemModbus.EmergencyDown, CultureInfo.InvariantCulture) && IsEmergencyUp == itemModbus.IsEmergencyUp && IsEmergencyDown == itemModbus.IsEmergencyDown
                && IsEmergencySaveDB == itemModbus.IsEmergencySaveDB && PeriodEmergencySaveDB == itemModbus.PeriodEmergencySaveDB)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(ItemModbus itemModbus)
        {
            // If parameter is null return false:
            if ((object)itemModbus == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (ID == itemModbus.ID && Description == itemModbus.Description && Function == itemModbus.Function && TypeValue == itemModbus.TypeValue && IsSaveDatabase == itemModbus.IsSaveDatabase
                 && TableName == itemModbus.TableName && PeridTimeSaveDB == itemModbus.PeridTimeSaveDB && Address == itemModbus.Address
                && FormulaText == itemModbus.FormulaText && Text == itemModbus.Text && Convert.ToSingle(EmergencyUp, CultureInfo.InvariantCulture) == Convert.ToSingle(itemModbus.EmergencyUp, CultureInfo.InvariantCulture)
                && Convert.ToSingle(EmergencyDown, CultureInfo.InvariantCulture) == Convert.ToSingle(itemModbus.EmergencyDown, CultureInfo.InvariantCulture) && IsEmergencyUp == itemModbus.IsEmergencyUp && IsEmergencyDown == itemModbus.IsEmergencyDown
                && IsEmergencySaveDB == itemModbus.IsEmergencySaveDB && PeriodEmergencySaveDB == itemModbus.PeriodEmergencySaveDB)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ItemModbus()
        {
            TypeValue = "float";
            Value = "###";
            Function = 3;
            Description = "Описание " + ((AppWPF)Application.Current).GenerateTextName.Next();
            IsSaveDatabase = false;
            TableName = "";
            PeridTimeSaveDB = 120;
            FormulaText = "";
            Text = "";
            EmergencyUp = 0;
            EmergencyDown = 0;
            PeriodEmergencySaveDB = 60;
            ID = ((AppWPF)Application.Current).GenerateID();
        }
    }

    [Serializable]
    public class ItemNet : Item
    {       
        public int Range0 { get; set; }
        public int Range1 { get; set; }

        [field: NonSerializedAttribute()]
        public ItemModbus ItemModbus;
                   
        public static bool operator == (ItemNet op1, ItemNet op2)
        {
            if ((object)op1 == null)
            {
                return (object)op2 == null;
            }

            return op1.Equals(op2);
        }

        public static bool operator != (ItemNet op1, ItemNet op2)
        {
            if ((object)op1 != null)
            {
                if ((object)op2 == null)
                {
                    return true;
                }
                else
                {
                    return !op1.Equals(op2);
                }
            }
            else
            {
                return (object)op2 != null;  
            }                       
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to ItemNet return false.
            ItemNet itemNet = obj as ItemNet;
            if ((System.Object)itemNet == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (ID == itemNet.ID && Description == itemNet.Description && Range0 == itemNet.Range0 && Range1 == itemNet.Range1 && TypeValue == itemNet.TypeValue && IsSaveDatabase == itemNet.IsSaveDatabase
                 && TableName == itemNet.TableName && PeridTimeSaveDB == itemNet.PeridTimeSaveDB && FormulaText == itemNet.FormulaText && Text == itemNet.Text && Convert.ToSingle(EmergencyUp, CultureInfo.InvariantCulture) == Convert.ToSingle(itemNet.EmergencyUp, CultureInfo.InvariantCulture)
                && Convert.ToSingle(EmergencyDown, CultureInfo.InvariantCulture) == Convert.ToSingle(itemNet.EmergencyDown, CultureInfo.InvariantCulture) && IsEmergencyUp == itemNet.IsEmergencyUp && IsEmergencyDown == itemNet.IsEmergencyDown
                && IsEmergencySaveDB == itemNet.IsEmergencySaveDB && PeriodEmergencySaveDB == itemNet.PeriodEmergencySaveDB)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(ItemNet itemNet)
        {
            // If parameter is null return false:
            if ((object)itemNet == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (ID == itemNet.ID && Description == itemNet.Description && Range0 == itemNet.Range0 && Range1 == itemNet.Range1 && TypeValue == itemNet.TypeValue && IsSaveDatabase == itemNet.IsSaveDatabase
                 && TableName == itemNet.TableName && PeridTimeSaveDB == itemNet.PeridTimeSaveDB && FormulaText == itemNet.FormulaText && Text == itemNet.Text && Convert.ToDecimal(EmergencyUp) == Convert.ToDecimal(itemNet.EmergencyUp)
                && Convert.ToDecimal(EmergencyDown) == Convert.ToDecimal(itemNet.EmergencyDown) && IsEmergencyUp == itemNet.IsEmergencyUp && IsEmergencyDown == itemNet.IsEmergencyDown
                && IsEmergencySaveDB == itemNet.IsEmergencySaveDB && PeriodEmergencySaveDB == itemNet.PeriodEmergencySaveDB)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ItemNet()
        {
            Range0 = 0;
            Range1 = 3;           
        }

        public byte[] Formula(int PeriodTime)
        {
            string modbusID;
            string itemModbusID;

            byte[] bWrite;

            if (ItemModbus != null)
            {
                if (ItemModbus.TypeValue == "float")
                {
                    bWrite = BitConverter.GetBytes((float)ItemModbus.Value);

                    return bWrite;
                }
                else if (ItemModbus.TypeValue == "int")
                {
                    bWrite = BitConverter.GetBytes((int)ItemModbus.Value);

                    return bWrite;
                }
                else if (ItemModbus.TypeValue == "uint")
                {
                    bWrite = bWrite = BitConverter.GetBytes((uint)ItemModbus.Value);

                    return bWrite;
                }
                else if (ItemModbus.TypeValue == "short")
                {
                    bWrite = BitConverter.GetBytes((short)ItemModbus.Value);

                    return bWrite;
                }
                else if (ItemModbus.TypeValue == "ushort")
                {
                    bWrite = BitConverter.GetBytes((ushort)ItemModbus.Value);

                    return bWrite;
                }
                else if (ItemModbus.TypeValue == "byte")
                {
                    bWrite = BitConverter.GetBytes((byte)ItemModbus.Value);

                    return bWrite;
                }
                else if (ItemModbus.TypeValue == "sbyte")
                {
                    bWrite = BitConverter.GetBytes((sbyte)ItemModbus.Value);

                    return bWrite;
                }
            }
            else if (FormulaText.IndexOf("ItemModbusID") != -1)
            {
                int modbusIDIndex = Text.IndexOf("ItemModbusID");

                modbusID = Text.Substring(modbusIDIndex, 32);

                itemModbusID = Text.Substring(modbusIDIndex + 33, 32);

                foreach (ModbusSer mb in ((AppWPF)Application.Current).CollectionModbusSers)
                {
                    if (mb.ID == modbusID)
                    {
                        foreach (ItemModbus im in mb.CollectionItemModbus)
                        {
                            if (im.ID == itemModbusID)
                            {
                                ItemModbus = im;

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
            else if (FormulaText.IndexOf("Period") != -1)
            {
                return bWrite = BitConverter.GetBytes(PeriodTime);
            }

            return null;
        }
    }

    public class AddItemNetWindow : Window
    {
        TextBox TBRange0;
        TextBox TBRange1;
        int BufferSize;
        int MaxDigit;
        ItemNet NewItemNet;
        TextBox TBDescriptionItemNet;
        TextBox TBPeriodTimeSaveDB;
        TextBox TBTableName;
        TextBox TBFormula;
        TextBox TBText;
        TextBox TBEmergencyUp;
        TextBox TBEmergencyDown;
        TextBox TBEmergencySaveBD;
        Popup PopupMessage = new Popup();
        Label LPopupMessage = new Label();
        ObservableCollection<ItemNet> CollectionNets;
        DataGrid DG = new DataGrid();
        decimal EscDigital;
        ItemNet Item;
        string EscText;

        public AddItemNetWindow(int bufferSize, ObservableCollection<ItemNet> collectionNets, EthernetOperational eo)
        {
            LPopupMessage = new Label();
            LPopupMessage.BorderThickness = new Thickness(1);
            LPopupMessage.BorderBrush = Brushes.Red;
            LPopupMessage.Background = Brushes.White;

            PopupMessage.AllowsTransparency = true;
            PopupMessage.Child = LPopupMessage;
            PopupMessage.PopupAnimation = PopupAnimation.Fade;
            PopupMessage.StaysOpen = false;

            RowDefinition row0 = new RowDefinition();
            row0.Height = GridLength.Auto;

            RowDefinition row1 = new RowDefinition();
            row1.Height = GridLength.Auto;

            CollectionNets = collectionNets;

            BufferSize = bufferSize;

            MaxDigit = bufferSize.ToString().Length; 

            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);

            double d;
            double dpiX = 0, dpiY = 0;
            if (source != null)
            {
                dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            }

            d = dpiX / 72;

            this.ResizeMode = ResizeMode.NoResize;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Owner = Application.Current.MainWindow;
            this.ShowInTaskbar = false;
            this.FontSize = 12 * d;
            this.Title = "Добавление сетевого параметра";

            NewItemNet = new ItemNet();

            ObservableCollection<ItemNet> collectionNet = new ObservableCollection<ItemNet>();
            collectionNet.Add(NewItemNet);

            Binding bindingCollectionNet = new Binding();
            bindingCollectionNet.NotifyOnSourceUpdated = true;
            bindingCollectionNet.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingCollectionNet.Source = collectionNet;
            
            DG.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            DG.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            DG.Margin = new Thickness(0, 0, 0, 5);
            DG.BorderThickness = new Thickness(3);
            DG.BorderBrush = Brushes.Black;
            DG.AutoGenerateColumns = false;
            DG.CellEditEnding += DG_CellEditEnding;
            DG.PreparingCellForEdit += DG_PreparingCellForEdit;
            DG.MaxHeight = 250;
            DG.MaxWidth = 800;
            DG.CanUserAddRows = false;
            DG.SetBinding(DataGrid.ItemsSourceProperty, bindingCollectionNet);            

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

            DG.Columns.Add(type);
            DG.Columns.Add(value);
            DG.Columns.Add(range0);
            DG.Columns.Add(range1);
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
                                                           
            Button apply = new Button();
            apply.Click += apply_ClickMouseDown;
            apply.IsDefault = true;
            apply.Margin = new Thickness(6);
            apply.Content = "Применить";

            Button cancel = new Button();
            cancel.IsCancel = true;
            cancel.Margin = new Thickness(6);
            cancel.Content = "Закрыть";

            StackPanel panelButton = new StackPanel();
            panelButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            panelButton.Orientation = Orientation.Horizontal;
            panelButton.SetValue(Grid.RowProperty, 1);
            panelButton.Children.Add(apply);
            panelButton.Children.Add(cancel);
            
            Grid grid = new Grid();
            grid.Children.Add(DG);
            grid.Children.Add(panelButton);

            grid.RowDefinitions.Add(row0);
            grid.RowDefinitions.Add(row1);

            this.Content = grid;
        }

        void BackspacePreviewTextKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (e.Key == Key.Back)
            {
                PopupMessage.IsOpen = false;
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
         
        void TextBoxFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            tb.SelectAll();

            e.Handled = true;
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

        private void DG_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            ContentPresenter element = (ContentPresenter)e.EditingElement;

            Item = (ItemNet)e.Row.DataContext;           

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
            else if (element.ContentTemplate.FindName("TextBoxRange0", element) != null)
            {
                TBRange0 = (TextBox)element.ContentTemplate.FindName("TextBoxRange0", element);

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

        private void DG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
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

                if (tb.Text.Length == 0)
                {
                    tb.Text = EscDigital.ToString();
                }

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    if (Item.TypeValue != "bool")
                    {
                        tb.Text = EscDigital.ToString();
                    }
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxEmergencyDown", element) != null)
            {
                TextBox tb = (TextBox)element.ContentTemplate.FindName("TextBoxEmergencyDown", element);

                if (tb.Text.Length == 0)
                {
                    tb.Text = EscDigital.ToString();
                }

                PopupMessage.IsOpen = false;

                if (e.EditAction == DataGridEditAction.Cancel)
                {
                    if (Item.TypeValue != "bool")
                    {
                        tb.Text = EscDigital.ToString();
                    }
                }
            }
            else if (element.ContentTemplate.FindName("TextBoxPeriodEmergencySaveDB", element) != null)
            {
                TextBox tb = (TextBox)element.ContentTemplate.FindName("TextBoxPeriodEmergencySaveDB", element);

                if (tb.Text.Length == 0)
                {
                    tb.Text = EscDigital.ToString();
                }

                PopupMessage.IsOpen = false;

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

            if (tb == TBPeriodTimeSaveDB || tb == TBEmergencySaveBD)
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
                    if (tb == TBPeriodTimeSaveDB)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e);
                    }
                    else if (tb == TBEmergencySaveBD)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e);
                    }
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 1 || d > 86400)
                    {
                        if (tb == TBPeriodTimeSaveDB)
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
                    if (tb == TBPeriodTimeSaveDB)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e);
                    }
                    else if (tb == TBEmergencySaveBD)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e);
                    }
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
                        PopupMessageShow(StaticValue.SRange0, tb, "Диапазон буфера 0 -  " + BufferSize, e);
                    }
                    else if (tb == TBRange1)
                    {
                        PopupMessageShow(StaticValue.SRange1, tb, "Диапазон буфера 0 -  " + BufferSize, e);
                    }
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > BufferSize)
                    {
                        if (tb == TBRange0)
                        {
                            PopupMessageShow(StaticValue.SRange0, tb, "Диапазон буфера 0 -  " + BufferSize, e);
                        }
                        else if (tb == TBRange1)
                        {
                            PopupMessageShow(StaticValue.SRange1, tb, "Диапазон буфера 0 -  " + BufferSize, e);
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
                        PopupMessageShow(StaticValue.SRange0, tb, "Диапазон буфера 0 -  " + BufferSize, e);
                    }
                    else if (tb == TBRange1)
                    {
                        PopupMessageShow(StaticValue.SRange1, tb, "Диапазон буфера 0 -  " + BufferSize, e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-65535", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-65535", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-65535", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-65535", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -2147483648 - 2147483647", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -2147483648 - 2147483647", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -2147483648 - 2147483647", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -2147483648 - 2147483647", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -9223372036854775808-9223372036854775807", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -9223372036854775808-9223372036854775807", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -9223372036854775808-9223372036854775807", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -9223372036854775808-9223372036854775807", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-18446744073709551615", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-18446744073709551615", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-18446744073709551615", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-18446744073709551615", e);
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

            if (tb == TBPeriodTimeSaveDB || tb == TBEmergencySaveBD)
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
                    if (tb == TBPeriodTimeSaveDB)
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
                        if (tb == TBPeriodTimeSaveDB)
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
                    if (tb == TBPeriodTimeSaveDB)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e);
                    }
                    else if (tb == TBEmergencySaveBD)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e);
                    }
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
                        PopupMessageShow(StaticValue.SRange0, tb, "Диапазон буфера 0 -  " + BufferSize, e);
                    }
                    else if (tb == TBRange1)
                    {
                        PopupMessageShow(StaticValue.SRange1, tb, "Диапазон буфера 0 -  " + BufferSize, e);
                    }
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > BufferSize)
                    {
                        if (tb == TBRange0)
                        {
                            PopupMessageShow(StaticValue.SRange0, tb, "Диапазон буфера 0 -  " + BufferSize, e);
                        }
                        else if (tb == TBRange1)
                        {
                            PopupMessageShow(StaticValue.SRange1, tb, "Диапазон буфера 0 -  " + BufferSize, e);
                        }
                    }
                }
                else
                {
                    if (tb == TBRange0)
                    {
                        PopupMessageShow(StaticValue.SRange0, tb, "Диапазон буфера 0 -  " + BufferSize, e);
                    }
                    else if (tb == TBRange1)
                    {
                        PopupMessageShow(StaticValue.SRange1, tb, "Диапазон буфера 0 -  " + BufferSize, e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -79,228,162,514,264,337,593,543,950,335 - 79,228,162,514,264,337,593,543,950,335", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -32768 - 32767", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -32768 - 32767", e);
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
                                up = Convert.ToInt16(((ItemNet)tb.DataContext).EmergencyUp);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -32768 - 32767", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -32768 - 32767", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-65535", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-65535", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-65535", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-65535", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -2147483648 - 2147483647", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -2147483648 - 2147483647", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -2147483648 - 2147483647", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -2147483648 - 2147483647", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -9223372036854775808-9223372036854775807", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -9223372036854775808-9223372036854775807", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -9223372036854775808-9223372036854775807", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -9223372036854775808-9223372036854775807", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-18446744073709551615", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-18446744073709551615", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-18446744073709551615", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-18446744073709551615", e);
                        }
                    }
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

                    PopupMessageShow(StaticValue.SDescription, tb, StaticValue.SDescriptionMessage, e);
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
                 
        void apply_ClickMouseDown(object sender, RoutedEventArgs e)
        {                                   
            CollectionNets.Add(NewItemNet);

            this.Close();
        }

        private void IgnoreCut(object sender, ExecutedRoutedEventArgs e)
        {
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
    }

    public class AddItemModbusWindow : Window
    {
        ItemModbus NewItemModbus;
        TextBox TBDescriptionItemNet;
        TextBox TBAddress;
        TextBox TBPeriodTimeSaveDB;
        TextBox TBTableName;
        TextBox TBFormula;
        TextBox TBText;
        TextBox TBEmergencyUp;
        TextBox TBEmergencyDown;
        TextBox TBEmergencySaveBD;
        DataGrid DG = new DataGrid();
        ObservableCollection<ItemModbus> CollectionNets;
        ItemModbus Item;
        string EscText;
        float EscDigital;

        Popup PopupMessage = new Popup();
        Label LPopupMessage = new Label();

        public AddItemModbusWindow(ObservableCollection<ItemModbus> oldCollectionNets)
        {
            LPopupMessage = new Label();
            LPopupMessage.BorderThickness = new Thickness(1);
            LPopupMessage.BorderBrush = Brushes.Red;
            LPopupMessage.Background = Brushes.White;

            PopupMessage.AllowsTransparency = true;
            PopupMessage.Child = LPopupMessage;
            PopupMessage.PopupAnimation = PopupAnimation.Fade;
            PopupMessage.StaysOpen = false;

            RowDefinition row0 = new RowDefinition();
            row0.Height = GridLength.Auto;

            RowDefinition row1 = new RowDefinition();
            row1.Height = GridLength.Auto;

            CollectionNets = oldCollectionNets;

            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);

            double d;
            double dpiX = 0, dpiY = 0;
            if (source != null)
            {
                dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            }

            d = dpiX / 72;

            this.ResizeMode = ResizeMode.NoResize;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Owner = Application.Current.MainWindow;
            this.ShowInTaskbar = false;
            this.FontSize = 12 * d;
            this.Title = "Добавление сетевого параметра";

            #region DG
            NewItemModbus = new ItemModbus();

            ObservableCollection<ItemModbus> CollectionNet = new ObservableCollection<ItemModbus>();
            CollectionNet.Add(NewItemModbus);

            Binding bindingCollectionNet = new Binding();
            bindingCollectionNet.NotifyOnSourceUpdated = true;
            bindingCollectionNet.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bindingCollectionNet.Source = CollectionNet;

            DG.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            DG.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            DG.BorderThickness = new Thickness(3);
            DG.BorderBrush = Brushes.Black;
            DG.SetValue(Grid.RowProperty, 0);
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
                      
            Button apply = new Button();
            apply.Click += apply_ClickMouseDown;
            apply.IsDefault = true;
            apply.Margin = new Thickness(6);
            apply.Content = "Применить";

            Button cancel = new Button();
            cancel.IsCancel = true;
            cancel.Margin = new Thickness(6);
            cancel.Content = "Закрыть";

            StackPanel panelButton = new StackPanel();
            panelButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            panelButton.Orientation = Orientation.Horizontal;
            panelButton.SetValue(Grid.RowProperty, 1);
            panelButton.Children.Add(apply);
            panelButton.Children.Add(cancel);

            Grid grid = new Grid();
            grid.Children.Add(DG);
            grid.Children.Add(panelButton);

            grid.RowDefinitions.Add(row0);
            grid.RowDefinitions.Add(row1);           

            this.Content = grid;
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

        void TextBoxFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            tb.SelectAll();

            e.Handled = true;
        }

        void BackspacePreviewTextKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (e.Key == Key.Back)
            {
                PopupMessage.IsOpen = false;
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

        private void DigitalTextBoxPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            TextBox tb = (TextBox)sender;
            string s;

            if (tb == TBPeriodTimeSaveDB || tb == TBEmergencySaveBD)
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
                    if (tb == TBPeriodTimeSaveDB)
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
                        if (tb == TBPeriodTimeSaveDB)
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
                    if (tb == TBPeriodTimeSaveDB)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e);
                    }
                    else if (tb == TBEmergencySaveBD)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e);
                    }
                }
            }            
            else if (tb == TBAddress)
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
                    PopupMessageShow(StaticValue.SAddress, tb, "Диапазон 0 - 65535", e);
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > 65535)
                    {
                        PopupMessageShow(StaticValue.SAddress, tb, "Диапазон 0 - 65535", e);
                    }
                    else
                    {
                        PopupMessage.IsOpen = false;

                        tb.Paste();
                    }
                }
                else
                {
                    PopupMessageShow(StaticValue.SAddress, tb, "Диапазон 0 - 65535", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-65535", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-65535", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-65535", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-65535", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -2147483648 - 2147483647", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -2147483648 - 2147483647", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -2147483648 - 2147483647", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -2147483648 - 2147483647", e);
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

            if (tb == TBPeriodTimeSaveDB || tb == TBEmergencySaveBD)
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
                    if (tb == TBPeriodTimeSaveDB)
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
                        if (tb == TBPeriodTimeSaveDB)
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
                    if (tb == TBPeriodTimeSaveDB)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveDB, tb, StaticValue.SRange1_86400, e);
                    }
                    else if (tb == TBEmergencySaveBD)
                    {
                        PopupMessageShow(StaticValue.SPeriodSaveSetDB, tb, StaticValue.SRange1_86400, e);
                    }
                }
            }           
            else if (tb == TBAddress)
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
                    PopupMessageShow(StaticValue.SAddress, tb, "Диапазон 0 - 65535", e);
                }
                else if (double.TryParse(s, out d))
                {
                    if (d < 0 || d > 65535)
                    {
                        PopupMessageShow(StaticValue.SAddress, tb, "Диапазон 0 - 65535", e);
                    }
                }
                else
                {
                    PopupMessageShow(StaticValue.SAddress, tb, "Диапазон 0 - 65535", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -32768 - 32767", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -32768 - 32767", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -32768 - 32767", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -32768 - 32767", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-65535", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-65535", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон 0-65535", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон 0-65535", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -2147483648 - 2147483647", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -2147483648 - 2147483647", e);
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
                            PopupMessageShow(StaticValue.SSetUp, tb, "Диапазон -2147483648 - 2147483647", e);
                        }
                        else if (tb == TBEmergencyDown)
                        {
                            PopupMessageShow(StaticValue.SSetDown, tb, "Диапазон -2147483648 - 2147483647", e);
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

            if (tb == TBDescriptionItemNet)
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
             
        void apply_ClickMouseDown(object sender, RoutedEventArgs e)
        {                          
            CollectionNets.Add(NewItemModbus);

            this.Close();
        }

        private void IgnoreCut(object sender, ExecutedRoutedEventArgs e)
        {
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

        void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            tb.SelectAll();

            e.Handled = true;
        }
    }
}

// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Resources;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text;
using System.Net.Sockets;
using System.IO.Ports;
using Npgsql;
using System.Security.Cryptography;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>

    public class FileAssociation
    {
        // Associate file extension with progID, description, icon and application
        public static void Associate(string extension,
               string progID, string description, string icon, string application)
        {
            Registry.ClassesRoot.CreateSubKey(extension).SetValue("", progID);
            if (progID != null && progID.Length > 0)
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(progID))
                {
                    if (description != null)
                        key.SetValue("", description);
                    if (icon != null)
                        key.CreateSubKey("DefaultIcon").SetValue("", ToShortPathName(icon));
                    if (application != null)
                        key.CreateSubKey(@"Shell\Open\Command").SetValue("",
                                    ToShortPathName(application) + " \"%1\"");
                }
        }

        // Return true if extension already associated in registry
        public static bool IsAssociated(string extension)
        {
            return (Registry.ClassesRoot.OpenSubKey(extension, false) != null);
        }

        [DllImport("Kernel32.dll")]
        private static extern uint GetShortPathName(string lpszLongPath,
            [Out] StringBuilder lpszShortPath, uint cchBuffer);

        // Return short path format of a file name
        private static string ToShortPathName(string longName)
        {
            StringBuilder s = new StringBuilder(1000);
            uint iSize = (uint)s.Capacity;
            uint iRet = GetShortPathName(longName, s, iSize);
            return s.ToString();
        }
    }

    public partial class AppWPF : Application
    {
        public Random GenerateTextName = new Random((int)DateTime.Now.Ticks);

        public SerializationSetting ConfigProgramBin;

        //Если запускаем через файл проекта нужен первый путь, иначе он изменится
        public string StartPath;

        private Dictionary<string, Page> collectionPage = new Dictionary<string,Page>();
        public Dictionary<string, Page> CollectionPage
        {
            get { return collectionPage; }
        }

        private ObservableCollection<EthernetSer> collectionEthernetSers = new ObservableCollection<EthernetSer>();
        public ObservableCollection<EthernetSer> CollectionEthernetSers
        {
            get { return collectionEthernetSers; }
        }

        private ObservableCollection<ModbusSer> collectionModbusSers = new ObservableCollection<ModbusSer>();
        public ObservableCollection<ModbusSer> CollectionModbusSers
        {
            get { return collectionModbusSers; }
        }

        private ObservableCollection<ComSer> collectionComSers = new ObservableCollection<ComSer>();
        public ObservableCollection<ComSer> CollectionComSers
        {
            get { return collectionComSers; }
        }

        private List<TabItemParent> collectionSaveTabItem = new List<TabItemParent>();
        public List<TabItemParent> CollectionSaveTabItem
        {
            get { return collectionSaveTabItem; }
        }

        private Dictionary<string, TabItemParent> collectionTabItemParent = new Dictionary<string,TabItemParent>();
        public Dictionary<string, TabItemParent> CollectionTabItemParent
        {
            get { return collectionTabItemParent; }
        }

        private Dictionary<string, ControlPanel> collectionControlPanel = new Dictionary<string,ControlPanel>();
        public Dictionary<string, ControlPanel> CollectionControlPanel
        {
            get { return collectionControlPanel; }
        }

        public bool IsSaveProject { get; set; }

        public Cursor CursorPipe;
        public Cursor CursorPipeInvalid;
        public Cursor CursorPipe90;
        public Cursor CursorPipe90Invalid;
        public Cursor CursorText;
        public Cursor CursorTextInvalid;
        public Cursor CursorEthernet;
        public Cursor CursorEthernetInvalid;
        public Cursor CursorCom;
        public Cursor CursorComInvalid;
        public Cursor CursorDisplay;
        public Cursor CursorDisplayInvalid;
        public Cursor CursorImage;
        public Cursor CursorImageInvalid;
        public Cursor CursorModbus;
        public Cursor CursorModbusInvalid;

        public void SaveTabItem(TabItemParent tabItemParent)
        {
            if (!tabItemParent.isSave)
            {
                StackPanel panel = (StackPanel)tabItemParent.Header;
                Label l = (Label)panel.Children[0];
                l.Content = tabItemParent.IS.Name + "*";

                tabItemParent.isSave = true;
                ((AppWPF)Application.Current).CollectionSaveTabItem.Add(tabItemParent);
            }
        }

        public string GenerateID()
        {
            byte[] bytes = new Byte[16];
            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
            rand.GetBytes(bytes);
            Guid myGuid = new Guid(bytes);
            return myGuid.ToString().Replace("-", "").Trim();
        }

        private void StartUp(object sender, StartupEventArgs e)
        {
            if (!FileAssociation.IsAssociated(".proj"))
            {
                var location = System.Reflection.Assembly.GetEntryAssembly().Location; // Полный путь с исполняемым екзешником
                FileAssociation.Associate(".proj", "ClassID.ProgID", "Проект PrimSCADA", "YourIcon.ico", location);
            }

            StreamResourceInfo sriPipe = Application.GetResourceStream(new Uri("Images/Pipe.ico", UriKind.Relative));
            CursorPipe = new Cursor(sriPipe.Stream);
            StreamResourceInfo sriPipeInvalid = Application.GetResourceStream(new Uri("Images/PipeDragInvalid.ico", UriKind.Relative));
            CursorPipeInvalid = new Cursor(sriPipeInvalid.Stream);
            StreamResourceInfo sriPipe90 = Application.GetResourceStream(new Uri("Images/Pipe90Drag.ico", UriKind.Relative));
            CursorPipe90 = new Cursor(sriPipe90.Stream);
            StreamResourceInfo sriPipe90Invalid = Application.GetResourceStream(new Uri("Images/Pipe90DragInvalid.ico", UriKind.Relative));
            CursorPipe90Invalid = new Cursor(sriPipe90Invalid.Stream);
            StreamResourceInfo sriText = Application.GetResourceStream(new Uri("Images/Text.ico", UriKind.Relative));
            CursorText = new Cursor(sriText.Stream);
            StreamResourceInfo sriTextInvalid = Application.GetResourceStream(new Uri("Images/TextInvalid.ico", UriKind.Relative));
            CursorTextInvalid = new Cursor(sriTextInvalid.Stream);
            StreamResourceInfo sriEthernet = Application.GetResourceStream(new Uri("Images/Ethernet.ico", UriKind.Relative));
            CursorEthernet = new Cursor(sriEthernet.Stream);
            StreamResourceInfo sriEthernetInvalid = Application.GetResourceStream(new Uri("Images/EthernetInvalid.ico", UriKind.Relative));
            CursorEthernetInvalid = new Cursor(sriEthernetInvalid.Stream);
            StreamResourceInfo sriCom = Application.GetResourceStream(new Uri("Images/Com.ico", UriKind.Relative));
            CursorCom = new Cursor(sriCom.Stream);
            StreamResourceInfo sriComInvalid = Application.GetResourceStream(new Uri("Images/ComInvalid.ico", UriKind.Relative));
            CursorComInvalid = new Cursor(sriComInvalid.Stream);
            StreamResourceInfo sriDisplay = Application.GetResourceStream(new Uri("Images/Display.ico", UriKind.Relative));
            CursorDisplay = new Cursor(sriDisplay.Stream);
            StreamResourceInfo sriDisplayInvalid = Application.GetResourceStream(new Uri("Images/DisplayInvalid.ico", UriKind.Relative));
            CursorDisplayInvalid = new Cursor(sriDisplayInvalid.Stream);
            StreamResourceInfo sriImage = Application.GetResourceStream(new Uri("Images/Image.ico", UriKind.Relative));
            CursorImage = new Cursor(sriImage.Stream);
            StreamResourceInfo sriImageInvalid = Application.GetResourceStream(new Uri("Images/ImageInvalid.ico", UriKind.Relative));
            CursorImageInvalid = new Cursor(sriImageInvalid.Stream);
            StreamResourceInfo sriModbus = Application.GetResourceStream(new Uri("Images/Modbus.ico", UriKind.Relative));
            CursorModbus = new Cursor(sriModbus.Stream);
            StreamResourceInfo sriModbusInvalid = Application.GetResourceStream(new Uri("Images/ModbusInvalid.ico", UriKind.Relative));
            CursorModbusInvalid = new Cursor(sriModbusInvalid.Stream);

            StartPath = Environment.CurrentDirectory + "\\Config.bin";

            if (File.Exists(Environment.CurrentDirectory + "\\Config.bin"))
            {
                Stream FileStream = File.OpenRead(Environment.CurrentDirectory + "\\Config.bin");
                BinaryFormatter deserializer = new BinaryFormatter();
                ConfigProgramBin = (SerializationSetting)deserializer.Deserialize(FileStream);
                FileStream.Close();

                if (string.IsNullOrEmpty(ConfigProgramBin.PathBrowseProject))
                {
                    ConfigProgramBin.PathBrowseProject = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SCADA\\Projects";
                }
            }
            else
            {
                Stream FileStream = File.Create(Environment.CurrentDirectory + "\\Config.bin");
                BinaryFormatter serializer = new BinaryFormatter();
                ConfigProgramBin = new SerializationSetting();
                ConfigProgramBin.PathBrowseProject = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SCADA\\Projects";
                serializer.Serialize(FileStream, ConfigProgramBin);
                FileStream.Close();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if(((MainWindow)this.MainWindow != null))
            {
                if(((MainWindow)this.MainWindow).CollectionTCPEthernetThread != null)
                {
                    foreach (EthernetThread client in ((MainWindow)this.MainWindow).CollectionTCPEthernetThread)
                    {
                        if (client.TcpClient != null)
                        {
                            client.TcpClient.Close();                        
                        }
                    }
                }

                if (((MainWindow)this.MainWindow).CollectionUDPEthernetThread != null)
                {
                    foreach (EthernetThread client in ((MainWindow)this.MainWindow).CollectionUDPEthernetThread)
                    {
                        if (client.UdpClient != null)
                        {
                            client.UdpClient.Close();
                        }
                    }
                }

                if (((MainWindow)this.MainWindow).CollectionSerialPortThread != null)
                {
                    foreach (SerialPort serialPort in ((MainWindow)this.MainWindow).CollectionSerialPortThread)
                    {
                        if (serialPort != null)
                        {
                            serialPort.Close();
                        }
                    }
                }

                if (((MainWindow)this.MainWindow).CollectionSQLThread != null)
                {
                    foreach (SQLThread connection in ((MainWindow)this.MainWindow).CollectionSQLThread)
                    {
                        if (connection.SQL != null)
                        {
                            connection.SQL.Close();
                            connection.SQL.Dispose();
                        }
                    }
                }
            }         
        }         
    }
}

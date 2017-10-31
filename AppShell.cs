using Microsoft.VisualBasic.ApplicationServices;
using Npgsql;
using SCADA;
using System.IO.Ports;
using System.Net.Sockets;



class SingleInstanceAppShell : WindowsFormsApplicationBase
{
 
    public SingleInstanceAppShell()
    {
        this.IsSingleInstance = true;
    }

    public SCADA.AppWPF AppWPF
    {
        get;
        private set;
    }

    protected override bool OnStartup(StartupEventArgs eventArgs)
    {
        AppWPF = new SCADA.AppWPF();
        AppWPF.InitializeComponent();
        AppWPF.Run();
        return false;
    }

    [System.STAThreadAttribute()]
    public static void Main(string[] args)
    {
        SingleInstanceAppShell appShell = null;

        try
        {
            appShell = new SingleInstanceAppShell();
            appShell.Run(args);   
        }
        catch(System.Exception ex)
        {
            if (((MainWindow)appShell.AppWPF.MainWindow != null))
            {
                if (((MainWindow)appShell.AppWPF.MainWindow).CollectionTCPEthernetThread != null)
                {
                    foreach (TcpClient client in ((MainWindow)appShell.AppWPF.MainWindow).CollectionTCPEthernetThread)
                    {
                        if (client != null)
                        {
                            client.Close();
                        }
                    }
                }

                if (((MainWindow)appShell.AppWPF.MainWindow).CollectionUDPEthernetThread != null)
                {
                    foreach (UdpClient client in ((MainWindow)appShell.AppWPF.MainWindow).CollectionUDPEthernetThread)
                    {
                        if (client != null)
                        {
                            client.Close();
                        }
                    }
                }

                if (((MainWindow)appShell.AppWPF.MainWindow).CollectionSerialPortThread != null)
                {
                    foreach (SerialPort serialPort in ((MainWindow)appShell.AppWPF.MainWindow).CollectionSerialPortThread)
                    {
                        if (serialPort != null)
                        {
                            serialPort.Close();
                        }
                    }
                }

                if (((MainWindow)appShell.AppWPF.MainWindow).CollectionSQLThread != null)
                {
                    foreach (NpgsqlConnection connection in ((MainWindow)appShell.AppWPF.MainWindow).CollectionSQLThread)
                    {
                        if (connection != null)
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
            }

            System.Windows.MessageBox.Show(ex.Message);
        }                    
    }
}
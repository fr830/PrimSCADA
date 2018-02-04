﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

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
                    foreach (EthernetThread client in ((MainWindow)appShell.AppWPF.MainWindow).CollectionTCPEthernetThread)
                    {
                        if (client.TcpClient != null)
                        {
                            client.TcpClient.Close();
                        }
                    }
                }

                if (((MainWindow)appShell.AppWPF.MainWindow).CollectionUDPEthernetThread != null)
                {
                    foreach (EthernetThread client in ((MainWindow)appShell.AppWPF.MainWindow).CollectionUDPEthernetThread)
                    {
                        if (client.UdpClient != null)
                        {
                            client.UdpClient.Close();
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
                    foreach (SQLThread connection in ((MainWindow)appShell.AppWPF.MainWindow).CollectionSQLThread)
                    {
                        if (connection.SQL != null)
                        {
                            connection.SQL.Close();
                            connection.SQL.Dispose();
                        }
                    }
                }
            }

            System.Windows.MessageBox.Show(ex.Message);
        }                    
    }
}
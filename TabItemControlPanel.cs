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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SCADA
{    
    public class TabItemControlPanel : TabItemParent
    {
        private ControlPanelScada cpS;
        public ControlPanelScada CPS
        {
            get { return cpS; }
            set { IS = value; cpS = value; }
        }

        private CanvasControlPanel canvasControlPanel;
        public CanvasControlPanel CanvasControlPanel
        {
            get { return canvasControlPanel; }
            set { CanvasTab = value; canvasControlPanel = value; }
        }

        static TabItemControlPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItemControlPanel), new FrameworkPropertyMetadata(typeof(TabItem)));
        }

        public TabItemControlPanel(ControlPanelScada cps)
        {
            CPS = cps;
            CanvasControlPanel canvasControlPanel = new CanvasControlPanel(this.CPS, this);
            CanvasControlPanel = canvasControlPanel;

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            Image imageClose = new Image();
            imageClose.Source = new BitmapImage(new Uri("Images/Close16.png", UriKind.Relative));

            Button close = new Button();
            close.Tag = this;
            close.Click += CloseTabItem;
            close.ToolTip = "Закрыть";
            close.Content = imageClose;
            close.Height = 16;
            close.Width = 16;

            Label lPageName = new Label();
            lPageName.Content = CPS.Name;

            ScrollViewer scroll = new ScrollViewer();
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.SetValue(Grid.RowProperty, 0);

            Image insertImage = new Image();
            insertImage.Source = new BitmapImage(new Uri("Images/Insert16.ico", UriKind.Relative));

            Binding BindingInsert = new Binding();
            BindingInsert.Source = mainWindow;
            BindingInsert.Path = new PropertyPath("IsBindingInsertControlPanel");
            BindingInsert.Mode = BindingMode.OneWay;

            MenuItem menuItemInsert = new MenuItem();
            menuItemInsert.SetBinding(MenuItem.IsEnabledProperty, BindingInsert);
            menuItemInsert.Icon = insertImage;
            menuItemInsert.Click += canvasControlPanel.Insert;
            menuItemInsert.Header = "Вставить";

            ContextMenu contextMenuCanvas = new ContextMenu();
            contextMenuCanvas.Items.Add(menuItemInsert);

            canvasControlPanel.ContextMenu = contextMenuCanvas;

            StackPanel panelTabItem = new StackPanel();
            panelTabItem.ToolTip = CPS.Path;
            panelTabItem.Orientation = Orientation.Horizontal;
            panelTabItem.Children.Add(lPageName);
            panelTabItem.Children.Add(close);

            this.Header = panelTabItem;
            this.Content = scroll;
            scroll.Content = canvasControlPanel;

            ((AppWPF)Application.Current).CollectionTabItemParent.Add(CPS.Path, this);

            ControlPanel controlPanel = ((AppWPF)Application.Current).CollectionControlPanel[CPS.Path];

            foreach (EthernetSer ethernetSer in controlPanel.CollectionEthernet)
            {
                EthernetControl ethernetControl = new EthernetControl(CPS, CanvasControlPanel, ethernetSer);

                ((AppWPF)Application.Current).CollectionEthernetSers.Add(ethernetSer);

                ethernetControl.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    ethernetControl.PathFigureBorder.StartPoint = ethernetSer.Border.point[0];
                    ethernetControl.PolyLineSegmentBorder.Points[0] = ethernetSer.Border.point[1];
                    ethernetControl.PolyLineSegmentBorder.Points[1] = ethernetSer.Border.point[2];
                    ethernetControl.PolyLineSegmentBorder.Points[2] = ethernetSer.Border.point[3];
                    ethernetControl.PolyLineSegmentBorder.Points[3] = ethernetSer.Border.point[4];

                    ethernetControl.PathFigureDownSize.StartPoint = ethernetSer.DownSize.point[0];
                    ethernetControl.LineSegmentDownSize.Point = ethernetSer.DownSize.point[1];

                    ethernetControl.PathFigureLeftSize.StartPoint = ethernetSer.LeftSize.point[0];
                    ethernetControl.LineSegmentLeftSize.Point = ethernetSer.LeftSize.point[1];

                    ethernetControl.PathFigureRightSize.StartPoint = ethernetSer.RightSize.point[0];
                    ethernetControl.LineSegmentRightSize.Point = ethernetSer.RightSize.point[1];

                    ethernetControl.PathFigureTopSize.StartPoint = ethernetSer.TopSize.point[0];
                    ethernetControl.LineSegmentTopSize.Point = ethernetSer.TopSize.point[1];
                }));                

                ethernetControl.SetValue(Canvas.LeftProperty, ethernetSer.Сoordinates.X);
                ethernetControl.SetValue(Canvas.TopProperty, ethernetSer.Сoordinates.Y);

                canvasControlPanel.Children.Add(ethernetControl);

                ethernetControl.ApplyTemplate();            

                foreach (EthernetOperational eo in ethernetControl.EthernetSer.CollectionEthernetOperational)
                {
                    eo.EthernetOperationalSearch.BufferSizeRec = eo.BufferSizeRec;
                    eo.EthernetOperationalSearch.BufferSizeSend = eo.BufferSizeSend;
                    eo.EthernetOperationalSearch.Description = eo.Description;
                }
            }
            foreach (ComSer comSer in controlPanel.CollectionCom)
            {
                ComControl comControl = new ComControl(CPS, CanvasControlPanel, comSer);

                ((AppWPF)Application.Current).CollectionComSers.Add(comSer);

                comControl.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    comControl.PathFigureBorder.StartPoint = comSer.Border.point[0];
                    comControl.PolyLineSegmentBorder.Points[0] = comSer.Border.point[1];
                    comControl.PolyLineSegmentBorder.Points[1] = comSer.Border.point[2];
                    comControl.PolyLineSegmentBorder.Points[2] = comSer.Border.point[3];
                    comControl.PolyLineSegmentBorder.Points[3] = comSer.Border.point[4];

                    comControl.PathFigureDownSize.StartPoint = comSer.DownSize.point[0];
                    comControl.LineSegmentDownSize.Point = comSer.DownSize.point[1];

                    comControl.PathFigureLeftSize.StartPoint = comSer.LeftSize.point[0];
                    comControl.LineSegmentLeftSize.Point = comSer.LeftSize.point[1];

                    comControl.PathFigureRightSize.StartPoint = comSer.RightSize.point[0];
                    comControl.LineSegmentRightSize.Point = comSer.RightSize.point[1];

                    comControl.PathFigureTopSize.StartPoint = comSer.TopSize.point[0];
                    comControl.LineSegmentTopSize.Point = comSer.TopSize.point[1];
                }));

                comControl.SetValue(Canvas.LeftProperty, comSer.Сoordinates.X);
                comControl.SetValue(Canvas.TopProperty, comSer.Сoordinates.Y);

                canvasControlPanel.Children.Add(comControl);

                comControl.ApplyTemplate();
            }
            foreach (ModbusSer modbusSer in controlPanel.CollectionModbus)
            {
                ModbusControl modbusControl = new ModbusControl(CPS, CanvasControlPanel, modbusSer);

                ((AppWPF)Application.Current).CollectionModbusSers.Add(modbusSer);

                modbusControl.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    modbusControl.PathFigureBorder.StartPoint = modbusSer.Border.point[0];
                    modbusControl.PolyLineSegmentBorder.Points[0] = modbusSer.Border.point[1];
                    modbusControl.PolyLineSegmentBorder.Points[1] = modbusSer.Border.point[2];
                    modbusControl.PolyLineSegmentBorder.Points[2] = modbusSer.Border.point[3];
                    modbusControl.PolyLineSegmentBorder.Points[3] = modbusSer.Border.point[4];

                    modbusControl.PathFigureDownSize.StartPoint = modbusSer.DownSize.point[0];
                    modbusControl.LineSegmentDownSize.Point = modbusSer.DownSize.point[1];

                    modbusControl.PathFigureLeftSize.StartPoint = modbusSer.LeftSize.point[0];
                    modbusControl.LineSegmentLeftSize.Point = modbusSer.LeftSize.point[1];

                    modbusControl.PathFigureRightSize.StartPoint = modbusSer.RightSize.point[0];
                    modbusControl.LineSegmentRightSize.Point = modbusSer.RightSize.point[1];

                    modbusControl.PathFigureTopSize.StartPoint = modbusSer.TopSize.point[0];
                    modbusControl.LineSegmentTopSize.Point = modbusSer.TopSize.point[1];
                }));

                modbusControl.SetValue(Canvas.LeftProperty, modbusSer.Сoordinates.X);
                modbusControl.SetValue(Canvas.TopProperty, modbusSer.Сoordinates.Y);

                canvasControlPanel.Children.Add(modbusControl);

                modbusControl.ApplyTemplate();                
            }
        }

        public override void DeleteTabItem()
        {
            Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

            if (((MainWindow)MainWindow).TabControlMain.Items.IndexOf(this) >= 0)
                ((MainWindow)MainWindow).TabControlMain.Items.Remove(this);

            ((AppWPF)Application.Current).CollectionTabItemParent.Remove(this.IS.Path);
            ((AppWPF)Application.Current).CollectionControlPanel.Remove(this.IS.Path);
            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Remove(this.IS.Path);
            ((AppWPF)Application.Current).CollectionSaveTabItem.Remove(this);
        }
    }
}

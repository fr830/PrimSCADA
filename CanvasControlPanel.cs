using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SCADA
{
    public class CanvasControlPanel : CanvasTab
    {
        private ControlPanelScada cpS;
        public ControlPanelScada CPS
        {
            get { return cpS; }
            set { IS = value; cpS = value; }
        }

        private TabItemControlPanel tabItemControlPanel;
        public TabItemControlPanel TabItemControlPanel
        {
            get { return tabItemControlPanel; }
            set { TabItemParent = value; tabItemControlPanel = value; }
        }

        public CanvasControlPanel(ControlPanelScada cps, TabItemControlPanel tabItemControlPanel)
        {           
            CPS = cps;
            TabItemControlPanel = tabItemControlPanel;
            
            this.Background = new SolidColorBrush(Colors.White);
            this.AllowDrop = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            DragAndDropCanvas dragItem = (DragAndDropCanvas)e.Data.GetData(typeof(DragAndDropCanvas));

            if (dragItem.IsEthernet)
            {
                Point p = new Point();
                p = e.GetPosition(this);

                EthernetSer ethernetSer = new EthernetSer(this.Children.Count + 1, 0, p);

                ControlPanel controlPanel = ((AppWPF)Application.Current).CollectionControlPanel[this.cpS.Path];
                controlPanel.CollectionEthernet.Add(ethernetSer);

                EthernetControl ethernetControl = new EthernetControl(this.CPS, this, ethernetSer);
                ethernetControl.SetValue(Canvas.TopProperty, p.Y);
                ethernetControl.SetValue(Canvas.LeftProperty, p.X);

                this.Children.Add(ethernetControl);

                ((AppWPF)Application.Current).SaveTabItem(this.TabItemControlPanel);

                ((AppWPF)Application.Current).CollectionEthernetSers.Add(ethernetSer);

                Mouse.OverrideCursor = null;
            }
            else if (dragItem.IsCom)
            {
                Point p = new Point();
                p = e.GetPosition(this);

                ComSer comSer = new ComSer(this.Children.Count + 1, 0, p);

                ControlPanel controlPanel = ((AppWPF)Application.Current).CollectionControlPanel[this.cpS.Path];
                controlPanel.CollectionCom.Add(comSer);

                ComControl comControl = new ComControl(this.CPS, this, comSer);
                comControl.SetValue(Canvas.TopProperty, p.Y);
                comControl.SetValue(Canvas.LeftProperty, p.X);

                this.Children.Add(comControl);

                ((AppWPF)Application.Current).SaveTabItem(this.TabItemControlPanel);

                ((AppWPF)Application.Current).CollectionComSers.Add(comSer);

                Mouse.OverrideCursor = null;
            }
            else if (dragItem.IsModbus)
            {
                Point p = new Point();
                p = e.GetPosition(this);

                ModbusSer modbusSer = new ModbusSer(this.Children.Count + 1, 0, p);

                ControlPanel controlPanel = ((AppWPF)Application.Current).CollectionControlPanel[this.cpS.Path];
                controlPanel.CollectionModbus.Add(modbusSer);

                ModbusControl modbusControl = new ModbusControl(this.CPS, this, modbusSer);
                modbusControl.SetValue(Canvas.TopProperty, p.Y);
                modbusControl.SetValue(Canvas.LeftProperty, p.X);

                this.Children.Add(modbusControl);

                ((AppWPF)Application.Current).SaveTabItem(this.TabItemControlPanel);

                ((AppWPF)Application.Current).CollectionModbusSers.Add(modbusSer);

                Mouse.OverrideCursor = null;
            }
            
            e.Handled = true;
        }

        public void Insert(object sender, RoutedEventArgs e)
        {
            ControlPanel controlPanel = ((AppWPF)Application.Current).CollectionControlPanel[this.CPS.Path];

            MainWindow mainWindow = (MainWindow)((AppWPF)System.Windows.Application.Current).MainWindow;

            IDataObject iData = Clipboard.GetDataObject();
            ClipboardManipulation clipboardManipulation = (ClipboardManipulation)iData.GetData("SCADA.ClipboardManipulation");

            double x = 0, y = 0;
            double relativelyX = 0, relativelyY = 0;
            double comparerRelativelyX = 0, comparerRelativelyY = 0;
            int countRelatively = 0;

            ControlOnCanvasControlPanel copyControlOnCanvas = null;
            EthernetControl cutEthernetControl = null;
            ComControl cutComControl = null;
            CanvasControlPanel cutCanvas = null;

            ControlPanelScada cps = this.CPS;

            switch (clipboardManipulation.Manipulation)
            {
                case 3:

                    foreach (ControlOnCanvas controlOnCanvas in this.SelectedControlOnCanvas)
                    {
                        controlOnCanvas.IsSelected = false;
                        controlOnCanvas.border.Pen.Brush.Opacity = 0;
                    }

                    this.SelectedControlOnCanvas.Clear();

                    if (mainWindow.CurrentObjects.Count == 1)
                    {                       
                        #region CopyEthernetControl
                        if (mainWindow.CurrentObjects[0] is EthernetControl)
                        {
                            EthernetControl ethernetControl = mainWindow.CurrentObjects[0] as EthernetControl;

                            EthernetSer curEthernetSer = ethernetControl.EthernetSer;

                            EthernetSer copyEthernetSer;

                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                XamlWriter.Save(curEthernetSer, TempStream);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    copyEthernetSer = (EthernetSer)XamlReader.Load(TempStreamRead);
                                }
                            }

                            EthernetControl copyEthernetControl = new EthernetControl(this.CPS, this, copyEthernetSer);
                            copyControlOnCanvas = copyEthernetControl;

                            copyEthernetSer.Сoordinates = Mouse.GetPosition(this);

                            copyEthernetControl.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                copyEthernetControl.PathFigureLeftSize.StartPoint = copyEthernetSer.LeftSize.point[0];
                                copyEthernetControl.LineSegmentLeftSize.Point = copyEthernetSer.LeftSize.point[1];

                                copyEthernetControl.PathFigureRightSize.StartPoint = copyEthernetSer.RightSize.point[0];
                                copyEthernetControl.LineSegmentRightSize.Point = copyEthernetSer.RightSize.point[1];

                                copyEthernetControl.PathFigureTopSize.StartPoint = copyEthernetSer.TopSize.point[0];
                                copyEthernetControl.LineSegmentTopSize.Point = copyEthernetSer.TopSize.point[1];

                                copyEthernetControl.PathFigureDownSize.StartPoint = copyEthernetSer.DownSize.point[0];
                                copyEthernetControl.LineSegmentDownSize.Point = copyEthernetSer.DownSize.point[1];

                                copyEthernetControl.PathFigureBorder.StartPoint = copyEthernetSer.Border.point[0];
                                copyEthernetControl.PolyLineSegmentBorder.Points[0] = copyEthernetSer.Border.point[1];
                                copyEthernetControl.PolyLineSegmentBorder.Points[1] = copyEthernetSer.Border.point[2];
                                copyEthernetControl.PolyLineSegmentBorder.Points[2] = copyEthernetSer.Border.point[3];
                                copyEthernetControl.PolyLineSegmentBorder.Points[3] = copyEthernetSer.Border.point[4];

                                copyEthernetControl.border.Pen.Brush.Opacity = 100;
                            }));

                            controlPanel.CollectionEthernet.Add(copyEthernetSer);

                            ((AppWPF)Application.Current).CollectionEthernetSers.Add(copyEthernetSer);
                        }
                        #endregion

                        #region CopyComControl
                        if (mainWindow.CurrentObjects[0] is ComControl)
                        {
                            ComControl comControl = mainWindow.CurrentObjects[0] as ComControl;

                            ComSer curComSer = comControl.ComSer;

                            ComSer copyComSer;

                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                XamlWriter.Save(curComSer, TempStream);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    copyComSer = (ComSer)XamlReader.Load(TempStreamRead);
                                }
                            }

                            ComControl copyComControl = new ComControl(this.CPS, this, copyComSer);
                            copyControlOnCanvas = copyComControl;

                            copyComSer.Сoordinates = Mouse.GetPosition(this);

                            copyComControl.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                copyComControl.PathFigureLeftSize.StartPoint = copyComSer.LeftSize.point[0];
                                copyComControl.LineSegmentLeftSize.Point = copyComSer.LeftSize.point[1];

                                copyComControl.PathFigureRightSize.StartPoint = copyComSer.RightSize.point[0];
                                copyComControl.LineSegmentRightSize.Point = copyComSer.RightSize.point[1];

                                copyComControl.PathFigureTopSize.StartPoint = copyComSer.TopSize.point[0];
                                copyComControl.LineSegmentTopSize.Point = copyComSer.TopSize.point[1];

                                copyComControl.PathFigureDownSize.StartPoint = copyComSer.DownSize.point[0];
                                copyComControl.LineSegmentDownSize.Point = copyComSer.DownSize.point[1];

                                copyComControl.PathFigureBorder.StartPoint = copyComSer.Border.point[0];
                                copyComControl.PolyLineSegmentBorder.Points[0] = copyComSer.Border.point[1];
                                copyComControl.PolyLineSegmentBorder.Points[1] = copyComSer.Border.point[2];
                                copyComControl.PolyLineSegmentBorder.Points[2] = copyComSer.Border.point[3];
                                copyComControl.PolyLineSegmentBorder.Points[3] = copyComSer.Border.point[4];

                                copyComControl.border.Pen.Brush.Opacity = 100;
                            }));

                            controlPanel.CollectionCom.Add(copyComSer);

                            ((AppWPF)Application.Current).CollectionComSers.Add(copyComSer);
                        }
                        #endregion

                        copyControlOnCanvas.IsSelected = true;

                        copyControlOnCanvas.SetValue(Canvas.LeftProperty, copyControlOnCanvas.controlOnCanvasSer.Сoordinates.X);
                        copyControlOnCanvas.SetValue(Canvas.TopProperty, copyControlOnCanvas.controlOnCanvasSer.Сoordinates.Y);

                        this.Children.Add(copyControlOnCanvas);

                        copyControlOnCanvas.ZIndex = this.Children.Count;

                        copyControlOnCanvas.ApplyTemplate();

                        this.SelectedControlOnCanvas.Add(copyControlOnCanvas);
                    }
                    else if (mainWindow.CurrentObjects.Count > 1)
                    {
                        #region CopyObjects

                        CollectionsEthernet curEthernetSers = new CollectionsEthernet();

                        CollectionsCom curComSers = new CollectionsCom();

                        foreach (ControlOnCanvas controlOnCanvas in mainWindow.CurrentObjects)
                        {
                            if (controlOnCanvas.controlOnCanvasSer.Transform == 0)
                            {
                                relativelyX = (double)controlOnCanvas.GetValue(Canvas.LeftProperty);
                                relativelyY = (double)controlOnCanvas.GetValue(Canvas.TopProperty);
                            }
                            else if (controlOnCanvas.controlOnCanvasSer.Transform == -90 || controlOnCanvas.controlOnCanvasSer.Transform == 270)
                            {
                                relativelyY = (double)controlOnCanvas.GetValue(Canvas.TopProperty) - controlOnCanvas.ActualWidth;
                                relativelyX = (double)controlOnCanvas.GetValue(Canvas.LeftProperty);
                            }
                            else if (controlOnCanvas.controlOnCanvasSer.Transform == -180 || controlOnCanvas.controlOnCanvasSer.Transform == 180)
                            {
                                relativelyY = (double)controlOnCanvas.GetValue(Canvas.TopProperty) - controlOnCanvas.ActualHeight;
                                relativelyX = (double)controlOnCanvas.GetValue(Canvas.LeftProperty) - controlOnCanvas.ActualWidth;
                            }
                            else if (controlOnCanvas.controlOnCanvasSer.Transform == -270 || controlOnCanvas.controlOnCanvasSer.Transform == 90)
                            {
                                relativelyY = (double)controlOnCanvas.GetValue(Canvas.TopProperty);
                                relativelyX = (double)controlOnCanvas.GetValue(Canvas.LeftProperty) - controlOnCanvas.ActualHeight;
                            }

                            countRelatively += 1;

                            if (countRelatively > 1)
                            {
                                if (relativelyX < comparerRelativelyX) comparerRelativelyX = relativelyX;
                                if (relativelyY < comparerRelativelyY) comparerRelativelyY = relativelyY;
                            }
                            else
                            {
                                comparerRelativelyX = relativelyX;
                                comparerRelativelyY = relativelyY;
                            }

                            if (controlOnCanvas is EthernetControl)
                            {
                                curEthernetSers.Add(((EthernetControl)controlOnCanvas).EthernetSer);
                            }
                            else if (controlOnCanvas is ComControl)
                            {
                                curComSers.Add(((ComControl)controlOnCanvas).ComSer);
                            }
                        }

                        List<EthernetSer> copyEthernetSers = new List<EthernetSer>();
                        List<ComSer> copyComSers = new List<ComSer>();
                        List<ControlOnCanvasSer> copyControlsOnCanvasSer = new List<ControlOnCanvasSer>();                      

                        if (curEthernetSers.Count != 0)
                        {
                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                XamlWriter.Save(curEthernetSers, TempStream);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    copyEthernetSers = (CollectionsEthernet)XamlReader.Load(TempStreamRead);
                                }
                            }
                        }
                        if (curComSers.Count != 0)
                        {
                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                XamlWriter.Save(curComSers, TempStream);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    copyComSers = (CollectionsCom)XamlReader.Load(TempStreamRead);
                                }
                            }
                        }

                        copyControlsOnCanvasSer.AddRange(copyEthernetSers);
                        copyControlsOnCanvasSer.AddRange(copyComSers);
                        copyControlsOnCanvasSer.Sort();

                        x = comparerRelativelyX - Mouse.GetPosition(this).X;
                        y = comparerRelativelyY - Mouse.GetPosition(this).Y;

                        foreach (ControlOnCanvasSer copyControlOnCanvasSer in copyControlsOnCanvasSer)
                        {
                            copyControlOnCanvasSer.Сoordinates = new Point(copyControlOnCanvasSer.Сoordinates.X - x, copyControlOnCanvasSer.Сoordinates.Y - y);

                            if (copyControlOnCanvasSer is EthernetSer)
                            {
                                EthernetSer copyEthernetSer = copyControlOnCanvasSer as EthernetSer;
                                EthernetControl copyEthernetControl = new EthernetControl(this.CPS, this, copyEthernetSer);
                                copyControlOnCanvas = copyEthernetControl;

                                copyEthernetControl.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    copyEthernetControl.PathFigureLeftSize.StartPoint = copyEthernetSer.LeftSize.point[0];
                                    copyEthernetControl.LineSegmentLeftSize.Point = copyEthernetSer.LeftSize.point[1];

                                    copyEthernetControl.PathFigureRightSize.StartPoint = copyEthernetSer.RightSize.point[0];
                                    copyEthernetControl.LineSegmentRightSize.Point = copyEthernetSer.RightSize.point[1];

                                    copyEthernetControl.PathFigureTopSize.StartPoint = copyEthernetSer.TopSize.point[0];
                                    copyEthernetControl.LineSegmentTopSize.Point = copyEthernetSer.TopSize.point[1];

                                    copyEthernetControl.PathFigureDownSize.StartPoint = copyEthernetSer.DownSize.point[0];
                                    copyEthernetControl.LineSegmentDownSize.Point = copyEthernetSer.DownSize.point[1];

                                    copyEthernetControl.PathFigureBorder.StartPoint = copyEthernetSer.Border.point[0];
                                    copyEthernetControl.PolyLineSegmentBorder.Points[0] = copyEthernetSer.Border.point[1];
                                    copyEthernetControl.PolyLineSegmentBorder.Points[1] = copyEthernetSer.Border.point[2];
                                    copyEthernetControl.PolyLineSegmentBorder.Points[2] = copyEthernetSer.Border.point[3];
                                    copyEthernetControl.PolyLineSegmentBorder.Points[3] = copyEthernetSer.Border.point[4];

                                    copyEthernetControl.border.Pen.Brush.Opacity = 100;
                                }));

                                controlPanel.CollectionEthernet.Add(copyEthernetSer);

                                ((AppWPF)Application.Current).CollectionEthernetSers.Add(copyEthernetSer);
                            }
                            else if (copyControlOnCanvasSer is ComSer)
                            {
                                ComSer copyComSer = copyControlOnCanvasSer as ComSer;
                                ComControl copyComControl = new ComControl(this.CPS, this, copyComSer);
                                copyControlOnCanvas = copyComControl;

                                copyComControl.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    copyComControl.PathFigureLeftSize.StartPoint = copyComSer.LeftSize.point[0];
                                    copyComControl.LineSegmentLeftSize.Point = copyComSer.LeftSize.point[1];

                                    copyComControl.PathFigureRightSize.StartPoint = copyComSer.RightSize.point[0];
                                    copyComControl.LineSegmentRightSize.Point = copyComSer.RightSize.point[1];

                                    copyComControl.PathFigureTopSize.StartPoint = copyComSer.TopSize.point[0];
                                    copyComControl.LineSegmentTopSize.Point = copyComSer.TopSize.point[1];

                                    copyComControl.PathFigureDownSize.StartPoint = copyComSer.DownSize.point[0];
                                    copyComControl.LineSegmentDownSize.Point = copyComSer.DownSize.point[1];

                                    copyComControl.PathFigureBorder.StartPoint = copyComSer.Border.point[0];
                                    copyComControl.PolyLineSegmentBorder.Points[0] = copyComSer.Border.point[1];
                                    copyComControl.PolyLineSegmentBorder.Points[1] = copyComSer.Border.point[2];
                                    copyComControl.PolyLineSegmentBorder.Points[2] = copyComSer.Border.point[3];
                                    copyComControl.PolyLineSegmentBorder.Points[3] = copyComSer.Border.point[4];

                                    copyComControl.border.Pen.Brush.Opacity = 100;
                                }));

                                controlPanel.CollectionCom.Add(copyComSer);

                                ((AppWPF)Application.Current).CollectionComSers.Add(copyComSer);
                            }
                            #endregion
                            copyControlOnCanvas.IsSelected = true;

                            copyControlOnCanvas.SetValue(Canvas.LeftProperty, copyControlOnCanvasSer.Сoordinates.X);
                            copyControlOnCanvas.SetValue(Canvas.TopProperty, copyControlOnCanvasSer.Сoordinates.Y);

                            this.Children.Add(copyControlOnCanvas);

                            copyControlOnCanvas.ZIndex = this.Children.Count;

                            copyControlOnCanvas.ApplyTemplate();

                            this.SelectedControlOnCanvas.Add(copyControlOnCanvas);
                        }
                    }

                    this.CountSelect = mainWindow.CurrentObjects.Count;

                    break;

                case 4:

                    if (mainWindow.CurrentObjects[0].IS.Path != this.CPS.Path)
                    {
                        cutCanvas = (CanvasControlPanel)mainWindow.CurrentObjects[0].CanvasTab;
                        mainWindow.CurrentObjects[0].CanvasTab.CountSelect = 0;

                        foreach (ControlOnCanvas controlOnCanvas in this.SelectedControlOnCanvas)
                        {
                            controlOnCanvas.IsSelected = false;
                            controlOnCanvas.border.Pen.Brush.Opacity = 0;
                        }

                        ((AppWPF)Application.Current).SaveTabItem(mainWindow.CurrentObjects[0].CanvasTab.TabItemParent);

                        cutCanvas.SelectedControlOnCanvas.Clear();
                        this.SelectedControlOnCanvas.Clear();
                    }
                    else
                    {
                        // Если вырезанный объект вставляется в тот же канвас ничего не происходит
                        Clipboard.Clear();
                        e.Handled = true;
                        return;
                    }

                    if (mainWindow.CurrentObjects.Count == 1)
                    {
                        #region CutObject

                        if (mainWindow.CurrentObjects[0] is ControlOnCanvasControlPanel)
                        {
                            if (mainWindow.CurrentObjects[0] is EthernetControl)
                            {
                                EthernetControl currentEthernetControl = mainWindow.CurrentObjects[0] as EthernetControl;

                                EthernetSer curEthernetSer = currentEthernetControl.EthernetSer;

                                cutEthernetControl = currentEthernetControl;
                                cutEthernetControl.CanvasControlPanel.Children.Remove(cutEthernetControl);

                                ControlPanel cutControlPanel = ((AppWPF)Application.Current).CollectionControlPanel[cutEthernetControl.CPS.Path];
                                cutControlPanel.CollectionEthernet.Remove(curEthernetSer);

                                controlPanel.CollectionEthernet.Add(curEthernetSer);
                            }
                            else if (mainWindow.CurrentObjects[0] is ComControl)
                            {
                                ComControl currentComControl = mainWindow.CurrentObjects[0] as ComControl;

                                ComSer curComSer = currentComControl.ComSer;

                                cutComControl = currentComControl;
                                cutComControl.CanvasControlPanel.Children.Remove(cutComControl);

                                ControlPanel cutControlPanel = ((AppWPF)Application.Current).CollectionControlPanel[cutComControl.CPS.Path];
                                cutControlPanel.CollectionCom.Remove(curComSer);

                                controlPanel.CollectionCom.Add(curComSer);
                            }
                        }

                        foreach (ControlOnCanvas controlOnCanvas in cutCanvas.Children)
                        {
                            if (controlOnCanvas.ZIndex > mainWindow.CurrentObjects[0].ZIndex)
                                controlOnCanvas.ZIndex -= 1;
                        }

                        mainWindow.CurrentObjects[0].IsSelected = true;
                        mainWindow.CurrentObjects[0].border.Pen.Brush.Opacity = 100;

                        this.Children.Add(mainWindow.CurrentObjects[0]);

                        mainWindow.CurrentObjects[0].IS = this.CPS;
                        mainWindow.CurrentObjects[0].CanvasTab = this;

                        mainWindow.CurrentObjects[0].controlOnCanvasSer.Сoordinates = Mouse.GetPosition(this);

                        mainWindow.CurrentObjects[0].SetValue(Canvas.LeftProperty, mainWindow.CurrentObjects[0].controlOnCanvasSer.Сoordinates.X);
                        mainWindow.CurrentObjects[0].SetValue(Canvas.TopProperty, mainWindow.CurrentObjects[0].controlOnCanvasSer.Сoordinates.Y);

                        mainWindow.CurrentObjects[0].ZIndex = this.Children.Count;

                        this.SelectedControlOnCanvas.Add(mainWindow.CurrentObjects[0]);

                        #endregion
                    }
                    else if (mainWindow.CurrentObjects.Count > 1)
                    {
                        #region CutObjects

                        List<EthernetControl> cutEthernetControls = new List<EthernetControl>();
                        List<ComControl> cutComControls = new List<ComControl>();
                        List<ControlOnCanvas> cutControlsOnCanvas = new List<ControlOnCanvas>();

                        foreach (ControlOnCanvas controlOnCanvas in mainWindow.CurrentObjects)
                        {
                            if (controlOnCanvas.controlOnCanvasSer.Transform == 0)
                            {
                                relativelyX = (double)controlOnCanvas.GetValue(Canvas.LeftProperty);
                                relativelyY = (double)controlOnCanvas.GetValue(Canvas.TopProperty);
                            }
                            else if (controlOnCanvas.controlOnCanvasSer.Transform == -90 || controlOnCanvas.controlOnCanvasSer.Transform == 270)
                            {
                                relativelyY = (double)controlOnCanvas.GetValue(Canvas.TopProperty) - controlOnCanvas.ActualWidth;
                                relativelyX = (double)controlOnCanvas.GetValue(Canvas.LeftProperty);
                            }
                            else if (controlOnCanvas.controlOnCanvasSer.Transform == -180 || controlOnCanvas.controlOnCanvasSer.Transform == 180)
                            {
                                relativelyY = (double)controlOnCanvas.GetValue(Canvas.TopProperty) - controlOnCanvas.ActualHeight;
                                relativelyX = (double)controlOnCanvas.GetValue(Canvas.LeftProperty) - controlOnCanvas.ActualWidth;
                            }
                            else if (controlOnCanvas.controlOnCanvasSer.Transform == -270 || controlOnCanvas.controlOnCanvasSer.Transform == 90)
                            {
                                relativelyY = (double)controlOnCanvas.GetValue(Canvas.TopProperty);
                                relativelyX = (double)controlOnCanvas.GetValue(Canvas.LeftProperty) - controlOnCanvas.ActualHeight;
                            }

                            countRelatively += 1;

                            if (countRelatively > 1)
                            {
                                if (relativelyX < comparerRelativelyX) comparerRelativelyX = relativelyX;
                                if (relativelyY < comparerRelativelyY) comparerRelativelyY = relativelyY;
                            }
                            else
                            {
                                comparerRelativelyX = relativelyX;
                                comparerRelativelyY = relativelyY;
                            }

                            if (controlOnCanvas is EthernetControl)
                            {
                                cutEthernetControls.Add((EthernetControl)controlOnCanvas);
                            }
                            else if (controlOnCanvas is ComControl)
                            {
                                cutComControls.Add((ComControl)controlOnCanvas);
                            } 
                        }

                        cutControlsOnCanvas.AddRange(cutEthernetControls);
                        cutControlsOnCanvas.AddRange(cutComControls);
                        cutControlsOnCanvas.Sort();

                        x = comparerRelativelyX - Mouse.GetPosition(this).X;
                        y = comparerRelativelyY - Mouse.GetPosition(this).Y;

                        foreach (ControlOnCanvas controlOnCanvas in cutControlsOnCanvas)
                        {                          
                            if (controlOnCanvas is EthernetControl)
                            {
                                cutEthernetControl = controlOnCanvas as EthernetControl;
                                cutEthernetControl.EthernetSer.Сoordinates = new Point(cutEthernetControl.EthernetSer.Сoordinates.X - x, cutEthernetControl.EthernetSer.Сoordinates.Y - y);

                                cutEthernetControl.CanvasControlPanel.Children.Remove(cutEthernetControl);

                                ControlPanel cutControlPanel = ((AppWPF)Application.Current).CollectionControlPanel[cutEthernetControl.CPS.Path];
                                cutControlPanel.CollectionEthernet.Remove(cutEthernetControl.EthernetSer);

                                cutEthernetControl.CPS = this.CPS;
                                cutEthernetControl.CanvasControlPanel = this;

                                controlPanel.CollectionEthernet.Add(cutEthernetControl.EthernetSer);
                            }
                            else if (controlOnCanvas is ComControl)
                            {
                                cutComControl = controlOnCanvas as ComControl;
                                cutComControl.ComSer.Сoordinates = new Point(cutComControl.ComSer.Сoordinates.X - x, cutComControl.ComSer.Сoordinates.Y - y);

                                cutComControl.CanvasControlPanel.Children.Remove(cutComControl);

                                ControlPanel cutControlPanel = ((AppWPF)Application.Current).CollectionControlPanel[cutComControl.CPS.Path];
                                cutControlPanel.CollectionCom.Remove(cutComControl.ComSer);

                                cutComControl.CPS = this.CPS;
                                cutComControl.CanvasControlPanel = this;

                                controlPanel.CollectionCom.Add(cutComControl.ComSer);
                            }

                            this.Children.Add(controlOnCanvas);
                                                       
                            controlOnCanvas.SetValue(Canvas.LeftProperty, controlOnCanvas.controlOnCanvasSer.Сoordinates.X);
                            controlOnCanvas.SetValue(Canvas.TopProperty, controlOnCanvas.controlOnCanvasSer.Сoordinates.Y);

                            controlOnCanvas.IsSelected = true;
                            controlOnCanvas.border.Pen.Brush.Opacity = 100;

                            controlOnCanvas.ZIndex = this.Children.Count;

                            this.SelectedControlOnCanvas.Add(controlOnCanvas);
                        }

                        List<ControlOnCanvas> collectionControl = new List<ControlOnCanvas>();

                        foreach (ControlOnCanvas controlOnCanvas in cutCanvas.Children)
                        {
                            collectionControl.Add(controlOnCanvas);
                        }

                        collectionControl.Sort();

                        int count = 0;
                        foreach (ControlOnCanvas controlOnCanvas in collectionControl)
                        {
                            count += 1;
                            controlOnCanvas.ZIndex = count;
                        }

                        #endregion
                    }

                    this.CountSelect = mainWindow.CurrentObjects.Count;
                    Clipboard.Clear();

                    break;
            }

            if (this.CountSelect > 1)
            {
                mainWindow.CoordinateObjectX.IsReadOnly = true;
                mainWindow.CoordinateObjectY.IsReadOnly = true;
                mainWindow.CoordinateObjectX.Text = null;
                mainWindow.CoordinateObjectY.Text = null;

                mainWindow.LabelSelected.Content = "Выделенно объектов: " + this.CountSelect;
            }
            else if (this.CountSelect == 1)
            {
                mainWindow.LabelSelected.Content = "Выделенно объектов: " + this.CountSelect;

                foreach (ControlOnCanvasControlPanel controlOnCanvasControlPanel in this.Children)
                {
                    if (controlOnCanvasControlPanel.IsSelected)
                    {
                        mainWindow.CoordinateObjectX.IsReadOnly = false;
                        mainWindow.CoordinateObjectY.IsReadOnly = false;
                        if (controlOnCanvasControlPanel.controlOnCanvasSer.Transform == 0)
                        {
                            controlOnCanvasControlPanel.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvasControlPanel.GetValue(Canvas.LeftProperty));
                            controlOnCanvasControlPanel.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvasControlPanel.GetValue(Canvas.TopProperty));
                        }
                        else if (controlOnCanvasControlPanel.controlOnCanvasSer.Transform == -90 || controlOnCanvasControlPanel.controlOnCanvasSer.Transform == 270)
                        {
                            controlOnCanvasControlPanel.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvasControlPanel.GetValue(Canvas.TopProperty) - controlOnCanvasControlPanel.ActualWidth);
                            controlOnCanvasControlPanel.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvasControlPanel.GetValue(Canvas.LeftProperty));
                        }
                        else if (controlOnCanvasControlPanel.controlOnCanvasSer.Transform == -180 || controlOnCanvasControlPanel.controlOnCanvasSer.Transform == 180)
                        {
                            controlOnCanvasControlPanel.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvasControlPanel.GetValue(Canvas.TopProperty) - controlOnCanvasControlPanel.ActualHeight);
                            controlOnCanvasControlPanel.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvasControlPanel.GetValue(Canvas.LeftProperty) - controlOnCanvasControlPanel.ActualWidth);
                        }
                        else if (controlOnCanvasControlPanel.controlOnCanvasSer.Transform == -270 || controlOnCanvasControlPanel.controlOnCanvasSer.Transform == 90)
                        {
                            controlOnCanvasControlPanel.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvasControlPanel.GetValue(Canvas.TopProperty));
                            controlOnCanvasControlPanel.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvasControlPanel.GetValue(Canvas.LeftProperty) - controlOnCanvasControlPanel.ActualHeight);
                        }
                    }
                }
            }

            this.RepositionAllObjects(this);
            this.InvalidateMeasure();

            ((AppWPF)Application.Current).SaveTabItem(TabItemControlPanel);

            e.Handled = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!((MainWindow)Application.Current.MainWindow).IsBindingStartProject)
            {
                if (Keyboard.Modifiers != ModifierKeys.Control)
                {
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                    this.CountSelect = 0;

                    foreach (ControlOnCanvas objectOnCanvas in this.SelectedControlOnCanvas)
                    {
                        objectOnCanvas.IsSelected = false;
                        objectOnCanvas.border.Pen.Brush.Opacity = 0;
                    }

                    mainWindow.LabelSelected.Content = "Выделенно объектов: " + CountSelect;
                    mainWindow.TextBoxDiameter.Text = null;
                    mainWindow.CoordinateObjectX.Text = null;
                    mainWindow.CoordinateObjectY.Text = null;
                    mainWindow.ComboBoxEnvironment.SelectedIndex = -1;

                    mainWindow.TextBoxDiameter.IsReadOnly = true;
                    mainWindow.CoordinateObjectX.IsReadOnly = true;
                    mainWindow.CoordinateObjectY.IsReadOnly = true;
                    mainWindow.ComboBoxEnvironment.IsEnabled = false;
                }

                Children.Add(SelectedRectangle);

                Canvas.SetZIndex(SelectedRectangle, this.Children.Count);

                SelectedRectangle.SetValue(Canvas.LeftProperty, e.GetPosition(this).X);
                SelectedRectangle.SetValue(Canvas.TopProperty, e.GetPosition(this).Y);

                Delta = e.GetPosition(Application.Current.MainWindow);

                ComparePoint = e.GetPosition(this);

                SelectedControlOnCanvas.Clear();

                this.CaptureMouse();
            }
           
            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            this.ReleaseMouseCapture();

            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Point coordinate = e.GetPosition(this);

            ((MainWindow)Application.Current.MainWindow).LabelCoordinateCursor.Content = string.Format("{0}{1:F2}{2}{3}{4:F2}", "X: ", coordinate.X, " ", "Y: ", coordinate.Y);

            if (this.IsMouseCaptured)
            {
                Delta.X = Delta.X - e.GetPosition(Application.Current.MainWindow).X;
                Delta.Y = Delta.Y - e.GetPosition(Application.Current.MainWindow).Y;

                // Если при выделении ширина -0 инвертируем 
                if ((SelectedRectangle.Width - Delta.X) < 0)
                {
                    IsNegativeSelectX = true;
                }

                // Если при выделении инвертируемого ширина -0 инвертируем обратно
                if ((SelectedRectangle.Width + Delta.X) < 0)
                {
                    IsNegativeSelectX = false;
                }

                if (IsNegativeSelectX)
                {
                    double x = (double)SelectedRectangle.GetValue(Canvas.LeftProperty);
                    x -= Delta.X;

                    SelectedRectangle.SetValue(Canvas.LeftProperty, x);
                    SelectedRectangle.Width += Delta.X;
                }
                else
                {
                    SelectedRectangle.Width -= Delta.X;
                }

                if ((SelectedRectangle.Height - Delta.Y) < 0)
                {
                    IsNegativeSelectY = true;
                }

                if ((SelectedRectangle.Height + Delta.Y) < 0)
                {
                    IsNegativeSelectY = false;
                }

                if (IsNegativeSelectY)
                {
                    double y = (double)SelectedRectangle.GetValue(Canvas.TopProperty);
                    y -= Delta.Y;

                    SelectedRectangle.SetValue(Canvas.TopProperty, y);
                    SelectedRectangle.Height += Delta.Y;
                }
                else
                {
                    SelectedRectangle.Height -= Delta.Y;
                }
            }

            Delta = e.GetPosition(Application.Current.MainWindow);

            e.Handled = true;
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);

            RectangleGeometry rectangleGeometry = new RectangleGeometry(new Rect(Canvas.GetLeft(SelectedRectangle), Canvas.GetTop(SelectedRectangle), SelectedRectangle.Width, SelectedRectangle.Height));
            GeometryHitTestParameters GeometryHit = new GeometryHitTestParameters(rectangleGeometry);

            VisualTreeHelper.HitTest(this, null, new HitTestResultCallback(MyHitTestResultCallback), GeometryHit);

            foreach (ControlOnCanvas hitControl in hitResultsList)
            {
                hitControl.AreaSelect();
            }

            if (hitResultsList.Count != 0)
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                if (CountSelect == 1)
                {                   
                    mainWindow.CoordinateObjectX.IsReadOnly = false;
                    mainWindow.CoordinateObjectY.IsReadOnly = false;

                    ControlOnCanvas controlOnCanvas = hitResultsList[0] as ControlOnCanvas;

                    if (controlOnCanvas.controlOnCanvasSer.Transform == 0)
                    {
                        mainWindow.CoordinateObjectX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty));
                        mainWindow.CoordinateObjectY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty));
                    }
                    else if (controlOnCanvas.controlOnCanvasSer.Transform == -90 || controlOnCanvas.controlOnCanvasSer.Transform == 270)
                    {
                        mainWindow.CoordinateObjectY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty) - this.ActualWidth);
                        mainWindow.CoordinateObjectX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty));
                    }
                    else if (controlOnCanvas.controlOnCanvasSer.Transform == -180 || controlOnCanvas.controlOnCanvasSer.Transform == 180)
                    {
                        mainWindow.CoordinateObjectY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty) - this.ActualHeight);
                        mainWindow.CoordinateObjectX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty) - this.ActualWidth);
                    }
                    else if (controlOnCanvas.controlOnCanvasSer.Transform == -270 || controlOnCanvas.controlOnCanvasSer.Transform == 90)
                    {
                        mainWindow.CoordinateObjectY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty));
                        mainWindow.CoordinateObjectX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty) - this.ActualHeight);
                    }
                }
                else if (CountSelect > 1)
                {
                    mainWindow.CoordinateObjectX.IsReadOnly = true;
                    mainWindow.CoordinateObjectY.IsReadOnly = true;
                    mainWindow.CoordinateObjectX.Text = null;
                    mainWindow.CoordinateObjectY.Text = null;                   
                }
            }

            hitResultsList.Clear();

            SelectedRectangle.Height = 0;
            SelectedRectangle.Width = 0;

            IsNegativeSelectX = false;
            IsNegativeSelectY = false;

            Children.Remove(SelectedRectangle);

            e.Handled = true;
        }             
    }
}

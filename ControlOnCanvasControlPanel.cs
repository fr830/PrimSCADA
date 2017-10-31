// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCADA
{
    public class ControlOnCanvasControlPanel : ControlOnCanvas
    {
        private CanvasControlPanel canvasControlPanel;
        public CanvasControlPanel CanvasControlPanel
        {
            get { return canvasControlPanel; }
            set { CanvasTab = value; canvasControlPanel = value; }
        }
        private ControlPanelScada cpS;
        public ControlPanelScada CPS
        {
            get { return cpS; }
            set { IS = value; cpS = value; }
        }

        protected ControlOnCanvasControlPanel(ControlOnCanvasSer ser)
            : base(ser)
        {
            ser.ControlItem = this;
        }

        public override void DeleteItem(ControlOnCanvas objectOnCanvas)
        {
            ControlPanel controlPanel = ((AppWPF)Application.Current).CollectionControlPanel[objectOnCanvas.IS.Path];         

            if (objectOnCanvas is EthernetControl)
            {
                EthernetControl ethernetControl = objectOnCanvas as EthernetControl;
                controlPanel.CollectionEthernet.Remove(ethernetControl.EthernetSer);
                ((AppWPF)Application.Current).CollectionEthernetSers.Remove(((EthernetControl)objectOnCanvas).EthernetSer);
            }
            else if (objectOnCanvas is ComControl)
            {
                ComControl comControl = objectOnCanvas as ComControl;
                controlPanel.CollectionCom.Remove(comControl.ComSer);
                ((AppWPF)Application.Current).CollectionComSers.Remove(((ComControl)objectOnCanvas).ComSer);
            }
            else if (objectOnCanvas is ModbusControl)
            {
                ModbusControl modbusControl = objectOnCanvas as ModbusControl;
                controlPanel.CollectionModbus.Remove(modbusControl.ModbusSer);
                ((AppWPF)Application.Current).CollectionModbusSers.Remove(modbusControl.ModbusSer);
            }

            objectOnCanvas.CanvasTab.Children.Remove(objectOnCanvas);
        }

        public override void SelectOne()
        {
            if (this.CanvasControlPanel.CountSelect == 1 && !this.IsSelected && Keyboard.Modifiers != ModifierKeys.Control)
            {
                ControlOnCanvas controlOnCanvas = CanvasControlPanel.SelectedControlOnCanvas[0];
                controlOnCanvas.IsSelected = false;
                controlOnCanvas.border.Pen.Brush.Opacity = 0;

                this.IsSelected = true;
                this.border.Pen.Brush.Opacity = 100;
                this.CanvasControlPanel.CountSelect = 1;

                CanvasControlPanel.SelectedControlOnCanvas[0] = this;
               
                CoordinateX.IsReadOnly = false;
                CoordinateY.IsReadOnly = false;

                if (controlOnCanvasSer.Transform == 0)
                {
                    CoordinateX.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.LeftProperty));
                    CoordinateY.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.TopProperty));
                }
                else if (controlOnCanvasSer.Transform == -90 || controlOnCanvasSer.Transform == 270)
                {
                    CoordinateY.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.TopProperty) - this.ActualWidth);
                    CoordinateX.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.LeftProperty));
                }
                else if (controlOnCanvasSer.Transform == -180 || controlOnCanvasSer.Transform == 180)
                {
                    CoordinateY.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.TopProperty) - this.ActualHeight);
                    CoordinateX.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.LeftProperty) - this.ActualWidth);
                }
                else if (controlOnCanvasSer.Transform == -270 || controlOnCanvasSer.Transform == 90)
                {
                    CoordinateY.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.TopProperty));
                    CoordinateX.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.LeftProperty) - this.ActualHeight);
                }
            }
            else if (this.CanvasControlPanel.CountSelect == 1 && !this.IsSelected && Keyboard.Modifiers == ModifierKeys.Control)
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                this.IsSelected = true;
                this.border.Pen.Brush.Opacity = 100;
                this.CanvasControlPanel.CountSelect += 1;

                CanvasControlPanel.SelectedControlOnCanvas.Add(this);
                           
                CoordinateX.IsReadOnly = true;
                CoordinateY.IsReadOnly = true;
                CoordinateX.Text = null;
                CoordinateY.Text = null;
            }
            else if (Keyboard.Modifiers != ModifierKeys.Control && this.IsSelected)
            {
                return;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && this.IsSelected)
            {
                this.IsSelected = false;
                this.border.Pen.Brush.Opacity = 0;
                this.CanvasControlPanel.CountSelect = 0;

                CanvasControlPanel.SelectedControlOnCanvas.Remove(this);

                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;               

                CoordinateX.IsReadOnly = true;
                CoordinateY.IsReadOnly = true;
                CoordinateX.Text = null;
                CoordinateY.Text = null;

            }
            else
            {
                this.IsSelected = true;
                this.border.Pen.Brush.Opacity = 100;
                this.CanvasControlPanel.CountSelect = 1;

                CanvasControlPanel.SelectedControlOnCanvas.Add(this);
                
                CoordinateX.IsReadOnly = false;
                CoordinateY.IsReadOnly = false;

                if (controlOnCanvasSer.Transform == 0)
                {
                    CoordinateX.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.LeftProperty));
                    CoordinateY.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.TopProperty));
                }
                else if (controlOnCanvasSer.Transform == -90 || controlOnCanvasSer.Transform == 270)
                {
                    CoordinateY.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.TopProperty) - this.ActualWidth);
                    CoordinateX.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.LeftProperty));
                }
                else if (controlOnCanvasSer.Transform == -180 || controlOnCanvasSer.Transform == 180)
                {
                    CoordinateY.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.TopProperty) - this.ActualHeight);
                    CoordinateX.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.LeftProperty) - this.ActualWidth);
                }
                else if (controlOnCanvasSer.Transform == -270 || controlOnCanvasSer.Transform == 90)
                {
                    CoordinateY.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.TopProperty));
                    CoordinateX.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.LeftProperty) - this.ActualHeight);
                }
            }

            LabelSelected.Content = "Выделенно объектов: " + this.CanvasControlPanel.CountSelect;
        }

        public override void Select()
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && this.IsSelected)
            {
                this.IsSelected = false;
                this.border.Pen.Brush.Opacity = 0;
                this.CanvasControlPanel.CountSelect -= 1;

                CanvasControlPanel.SelectedControlOnCanvas.Remove(this);

                LabelSelected.Content = "Выделенно объектов: " + this.CanvasControlPanel.CountSelect;

                if (this.CanvasControlPanel.CountSelect == 1)
                {
                    CoordinateX.IsReadOnly = false;
                    CoordinateY.IsReadOnly = false;

                    foreach (ControlOnCanvasControlPanel controlOnCanvas in this.CanvasControlPanel.SelectedControlOnCanvas)
                    {                       
                        if (controlOnCanvasSer.Transform == 0)
                        {
                            CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty));
                            CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty));
                        }
                        else if (controlOnCanvasSer.Transform == -90 || controlOnCanvasSer.Transform == 270)
                        {
                            CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty) - this.ActualWidth);
                            CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty));
                        }
                        else if (controlOnCanvasSer.Transform == -180 || controlOnCanvasSer.Transform == 180)
                        {
                            CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty) - this.ActualHeight);
                            CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty) - this.ActualWidth);
                        }
                        else if (controlOnCanvasSer.Transform == -270 || controlOnCanvasSer.Transform == 90)
                        {
                            CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty));
                            CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty) - this.ActualHeight);
                        }
                    }
                }                

                return;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && !this.IsSelected)
            {
                this.IsSelected = true;
                this.border.Pen.Brush.Opacity = 100;
                this.CanvasControlPanel.CountSelect += 1;

                CanvasControlPanel.SelectedControlOnCanvas.Add(this);

                LabelSelected.Content = "Выделенно объектов: " + this.CanvasControlPanel.CountSelect;                              

                return;
            }
            else if (Keyboard.Modifiers != ModifierKeys.Control && !this.IsSelected)
            {
                foreach (ControlOnCanvasControlPanel controlOnCanvas in this.CanvasControlPanel.SelectedControlOnCanvas)
                {
                    controlOnCanvas.IsSelected = false;
                    controlOnCanvas.border.Pen.Brush.Opacity = 0;
                }

                this.IsSelected = true;
                this.border.Pen.Brush.Opacity = 100;
                this.CanvasControlPanel.CountSelect = 1;

                CanvasControlPanel.SelectedControlOnCanvas.Clear();
                CanvasControlPanel.SelectedControlOnCanvas.Add(this);

                LabelSelected.Content = "Выделенно объектов: " + this.CanvasControlPanel.CountSelect;
              
                CoordinateX.IsReadOnly = false;
                CoordinateY.IsReadOnly = false;

                if (controlOnCanvasSer.Transform == 0)
                {
                    CoordinateX.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.LeftProperty));
                    CoordinateY.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.TopProperty));
                }
                else if (controlOnCanvasSer.Transform == -90 || controlOnCanvasSer.Transform == 270)
                {
                    CoordinateY.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.TopProperty) - this.ActualWidth);
                    CoordinateX.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.LeftProperty));
                }
                else if (controlOnCanvasSer.Transform == -180 || controlOnCanvasSer.Transform == 180)
                {
                    CoordinateY.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.TopProperty) - this.ActualHeight);
                    CoordinateX.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.LeftProperty) - this.ActualWidth);
                }
                else if (controlOnCanvasSer.Transform == -270 || controlOnCanvasSer.Transform == 90)
                {
                    CoordinateY.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.TopProperty));
                    CoordinateX.Text = string.Format("{0:F2}", (double)this.GetValue(Canvas.LeftProperty) - this.ActualHeight);
                }
            }
        }
    }
}

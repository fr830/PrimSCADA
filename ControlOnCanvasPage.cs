using System;
using System.Collections.Generic;
using System.IO;
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

namespace SCADA
{
    public class ControlOnCanvasPage : ControlOnCanvas
    {
        static ControlOnCanvasPage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ControlOnCanvasPage), new FrameworkPropertyMetadata(typeof(ControlOnCanvasPage)));
        }

        protected ControlOnCanvasPage(ControlOnCanvasSer ser)
            : base(ser)
        {
            ser.ControlItem = this;
        }

        private CanvasPage canvasPage;
        public CanvasPage CanvasPage
        {
            get { return canvasPage; }
            set { CanvasTab = value; canvasPage = value; }
        }
        private PageScada pS;
        public PageScada PS
        {
            get { return pS; }
            set { IS = value; pS = value; }
        }
                      
        public override void DeleteItem(ControlOnCanvas objectOnCanvas)
        {
            Page page = ((AppWPF)Application.Current).CollectionPage[objectOnCanvas.IS.Path];

            if (objectOnCanvas is Pipe)
            {
                Pipe pipe = objectOnCanvas as Pipe;
                page.CollectionPipe.Remove(pipe.PipeSer);
            }
            else if (objectOnCanvas is Pipe90)
            {
                Pipe90 pipe90 = objectOnCanvas as Pipe90;
                page.CollectionPipe90.Remove(pipe90.Pipe90Ser);
            }
            else if (objectOnCanvas is Text)
            {
                Text text = objectOnCanvas as Text;
                page.CollectionText.Remove(text.TextSer);
            }
            else if (objectOnCanvas is Display)
            {
                Display display = objectOnCanvas as Display;
                page.CollectionDisplay.Remove(display.DisplaySer);
            }
            else if(objectOnCanvas is ImageControl)
            {
                ImageControl imageControl = objectOnCanvas as ImageControl;
                page.CollectionImage.Remove(imageControl.ImageSer);
            }

            objectOnCanvas.CanvasTab.Children.Remove(objectOnCanvas);     
        }
              
        public override void SelectOne()
        {
            if (this.CanvasPage.CountSelect == 1 && !this.IsSelected && Keyboard.Modifiers != ModifierKeys.Control)
            {
                ControlOnCanvas controlOnCanvas = CanvasPage.SelectedControlOnCanvas[0];              
                controlOnCanvas.IsSelected = false;
                controlOnCanvas.border.Pen.Brush.Opacity = 0;
                
                this.IsSelected = true;
                this.border.Pen.Brush.Opacity = 100;
                this.CanvasPage.CountSelect = 1;

                CanvasPage.SelectedControlOnCanvas[0] = this;

                if (this is PipeOnCanvas)
                {                  
                    PipeOnCanvas pipeOnCanvas = this as PipeOnCanvas;
                    pipeOnCanvas.TextBoxDiameter.IsReadOnly = false;
                    pipeOnCanvas.ComboBoxEnvironment.IsEnabled = true;
                    pipeOnCanvas.TextBoxDiameter.Text = string.Format("{0:F2}", pipeOnCanvas.Diameter);                                     
                    pipeOnCanvas.ComboBoxEnvironment.SelectedIndex = pipeOnCanvas.IntEnvironment;
                }

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
            else if (this.CanvasPage.CountSelect == 1 && !this.IsSelected && Keyboard.Modifiers == ModifierKeys.Control)
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                this.IsSelected = true;
                this.border.Pen.Brush.Opacity = 100;
                this.CanvasPage.CountSelect += 1;

                CanvasPage.SelectedControlOnCanvas.Add(this);

                PipeOnCanvas pipeOld = null;
                PipeOnCanvas pipe = null;
                bool falseComparer = false;
                int countPipe = 0;

                foreach (ControlOnCanvasPage controlOnCanvas in CanvasPage.SelectedControlOnCanvas)
                {                              
                   if (controlOnCanvas is PipeOnCanvas)
                   {
                       countPipe += 1;

                       if (!falseComparer)
                       {
                           if (countPipe > 1)
                           {
                               pipe = controlOnCanvas as PipeOnCanvas;

                               if (Math.Round(pipeOld.Diameter, 2, MidpointRounding.AwayFromZero) != Math.Round(pipe.Diameter, 2, MidpointRounding.AwayFromZero))
                               {
                                   pipe.TextBoxDiameter.Text = "-";
                               }
                               else
                               {
                                   pipe.TextBoxDiameter.Text = string.Format("{0:F2}", pipe.Diameter);
                               }
                               if (pipeOld.IntEnvironment != pipe.IntEnvironment)
                               {
                                   pipe.ComboBoxEnvironment.SelectedIndex = -1;
                               }
                               else
                               {
                                   pipe.ComboBoxEnvironment.SelectedIndex = pipe.IntEnvironment;
                               }

                               if (Math.Round(pipeOld.Diameter, 2, MidpointRounding.AwayFromZero) != Math.Round(pipe.Diameter, 2, MidpointRounding.AwayFromZero))
                               {
                                   falseComparer = true;
                               }
                               else if (pipeOld.IntEnvironment != pipe.IntEnvironment)
                               {
                                   falseComparer = true;
                               }
                           }
                       }
                                                                               
                       pipeOld = (PipeOnCanvas)controlOnCanvas;
                   }                                       
                }

                if (countPipe == 1)
                {
                    mainWindow.TextBoxDiameter.IsReadOnly = false;
                    mainWindow.ComboBoxEnvironment.IsEnabled = true;
                    mainWindow.TextBoxDiameter.Text = string.Format("{0:F2}", pipeOld.Diameter);
                    mainWindow.ComboBoxEnvironment.SelectedIndex = pipeOld.IntEnvironment;
                }
                
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
                this.CanvasPage.CountSelect = 0;

                CanvasPage.SelectedControlOnCanvas.Remove(this);

                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                if (this is PipeOnCanvas)
                {
                    mainWindow.TextBoxDiameter.IsReadOnly = true;
                    mainWindow.ComboBoxEnvironment.IsEnabled = false;
                    mainWindow.ComboBoxEnvironment.SelectedIndex = -1;
                    mainWindow.TextBoxDiameter.Text = null;
                }
                                                             
                CoordinateX.IsReadOnly = true;
                CoordinateY.IsReadOnly = true;
                CoordinateX.Text = null;
                CoordinateY.Text = null;
                
            }
            else
            {
                this.IsSelected = true;
                this.border.Pen.Brush.Opacity = 100;
                this.CanvasPage.CountSelect = 1;

                CanvasPage.SelectedControlOnCanvas.Add(this);
               
                if (this is PipeOnCanvas)
                {
                    PipeOnCanvas pipeOnCanvas = this as PipeOnCanvas;
                    pipeOnCanvas.TextBoxDiameter.IsReadOnly = false;
                    pipeOnCanvas.ComboBoxEnvironment.IsEnabled = true;
                    pipeOnCanvas.TextBoxDiameter.Text = string.Format("{0:F2}", pipeOnCanvas.Diameter);                   
                    pipeOnCanvas.ComboBoxEnvironment.SelectedIndex = pipeOnCanvas.IntEnvironment;
                }

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

            LabelSelected.Content = "Выделенно объектов: " + this.CanvasPage.CountSelect;
        }

        public override void Select()
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && this.IsSelected)
            {
                this.IsSelected = false;
                this.border.Pen.Brush.Opacity = 0;
                this.CanvasPage.CountSelect -= 1;

                CanvasPage.SelectedControlOnCanvas.Remove(this);

                LabelSelected.Content = "Выделенно объектов: " + this.CanvasPage.CountSelect;

                PipeOnCanvas pipeOnCanvas = null;

                if (this.CanvasPage.CountSelect == 1)
                {
                    CoordinateX.IsReadOnly = false;
                    CoordinateY.IsReadOnly = false;

                    foreach (ControlOnCanvasPage controlOnCanvas in this.CanvasPage.SelectedControlOnCanvas)
                    {
                        if (controlOnCanvas is PipeOnCanvas)
                        {
                            pipeOnCanvas = controlOnCanvas as PipeOnCanvas;
                            pipeOnCanvas.TextBoxDiameter.Text = string.Format("{0:F2}", pipeOnCanvas.Diameter);
                            pipeOnCanvas.ComboBoxEnvironment.SelectedIndex = pipeOnCanvas.IntEnvironment;
                        }     

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
                else 
                {
                    int countPipe = 0;
                    PipeOnCanvas pipe = null;
                    PipeOnCanvas pipeOld = null;
                    bool falseComparer = false;

                    foreach (ControlOnCanvasPage controlOnCanvas in this.CanvasPage.SelectedControlOnCanvas)
                    {
                        if (controlOnCanvas is PipeOnCanvas)
                        {
                            pipe = controlOnCanvas as PipeOnCanvas;

                            countPipe += 1;

                            if (!falseComparer)
                            {
                                if (countPipe > 1)
                                {
                                    pipe = controlOnCanvas as PipeOnCanvas;

                                    if (Math.Round(pipeOld.Diameter, 2, MidpointRounding.AwayFromZero) != Math.Round(pipe.Diameter, 2, MidpointRounding.AwayFromZero))
                                    {
                                        pipe.TextBoxDiameter.Text = "-";
                                    }
                                    else
                                    {
                                        pipe.TextBoxDiameter.Text = string.Format("{0:F2}", pipe.Diameter);
                                    }
                                    if (pipeOld.IntEnvironment != pipe.IntEnvironment)
                                    {
                                        pipe.ComboBoxEnvironment.SelectedIndex = -1;
                                    }
                                    else
                                    {
                                        pipe.ComboBoxEnvironment.SelectedIndex = pipe.IntEnvironment;
                                    }

                                    if (Math.Round(pipeOld.Diameter, 2, MidpointRounding.AwayFromZero) != Math.Round(pipe.Diameter, 2, MidpointRounding.AwayFromZero))
                                    {
                                        falseComparer = true;
                                    }
                                    else if (pipeOld.IntEnvironment != pipe.IntEnvironment)
                                    {
                                        falseComparer = true;
                                    }
                                }
                            }
                            
                            pipeOld = pipe;                             
                        }                        
                    }

                    if (countPipe == 0)
                    {
                        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                        mainWindow.TextBoxDiameter.IsReadOnly = true;
                        mainWindow.ComboBoxEnvironment.IsEnabled = false;
                        mainWindow.ComboBoxEnvironment.SelectedIndex = -1;
                        mainWindow.TextBoxDiameter.Text = null;
                    }
                    else if (countPipe == 1)
                    {
                        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                        mainWindow.TextBoxDiameter.IsReadOnly = false;
                        mainWindow.ComboBoxEnvironment.IsEnabled = true;
                        mainWindow.ComboBoxEnvironment.SelectedIndex = pipe.IntEnvironment;
                        mainWindow.TextBoxDiameter.Text = string.Format("{0:F2}", pipe.Diameter);
                    }
                }
             
                return;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && !this.IsSelected)
            {
                this.IsSelected = true;
                this.border.Pen.Brush.Opacity = 100;
                this.CanvasPage.CountSelect += 1;

                CanvasPage.SelectedControlOnCanvas.Add(this);

                LabelSelected.Content = "Выделенно объектов: " + this.CanvasPage.CountSelect;
              
                int countPipe = 0;
                PipeOnCanvas pipe = null;
                PipeOnCanvas pipeOld = null;
                bool falseComparer = false;

                foreach (ControlOnCanvasPage controlOnCanvas in this.CanvasPage.SelectedControlOnCanvas)
                {
                    if (controlOnCanvas is PipeOnCanvas)
                    {
                        pipe = controlOnCanvas as PipeOnCanvas;
                        countPipe += 1;

                        if (!falseComparer)
                        {
                            if (countPipe > 1)
                            {
                                if (Math.Round(pipeOld.Diameter, 2, MidpointRounding.AwayFromZero) != Math.Round(pipe.Diameter, 2, MidpointRounding.AwayFromZero))
                                {
                                    pipe.TextBoxDiameter.Text = "-";
                                }
                                else
                                {
                                    pipe.TextBoxDiameter.Text = string.Format("{0:F2}", pipe.Diameter);
                                }
                                if (pipeOld.IntEnvironment != pipe.IntEnvironment)
                                {
                                    pipe.ComboBoxEnvironment.SelectedIndex = -1;
                                }
                                else
                                {
                                    pipe.ComboBoxEnvironment.SelectedIndex = pipe.IntEnvironment;
                                }

                                if (Math.Round(pipeOld.Diameter, 2, MidpointRounding.AwayFromZero) != Math.Round(pipe.Diameter, 2, MidpointRounding.AwayFromZero))
                                {
                                    falseComparer = true;
                                }
                                else if (pipeOld.IntEnvironment != pipe.IntEnvironment)
                                {
                                    falseComparer = true;
                                }
                            }
                        }
                       
                        pipeOld = pipe;
                    }                   
                }

                if (countPipe == 0)
                {
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow.TextBoxDiameter.IsReadOnly = true;
                    mainWindow.ComboBoxEnvironment.IsEnabled = false;
                    mainWindow.ComboBoxEnvironment.SelectedIndex = -1;
                    mainWindow.TextBoxDiameter.Text = null;
                }
                else if (countPipe == 1)
                {
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow.TextBoxDiameter.IsReadOnly = false;
                    mainWindow.ComboBoxEnvironment.IsEnabled = true;
                    mainWindow.ComboBoxEnvironment.SelectedIndex = pipe.IntEnvironment;
                    mainWindow.TextBoxDiameter.Text = string.Format("{0:F2}", pipe.Diameter);
                }
                
                return;
            }
            else if (Keyboard.Modifiers != ModifierKeys.Control && !this.IsSelected)
            {
                foreach (ControlOnCanvasPage controlOnCanvas in this.CanvasPage.SelectedControlOnCanvas)
                {
                    controlOnCanvas.IsSelected = false;
                    controlOnCanvas.border.Pen.Brush.Opacity = 0;
                }

                this.IsSelected = true;
                this.border.Pen.Brush.Opacity = 100;
                this.CanvasPage.CountSelect = 1;

                CanvasPage.SelectedControlOnCanvas.Clear();
                CanvasPage.SelectedControlOnCanvas.Add(this);

                LabelSelected.Content = "Выделенно объектов: " + this.CanvasPage.CountSelect;

                if (this is PipeOnCanvas)
                {
                    PipeOnCanvas pipeOnCanvas = this as PipeOnCanvas;
                    pipeOnCanvas.TextBoxDiameter.IsReadOnly = false;
                    pipeOnCanvas.ComboBoxEnvironment.IsEnabled = true;
                    pipeOnCanvas.TextBoxDiameter.Text = string.Format("{0:F2}", pipeOnCanvas.Diameter);
                    pipeOnCanvas.ComboBoxEnvironment.SelectedIndex = pipeOnCanvas.IntEnvironment;
                }

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

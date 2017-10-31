// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SCADA
{
    public class CanvasPage : CanvasTab
    {
        private PageScada pS;
        public PageScada PS
        {
            get { return pS; }
            set { IS = value; pS = value; }
        }

        private TabItemPage tabItemPage;
        public TabItemPage TabItemPage
        {
            get { return tabItemPage; }
            set { TabItemParent = value; tabItemPage = value; }
        }

        public CanvasPage(PageScada ps, TabItemPage tabItemPage)
        {
            PS = ps;
            TabItemPage = tabItemPage;
           
            this.Background = new SolidColorBrush(Colors.White);
            this.AllowDrop = true;
        }
       
        protected override void OnDrop(DragEventArgs e)
        {
 	        base.OnDrop(e);

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            DragAndDropCanvas dragItem = (DragAndDropCanvas)e.Data.GetData(typeof(DragAndDropCanvas));

            if (dragItem.IsPipe)
            {
                Point p = new Point();
                p = e.GetPosition(this);

                PipeSer pipeSer = new PipeSer(this.Children.Count + 1, 0, p);

                Page page = ((AppWPF)Application.Current).CollectionPage[this.pS.Path];
                page.CollectionPipe.Add(pipeSer);

                Pipe pipe = new Pipe(this.PS, this, pipeSer);
                pipe.SetValue(Canvas.TopProperty, p.Y);
                pipe.SetValue(Canvas.LeftProperty, p.X);

                this.Children.Add(pipe);
              
                ((AppWPF)Application.Current).SaveTabItem(this.TabItemPage);
                          
                Mouse.OverrideCursor = null;
            }
            else if (dragItem.IsPipe90)
            {
                Point p = new Point();
                p = e.GetPosition(this);

                Pipe90Ser pipe90Ser = new Pipe90Ser(this.Children.Count + 1, 0, p);

                Page page = ((AppWPF)Application.Current).CollectionPage[this.pS.Path];
                page.CollectionPipe90.Add(pipe90Ser);

                Pipe90 pipe90 = new Pipe90(this.PS, this, pipe90Ser);
                pipe90.SetValue(Canvas.TopProperty, p.Y);
                pipe90.SetValue(Canvas.LeftProperty, p.X);
               
                this.Children.Add(pipe90);

                ((AppWPF)Application.Current).SaveTabItem(this.TabItemPage);

                Mouse.OverrideCursor = null;
            }
            else if (dragItem.IsText)
            {
                Point p = new Point();
                p = e.GetPosition(this);

                TextSer textSer = new TextSer(this.Children.Count + 1, 0, p);

                Page page = ((AppWPF)Application.Current).CollectionPage[this.pS.Path];
                page.CollectionText.Add(textSer);

                Text text = new Text(this.PS, this, textSer);
                text.SetValue(Canvas.TopProperty, p.Y);
                text.SetValue(Canvas.LeftProperty, p.X);

                this.Children.Add(text);

                ((AppWPF)Application.Current).SaveTabItem(this.TabItemPage);
                
                Mouse.OverrideCursor = null;              
            }
            else if (dragItem.IsDisplay)
            {
                Point p = new Point();
                p = e.GetPosition(this);

                DisplaySer displaySer = new DisplaySer(this.Children.Count + 1, 0, p);

                Page page = ((AppWPF)Application.Current).CollectionPage[this.pS.Path];
                page.CollectionDisplay.Add(displaySer);

                Display display = new Display(this.PS, this, displaySer);
                display.SetValue(Canvas.TopProperty, p.Y);
                display.SetValue(Canvas.LeftProperty, p.X);

                this.Children.Add(display);

                ((AppWPF)Application.Current).SaveTabItem(this.TabItemPage);

                Mouse.OverrideCursor = null;
            }
            else if (dragItem.IsImage)
            {
                Point p = new Point();
                p = e.GetPosition(this);

                ImageSer imageSer = new ImageSer(this.Children.Count + 1, 0, p);

                Page page = ((AppWPF)Application.Current).CollectionPage[this.pS.Path];
                page.CollectionImage.Add(imageSer);

                ImageControl imageControl = new ImageControl(this.PS, this, imageSer);
                imageControl.SetValue(Canvas.TopProperty, p.Y);
                imageControl.SetValue(Canvas.LeftProperty, p.X);

                this.Children.Add(imageControl);

                ((AppWPF)Application.Current).SaveTabItem(this.TabItemPage);

                Mouse.OverrideCursor = null;
            }
            
            e.Handled = true;
        }

        public void Insert(object sender, RoutedEventArgs e)
        {
            Page page = ((AppWPF)Application.Current).CollectionPage[this.PS.Path];

            MainWindow mainWindow = (MainWindow)((AppWPF)System.Windows.Application.Current).MainWindow;

            IDataObject iData = Clipboard.GetDataObject();
            ClipboardManipulation clipboardManipulation = (ClipboardManipulation)iData.GetData("SCADA.ClipboardManipulation");

            double x = 0, y = 0;
            double relativelyX = 0, relativelyY = 0;
            double comparerRelativelyX = 0, comparerRelativelyY = 0;
            int countRelatively = 0;

            PipeOnCanvas pipeOnCanvas = null;
            PipeOnCanvas pipeOld = null;
            ControlOnCanvasPage copyControlOnCanvas = null;
            Pipe cutPipe = null;
            Pipe90 cutPipe90 = null;
            Text cutText = null;
            Display cutDisplay = null;
            ImageControl cutImageControl = null;
            int countPipe = 0;
            bool falseComparer = false;
            CanvasPage cutCanvas = null;

            PageScada ps = this.PS;

            switch (clipboardManipulation.Manipulation)
            {
                case 1:

                    foreach (ControlOnCanvas controlOnCanvas in this.SelectedControlOnCanvas)
                    {
                        controlOnCanvas.IsSelected = false;
                        controlOnCanvas.border.Pen.Brush.Opacity = 0;
                    }

                    this.SelectedControlOnCanvas.Clear();

                    if (mainWindow.CurrentObjects.Count == 1)
                    {
                        #region CopyPipe
                        if (mainWindow.CurrentObjects[0] is Pipe)
                        {
                            Pipe currentPipe = mainWindow.CurrentObjects[0] as Pipe;

                            PipeSer curPipeSer = currentPipe.PipeSer;

                            PipeSer copyPipeSer;

                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                BinaryFormatter serializer = new BinaryFormatter();

                                serializer.Serialize(TempStream, curPipeSer);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    BinaryFormatter deserializer = new BinaryFormatter();

                                    copyPipeSer = (PipeSer)deserializer.Deserialize(TempStreamRead);
                                }
                            }
                            
                            Pipe copyPipe = new Pipe(this.PS, this, copyPipeSer);
                            copyControlOnCanvas = copyPipe;

                            copyPipeSer.Сoordinates = Mouse.GetPosition(this);

                            copyPipe.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                copyPipe.PathFigureLeftSize.StartPoint = copyPipeSer.LeftSize.point[0];
                                copyPipe.LineSegmentLeftSize.Point = copyPipeSer.LeftSize.point[1];

                                copyPipe.PathFigureRightSize.StartPoint = copyPipeSer.RightSize.point[0];
                                copyPipe.LineSegmentRightSize.Point = copyPipeSer.RightSize.point[1];

                                copyPipe.PathFigureTopSize.StartPoint = copyPipeSer.TopSize.point[0];
                                copyPipe.LineSegmentTopSize.Point = copyPipeSer.TopSize.point[1];

                                copyPipe.PathFigureDownSize.StartPoint = copyPipeSer.DownSize.point[0];
                                copyPipe.LineSegmentDownSize.Point = copyPipeSer.DownSize.point[1];

                                copyPipe.PathFigureLeftFlange.StartPoint = copyPipeSer.LeftFlange.point[0];
                                copyPipe.PolyLineSegmentLeftFlange.Points[0] = copyPipeSer.LeftFlange.point[1];
                                copyPipe.PolyLineSegmentLeftFlange.Points[1] = copyPipeSer.LeftFlange.point[2];
                                copyPipe.PolyLineSegmentLeftFlange.Points[2] = copyPipeSer.LeftFlange.point[3];
                                copyPipe.PolyLineSegmentLeftFlange.Points[3] = copyPipeSer.LeftFlange.point[4];

                                copyPipe.PathFigureRightFlange.StartPoint = copyPipeSer.RightFlange.point[0];
                                copyPipe.PolyLineSegmentRightFlange.Points[0] = copyPipeSer.RightFlange.point[1];
                                copyPipe.PolyLineSegmentRightFlange.Points[1] = copyPipeSer.RightFlange.point[2];
                                copyPipe.PolyLineSegmentRightFlange.Points[2] = copyPipeSer.RightFlange.point[3];
                                copyPipe.PolyLineSegmentRightFlange.Points[3] = copyPipeSer.RightFlange.point[4];

                                copyPipe.PathFigurePipe.StartPoint = copyPipeSer.Pipe.point[0];
                                copyPipe.PolyLineSegmentPipe.Points[0] = copyPipeSer.Pipe.point[1];
                                copyPipe.PolyLineSegmentPipe.Points[1] = copyPipeSer.Pipe.point[2];
                                copyPipe.PolyLineSegmentPipe.Points[2] = copyPipeSer.Pipe.point[3];
                                copyPipe.PolyLineSegmentPipe.Points[3] = copyPipeSer.Pipe.point[4];

                                copyPipe.PathFigureBorder.StartPoint = copyPipeSer.BorderPipe.point[0];
                                copyPipe.PolyLineSegmentBorder.Points[0] = copyPipeSer.BorderPipe.point[1];
                                copyPipe.PolyLineSegmentBorder.Points[1] = copyPipeSer.BorderPipe.point[2];
                                copyPipe.PolyLineSegmentBorder.Points[2] = copyPipeSer.BorderPipe.point[3];
                                copyPipe.PolyLineSegmentBorder.Points[3] = copyPipeSer.BorderPipe.point[4];

                                copyPipe.border.Pen.Brush.Opacity = 100;

                                copyPipe.Diameter = (copyPipe.PathFigureDownSize.StartPoint.Y - copyPipe.PathFigureTopSize.StartPoint.Y);
                            }));

                            page.CollectionPipe.Add(copyPipeSer);
                        }
                        #endregion
                        #region CopyPipe90
                        else if (mainWindow.CurrentObjects[0] is Pipe90)
                        {
                            Pipe90 currentPipe90 = mainWindow.CurrentObjects[0] as Pipe90;

                            Pipe90Ser curPipe90Ser = currentPipe90.Pipe90Ser;

                            Pipe90Ser copyPipe90Ser;

                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                BinaryFormatter serializer = new BinaryFormatter();

                                serializer.Serialize(TempStream, curPipe90Ser);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    BinaryFormatter deserializer = new BinaryFormatter();

                                    copyPipe90Ser = (Pipe90Ser)deserializer.Deserialize(TempStreamRead);

                                }
                            }

                            Pipe90 copyPipe90 = new Pipe90(this.PS, this, copyPipe90Ser);
                            copyControlOnCanvas = copyPipe90;

                            copyPipe90Ser.Сoordinates = Mouse.GetPosition(this);

                            copyPipe90.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                copyPipe90.PathFigureTopLenghtSize.StartPoint = copyPipe90Ser.TopLenghtSize.point[0];
                                copyPipe90.LineSegmentTopLenghtSize.Point = copyPipe90Ser.TopLenghtSize.point[1];

                                copyPipe90.PathFigureDownLenghtSize.StartPoint = copyPipe90Ser.DownLenghtSize.point[0];
                                copyPipe90.LineSegmentDownLenghtSize.Point = copyPipe90Ser.DownLenghtSize.point[1];

                                copyPipe90.PathFigureTopSize.StartPoint = copyPipe90Ser.TopSize.point[0];
                                copyPipe90.LineSegmentTopSize.Point = copyPipe90Ser.TopSize.point[1];

                                copyPipe90.PathFigureDownSize.StartPoint = copyPipe90Ser.DownSize.point[0];
                                copyPipe90.LineSegmentDownSize.Point = copyPipe90Ser.DownSize.point[1];

                                copyPipe90.PathFigureLeftFlange.StartPoint = copyPipe90Ser.LeftFlange.point[0];
                                copyPipe90.PolyLineSegmentLeftFlange.Points[0] = copyPipe90Ser.LeftFlange.point[1];
                                copyPipe90.PolyLineSegmentLeftFlange.Points[1] = copyPipe90Ser.LeftFlange.point[2];
                                copyPipe90.PolyLineSegmentLeftFlange.Points[2] = copyPipe90Ser.LeftFlange.point[3];
                                copyPipe90.PolyLineSegmentLeftFlange.Points[3] = copyPipe90Ser.LeftFlange.point[4];

                                copyPipe90.PathFigureRightFlange.StartPoint = copyPipe90Ser.RightFlange.point[0];
                                copyPipe90.PolyLineSegmentRightFlange.Points[0] = copyPipe90Ser.RightFlange.point[1];
                                copyPipe90.PolyLineSegmentRightFlange.Points[1] = copyPipe90Ser.RightFlange.point[2];
                                copyPipe90.PolyLineSegmentRightFlange.Points[2] = copyPipe90Ser.RightFlange.point[3];
                                copyPipe90.PolyLineSegmentRightFlange.Points[3] = copyPipe90Ser.RightFlange.point[4];

                                copyPipe90.PathFigureTopImage.StartPoint = copyPipe90Ser.TopImage.point[0];
                                copyPipe90.PolyLineSegmentTopImage.Points[0] = copyPipe90Ser.TopImage.point[1];
                                copyPipe90.PolyLineSegmentTopImage.Points[1] = copyPipe90Ser.TopImage.point[2];
                                copyPipe90.PolyLineSegmentTopImage.Points[2] = copyPipe90Ser.TopImage.point[3];
                                copyPipe90.PolyLineSegmentTopImage.Points[3] = copyPipe90Ser.TopImage.point[4];

                                copyPipe90.PathFigureDownImage.StartPoint = copyPipe90Ser.DownImage.point[0];
                                copyPipe90.PolyLineSegmentDownImage.Points[0] = copyPipe90Ser.DownImage.point[1];
                                copyPipe90.PolyLineSegmentDownImage.Points[1] = copyPipe90Ser.DownImage.point[2];
                                copyPipe90.PolyLineSegmentDownImage.Points[2] = copyPipe90Ser.DownImage.point[3];
                                copyPipe90.PolyLineSegmentDownImage.Points[3] = copyPipe90Ser.DownImage.point[4];

                                copyPipe90.PathFigureLeftDownSize.StartPoint = copyPipe90Ser.LeftDownSize.point[0];
                                copyPipe90.LineSegmentLeftDownSize.Point = copyPipe90Ser.LeftDownSize.point[1];

                                copyPipe90.PathFigureRightDownSize.StartPoint = copyPipe90Ser.RightDownSize.point[0];
                                copyPipe90.LineSegmentRightDownSize.Point = copyPipe90Ser.RightDownSize.point[1];

                                copyPipe90.PathFigureBorder.StartPoint = copyPipe90Ser.BorderPipe90.point[0];
                                copyPipe90.PolyLineSegmentBorder.Points[0] = copyPipe90Ser.BorderPipe90.point[1];
                                copyPipe90.PolyLineSegmentBorder.Points[1] = copyPipe90Ser.BorderPipe90.point[2];
                                copyPipe90.PolyLineSegmentBorder.Points[2] = copyPipe90Ser.BorderPipe90.point[3];
                                copyPipe90.PolyLineSegmentBorder.Points[3] = copyPipe90Ser.BorderPipe90.point[4];

                                copyPipe90.border.Pen.Brush.Opacity = 100;

                                copyPipe90.Diameter = (copyPipe90.PathFigureDownSize.StartPoint.Y - copyPipe90.PathFigureTopSize.StartPoint.Y);
                            }));

                            page.CollectionPipe90.Add(copyPipe90Ser);
                        }
                        #endregion
                        #region CopyText
                        else if (mainWindow.CurrentObjects[0] is Text)
                        {
                            Text text = mainWindow.CurrentObjects[0] as Text;

                            TextSer curTextSer = text.TextSer;

                            TextSer copyTextSer;

                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                XamlWriter.Save(curTextSer, TempStream);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    copyTextSer = (TextSer)XamlReader.Load(TempStreamRead);
                                }
                            }

                            Text copyText = new Text(this.PS, this, copyTextSer);
                            copyControlOnCanvas = copyText;

                            copyTextSer.Сoordinates = Mouse.GetPosition(this);

                            copyText.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                copyText.PathFigureLeftSize.StartPoint = copyTextSer.LeftSize.point[0];
                                copyText.LineSegmentLeftSize.Point = copyTextSer.LeftSize.point[1];

                                copyText.PathFigureRightSize.StartPoint = copyTextSer.RightSize.point[0];
                                copyText.LineSegmentRightSize.Point = copyTextSer.RightSize.point[1];
                               
                                copyText.PathFigureTopSize.StartPoint = copyTextSer.TopSize.point[0];
                                copyText.LineSegmentTopSize.Point = copyTextSer.TopSize.point[1];

                                copyText.PathFigureDownSize.StartPoint = copyTextSer.DownSize.point[0];
                                copyText.LineSegmentDownSize.Point = copyTextSer.DownSize.point[1];
                               
                                copyText.PathFigureBorder.StartPoint = copyTextSer.Border.point[0];
                                copyText.PolyLineSegmentBorder.Points[0] = copyTextSer.Border.point[1];
                                copyText.PolyLineSegmentBorder.Points[1] = copyTextSer.Border.point[2];
                                copyText.PolyLineSegmentBorder.Points[2] = copyTextSer.Border.point[3];
                                copyText.PolyLineSegmentBorder.Points[3] = copyTextSer.Border.point[4];

                                copyText.border.Pen.Brush.Opacity = 100;                                
                            }));

                            page.CollectionText.Add(copyTextSer);
                        }
                        #endregion
                        #region CopyDisplay
                        else if (mainWindow.CurrentObjects[0] is Display)
                        {
                            Display display = mainWindow.CurrentObjects[0] as Display;

                            DisplaySer curDisplaySer = display.DisplaySer;

                            DisplaySer copyDisplaySer;

                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                XamlWriter.Save(curDisplaySer, TempStream);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    copyDisplaySer = (DisplaySer)XamlReader.Load(TempStreamRead);
                                }
                            }

                            Display copyDisplay = new Display(this.PS, this, copyDisplaySer);
                            copyControlOnCanvas = copyDisplay;

                            copyDisplaySer.Сoordinates = Mouse.GetPosition(this);

                            copyDisplay.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                copyDisplay.PathFigureLeftSize.StartPoint = copyDisplaySer.LeftSize.point[0];
                                copyDisplay.LineSegmentLeftSize.Point = copyDisplaySer.LeftSize.point[1];

                                copyDisplay.PathFigureRightSize.StartPoint = copyDisplaySer.RightSize.point[0];
                                copyDisplay.LineSegmentRightSize.Point = copyDisplaySer.RightSize.point[1];

                                copyDisplay.PathFigureTopSize.StartPoint = copyDisplaySer.TopSize.point[0];
                                copyDisplay.LineSegmentTopSize.Point = copyDisplaySer.TopSize.point[1];

                                copyDisplay.PathFigureDownSize.StartPoint = copyDisplaySer.DownSize.point[0];
                                copyDisplay.LineSegmentDownSize.Point = copyDisplaySer.DownSize.point[1];

                                copyDisplay.PathFigureBorder.StartPoint = copyDisplaySer.Border.point[0];
                                copyDisplay.PolyLineSegmentBorder.Points[0] = copyDisplaySer.Border.point[1];
                                copyDisplay.PolyLineSegmentBorder.Points[1] = copyDisplaySer.Border.point[2];
                                copyDisplay.PolyLineSegmentBorder.Points[2] = copyDisplaySer.Border.point[3];
                                copyDisplay.PolyLineSegmentBorder.Points[3] = copyDisplaySer.Border.point[4];

                                copyDisplay.border.Pen.Brush.Opacity = 100;
                            }));

                            page.CollectionDisplay.Add(copyDisplaySer);
                        }
                        #endregion
                        #region CopyImage
                        else if (mainWindow.CurrentObjects[0] is ImageControl)
                        {
                            ImageControl imageControl = mainWindow.CurrentObjects[0] as ImageControl;

                            ImageSer curImageSer = imageControl.ImageSer;

                            ImageSer copyImageSer;

                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                XamlWriter.Save(curImageSer, TempStream);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    copyImageSer = (ImageSer)XamlReader.Load(TempStreamRead);
                                }
                            }

                            ImageControl copyImageControl = new ImageControl(this.PS, this, copyImageSer);
                            copyControlOnCanvas = copyImageControl;

                            copyImageSer.Сoordinates = Mouse.GetPosition(this);

                            copyImageControl.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                copyImageControl.PathFigureLeftSize.StartPoint = copyImageSer.LeftSize.point[0];
                                copyImageControl.LineSegmentLeftSize.Point = copyImageSer.LeftSize.point[1];

                                copyImageControl.PathFigureRightSize.StartPoint = copyImageSer.RightSize.point[0];
                                copyImageControl.LineSegmentRightSize.Point = copyImageSer.RightSize.point[1];

                                copyImageControl.PathFigureTopSize.StartPoint = copyImageSer.TopSize.point[0];
                                copyImageControl.LineSegmentTopSize.Point = copyImageSer.TopSize.point[1];

                                copyImageControl.PathFigureDownSize.StartPoint = copyImageSer.DownSize.point[0];
                                copyImageControl.LineSegmentDownSize.Point = copyImageSer.DownSize.point[1];

                                copyImageControl.PathFigureBorder.StartPoint = copyImageSer.Border.point[0];
                                copyImageControl.PolyLineSegmentBorder.Points[0] = copyImageSer.Border.point[1];
                                copyImageControl.PolyLineSegmentBorder.Points[1] = copyImageSer.Border.point[2];
                                copyImageControl.PolyLineSegmentBorder.Points[2] = copyImageSer.Border.point[3];
                                copyImageControl.PolyLineSegmentBorder.Points[3] = copyImageSer.Border.point[4];

                                copyImageControl.border.Pen.Brush.Opacity = 100;

                            }));

                            page.CollectionImage.Add(copyImageSer);
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

                        List<PipeSer> curPipeSers = new List<PipeSer>();
                        List<Pipe90Ser> curPipe90Sers = new List<Pipe90Ser>();
                        CollectionsText curTextSers = new CollectionsText();
                        CollectionsDisplay curDisplaySers = new CollectionsDisplay();
                        CollectionsImage curImageSers = new CollectionsImage();

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

                            if (controlOnCanvas is Pipe)
                            {
                                curPipeSers.Add(((Pipe)controlOnCanvas).PipeSer);
                            }
                            else if (controlOnCanvas is Pipe90)
                            {
                                curPipe90Sers.Add(((Pipe90)controlOnCanvas).Pipe90Ser);
                            }
                            else if (controlOnCanvas is Text)
                            {
                                curTextSers.Add(((Text)controlOnCanvas).TextSer);
                            }
                            else if (controlOnCanvas is Display)
                            {
                                curDisplaySers.Add(((Display)controlOnCanvas).DisplaySer);
                            }
                            else if(controlOnCanvas is ImageControl)
                            {
                                curImageSers.Add(((ImageControl)controlOnCanvas).ImageSer);
                            }
                        }

                        List<PipeSer> copyPipeSers = new List<PipeSer>();
                        List<Pipe90Ser> copyPipe90Sers = new List<Pipe90Ser>();
                        List<TextSer> copyTextSers = new List<TextSer>();
                        List<DisplaySer> copyDisplaySers = new List<DisplaySer>();
                        List<ImageSer> copyImageSers = new List<ImageSer>();
                        List<ControlOnCanvasSer> copyControlsOnCanvasSer = new List<ControlOnCanvasSer>();

                        if (curPipeSers.Count != 0)
                        {
                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                BinaryFormatter serializer = new BinaryFormatter();

                                serializer.Serialize(TempStream, curPipeSers);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    BinaryFormatter deserializer = new BinaryFormatter();

                                    copyPipeSers = (List<PipeSer>)deserializer.Deserialize(TempStreamRead);
                                }
                            }
                        }

                        if (curPipe90Sers.Count != 0)
                        {
                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                BinaryFormatter serializer = new BinaryFormatter();

                                serializer.Serialize(TempStream, curPipe90Sers);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    BinaryFormatter deserializer = new BinaryFormatter();

                                    copyPipe90Sers = (List<Pipe90Ser>)deserializer.Deserialize(TempStreamRead);
                                }
                            }
                        }

                        if (curTextSers.Count != 0)
                        {
                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                XamlWriter.Save(curTextSers, TempStream);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {                                  
                                    copyTextSers = (List<TextSer>)XamlReader.Load(TempStreamRead);
                                }
                            }
                        }

                        if (curDisplaySers.Count != 0)
                        {
                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                XamlWriter.Save(curDisplaySers, TempStream);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    copyDisplaySers = (List<DisplaySer>)XamlReader.Load(TempStreamRead);
                                }
                            }
                        }

                        if (curImageSers.Count != 0)
                        {
                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                XamlWriter.Save(curImageSers, TempStream);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    copyImageSers = (List<ImageSer>)XamlReader.Load(TempStreamRead);
                                }
                            }
                        }

                        copyControlsOnCanvasSer.AddRange(copyPipeSers);
                        copyControlsOnCanvasSer.AddRange(copyPipe90Sers);
                        copyControlsOnCanvasSer.AddRange(copyTextSers);
                        copyControlsOnCanvasSer.AddRange(copyDisplaySers);
                        copyControlsOnCanvasSer.AddRange(copyImageSers);
                        copyControlsOnCanvasSer.Sort();

                        x = comparerRelativelyX - Mouse.GetPosition(this).X;
                        y = comparerRelativelyY - Mouse.GetPosition(this).Y;

                        foreach (ControlOnCanvasSer copyControlOnCanvasSer in copyControlsOnCanvasSer)
                        {
                            copyControlOnCanvasSer.Сoordinates = new Point(copyControlOnCanvasSer.Сoordinates.X - x, copyControlOnCanvasSer.Сoordinates.Y - y);

                            #region CopyPipes

                            if (copyControlOnCanvasSer is PipeSer)
                            {                               
                                PipeSer copyPipeSer = copyControlOnCanvasSer as PipeSer;
                                Pipe copyPipe = new Pipe(this.PS, this, copyPipeSer);
                                copyControlOnCanvas = copyPipe;

                                copyPipe.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    copyPipe.PathFigureLeftSize.StartPoint = copyPipeSer.LeftSize.point[0];
                                    copyPipe.LineSegmentLeftSize.Point = copyPipeSer.LeftSize.point[1];

                                    copyPipe.PathFigureRightSize.StartPoint = copyPipeSer.RightSize.point[0];
                                    copyPipe.LineSegmentRightSize.Point = copyPipeSer.RightSize.point[1];

                                    copyPipe.PathFigureTopSize.StartPoint = copyPipeSer.TopSize.point[0];
                                    copyPipe.LineSegmentTopSize.Point = copyPipeSer.TopSize.point[1];

                                    copyPipe.PathFigureDownSize.StartPoint = copyPipeSer.DownSize.point[0];
                                    copyPipe.LineSegmentDownSize.Point = copyPipeSer.DownSize.point[1];

                                    copyPipe.PathFigureLeftFlange.StartPoint = copyPipeSer.LeftFlange.point[0];
                                    copyPipe.PolyLineSegmentLeftFlange.Points[0] = copyPipeSer.LeftFlange.point[1];
                                    copyPipe.PolyLineSegmentLeftFlange.Points[1] = copyPipeSer.LeftFlange.point[2];
                                    copyPipe.PolyLineSegmentLeftFlange.Points[2] = copyPipeSer.LeftFlange.point[3];
                                    copyPipe.PolyLineSegmentLeftFlange.Points[3] = copyPipeSer.LeftFlange.point[4];

                                    copyPipe.PathFigureRightFlange.StartPoint = copyPipeSer.RightFlange.point[0];
                                    copyPipe.PolyLineSegmentRightFlange.Points[0] = copyPipeSer.RightFlange.point[1];
                                    copyPipe.PolyLineSegmentRightFlange.Points[1] = copyPipeSer.RightFlange.point[2];
                                    copyPipe.PolyLineSegmentRightFlange.Points[2] = copyPipeSer.RightFlange.point[3];
                                    copyPipe.PolyLineSegmentRightFlange.Points[3] = copyPipeSer.RightFlange.point[4];

                                    copyPipe.PathFigurePipe.StartPoint = copyPipeSer.Pipe.point[0];
                                    copyPipe.PolyLineSegmentPipe.Points[0] = copyPipeSer.Pipe.point[1];
                                    copyPipe.PolyLineSegmentPipe.Points[1] = copyPipeSer.Pipe.point[2];
                                    copyPipe.PolyLineSegmentPipe.Points[2] = copyPipeSer.Pipe.point[3];
                                    copyPipe.PolyLineSegmentPipe.Points[3] = copyPipeSer.Pipe.point[4];

                                    copyPipe.PathFigureBorder.StartPoint = copyPipeSer.BorderPipe.point[0];
                                    copyPipe.PolyLineSegmentBorder.Points[0] = copyPipeSer.BorderPipe.point[1];
                                    copyPipe.PolyLineSegmentBorder.Points[1] = copyPipeSer.BorderPipe.point[2];
                                    copyPipe.PolyLineSegmentBorder.Points[2] = copyPipeSer.BorderPipe.point[3];
                                    copyPipe.PolyLineSegmentBorder.Points[3] = copyPipeSer.BorderPipe.point[4];

                                    copyPipe.border.Pen.Brush.Opacity = 100;

                                    copyPipe.Diameter = (copyPipe.PathFigureDownSize.StartPoint.Y - copyPipe.PathFigureTopSize.StartPoint.Y);                                 
                                }));

                                page.CollectionPipe.Add(copyPipeSer);
                            }
                            #endregion
                            #region CopyPipes90

                            else if (copyControlOnCanvasSer is Pipe90Ser)
                            {                               
                                Pipe90Ser copyPipe90Ser = copyControlOnCanvasSer as Pipe90Ser;
                                Pipe90 copyPipe90 = new Pipe90(this.PS, this, copyPipe90Ser);
                                copyControlOnCanvas = copyPipe90;

                                copyPipe90.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    copyPipe90.PathFigureTopLenghtSize.StartPoint = copyPipe90Ser.TopLenghtSize.point[0];
                                    copyPipe90.LineSegmentTopLenghtSize.Point = copyPipe90Ser.TopLenghtSize.point[1];

                                    copyPipe90.PathFigureDownLenghtSize.StartPoint = copyPipe90Ser.DownLenghtSize.point[0];
                                    copyPipe90.LineSegmentDownLenghtSize.Point = copyPipe90Ser.DownLenghtSize.point[1];

                                    copyPipe90.PathFigureTopSize.StartPoint = copyPipe90Ser.TopSize.point[0];
                                    copyPipe90.LineSegmentTopSize.Point = copyPipe90Ser.TopSize.point[1];

                                    copyPipe90.PathFigureDownSize.StartPoint = copyPipe90Ser.DownSize.point[0];
                                    copyPipe90.LineSegmentDownSize.Point = copyPipe90Ser.DownSize.point[1];

                                    copyPipe90.PathFigureLeftFlange.StartPoint = copyPipe90Ser.LeftFlange.point[0];
                                    copyPipe90.PolyLineSegmentLeftFlange.Points[0] = copyPipe90Ser.LeftFlange.point[1];
                                    copyPipe90.PolyLineSegmentLeftFlange.Points[1] = copyPipe90Ser.LeftFlange.point[2];
                                    copyPipe90.PolyLineSegmentLeftFlange.Points[2] = copyPipe90Ser.LeftFlange.point[3];
                                    copyPipe90.PolyLineSegmentLeftFlange.Points[3] = copyPipe90Ser.LeftFlange.point[4];

                                    copyPipe90.PathFigureRightFlange.StartPoint = copyPipe90Ser.RightFlange.point[0];
                                    copyPipe90.PolyLineSegmentRightFlange.Points[0] = copyPipe90Ser.RightFlange.point[1];
                                    copyPipe90.PolyLineSegmentRightFlange.Points[1] = copyPipe90Ser.RightFlange.point[2];
                                    copyPipe90.PolyLineSegmentRightFlange.Points[2] = copyPipe90Ser.RightFlange.point[3];
                                    copyPipe90.PolyLineSegmentRightFlange.Points[3] = copyPipe90Ser.RightFlange.point[4];

                                    copyPipe90.PathFigureTopImage.StartPoint = copyPipe90Ser.TopImage.point[0];
                                    copyPipe90.PolyLineSegmentTopImage.Points[0] = copyPipe90Ser.TopImage.point[1];
                                    copyPipe90.PolyLineSegmentTopImage.Points[1] = copyPipe90Ser.TopImage.point[2];
                                    copyPipe90.PolyLineSegmentTopImage.Points[2] = copyPipe90Ser.TopImage.point[3];
                                    copyPipe90.PolyLineSegmentTopImage.Points[3] = copyPipe90Ser.TopImage.point[4];

                                    copyPipe90.PathFigureDownImage.StartPoint = copyPipe90Ser.DownImage.point[0];
                                    copyPipe90.PolyLineSegmentDownImage.Points[0] = copyPipe90Ser.DownImage.point[1];
                                    copyPipe90.PolyLineSegmentDownImage.Points[1] = copyPipe90Ser.DownImage.point[2];
                                    copyPipe90.PolyLineSegmentDownImage.Points[2] = copyPipe90Ser.DownImage.point[3];
                                    copyPipe90.PolyLineSegmentDownImage.Points[3] = copyPipe90Ser.DownImage.point[4];

                                    copyPipe90.PathFigureLeftDownSize.StartPoint = copyPipe90Ser.LeftDownSize.point[0];
                                    copyPipe90.LineSegmentLeftDownSize.Point = copyPipe90Ser.LeftDownSize.point[1];

                                    copyPipe90.PathFigureRightDownSize.StartPoint = copyPipe90Ser.RightDownSize.point[0];
                                    copyPipe90.LineSegmentRightDownSize.Point = copyPipe90Ser.RightDownSize.point[1];

                                    copyPipe90.PathFigureBorder.StartPoint = copyPipe90Ser.BorderPipe90.point[0];
                                    copyPipe90.PolyLineSegmentBorder.Points[0] = copyPipe90Ser.BorderPipe90.point[1];
                                    copyPipe90.PolyLineSegmentBorder.Points[1] = copyPipe90Ser.BorderPipe90.point[2];
                                    copyPipe90.PolyLineSegmentBorder.Points[2] = copyPipe90Ser.BorderPipe90.point[3];
                                    copyPipe90.PolyLineSegmentBorder.Points[3] = copyPipe90Ser.BorderPipe90.point[4];

                                    copyPipe90.border.Pen.Brush.Opacity = 100;

                                    copyPipe90.Diameter = (copyPipe90.PathFigureDownSize.StartPoint.Y - copyPipe90.PathFigureTopSize.StartPoint.Y);                                    
                                }));

                                page.CollectionPipe90.Add(copyPipe90Ser);
                            }
                            #endregion
                            #region CopyTextSers
                            else if (copyControlOnCanvasSer is TextSer)
                            {
                                TextSer copyTextSer = copyControlOnCanvasSer as TextSer;
                                Text copyText = new Text(this.PS, this, copyTextSer);
                                copyControlOnCanvas = copyText;

                                copyText.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    copyText.PathFigureLeftSize.StartPoint = copyTextSer.LeftSize.point[0];
                                    copyText.LineSegmentLeftSize.Point = copyTextSer.LeftSize.point[1];

                                    copyText.PathFigureRightSize.StartPoint = copyTextSer.RightSize.point[0];
                                    copyText.LineSegmentRightSize.Point = copyTextSer.RightSize.point[1];

                                    copyText.PathFigureTopSize.StartPoint = copyTextSer.TopSize.point[0];
                                    copyText.LineSegmentTopSize.Point = copyTextSer.TopSize.point[1];

                                    copyText.PathFigureDownSize.StartPoint = copyTextSer.DownSize.point[0];
                                    copyText.LineSegmentDownSize.Point = copyTextSer.DownSize.point[1];

                                    copyText.PathFigureBorder.StartPoint = copyTextSer.Border.point[0];
                                    copyText.PolyLineSegmentBorder.Points[0] = copyTextSer.Border.point[1];
                                    copyText.PolyLineSegmentBorder.Points[1] = copyTextSer.Border.point[2];
                                    copyText.PolyLineSegmentBorder.Points[2] = copyTextSer.Border.point[3];
                                    copyText.PolyLineSegmentBorder.Points[3] = copyTextSer.Border.point[4];

                                    copyText.border.Pen.Brush.Opacity = 100;
                                }));
                                                  
                                page.CollectionText.Add(copyTextSer);
                            }
                            #endregion
                            #region CopyDisplaySers
                            else if (copyControlOnCanvasSer is DisplaySer)
                            {
                                DisplaySer copyDisplaySer = copyControlOnCanvasSer as DisplaySer;
                                Display copyDisplay = new Display(this.PS, this, copyDisplaySer);
                                copyControlOnCanvas = copyDisplay;

                                copyDisplay.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    copyDisplay.PathFigureLeftSize.StartPoint = copyDisplaySer.LeftSize.point[0];
                                    copyDisplay.LineSegmentLeftSize.Point = copyDisplaySer.LeftSize.point[1];

                                    copyDisplay.PathFigureRightSize.StartPoint = copyDisplaySer.RightSize.point[0];
                                    copyDisplay.LineSegmentRightSize.Point = copyDisplaySer.RightSize.point[1];

                                    copyDisplay.PathFigureTopSize.StartPoint = copyDisplaySer.TopSize.point[0];
                                    copyDisplay.LineSegmentTopSize.Point = copyDisplaySer.TopSize.point[1];

                                    copyDisplay.PathFigureDownSize.StartPoint = copyDisplaySer.DownSize.point[0];
                                    copyDisplay.LineSegmentDownSize.Point = copyDisplaySer.DownSize.point[1];

                                    copyDisplay.PathFigureBorder.StartPoint = copyDisplaySer.Border.point[0];
                                    copyDisplay.PolyLineSegmentBorder.Points[0] = copyDisplaySer.Border.point[1];
                                    copyDisplay.PolyLineSegmentBorder.Points[1] = copyDisplaySer.Border.point[2];
                                    copyDisplay.PolyLineSegmentBorder.Points[2] = copyDisplaySer.Border.point[3];
                                    copyDisplay.PolyLineSegmentBorder.Points[3] = copyDisplaySer.Border.point[4];

                                    copyDisplay.border.Pen.Brush.Opacity = 100;
                                }));

                                page.CollectionDisplay.Add(copyDisplaySer);
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
                        #endregion

                    this.CountSelect = mainWindow.CurrentObjects.Count;

                    break;

                case 2:

                    if (mainWindow.CurrentObjects[0].IS.Path != this.PS.Path)
                    {
                        cutCanvas = (CanvasPage)mainWindow.CurrentObjects[0].CanvasTab;
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

                        if (mainWindow.CurrentObjects[0] is ControlOnCanvasPage)
                        {
                            if (mainWindow.CurrentObjects[0] is Pipe)
                            {
                                Pipe currentPipe = mainWindow.CurrentObjects[0] as Pipe;

                                PipeSer curPipeSer = currentPipe.PipeSer;

                                cutPipe = currentPipe;
                                cutPipe.CanvasPage.Children.Remove(cutPipe);

                                Page cutPage = ((AppWPF)Application.Current).CollectionPage[cutPipe.PS.Path];
                                cutPage.CollectionPipe.Remove(curPipeSer);

                                page.CollectionPipe.Add(curPipeSer);
                            }
                            else if (mainWindow.CurrentObjects[0] is Pipe90)
                            {
                                Pipe90 currentPipe90 = mainWindow.CurrentObjects[0] as Pipe90;

                                Pipe90Ser curPipe90Ser = currentPipe90.Pipe90Ser;

                                cutPipe90 = currentPipe90;
                                cutPipe90.CanvasPage.Children.Remove(cutPipe90);

                                Page cutPage = ((AppWPF)Application.Current).CollectionPage[cutPipe90.PS.Path];
                                cutPage.CollectionPipe90.Remove(curPipe90Ser);

                                page.CollectionPipe90.Add(curPipe90Ser);
                            }
                            else if (mainWindow.CurrentObjects[0] is Text)
                            {
                                Text currentText = mainWindow.CurrentObjects[0] as Text;

                                TextSer curTextSer = currentText.TextSer;

                                cutText = currentText;
                                cutText.CanvasPage.Children.Remove(cutText);

                                Page cutPage = ((AppWPF)Application.Current).CollectionPage[cutText.PS.Path];
                                cutPage.CollectionText.Remove(curTextSer);

                                page.CollectionText.Add(curTextSer);
                            }
                            else if (mainWindow.CurrentObjects[0] is Display)
                            {
                                Display currentDisplay = mainWindow.CurrentObjects[0] as Display;

                                DisplaySer curDisplaySer = currentDisplay.DisplaySer;

                                cutDisplay = currentDisplay;
                                cutDisplay.CanvasPage.Children.Remove(cutDisplay);

                                Page cutPage = ((AppWPF)Application.Current).CollectionPage[cutDisplay.PS.Path];
                                cutPage.CollectionDisplay.Remove(curDisplaySer);

                                page.CollectionDisplay.Add(curDisplaySer);
                            }
                            else if (mainWindow.CurrentObjects[0] is ImageControl)
                            {
                                ImageControl currentImageControl = mainWindow.CurrentObjects[0] as ImageControl;

                                ImageSer curImageSer = currentImageControl.ImageSer;

                                cutImageControl = currentImageControl;
                                cutImageControl.CanvasPage.Children.Remove(cutDisplay);

                                Page cutPage = ((AppWPF)Application.Current).CollectionPage[cutDisplay.PS.Path];
                                cutPage.CollectionImage.Remove(curImageSer);

                                page.CollectionImage.Add(curImageSer);
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

                        mainWindow.CurrentObjects[0].IS = this.PS;
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

                        List<Pipe> cutPipes = new List<Pipe>();
                        List<Pipe90> cutPipes90 = new List<Pipe90>();
                        List<Text> cutTexts = new List<Text>();
                        List<Display> cutDisplays = new List<Display>();
                        List<ImageControl> cutImageControls = new List<ImageControl>();
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

                            if (controlOnCanvas is Pipe)
                            {
                                cutPipes.Add((Pipe)controlOnCanvas);
                            }
                            else if (controlOnCanvas is Pipe90)
                            {
                                cutPipes90.Add((Pipe90)controlOnCanvas);
                            }
                            else if (controlOnCanvas is Text)
                            {
                                cutTexts.Add((Text)controlOnCanvas);
                            }
                            else if (controlOnCanvas is Display)
                            {
                                cutDisplays.Add((Display)controlOnCanvas);
                            }
                            else if(controlOnCanvas is ImageControl)
                            {
                                cutImageControls.Add((ImageControl)controlOnCanvas);
                            }
                        }

                        cutControlsOnCanvas.AddRange(cutPipes);
                        cutControlsOnCanvas.AddRange(cutPipes90);
                        cutControlsOnCanvas.AddRange(cutTexts);
                        cutControlsOnCanvas.AddRange(cutDisplays);
                        cutControlsOnCanvas.AddRange(cutImageControls);
                        cutControlsOnCanvas.Sort();
                       
                        x = comparerRelativelyX - Mouse.GetPosition(this).X;
                        y = comparerRelativelyY - Mouse.GetPosition(this).Y;

                        foreach (ControlOnCanvas controlOnCanvas in cutControlsOnCanvas)
                        {
                            if (controlOnCanvas is Pipe)
                            {
                                cutPipe = controlOnCanvas as Pipe;
                                cutPipe.PipeSer.Сoordinates = new Point(cutPipe.PipeSer.Сoordinates.X - x, cutPipe.PipeSer.Сoordinates.Y - y);

                                cutPipe.CanvasPage.Children.Remove(cutPipe);

                                Page cutPage = ((AppWPF)Application.Current).CollectionPage[cutPipe.PS.Path];
                                cutPage.CollectionPipe.Remove(cutPipe.PipeSer);

                                cutPipe.PS = this.PS;
                                cutPipe.CanvasPage = this;

                                page.CollectionPipe.Add(cutPipe.PipeSer);
                            }

                            if (controlOnCanvas is Pipe90)
                            {
                                cutPipe90 = controlOnCanvas as Pipe90;
                                cutPipe90.Pipe90Ser.Сoordinates = new Point(cutPipe90.Pipe90Ser.Сoordinates.X - x, cutPipe90.Pipe90Ser.Сoordinates.Y - y);

                                cutPipe90.CanvasPage.Children.Remove(cutPipe90);

                                Page cutPage = ((AppWPF)Application.Current).CollectionPage[cutPipe90.PS.Path];
                                cutPage.CollectionPipe90.Remove(cutPipe90.Pipe90Ser);

                                cutPipe90.PS = this.PS;
                                cutPipe90.CanvasPage = this;

                                page.CollectionPipe90.Add(cutPipe90.Pipe90Ser);
                            }

                            if (controlOnCanvas is Text)
                            {
                                cutText = controlOnCanvas as Text;
                                cutText.TextSer.Сoordinates = new Point(cutText.TextSer.Сoordinates.X - x, cutText.TextSer.Сoordinates.Y - y);

                                cutText.CanvasPage.Children.Remove(cutText);

                                Page cutPage = ((AppWPF)Application.Current).CollectionPage[cutText.PS.Path];
                                cutPage.CollectionText.Remove(cutText.TextSer);

                                cutText.PS = this.PS;
                                cutText.CanvasPage = this;

                                page.CollectionText.Add(cutText.TextSer);
                            }

                            if (controlOnCanvas is Display)
                            {
                                cutDisplay = controlOnCanvas as Display;
                                cutDisplay.DisplaySer.Сoordinates = new Point(cutDisplay.DisplaySer.Сoordinates.X - x, cutDisplay.DisplaySer.Сoordinates.Y - y);

                                cutDisplay.CanvasPage.Children.Remove(cutDisplay);

                                Page cutPage = ((AppWPF)Application.Current).CollectionPage[cutDisplay.PS.Path];
                                cutPage.CollectionDisplay.Remove(cutDisplay.DisplaySer);

                                cutDisplay.PS = this.PS;
                                cutDisplay.CanvasPage = this;

                                page.CollectionDisplay.Add(cutDisplay.DisplaySer);
                            }

                            if (controlOnCanvas is ImageControl)
                            {
                                cutImageControl = controlOnCanvas as ImageControl;
                                cutImageControl.ImageSer.Сoordinates = new Point(cutImageControl.ImageSer.Сoordinates.X - x, cutImageControl.ImageSer.Сoordinates.Y - y);

                                cutImageControl.CanvasPage.Children.Remove(cutImageControl);

                                Page cutPage = ((AppWPF)Application.Current).CollectionPage[cutImageControl.PS.Path];
                                cutPage.CollectionImage.Remove(cutImageControl.ImageSer);

                                cutImageControl.PS = this.PS;
                                cutImageControl.CanvasPage = this;

                                page.CollectionImage.Add(cutImageControl.ImageSer);
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

                PipeOnCanvas pipe = null;

                foreach (ControlOnCanvasPage controlOnCanvas in this.Children)
                {
                    if (controlOnCanvas is PipeOnCanvas)
                    {
                        if (controlOnCanvas.IsSelected)
                        {
                            countPipe += 1;
                            pipe = controlOnCanvas as PipeOnCanvas;

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

                            pipeOld = (PipeOnCanvas)controlOnCanvas;
                        }
                    }
                }

                if (countPipe > 0)
                {
                    mainWindow.TextBoxDiameter.IsReadOnly = false;
                    mainWindow.ComboBoxEnvironment.IsEnabled = true;
                }
                if (countPipe == 1)
                {
                    mainWindow.TextBoxDiameter.IsReadOnly = false;
                    mainWindow.ComboBoxEnvironment.IsEnabled = true;
                    mainWindow.TextBoxDiameter.Text = string.Format("{0:F2}", pipe.Diameter);
                    mainWindow.ComboBoxEnvironment.SelectedIndex = pipe.IntEnvironment;
                }
                else if (countPipe == 0)
                {
                    mainWindow.TextBoxDiameter.IsReadOnly = true;
                    mainWindow.ComboBoxEnvironment.IsEnabled = false;
                    mainWindow.TextBoxDiameter.Text = null;
                    mainWindow.ComboBoxEnvironment.SelectedIndex = -1;
                }
            }
            else if (this.CountSelect == 1)
            {
                mainWindow.LabelSelected.Content = "Выделенно объектов: " + this.CountSelect;

                foreach (ControlOnCanvasPage controlOnCanvasPage in this.Children)
                {
                    if (controlOnCanvasPage.IsSelected)
                    {
                        mainWindow.CoordinateObjectX.IsReadOnly = false;
                        mainWindow.CoordinateObjectY.IsReadOnly = false;
                        if (controlOnCanvasPage.controlOnCanvasSer.Transform == 0)
                        {
                            controlOnCanvasPage.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvasPage.GetValue(Canvas.LeftProperty));
                            controlOnCanvasPage.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvasPage.GetValue(Canvas.TopProperty));
                        }
                        else if (controlOnCanvasPage.controlOnCanvasSer.Transform == -90 || controlOnCanvasPage.controlOnCanvasSer.Transform == 270)
                        {
                            controlOnCanvasPage.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvasPage.GetValue(Canvas.TopProperty) - controlOnCanvasPage.ActualWidth);
                            controlOnCanvasPage.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvasPage.GetValue(Canvas.LeftProperty));
                        }
                        else if (controlOnCanvasPage.controlOnCanvasSer.Transform == -180 || controlOnCanvasPage.controlOnCanvasSer.Transform == 180)
                        {
                            controlOnCanvasPage.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvasPage.GetValue(Canvas.TopProperty) - controlOnCanvasPage.ActualHeight);
                            controlOnCanvasPage.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvasPage.GetValue(Canvas.LeftProperty) - controlOnCanvasPage.ActualWidth);
                        }
                        else if (controlOnCanvasPage.controlOnCanvasSer.Transform == -270 || controlOnCanvasPage.controlOnCanvasSer.Transform == 90)
                        {
                            controlOnCanvasPage.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvasPage.GetValue(Canvas.TopProperty));
                            controlOnCanvasPage.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvasPage.GetValue(Canvas.LeftProperty) - controlOnCanvasPage.ActualHeight);
                        }

                        if (controlOnCanvasPage is PipeOnCanvas)
                        {
                            pipeOnCanvas = controlOnCanvasPage as PipeOnCanvas;

                            mainWindow.TextBoxDiameter.IsReadOnly = false;
                            mainWindow.ComboBoxEnvironment.IsEnabled = true;
                            mainWindow.TextBoxDiameter.Text = string.Format("{0:F2}", pipeOnCanvas.Diameter);
                            mainWindow.ComboBoxEnvironment.SelectedIndex = pipeOnCanvas.IntEnvironment;
                        }
                    }
                }
            }

            this.RepositionAllObjects(this);
            this.InvalidateMeasure();

            ((AppWPF)Application.Current).SaveTabItem(TabItemPage);

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
                    foreach (ControlOnCanvasPage objectOnCanvas in this.SelectedControlOnCanvas)
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

            ((MainWindow)Application.Current.MainWindow).LabelCoordinateCursor.Content = string.Format("{0}{1:F2}{2}{3}{4:F2}","X: ", coordinate.X, " ", "Y: ", coordinate.Y);

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

            foreach (ControlOnCanvasPage hitControl in hitResultsList)
            {
                hitControl.AreaSelect();
            }

            if (hitResultsList.Count != 0)
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                if (CountSelect == 1)
                {
                    if (hitResultsList[0] is PipeOnCanvas)
                    {
                        PipeOnCanvas pipeOnCanvas;
                        pipeOnCanvas = hitResultsList[0] as PipeOnCanvas;

                        pipeOnCanvas.TextBoxDiameter.IsReadOnly = false;
                        pipeOnCanvas.ComboBoxEnvironment.IsEnabled = true;
                        pipeOnCanvas.TextBoxDiameter.Text = string.Format("{0:F2}", pipeOnCanvas.Diameter);
                        pipeOnCanvas.ComboBoxEnvironment.SelectedIndex = pipeOnCanvas.IntEnvironment;
                    }

                    mainWindow.CoordinateObjectX.IsReadOnly = false;
                    mainWindow.CoordinateObjectY.IsReadOnly = false;

                    ControlOnCanvasPage controlOnCanvas = hitResultsList[0] as ControlOnCanvasPage;

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
               
                    PipeOnCanvas pipeOld = null;
                    PipeOnCanvas pipe = null;
                    int countPipe = 0;
                    bool falseComparer = false;

                    foreach (ControlOnCanvasPage controlOnCanvas in SelectedControlOnCanvas)
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
                        mainWindow.TextBoxDiameter.IsReadOnly = true;
                        mainWindow.ComboBoxEnvironment.IsEnabled = false;
                        mainWindow.TextBoxDiameter.Text = null;
                        mainWindow.ComboBoxEnvironment.SelectedIndex = -1;
                    }
                    else if (countPipe == 1)
                    {
                        mainWindow.TextBoxDiameter.IsReadOnly = false;
                        mainWindow.ComboBoxEnvironment.IsEnabled = true;
                        mainWindow.TextBoxDiameter.Text = string.Format("{0:F2}", pipeOld.Diameter);
                        mainWindow.ComboBoxEnvironment.SelectedIndex = pipeOld.IntEnvironment;
                    }
                    else if (countPipe > 1)
                    {
                        mainWindow.TextBoxDiameter.IsReadOnly = false;
                        mainWindow.ComboBoxEnvironment.IsEnabled = true;
                    }
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

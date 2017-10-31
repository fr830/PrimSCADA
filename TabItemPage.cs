// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SCADA
{
    public class TabItemPage : TabItemParent
    {
        private PageScada pS;
        public PageScada PS
        {
            get { return pS; }
            set { IS = value; pS = value; }
        }

        private CanvasPage canvasPage;
        public CanvasPage CanvasPage
        {
            get { return canvasPage; }
            set { CanvasTab = value; canvasPage = value; }
        }

        static TabItemPage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItemPage), new FrameworkPropertyMetadata(typeof(TabItem)));
        }

        public TabItemPage(PageScada ps)
        {
            PS = ps;
            CanvasPage canvasPage = new CanvasPage(this.PS, this);
            CanvasPage = canvasPage;

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
            lPageName.Content = PS.Name;

            ScrollViewer scroll = new ScrollViewer();
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.SetValue(Grid.RowProperty, 0);

            Image insertImage = new Image();
            insertImage.Source = new BitmapImage(new Uri("Images/Insert16.ico", UriKind.Relative));

            Binding BindingInsert = new Binding();
            BindingInsert.Source = mainWindow;
            BindingInsert.Path = new PropertyPath("IsBindingInsertObject");
            BindingInsert.Mode = BindingMode.OneWay;

            MenuItem menuItemInsert = new MenuItem();
            menuItemInsert.SetBinding(MenuItem.IsEnabledProperty, BindingInsert);
            menuItemInsert.Icon = insertImage;
            menuItemInsert.Click += canvasPage.Insert;
            menuItemInsert.Header = "Вставить";

            ContextMenu contextMenuCanvas = new ContextMenu();
            contextMenuCanvas.Items.Add(menuItemInsert);
           
            canvasPage.ContextMenu = contextMenuCanvas;

            StackPanel panelTabItem = new StackPanel();
            panelTabItem.ToolTip = PS.Path;
            panelTabItem.Orientation = Orientation.Horizontal;
            panelTabItem.Children.Add(lPageName);
            panelTabItem.Children.Add(close);

            this.Header = panelTabItem;
            this.Content = scroll;
            scroll.Content = canvasPage;

            ((AppWPF)Application.Current).CollectionTabItemParent.Add(PS.Path, this);

            Page page = ((AppWPF)Application.Current).CollectionPage[PS.Path];

            foreach (PipeSer pipeSer in page.CollectionPipe)
            {
                Pipe pipe = new Pipe(PS, CanvasPage, pipeSer);

                pipe.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    pipe.PathFigureLeftSize.StartPoint = pipeSer.LeftSize.point[0];
                    pipe.LineSegmentLeftSize.Point = pipeSer.LeftSize.point[1];

                    pipe.PathFigureRightSize.StartPoint = pipeSer.RightSize.point[0];
                    pipe.LineSegmentRightSize.Point = pipeSer.RightSize.point[1];

                    pipe.PathFigureTopSize.StartPoint = pipeSer.TopSize.point[0];
                    pipe.LineSegmentTopSize.Point = pipeSer.TopSize.point[1];

                    pipe.PathFigureDownSize.StartPoint = pipeSer.DownSize.point[0];
                    pipe.LineSegmentDownSize.Point = pipeSer.DownSize.point[1];

                    pipe.PathFigureLeftFlange.StartPoint = pipeSer.LeftFlange.point[0];
                    pipe.PolyLineSegmentLeftFlange.Points[0] = pipeSer.LeftFlange.point[1];
                    pipe.PolyLineSegmentLeftFlange.Points[1] = pipeSer.LeftFlange.point[2];
                    pipe.PolyLineSegmentLeftFlange.Points[2] = pipeSer.LeftFlange.point[3];
                    pipe.PolyLineSegmentLeftFlange.Points[3] = pipeSer.LeftFlange.point[4];

                    pipe.PathFigureRightFlange.StartPoint = pipeSer.RightFlange.point[0];
                    pipe.PolyLineSegmentRightFlange.Points[0] = pipeSer.RightFlange.point[1];
                    pipe.PolyLineSegmentRightFlange.Points[1] = pipeSer.RightFlange.point[2];
                    pipe.PolyLineSegmentRightFlange.Points[2] = pipeSer.RightFlange.point[3];
                    pipe.PolyLineSegmentRightFlange.Points[3] = pipeSer.RightFlange.point[4];

                    pipe.PathFigurePipe.StartPoint = pipeSer.Pipe.point[0];
                    pipe.PolyLineSegmentPipe.Points[0] = pipeSer.Pipe.point[1];
                    pipe.PolyLineSegmentPipe.Points[1] = pipeSer.Pipe.point[2];
                    pipe.PolyLineSegmentPipe.Points[2] = pipeSer.Pipe.point[3];
                    pipe.PolyLineSegmentPipe.Points[3] = pipeSer.Pipe.point[4];

                    pipe.PathFigureBorder.StartPoint = pipeSer.BorderPipe.point[0];
                    pipe.PolyLineSegmentBorder.Points[0] = pipeSer.BorderPipe.point[1];
                    pipe.PolyLineSegmentBorder.Points[1] = pipeSer.BorderPipe.point[2];
                    pipe.PolyLineSegmentBorder.Points[2] = pipeSer.BorderPipe.point[3];
                    pipe.PolyLineSegmentBorder.Points[3] = pipeSer.BorderPipe.point[4];

                    pipe.Diameter = (pipe.PathFigureDownSize.StartPoint.Y - pipe.PathFigureTopSize.StartPoint.Y);                  
                }));

                pipe.SetValue(Canvas.LeftProperty, pipeSer.Сoordinates.X);
                pipe.SetValue(Canvas.TopProperty, pipeSer.Сoordinates.Y);

                canvasPage.Children.Add(pipe);
                pipe.ApplyTemplate();
            }
            foreach (Pipe90Ser pipe90Ser in page.CollectionPipe90)
            {
                Pipe90 pipe90 = new Pipe90(PS, CanvasPage, pipe90Ser);

                pipe90.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    pipe90.PathFigureTopLenghtSize.StartPoint = pipe90Ser.TopLenghtSize.point[0];
                    pipe90.LineSegmentTopLenghtSize.Point = pipe90Ser.TopLenghtSize.point[1];

                    pipe90.PathFigureDownLenghtSize.StartPoint = pipe90Ser.DownLenghtSize.point[0];
                    pipe90.LineSegmentDownLenghtSize.Point = pipe90Ser.DownLenghtSize.point[1];

                    pipe90.PathFigureTopSize.StartPoint = pipe90Ser.TopSize.point[0];
                    pipe90.LineSegmentTopSize.Point = pipe90Ser.TopSize.point[1];

                    pipe90.PathFigureDownSize.StartPoint = pipe90Ser.DownSize.point[0];
                    pipe90.LineSegmentDownSize.Point = pipe90Ser.DownSize.point[1];

                    pipe90.PathFigureLeftFlange.StartPoint = pipe90Ser.LeftFlange.point[0];
                    pipe90.PolyLineSegmentLeftFlange.Points[0] = pipe90Ser.LeftFlange.point[1];
                    pipe90.PolyLineSegmentLeftFlange.Points[1] = pipe90Ser.LeftFlange.point[2];
                    pipe90.PolyLineSegmentLeftFlange.Points[2] = pipe90Ser.LeftFlange.point[3];
                    pipe90.PolyLineSegmentLeftFlange.Points[3] = pipe90Ser.LeftFlange.point[4];

                    pipe90.PathFigureRightFlange.StartPoint = pipe90Ser.RightFlange.point[0];
                    pipe90.PolyLineSegmentRightFlange.Points[0] = pipe90Ser.RightFlange.point[1];
                    pipe90.PolyLineSegmentRightFlange.Points[1] = pipe90Ser.RightFlange.point[2];
                    pipe90.PolyLineSegmentRightFlange.Points[2] = pipe90Ser.RightFlange.point[3];
                    pipe90.PolyLineSegmentRightFlange.Points[3] = pipe90Ser.RightFlange.point[4];

                    pipe90.PathFigureTopImage.StartPoint = pipe90Ser.TopImage.point[0];
                    pipe90.PolyLineSegmentTopImage.Points[0] = pipe90Ser.TopImage.point[1];
                    pipe90.PolyLineSegmentTopImage.Points[1] = pipe90Ser.TopImage.point[2];
                    pipe90.PolyLineSegmentTopImage.Points[2] = pipe90Ser.TopImage.point[3];
                    pipe90.PolyLineSegmentTopImage.Points[3] = pipe90Ser.TopImage.point[4];

                    pipe90.PathFigureDownImage.StartPoint = pipe90Ser.DownImage.point[0];
                    pipe90.PolyLineSegmentDownImage.Points[0] = pipe90Ser.DownImage.point[1];
                    pipe90.PolyLineSegmentDownImage.Points[1] = pipe90Ser.DownImage.point[2];
                    pipe90.PolyLineSegmentDownImage.Points[2] = pipe90Ser.DownImage.point[3];
                    pipe90.PolyLineSegmentDownImage.Points[3] = pipe90Ser.DownImage.point[4];

                    pipe90.PathFigureLeftDownSize.StartPoint = pipe90Ser.LeftDownSize.point[0];
                    pipe90.LineSegmentLeftDownSize.Point = pipe90Ser.LeftDownSize.point[1];

                    pipe90.PathFigureRightDownSize.StartPoint = pipe90Ser.RightDownSize.point[0];
                    pipe90.LineSegmentRightDownSize.Point = pipe90Ser.RightDownSize.point[1];

                    pipe90.PathFigureBorder.StartPoint = pipe90Ser.BorderPipe90.point[0];
                    pipe90.PolyLineSegmentBorder.Points[0] = pipe90Ser.BorderPipe90.point[1];
                    pipe90.PolyLineSegmentBorder.Points[1] = pipe90Ser.BorderPipe90.point[2];
                    pipe90.PolyLineSegmentBorder.Points[2] = pipe90Ser.BorderPipe90.point[3];
                    pipe90.PolyLineSegmentBorder.Points[3] = pipe90Ser.BorderPipe90.point[4];

                    pipe90.Diameter = (pipe90.PathFigureDownSize.StartPoint.Y - pipe90.PathFigureTopSize.StartPoint.Y);                   
                }));

                pipe90.SetValue(Canvas.LeftProperty, pipe90Ser.Сoordinates.X);
                pipe90.SetValue(Canvas.TopProperty, pipe90Ser.Сoordinates.Y);

                canvasPage.Children.Add(pipe90);
                pipe90.ApplyTemplate();
            }
            foreach (TextSer textSer in page.CollectionText)
            {
                Text text = new Text(PS, CanvasPage, textSer);

                text.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    text.PathFigureBorder.StartPoint = textSer.Border.point[0];
                    text.PolyLineSegmentBorder.Points[0] = textSer.Border.point[1];
                    text.PolyLineSegmentBorder.Points[1] = textSer.Border.point[2];
                    text.PolyLineSegmentBorder.Points[2] = textSer.Border.point[3];
                    text.PolyLineSegmentBorder.Points[3] = textSer.Border.point[4];

                    text.PathFigureDownSize.StartPoint = textSer.DownSize.point[0];
                    text.LineSegmentDownSize.Point = textSer.DownSize.point[1];

                    text.PathFigureLeftSize.StartPoint = textSer.LeftSize.point[0];
                    text.LineSegmentLeftSize.Point = textSer.LeftSize.point[1];

                    text.PathFigureRightSize.StartPoint = textSer.RightSize.point[0];
                    text.LineSegmentRightSize.Point = textSer.RightSize.point[1];

                    text.PathFigureTopSize.StartPoint = textSer.TopSize.point[0];
                    text.LineSegmentTopSize.Point = textSer.TopSize.point[1];
                }));

                text.SetValue(Canvas.LeftProperty, textSer.Сoordinates.X);
                text.SetValue(Canvas.TopProperty, textSer.Сoordinates.Y);

                canvasPage.Children.Add(text);

                text.ApplyTemplate();
            }
            foreach (DisplaySer displaySer in page.CollectionDisplay)
            {
                Display display = new Display(PS, CanvasPage, displaySer);

                display.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    display.PathFigureBorder.StartPoint = displaySer.Border.point[0];
                    display.PolyLineSegmentBorder.Points[0] = displaySer.Border.point[1];
                    display.PolyLineSegmentBorder.Points[1] = displaySer.Border.point[2];
                    display.PolyLineSegmentBorder.Points[2] = displaySer.Border.point[3];
                    display.PolyLineSegmentBorder.Points[3] = displaySer.Border.point[4];

                    display.PathFigureDownSize.StartPoint = displaySer.DownSize.point[0];
                    display.LineSegmentDownSize.Point = displaySer.DownSize.point[1];

                    display.PathFigureLeftSize.StartPoint = displaySer.LeftSize.point[0];
                    display.LineSegmentLeftSize.Point = displaySer.LeftSize.point[1];

                    display.PathFigureRightSize.StartPoint = displaySer.RightSize.point[0];
                    display.LineSegmentRightSize.Point = displaySer.RightSize.point[1];

                    display.PathFigureTopSize.StartPoint = displaySer.TopSize.point[0];
                    display.LineSegmentTopSize.Point = displaySer.TopSize.point[1];
                }));

                display.SetValue(Canvas.LeftProperty, displaySer.Сoordinates.X);
                display.SetValue(Canvas.TopProperty, displaySer.Сoordinates.Y);

                canvasPage.Children.Add(display);

                display.ApplyTemplate();

                if (display.DisplaySer.IsEthernet)
                {
                    foreach (EthernetSer ethernetSer in ((AppWPF)Application.Current).CollectionEthernetSers)
                    {
                        if (ethernetSer.ID == displaySer.EthernetSearch)
                        {
                            if (displaySer.EthernetOperationalSearch != null)
                            {                               
                                foreach (EthernetOperational eo in ethernetSer.CollectionEthernetOperational)
                                {
                                    if (eo.EthernetOperationalSearch == displaySer.EthernetOperationalSearch)
                                    {
                                        displaySer.EthernetOperationalSearch = eo.EthernetOperationalSearch;

                                        if (displaySer.IsCollRec)
                                        {
                                            foreach (ItemNet itemNet in eo.CollectionItemNetRec)
                                            {
                                                if (displaySer.ItemNetSearch == itemNet)
                                                {
                                                    displaySer.ItemNetSearch = itemNet;

                                                    display.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                                    {
                                                        Binding valueBinding = new Binding();
                                                        valueBinding.Source = displaySer.ItemNetSearch;
                                                        valueBinding.Path = new PropertyPath("Value");

                                                        display.RunBinding.SetBinding(Run.TextProperty, valueBinding);
                                                    }));

                                                    break;
                                                }
                                            }
                                        }

                                        if (displaySer.IsCollSend)
                                        {
                                            foreach (ItemNet itemNet in eo.CollectionItemNetSend)
                                            {
                                                if (displaySer.ItemNetSearch == itemNet)
                                                {
                                                    displaySer.ItemNetSearch = itemNet;

                                                    display.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                                    {
                                                        Binding valueBinding = new Binding();
                                                        valueBinding.Source = displaySer.ItemNetSearch;
                                                        valueBinding.Path = new PropertyPath("Value");

                                                        display.RunBinding.SetBinding(Run.TextProperty, valueBinding);
                                                    }));

                                                    break;
                                                }
                                            }
                                        }

                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (displaySer.IsCollRec)
                                {
                                    foreach (ItemNet itemNet in ethernetSer.CollectionItemNetRec)
                                    {
                                        if (displaySer.ItemNetSearch == itemNet)
                                        {
                                            displaySer.ItemNetSearch = itemNet;

                                            display.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                            {
                                                Binding valueBinding = new Binding();
                                                valueBinding.Converter = new ValueItemNetConverter(displaySer.ItemNetSearch);
                                                valueBinding.Source = displaySer.ItemNetSearch;
                                                valueBinding.Path = new PropertyPath("Value");

                                                display.RunBinding.SetBinding(Run.TextProperty, valueBinding);
                                            }));

                                            break;
                                        }
                                    }
                                }

                                if (displaySer.IsCollSend)
                                {
                                    foreach (ItemNet itemNet in ethernetSer.CollectionItemNetSend)
                                    {
                                        if (displaySer.ItemNetSearch == itemNet)
                                        {
                                            displaySer.ItemNetSearch = itemNet;

                                            display.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                            {
                                                Binding valueBinding = new Binding();
                                                valueBinding.Converter = new ValueItemNetConverter(displaySer.ItemNetSearch);
                                                valueBinding.Source = displaySer.ItemNetSearch;
                                                valueBinding.Path = new PropertyPath("Value");

                                                display.RunBinding.SetBinding(Run.TextProperty, valueBinding);
                                            }));

                                            break;
                                        }
                                    }
                                }
                            }                           
                        }
                    }
                }
                else if (display.DisplaySer.IsModbus)
                {
                    foreach (ModbusSer modbusSer in ((AppWPF)Application.Current).CollectionModbusSers)
                    {
                        if (modbusSer.ID == displaySer.ModbusSearch)
                        {
                            foreach (ItemModbus itemModbus in modbusSer.CollectionItemModbus)
                            {
                                if (displaySer.ItemModbusSearch == itemModbus)
                                {
                                    displaySer.ItemModbusSearch = itemModbus;

                                    display.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                    {
                                        Binding valueBinding = new Binding();
                                        valueBinding.Converter = new ValueItemModbusConverter(displaySer.ItemModbusSearch);
                                        valueBinding.Source = displaySer.ItemModbusSearch;
                                        valueBinding.Path = new PropertyPath("Value");

                                        display.RunBinding.SetBinding(Run.TextProperty, valueBinding);
                                    }));

                                    break;
                                }
                            }

                            break;
                        }
                    }
                }               
            }
            foreach (ImageSer imageSer in page.CollectionImage)
            {
                ImageControl imageControl = new ImageControl(PS, CanvasPage, imageSer);

                imageControl.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    imageControl.PathFigureBorder.StartPoint = imageSer.Border.point[0];
                    imageControl.PolyLineSegmentBorder.Points[0] = imageSer.Border.point[1];
                    imageControl.PolyLineSegmentBorder.Points[1] = imageSer.Border.point[2];
                    imageControl.PolyLineSegmentBorder.Points[2] = imageSer.Border.point[3];
                    imageControl.PolyLineSegmentBorder.Points[3] = imageSer.Border.point[4];

                    imageControl.PathFigureDownSize.StartPoint = imageSer.DownSize.point[0];
                    imageControl.LineSegmentDownSize.Point = imageSer.DownSize.point[1];

                    imageControl.PathFigureLeftSize.StartPoint = imageSer.LeftSize.point[0];
                    imageControl.LineSegmentLeftSize.Point = imageSer.LeftSize.point[1];

                    imageControl.PathFigureRightSize.StartPoint = imageSer.RightSize.point[0];
                    imageControl.LineSegmentRightSize.Point = imageSer.RightSize.point[1];

                    imageControl.PathFigureTopSize.StartPoint = imageSer.TopSize.point[0];
                    imageControl.LineSegmentTopSize.Point = imageSer.TopSize.point[1];
                    
                    if (imageSer.PathImage != null)
                    {
                        try
                        {                           
                            BitmapImage bi = new BitmapImage(new Uri(@imageSer.PathImage, UriKind.RelativeOrAbsolute));

                            if (bi.PixelWidth < 10 || bi.PixelWidth < 10)
                            {
                                new Exception();
                            }

                            imageControl.templateImage.Source = bi;
                        }
                        catch
                        {
                            imageControl.templateImage.Source = new BitmapImage(new Uri("../Images/ImageNotFound.png", UriKind.RelativeOrAbsolute));

                            bool rotate0 = false;
                            bool rotate90 = false;
                            bool rotate180 = false;
                            bool rotate270 = false;

                            if ((int)imageControl.RenderTransform.Value.M11 == 0 && (int)imageControl.RenderTransform.Value.M12 == -1) rotate90 = true;
                            else if ((int)imageControl.RenderTransform.Value.M11 == -1 && (int)imageControl.RenderTransform.Value.M12 == 0) rotate180 = true;
                            else if ((int)imageControl.RenderTransform.Value.M11 == 0 && (int)imageControl.RenderTransform.Value.M12 == 1) rotate270 = true;
                            else rotate0 = true;

                            if (rotate0)
                            {
                                SizeImage((BitmapImage)imageControl.templateImage.Source, imageControl);
                            }
                            else if (rotate90)
                            {
                                SizeImage((BitmapImage)imageControl.templateImage.Source, imageControl);
                            }
                            else if (rotate180)
                            {
                                SizeImage((BitmapImage)imageControl.templateImage.Source, imageControl);
                            }
                            else if (rotate270)
                            {
                                SizeImage((BitmapImage)imageControl.templateImage.Source, imageControl);
                            }
                        }
                    }
                    else if (imageControl.ImageSer.LibraryImage != null)
                    {                       
                        BitmapImage bi = new BitmapImage(new Uri(@"pack://application:,,,/Images/" + imageControl.ImageSer.LibraryImage + ".png", UriKind.RelativeOrAbsolute));

                        imageControl.templateImage.Source = bi;
                    }
                    else
                    {
                        imageControl.templateImage.Source = new BitmapImage(new Uri("../Images/ImageNotFound.png", UriKind.RelativeOrAbsolute));

                        bool rotate0 = false;
                        bool rotate90 = false;
                        bool rotate180 = false;
                        bool rotate270 = false;

                        if ((int)imageControl.RenderTransform.Value.M11 == 0 && (int)imageControl.RenderTransform.Value.M12 == -1) rotate90 = true;
                        else if ((int)imageControl.RenderTransform.Value.M11 == -1 && (int)imageControl.RenderTransform.Value.M12 == 0) rotate180 = true;
                        else if ((int)imageControl.RenderTransform.Value.M11 == 0 && (int)imageControl.RenderTransform.Value.M12 == 1) rotate270 = true;
                        else rotate0 = true;

                        if (rotate0)
                        {
                            SizeImage((BitmapImage)imageControl.templateImage.Source, imageControl);
                        }
                        else if (rotate90)
                        {
                            SizeImage((BitmapImage)imageControl.templateImage.Source, imageControl);
                        }
                        else if (rotate180)
                        {
                            SizeImage((BitmapImage)imageControl.templateImage.Source, imageControl);
                        }
                        else if (rotate270)
                        {
                            SizeImage((BitmapImage)imageControl.templateImage.Source, imageControl);
                        }
                    }


                    if (imageControl.ImageSer.StretchImage == "None")
                    {
                        imageControl.templateImage.Stretch = Stretch.None;
                    }
                    else if (imageControl.ImageSer.StretchImage == "Fill")
                    {
                        imageControl.templateImage.Stretch = Stretch.Fill;
                    }
                    else if (imageControl.ImageSer.StretchImage == "Uniform")
                    {
                        imageControl.templateImage.Stretch = Stretch.Uniform;
                    }
                    else if (imageControl.ImageSer.StretchImage == "UniformToFill")
                    {
                        imageControl.templateImage.Stretch = Stretch.UniformToFill;
                    }
                }));

                imageControl.SetValue(Canvas.LeftProperty, imageSer.Сoordinates.X);
                imageControl.SetValue(Canvas.TopProperty, imageSer.Сoordinates.Y);

                canvasPage.Children.Add(imageControl);

                imageControl.ApplyTemplate();
            }
        }

        void SizeImage(BitmapImage bi, ImageControl imageControl)
        {
            Point downSizePoint = imageControl.LineSegmentDownSize.Point;

            Point borderPipePoint2;
            Point borderPipePoint3;

            Point p1 = imageControl.PathFigureLeftSize.StartPoint;
            Point p2 = imageControl.PathFigureRightSize.StartPoint;
            Point pDown = imageControl.LineSegmentRightSize.Point;

            double diff = p2.X - p1.X;
            double diffHeight = pDown.Y - p2.Y;

            double incWidth = bi.PixelWidth + 6 - diff;
            double incHeight = bi.PixelHeight + 4 - diffHeight;

            p2.X += incWidth;
            imageControl.PathFigureRightSize.StartPoint = p2;
            Point p3 = imageControl.LineSegmentRightSize.Point;
            p3.X += incWidth;
            imageControl.LineSegmentRightSize.Point = p3;

            borderPipePoint2 = imageControl.PolyLineSegmentBorder.Points[0];
            borderPipePoint3 = imageControl.PolyLineSegmentBorder.Points[1];
            borderPipePoint2.X += incWidth;
            imageControl.PolyLineSegmentBorder.Points[0] = borderPipePoint2;
            borderPipePoint3.X += incWidth;
            imageControl.PolyLineSegmentBorder.Points[1] = borderPipePoint3;

            Point topSizePoint = imageControl.LineSegmentTopSize.Point;
            topSizePoint.X += incWidth;
            imageControl.LineSegmentTopSize.Point = topSizePoint;


            downSizePoint.X += incWidth;
            imageControl.LineSegmentDownSize.Point = downSizePoint;

            imageControl.templateBorder.Width += incWidth;

            //По оси y
            downSizePoint = imageControl.PathFigureDownSize.StartPoint;
            downSizePoint.Y += incHeight;
            imageControl.PathFigureDownSize.StartPoint = downSizePoint;
            Point downSizePoint2 = imageControl.LineSegmentDownSize.Point;
            downSizePoint2.Y += incHeight;
            imageControl.LineSegmentDownSize.Point = downSizePoint2;

            Point leftSizePoint = imageControl.LineSegmentLeftSize.Point;
            leftSizePoint.Y += incHeight;
            imageControl.LineSegmentLeftSize.Point = leftSizePoint;

            Point rightSizePoint = imageControl.LineSegmentRightSize.Point;
            rightSizePoint.Y += incHeight;
            imageControl.LineSegmentRightSize.Point = rightSizePoint;

            borderPipePoint2 = imageControl.PolyLineSegmentBorder.Points[1];
            borderPipePoint3 = imageControl.PolyLineSegmentBorder.Points[2];
            borderPipePoint2.Y += incHeight;
            imageControl.PolyLineSegmentBorder.Points[1] = borderPipePoint2;
            borderPipePoint3.Y += incHeight;
            imageControl.PolyLineSegmentBorder.Points[2] = borderPipePoint3;

            imageControl.templateBorder.Height += incHeight;
        }

        public override void DeleteTabItem()
        {
            Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

            if (((MainWindow)MainWindow).TabControlMain.Items.IndexOf(this) >= 0)
                ((MainWindow)MainWindow).TabControlMain.Items.Remove(this);

            ((AppWPF)Application.Current).CollectionTabItemParent.Remove(this.IS.Path);
            ((AppWPF)Application.Current).CollectionPage.Remove(this.IS.Path);
            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Remove(this.IS.Path);
            ((AppWPF)Application.Current).CollectionSaveTabItem.Remove(this);
        }      
    }
}

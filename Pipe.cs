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
    public class Pipe : PipeOnCanvas
    {
        public GeometryDrawing rightSize;
        public GeometryDrawing leftSize;
        public GeometryDrawing pipe;
        public GeometryDrawing topSize;
        public GeometryDrawing downSize;
        public GeometryDrawing rightFlange;
        public GeometryDrawing leftFlange;

        private int intEnvironment;
        public override int IntEnvironment
        {
            get { return intEnvironment; }
            set { intEnvironment = value;

            if (value > 4 || value < 0)
            {
                MessageBox.Show("intEnvironment вышел из диапозона", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            if (value == 4)
            {
                pipe.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushAirPipe");
                leftFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushAirPipeFlange");
                rightFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushAirPipeFlange");
                PipeSer.Environment = 4;
            }
            else if (value == 3)
            {
                pipe.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushMasutPipe");
                leftFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushMasutPipeFlange");
                rightFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushMasutPipeFlange");
                PipeSer.Environment = 3;
            }
            else if (value == 2)
            {
                pipe.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushWaterPipe");
                leftFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushWaterPipeFlange");
                rightFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushWaterPipeFlange");
                PipeSer.Environment = 2;
            }
            else if (value == 0)
            {
                pipe.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushExhaustPipe");
                leftFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushExhaustPipeFlange");
                rightFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushExhaustPipeFlange");
                PipeSer.Environment = 0;
            }
            else if (value == 1)
            {
                pipe.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushSteamPipe");
                leftFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushSteamPipeFlange");
                rightFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushSteamPipeFlange");
                PipeSer.Environment = 1;
            }
            }
        }

        private PipeSer pipeSer;
        public PipeSer PipeSer
        {
            get { return pipeSer; }
            set { controlOnCanvasSer = value; pipeSer = value; }
        }
       
        private bool Issticking;

        private int SumStickingX;
        private int SumStickingY;

        Point p;
        Point Position;

        bool IsRightSize;
        bool IsRightSizeUp;
        bool IsLeftSize;
        bool IsLeftSizeDown;
        bool IsPipeControlMove;
        bool IsRightSizeLeft;
        bool IsLeftSizeRight;
        bool IsRightSizeDown;
        bool IsLeftSizeUp;
        bool IsTopSize;
        bool IsDownSize;
        bool IsTopSizeLeft;
        bool IsDownSizeRight;
        bool IsTopSizeDown;
        bool IsDownSizeTop;
        bool IsTopSizeRight;
        bool IsDownSizeLeft;
        bool IsControlMoveMassive;

        public PathGeometry PathGeometryPipe;
        public PathFigure PathFigurePipe;
        public PolyLineSegment PolyLineSegmentPipe;

        public PathGeometry PathGeometryRightSize;
        public PathFigure PathFigureRightSize;
        public LineSegment LineSegmentRightSize;

        public PathGeometry PathGeometryBorder;
        public PathFigure PathFigureBorder;
        public PolyLineSegment PolyLineSegmentBorder;

        public PathGeometry PathGeometryRightFlange;
        public PathFigure PathFigureRightFlange;
        public PolyLineSegment PolyLineSegmentRightFlange;

        public PathGeometry PathGeometryTopSize;
        public PathFigure PathFigureTopSize;
        public LineSegment LineSegmentTopSize;

        public PathGeometry PathGeometryDownSize;
        public PathFigure PathFigureDownSize;
        public LineSegment LineSegmentDownSize;

        public PathGeometry PathGeometryLeftFlange;
        public PathFigure PathFigureLeftFlange;
        public PolyLineSegment PolyLineSegmentLeftFlange;

        public PathGeometry PathGeometryLeftSize;
        public PathFigure PathFigureLeftSize;
        public LineSegment LineSegmentLeftSize;
     
        static Pipe()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pipe), new FrameworkPropertyMetadata(typeof(Pipe)));
        }

        public Pipe(PageScada ps, CanvasPage canvasPage, PipeSer pipeSer)
            : base(pipeSer)
        {
            this.Focusable = false;
            PS = ps;
            CanvasPage = canvasPage;
            PipeSer = pipeSer;
            intEnvironment = pipeSer.Environment; // Присваиваем закрытому полю, в загрузке шаблона присваиваем уже свойству     

            menuItemProperties.Click += Properties;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            rightSize = GetTemplateChild("RightSize") as GeometryDrawing;
            leftSize = GetTemplateChild("LeftSize") as GeometryDrawing;
            pipe = GetTemplateChild("Pipe") as GeometryDrawing;
            border = GetTemplateChild("Border") as GeometryDrawing;
            topSize = GetTemplateChild("TopSize") as GeometryDrawing;
            downSize = GetTemplateChild("DownSize") as GeometryDrawing;
            rightFlange = GetTemplateChild("RightFlange") as GeometryDrawing;
            leftFlange = GetTemplateChild("LeftFlange") as GeometryDrawing;

            PathGeometryPipe = (PathGeometry)pipe.Geometry;
            PathFigurePipe = PathGeometryPipe.Figures[0];
            PolyLineSegmentPipe = (PolyLineSegment)PathFigurePipe.Segments[0];

            PathGeometryRightSize = (PathGeometry)rightSize.Geometry;
            PathFigureRightSize = PathGeometryRightSize.Figures[0];
            LineSegmentRightSize = (LineSegment)PathFigureRightSize.Segments[0];

            PathGeometryBorder = (PathGeometry)border.Geometry;
            PathFigureBorder = PathGeometryBorder.Figures[0];
            PolyLineSegmentBorder = (PolyLineSegment)PathFigureBorder.Segments[0];

            PathGeometryRightFlange = (PathGeometry)rightFlange.Geometry;
            PathFigureRightFlange = PathGeometryRightFlange.Figures[0];
            PolyLineSegmentRightFlange = (PolyLineSegment)PathFigureRightFlange.Segments[0];

            PathGeometryTopSize = (PathGeometry)topSize.Geometry;
            PathFigureTopSize = PathGeometryTopSize.Figures[0];
            LineSegmentTopSize = (LineSegment)PathFigureTopSize.Segments[0];

            PathGeometryDownSize = (PathGeometry)downSize.Geometry;
            PathFigureDownSize = PathGeometryDownSize.Figures[0];
            LineSegmentDownSize = (LineSegment)PathFigureDownSize.Segments[0];

            PathGeometryLeftFlange = (PathGeometry)leftFlange.Geometry;
            PathFigureLeftFlange = PathGeometryLeftFlange.Figures[0];
            PolyLineSegmentLeftFlange = (PolyLineSegment)PathFigureLeftFlange.Segments[0];

            PathGeometryLeftSize = (PathGeometry)leftSize.Geometry;
            PathFigureLeftSize = PathGeometryLeftSize.Figures[0];
            LineSegmentLeftSize = (LineSegment)PathFigureLeftSize.Segments[0];

            Point topSizePoint = PathFigureTopSize.StartPoint;

            Point downSizePoint = PathFigureDownSize.StartPoint;

            Diameter = (downSizePoint.Y - topSizePoint.Y);

            // Присваиваем в загрузки шаблона, чтобы были инициализированы закрашиваемые объекты
            IntEnvironment = intEnvironment;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if(IsSelected)
            {
                DialogWindowPropertiesPipe WindowProperties = new DialogWindowPropertiesPipe(this);
                WindowProperties.Owner = Application.Current.MainWindow;
                WindowProperties.ShowDialog();
            }
            
            e.Handled = true;
        }

        private void Properties(object sender, RoutedEventArgs e)
        {            
            DialogWindowPropertiesPipe WindowProperties = new DialogWindowPropertiesPipe(this);
            WindowProperties.Owner = Application.Current.MainWindow;
            WindowProperties.ShowDialog();

            e.Handled = true;
        }
            
        public void ChangePipeSer()
        {
            pipeSer.LeftSize.point[0] = PathFigureLeftSize.StartPoint;
            pipeSer.LeftSize.point[1] = LineSegmentLeftSize.Point;

            pipeSer.RightSize.point[0] = PathFigureRightSize.StartPoint;
            pipeSer.RightSize.point[1] = LineSegmentRightSize.Point;

            pipeSer.TopSize.point[0] = PathFigureTopSize.StartPoint;
            pipeSer.TopSize.point[1] = LineSegmentTopSize.Point;

            pipeSer.DownSize.point[0] = PathFigureDownSize.StartPoint;
            pipeSer.DownSize.point[1] = LineSegmentDownSize.Point;

            pipeSer.LeftFlange.point[0] = PathFigureLeftFlange.StartPoint;
            pipeSer.LeftFlange.point[1] = PolyLineSegmentLeftFlange.Points[0];
            pipeSer.LeftFlange.point[2] = PolyLineSegmentLeftFlange.Points[1];
            pipeSer.LeftFlange.point[3] = PolyLineSegmentLeftFlange.Points[2];
            pipeSer.LeftFlange.point[4] = PolyLineSegmentLeftFlange.Points[3];

            pipeSer.RightFlange.point[0] = PathFigureRightFlange.StartPoint;
            pipeSer.RightFlange.point[1] = PolyLineSegmentRightFlange.Points[0];
            pipeSer.RightFlange.point[2] = PolyLineSegmentRightFlange.Points[1];
            pipeSer.RightFlange.point[3] = PolyLineSegmentRightFlange.Points[2];
            pipeSer.RightFlange.point[4] = PolyLineSegmentRightFlange.Points[3];

            pipeSer.Pipe.point[0] = PathFigurePipe.StartPoint;
            pipeSer.Pipe.point[1] = PolyLineSegmentPipe.Points[0];
            pipeSer.Pipe.point[2] = PolyLineSegmentPipe.Points[1];
            pipeSer.Pipe.point[3] = PolyLineSegmentPipe.Points[2];
            pipeSer.Pipe.point[4] = PolyLineSegmentPipe.Points[3];

            pipeSer.BorderPipe.point[0] = PathFigureBorder.StartPoint;
            pipeSer.BorderPipe.point[1] = PolyLineSegmentBorder.Points[0];
            pipeSer.BorderPipe.point[2] = PolyLineSegmentBorder.Points[1];
            pipeSer.BorderPipe.point[3] = PolyLineSegmentBorder.Points[2];
            pipeSer.BorderPipe.point[4] = PolyLineSegmentBorder.Points[3];

            pipeSer.Сoordinates = new Point((double)this.GetValue(Canvas.LeftProperty), (double)this.GetValue(Canvas.TopProperty));

            ((AppWPF)Application.Current).SaveTabItem(CanvasPage.TabItemPage);
        }
                  
        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);

            if (!this.IsSelected) e.Handled = true;          
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
           
            Point translatePointMainWindow = e.GetPosition(Application.Current.MainWindow);
            Point translatePointCanvas = e.GetPosition(CanvasPage);

            p.X = p.X - translatePointMainWindow.X;
            p.Y = p.Y - translatePointMainWindow.Y;
          
            ((MainWindow)Application.Current.MainWindow).LabelCoordinateCursor.Content = string.Format("{0}{1:F2}{2}{3}{4:F2}", "X: ", translatePointCanvas.X, " ", "Y: ", translatePointCanvas.Y);

            #region IsLeftSize
            if (IsLeftSize)
            {               
                Point test3 = PathFigurePipe.StartPoint;

                if (test3.X < 20)
                {
                    if ((p.X < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                p.X *= -1;

                Point p2 = PathFigureRightSize.StartPoint;
                p2.X -= p.X;
                PathFigureRightSize.StartPoint = p2;
                Point p3 = LineSegmentRightSize.Point;
                p3.X -= p.X;
                LineSegmentRightSize.Point = p3;

                Point pipe2 = PathFigurePipe.StartPoint;
                pipe2.X -= p.X;
                PathFigurePipe.StartPoint = pipe2;
                Point pipe3 = PolyLineSegmentPipe.Points[0];
                Point pipe4 = PolyLineSegmentPipe.Points[3];
                pipe3.X -= p.X;
                pipe4.X -= p.X;
                PolyLineSegmentPipe.Points[0] = pipe3;
                PolyLineSegmentPipe.Points[3] = pipe4;
            
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X -= p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X -= p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X -= p.X;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[2];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[3];
                rightFlangePoint2.X -= p.X;
                rightFlangePoint3.X -= p.X;
                rightFlangePoint4.X -= p.X;
                rightFlangePoint5.X -= p.X;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint5;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X -= p.X;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X -= p.X;
                LineSegmentDownSize.Point = downSizePoint;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x += p.X;

                this.SetValue(Canvas.LeftProperty, x);

                Coordinate();
            }
            else if (IsLeftSizeDown)
            {
                Point test3 = PathFigurePipe.StartPoint;

                if (test3.X < 20)
                {
                    if ((p.Y > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                p.Y *= -1;

                Point p2 = PathFigureRightSize.StartPoint;
                p2.X += p.Y;
                PathFigureRightSize.StartPoint = p2;
                Point p3 = LineSegmentRightSize.Point;
                p3.X += p.Y;
                LineSegmentRightSize.Point = p3;

                Point pipe2 = PathFigurePipe.StartPoint;
                pipe2.X += p.Y;
                PathFigurePipe.StartPoint = pipe2;
                Point pipe3 = PolyLineSegmentPipe.Points[0];
                Point pipe4 = PolyLineSegmentPipe.Points[3];
                pipe3.X += p.Y;
                pipe4.X += p.Y;
                PolyLineSegmentPipe.Points[0] = pipe3;
                PolyLineSegmentPipe.Points[3] = pipe4;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X += p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.Y;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[2];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[3];
                rightFlangePoint2.X += p.Y;
                rightFlangePoint3.X += p.Y;
                rightFlangePoint4.X += p.Y;
                rightFlangePoint5.X += p.Y;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint5;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X += p.Y;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X += p.Y;
                LineSegmentDownSize.Point = downSizePoint;

                double x = (double)this.GetValue(Canvas.TopProperty);
                x += p.Y;

                this.SetValue(Canvas.TopProperty, x);
            }            
            else if (IsLeftSizeRight)
            {
                Point test3 = PathFigurePipe.StartPoint;

                if (test3.X < 20)
                {
                    if ((p.X > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                p.X *= -1;

                Point p2 = PathFigureRightSize.StartPoint;
                p2.X += p.X;
                PathFigureRightSize.StartPoint = p2;
                Point p3 = LineSegmentRightSize.Point;
                p3.X += p.X;
                LineSegmentRightSize.Point = p3;

                Point pipe2 = PathFigurePipe.StartPoint;
                pipe2.X += p.X;
                PathFigurePipe.StartPoint = pipe2;
                Point pipe3 = PolyLineSegmentPipe.Points[0];
                Point pipe4 = PolyLineSegmentPipe.Points[3];
                pipe3.X += p.X;
                pipe4.X += p.X;
                PolyLineSegmentPipe.Points[0] = pipe3;
                PolyLineSegmentPipe.Points[3] = pipe4;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X += p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X += p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.X;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[2];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[3];
                rightFlangePoint2.X += p.X;
                rightFlangePoint3.X += p.X;
                rightFlangePoint4.X += p.X;
                rightFlangePoint5.X += p.X;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint5;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X += p.X;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X += p.X;
                LineSegmentDownSize.Point = downSizePoint;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x += p.X;

                this.SetValue(Canvas.LeftProperty, x);
            }
            else if (IsLeftSizeUp)
            {
                Point test3 = PathFigurePipe.StartPoint;

                if (test3.X < 20)
                {
                    if ((p.Y < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                p.Y *= -1;

                Point p2 = PathFigureRightSize.StartPoint;
                p2.X -= p.Y;
                PathFigureRightSize.StartPoint = p2;
                Point p3 = LineSegmentRightSize.Point;
                p3.X -= p.Y;
                LineSegmentRightSize.Point = p3;

                Point pipe2 = PathFigurePipe.StartPoint;
                pipe2.X -= p.Y;
                PathFigurePipe.StartPoint = pipe2;
                Point pipe3 = PolyLineSegmentPipe.Points[0];
                Point pipe4 = PolyLineSegmentPipe.Points[3];
                pipe3.X -= p.Y;
                pipe4.X -= p.Y;
                PolyLineSegmentPipe.Points[0] = pipe3;
                PolyLineSegmentPipe.Points[3] = pipe4;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X -= p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X -= p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X -= p.Y;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[2];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[3];
                rightFlangePoint2.X -= p.Y;
                rightFlangePoint3.X -= p.Y;
                rightFlangePoint4.X -= p.Y;
                rightFlangePoint5.X -= p.Y;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint5;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X -= p.Y;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X -= p.Y;
                LineSegmentDownSize.Point = downSizePoint;

                double x = (double)this.GetValue(Canvas.TopProperty);
                x += p.Y;

                this.SetValue(Canvas.TopProperty, x);

                Coordinate();
            }
            #endregion
            #region IsRightSize
            if (IsRightSize)
            {
                Point test3 = PathFigurePipe.StartPoint;

                if (test3.X < 20)
                {
                    if ((p.X > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point p2 = PathFigureRightSize.StartPoint;
                p2.X -= p.X;
                PathFigureRightSize.StartPoint = p2;
                Point p3 = LineSegmentRightSize.Point;
                p3.X -= p.X;
                LineSegmentRightSize.Point = p3;

                Point pipe2 = PathFigurePipe.StartPoint;
                pipe2.X -= p.X;
                PathFigurePipe.StartPoint = pipe2;
                Point pipe3 = PolyLineSegmentPipe.Points[0];
                Point pipe4 = PolyLineSegmentPipe.Points[3];
                pipe3.X -= p.X;
                pipe4.X -= p.X;
                PolyLineSegmentPipe.Points[0] = pipe3;
                PolyLineSegmentPipe.Points[3] = pipe4;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X -= p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X -= p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X -= p.X;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[2];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[3];
                rightFlangePoint2.X -= p.X;
                rightFlangePoint3.X -= p.X;
                rightFlangePoint4.X -= p.X;
                rightFlangePoint5.X -= p.X;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint5;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X -= p.X;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X -= p.X;
                LineSegmentDownSize.Point = downSizePoint;
            }
            else if (IsRightSizeUp)
            {
                Point test3 = PathFigurePipe.StartPoint;

                if (test3.X < 20)
                {
                    if ((p.Y < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point p2 = PathFigureRightSize.StartPoint;
                p2.X += p.Y;
                PathFigureRightSize.StartPoint = p2;
                Point p3 = LineSegmentRightSize.Point;
                p3.X += p.Y;
                LineSegmentRightSize.Point = p3;

                Point pipe2 = PathFigurePipe.StartPoint;
                pipe2.X += p.Y;
                PathFigurePipe.StartPoint = pipe2;
                Point pipe3 = PolyLineSegmentPipe.Points[0];
                Point pipe4 = PolyLineSegmentPipe.Points[3];
                pipe3.X += p.Y;
                pipe4.X += p.Y;
                PolyLineSegmentPipe.Points[0] = pipe3;
                PolyLineSegmentPipe.Points[3] = pipe4;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X += p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.Y;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[2];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[3];
                rightFlangePoint2.X += p.Y;
                rightFlangePoint3.X += p.Y;
                rightFlangePoint4.X += p.Y;
                rightFlangePoint5.X += p.Y;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint5;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X += p.Y;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X += p.Y;
                LineSegmentDownSize.Point = downSizePoint;

                Coordinate();
            }
            else if (IsRightSizeLeft)
            {
                Point test3 = PathFigurePipe.StartPoint;

                if (test3.X < 20)
                {
                    if ((p.X < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point p2 = PathFigureRightSize.StartPoint;
                p2.X += p.X;
                PathFigureRightSize.StartPoint = p2;
                Point p3 = LineSegmentRightSize.Point;
                p3.X += p.X;
                LineSegmentRightSize.Point = p3;

                Point pipe2 = PathFigurePipe.StartPoint;
                pipe2.X += p.X;
                PathFigurePipe.StartPoint = pipe2;
                Point pipe3 = PolyLineSegmentPipe.Points[0];
                Point pipe4 = PolyLineSegmentPipe.Points[3];
                pipe3.X += p.X;
                pipe4.X += p.X;
                PolyLineSegmentPipe.Points[0] = pipe3;
                PolyLineSegmentPipe.Points[3] = pipe4;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X += p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X += p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.X;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[2];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[3];
                rightFlangePoint2.X += p.X;
                rightFlangePoint3.X += p.X;
                rightFlangePoint4.X += p.X;
                rightFlangePoint5.X += p.X;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint5;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X += p.X;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X += p.X;
                LineSegmentDownSize.Point = downSizePoint;

                Coordinate();
            }
            else if (IsRightSizeDown)
            {
                Point test3 = PathFigurePipe.StartPoint;

                if (test3.X < 20)
                {
                    if ((p.Y > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point p2 = PathFigureRightSize.StartPoint;
                p2.X -= p.Y;
                PathFigureRightSize.StartPoint = p2;
                Point p3 = LineSegmentRightSize.Point;
                p3.X -= p.Y;
                LineSegmentRightSize.Point = p3;

                Point pipe2 = PathFigurePipe.StartPoint;
                pipe2.X -= p.Y;
                PathFigurePipe.StartPoint = pipe2;
                Point pipe3 = PolyLineSegmentPipe.Points[0];
                Point pipe4 = PolyLineSegmentPipe.Points[3];
                pipe3.X -= p.Y;
                pipe4.X -= p.Y;
                PolyLineSegmentPipe.Points[0] = pipe3;
                PolyLineSegmentPipe.Points[3] = pipe4;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X -= p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X -= p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X -= p.Y;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[2];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[3];
                rightFlangePoint2.X -= p.Y;
                rightFlangePoint3.X -= p.Y;
                rightFlangePoint4.X -= p.Y;
                rightFlangePoint5.X -= p.Y;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint5;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X -= p.Y;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X -= p.Y;
                LineSegmentDownSize.Point = downSizePoint;
            }           
            #endregion
            #region IsTopSize
            else if (IsTopSize)
            {
                Point topSizePoint = PathFigureTopSize.StartPoint;
                Point downSizePoint = PathFigureDownSize.StartPoint;

                if ((downSizePoint.Y - topSizePoint.Y) + p.Y < 10)
                {
                    if ((p.Y < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
                else if ((downSizePoint.Y - topSizePoint.Y) + p.Y > 500)
                {
                    if ((p.Y > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                p.Y *= -1;
                           
                downSizePoint.Y -= p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y -= p.Y;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = PathFigureRightSize.StartPoint;
                rightSizePoint.Y -= p.Y;
                PathFigureRightSize.StartPoint = rightSizePoint;

                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y -= p.Y;
                rightFlangePoint3.Y -= p.Y;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint3;

                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[2];
                leftFlangePoint2.Y -= p.Y;
                leftFlangePoint3.Y -= p.Y;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint3;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y -= p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigurePipe.StartPoint;
                pointPipe.Y -= p.Y;
                PathFigurePipe.StartPoint = pointPipe;
                Point pointPipe2 = PolyLineSegmentPipe.Points[2];
                Point pointPipe3 = PolyLineSegmentPipe.Points[3];
                pointPipe2.Y -= p.Y;
                pointPipe3.Y -= p.Y;
                PolyLineSegmentPipe.Points[2] = pointPipe2;
                PolyLineSegmentPipe.Points[3] = pointPipe3;

                double y = (double)this.GetValue(Canvas.TopProperty);
                y += p.Y;

                this.SetValue(Canvas.TopProperty, y);

                Coordinate();
            }
            else if (IsTopSizeLeft)
            {
                Point topSizePoint = PathFigureTopSize.StartPoint;
                Point downSizePoint = PathFigureDownSize.StartPoint;

                if ((downSizePoint.Y - topSizePoint.Y) + p.X < 10)
                {
                    if ((p.X < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
                else if ((downSizePoint.Y - topSizePoint.Y) + p.X > 500)
                {
                    if ((p.X > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                p.X *= -1;
               
                downSizePoint.Y -= p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y -= p.X;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = PathFigureRightSize.StartPoint;
                rightSizePoint.Y -= p.X;
                PathFigureRightSize.StartPoint = rightSizePoint;

                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y -= p.X;
                rightFlangePoint3.Y -= p.X;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint3;

                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[2];
                leftFlangePoint2.Y -= p.X;
                leftFlangePoint3.Y -= p.X;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint3;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y -= p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigurePipe.StartPoint;
                pointPipe.Y -= p.X;
                PathFigurePipe.StartPoint = pointPipe;
                Point pointPipe2 = PolyLineSegmentPipe.Points[2];
                Point pointPipe3 = PolyLineSegmentPipe.Points[3];
                pointPipe2.Y -= p.X;
                pointPipe3.Y -= p.X;
                PolyLineSegmentPipe.Points[2] = pointPipe2;
                PolyLineSegmentPipe.Points[3] = pointPipe3;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x += p.X;

                this.SetValue(Canvas.LeftProperty, x);

                Coordinate();
            }
            else if (IsTopSizeDown)
            {
                Point topSizePoint = PathFigureTopSize.StartPoint;
                Point downSizePoint = PathFigureDownSize.StartPoint;

                if ((downSizePoint.Y - topSizePoint.Y) - p.Y < 10)
                {
                    if ((p.Y > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
                else if ((downSizePoint.Y - topSizePoint.Y) - p.Y > 500)
                {
                    if ((p.Y < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
               
                p.Y *= -1;
                
                downSizePoint.Y += p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y += p.Y;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = PathFigureRightSize.StartPoint;
                rightSizePoint.Y += p.Y;
                PathFigureRightSize.StartPoint = rightSizePoint;

                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.Y;
                rightFlangePoint3.Y += p.Y;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint3;

                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[2];
                leftFlangePoint2.Y += p.Y;
                leftFlangePoint3.Y += p.Y;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint3;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigurePipe.StartPoint;
                pointPipe.Y += p.Y;
                PathFigurePipe.StartPoint = pointPipe;
                Point pointPipe2 = PolyLineSegmentPipe.Points[2];
                Point pointPipe3 = PolyLineSegmentPipe.Points[3];
                pointPipe2.Y += p.Y;
                pointPipe3.Y += p.Y;
                PolyLineSegmentPipe.Points[2] = pointPipe2;
                PolyLineSegmentPipe.Points[3] = pointPipe3;

                double y = (double)this.GetValue(Canvas.TopProperty);
                y += p.Y;

                this.SetValue(Canvas.TopProperty, y);
            }
            else if (IsTopSizeRight)
            {
                Point topSizePoint = PathFigureTopSize.StartPoint;
                Point downSizePoint = PathFigureDownSize.StartPoint;

                if ((downSizePoint.Y - topSizePoint.Y) - p.X < 10)
                {
                    if ((p.X > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
                else if ((downSizePoint.Y - topSizePoint.Y) - p.X > 500)
                {
                    if ((p.X < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
            
                p.X *= -1;
               
                downSizePoint.Y += p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y += p.X;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = PathFigureRightSize.StartPoint;
                rightSizePoint.Y += p.X;
                PathFigureRightSize.StartPoint = rightSizePoint;

                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.X;
                rightFlangePoint3.Y += p.X;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint3;

                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[2];
                leftFlangePoint2.Y += p.X;
                leftFlangePoint3.Y += p.X;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint3;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y += p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigurePipe.StartPoint;
                pointPipe.Y += p.X;
                PathFigurePipe.StartPoint = pointPipe;
                Point pointPipe2 = PolyLineSegmentPipe.Points[2];
                Point pointPipe3 = PolyLineSegmentPipe.Points[3];
                pointPipe2.Y += p.X;
                pointPipe3.Y += p.X;
                PolyLineSegmentPipe.Points[2] = pointPipe2;
                PolyLineSegmentPipe.Points[3] = pointPipe3;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x += p.X;

                this.SetValue(Canvas.LeftProperty, x);
            }
            #endregion
            #region IsDownSize
            else if (IsDownSize)
            {
                Point topSizePoint = PathFigureTopSize.StartPoint;
                Point downSizePoint = PathFigureDownSize.StartPoint;

                if ((downSizePoint.Y - topSizePoint.Y) - p.Y < 10)
                {
                    if ((p.Y > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
                else if ((downSizePoint.Y - topSizePoint.Y) - p.Y > 500)
                {
                    if ((p.Y < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
              
                downSizePoint.Y -= p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                PathGeometry g2 = (PathGeometry)leftSize.Geometry;
                PathFigure pf2 = g2.Figures[0];
                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y -= p.Y;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = PathFigureRightSize.StartPoint;
                rightSizePoint.Y -= p.Y;
                PathFigureRightSize.StartPoint = rightSizePoint;

                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y -= p.Y;
                rightFlangePoint3.Y -= p.Y;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint3;

                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[2];
                leftFlangePoint2.Y -= p.Y;
                leftFlangePoint3.Y -= p.Y;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint3;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y -= p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigurePipe.StartPoint;
                pointPipe.Y -= p.Y;
                PathFigurePipe.StartPoint = pointPipe;
                Point pointPipe2 = PolyLineSegmentPipe.Points[2];
                Point pointPipe3 = PolyLineSegmentPipe.Points[3];
                pointPipe2.Y -= p.Y;
                pointPipe3.Y -= p.Y;
                PolyLineSegmentPipe.Points[2] = pointPipe2;
                PolyLineSegmentPipe.Points[3] = pointPipe3;
            }
            else if (IsDownSizeRight)
            {
                Point topSizePoint = PathFigureTopSize.StartPoint;
                Point downSizePoint = PathFigureDownSize.StartPoint;

                if ((downSizePoint.Y - topSizePoint.Y) - p.X < 10)
                {
                    if ((p.X > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
                else if ((downSizePoint.Y - topSizePoint.Y) - p.X > 500)
                {
                    if ((p.X < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
              
                downSizePoint.Y -= p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y -= p.X;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = PathFigureRightSize.StartPoint;
                rightSizePoint.Y -= p.X;
                PathFigureRightSize.StartPoint = rightSizePoint;

                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y -= p.X;
                rightFlangePoint3.Y -= p.X;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint3;

                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[2];
                leftFlangePoint2.Y -= p.X;
                leftFlangePoint3.Y -= p.X;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint3;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y -= p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigurePipe.StartPoint;
                pointPipe.Y -= p.X;
                PathFigurePipe.StartPoint = pointPipe;
                Point pointPipe2 = PolyLineSegmentPipe.Points[2];
                Point pointPipe3 = PolyLineSegmentPipe.Points[3];
                pointPipe2.Y -= p.X;
                pointPipe3.Y -= p.X;
                PolyLineSegmentPipe.Points[2] = pointPipe2;
                PolyLineSegmentPipe.Points[3] = pointPipe3;
            }
            else if (IsDownSizeTop)
            {
                Point topSizePoint = PathFigureTopSize.StartPoint;
                Point downSizePoint = PathFigureDownSize.StartPoint;

                if ((downSizePoint.Y - topSizePoint.Y) + p.Y < 10)
                {
                    if ((p.Y < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
                else if ((downSizePoint.Y - topSizePoint.Y) + p.Y > 500)
                {
                    if ((p.Y > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
             
                downSizePoint.Y += p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y += p.Y;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = PathFigureRightSize.StartPoint;
                rightSizePoint.Y += p.Y;
                PathFigureRightSize.StartPoint = rightSizePoint;

                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.Y;
                rightFlangePoint3.Y += p.Y;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint3;

                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[2];
                leftFlangePoint2.Y += p.Y;
                leftFlangePoint3.Y += p.Y;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint3;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigurePipe.StartPoint;
                pointPipe.Y += p.Y;
                PathFigurePipe.StartPoint = pointPipe;
                Point pointPipe2 = PolyLineSegmentPipe.Points[2];
                Point pointPipe3 = PolyLineSegmentPipe.Points[3];
                pointPipe2.Y += p.Y;
                pointPipe3.Y += p.Y;
                PolyLineSegmentPipe.Points[2] = pointPipe2;
                PolyLineSegmentPipe.Points[3] = pointPipe3;

                Coordinate();
            }
            else if (IsDownSizeLeft)
            {
                Point topSizePoint = PathFigureTopSize.StartPoint;
                Point downSizePoint = PathFigureDownSize.StartPoint;

                if ((downSizePoint.Y - topSizePoint.Y) + p.X < 10)
                {
                    if ((p.X < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
                else if ((downSizePoint.Y - topSizePoint.Y) + p.X > 500)
                {
                    if ((p.X > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }
             
                downSizePoint.Y += p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y += p.X;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = PathFigureRightSize.StartPoint;
                rightSizePoint.Y += p.X;
                PathFigureRightSize.StartPoint = rightSizePoint;

                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.X;
                rightFlangePoint3.Y += p.X;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint3;

                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[2];
                leftFlangePoint2.Y += p.X;
                leftFlangePoint3.Y += p.X;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint3;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y += p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigurePipe.StartPoint;
                pointPipe.Y += p.X;
                PathFigurePipe.StartPoint = pointPipe;
                Point pointPipe2 = PolyLineSegmentPipe.Points[2];
                Point pointPipe3 = PolyLineSegmentPipe.Points[3];
                pointPipe2.Y += p.X;
                pointPipe3.Y += p.X;
                PolyLineSegmentPipe.Points[2] = pointPipe2;
                PolyLineSegmentPipe.Points[3] = pointPipe3;

                Coordinate();
            }
            #endregion
            else if (IsControlMoveMassive)
            {
                double transformX = (double)this.GetValue(Canvas.LeftProperty);
                double transformY = (double)this.GetValue(Canvas.TopProperty);

                if (controlOnCanvasSer.Transform == -90 || controlOnCanvasSer.Transform == 270)
                {
                    transformY -= ActualWidth;
                }
                else if (controlOnCanvasSer.Transform == -180 || controlOnCanvasSer.Transform == 180)
                {
                    transformY -= this.ActualHeight;
                    transformX -= this.ActualWidth;
                }
                else if (controlOnCanvasSer.Transform == -270 || controlOnCanvasSer.Transform == 90)
                {
                    transformX -= this.ActualHeight;
                }

                CoordinateX.Text = string.Format("{0:F2}", transformX);
                CoordinateY.Text = string.Format("{0:F2}", transformY);

                foreach (ControlOnCanvasPage controlMove in this.CanvasPage.Children)
                {
                    if (controlMove.IsSelected)
                    {
                        double x = (double)controlMove.GetValue(Canvas.LeftProperty);
                        x -= p.X;

                        double y = (double)controlMove.GetValue(Canvas.TopProperty);
                        y -= p.Y;

                        controlMove.SetValue(Canvas.LeftProperty, x);
                        controlMove.SetValue(Canvas.TopProperty, y);
                    }                        
                }
            }
            else if (IsPipeControlMove && CanvasPage.CountSelect <= 1)
            {
                double x = (double)this.GetValue(Canvas.LeftProperty);
                x -= p.X;

                double y = (double)this.GetValue(Canvas.TopProperty);
                y -= p.Y;

                foreach (ControlOnCanvasPage controlOnCanvas in this.CanvasPage.Children)
                {
                    if (controlOnCanvas is Pipe)
                    {
                        Pipe pipe = controlOnCanvas as Pipe;

                        if (this != pipe)
                        {                                                      
                            point1 = this.TranslatePoint(PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe.TranslatePoint(pipe.LineSegmentRightSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe.TranslatePoint(pipe.PathFigureRightSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }

                            if (!Issticking)
                            {   
                                if (this != pipe)
                                {
                                    point1 = this.TranslatePoint(PathFigureRightSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(LineSegmentRightSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe.TranslatePoint(pipe.LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe.TranslatePoint(pipe.PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

                                    double x1_2, y1_2, x2_2, y2_2, x3_2, y3_2, x4_2, y4_2;

                                    x1_2 = point1.X;
                                    y1_2 = point1.Y;

                                    x2_2 = point3.X;
                                    y2_2 = point3.Y;

                                    x3_2 = point2.X;
                                    y3_2 = point2.Y;

                                    x4_2 = point4.X;
                                    y4_2 = point4.Y;

                                    int test5 = (int)Math.Abs(Math.Abs(x1_2) - Math.Abs(x2_2));
                                    int test6 = (int)Math.Abs(Math.Abs(y1_2) - Math.Abs(y2_2));
                                    int test7 = (int)Math.Abs(Math.Abs(x3_2) - Math.Abs(x4_2));
                                    int test8 = (int)Math.Abs(Math.Abs(y3_2) - Math.Abs(y4_2));

                                    if (x1_2 == x2_2 && y1_2 == y2_2 && x3_2 == x4_2 && y3_2 == y4_2)
                                    {
                                        Issticking = true;
                                    }
                                    else if (test5 < 10 && test6 < 10 && test7 < 10 && test8 < 10)
                                    {
                                        this.SetValue(Canvas.LeftProperty, x - (x1_2 - x2_2));
                                        this.SetValue(Canvas.TopProperty, y - (y1_2 - y2_2));

                                        Issticking = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (controlOnCanvas is Pipe90)
                    {
                        Pipe90 pipe90 = controlOnCanvas as Pipe90;

                        if ((int)this.RenderTransform.Value.M11 == 1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe90.RenderTransform.Value.M11 == 1 && (int)pipe90.RenderTransform.Value.M12 == 0)
                        {
                            point1 = this.TranslatePoint(PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe90.RenderTransform.Value.M11 == 1 && (int)pipe90.RenderTransform.Value.M12 == 0)
                        {
                            point1 = this.TranslatePoint(PathFigureRightSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentRightSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && (int)pipe90.RenderTransform.Value.M11 == 1 && (int)pipe90.RenderTransform.Value.M12 == 0)
                        {
                            point1 = this.TranslatePoint(PathFigureRightSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentRightSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1 && (int)pipe90.RenderTransform.Value.M11 == 1 && (int)pipe90.RenderTransform.Value.M12 == 0)
                        {
                            point1 = this.TranslatePoint(PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == 1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe90.RenderTransform.Value.M11 == 0 && (int)pipe90.RenderTransform.Value.M12 == -1)
                        {
                            point1 = this.TranslatePoint(PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && (int)pipe90.RenderTransform.Value.M11 == 0 && (int)pipe90.RenderTransform.Value.M12 == -1)
                        {
                            point1 = this.TranslatePoint(PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1 && (int)pipe90.RenderTransform.Value.M11 == 0 && (int)pipe90.RenderTransform.Value.M12 == -1)
                        {
                            point1 = this.TranslatePoint(PathFigureRightSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentRightSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == 1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe90.RenderTransform.Value.M11 == -1 && (int)pipe90.RenderTransform.Value.M12 == 0)
                        {
                            point1 = this.TranslatePoint(PathFigureRightSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentRightSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe90.RenderTransform.Value.M11 == -1 && (int)pipe90.RenderTransform.Value.M12 == 0)
                        {
                            point1 = this.TranslatePoint(PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && (int)pipe90.RenderTransform.Value.M11 == -1 && (int)pipe90.RenderTransform.Value.M12 == 0)
                        {
                            point1 = this.TranslatePoint(PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1 && (int)pipe90.RenderTransform.Value.M11 == -1 && (int)pipe90.RenderTransform.Value.M12 == 0)
                        {
                            point1 = this.TranslatePoint(PathFigureRightSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentRightSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == 1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe90.RenderTransform.Value.M11 == 0 && (int)pipe90.RenderTransform.Value.M12 == 1)
                        {
                            point1 = this.TranslatePoint(PathFigureRightSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentRightSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe90.RenderTransform.Value.M11 == 0 && (int)pipe90.RenderTransform.Value.M12 == 1)
                        {
                            point1 = this.TranslatePoint(PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && (int)pipe90.RenderTransform.Value.M11 == 0 && (int)pipe90.RenderTransform.Value.M12 == 1)
                        {
                            point1 = this.TranslatePoint(PathFigureRightSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentRightSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1 && (int)pipe90.RenderTransform.Value.M11 == 0 && (int)pipe90.RenderTransform.Value.M12 == 1)
                        {
                            point1 = this.TranslatePoint(PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point2 = this.TranslatePoint(LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point3 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                            point4 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

                            double x1, y1, x2, y2, x3, y3, x4, y4;

                            x1 = point1.X;
                            y1 = point1.Y;

                            x2 = point3.X;
                            y2 = point3.Y;

                            x3 = point2.X;
                            y3 = point2.Y;

                            x4 = point4.X;
                            y4 = point4.Y;

                            int test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                            int test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                            int test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                            int test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                            if (x1 == x2 && y1 == y2 && x3 == x4 && y3 == y4)
                            {
                                Issticking = true;
                            }
                            else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                            {
                                this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                Issticking = true;
                            }
                        }
                    }
                }
                if (Issticking)
                {
                    SumStickingX += (int)p.X;
                    SumStickingY += (int)p.Y;

                    if (SumStickingX > 50 || SumStickingY > 50 || SumStickingX < -50 || SumStickingY < -50)
                    {
                        Issticking = false;

                        this.SetValue(Canvas.LeftProperty, x - SumStickingX);
                        this.SetValue(Canvas.TopProperty, y - SumStickingY);

                        SumStickingX = 0;
                        SumStickingY = 0;
                    }
                }
                else
                {
                    this.SetValue(Canvas.LeftProperty, x);
                    this.SetValue(Canvas.TopProperty, y);
                }

                double transformX = (double)this.GetValue(Canvas.LeftProperty);
                double transformY = (double)this.GetValue(Canvas.TopProperty);

                if (controlOnCanvasSer.Transform == -90 || controlOnCanvasSer.Transform == 270)
                {
                    transformY -= ActualWidth;
                }
                else if (controlOnCanvasSer.Transform == -180 || controlOnCanvasSer.Transform == 180)
                {
                    transformY -= this.ActualHeight;
                    transformX -= this.ActualWidth;
                }
                else if (controlOnCanvasSer.Transform == -270 || controlOnCanvasSer.Transform == 90)
                {
                    transformX -= this.ActualHeight;
                }
               
                CoordinateX.Text = string.Format("{0:F2}", transformX);
                CoordinateY.Text = string.Format("{0:F2}", transformY);
            }
            if(!this.IsMouseCaptured)
            {
                if (!this.IsSelected || this.CanvasPage.CountSelect > 1)
                {
                    this.Cursor = null;

                    p = e.GetPosition(Application.Current.MainWindow);

                    e.Handled = true;
                    return;
                }
                if (rightSize.Geometry.StrokeContains(rightSize.Pen, e.GetPosition(this)))
                {
                    if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && !this.IsMouseCaptured) this.Cursor = Cursors.SizeNS;
                    else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1 && !this.IsMouseCaptured) this.Cursor = Cursors.SizeNS;
                    else if (!this.IsMouseCaptured) this.Cursor = Cursors.SizeWE;
                }
                else if (leftSize.Geometry.StrokeContains(leftSize.Pen, e.GetPosition(this)))
                {
                    if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && !this.IsMouseCaptured) this.Cursor = Cursors.SizeNS;
                    else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1 && !this.IsMouseCaptured) this.Cursor = Cursors.SizeNS;
                    else if (!this.IsMouseCaptured) this.Cursor = Cursors.SizeWE;
                }
                else if (topSize.Geometry.StrokeContains(topSize.Pen, e.GetPosition(this)))
                {
                    if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && !this.IsMouseCaptured) this.Cursor = Cursors.SizeWE;
                    else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1) this.Cursor = Cursors.SizeWE;
                    else if (!this.IsMouseCaptured) this.Cursor = Cursors.SizeNS;
                }
                else if (downSize.Geometry.StrokeContains(downSize.Pen, e.GetPosition(this)))
                {
                    if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && !this.IsMouseCaptured) this.Cursor = Cursors.SizeWE;
                    else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1) this.Cursor = Cursors.SizeWE;
                    else if (!this.IsMouseCaptured) this.Cursor = Cursors.SizeNS;
                }            
                else if (!this.IsMouseCaptured) this.Cursor = null;
            }
            p = e.GetPosition(Application.Current.MainWindow);  
        
            e.Handled = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!((MainWindow)Application.Current.MainWindow).IsBindingStartProject)
            {
                CompareSaveHeight = ActualHeight;
                CompareSaveWidth = ActualWidth;
                CompareSaveX = Canvas.GetLeft(this);
                CompareSaveY = Canvas.GetTop(this);

                p = e.GetPosition(Application.Current.MainWindow);

                if (this.CanvasPage.CountSelect <= 1)
                {
                    Position = e.GetPosition(this);

                    if (rightSize.Geometry.StrokeContains(rightSize.Pen, Position) && this.IsSelected)
                    {
                        if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1) IsRightSizeUp = true;
                        else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0) IsRightSizeLeft = true;
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1) IsRightSizeDown = true;
                        else IsRightSize = true;

                        this.CaptureMouse();
                    }
                    else if (leftSize.Geometry.StrokeContains(leftSize.Pen, Position) && this.IsSelected)
                    {
                        if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1) IsLeftSizeDown = true;
                        else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0) IsLeftSizeRight = true;
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1) IsLeftSizeUp = true;
                        else IsLeftSize = true;

                        this.CaptureMouse();
                    }
                    else if (topSize.Geometry.StrokeContains(topSize.Pen, Position) && this.IsSelected)
                    {
                        if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1) IsTopSizeLeft = true;
                        else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0) IsTopSizeDown = true;
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1) IsTopSizeRight = true;
                        else IsTopSize = true;

                        this.CaptureMouse();
                    }
                    else if (downSize.Geometry.StrokeContains(downSize.Pen, Position) && this.IsSelected)
                    {
                        if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1) IsDownSizeRight = true;
                        else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0) IsDownSizeTop = true;
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1) IsDownSizeLeft = true;
                        else IsDownSize = true;

                        this.CaptureMouse();
                    }
                    else
                    {
                        this.SelectOne();

                        IsPipeControlMove = true;

                        this.CaptureMouse();
                    }
                }
                else
                {
                    if (!this.IsSelected && Keyboard.Modifiers != ModifierKeys.Control)
                    {
                        this.Select();
                        IsPipeControlMove = true;
                        this.CaptureMouse();
                    }
                    else if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        this.Select();
                        IsControlMoveMassive = true;
                        this.CaptureMouse();
                    }
                    else
                    {
                        IsControlMoveMassive = true;
                        this.CaptureMouse();
                    }
                }
            }
           
            e.Handled = true;
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);

            IsRightSizeDown = false;
            IsLeftSizeRight = false;
            IsRightSizeLeft = false;
            IsLeftSizeDown = false;
            IsRightSizeUp = false;
            IsLeftSize = false;
            IsPipeControlMove = false;
            IsRightSize = false;
            IsLeftSizeUp = false;
            IsTopSize = false;
            IsDownSize = false;
            IsTopSizeLeft = false;
            IsDownSizeRight = false;
            IsTopSizeDown = false;
            IsDownSizeTop = false;
            IsTopSizeRight = false;
            IsDownSizeLeft = false;

            Issticking = false;

            SumStickingX = 0;
            SumStickingY = 0;

            if (Math.Floor(CompareSaveHeight * 100) / 100 != Math.Floor(ActualHeight * 100) / 100
                || Math.Floor(CompareSaveWidth * 100) / 100 != Math.Floor(ActualWidth * 100) / 100
                || Math.Floor(CompareSaveX * 100) / 100 != Math.Floor(Canvas.GetLeft(this) * 100) / 100
                || Math.Floor(CompareSaveY * 100) / 100 != Math.Floor(Canvas.GetTop(this) * 100) / 100)
            {
                double x, y, z, t;
                x = Math.Floor(CompareSaveX * 100) / 100;
                y = Math.Floor(Canvas.GetLeft(this) * 100) / 100;
                z = Math.Floor(CompareSaveY * 100) / 100;
                t = Math.Floor(Canvas.GetTop(this) * 100) / 100;
                CanvasTab.RepositionAllObjects(CanvasTab);
                CanvasTab.InvalidateMeasure();

                this.ChangePipeSer();

                if (IsControlMoveMassive)
                {
                    foreach (ControlOnCanvasPage controlOnCanvas in this.CanvasPage.SelectedControlOnCanvas)
                    {
                        controlOnCanvas.controlOnCanvasSer.Сoordinates = new Point((double)controlOnCanvas.GetValue(Canvas.LeftProperty), (double)controlOnCanvas.GetValue(Canvas.TopProperty));
                    }
                }
                else
                {
                    double d;
                    if (double.TryParse(this.CoordinateX.Text, out d))
                    {
                        if (double.Parse(this.CoordinateX.Text) < 0)
                            this.CoordinateX.Text = "0";
                        if (double.Parse(this.CoordinateY.Text) < 0)
                            this.CoordinateY.Text = "0";
                    }                   
                }
            }

            // Иначе при выделении и не изменении false ни сбросится
            if (IsControlMoveMassive)
            {
                CoordinateX.Text = null;
                CoordinateY.Text = null;
              
                IsControlMoveMassive = false;
            }
                                           
            e.Handled = true;
        }

        public Point point1 { get; set; }

        public Point point2 { get; set; }

        public Point point3 { get; set; }

        public Point point4 { get; set; }
    }
}

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
    public class Pipe90 : PipeOnCanvas
    {
        public GeometryDrawing rightDownSize;
        public GeometryDrawing leftDownSize;
        public GeometryDrawing topSize;
        public GeometryDrawing downSize;
        public GeometryDrawing rightFlange;
        public GeometryDrawing leftFlange;
        public GeometryDrawing topImage;
        public GeometryDrawing downImage;
        public GeometryDrawing downLenghtSize;
        public GeometryDrawing topLenghtSize;

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
                downImage.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushAirPipe_Down");
                topImage.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushAirPipe");
                leftFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushAirPipeFlange_Down");
                rightFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushAirPipeFlange");
                Pipe90Ser.Environment = 4;
            }
            else if (value == 3)
            {
                downImage.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushMasutPipe_Down");
                topImage.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushMasutPipe");
                leftFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushMasutPipeFlange_Down");
                rightFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushMasutPipeFlange");
                Pipe90Ser.Environment = 3;
            }
            else if (value == 2)
            {
                downImage.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushWaterPipe_Down");
                topImage.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushWaterPipe");
                leftFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushWaterPipeFlange_Down");
                rightFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushWaterPipeFlange");
                Pipe90Ser.Environment = 2;
            }
            else if (value == 0)
            {
                downImage.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushExhaustPipe_Down");
                topImage.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushExhaustPipe");
                leftFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushExhaustPipeFlange_Down");
                rightFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushExhaustPipeFlange");
                Pipe90Ser.Environment = 0;
            }
            else if (value == 1)
            {
                downImage.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushSteamPipe_Down");
                topImage.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushSteamPipe");
                leftFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushSteamPipeFlange_Down");
                rightFlange.Brush = (LinearGradientBrush)Application.Current.FindResource("BrushSteamPipeFlange");
                Pipe90Ser.Environment = 1;
            }
            }
        }

        private Pipe90Ser pipe90Ser;
        public Pipe90Ser Pipe90Ser
        {
            get { return pipe90Ser; }
            set { controlOnCanvasSer = value; pipe90Ser = value; }
        }
       
        private bool Issticking;

        private int SumStickingX;
        private int SumStickingY;

        bool IsTransformMove;

        Point p;
        Point Position;

        bool IstopLenghtSize;
        bool IstopLenghtSizeUp;
        bool IstopLenghtSizeLeft;
        bool IstopLenghtSizeDown;
        bool IsDownLenghtSize;
        bool IsDownLenghtSizeRight;
        bool IsPipeControlMove;     
        bool IsDownLenghtSizeUp;
        bool IsDownLenghtSizeLeft;
        bool IsTopSize;
        bool IsDownSize;
        bool IsTopSizeRight;
        bool IsTopSizeLeft;
        bool IsDownSizeTop;
        bool IsTopSizeDown;
        bool IsDownSizeLeft;
        bool IsDownSizeDown;
        bool IsleftDownSize;
        bool IsleftDownSizeRight;
        bool IsleftDownSizeTop;
        bool IsleftDownSizeLeft;
        bool IsRightDownSize;
        bool IsRightDownSizeRight;
        bool IsRightDownSizeTop;
        bool IsRightDownSizeLeft;
        bool IsControlMoveMassive;

        public PathGeometry PathGeometryDownSize;
        public PathFigure PathFigureDownSize;
        public LineSegment LineSegmentDownSize;

        public PathGeometry PathGeometryTopLenghtSize;
        public PathFigure PathFigureTopLenghtSize;
        public LineSegment LineSegmentTopLenghtSize;

        public PathGeometry PathGeometryTopImage;
        public PathFigure PathFigureTopImage;
        public PolyLineSegment PolyLineSegmentTopImage;

        public PathGeometry PathGeometryBorder;
        public PathFigure PathFigureBorder;
        public PolyLineSegment PolyLineSegmentBorder;

        public PathGeometry PathGeometryRightFlange;
        public PathFigure PathFigureRightFlange;
        public PolyLineSegment PolyLineSegmentRightFlange;

        public PathGeometry PathGeometryTopSize;
        public PathFigure PathFigureTopSize;
        public LineSegment LineSegmentTopSize;

        public PathGeometry PathGeometryDownLenghtSize;
        public PathFigure PathFigureDownLenghtSize;
        public LineSegment LineSegmentDownLenghtSize;

        public PathGeometry PathGeometryDownImage;
        public PathFigure PathFigureDownImage;
        public PolyLineSegment PolyLineSegmentDownImage;

        public PathGeometry PathGeometryLeftFlange;
        public PathFigure PathFigureLeftFlange;
        public PolyLineSegment PolyLineSegmentLeftFlange;

        public PathGeometry PathGeometryLeftDownSize;
        public PathFigure PathFigureLeftDownSize;
        public LineSegment LineSegmentLeftDownSize;

        public PathGeometry PathGeometryRightDownSize;
        public PathFigure PathFigureRightDownSize;
        public LineSegment LineSegmentRightDownSize;
       
        static Pipe90()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pipe90), new FrameworkPropertyMetadata(typeof(Pipe90)));
        }

        public Pipe90(PageScada ps, CanvasPage canvasPage, Pipe90Ser pipe90Ser)
            : base(pipe90Ser) 
        {
            this.Focusable = false;
            PS = ps;
            CanvasPage = canvasPage;
            Pipe90Ser = pipe90Ser;
            intEnvironment = pipe90Ser.Environment;
            
            menuItemProperties.Click += Properties;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (IsSelected)
            {
                DialogWindowPropertiesPipe90 WindowProperties = new DialogWindowPropertiesPipe90(this);
                WindowProperties.Owner = Application.Current.MainWindow;
                WindowProperties.ShowDialog();
            }
            
            e.Handled = true;
        }

        private void Properties(object sender, RoutedEventArgs e)
        {
            DialogWindowPropertiesPipe90 WindowProperties = new DialogWindowPropertiesPipe90(this);
            WindowProperties.Owner = Application.Current.MainWindow;
            WindowProperties.ShowDialog();
            e.Handled = true;
        }
     
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            rightDownSize = GetTemplateChild("RightDownSize") as GeometryDrawing;
            leftDownSize = GetTemplateChild("LeftDownSize") as GeometryDrawing;
            border = GetTemplateChild("Border") as GeometryDrawing;
            topSize = GetTemplateChild("TopSize") as GeometryDrawing;
            downSize = GetTemplateChild("DownSize") as GeometryDrawing;
            rightFlange = GetTemplateChild("RightFlange") as GeometryDrawing;
            leftFlange = GetTemplateChild("LeftFlange") as GeometryDrawing;
            topImage = GetTemplateChild("TopImage") as GeometryDrawing;
            downImage = GetTemplateChild("DownImage") as GeometryDrawing;
            downLenghtSize = GetTemplateChild("DownLenghtSize") as GeometryDrawing;
            topLenghtSize = GetTemplateChild("TopLenghtSize") as GeometryDrawing;

            PathGeometryDownSize = (PathGeometry)downSize.Geometry;
            PathFigureDownSize = PathGeometryDownSize.Figures[0];
            LineSegmentDownSize = (LineSegment)PathFigureDownSize.Segments[0];

            PathGeometryTopLenghtSize = (PathGeometry)topLenghtSize.Geometry;
            PathFigureTopLenghtSize = PathGeometryTopLenghtSize.Figures[0];
            LineSegmentTopLenghtSize = (LineSegment)PathFigureTopLenghtSize.Segments[0];

            PathGeometryTopImage = (PathGeometry)topImage.Geometry;
            PathFigureTopImage = PathGeometryTopImage.Figures[0];
            PolyLineSegmentTopImage = (PolyLineSegment)PathFigureTopImage.Segments[0];

            PathGeometryBorder = (PathGeometry)border.Geometry;
            PathFigureBorder = PathGeometryBorder.Figures[0];
            PolyLineSegmentBorder = (PolyLineSegment)PathFigureBorder.Segments[0];

            PathGeometryRightFlange = (PathGeometry)rightFlange.Geometry;
            PathFigureRightFlange = PathGeometryRightFlange.Figures[0];
            PolyLineSegmentRightFlange = (PolyLineSegment)PathFigureRightFlange.Segments[0];

            PathGeometryTopSize = (PathGeometry)topSize.Geometry;
            PathFigureTopSize = PathGeometryTopSize.Figures[0];
            LineSegmentTopSize = (LineSegment)PathFigureTopSize.Segments[0];

            PathGeometryDownLenghtSize = (PathGeometry)downLenghtSize.Geometry;
            PathFigureDownLenghtSize = PathGeometryDownLenghtSize.Figures[0];
            LineSegmentDownLenghtSize = (LineSegment)PathFigureDownLenghtSize.Segments[0];

            PathGeometryDownImage = (PathGeometry)downImage.Geometry;
            PathFigureDownImage = PathGeometryDownImage.Figures[0];
            PolyLineSegmentDownImage = (PolyLineSegment)PathFigureDownImage.Segments[0];

            PathGeometryLeftFlange = (PathGeometry)leftFlange.Geometry;
            PathFigureLeftFlange = PathGeometryLeftFlange.Figures[0];
            PolyLineSegmentLeftFlange = (PolyLineSegment)PathFigureLeftFlange.Segments[0];

            PathGeometryLeftDownSize = (PathGeometry)leftDownSize.Geometry;
            PathFigureLeftDownSize = PathGeometryLeftDownSize.Figures[0];
            LineSegmentLeftDownSize = (LineSegment)PathFigureLeftDownSize.Segments[0];

            PathGeometryRightDownSize = (PathGeometry)rightDownSize.Geometry;
            PathFigureRightDownSize = PathGeometryRightDownSize.Figures[0];
            LineSegmentRightDownSize = (LineSegment)PathFigureRightDownSize.Segments[0];

            Point topSizePoint = PathFigureTopSize.StartPoint;

            Point downSizePoint = PathFigureDownSize.StartPoint;

            Diameter = (downSizePoint.Y - topSizePoint.Y);

            IntEnvironment = intEnvironment;
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

            #region IstopLenghtSize
            if (IstopLenghtSize)
            {
                Point test4 = PathFigureDownSize.StartPoint;

                double delta = test4.X - LineSegmentDownSize.Point.X;

                if (delta < 20)
                {
                    if ((p.X > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point p2 = PathFigureTopLenghtSize.StartPoint;
                p2.X -= p.X;
                PathFigureTopLenghtSize.StartPoint = p2;
                Point p3 = LineSegmentTopLenghtSize.Point;
                p3.X -= p.X;
                LineSegmentTopLenghtSize.Point = p3;

                Point pipe3 = PolyLineSegmentTopImage.Points[1];
                Point pipe4 = PolyLineSegmentTopImage.Points[2];
                pipe3.X -= p.X;
                pipe4.X -= p.X;
                PolyLineSegmentTopImage.Points[1] = pipe3;
                PolyLineSegmentTopImage.Points[2] = pipe4;

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

                Point topSizePoint = PathFigureTopSize.StartPoint;
                topSizePoint.X -= p.X;
                PathFigureTopSize.StartPoint = topSizePoint;

                Point downSizePoint = PathFigureDownSize.StartPoint;
                downSizePoint.X -= p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
            }
            else if (IstopLenghtSizeUp)
            {
                Point test4 = PathFigureDownSize.StartPoint;

                double delta = test4.X - LineSegmentDownSize.Point.X;

                if (delta < 20)
                {
                    if ((p.Y < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point p2 = PathFigureTopLenghtSize.StartPoint;
                p2.X += p.Y;
                PathFigureTopLenghtSize.StartPoint = p2;
                Point p3 = LineSegmentTopLenghtSize.Point;
                p3.X += p.Y;
                LineSegmentTopLenghtSize.Point = p3;

                Point pipe3 = PolyLineSegmentTopImage.Points[1];
                Point pipe4 = PolyLineSegmentTopImage.Points[2];
                pipe3.X += p.Y;
                pipe4.X += p.Y;
                PolyLineSegmentTopImage.Points[1] = pipe3;
                PolyLineSegmentTopImage.Points[2] = pipe4;

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

                Point topSizePoint = PathFigureTopSize.StartPoint;
                topSizePoint.X += p.Y;
                PathFigureTopSize.StartPoint = topSizePoint;

                Point downSizePoint = PathFigureDownSize.StartPoint;
                downSizePoint.X += p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;

                Coordinate();
            }
            else if (IstopLenghtSizeLeft)
            {
                Point test4 = PathFigureDownSize.StartPoint;

                double delta = test4.X - LineSegmentDownSize.Point.X;

                if (delta < 20)
                {
                    if ((p.X < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point p2 = PathFigureTopLenghtSize.StartPoint;
                p2.X += p.X;
                PathFigureTopLenghtSize.StartPoint = p2;
                Point p3 = LineSegmentTopLenghtSize.Point;
                p3.X += p.X;
                LineSegmentTopLenghtSize.Point = p3;

                Point pipe3 = PolyLineSegmentTopImage.Points[1];
                Point pipe4 = PolyLineSegmentTopImage.Points[2];
                pipe3.X += p.X;
                pipe4.X += p.X;
                PolyLineSegmentTopImage.Points[1] = pipe3;
                PolyLineSegmentTopImage.Points[2] = pipe4;

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

                Point topSizePoint = PathFigureTopSize.StartPoint;
                topSizePoint.X += p.X;
                PathFigureTopSize.StartPoint = topSizePoint;

                Point downSizePoint = PathFigureDownSize.StartPoint;
                downSizePoint.X += p.X;
                PathFigureDownSize.StartPoint = downSizePoint;

                Coordinate();
            }
            else if (IstopLenghtSizeDown)
            {
                Point test4 = PathFigureDownSize.StartPoint;

                double delta = test4.X - LineSegmentDownSize.Point.X;

                if (delta < 20)
                {
                    if ((p.Y > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point p2 = PathFigureTopLenghtSize.StartPoint;
                p2.X -= p.Y;
                PathFigureTopLenghtSize.StartPoint = p2;
                Point p3 = LineSegmentTopLenghtSize.Point;
                p3.X -= p.Y;
                LineSegmentTopLenghtSize.Point = p3;

                Point pipe3 = PolyLineSegmentTopImage.Points[1];
                Point pipe4 = PolyLineSegmentTopImage.Points[2];
                pipe3.X -= p.Y;
                pipe4.X -= p.Y;
                PolyLineSegmentTopImage.Points[1] = pipe3;
                PolyLineSegmentTopImage.Points[2] = pipe4;

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

                Point topSizePoint = PathFigureTopSize.StartPoint;
                topSizePoint.X -= p.Y;
                PathFigureTopSize.StartPoint = topSizePoint;

                Point downSizePoint = PathFigureDownSize.StartPoint;
                downSizePoint.X -= p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
            }
            #endregion
            #region IsDownLenghtSize
            else if (IsDownLenghtSize)
            {
                Point test4 = PathFigureRightDownSize.StartPoint;

                double delta = test4.Y - LineSegmentRightDownSize.Point.Y;

                if (delta < 20)
                {
                    if ((p.Y > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point p2 = PathFigureDownLenghtSize.StartPoint;
                p2.Y -= p.Y;
                PathFigureDownLenghtSize.StartPoint = p2;
                Point p3 = LineSegmentDownLenghtSize.Point;
                p3.Y -= p.Y;
                LineSegmentDownLenghtSize.Point = p3;

                Point pipe3 = PolyLineSegmentDownImage.Points[1];
                Point pipe4 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y -= p.Y;
                pipe4.Y -= p.Y;
                PolyLineSegmentDownImage.Points[1] = pipe3;
                PolyLineSegmentDownImage.Points[2] = pipe4;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y -= p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point rightFlangePoint = PathFigureLeftFlange.StartPoint;
                rightFlangePoint.Y -= p.Y;
                PathFigureLeftFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point rightFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point rightFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                rightFlangePoint2.Y -= p.Y;
                rightFlangePoint3.Y -= p.Y;
                rightFlangePoint4.Y -= p.Y;
                rightFlangePoint5.Y -= p.Y;
                PolyLineSegmentLeftFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = rightFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = rightFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = rightFlangePoint5;

                Point topSizePoint = PathFigureLeftDownSize.StartPoint;
                topSizePoint.Y -= p.Y;
                PathFigureLeftDownSize.StartPoint = topSizePoint;

                Point downSizePoint = PathFigureRightDownSize.StartPoint;
                downSizePoint.Y -= p.Y;
                PathFigureRightDownSize.StartPoint = downSizePoint;
            }
            else if (IsDownLenghtSizeRight)
            {
                Point test4 = PathFigureRightDownSize.StartPoint;

                double delta = test4.Y - LineSegmentRightDownSize.Point.Y;

                if (delta < 20)
                {
                    if ((p.X > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point p2 = PathFigureDownLenghtSize.StartPoint;
                p2.Y -= p.X;
                PathFigureDownLenghtSize.StartPoint = p2;
                Point p3 = LineSegmentDownLenghtSize.Point;
                p3.Y -= p.X;
                LineSegmentDownLenghtSize.Point = p3;

                Point pipe3 = PolyLineSegmentDownImage.Points[1];
                Point pipe4 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y -= p.X;
                pipe4.Y -= p.X;
                PolyLineSegmentDownImage.Points[1] = pipe3;
                PolyLineSegmentDownImage.Points[2] = pipe4;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y -= p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point rightFlangePoint = PathFigureLeftFlange.StartPoint;
                rightFlangePoint.Y -= p.X;
                PathFigureLeftFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point rightFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point rightFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                rightFlangePoint2.Y -= p.X;
                rightFlangePoint3.Y -= p.X;
                rightFlangePoint4.Y -= p.X;
                rightFlangePoint5.Y -= p.X;
                PolyLineSegmentLeftFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = rightFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = rightFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = rightFlangePoint5;

                Point topSizePoint = PathFigureLeftDownSize.StartPoint;
                topSizePoint.Y -= p.X;
                PathFigureLeftDownSize.StartPoint = topSizePoint;

                Point downSizePoint = PathFigureRightDownSize.StartPoint;
                downSizePoint.Y -= p.X;
                PathFigureRightDownSize.StartPoint = downSizePoint;
            }
            else if (IsDownLenghtSizeUp)
            {
                Point test4 = PathFigureRightDownSize.StartPoint;

                double delta = test4.Y - LineSegmentRightDownSize.Point.Y;

                if (delta < 20)
                {
                    if ((p.Y < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point p2 = PathFigureDownLenghtSize.StartPoint;
                p2.Y += p.Y;
                PathFigureDownLenghtSize.StartPoint = p2;
                Point p3 = LineSegmentDownLenghtSize.Point;
                p3.Y += p.Y;
                LineSegmentDownLenghtSize.Point = p3;

                Point pipe3 = PolyLineSegmentDownImage.Points[1];
                Point pipe4 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y += p.Y;
                pipe4.Y += p.Y;
                PolyLineSegmentDownImage.Points[1] = pipe3;
                PolyLineSegmentDownImage.Points[2] = pipe4;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point rightFlangePoint = PathFigureLeftFlange.StartPoint;
                rightFlangePoint.Y += p.Y;
                PathFigureLeftFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point rightFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point rightFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                rightFlangePoint2.Y += p.Y;
                rightFlangePoint3.Y += p.Y;
                rightFlangePoint4.Y += p.Y;
                rightFlangePoint5.Y += p.Y;
                PolyLineSegmentLeftFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = rightFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = rightFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = rightFlangePoint5;

                Point topSizePoint = PathFigureLeftDownSize.StartPoint;
                topSizePoint.Y += p.Y;
                PathFigureLeftDownSize.StartPoint = topSizePoint;

                Point downSizePoint = PathFigureRightDownSize.StartPoint;
                downSizePoint.Y += p.Y;
                PathFigureRightDownSize.StartPoint = downSizePoint;

                Coordinate();
            }
            else if (IsDownLenghtSizeLeft)
            {
                Point test4 = PathFigureRightDownSize.StartPoint;

                double delta = test4.Y - LineSegmentRightDownSize.Point.Y;

                if (delta < 20)
                {
                    if ((p.X < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point p2 = PathFigureDownLenghtSize.StartPoint;
                p2.Y += p.X;
                PathFigureDownLenghtSize.StartPoint = p2;
                Point p3 = LineSegmentDownLenghtSize.Point;
                p3.Y += p.X;
                LineSegmentDownLenghtSize.Point = p3;

                Point pipe3 = PolyLineSegmentDownImage.Points[1];
                Point pipe4 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y += p.X;
                pipe4.Y += p.X;
                PolyLineSegmentDownImage.Points[1] = pipe3;
                PolyLineSegmentDownImage.Points[2] = pipe4;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y += p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point rightFlangePoint = PathFigureLeftFlange.StartPoint;
                rightFlangePoint.Y += p.X;
                PathFigureLeftFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point rightFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point rightFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                rightFlangePoint2.Y += p.X;
                rightFlangePoint3.Y += p.X;
                rightFlangePoint4.Y += p.X;
                rightFlangePoint5.Y += p.X;
                PolyLineSegmentLeftFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = rightFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = rightFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = rightFlangePoint5;

                Point topSizePoint = PathFigureLeftDownSize.StartPoint;
                topSizePoint.Y += p.X;
                PathFigureLeftDownSize.StartPoint = topSizePoint;

                Point downSizePoint = PathFigureRightDownSize.StartPoint;
                downSizePoint.Y += p.X;
                PathFigureRightDownSize.StartPoint = downSizePoint;

                Coordinate();
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
               
                p.Y *= -1;
               
                topSizePoint.X -= p.Y;
                PathFigureTopSize.StartPoint = topSizePoint;
              
                downSizePoint.Y -= p.Y;
                downSizePoint.X -= p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.Y;
                downSizePoint2.X -= p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X -= p.Y;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y -= p.Y;
                leftSizePoint.X -= p.Y;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X -= p.Y;
                rightFlangePoint.Y -= p.Y;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y -= p.Y;
                rightFlangePoint2.X -= p.Y;
                rightFlangePoint3.Y -= p.Y;               
                rightFlangePoint3.X -= p.Y;
                rightFlangePoint4.X -= p.Y;
                rightFlangePoint5.X -= p.Y;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;
                                                      
                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X -= p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y -= p.Y;
                borderPipePoint2.X -= p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y -= p.Y;
                pointPipe.X -= p.Y;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y -= p.Y;
                pointPipe3.Y -= p.Y;
                pointPipe2.X -= p.Y;
                pointPipe3.X -= p.Y;
                pointPipe4.X -= p.Y;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y -= p.Y;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y -= p.Y;
                downSizePoint3.X -= p.Y;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X -= p.Y;
                downSizePoint4.Y -= p.Y;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y -= p.Y;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y -= p.Y;
                leftFlangePoint3.Y -= p.Y;
                leftFlangePoint3.X -= p.Y;
                leftFlangePoint4.Y -= p.Y;
                leftFlangePoint4.X -= p.Y;
                leftFlangePoint5.Y -= p.Y;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y -= p.Y;
                pipe3.X -= p.Y;
                pipe4.Y -= p.Y;
                pipe4.X -= p.Y;
                pipe5.Y -= p.Y;                
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y -= p.Y;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y -= p.Y;
                downLenghtPoint2.X -= p.Y;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x += p.Y;
                this.SetValue(Canvas.LeftProperty, x);

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
              
                p.X *= -1;

                topSizePoint.X -= p.X;
                PathFigureTopSize.StartPoint = topSizePoint;

                downSizePoint.Y -= p.X;
                downSizePoint.X -= p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.X;
                downSizePoint2.X -= p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X -= p.X;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y -= p.X;
                leftSizePoint.X -= p.X;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X -= p.X;
                rightFlangePoint.Y -= p.X;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y -= p.X;
                rightFlangePoint2.X -= p.X;
                rightFlangePoint3.Y -= p.X;
                rightFlangePoint3.X -= p.X;
                rightFlangePoint4.X -= p.X;
                rightFlangePoint5.X -= p.X;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X -= p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y -= p.X;
                borderPipePoint2.X -= p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y -= p.X;
                pointPipe.X -= p.X;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y -= p.X;
                pointPipe3.Y -= p.X;
                pointPipe2.X -= p.X;
                pointPipe3.X -= p.X;
                pointPipe4.X -= p.X;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y -= p.X;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y -= p.X;
                downSizePoint3.X -= p.X;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X -= p.X;
                downSizePoint4.Y -= p.X;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y -= p.X;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y -= p.X;
                leftFlangePoint3.Y -= p.X;
                leftFlangePoint3.X -= p.X;
                leftFlangePoint4.Y -= p.X;
                leftFlangePoint4.X -= p.X;
                leftFlangePoint5.Y -= p.X;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y -= p.X;
                pipe3.X -= p.X;
                pipe4.Y -= p.X;
                pipe4.X -= p.X;
                pipe5.Y -= p.X;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y -= p.X;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y -= p.X;
                downLenghtPoint2.X -= p.X;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

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
              
                p.Y *= -1;

                topSizePoint.X += p.Y;
                PathFigureTopSize.StartPoint = topSizePoint;

                downSizePoint.Y += p.Y;
                downSizePoint.X += p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.Y;
                downSizePoint2.X += p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X += p.Y;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y += p.Y;
                leftSizePoint.X += p.Y;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.Y;
                rightFlangePoint.Y += p.Y;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.Y;
                rightFlangePoint2.X += p.Y;
                rightFlangePoint3.Y += p.Y;
                rightFlangePoint3.X += p.Y;
                rightFlangePoint4.X += p.Y;
                rightFlangePoint5.X += p.Y;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X += p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y += p.Y;
                borderPipePoint2.X += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y += p.Y;
                pointPipe.X += p.Y;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y += p.Y;
                pointPipe3.Y += p.Y;
                pointPipe2.X += p.Y;
                pointPipe3.X += p.Y;
                pointPipe4.X += p.Y;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y += p.Y;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y += p.Y;
                downSizePoint3.X += p.Y;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X += p.Y;
                downSizePoint4.Y += p.Y;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y += p.Y;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y += p.Y;
                leftFlangePoint3.Y += p.Y;
                leftFlangePoint3.X += p.Y;
                leftFlangePoint4.Y += p.Y;
                leftFlangePoint4.X += p.Y;
                leftFlangePoint5.Y += p.Y;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y += p.Y;
                pipe3.X += p.Y;
                pipe4.Y += p.Y;
                pipe4.X += p.Y;
                pipe5.Y += p.Y;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y += p.Y;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y += p.Y;
                downLenghtPoint2.X += p.Y;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                double y = (double)this.GetValue(Canvas.TopProperty);
                y += p.Y;

                this.SetValue(Canvas.TopProperty, y);

                Coordinate();
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
                
                p.X *= -1;
            
                topSizePoint.X += p.X;
                PathFigureTopSize.StartPoint = topSizePoint;
                
                downSizePoint.Y += p.X;
                downSizePoint.X += p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.X;
                downSizePoint2.X += p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X += p.X;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y += p.X;
                leftSizePoint.X += p.X;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.X;
                rightFlangePoint.Y += p.X;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.X;
                rightFlangePoint2.X += p.X;
                rightFlangePoint3.Y += p.X;
                rightFlangePoint3.X += p.X;
                rightFlangePoint4.X += p.X;
                rightFlangePoint5.X += p.X;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X += p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y += p.X;
                borderPipePoint2.X += p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y += p.X;
                pointPipe.X += p.X;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y += p.X;
                pointPipe3.Y += p.X;
                pointPipe2.X += p.X;
                pointPipe3.X += p.X;
                pointPipe4.X += p.X;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y += p.X;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y += p.X;
                downSizePoint3.X += p.X;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X += p.X;
                downSizePoint4.Y += p.X;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y += p.X;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y += p.X;
                leftFlangePoint3.Y += p.X;
                leftFlangePoint3.X += p.X;
                leftFlangePoint4.Y += p.X;
                leftFlangePoint4.X += p.X;
                leftFlangePoint5.Y += p.X;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y += p.X;
                pipe3.X += p.X;
                pipe4.Y += p.X;
                pipe4.X += p.X;
                pipe5.Y += p.X;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y += p.X;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y += p.X;
                downLenghtPoint2.X += p.X;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

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
              
                p.Y *= -1;
                
                topSizePoint.X += p.Y;
                PathFigureTopSize.StartPoint = topSizePoint;
              
                downSizePoint.Y += p.Y;
                downSizePoint.X += p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.Y;
                downSizePoint2.X += p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X += p.Y;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y += p.Y;
                leftSizePoint.X += p.Y;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.Y;
                rightFlangePoint.Y += p.Y;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.Y;
                rightFlangePoint2.X += p.Y;
                rightFlangePoint3.Y += p.Y;
                rightFlangePoint3.X += p.Y;
                rightFlangePoint4.X += p.Y;
                rightFlangePoint5.X += p.Y;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X += p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y += p.Y;
                borderPipePoint2.X += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y += p.Y;
                pointPipe.X += p.Y;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y += p.Y;
                pointPipe3.Y += p.Y;
                pointPipe2.X += p.Y;
                pointPipe3.X += p.Y;
                pointPipe4.X += p.Y;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y += p.Y;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y += p.Y;
                downSizePoint3.X += p.Y;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X += p.Y;
                downSizePoint4.Y += p.Y;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y += p.Y;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y += p.Y;
                leftFlangePoint3.Y += p.Y;
                leftFlangePoint3.X += p.Y;
                leftFlangePoint4.Y += p.Y;
                leftFlangePoint4.X += p.Y;
                leftFlangePoint5.Y += p.Y;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y += p.Y;
                pipe3.X += p.Y;
                pipe4.Y += p.Y;
                pipe4.X += p.Y;
                pipe5.Y += p.Y;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y += p.Y;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y += p.Y;
                downLenghtPoint2.X += p.Y;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;
            }
            else if (IsDownSizeTop)
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
               
                p.X *= -1;

                topSizePoint.X += p.X;
                PathFigureTopSize.StartPoint = topSizePoint;

                downSizePoint.Y += p.X;
                downSizePoint.X += p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.X;
                downSizePoint2.X += p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X += p.X;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y += p.X;
                leftSizePoint.X += p.X;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.X;
                rightFlangePoint.Y += p.X;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.X;
                rightFlangePoint2.X += p.X;
                rightFlangePoint3.Y += p.X;
                rightFlangePoint3.X += p.X;
                rightFlangePoint4.X += p.X;
                rightFlangePoint5.X += p.X;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X += p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y += p.X;
                borderPipePoint2.X += p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y += p.X;
                pointPipe.X += p.X;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y += p.X;
                pointPipe3.Y += p.X;
                pointPipe2.X += p.X;
                pointPipe3.X += p.X;
                pointPipe4.X += p.X;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y += p.X;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y += p.X;
                downSizePoint3.X += p.X;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X += p.X;
                downSizePoint4.Y += p.X;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y += p.X;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y += p.X;
                leftFlangePoint3.Y += p.X;
                leftFlangePoint3.X += p.X;
                leftFlangePoint4.Y += p.X;
                leftFlangePoint4.X += p.X;
                leftFlangePoint5.Y += p.X;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y += p.X;
                pipe3.X += p.X;
                pipe4.Y += p.X;
                pipe4.X += p.X;
                pipe5.Y += p.X;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y += p.X;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y += p.X;
                downLenghtPoint2.X += p.X;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                Coordinate();
            }
            else if (IsDownSizeLeft)
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
              
                p.Y *= -1;

                topSizePoint.X -= p.Y;
                PathFigureTopSize.StartPoint = topSizePoint;

                downSizePoint.Y -= p.Y;
                downSizePoint.X -= p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.Y;
                downSizePoint2.X -= p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X -= p.Y;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y -= p.Y;
                leftSizePoint.X -= p.Y;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X -= p.Y;
                rightFlangePoint.Y -= p.Y;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y -= p.Y;
                rightFlangePoint2.X -= p.Y;
                rightFlangePoint3.Y -= p.Y;
                rightFlangePoint3.X -= p.Y;
                rightFlangePoint4.X -= p.Y;
                rightFlangePoint5.X -= p.Y;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X -= p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y -= p.Y;
                borderPipePoint2.X -= p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y -= p.Y;
                pointPipe.X -= p.Y;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y -= p.Y;
                pointPipe3.Y -= p.Y;
                pointPipe2.X -= p.Y;
                pointPipe3.X -= p.Y;
                pointPipe4.X -= p.Y;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y -= p.Y;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y -= p.Y;
                downSizePoint3.X -= p.Y;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X -= p.Y;
                downSizePoint4.Y -= p.Y;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y -= p.Y;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y -= p.Y;
                leftFlangePoint3.Y -= p.Y;
                leftFlangePoint3.X -= p.Y;
                leftFlangePoint4.Y -= p.Y;
                leftFlangePoint4.X -= p.Y;
                leftFlangePoint5.Y -= p.Y;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y -= p.Y;
                pipe3.X -= p.Y;
                pipe4.Y -= p.Y;
                pipe4.X -= p.Y;
                pipe5.Y -= p.Y;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y -= p.Y;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y -= p.Y;
                downLenghtPoint2.X -= p.Y;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                Coordinate();
            }
            else if (IsDownSizeDown)
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
               
                p.X *= -1;
              
                topSizePoint.X -= p.X;
                PathFigureTopSize.StartPoint = topSizePoint;

                downSizePoint.Y -= p.X;
                downSizePoint.X -= p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.X;
                downSizePoint2.X -= p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X -= p.X;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y -= p.X;
                leftSizePoint.X -= p.X;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X -= p.X;
                rightFlangePoint.Y -= p.X;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y -= p.X;
                rightFlangePoint2.X -= p.X;
                rightFlangePoint3.Y -= p.X;
                rightFlangePoint3.X -= p.X;
                rightFlangePoint4.X -= p.X;
                rightFlangePoint5.X -= p.X;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X -= p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y -= p.X;
                borderPipePoint2.X -= p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y -= p.X;
                pointPipe.X -= p.X;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y -= p.X;
                pointPipe3.Y -= p.X;
                pointPipe2.X -= p.X;
                pointPipe3.X -= p.X;
                pointPipe4.X -= p.X;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y -= p.X;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y -= p.X;
                downSizePoint3.X -= p.X;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X -= p.X;
                downSizePoint4.Y -= p.X;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y -= p.X;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y -= p.X;
                leftFlangePoint3.Y -= p.X;
                leftFlangePoint3.X -= p.X;
                leftFlangePoint4.Y -= p.X;
                leftFlangePoint4.X -= p.X;
                leftFlangePoint5.Y -= p.X;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y -= p.X;
                pipe3.X -= p.X;
                pipe4.Y -= p.X;
                pipe4.X -= p.X;
                pipe5.Y -= p.X;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y -= p.X;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y -= p.X;
                downLenghtPoint2.X -= p.X;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                Coordinate();
            }
            #endregion
            #region IsLeftDown
            else if (IsleftDownSize)
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
               
                p.X *= -1;

                topSizePoint.X -= p.X;
                PathFigureTopSize.StartPoint = topSizePoint;

                downSizePoint.Y -= p.X;
                downSizePoint.X -= p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.X;
                downSizePoint2.X -= p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X -= p.X;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y -= p.X;
                leftSizePoint.X -= p.X;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X -= p.X;
                rightFlangePoint.Y -= p.X;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y -= p.X;
                rightFlangePoint2.X -= p.X;
                rightFlangePoint3.Y -= p.X;
                rightFlangePoint3.X -= p.X;
                rightFlangePoint4.X -= p.X;
                rightFlangePoint5.X -= p.X;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X -= p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y -= p.X;
                borderPipePoint2.X -= p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y -= p.X;
                pointPipe.X -= p.X;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y -= p.X;
                pointPipe3.Y -= p.X;
                pointPipe2.X -= p.X;
                pointPipe3.X -= p.X;
                pointPipe4.X -= p.X;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y -= p.X;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y -= p.X;
                downSizePoint3.X -= p.X;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X -= p.X;
                downSizePoint4.Y -= p.X;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y -= p.X;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y -= p.X;
                leftFlangePoint3.Y -= p.X;
                leftFlangePoint3.X -= p.X;
                leftFlangePoint4.Y -= p.X;
                leftFlangePoint4.X -= p.X;
                leftFlangePoint5.Y -= p.X;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y -= p.X;
                pipe3.X -= p.X;
                pipe4.Y -= p.X;
                pipe4.X -= p.X;
                pipe5.Y -= p.X;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y -= p.X;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y -= p.X;
                downLenghtPoint2.X -= p.X;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x += p.X;
                this.SetValue(Canvas.LeftProperty, x);

                double y = (double)this.GetValue(Canvas.TopProperty);
                y += p.X;
                this.SetValue(Canvas.TopProperty, y);

                Coordinate();
            }
            else if (IsleftDownSizeRight)
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
            
                p.Y *= -1;

                topSizePoint.X += p.Y;
                PathFigureTopSize.StartPoint = topSizePoint;

                downSizePoint.Y += p.Y;
                downSizePoint.X += p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.Y;
                downSizePoint2.X += p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X += p.Y;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y += p.Y;
                leftSizePoint.X += p.Y;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.Y;
                rightFlangePoint.Y += p.Y;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.Y;
                rightFlangePoint2.X += p.Y;
                rightFlangePoint3.Y += p.Y;
                rightFlangePoint3.X += p.Y;
                rightFlangePoint4.X += p.Y;
                rightFlangePoint5.X += p.Y;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X += p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y += p.Y;
                borderPipePoint2.X += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y += p.Y;
                pointPipe.X += p.Y;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y += p.Y;
                pointPipe3.Y += p.Y;
                pointPipe2.X += p.Y;
                pointPipe3.X += p.Y;
                pointPipe4.X += p.Y;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y += p.Y;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y += p.Y;
                downSizePoint3.X += p.Y;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X += p.Y;
                downSizePoint4.Y += p.Y;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y += p.Y;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y += p.Y;
                leftFlangePoint3.Y += p.Y;
                leftFlangePoint3.X += p.Y;
                leftFlangePoint4.Y += p.Y;
                leftFlangePoint4.X += p.Y;
                leftFlangePoint5.Y += p.Y;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y += p.Y;
                pipe3.X += p.Y;
                pipe4.Y += p.Y;
                pipe4.X += p.Y;
                pipe5.Y += p.Y;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y += p.Y;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y += p.Y;
                downLenghtPoint2.X += p.Y;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                double y = (double)this.GetValue(Canvas.TopProperty);
                y += p.Y;
                this.SetValue(Canvas.TopProperty, y);

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x -= p.Y;
                this.SetValue(Canvas.LeftProperty, x);

                Coordinate();
            }
            else if (IsleftDownSizeTop)
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
                
                p.X *= -1;
               
                topSizePoint.X += p.X;
                PathFigureTopSize.StartPoint = topSizePoint;

                downSizePoint.Y += p.X;
                downSizePoint.X += p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.X;
                downSizePoint2.X += p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X += p.X;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y += p.X;
                leftSizePoint.X += p.X;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.X;
                rightFlangePoint.Y += p.X;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.X;
                rightFlangePoint2.X += p.X;
                rightFlangePoint3.Y += p.X;
                rightFlangePoint3.X += p.X;
                rightFlangePoint4.X += p.X;
                rightFlangePoint5.X += p.X;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X += p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y += p.X;
                borderPipePoint2.X += p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y += p.X;
                pointPipe.X += p.X;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y += p.X;
                pointPipe3.Y += p.X;
                pointPipe2.X += p.X;
                pointPipe3.X += p.X;
                pointPipe4.X += p.X;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y += p.X;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y += p.X;
                downSizePoint3.X += p.X;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X += p.X;
                downSizePoint4.Y += p.X;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y += p.X;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y += p.X;
                leftFlangePoint3.Y += p.X;
                leftFlangePoint3.X += p.X;
                leftFlangePoint4.Y += p.X;
                leftFlangePoint4.X += p.X;
                leftFlangePoint5.Y += p.X;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y += p.X;
                pipe3.X += p.X;
                pipe4.Y += p.X;
                pipe4.X += p.X;
                pipe5.Y += p.X;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y += p.X;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y += p.X;
                downLenghtPoint2.X += p.X;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x += p.X;
                this.SetValue(Canvas.LeftProperty, x);

                double y = (double)this.GetValue(Canvas.TopProperty);
                y += p.X;
                this.SetValue(Canvas.TopProperty, y);               
            }
            else if (IsleftDownSizeLeft)
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
               
                p.Y *= 1;
               
                topSizePoint.X += p.Y;
                PathFigureTopSize.StartPoint = topSizePoint;
               
                downSizePoint.Y += p.Y;
                downSizePoint.X += p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.Y;
                downSizePoint2.X += p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X += p.Y;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y += p.Y;
                leftSizePoint.X += p.Y;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.Y;
                rightFlangePoint.Y += p.Y;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.Y;
                rightFlangePoint2.X += p.Y;
                rightFlangePoint3.Y += p.Y;
                rightFlangePoint3.X += p.Y;
                rightFlangePoint4.X += p.Y;
                rightFlangePoint5.X += p.Y;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X += p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y += p.Y;
                borderPipePoint2.X += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y += p.Y;
                pointPipe.X += p.Y;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y += p.Y;
                pointPipe3.Y += p.Y;
                pointPipe2.X += p.Y;
                pointPipe3.X += p.Y;
                pointPipe4.X += p.Y;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y += p.Y;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y += p.Y;
                downSizePoint3.X += p.Y;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X += p.Y;
                downSizePoint4.Y += p.Y;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y += p.Y;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y += p.Y;
                leftFlangePoint3.Y += p.Y;
                leftFlangePoint3.X += p.Y;
                leftFlangePoint4.Y += p.Y;
                leftFlangePoint4.X += p.Y;
                leftFlangePoint5.Y += p.Y;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y += p.Y;
                pipe3.X += p.Y;
                pipe4.Y += p.Y;
                pipe4.X += p.Y;
                pipe5.Y += p.Y;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y += p.Y;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y += p.Y;
                downLenghtPoint2.X += p.Y;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x += p.Y;
                this.SetValue(Canvas.LeftProperty, x);

                double y = (double)this.GetValue(Canvas.TopProperty);
                y -= p.Y;
                this.SetValue(Canvas.TopProperty, y);

                Coordinate();
            }
            #endregion
            #region IsRightDownSize
            else if (IsRightDownSize)
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
               
                p.X *= -1;
               
                topSizePoint.X += p.X;
                PathFigureTopSize.StartPoint = topSizePoint;
              
                downSizePoint.Y += p.X;
                downSizePoint.X += p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.X;
                downSizePoint2.X += p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X += p.X;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y += p.X;
                leftSizePoint.X += p.X;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.X;
                rightFlangePoint.Y += p.X;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.X;
                rightFlangePoint2.X += p.X;
                rightFlangePoint3.Y += p.X;
                rightFlangePoint3.X += p.X;
                rightFlangePoint4.X += p.X;
                rightFlangePoint5.X += p.X;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X += p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y += p.X;
                borderPipePoint2.X += p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y += p.X;
                pointPipe.X += p.X;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y += p.X;
                pointPipe3.Y += p.X;
                pointPipe2.X += p.X;
                pointPipe3.X += p.X;
                pointPipe4.X += p.X;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y += p.X;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y += p.X;
                downSizePoint3.X += p.X;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X += p.X;
                downSizePoint4.Y += p.X;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y += p.X;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y += p.X;
                leftFlangePoint3.Y += p.X;
                leftFlangePoint3.X += p.X;
                leftFlangePoint4.Y += p.X;
                leftFlangePoint4.X += p.X;
                leftFlangePoint5.Y += p.X;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y += p.X;
                pipe3.X += p.X;
                pipe4.Y += p.X;
                pipe4.X += p.X;
                pipe5.Y += p.X;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y += p.X;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y += p.X;
                downLenghtPoint2.X += p.X;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                double y = (double)this.GetValue(Canvas.TopProperty);
                y -= p.X;
                this.SetValue(Canvas.TopProperty, y);

                Coordinate();
            }
            else if (IsRightDownSizeRight)
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
               
                p.Y *= 1;
               
                topSizePoint.X += p.Y;
                PathFigureTopSize.StartPoint = topSizePoint;

                downSizePoint.Y += p.Y;
                downSizePoint.X += p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.Y;
                downSizePoint2.X += p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X += p.Y;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y += p.Y;
                leftSizePoint.X += p.Y;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.Y;
                rightFlangePoint.Y += p.Y;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.Y;
                rightFlangePoint2.X += p.Y;
                rightFlangePoint3.Y += p.Y;
                rightFlangePoint3.X += p.Y;
                rightFlangePoint4.X += p.Y;
                rightFlangePoint5.X += p.Y;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X += p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y += p.Y;
                borderPipePoint2.X += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y += p.Y;
                pointPipe.X += p.Y;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y += p.Y;
                pointPipe3.Y += p.Y;
                pointPipe2.X += p.Y;
                pointPipe3.X += p.Y;
                pointPipe4.X += p.Y;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                PathGeometry g8 = (PathGeometry)leftDownSize.Geometry;
                PathFigure pf8 = g8.Figures[0];
                Point topSizePoint3 = pf8.StartPoint;
                topSizePoint3.Y += p.Y;
                pf8.StartPoint = topSizePoint3;
                LineSegment topLineSegment = (LineSegment)pf8.Segments[0];

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y += p.Y;
                downSizePoint3.X += p.Y;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X += p.Y;
                downSizePoint4.Y += p.Y;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y += p.Y;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y += p.Y;
                leftFlangePoint3.Y += p.Y;
                leftFlangePoint3.X += p.Y;
                leftFlangePoint4.Y += p.Y;
                leftFlangePoint4.X += p.Y;
                leftFlangePoint5.Y += p.Y;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y += p.Y;
                pipe3.X += p.Y;
                pipe4.Y += p.Y;
                pipe4.X += p.Y;
                pipe5.Y += p.Y;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y += p.Y;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y += p.Y;
                downLenghtPoint2.X += p.Y;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x -= p.Y;
                this.SetValue(Canvas.LeftProperty, x);

                Coordinate();
            }
            else if (IsRightDownSizeTop)
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

                p.X *= -1;
               
                topSizePoint.X -= p.X;
                PathFigureTopSize.StartPoint = topSizePoint;
               
                downSizePoint.Y -= p.X;
                downSizePoint.X -= p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.X;
                downSizePoint2.X -= p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X -= p.X;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y -= p.X;
                leftSizePoint.X -= p.X;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X -= p.X;
                rightFlangePoint.Y -= p.X;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y -= p.X;
                rightFlangePoint2.X -= p.X;
                rightFlangePoint3.Y -= p.X;
                rightFlangePoint3.X -= p.X;
                rightFlangePoint4.X -= p.X;
                rightFlangePoint5.X -= p.X;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X -= p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y -= p.X;
                borderPipePoint2.X -= p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y -= p.X;
                pointPipe.X -= p.X;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y -= p.X;
                pointPipe3.Y -= p.X;
                pointPipe2.X -= p.X;
                pointPipe3.X -= p.X;
                pointPipe4.X -= p.X;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y -= p.X;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y -= p.X;
                downSizePoint3.X -= p.X;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X -= p.X;
                downSizePoint4.Y -= p.X;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y -= p.X;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y -= p.X;
                leftFlangePoint3.Y -= p.X;
                leftFlangePoint3.X -= p.X;
                leftFlangePoint4.Y -= p.X;
                leftFlangePoint4.X -= p.X;
                leftFlangePoint5.Y -= p.X;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y -= p.X;
                pipe3.X -= p.X;
                pipe4.Y -= p.X;
                pipe4.X -= p.X;
                pipe5.Y -= p.X;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y -= p.X;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y -= p.X;
                downLenghtPoint2.X -= p.X;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                double y = (double)this.GetValue(Canvas.TopProperty);
                y -= p.X;
                this.SetValue(Canvas.TopProperty, y);

                Coordinate();
            }
            else if (IsRightDownSizeLeft)
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
              
                p.Y *= -1;
              
                topSizePoint.X += p.Y;
                PathFigureTopSize.StartPoint = topSizePoint;

                downSizePoint.Y += p.Y;
                downSizePoint.X += p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.Y;
                downSizePoint2.X += p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Diameter = (downSizePoint.Y - topSizePoint.Y);
                TextBoxDiameter.Text = string.Format("{0:F2}", Diameter);

                Point leftSizePoint2 = PathFigureTopLenghtSize.StartPoint;
                leftSizePoint2.X += p.Y;
                PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                Point leftSizePoint = LineSegmentTopLenghtSize.Point;
                leftSizePoint.Y += p.Y;
                leftSizePoint.X += p.Y;
                LineSegmentTopLenghtSize.Point = leftSizePoint;

                Point rightFlangePoint = PathFigureRightFlange.StartPoint;
                rightFlangePoint.X += p.Y;
                rightFlangePoint.Y += p.Y;
                PathFigureRightFlange.StartPoint = rightFlangePoint;
                Point rightFlangePoint2 = PolyLineSegmentRightFlange.Points[0];
                Point rightFlangePoint3 = PolyLineSegmentRightFlange.Points[3];
                Point rightFlangePoint4 = PolyLineSegmentRightFlange.Points[1];
                Point rightFlangePoint5 = PolyLineSegmentRightFlange.Points[2];
                rightFlangePoint2.Y += p.Y;
                rightFlangePoint2.X += p.Y;
                rightFlangePoint3.Y += p.Y;
                rightFlangePoint3.X += p.Y;
                rightFlangePoint4.X += p.Y;
                rightFlangePoint5.X += p.Y;
                PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                Point borderPipePoint1 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint1.X += p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                borderPipePoint2.Y += p.Y;
                borderPipePoint2.X += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                Point pointPipe = PathFigureTopImage.StartPoint;
                pointPipe.Y += p.Y;
                pointPipe.X += p.Y;
                PathFigureTopImage.StartPoint = pointPipe;
                Point pointPipe4 = PolyLineSegmentTopImage.Points[1];
                Point pointPipe2 = PolyLineSegmentTopImage.Points[2];
                Point pointPipe3 = PolyLineSegmentTopImage.Points[3];
                pointPipe2.Y += p.Y;
                pointPipe3.Y += p.Y;
                pointPipe2.X += p.Y;
                pointPipe3.X += p.Y;
                pointPipe4.X += p.Y;
                PolyLineSegmentTopImage.Points[1] = pointPipe4;
                PolyLineSegmentTopImage.Points[2] = pointPipe2;
                PolyLineSegmentTopImage.Points[3] = pointPipe3;

                Point topSizePoint3 = PathFigureLeftDownSize.StartPoint;
                topSizePoint3.Y += p.Y;
                PathFigureLeftDownSize.StartPoint = topSizePoint3;

                Point downSizePoint3 = PathFigureRightDownSize.StartPoint;
                downSizePoint3.Y += p.Y;
                downSizePoint3.X += p.Y;
                PathFigureRightDownSize.StartPoint = downSizePoint3;
                Point downSizePoint4 = LineSegmentRightDownSize.Point;
                downSizePoint4.X += p.Y;
                downSizePoint4.Y += p.Y;
                LineSegmentRightDownSize.Point = downSizePoint4;

                Point leftFlangePoint = PathFigureLeftFlange.StartPoint;
                leftFlangePoint.Y += p.Y;
                PathFigureLeftFlange.StartPoint = leftFlangePoint;
                Point leftFlangePoint2 = PolyLineSegmentLeftFlange.Points[0];
                Point leftFlangePoint3 = PolyLineSegmentLeftFlange.Points[1];
                Point leftFlangePoint4 = PolyLineSegmentLeftFlange.Points[2];
                Point leftFlangePoint5 = PolyLineSegmentLeftFlange.Points[3];
                leftFlangePoint2.Y += p.Y;
                leftFlangePoint3.Y += p.Y;
                leftFlangePoint3.X += p.Y;
                leftFlangePoint4.Y += p.Y;
                leftFlangePoint4.X += p.Y;
                leftFlangePoint5.Y += p.Y;
                PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                Point pipe3 = PolyLineSegmentDownImage.Points[0];
                Point pipe4 = PolyLineSegmentDownImage.Points[1];
                Point pipe5 = PolyLineSegmentDownImage.Points[2];
                pipe3.Y += p.Y;
                pipe3.X += p.Y;
                pipe4.Y += p.Y;
                pipe4.X += p.Y;
                pipe5.Y += p.Y;
                PolyLineSegmentDownImage.Points[0] = pipe3;
                PolyLineSegmentDownImage.Points[1] = pipe4;
                PolyLineSegmentDownImage.Points[2] = pipe5;

                Point downLenghtPoint = PathFigureDownLenghtSize.StartPoint;
                downLenghtPoint.Y += p.Y;
                PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                Point downLenghtPoint2 = LineSegmentDownLenghtSize.Point;
                downLenghtPoint2.Y += p.Y;
                downLenghtPoint2.X += p.Y;
                LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x += p.Y;
                this.SetValue(Canvas.LeftProperty, x);
            }
            #endregion
            #region MovePipe90
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

                foreach (ControlOnCanvas controlMove in this.CanvasPage.Children)
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
                double y = (double)this.GetValue(Canvas.TopProperty);
                x -= p.X;
                y -= p.Y;

                if (!(this.CanvasPage.CountSelect > 1))
                {
                    foreach (ControlOnCanvas controlOnCanvas in this.CanvasPage.Children)
                    {
                        if (controlOnCanvas is Pipe90)
                        {
                            Pipe90 pipe90 = controlOnCanvas as Pipe90;

                            if (this != pipe90)
                            {
                                if ((int)this.RenderTransform.Value.M11 == 1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe90.RenderTransform.Value.M11 == 0 && (int)pipe90.RenderTransform.Value.M12 == -1)
                                {
                                    point1 = this.TranslatePoint(LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                                else if ((int)pipe90.RenderTransform.Value.M11 == 1 && (int)pipe90.RenderTransform.Value.M12 == 0 && (int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1)
                                {
                                    point1 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
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
                                else if ((int)this.RenderTransform.Value.M11 == 1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe90.RenderTransform.Value.M11 == -1 && (int)pipe90.RenderTransform.Value.M12 == 0)
                                {
                                    point1 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

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

                                    if (x1 == x2 && y1 == y2 - 1 && x3 == x4 && y3 == y4 - 1)
                                    {
                                        Issticking = true;
                                    }
                                    else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                                    {
                                        this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                        this.SetValue(Canvas.TopProperty, y - (y1 - y2) - 1);

                                        Issticking = true;
                                    }

                                    point1 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe90.TranslatePoint(pipe90.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe90.TranslatePoint(pipe90.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

                                    x1 = point1.X;
                                    y1 = point1.Y;

                                    x2 = point3.X;
                                    y2 = point3.Y;

                                    x3 = point2.X;
                                    y3 = point2.Y;

                                    x4 = point4.X;
                                    y4 = point4.Y;

                                    test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                                    test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                                    test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                                    test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                                    if (x1 == x2 - 1 && y1 == y2 && x3 == x4 - 1 && y3 == y4)
                                    {
                                        Issticking = true;
                                    }
                                    else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                                    {
                                        this.SetValue(Canvas.LeftProperty, x - (x1 - x2) - 1);
                                        this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                        Issticking = true;
                                    }
                                }
                                else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe90.RenderTransform.Value.M11 == 1 && (int)pipe90.RenderTransform.Value.M12 == 0)
                                {
                                    point1 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

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

                                    if (x1 == x2 && y1 == y2 + 1 && x3 == x4 && y3 == y4 + 1)
                                    {
                                        Issticking = true;
                                    }
                                    else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                                    {
                                        this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                        this.SetValue(Canvas.TopProperty, y - (y1 - y2) + 1);

                                        Issticking = true;
                                    }

                                    point1 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe90.TranslatePoint(pipe90.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe90.TranslatePoint(pipe90.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

                                    x1 = point1.X;
                                    y1 = point1.Y;

                                    x2 = point3.X;
                                    y2 = point3.Y;

                                    x3 = point2.X;
                                    y3 = point2.Y;

                                    x4 = point4.X;
                                    y4 = point4.Y;

                                    test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                                    test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                                    test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                                    test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                                    if (x1 == x2 + 1 && y1 == y2 && x3 == x4 + 1 && y3 == y4)
                                    {
                                        Issticking = true;
                                    }
                                    else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                                    {
                                        this.SetValue(Canvas.LeftProperty, x - (x1 - x2) + 1);
                                        this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                        Issticking = true;
                                    }
                                }
                                else if ((int)this.RenderTransform.Value.M11 == 1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe90.RenderTransform.Value.M11 == 0 && (int)pipe90.RenderTransform.Value.M12 == 1)
                                {
                                    point1 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
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
                                    point1 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                                    point1 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                                else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe90.RenderTransform.Value.M11 == 0 && (int)pipe90.RenderTransform.Value.M12 == -1)
                                {
                                    point1 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
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
                                    point1 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe90.TranslatePoint(pipe90.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe90.TranslatePoint(pipe90.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

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

                                    if (x1 == x2 && y1 == y2 + 1 && x3 == x4 && y3 == y4 + 1)
                                    {
                                        Issticking = true;
                                    }
                                    else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                                    {
                                        this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                        this.SetValue(Canvas.TopProperty, y - (y1 - y2) + 1);

                                        Issticking = true;
                                    }

                                    point1 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

                                    x1 = point1.X;
                                    y1 = point1.Y;

                                    x2 = point3.X;
                                    y2 = point3.Y;

                                    x3 = point2.X;
                                    y3 = point2.Y;

                                    x4 = point4.X;
                                    y4 = point4.Y;

                                    test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                                    test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                                    test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                                    test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                                    if (x1 == x2 - 1 && y1 == y2 && x3 == x4 - 1 && y3 == y4)
                                    {
                                        Issticking = true;
                                    }
                                    else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                                    {
                                        this.SetValue(Canvas.LeftProperty, x - (x1 - x2) - 1);
                                        this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                        Issticking = true;
                                    }
                                }
                                else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1 && (int)pipe90.RenderTransform.Value.M11 == 0 && (int)pipe90.RenderTransform.Value.M12 == -1)
                                {
                                    point1 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe90.TranslatePoint(pipe90.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe90.TranslatePoint(pipe90.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

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

                                    if (x1 == x2 && y1 == y2 - 1 && x3 == x4 && y3 == y4 - 1)
                                    {
                                        Issticking = true;
                                    }
                                    else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                                    {
                                        this.SetValue(Canvas.LeftProperty, x - (x1 - x2));
                                        this.SetValue(Canvas.TopProperty, y - (y1 - y2) - 1);

                                        Issticking = true;
                                    }

                                    point1 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

                                    x1 = point1.X;
                                    y1 = point1.Y;

                                    x2 = point3.X;
                                    y2 = point3.Y;

                                    x3 = point2.X;
                                    y3 = point2.Y;

                                    x4 = point4.X;
                                    y4 = point4.Y;

                                    test = (int)Math.Abs(Math.Abs(x1) - Math.Abs(x2));
                                    test2 = (int)Math.Abs(Math.Abs(y1) - Math.Abs(y2));
                                    test3 = (int)Math.Abs(Math.Abs(x3) - Math.Abs(x4));
                                    test4 = (int)Math.Abs(Math.Abs(y3) - Math.Abs(y4));

                                    if (x1 == x2 + 1 && y1 == y2 && x3 == x4 + 1 && y3 == y4)
                                    {
                                        Issticking = true;
                                    }
                                    else if (test < 10 && test2 < 10 && test3 < 10 && test4 < 10)
                                    {
                                        this.SetValue(Canvas.LeftProperty, x - (x1 - x2) + 1);
                                        this.SetValue(Canvas.TopProperty, y - (y1 - y2));

                                        Issticking = true;
                                    }
                                }
                                else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe90.RenderTransform.Value.M11 == 0 && (int)pipe90.RenderTransform.Value.M12 == 1)
                                {
                                    point1 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                                    point1 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point3 = pipe90.TranslatePoint(pipe90.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point4 = pipe90.TranslatePoint(pipe90.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                                    point1 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                    point2 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
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
                            }
                        }
                        else if (controlOnCanvas is Pipe)
                        {
                            Pipe pipe = controlOnCanvas as Pipe;

                            if ((int)this.RenderTransform.Value.M11 == 1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe.RenderTransform.Value.M11 == 1 && (int)pipe.RenderTransform.Value.M12 == 0)
                            {
                                point1 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point3 = pipe.TranslatePoint(pipe.PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point4 = pipe.TranslatePoint(pipe.LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                            else if ((int)this.RenderTransform.Value.M11 == 1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe.RenderTransform.Value.M11 == -1 && (int)pipe.RenderTransform.Value.M12 == 0)
                            {
                                point1 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point3 = pipe.TranslatePoint(pipe.PathFigureRightSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point4 = pipe.TranslatePoint(pipe.LineSegmentRightSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                            else if ((int)this.RenderTransform.Value.M11 == 1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe.RenderTransform.Value.M11 == 0 && (int)pipe.RenderTransform.Value.M12 == 1)
                            {
                                point1 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point3 = pipe.TranslatePoint(pipe.LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point4 = pipe.TranslatePoint(pipe.PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                            else if ((int)this.RenderTransform.Value.M11 == 1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe.RenderTransform.Value.M11 == 0 && (int)pipe.RenderTransform.Value.M12 == -1)
                            {
                                point1 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
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
                            }
                            else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && (int)pipe.RenderTransform.Value.M11 == 1 && (int)pipe.RenderTransform.Value.M12 == 0)
                            {
                                point1 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point3 = pipe.TranslatePoint(pipe.LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point4 = pipe.TranslatePoint(pipe.PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                            else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && (int)pipe.RenderTransform.Value.M11 == -1 && (int)pipe.RenderTransform.Value.M12 == 0)
                            {
                                point1 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
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
                            }
                            else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && (int)pipe.RenderTransform.Value.M11 == 0 && (int)pipe.RenderTransform.Value.M12 == -1)
                            {
                                point1 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point3 = pipe.TranslatePoint(pipe.PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point4 = pipe.TranslatePoint(pipe.LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                            else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && (int)pipe.RenderTransform.Value.M11 == 0 && (int)pipe.RenderTransform.Value.M12 == 1)
                            {
                                point1 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point3 = pipe.TranslatePoint(pipe.PathFigureRightSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point4 = pipe.TranslatePoint(pipe.LineSegmentRightSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                            else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe.RenderTransform.Value.M11 == 1 && (int)pipe.RenderTransform.Value.M12 == 0)
                            {
                                point1 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point3 = pipe.TranslatePoint(pipe.PathFigureRightSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point4 = pipe.TranslatePoint(pipe.LineSegmentRightSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                            else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe.RenderTransform.Value.M11 == -1 && (int)pipe.RenderTransform.Value.M12 == 0)
                            {
                                point1 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point3 = pipe.TranslatePoint(pipe.PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point4 = pipe.TranslatePoint(pipe.LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                            else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe.RenderTransform.Value.M11 == 0 && (int)pipe.RenderTransform.Value.M12 == -1)
                            {
                                point1 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point3 = pipe.TranslatePoint(pipe.LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point4 = pipe.TranslatePoint(pipe.PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                            else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0 && (int)pipe.RenderTransform.Value.M11 == 0 && (int)pipe.RenderTransform.Value.M12 == 1)
                            {
                                point1 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
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
                            }
                            else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1 && (int)pipe.RenderTransform.Value.M11 == 1 && (int)pipe.RenderTransform.Value.M12 == 0)
                            {
                                point1 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
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
                            }
                            else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1 && (int)pipe.RenderTransform.Value.M11 == -1 && (int)pipe.RenderTransform.Value.M12 == 0)
                            {
                                point1 = this.TranslatePoint(this.PathFigureDownLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentDownLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point3 = pipe.TranslatePoint(pipe.LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point4 = pipe.TranslatePoint(pipe.PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                            else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1 && (int)pipe.RenderTransform.Value.M11 == 0 && (int)pipe.RenderTransform.Value.M12 == -1)
                            {
                                point1 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point3 = pipe.TranslatePoint(pipe.PathFigureRightSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point4 = pipe.TranslatePoint(pipe.LineSegmentRightSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
                            else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1 && (int)pipe.RenderTransform.Value.M11 == 0 && (int)pipe.RenderTransform.Value.M12 == 1)
                            {
                                point1 = this.TranslatePoint(this.PathFigureTopLenghtSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point2 = this.TranslatePoint(this.LineSegmentTopLenghtSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point3 = pipe.TranslatePoint(pipe.PathFigureLeftSize.StartPoint, ((AppWPF)System.Windows.Application.Current).MainWindow);
                                point4 = pipe.TranslatePoint(pipe.LineSegmentLeftSize.Point, ((AppWPF)System.Windows.Application.Current).MainWindow);

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
            #endregion

            if (!this.IsMouseCaptured)
            {
                if (!this.IsSelected || this.CanvasPage.CountSelect > 1)
                {
                    this.Cursor = null;

                    p = e.GetPosition(Application.Current.MainWindow);

                    e.Handled = true;
                    return;
                }
                if (leftDownSize.Geometry.StrokeContains(leftDownSize.Pen, e.GetPosition(this)))
                {
                    if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && !this.IsMouseCaptured) this.Cursor = Cursors.SizeNS;
                    else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1 && !this.IsMouseCaptured) this.Cursor = Cursors.SizeNS;
                    else if (!this.IsMouseCaptured) this.Cursor = Cursors.SizeWE;
                }
                else if (rightDownSize.Geometry.StrokeContains(rightDownSize.Pen, e.GetPosition(this)))
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
                else if (downLenghtSize.Geometry.StrokeContains(downLenghtSize.Pen, e.GetPosition(this)))
                {
                    if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && !this.IsMouseCaptured) this.Cursor = Cursors.SizeWE;
                    else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1) this.Cursor = Cursors.SizeWE;
                    else if (!this.IsMouseCaptured) this.Cursor = Cursors.SizeNS;
                }
                else if (topLenghtSize.Geometry.StrokeContains(topLenghtSize.Pen, e.GetPosition(this)))
                {
                    if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1 && !this.IsMouseCaptured) this.Cursor = Cursors.SizeNS;
                    else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1) this.Cursor = Cursors.SizeNS;
                    else if (!this.IsMouseCaptured) this.Cursor = Cursors.SizeWE;
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
                p = e.GetPosition(Application.Current.MainWindow);

                CompareSaveHeight = ActualHeight;
                CompareSaveWidth = ActualWidth;
                CompareSaveX = Canvas.GetLeft(this);
                CompareSaveY = Canvas.GetTop(this);

                if (this.CanvasPage.CountSelect <= 1)
                {
                    Position = e.GetPosition(this);

                    if (topLenghtSize.Geometry.StrokeContains(topLenghtSize.Pen, Position) && this.IsSelected)
                    {
                        if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1) IstopLenghtSizeUp = true;
                        else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0) IstopLenghtSizeLeft = true;
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1) IstopLenghtSizeDown = true;
                        else IstopLenghtSize = true;

                        this.CaptureMouse();
                    }
                    else if (downLenghtSize.Geometry.StrokeContains(downLenghtSize.Pen, Position) && this.IsSelected)
                    {
                        if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1) IsDownLenghtSizeRight = true;
                        else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0) IsDownLenghtSizeUp = true;
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1) IsDownLenghtSizeLeft = true;
                        else IsDownLenghtSize = true;

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
                        if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1) IsDownSizeTop = true;
                        else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0) IsDownSizeLeft = true;
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1) IsDownSizeDown = true;
                        else IsDownSize = true;

                        this.CaptureMouse();
                    }
                    else if (leftDownSize.Geometry.StrokeContains(leftDownSize.Pen, Position) && this.IsSelected)
                    {
                        if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1) IsleftDownSizeRight = true;
                        else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0) IsleftDownSizeTop = true;
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1) IsleftDownSizeLeft = true;
                        else IsleftDownSize = true;

                        this.CaptureMouse();
                    }
                    else if (rightDownSize.Geometry.StrokeContains(rightDownSize.Pen, Position) && this.IsSelected)
                    {
                        if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == -1) IsRightDownSizeRight = true;
                        else if ((int)this.RenderTransform.Value.M11 == -1 && (int)this.RenderTransform.Value.M12 == 0) IsRightDownSizeTop = true;
                        else if ((int)this.RenderTransform.Value.M11 == 0 && (int)this.RenderTransform.Value.M12 == 1) IsRightDownSizeLeft = true;
                        else IsRightDownSize = true;

                        this.CaptureMouse();
                    }
                    else
                    {
                        this.SelectOne();

                        IsTransformMove = true;
                        IsPipeControlMove = true;

                        this.CaptureMouse();
                    }
                }
                else
                {
                    IsTransformMove = true;

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

            IstopLenghtSize = false;
            IstopLenghtSizeDown = false;           
            IstopLenghtSizeLeft = false;           
            IstopLenghtSizeUp = false;
            IsDownLenghtSize = false;
            IsDownLenghtSizeRight = false;
            IsDownLenghtSizeUp = false;
            IsPipeControlMove = false;          
            IsDownLenghtSizeLeft = false;
            IsTopSize = false;
            IsDownSize = false;
            IsTopSizeRight = false;
            IsTopSizeLeft = false;
            IsDownSizeTop = false;
            IsTopSizeDown = false;
            IsDownSizeLeft = false;
            IsDownSizeDown = false;
            IsleftDownSize = false;
            IsleftDownSizeRight = false;
            IsleftDownSizeTop = false;
            IsleftDownSizeLeft = false;
            IsRightDownSize = false;
            IsRightDownSizeRight = false;
            IsRightDownSizeTop = false;
            IsRightDownSizeLeft = false;

            Issticking = false;

            SumStickingX = 0;
            SumStickingY = 0;

            if (Math.Floor(CompareSaveHeight * 100) / 100 != Math.Floor(ActualHeight * 100) / 100
                || Math.Floor(CompareSaveWidth * 100) / 100 != Math.Floor(ActualWidth * 100) / 100
                || Math.Floor(CompareSaveX * 100) / 100 != Math.Floor(Canvas.GetLeft(this) * 100) / 100
                || Math.Floor(CompareSaveY * 100) / 100 != Math.Floor(Canvas.GetTop(this) * 100) / 100)
            {
                CanvasTab.RepositionAllObjects(CanvasTab);
                CanvasTab.InvalidateMeasure();

                this.ChangePipe90Ser();

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

        private void ChangePipe90Ser()
        {           
            pipe90Ser.TopLenghtSize.point[0] = this.PathFigureTopLenghtSize.StartPoint;
            pipe90Ser.TopLenghtSize.point[1] = this.LineSegmentTopLenghtSize.Point;

            pipe90Ser.DownLenghtSize.point[0] = this.PathFigureDownLenghtSize.StartPoint;
            pipe90Ser.DownLenghtSize.point[1] = this.LineSegmentDownLenghtSize.Point;

            pipe90Ser.TopSize.point[0] = this.PathFigureTopSize.StartPoint;
            pipe90Ser.TopSize.point[1] = this.LineSegmentTopSize.Point;

            pipe90Ser.DownSize.point[0] = this.PathFigureDownSize.StartPoint;
            pipe90Ser.DownSize.point[1] = this.LineSegmentDownSize.Point;

            pipe90Ser.LeftFlange.point[0] = this.PathFigureLeftFlange.StartPoint;
            pipe90Ser.LeftFlange.point[1] = this.PolyLineSegmentLeftFlange.Points[0];
            pipe90Ser.LeftFlange.point[2] = this.PolyLineSegmentLeftFlange.Points[1];
            pipe90Ser.LeftFlange.point[3] = this.PolyLineSegmentLeftFlange.Points[2];
            pipe90Ser.LeftFlange.point[4] = this.PolyLineSegmentLeftFlange.Points[3];

            pipe90Ser.RightFlange.point[0] = this.PathFigureRightFlange.StartPoint;
            pipe90Ser.RightFlange.point[1] = this.PolyLineSegmentRightFlange.Points[0];
            pipe90Ser.RightFlange.point[2] = this.PolyLineSegmentRightFlange.Points[1];
            pipe90Ser.RightFlange.point[3] = this.PolyLineSegmentRightFlange.Points[2];
            pipe90Ser.RightFlange.point[4] = this.PolyLineSegmentRightFlange.Points[3];

            pipe90Ser.TopImage.point[0] = this.PathFigureTopImage.StartPoint;
            pipe90Ser.TopImage.point[1] = this.PolyLineSegmentTopImage.Points[0];
            pipe90Ser.TopImage.point[2] = this.PolyLineSegmentTopImage.Points[1];
            pipe90Ser.TopImage.point[3] = this.PolyLineSegmentTopImage.Points[2];
            pipe90Ser.TopImage.point[4] = this.PolyLineSegmentTopImage.Points[3];

            pipe90Ser.DownImage.point[0] = this.PathFigureDownImage.StartPoint;
            pipe90Ser.DownImage.point[1] = this.PolyLineSegmentDownImage.Points[0];
            pipe90Ser.DownImage.point[2] = this.PolyLineSegmentDownImage.Points[1];
            pipe90Ser.DownImage.point[3] = this.PolyLineSegmentDownImage.Points[2];
            pipe90Ser.DownImage.point[4] = this.PolyLineSegmentDownImage.Points[3];

            pipe90Ser.LeftDownSize.point[0] = this.PathFigureLeftDownSize.StartPoint;
            pipe90Ser.LeftDownSize.point[1] = this.LineSegmentLeftDownSize.Point;

            pipe90Ser.RightDownSize.point[0] = this.PathFigureRightDownSize.StartPoint;
            pipe90Ser.RightDownSize.point[1] = this.LineSegmentRightDownSize.Point;

            pipe90Ser.BorderPipe90.point[0] = this.PathFigureBorder.StartPoint;
            pipe90Ser.BorderPipe90.point[1] = this.PolyLineSegmentBorder.Points[0];
            pipe90Ser.BorderPipe90.point[2] = this.PolyLineSegmentBorder.Points[1];
            pipe90Ser.BorderPipe90.point[3] = this.PolyLineSegmentBorder.Points[2];
            pipe90Ser.BorderPipe90.point[4] = this.PolyLineSegmentBorder.Points[3];

            pipe90Ser.Сoordinates = new Point((double)this.GetValue(Canvas.LeftProperty), (double)this.GetValue(Canvas.TopProperty));

            ((AppWPF)Application.Current).SaveTabItem(CanvasPage.TabItemPage);
        }

        public Point point1 { get; set; }
        public Point point2 { get; set; }
        public Point point3 { get; set; }
        public Point point4 { get; set; }
    }
}

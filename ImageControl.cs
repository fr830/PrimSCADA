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

namespace SCADA
{  
    public class ImageControl : ControlOnCanvasPage
    {
        public Border templateBorder;
        public Image templateImage { get; private set; }
        public GeometryDrawing leftSize;
        public GeometryDrawing rightSize;
        public GeometryDrawing topSize;
        public GeometryDrawing downSize;

        public PathGeometry PathGeometryRightSize;
        public PathFigure PathFigureRightSize;
        public LineSegment LineSegmentRightSize;

        public PathGeometry PathGeometryBorder;
        public PathFigure PathFigureBorder;
        public PolyLineSegment PolyLineSegmentBorder;

        public PathGeometry PathGeometryTopSize;
        public PathFigure PathFigureTopSize;
        public LineSegment LineSegmentTopSize;

        public PathGeometry PathGeometryDownSize;
        public PathFigure PathFigureDownSize;
        public LineSegment LineSegmentDownSize;

        public PathGeometry PathGeometryLeftSize;
        public PathFigure PathFigureLeftSize;
        public LineSegment LineSegmentLeftSize;

        Point p;
        Point Position;

        bool IsRightSize;
        bool IsRightSizeUp;
        bool IsLeftSize;
        bool IsLeftSizeDown;
        bool IsTextControlMove;
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

        public bool IsSave;

        private ImageSer imageSer;
        public ImageSer ImageSer
        {
            get { return imageSer; }
            set { controlOnCanvasSer = value; imageSer = value; }
        }

        static ImageControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageControl), new FrameworkPropertyMetadata(typeof(ImageControl)));
        }

        public ImageControl(PageScada ps, CanvasPage canvasPage, ImageSer imageSer) 
            :base(imageSer)
        {
            this.Focusable = false;
            PS = ps;
            CanvasPage = canvasPage;
            ImageSer = imageSer;
                       
            menuItemProperties.Click += Properties;
        }

        private void Properties(object sender, RoutedEventArgs e)
        {
            DialogWindowPropertiesImage WindowProperties = new DialogWindowPropertiesImage(this);
            WindowProperties.Owner = Application.Current.MainWindow;
            WindowProperties.ShowDialog();

            e.Handled = true;
        }

        public void ChangeImageSer()
        {
            imageSer.LeftSize.point[0] = PathFigureLeftSize.StartPoint;
            imageSer.LeftSize.point[1] = LineSegmentLeftSize.Point;

            imageSer.RightSize.point[0] = PathFigureRightSize.StartPoint;
            imageSer.RightSize.point[1] = LineSegmentRightSize.Point;

            imageSer.TopSize.point[0] = PathFigureTopSize.StartPoint;
            imageSer.TopSize.point[1] = LineSegmentTopSize.Point;

            imageSer.DownSize.point[0] = PathFigureDownSize.StartPoint;
            imageSer.DownSize.point[1] = LineSegmentDownSize.Point;

            imageSer.Border.point[0] = PathFigureBorder.StartPoint;
            imageSer.Border.point[1] = PolyLineSegmentBorder.Points[0];
            imageSer.Border.point[2] = PolyLineSegmentBorder.Points[1];
            imageSer.Border.point[3] = PolyLineSegmentBorder.Points[2];
            imageSer.Border.point[4] = PolyLineSegmentBorder.Points[3];

            imageSer.Сoordinates = new Point((double)this.GetValue(Canvas.LeftProperty), (double)this.GetValue(Canvas.TopProperty));

            imageSer.Width = templateBorder.Width;
            imageSer.Height = templateBorder.Height;

            ((AppWPF)Application.Current).SaveTabItem(CanvasTab.TabItemParent);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (IsSelected)
            {
                DialogWindowPropertiesImage WindowProperties = new DialogWindowPropertiesImage(this);
                WindowProperties.Owner = Application.Current.MainWindow;
                WindowProperties.ShowDialog();
            }

            e.Handled = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            leftSize = GetTemplateChild("LeftSize") as GeometryDrawing;
            rightSize = GetTemplateChild("RightSize") as GeometryDrawing;
            topSize = GetTemplateChild("TopSize") as GeometryDrawing;
            downSize = GetTemplateChild("DownSize") as GeometryDrawing;
            border = GetTemplateChild("Border") as GeometryDrawing;

            PathGeometryRightSize = (PathGeometry)rightSize.Geometry;
            PathFigureRightSize = PathGeometryRightSize.Figures[0];
            LineSegmentRightSize = (LineSegment)PathFigureRightSize.Segments[0];

            PathGeometryBorder = (PathGeometry)border.Geometry;
            PathFigureBorder = PathGeometryBorder.Figures[0];
            PolyLineSegmentBorder = (PolyLineSegment)PathFigureBorder.Segments[0];

            PathGeometryTopSize = (PathGeometry)topSize.Geometry;
            PathFigureTopSize = PathGeometryTopSize.Figures[0];
            LineSegmentTopSize = (LineSegment)PathFigureTopSize.Segments[0];

            PathGeometryDownSize = (PathGeometry)downSize.Geometry;
            PathFigureDownSize = PathGeometryDownSize.Figures[0];
            LineSegmentDownSize = (LineSegment)PathFigureDownSize.Segments[0];

            PathGeometryLeftSize = (PathGeometry)leftSize.Geometry;
            PathFigureLeftSize = PathGeometryLeftSize.Figures[0];
            LineSegmentLeftSize = (LineSegment)PathFigureLeftSize.Segments[0];

            templateBorder = GetTemplateChild("TemplateBorder") as Border;

            templateImage = GetTemplateChild("TemplateImage") as Image;            

            Thickness thickness = new Thickness(ImageSer.BorderThickness);
            BorderThickness = thickness;
            Background = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(imageSer.ColorBackGround)));
            templateBorder.BorderBrush = ImageSer.ColorBorder;

            templateBorder.Width = ImageSer.Width;
            templateBorder.Height = ImageSer.Height;                                                          
        }

        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);

            if (!this.IsSelected) e.Handled = true;
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

                if (this.CanvasTab.CountSelect <= 1)
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

                        IsTextControlMove = true;

                        this.CaptureMouse();
                    }
                }
                else
                {
                    if (!this.IsSelected && Keyboard.Modifiers != ModifierKeys.Control)
                    {
                        this.Select();
                        IsTextControlMove = true;
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
                p.X *= -1;

                if (templateBorder.Width - p.X < 10)
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

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X -= p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X -= p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X -= p.X;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X -= p.X;
                LineSegmentDownSize.Point = downSizePoint;

                templateBorder.Width -= p.X;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x += p.X;

                this.SetValue(Canvas.LeftProperty, x);

                Coordinate();
            }
            else if (IsLeftSizeDown)
            {
                p.Y *= -1;

                if (templateBorder.Width + p.Y < 10)
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

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X += p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X += p.Y;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X += p.Y;
                LineSegmentDownSize.Point = downSizePoint;

                templateBorder.Width += p.Y;

                double x = (double)this.GetValue(Canvas.TopProperty);
                x += p.Y;

                this.SetValue(Canvas.TopProperty, x);
            }
            else if (IsLeftSizeRight)
            {
                p.X *= -1;

                if (templateBorder.Width + p.X < 10)
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

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X += p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X += p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X += p.X;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X += p.X;
                LineSegmentDownSize.Point = downSizePoint;

                templateBorder.Width += p.X;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x += p.X;

                this.SetValue(Canvas.LeftProperty, x);
            }
            else if (IsLeftSizeUp)
            {
                p.Y *= -1;

                if (templateBorder.Width - p.Y < 10)
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

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X -= p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X -= p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X -= p.Y;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X -= p.Y;
                LineSegmentDownSize.Point = downSizePoint;

                templateBorder.Width -= p.Y;

                double x = (double)this.GetValue(Canvas.TopProperty);
                x += p.Y;

                this.SetValue(Canvas.TopProperty, x);

                Coordinate();
            }
            #endregion
            #region IsRightSize
            if (IsRightSize)
            {
                if (templateBorder.Width - p.X < 10)
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

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X -= p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X -= p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X -= p.X;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X -= p.X;
                LineSegmentDownSize.Point = downSizePoint;

                templateBorder.Width -= p.X;
            }
            else if (IsRightSizeUp)
            {
                if (templateBorder.Width + p.Y < 10)
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

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X += p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X += p.Y;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X += p.Y;
                LineSegmentDownSize.Point = downSizePoint;

                templateBorder.Width += p.Y;

                Coordinate();
            }
            else if (IsRightSizeLeft)
            {
                if (templateBorder.Width + p.X < 10)
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

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X += p.X;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X += p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X += p.X;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X += p.X;
                LineSegmentDownSize.Point = downSizePoint;

                templateBorder.Width += p.X;

                Coordinate();
            }
            else if (IsRightSizeDown)
            {
                if (templateBorder.Width - p.Y < 10)
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

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[0];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[1];
                borderPipePoint2.X -= p.Y;
                PolyLineSegmentBorder.Points[0] = borderPipePoint2;
                borderPipePoint3.X -= p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint3;

                Point topSizePoint = LineSegmentTopSize.Point;
                topSizePoint.X -= p.Y;
                LineSegmentTopSize.Point = topSizePoint;

                Point downSizePoint = LineSegmentDownSize.Point;
                downSizePoint.X -= p.Y;
                LineSegmentDownSize.Point = downSizePoint;

                templateBorder.Width -= p.Y;
            }
            #endregion
            #region IsTopSize
            else if (IsTopSize)
            {
                p.Y *= -1;

                if (templateBorder.Height - p.Y < 10)
                {
                    if ((p.Y > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point downSizePoint = PathFigureDownSize.StartPoint;
                downSizePoint.Y -= p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y -= p.Y;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = LineSegmentRightSize.Point;
                rightSizePoint.Y -= p.Y;
                LineSegmentRightSize.Point = rightSizePoint;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y -= p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                templateBorder.Height -= p.Y;

                double y = (double)this.GetValue(Canvas.TopProperty);
                y += p.Y;

                this.SetValue(Canvas.TopProperty, y);

                Coordinate();
            }
            else if (IsTopSizeLeft)
            {
                p.X *= -1;

                if (templateBorder.Height - p.X < 10)
                {
                    if ((p.X > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point downSizePoint = PathFigureDownSize.StartPoint;
                downSizePoint.Y -= p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y -= p.X;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = LineSegmentRightSize.Point;
                rightSizePoint.Y -= p.X;
                LineSegmentRightSize.Point = rightSizePoint;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y -= p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                templateBorder.Height -= p.X;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x += p.X;

                this.SetValue(Canvas.LeftProperty, x);

                Coordinate();
            }
            else if (IsTopSizeDown)
            {
                p.Y *= -1;

                if (templateBorder.Height + p.Y < 10)
                {
                    if ((p.Y < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point downSizePoint = PathFigureDownSize.StartPoint;
                downSizePoint.Y += p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y += p.Y;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = LineSegmentRightSize.Point;
                rightSizePoint.Y += p.Y;
                LineSegmentRightSize.Point = rightSizePoint;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                templateBorder.Height += p.Y;

                double y = (double)this.GetValue(Canvas.TopProperty);
                y += p.Y;

                this.SetValue(Canvas.TopProperty, y);
            }
            else if (IsTopSizeRight)
            {
                p.X *= -1;

                if (templateBorder.Height + p.X < 10)
                {
                    if ((p.X < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point downSizePoint = PathFigureDownSize.StartPoint;
                downSizePoint.Y += p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y += p.X;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = LineSegmentRightSize.Point;
                rightSizePoint.Y += p.X;
                LineSegmentRightSize.Point = rightSizePoint;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y += p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                templateBorder.Height += p.X;

                double x = (double)this.GetValue(Canvas.LeftProperty);
                x += p.X;

                this.SetValue(Canvas.LeftProperty, x);
            }
            #endregion
            #region IsDownSize
            else if (IsDownSize)
            {
                if (templateBorder.Height - p.Y < 10)
                {
                    if ((p.Y > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point downSizePoint = PathFigureDownSize.StartPoint;
                downSizePoint.Y -= p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y -= p.Y;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = LineSegmentRightSize.Point;
                rightSizePoint.Y -= p.Y;
                LineSegmentRightSize.Point = rightSizePoint;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y -= p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                templateBorder.Height -= p.Y;
            }
            else if (IsDownSizeRight)
            {
                if (templateBorder.Height - p.X < 10)
                {
                    if ((p.X > 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point downSizePoint = PathFigureDownSize.StartPoint;
                downSizePoint.Y -= p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y -= p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y -= p.X;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = LineSegmentRightSize.Point;
                rightSizePoint.Y -= p.X;
                LineSegmentRightSize.Point = rightSizePoint;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y -= p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y -= p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                templateBorder.Height -= p.X;
            }
            else if (IsDownSizeTop)
            {
                if (templateBorder.Height + p.Y < 10)
                {
                    if ((p.Y < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point downSizePoint = PathFigureDownSize.StartPoint;
                downSizePoint.Y += p.Y;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.Y;
                LineSegmentDownSize.Point = downSizePoint2;

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y += p.Y;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = LineSegmentRightSize.Point;
                rightSizePoint.Y += p.Y;
                LineSegmentRightSize.Point = rightSizePoint;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y += p.Y;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.Y;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                templateBorder.Height += p.Y;

                Coordinate();
            }
            else if (IsDownSizeLeft)
            {
                if (templateBorder.Height + p.X < 10)
                {
                    if ((p.X < 0))
                    {
                        p = e.GetPosition(Application.Current.MainWindow);
                        e.Handled = true;
                        return;
                    }
                }

                Point downSizePoint = PathFigureDownSize.StartPoint;
                downSizePoint.Y += p.X;
                PathFigureDownSize.StartPoint = downSizePoint;
                Point downSizePoint2 = LineSegmentDownSize.Point;
                downSizePoint2.Y += p.X;
                LineSegmentDownSize.Point = downSizePoint2;

                Point leftSizePoint = LineSegmentLeftSize.Point;
                leftSizePoint.Y += p.X;
                LineSegmentLeftSize.Point = leftSizePoint;

                Point rightSizePoint = LineSegmentRightSize.Point;
                rightSizePoint.Y += p.X;
                LineSegmentRightSize.Point = rightSizePoint;

                Point borderPipePoint2 = PolyLineSegmentBorder.Points[1];
                Point borderPipePoint3 = PolyLineSegmentBorder.Points[2];
                borderPipePoint2.Y += p.X;
                PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                borderPipePoint3.Y += p.X;
                PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                templateBorder.Height += p.X;

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
            else if (IsTextControlMove && CanvasPage.CountSelect <= 1)
            {
                double x = (double)this.GetValue(Canvas.LeftProperty);
                x -= p.X;

                double y = (double)this.GetValue(Canvas.TopProperty);
                y -= p.Y;

                this.SetValue(Canvas.LeftProperty, x);
                this.SetValue(Canvas.TopProperty, y);

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
            if (!this.IsMouseCaptured)
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

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);

            IsRightSizeDown = false;
            IsLeftSizeRight = false;
            IsRightSizeLeft = false;
            IsLeftSizeDown = false;
            IsRightSizeUp = false;
            IsLeftSize = false;
            IsTextControlMove = false;
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

                this.ChangeImageSer();

                if (IsControlMoveMassive)
                {
                    foreach (ControlOnCanvasPage controlOnCanvas in this.CanvasTab.SelectedControlOnCanvas)
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
    }
}

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
    public class CanvasTab : Canvas
    {
        public TabItemParent TabItemParent { get; set; }
        public ItemScada IS { get; set; }
        public int CountSelect { get; set; }   // счетчик выделенных объектов на канвасе
        protected Point Delta;
        protected Point ComparePoint;
        protected Rect rect = new Rect(0, 0, 0, 0);
        protected RectangleGeometry rectangleGeometry = new RectangleGeometry();
        public bool IsNegativeSelectX { get; set; }
        public bool IsNegativeSelectY { get; set; }

        protected System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();

        protected Rectangle selectedRectangle;
        public Rectangle SelectedRectangle
        {
            get { return selectedRectangle; }
            set { selectedRectangle = value; }
        }
        public List<ControlOnCanvas> SelectedControlOnCanvas = new List<ControlOnCanvas>();

        protected List<ControlOnCanvas> hitResultsList = new List<ControlOnCanvas>();

        static CanvasTab()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CanvasTab), new FrameworkPropertyMetadata(typeof(CanvasTab)));
        }

        protected CanvasTab()
        {
            SelectedRectangle = new Rectangle();
            SelectedRectangle.Width = 0;
            SelectedRectangle.Height = 0;
            SelectedRectangle.Stroke = new SolidColorBrush(Colors.Black);
            SelectedRectangle.StrokeThickness = 2;
        }
        
        
        protected override void OnMouseLeave(MouseEventArgs e)
        {
 	        base.OnMouseLeave(e);

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            if (mainWindow != null) // Если не проверить на пустую ссылку, то при выделенном объекте на канвасе и выходе при alt+f4 выдает исключение
                mainWindow.LabelCoordinateCursor.Content = null;

            e.Handled = true;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            mainWindow.LabelCoordinateCursor.Content = null;

            e.Handled = true;
        }
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            Point coordinate = e.GetPosition(this);

            if (mainWindow.TabControlMain.SelectedIndex != -1)
            {
                mainWindow.LabelCoordinateCursor.Content = string.Format("{0}{1:F2}{2}{3}{4}", "X: ", coordinate.X, " ", "Y: ", coordinate.Y);
            }

            e.Effects = DragDropEffects.None;

            if (this is CanvasControlPanel)
            {
                DragAndDropCanvas data = (DragAndDropCanvas)e.Data.GetData(typeof(DragAndDropCanvas));

                if (data != null)
                {
                    if (data.IsEthernet || data.IsCom || data.IsModbus)
                    {
                        e.Effects = DragDropEffects.Copy;
                    }
                }
            }
            else if (this is CanvasPage)
            {
                DragAndDropCanvas data = (DragAndDropCanvas)e.Data.GetData(typeof(DragAndDropCanvas));

                if (data != null)
                {
                    if (!data.IsEthernet && !data.IsCom && !data.IsModbus)
                    {
                        e.Effects = DragDropEffects.Copy;
                    }                  
                }
            }           

            e.Handled = true;
        }
        
        public HitTestResultBehavior MyHitTestResultCallback(HitTestResult result)
        {
            // Retrieve the results of the hit test.
            IntersectionDetail intersectionDetail = ((GeometryHitTestResult)result).IntersectionDetail;

            ControlOnCanvas controlAdd = null;

            if (result.VisualHit is Image)
            {
                if (((Image)result.VisualHit).TemplatedParent is ControlOnCanvas)
                {
                    controlAdd = ((Image)result.VisualHit).TemplatedParent as ControlOnCanvas;
                }
            }

            switch (intersectionDetail)
            {
                case IntersectionDetail.FullyContains:

                    // Add the hit test result to the list that will be processed after the enumeration.

                    if (controlAdd != null)
                        hitResultsList.Add(controlAdd);

                    return HitTestResultBehavior.Continue;

                case IntersectionDetail.Intersects:

                    // Set the behavior to return visuals at all z-order levels. 
                    if (controlAdd != null)
                        hitResultsList.Add(controlAdd);

                    return HitTestResultBehavior.Continue;

                case IntersectionDetail.FullyInside:

                    // Set the behavior to return visuals at all z-order levels. 
                    if (controlAdd != null)
                        hitResultsList.Add(controlAdd);

                    return HitTestResultBehavior.Continue;

                default:
                    return HitTestResultBehavior.Stop;
            }
        }

        public void RepositionAllObjects(Canvas canvas)
        {
            adjustNodesHorizontally(canvas);
            adjustNodesVertically(canvas);
        }

        private void adjustNodesVertically(Canvas canvas)
        {
            double minLeft = Canvas.GetLeft(canvas.Children[0]);
            foreach (UIElement child in canvas.Children)
            {
                double left = Canvas.GetLeft(child);
                if (child is ControlOnCanvas)
                {
                    Matrix m = ((ControlOnCanvas)child).RenderTransform.Value;
                    if (((int)m.M11 == -1) && ((int)m.M12 == 0))
                    {
                        double d = ((ControlOnCanvas)child).ActualWidth * -1;

                        left += d;
                    }
                    else if (((int)m.M11 == 0) && ((int)m.M12 == 1))
                    {
                        double d = ((ControlOnCanvas)child).ActualHeight * -1;

                        left += d;
                    }
                }

                if (left < minLeft)
                    minLeft = left;
            }

            if (minLeft < 0)
            {
                minLeft = -minLeft;
                foreach (ControlOnCanvas controlOnCanvas in canvas.Children)
                {
                    Canvas.SetLeft(controlOnCanvas, Canvas.GetLeft(controlOnCanvas) + minLeft);
                    controlOnCanvas.controlOnCanvasSer.Сoordinates = new Point(Canvas.GetLeft(controlOnCanvas), controlOnCanvas.controlOnCanvasSer.Сoordinates.Y);
                }
            }
        }

        private void adjustNodesHorizontally(Canvas canvas)
        {
            double minTop = Canvas.GetTop(canvas.Children[0]);
            foreach (UIElement child in canvas.Children)
            {
                double top = Canvas.GetTop(child);
                if (child is ControlOnCanvas)
                {
                    Matrix m = ((ControlOnCanvas)child).RenderTransform.Value;

                    if (((int)m.M11 == 0) && ((int)m.M12 == -1))
                    {
                        double d = ((ControlOnCanvas)child).ActualWidth * -1;

                        top += d;
                    }
                    else if (((int)m.M11 == -1) && ((int)m.M12 == 0))
                    {
                        double d = ((ControlOnCanvas)child).ActualHeight * -1;

                        top += d;
                    }
                }

                if (top < minTop)
                    minTop = top;
            }

            if (minTop < 0)
            {
                minTop = -minTop;
                foreach (ControlOnCanvas controlOnCanvas in canvas.Children)
                {
                    Canvas.SetTop(controlOnCanvas, Canvas.GetTop(controlOnCanvas) + minTop);
                    controlOnCanvas.controlOnCanvasSer.Сoordinates = new Point(controlOnCanvas.controlOnCanvasSer.Сoordinates.X, Canvas.GetTop(controlOnCanvas));
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);
            var desiredSize = new Size();
            foreach (UIElement child in Children)
            {
                desiredSize = new Size(
                    Math.Max(desiredSize.Width, GetLeft(child) + child.DesiredSize.Width),
                    Math.Max(desiredSize.Height, GetTop(child) + child.DesiredSize.Height));
            }
            return desiredSize;
        }
    }
}

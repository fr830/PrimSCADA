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
    public class ControlOnCanvas : Control, IComparable
    {
        public CanvasTab CanvasTab { get; set; }
        public bool IsSelected { get; set; }
        public GeometryDrawing border { get; set; }
        public ControlOnCanvasSer controlOnCanvasSer { get; set; }
        public TextBox CoordinateX { get; set; }
        public TextBox CoordinateY { get; set; }
        public Label LabelSelected { get; set; }
        public ItemScada IS { get; set; }
       
        protected MenuItem menuItemCut;
        protected MenuItem menuItemCopy;
        protected MenuItem menuItemProperties;
        protected MenuItem menuItemDelete;
        protected MenuItem menuItemForward;
        protected MenuItem menuItemBackward;

        // Если изменена высота или ширина или координаты, то контрол изменился и подлежит сохранению
        public double CompareSaveHeight { get; set; }
        public double CompareSaveWidth { get; set; }
        public double CompareSaveX { get; set; }
        public double CompareSaveY { get; set; }

        private RotateTransform rotateTransform = new RotateTransform();
        public RotateTransform RotateTransform
        {
            get { return rotateTransform; }
        }

        private int rotate;
        public int Rotate
        {
            get { return rotate; }
            set 
            {
                if (value % 360 == 0) rotate = 0;
                else rotate = value;
                controlOnCanvasSer.Transform = rotate;
                this.RotateTransform.Angle = rotate;
            }
        }

        private int zIndex;
        public int ZIndex
        {
            get { return zIndex; }
            set
            {
                zIndex = value;
                controlOnCanvasSer.ZIndex = value;
                Canvas.SetZIndex(this, value);
            }
        }

        static ControlOnCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ControlOnCanvas), new FrameworkPropertyMetadata(typeof(ControlOnCanvas)));
        }

        public void Coordinate()
        {
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

        protected ControlOnCanvas(ControlOnCanvasSer ser)
        {
            controlOnCanvasSer = ser;
            Rotate = ser.Transform;
            ZIndex = ser.ZIndex;

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            CoordinateX = mainWindow.CoordinateObjectX;
            CoordinateY = mainWindow.CoordinateObjectY;
            LabelSelected = mainWindow.LabelSelected;
           
            this.RenderTransform = RotateTransform;

            ContextMenu contextMenu = new ContextMenu();

            Image properties = new Image();
            properties.Source = new BitmapImage(new Uri("Images/SettingEditor.png", UriKind.Relative));

            Image rotateLeft90 = new Image();
            rotateLeft90.Source = new BitmapImage(new Uri("Images/RotateLeft90_16.png", UriKind.Relative));

            Image rotateRight90 = new Image();
            rotateRight90.Source = new BitmapImage(new Uri("Images/RotateRight90_16.png", UriKind.Relative));

            Image deleteImage = new Image();
            deleteImage.Source = new BitmapImage(new Uri("Images/Close16.png", UriKind.Relative));

            Image copyImage = new Image();
            copyImage.Source = new BitmapImage(new Uri("Images/CopyObject16.ico", UriKind.Relative));

            Image cutImage = new Image();
            cutImage.Source = new BitmapImage(new Uri("Images/cut16.png", UriKind.Relative));

            Image forwardImage = new Image();
            forwardImage.Source = new BitmapImage(new Uri("Images/Forward.png", UriKind.Relative));

            Image backwardImage = new Image();
            backwardImage.Source = new BitmapImage(new Uri("Images/Backward.png", UriKind.Relative));

            MenuItem menuItemRotateLeft90 = new MenuItem();
            menuItemRotateLeft90.Click += RotateLeft;
            menuItemRotateLeft90.Icon = rotateLeft90;
            menuItemRotateLeft90.Header = "Поернуть на 90 влево";

            MenuItem menuItemRotateRight90 = new MenuItem();
            menuItemRotateRight90.Click += RotateRight;
            menuItemRotateRight90.Icon = rotateRight90;
            menuItemRotateRight90.Header = "Повернуть на 90 вправо";

            menuItemCut = new MenuItem();
            menuItemCut.Icon = cutImage;
            menuItemCut.Click += Cut;
            menuItemCut.Header = "Вырезать";

            menuItemCopy = new MenuItem();
            menuItemCopy.Click += Copy;
            menuItemCopy.Icon = copyImage;
            menuItemCopy.Header = "Копировать";

            menuItemProperties = new MenuItem();
            menuItemProperties.Icon = properties;
            menuItemProperties.Header = "Свойства";

            menuItemDelete = new MenuItem();
            menuItemDelete.Click += Delete;
            menuItemDelete.Icon = deleteImage;
            menuItemDelete.Header = "Удалить";

            menuItemForward = new MenuItem();
            menuItemForward.Click += Forward;
            menuItemForward.Icon = forwardImage;
            menuItemForward.Header = "На передний план";

            menuItemBackward = new MenuItem();
            menuItemBackward.Click += Backward;
            menuItemBackward.Icon = backwardImage;
            menuItemBackward.Header = "На задний план";

            contextMenu.Items.Add(menuItemRotateLeft90);
            contextMenu.Items.Add(menuItemRotateRight90);
            contextMenu.Items.Add(menuItemCut);
            contextMenu.Items.Add(menuItemCopy);
            contextMenu.Items.Add(menuItemForward);
            contextMenu.Items.Add(menuItemBackward);
            contextMenu.Items.Add(menuItemProperties);
            contextMenu.Items.Add(menuItemDelete);

            this.ContextMenu = contextMenu;          
        }

        private void Backward(object sender, RoutedEventArgs e)
        {
            int compare = this.ZIndex;
            int relatively = compare;

            // Если zindex равен нижнему индексу ничего не делаем
            if (Canvas.GetZIndex(this) == 1)
            {
                e.Handled = true;
                return;
            }
            else
            {
                foreach (ControlOnCanvas controlOnCanvas in CanvasTab.Children)
                {                   
                    if (this == controlOnCanvas)
                        this.ZIndex = 1;
                    else
                        if (controlOnCanvas.ZIndex < relatively)
                            controlOnCanvas.ZIndex += 1;                   
                }
            }

            if (compare != this.ZIndex)
                ((AppWPF)Application.Current).SaveTabItem(this.CanvasTab.TabItemParent);

            e.Handled = true;
        }

        private void Forward(object sender, RoutedEventArgs e)
        {
            int compare = this.ZIndex;
            int relatively = compare;
            int i = compare;
            // Если zindex равен верхнему индексу ничего не делаем
            if (i == CanvasTab.Children.Count)
            {
                e.Handled = true;
                return;
            }
            else
            {
                foreach (ControlOnCanvas controlOnCanvas in CanvasTab.Children)
                {
                    if (controlOnCanvas == this)                       
                        this.ZIndex = CanvasTab.Children.Count;
                    else if (controlOnCanvas.ZIndex > relatively)
                            controlOnCanvas.ZIndex -= 1;               
                }
            }

            if (compare != this.ZIndex)
                ((AppWPF)Application.Current).SaveTabItem(this.CanvasTab.TabItemParent);

            e.Handled = true;
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)((AppWPF)System.Windows.Application.Current).MainWindow;
            mainWindow.CurrentObjects.Clear();

            foreach (ControlOnCanvas controlOnCanvas in this.CanvasTab.SelectedControlOnCanvas)
            {
                mainWindow.CurrentObjects.Add(controlOnCanvas);
            }

            ClipboardManipulation copyObjects = null;

            if (this is ControlOnCanvasPage)
            {
                copyObjects = new ClipboardManipulation(1);
            }
            else if (this is ControlOnCanvasControlPanel)
            {
                copyObjects = new ClipboardManipulation(3);
            }

            Clipboard.SetDataObject(copyObjects);

            e.Handled = true;
            return;
        }

        private void Cut(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)((AppWPF)System.Windows.Application.Current).MainWindow;
            mainWindow.CurrentObjects.Clear();

            foreach (ControlOnCanvas controlOnCanvas in this.CanvasTab.Children)
            {
                if (controlOnCanvas.IsSelected)
                {
                    mainWindow.CurrentObjects.Add(controlOnCanvas);
                }
            }

            ClipboardManipulation cutObjects = null;

            if(this is ControlOnCanvasPage)
            {
                cutObjects = new ClipboardManipulation(2);
            }
            else if (this is ControlOnCanvasControlPanel)
            {
                cutObjects = new ClipboardManipulation(4);
            }
            
            Clipboard.SetDataObject(cutObjects);

            e.Handled = true;
        }

        public void AreaSelect()
        {
            if (!this.IsSelected)
            {
                this.IsSelected = true;
                this.border.Pen.Brush.Opacity = 100;
                this.CanvasTab.CountSelect += 1;

                CanvasTab.SelectedControlOnCanvas.Add(this);
            }

            LabelSelected.Content = "Выделенно объектов: " + CanvasTab.CountSelect;
        }

        public virtual void SelectOne() { }
        public virtual void Select() { }

        protected void RotateRight(object sender, RoutedEventArgs e)
        {
            Rotate = Rotate + 90;

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
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
            }));

            this.CanvasTab.RepositionAllObjects(this.CanvasTab);
            this.CanvasTab.InvalidateMeasure();

            e.Handled = true;
        }

        protected void RotateLeft(object sender, RoutedEventArgs e)
        {
            Rotate = Rotate - 90;
          
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
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
            }));

            this.CanvasTab.RepositionAllObjects(this.CanvasTab);
            this.CanvasTab.InvalidateMeasure();

            e.Handled = true;
        }

        public virtual void DeleteItem(ControlOnCanvas objectOnCanvas) { }

        public void Delete(object sender, RoutedEventArgs e)
        {
            foreach (ControlOnCanvas objectOnCanvas in CanvasTab.SelectedControlOnCanvas)
            {
                DeleteItem(objectOnCanvas);
            }

            if(CanvasTab.Children.Count != 0)
            {
                int count = 0;
                List<ControlOnCanvas> collectionControl = new List<ControlOnCanvas>();

                foreach (ControlOnCanvas controlOnCanvas in CanvasTab.Children)
                {
                    collectionControl.Add(controlOnCanvas);
                }

                collectionControl.Sort();                    
                foreach (ControlOnCanvas controlOnCanvas in collectionControl)
                {
                    count += 1;
                    controlOnCanvas.ZIndex = count;
                }
            }          

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            mainWindow.CoordinateObjectX.IsReadOnly = true;
            mainWindow.CoordinateObjectY.IsReadOnly = true;
            mainWindow.CoordinateObjectX.Text = null;
            mainWindow.CoordinateObjectY.Text = null;

            mainWindow.TextBoxDiameter.IsReadOnly = true;
            mainWindow.TextBoxDiameter.Text = null;

            mainWindow.ComboBoxEnvironment.IsEnabled = false;
            mainWindow.ComboBoxEnvironment.SelectedIndex = -1;

            CanvasTab.CountSelect = 0;
            LabelSelected.Content = "Выделенно объектов: " + CanvasTab.CountSelect;

            ((AppWPF)Application.Current).SaveTabItem(CanvasTab.TabItemParent);

            CanvasTab.SelectedControlOnCanvas.Clear();

            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }            

            e.Handled = true;
        }
       
        public int CompareTo(object obj)
        {
            ControlOnCanvas otherControlOnCanvas = obj as ControlOnCanvas;

            if (otherControlOnCanvas != null) return this.ZIndex.CompareTo(otherControlOnCanvas.controlOnCanvasSer.ZIndex);
            else throw new ArgumentException("Пустой объект для сортировки");
        }
    }
}

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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для DialogWindowPropertiesPipe90.xaml
    /// </summary>
    public partial class DialogWindowPropertiesPipe90 : Window
    {
        private Pipe90 pipe90;
        public Pipe90 Pipe90
        {
            get { return pipe90; }
            set { pipe90 = value; }
        }

        ComboBox ComboBoxEnvironment;
        TextBox textBoxDiameter;
        bool IsSave;

        public DialogWindowPropertiesPipe90(Pipe90 pipe)
        {
            InitializeComponent();

            Pipe90 = pipe;
        }

        private void SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != e.OldValue)// Новое выделение не должно совпадать со старым
            {
                TreeViewItem Selected = (TreeViewItem)e.NewValue;
                if ((string)Selected.Header == "Общие")
                {
                    GridSetting.Children.Clear();
                    GridSetting.RowDefinitions.Clear();

                    GroupBoxSetting.Header = "Общие";

                    RowDefinition Row0 = new RowDefinition();
                    Row0.Height = new GridLength(30, GridUnitType.Pixel);

                    RowDefinition Row1 = new RowDefinition();
                    Row1.Height = new GridLength(30, GridUnitType.Pixel);

                    RowDefinition Row2 = new RowDefinition();
                    Row2.Height = new GridLength(30, GridUnitType.Pixel);

                    RowDefinition Row3 = new RowDefinition();
                    Row3.Height = new GridLength(30, GridUnitType.Pixel);

                    ColumnDefinition Column0 = new ColumnDefinition();
                    Column0.Width = new GridLength(400, GridUnitType.Auto);

                    ColumnDefinition Column1 = new ColumnDefinition();
                    Column1.Width = new GridLength(1, GridUnitType.Star);

                    GridSetting.RowDefinitions.Add(Row0);
                    GridSetting.RowDefinitions.Add(Row1);
                    GridSetting.RowDefinitions.Add(Row2);
                    GridSetting.RowDefinitions.Add(Row3);
                    GridSetting.ColumnDefinitions.Add(Column0);
                    GridSetting.ColumnDefinitions.Add(Column1);

                    Label LebelEnvironment = new Label();
                    LebelEnvironment.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    LebelEnvironment.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    LebelEnvironment.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
                    LebelEnvironment.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
                    LebelEnvironment.Content = "Среда: ";
                    LebelEnvironment.SetValue(Grid.RowProperty, 0);
                    LebelEnvironment.SetValue(Grid.ColumnProperty, 0);

                    Label LebelDiameter = new Label();
                    LebelDiameter.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    LebelDiameter.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    LebelDiameter.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
                    LebelDiameter.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
                    LebelDiameter.Content = "Диаметр: ";
                    LebelDiameter.SetValue(Grid.RowProperty, 1);
                    LebelDiameter.SetValue(Grid.ColumnProperty, 0);

                    Label LebelZIndex = new Label();
                    LebelZIndex.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    LebelZIndex.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    LebelZIndex.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
                    LebelZIndex.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
                    LebelZIndex.Content = "ZIndex: ";
                    LebelZIndex.SetValue(Grid.RowProperty, 2);
                    LebelZIndex.SetValue(Grid.ColumnProperty, 0);

                    ComboBoxItem itemExhaus = new ComboBoxItem();
                    itemExhaus.Content = "Уходящие газы";

                    ComboBoxItem itemSteam = new ComboBoxItem();
                    itemSteam.Content = "Пар";

                    ComboBoxItem itemWater = new ComboBoxItem();
                    itemWater.Content = "Вода";

                    ComboBoxItem itemMasut = new ComboBoxItem();
                    itemMasut.Content = "Мазут";

                    ComboBoxItem itemAir = new ComboBoxItem();
                    itemAir.Content = "Воздух";

                    ComboBoxEnvironment = new ComboBox();
                    ComboBoxEnvironment.Items.Add(itemExhaus);
                    ComboBoxEnvironment.Items.Add(itemMasut);
                    ComboBoxEnvironment.Items.Add(itemWater);
                    ComboBoxEnvironment.Items.Add(itemSteam);
                    ComboBoxEnvironment.Items.Add(itemAir);

                    if (Pipe90.Pipe90Ser.Environment == 4)
                    { ComboBoxEnvironment.SelectedIndex = 4; }
                    else if (Pipe90.Pipe90Ser.Environment == 3)
                    { ComboBoxEnvironment.SelectedIndex = 3; }
                    else if (Pipe90.Pipe90Ser.Environment == 2)
                    { ComboBoxEnvironment.SelectedIndex = 2; }
                    else if (Pipe90.Pipe90Ser.Environment == 0)
                    { ComboBoxEnvironment.SelectedIndex = 0; }
                    else if (Pipe90.Pipe90Ser.Environment == 1)
                    { ComboBoxEnvironment.SelectedIndex = 1; }

                    ComboBoxEnvironment.SetValue(Grid.RowProperty, 0);
                    ComboBoxEnvironment.SetValue(Grid.ColumnProperty, 1);

                    textBoxDiameter = new TextBox();
                    textBoxDiameter.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                    textBoxDiameter.Text = Convert.ToString(this.Pipe90.Diameter);
                    textBoxDiameter.SetValue(Grid.RowProperty, 1);
                    textBoxDiameter.SetValue(Grid.ColumnProperty, 1);

                    TextBox tbZIndex = new TextBox();
                    tbZIndex.IsReadOnly = true;
                    tbZIndex.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                    tbZIndex.Text = Convert.ToString(this.Pipe90.ZIndex);
                    tbZIndex.SetValue(Grid.RowProperty, 2);
                    tbZIndex.SetValue(Grid.ColumnProperty, 1);

                    GridSetting.Children.Add(LebelEnvironment);
                    GridSetting.Children.Add(LebelDiameter);
                    GridSetting.Children.Add(LebelZIndex);
                    GridSetting.Children.Add(ComboBoxEnvironment);
                    GridSetting.Children.Add(textBoxDiameter);
                    GridSetting.Children.Add(tbZIndex);
                }

                e.Handled = true;
            }
        }

        void PipeSerSave(Pipe pipe)
        {
            pipe.PipeSer.LeftSize.point[0] = pipe.PathFigureLeftSize.StartPoint;
            pipe.PipeSer.LeftSize.point[1] = pipe.LineSegmentLeftSize.Point;

            pipe.PipeSer.RightSize.point[0] = pipe.PathFigureRightSize.StartPoint;
            pipe.PipeSer.RightSize.point[1] = pipe.LineSegmentRightSize.Point;

            pipe.PipeSer.TopSize.point[0] = pipe.PathFigureTopSize.StartPoint;
            pipe.PipeSer.TopSize.point[1] = pipe.LineSegmentTopSize.Point;

            pipe.PipeSer.DownSize.point[0] = pipe.PathFigureDownSize.StartPoint;
            pipe.PipeSer.DownSize.point[1] = pipe.LineSegmentDownSize.Point;

            pipe.PipeSer.LeftFlange.point[0] = pipe.PathFigureLeftFlange.StartPoint;
            pipe.PipeSer.LeftFlange.point[1] = pipe.PolyLineSegmentLeftFlange.Points[0];
            pipe.PipeSer.LeftFlange.point[2] = pipe.PolyLineSegmentLeftFlange.Points[1];
            pipe.PipeSer.LeftFlange.point[3] = pipe.PolyLineSegmentLeftFlange.Points[2];
            pipe.PipeSer.LeftFlange.point[4] = pipe.PolyLineSegmentLeftFlange.Points[3];

            pipe.PipeSer.RightFlange.point[0] = pipe.PathFigureRightFlange.StartPoint;
            pipe.PipeSer.RightFlange.point[1] = pipe.PolyLineSegmentRightFlange.Points[0];
            pipe.PipeSer.RightFlange.point[2] = pipe.PolyLineSegmentRightFlange.Points[1];
            pipe.PipeSer.RightFlange.point[3] = pipe.PolyLineSegmentRightFlange.Points[2];
            pipe.PipeSer.RightFlange.point[4] = pipe.PolyLineSegmentRightFlange.Points[3];

            pipe.PipeSer.Pipe.point[0] = pipe.PathFigurePipe.StartPoint;
            pipe.PipeSer.Pipe.point[1] = pipe.PolyLineSegmentPipe.Points[0];
            pipe.PipeSer.Pipe.point[2] = pipe.PolyLineSegmentPipe.Points[1];
            pipe.PipeSer.Pipe.point[3] = pipe.PolyLineSegmentPipe.Points[2];
            pipe.PipeSer.Pipe.point[4] = pipe.PolyLineSegmentPipe.Points[3];

            pipe.PipeSer.BorderPipe.point[0] = pipe.PathFigureBorder.StartPoint;
            pipe.PipeSer.BorderPipe.point[1] = pipe.PolyLineSegmentBorder.Points[0];
            pipe.PipeSer.BorderPipe.point[2] = pipe.PolyLineSegmentBorder.Points[1];
            pipe.PipeSer.BorderPipe.point[3] = pipe.PolyLineSegmentBorder.Points[2];
            pipe.PipeSer.BorderPipe.point[4] = pipe.PolyLineSegmentBorder.Points[3];
        }

        void Pipe90SerSave(Pipe90 pipe90)
        {
            pipe90.Pipe90Ser.TopLenghtSize.point[0] = pipe90.PathFigureTopLenghtSize.StartPoint;
            pipe90.Pipe90Ser.TopLenghtSize.point[1] = pipe90.LineSegmentTopLenghtSize.Point;

            pipe90.Pipe90Ser.DownLenghtSize.point[0] = pipe90.PathFigureDownLenghtSize.StartPoint;
            pipe90.Pipe90Ser.DownLenghtSize.point[1] = pipe90.LineSegmentDownLenghtSize.Point;

            pipe90.Pipe90Ser.TopSize.point[0] = pipe90.PathFigureTopSize.StartPoint;
            pipe90.Pipe90Ser.TopSize.point[1] = pipe90.LineSegmentTopSize.Point;

            pipe90.Pipe90Ser.DownSize.point[0] = pipe90.PathFigureDownSize.StartPoint;
            pipe90.Pipe90Ser.DownSize.point[1] = pipe90.LineSegmentDownSize.Point;

            pipe90.Pipe90Ser.LeftFlange.point[0] = pipe90.PathFigureLeftFlange.StartPoint;
            pipe90.Pipe90Ser.LeftFlange.point[1] = pipe90.PolyLineSegmentLeftFlange.Points[0];
            pipe90.Pipe90Ser.LeftFlange.point[2] = pipe90.PolyLineSegmentLeftFlange.Points[1];
            pipe90.Pipe90Ser.LeftFlange.point[3] = pipe90.PolyLineSegmentLeftFlange.Points[2];
            pipe90.Pipe90Ser.LeftFlange.point[4] = pipe90.PolyLineSegmentLeftFlange.Points[3];

            pipe90.Pipe90Ser.RightFlange.point[0] = pipe90.PathFigureRightFlange.StartPoint;
            pipe90.Pipe90Ser.RightFlange.point[1] = pipe90.PolyLineSegmentRightFlange.Points[0];
            pipe90.Pipe90Ser.RightFlange.point[2] = pipe90.PolyLineSegmentRightFlange.Points[1];
            pipe90.Pipe90Ser.RightFlange.point[3] = pipe90.PolyLineSegmentRightFlange.Points[2];
            pipe90.Pipe90Ser.RightFlange.point[4] = pipe90.PolyLineSegmentRightFlange.Points[3];

            pipe90.Pipe90Ser.TopImage.point[0] = pipe90.PathFigureTopImage.StartPoint;
            pipe90.Pipe90Ser.TopImage.point[1] = pipe90.PolyLineSegmentTopImage.Points[0];
            pipe90.Pipe90Ser.TopImage.point[2] = pipe90.PolyLineSegmentTopImage.Points[1];
            pipe90.Pipe90Ser.TopImage.point[3] = pipe90.PolyLineSegmentTopImage.Points[2];
            pipe90.Pipe90Ser.TopImage.point[4] = pipe90.PolyLineSegmentTopImage.Points[3];

            pipe90.Pipe90Ser.DownImage.point[0] = pipe90.PathFigureDownImage.StartPoint;
            pipe90.Pipe90Ser.DownImage.point[1] = pipe90.PolyLineSegmentDownImage.Points[0];
            pipe90.Pipe90Ser.DownImage.point[2] = pipe90.PolyLineSegmentDownImage.Points[1];
            pipe90.Pipe90Ser.DownImage.point[3] = pipe90.PolyLineSegmentDownImage.Points[2];
            pipe90.Pipe90Ser.DownImage.point[4] = pipe90.PolyLineSegmentDownImage.Points[3];

            pipe90.Pipe90Ser.LeftDownSize.point[0] = pipe90.PathFigureLeftDownSize.StartPoint;
            pipe90.Pipe90Ser.LeftDownSize.point[1] = pipe90.LineSegmentLeftDownSize.Point;

            pipe90.Pipe90Ser.RightDownSize.point[0] = pipe90.PathFigureRightDownSize.StartPoint;
            pipe90.Pipe90Ser.RightDownSize.point[1] = pipe90.LineSegmentRightDownSize.Point;

            pipe90.Pipe90Ser.BorderPipe90.point[0] = pipe90.PathFigureBorder.StartPoint;
            pipe90.Pipe90Ser.BorderPipe90.point[1] = pipe90.PolyLineSegmentBorder.Points[0];
            pipe90.Pipe90Ser.BorderPipe90.point[2] = pipe90.PolyLineSegmentBorder.Points[1];
            pipe90.Pipe90Ser.BorderPipe90.point[3] = pipe90.PolyLineSegmentBorder.Points[2];
            pipe90.Pipe90Ser.BorderPipe90.point[4] = pipe90.PolyLineSegmentBorder.Points[3];
        }                                

        private void Apply(object sender, RoutedEventArgs e)
        {
            pipe90.ComboBoxEnvironment.SelectedIndex = ComboBoxEnvironment.SelectedIndex;
            pipe90.TextBoxDiameter.Text = string.Format("{0:F2}", Convert.ToDouble(textBoxDiameter.Text));

            int selectEnvironment = ComboBoxEnvironment.SelectedIndex;
            double selectDiameter = Math.Round(Convert.ToDouble(textBoxDiameter.Text), 2, MidpointRounding.AwayFromZero);

            int countPipeSelected = 0;

            foreach (ControlOnCanvasPage controlOnCanvas in pipe90.CanvasPage.Children)
            {
                if (controlOnCanvas.IsSelected)
                {
                    countPipeSelected += 1;
                }
            }

            foreach (ControlOnCanvasPage objectOnCanvas in pipe90.CanvasPage.Children)
            {
                if (objectOnCanvas is Pipe90 && objectOnCanvas.IsSelected)
                {
                    Pipe90 pipe90OnCanvas = objectOnCanvas as Pipe90;

                    if (pipe90OnCanvas.Pipe90Ser.Environment != selectEnvironment)
                    {
                        IsSave = true;

                        pipe90OnCanvas.IntEnvironment = ComboBoxEnvironment.SelectedIndex;
                    }

                    if (pipe90OnCanvas.Diameter != selectDiameter)
                    {
                        IsSave = true;

                        double delta = selectDiameter - pipe90OnCanvas.Diameter;

                        Point topSizePoint = pipe90OnCanvas.PathFigureTopSize.StartPoint;
                        topSizePoint.X += delta;
                        pipe90OnCanvas.PathFigureTopSize.StartPoint = topSizePoint;

                        Point downSizePoint = pipe90OnCanvas.PathFigureDownSize.StartPoint;
                        downSizePoint.Y += delta;
                        downSizePoint.X += delta;
                        pipe90OnCanvas.PathFigureDownSize.StartPoint = downSizePoint;
                        Point downSizePoint2 = pipe90OnCanvas.LineSegmentDownSize.Point;
                        downSizePoint2.Y += delta;
                        downSizePoint2.X += delta;
                        pipe90OnCanvas.LineSegmentDownSize.Point = downSizePoint2;

                        Point leftSizePoint2 = pipe90OnCanvas.PathFigureTopLenghtSize.StartPoint;
                        leftSizePoint2.X += delta;
                        pipe90OnCanvas.PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                        Point leftSizePoint = pipe90OnCanvas.LineSegmentTopLenghtSize.Point;
                        leftSizePoint.Y += delta;
                        leftSizePoint.X += delta;
                        pipe90OnCanvas.LineSegmentTopLenghtSize.Point = leftSizePoint;

                        Point rightFlangePoint = pipe90OnCanvas.PathFigureRightFlange.StartPoint;
                        rightFlangePoint.X += delta;
                        rightFlangePoint.Y += delta;
                        pipe90OnCanvas.PathFigureRightFlange.StartPoint = rightFlangePoint;
                        Point rightFlangePoint2 = pipe90OnCanvas.PolyLineSegmentRightFlange.Points[0];
                        Point rightFlangePoint3 = pipe90OnCanvas.PolyLineSegmentRightFlange.Points[3];
                        Point rightFlangePoint4 = pipe90OnCanvas.PolyLineSegmentRightFlange.Points[1];
                        Point rightFlangePoint5 = pipe90OnCanvas.PolyLineSegmentRightFlange.Points[2];
                        rightFlangePoint2.Y += delta;
                        rightFlangePoint2.X += delta;
                        rightFlangePoint3.Y += delta;
                        rightFlangePoint3.X += delta;
                        rightFlangePoint4.X += delta;
                        rightFlangePoint5.X += delta;
                        pipe90OnCanvas.PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                        pipe90OnCanvas.PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                        pipe90OnCanvas.PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                        pipe90OnCanvas.PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                        Point borderPipePoint1 = pipe90OnCanvas.PolyLineSegmentBorder.Points[0];
                        Point borderPipePoint2 = pipe90OnCanvas.PolyLineSegmentBorder.Points[1];
                        Point borderPipePoint3 = pipe90OnCanvas.PolyLineSegmentBorder.Points[2];
                        borderPipePoint1.X += delta;
                        pipe90OnCanvas.PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                        borderPipePoint2.Y += delta;
                        borderPipePoint2.X += delta;
                        pipe90OnCanvas.PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                        borderPipePoint3.Y += delta;
                        pipe90OnCanvas.PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                        Point pointPipe = pipe90OnCanvas.PathFigureTopImage.StartPoint;
                        pointPipe.Y += delta;
                        pointPipe.X += delta;
                        pipe90OnCanvas.PathFigureTopImage.StartPoint = pointPipe;
                        Point pointPipe4 = pipe90OnCanvas.PolyLineSegmentTopImage.Points[1];
                        Point pointPipe2 = pipe90OnCanvas.PolyLineSegmentTopImage.Points[2];
                        Point pointPipe3 = pipe90OnCanvas.PolyLineSegmentTopImage.Points[3];
                        pointPipe2.Y += delta;
                        pointPipe3.Y += delta;
                        pointPipe2.X += delta;
                        pointPipe3.X += delta;
                        pointPipe4.X += delta;
                        pipe90OnCanvas.PolyLineSegmentTopImage.Points[1] = pointPipe4;
                        pipe90OnCanvas.PolyLineSegmentTopImage.Points[2] = pointPipe2;
                        pipe90OnCanvas.PolyLineSegmentTopImage.Points[3] = pointPipe3;

                        Point topSizePoint3 = pipe90OnCanvas.PathFigureLeftDownSize.StartPoint;
                        topSizePoint3.Y += delta;
                        pipe90OnCanvas.PathFigureLeftDownSize.StartPoint = topSizePoint3;

                        Point downSizePoint3 = pipe90OnCanvas.PathFigureRightDownSize.StartPoint;
                        downSizePoint3.Y += delta;
                        downSizePoint3.X += delta;
                        pipe90OnCanvas.PathFigureRightDownSize.StartPoint = downSizePoint3;
                        Point downSizePoint4 = pipe90OnCanvas.LineSegmentRightDownSize.Point;
                        downSizePoint4.X += delta;
                        downSizePoint4.Y += delta;
                        pipe90OnCanvas.LineSegmentRightDownSize.Point = downSizePoint4;

                        Point leftFlangePoint = pipe90OnCanvas.PathFigureLeftFlange.StartPoint;
                        leftFlangePoint.Y += delta;
                        pipe90OnCanvas.PathFigureLeftFlange.StartPoint = leftFlangePoint;
                        Point leftFlangePoint2 = pipe90OnCanvas.PolyLineSegmentLeftFlange.Points[0];
                        Point leftFlangePoint3 = pipe90OnCanvas.PolyLineSegmentLeftFlange.Points[1];
                        Point leftFlangePoint4 = pipe90OnCanvas.PolyLineSegmentLeftFlange.Points[2];
                        Point leftFlangePoint5 = pipe90OnCanvas.PolyLineSegmentLeftFlange.Points[3];
                        leftFlangePoint2.Y += delta;
                        leftFlangePoint3.Y += delta;
                        leftFlangePoint3.X += delta;
                        leftFlangePoint4.Y += delta;
                        leftFlangePoint4.X += delta;
                        leftFlangePoint5.Y += delta;
                        pipe90OnCanvas.PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                        pipe90OnCanvas.PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                        pipe90OnCanvas.PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                        pipe90OnCanvas.PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                        Point pipe3 = pipe90OnCanvas.PolyLineSegmentDownImage.Points[0];
                        Point pipe4 = pipe90OnCanvas.PolyLineSegmentDownImage.Points[1];
                        Point pipe5 = pipe90OnCanvas.PolyLineSegmentDownImage.Points[2];
                        pipe3.Y += delta;
                        pipe3.X += delta;
                        pipe4.Y += delta;
                        pipe4.X += delta;
                        pipe5.Y += delta;
                        pipe90OnCanvas.PolyLineSegmentDownImage.Points[0] = pipe3;
                        pipe90OnCanvas.PolyLineSegmentDownImage.Points[1] = pipe4;
                        pipe90OnCanvas.PolyLineSegmentDownImage.Points[2] = pipe5;

                        Point downLenghtPoint = pipe90OnCanvas.PathFigureDownLenghtSize.StartPoint;
                        downLenghtPoint.Y += delta;
                        pipe90OnCanvas.PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                        Point downLenghtPoint2 = pipe90OnCanvas.LineSegmentDownLenghtSize.Point;
                        downLenghtPoint2.Y += delta;
                        downLenghtPoint2.X += delta;
                        pipe90OnCanvas.LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                        pipe90OnCanvas.Diameter = (downSizePoint.Y - topSizePoint.Y);

                        this.SetValue(Canvas.TopProperty, (double)GetValue(Canvas.TopProperty) + delta);

                        Pipe90SerSave(pipe90OnCanvas);
                    }

                    pipe90.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        if (countPipeSelected == 1)
                        {
                            if (pipe90OnCanvas.controlOnCanvasSer.Transform == 0)
                            {
                                pipe90OnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)pipe90OnCanvas.GetValue(Canvas.LeftProperty));
                                pipe90OnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)pipe90OnCanvas.GetValue(Canvas.TopProperty));
                            }
                            else if (pipe90OnCanvas.controlOnCanvasSer.Transform == -90 || pipe90OnCanvas.controlOnCanvasSer.Transform == 270)
                            {
                                pipe90OnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)pipe90OnCanvas.GetValue(Canvas.TopProperty) - pipe90OnCanvas.ActualWidth);
                                pipe90OnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)pipe90OnCanvas.GetValue(Canvas.LeftProperty));
                            }
                            else if (pipe90OnCanvas.controlOnCanvasSer.Transform == -180 || pipe90OnCanvas.controlOnCanvasSer.Transform == 180)
                            {
                                pipe90OnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)pipe90OnCanvas.GetValue(Canvas.TopProperty) - pipe90OnCanvas.ActualHeight);
                                pipe90OnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)pipe90OnCanvas.GetValue(Canvas.LeftProperty) - pipe90OnCanvas.ActualWidth);
                            }
                            else if (pipe90OnCanvas.controlOnCanvasSer.Transform == -270 || pipe90OnCanvas.controlOnCanvasSer.Transform == 90)
                            {
                                pipe90OnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)pipe90OnCanvas.GetValue(Canvas.TopProperty));
                                pipe90OnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)pipe90OnCanvas.GetValue(Canvas.LeftProperty) - pipe90OnCanvas.ActualHeight);
                            }
                        }
                    }));
                }
                else if (objectOnCanvas is Pipe && objectOnCanvas.IsSelected)
                {
                    Pipe pipeOnCanvas = objectOnCanvas as Pipe;

                    if (pipeOnCanvas.PipeSer.Environment != selectEnvironment)
                    {
                        IsSave = true;
                       
                        pipeOnCanvas.IntEnvironment = ComboBoxEnvironment.SelectedIndex;
                    }

                    if (pipeOnCanvas.Diameter != selectDiameter)
                    {
                        IsSave = true;

                        double delta = selectDiameter - pipeOnCanvas.Diameter;

                        Point topSizePoint = pipeOnCanvas.PathFigureTopSize.StartPoint;

                        Point downSizePoint = pipeOnCanvas.PathFigureDownSize.StartPoint;
                        downSizePoint.Y += delta;
                        pipeOnCanvas.PathFigureDownSize.StartPoint = downSizePoint;
                        Point downSizePoint2 = pipeOnCanvas.LineSegmentDownSize.Point;
                        downSizePoint2.Y += delta;
                        pipeOnCanvas.LineSegmentDownSize.Point = downSizePoint2;

                        Point leftSizePoint = pipeOnCanvas.LineSegmentLeftSize.Point;
                        leftSizePoint.Y += delta;
                        pipeOnCanvas.LineSegmentLeftSize.Point = leftSizePoint;

                        Point rightSizePoint = pipeOnCanvas.PathFigureRightSize.StartPoint;
                        rightSizePoint.Y += delta;
                        pipeOnCanvas.PathFigureRightSize.StartPoint = rightSizePoint;

                        Point rightFlangePoint2 = pipeOnCanvas.PolyLineSegmentRightFlange.Points[1];
                        Point rightFlangePoint3 = pipeOnCanvas.PolyLineSegmentRightFlange.Points[2];
                        rightFlangePoint2.Y += delta;
                        rightFlangePoint3.Y += delta;
                        pipeOnCanvas.PolyLineSegmentRightFlange.Points[1] = rightFlangePoint2;
                        pipeOnCanvas.PolyLineSegmentRightFlange.Points[2] = rightFlangePoint3;

                        Point leftFlangePoint2 = pipeOnCanvas.PolyLineSegmentLeftFlange.Points[1];
                        Point leftFlangePoint3 = pipeOnCanvas.PolyLineSegmentLeftFlange.Points[2];
                        leftFlangePoint2.Y += delta;
                        leftFlangePoint3.Y += delta;
                        pipeOnCanvas.PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint2;
                        pipeOnCanvas.PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint3;

                        Point borderPipePoint2 = pipeOnCanvas.PolyLineSegmentBorder.Points[1];
                        Point borderPipePoint3 = pipeOnCanvas.PolyLineSegmentBorder.Points[2];
                        borderPipePoint2.Y += delta;
                        pipeOnCanvas.PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                        borderPipePoint3.Y += delta;
                        pipeOnCanvas.PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                        Point pointPipe = pipeOnCanvas.PathFigurePipe.StartPoint;
                        pointPipe.Y += delta;
                        pipeOnCanvas.PathFigurePipe.StartPoint = pointPipe;
                        Point pointPipe2 = pipeOnCanvas.PolyLineSegmentPipe.Points[2];
                        Point pointPipe3 = pipeOnCanvas.PolyLineSegmentPipe.Points[3];
                        pointPipe2.Y += delta;
                        pointPipe3.Y += delta;
                        pipeOnCanvas.PolyLineSegmentPipe.Points[2] = pointPipe2;
                        pipeOnCanvas.PolyLineSegmentPipe.Points[3] = pointPipe3;

                        pipeOnCanvas.Diameter = (downSizePoint.Y - topSizePoint.Y);

                        PipeSerSave(pipeOnCanvas);
                    }

                    pipe90.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        if (countPipeSelected == 1)
                        {
                            if (pipeOnCanvas.controlOnCanvasSer.Transform == 0)
                            {
                                pipeOnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)pipeOnCanvas.GetValue(Canvas.LeftProperty));
                                pipeOnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)pipeOnCanvas.GetValue(Canvas.TopProperty));
                            }
                            else if (pipeOnCanvas.controlOnCanvasSer.Transform == -90 || pipeOnCanvas.controlOnCanvasSer.Transform == 270)
                            {
                                pipeOnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)pipeOnCanvas.GetValue(Canvas.TopProperty) - pipeOnCanvas.ActualWidth);
                                pipeOnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)pipeOnCanvas.GetValue(Canvas.LeftProperty));
                            }
                            else if (pipeOnCanvas.controlOnCanvasSer.Transform == -180 || pipeOnCanvas.controlOnCanvasSer.Transform == 180)
                            {
                                pipeOnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)pipeOnCanvas.GetValue(Canvas.TopProperty) - pipeOnCanvas.ActualHeight);
                                pipeOnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)pipeOnCanvas.GetValue(Canvas.LeftProperty) - pipeOnCanvas.ActualWidth);
                            }
                            else if (pipeOnCanvas.controlOnCanvasSer.Transform == -270 || pipeOnCanvas.controlOnCanvasSer.Transform == 90)
                            {
                                pipeOnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)pipeOnCanvas.GetValue(Canvas.TopProperty));
                                pipeOnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)pipeOnCanvas.GetValue(Canvas.LeftProperty) - pipeOnCanvas.ActualHeight);
                            }
                        }
                    }));
                }
            }

            if (IsSave)
            {
                ((AppWPF)Application.Current).SaveTabItem(pipe90.CanvasPage.TabItemPage);
            }
            
            e.Handled = true;
            this.Close();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.Close();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            TreeViewItemGeneral.IsSelected = true;
            e.Handled = true;
        }
    }
}

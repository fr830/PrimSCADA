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
using System.Windows.Shapes;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для DialogWindowPropertiesImage.xaml
    /// </summary>
    public partial class DialogWindowPropertiesImage : Window
    {
        GridPropertiesImageGeneral PropertiesGeneral;
        GridPropertiesImageLibrary PropertiesLibrary;
        ImageSer ImageSer;

        string OldPathImage;
        string OldStretchImage;
        string OldLibraryImage;
        Brush OldBrushBorder;
        bool OldIsPathImage;
       
        private ImageControl imageControl;
        public ImageControl ImageControl
        {
            get { return imageControl; }
            set { imageControl = value; }
        }

        public DialogWindowPropertiesImage(ImageControl imageControl)
        {
            InitializeComponent();

            PropertiesGeneral = new GridPropertiesImageGeneral(imageControl);

            PropertiesLibrary = new GridPropertiesImageLibrary();

            ImageControl = imageControl;

            OldPathImage = imageControl.ImageSer.PathImage;

            OldStretchImage = imageControl.ImageSer.StretchImage;

            PropertiesGeneral.TBPathImage.Text = imageControl.ImageSer.PathImage;

            ImageSer = imageControl.ImageSer;

            OldLibraryImage = imageControl.ImageSer.LibraryImage;

            OldBrushBorder = ImageSer.ColorBorder;

            OldIsPathImage = ImageSer.IsPathImage;
        }

        void SizeImage(BitmapImage bi)
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

        private void Apply(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            string s = (string)((TreeViewItem)TreeViewProperties.SelectedItem).Header;

            if (s == "Общие")
            {                
                if (PropertiesGeneral.RBPathImage.IsChecked == true)
                {
                    if (PropertiesGeneral.TBPathImage.Text != OldPathImage)
                    {
                        if (PropertiesGeneral.TBPathImage.Text == null)
                        {
                            imageControl.templateImage.Source = null;
                        }
                        else
                        {
                            try
                            {
                                BitmapImage bi = new BitmapImage(new Uri(@PropertiesGeneral.TBPathImage.Text, UriKind.RelativeOrAbsolute));

                                if (bi.PixelWidth < 10 || bi.PixelWidth < 10)
                                {
                                    MessageBox.Show("Ширина или высота не может быть меньше 10 пикселей.", "Ошибка размеров изображения", MessageBoxButton.OK);

                                    return;
                                }

                                imageControl.templateImage.Source = bi;

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
                                    SizeImage(bi);
                                }
                                else if (rotate90)
                                {
                                    SizeImage(bi);
                                }
                                else if (rotate180)
                                {
                                    SizeImage(bi);
                                }
                                else if (rotate270)
                                {
                                    SizeImage(bi);
                                }

                                imageControl.ChangeImageSer();
                            }
                            catch (SystemException ex)
                            {
                                MessageBox.Show(ex.ToString(), "Ошибка открытия изображения", MessageBoxButton.OK);

                                return;
                            }
                        }

                        imageControl.ImageSer.PathImage = PropertiesGeneral.TBPathImage.Text;

                        ((AppWPF)Application.Current).SaveTabItem(ImageSer.ControlItem.CanvasTab.TabItemParent);
                    }
                }
                else
                {
                    if (PropertiesGeneral.CBLibrary.SelectedItem != null)
                    {
                        if (PropertiesGeneral.CBLibrary.SelectedItem != OldLibraryImage)
                        {
                            try
                            {
                                BitmapImage bi = new BitmapImage(new Uri(@"pack://application:,,,/Images/" + PropertiesGeneral.CBLibrary.SelectedItem + ".png", UriKind.RelativeOrAbsolute));

                                if (bi.PixelWidth < 10 || bi.PixelWidth < 10)
                                {
                                    MessageBox.Show("Ширина или высота не может быть меньше 10 пикселей.", "Ошибка размеров изображения", MessageBoxButton.OK);

                                    return;
                                }

                                imageControl.templateImage.Source = bi;

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
                                    SizeImage(bi);
                                }
                                else if (rotate90)
                                {
                                    SizeImage(bi);
                                }
                                else if (rotate180)
                                {
                                    SizeImage(bi);
                                }
                                else if (rotate270)
                                {
                                    SizeImage(bi);
                                }

                                imageControl.ImageSer.LibraryImage = (string)PropertiesGeneral.CBLibrary.SelectedItem;
                                imageControl.ChangeImageSer();

                                ((AppWPF)Application.Current).SaveTabItem(ImageSer.ControlItem.CanvasTab.TabItemParent);
                            }
                            catch (SystemException ex)
                            {
                                MessageBox.Show(ex.ToString(), "Ошибка открытия изображения", MessageBoxButton.OK);

                                return;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Выберите изображение", "Ошибка", MessageBoxButton.OK);

                        return;
                    }
                }

                if (PropertiesGeneral.RBPathImage.IsChecked != OldIsPathImage)
                {
                    imageControl.ImageSer.IsPathImage = (bool)PropertiesGeneral.RBPathImage.IsChecked;
                }

                if (PropertiesGeneral.CBStretch.SelectedValue != OldStretchImage)
                {
                    imageControl.ImageSer.StretchImage = (string)PropertiesGeneral.CBStretch.SelectedValue;

                    if (PropertiesGeneral.CBStretch.SelectedValue == "None")
                    {
                        imageControl.templateImage.Stretch = Stretch.None;
                    }
                    else if (PropertiesGeneral.CBStretch.SelectedValue == "Fill")
                    {
                        imageControl.templateImage.Stretch = Stretch.Fill;
                    }
                    else if (PropertiesGeneral.CBStretch.SelectedValue == "Uniform")
                    {
                        imageControl.templateImage.Stretch = Stretch.Uniform;
                    }
                    else if (PropertiesGeneral.CBStretch.SelectedValue == "UniformToFill")
                    {
                        imageControl.templateImage.Stretch = Stretch.UniformToFill;
                    }

                    ((AppWPF)Application.Current).SaveTabItem(ImageSer.ControlItem.CanvasTab.TabItemParent);
                }
                
                if (PropertiesGeneral.CHBImageToFull.IsChecked == true)
                {
                    if (PropertiesGeneral.TBPathImage.Text != null)
                    {
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
                            SizeImage((BitmapImage)imageControl.templateImage.Source);
                        }
                        else if (rotate90)
                        {
                            SizeImage((BitmapImage)imageControl.templateImage.Source);
                        }
                        else if (rotate180)
                        {
                            SizeImage((BitmapImage)imageControl.templateImage.Source);
                        }
                        else if (rotate270)
                        {
                            SizeImage((BitmapImage)imageControl.templateImage.Source);
                        }

                        imageControl.ChangeImageSer();

                        ((AppWPF)Application.Current).SaveTabItem(ImageSer.ControlItem.CanvasTab.TabItemParent);
                    }
                } 
            }            

            if (PropertiesGeneral.NewBrush != null)
            {
                if (OldBrushBorder is SolidColorBrush && PropertiesGeneral.NewBrush is SolidColorBrush)
                {
                    SolidColorBrush oldBrushBorder = OldBrushBorder as SolidColorBrush;
                    SolidColorBrush newSolidColorBrush = PropertiesGeneral.NewBrush as SolidColorBrush;

                    if (oldBrushBorder.Color.A != newSolidColorBrush.Color.A || oldBrushBorder.Color.B != newSolidColorBrush.Color.B || oldBrushBorder.Color.G != newSolidColorBrush.Color.G || oldBrushBorder.Color.R != newSolidColorBrush.Color.R || oldBrushBorder.Color.ScA != newSolidColorBrush.Color.ScA || oldBrushBorder.Color.ScB != newSolidColorBrush.Color.ScB || oldBrushBorder.Color.ScG != newSolidColorBrush.Color.ScG || oldBrushBorder.Color.ScR != newSolidColorBrush.Color.ScR || oldBrushBorder.Opacity != newSolidColorBrush.Opacity)
                    {
                        ImageSer.ColorBorder = newSolidColorBrush;

                        imageControl.templateBorder.BorderBrush = newSolidColorBrush;

                        ((AppWPF)Application.Current).SaveTabItem(ImageSer.ControlItem.CanvasTab.TabItemParent);
                    }
                }
                else if (OldBrushBorder is LinearGradientBrush && PropertiesGeneral.NewBrush is LinearGradientBrush)
                {
                    LinearGradientBrush oldBrushBorder = OldBrushBorder as LinearGradientBrush;
                    LinearGradientBrush newLinearGradientBrush = PropertiesGeneral.NewBrush as LinearGradientBrush;

                    int count = 0;

                    if (oldBrushBorder.ColorInterpolationMode != newLinearGradientBrush.ColorInterpolationMode || oldBrushBorder.EndPoint != newLinearGradientBrush.EndPoint || oldBrushBorder.Opacity != newLinearGradientBrush.Opacity || oldBrushBorder.MappingMode != newLinearGradientBrush.MappingMode || oldBrushBorder.SpreadMethod != newLinearGradientBrush.SpreadMethod || oldBrushBorder.StartPoint != newLinearGradientBrush.StartPoint || oldBrushBorder.GradientStops.Count != newLinearGradientBrush.GradientStops.Count)
                    {
                        ImageSer.ColorBorder = newLinearGradientBrush;

                        imageControl.templateBorder.BorderBrush = newLinearGradientBrush;

                        ((AppWPF)Application.Current).SaveTabItem(ImageSer.ControlItem.CanvasTab.TabItemParent);
                    }
                    else
                    {
                        GradientStop newGradientStop;

                        foreach (GradientStop runGradientStop in oldBrushBorder.GradientStops)
                        {
                            newGradientStop = newLinearGradientBrush.GradientStops[count];

                            if (runGradientStop.Color.A != newGradientStop.Color.A || runGradientStop.Color.B != newGradientStop.Color.B || runGradientStop.Color.G != newGradientStop.Color.G || runGradientStop.Color.R != newGradientStop.Color.R || runGradientStop.Color.ScA != newGradientStop.Color.ScA || runGradientStop.Color.ScB != newGradientStop.Color.ScB || runGradientStop.Color.ScG != newGradientStop.Color.ScG || runGradientStop.Color.ScR != newGradientStop.Color.ScR)
                            {
                                ImageSer.ColorBorder = newLinearGradientBrush;

                                imageControl.templateBorder.BorderBrush = newLinearGradientBrush;

                                ((AppWPF)Application.Current).SaveTabItem(ImageSer.ControlItem.CanvasTab.TabItemParent);

                                break;
                            }

                            count++;
                        }
                    }
                }
                else if (OldBrushBorder is RadialGradientBrush && PropertiesGeneral.NewBrush is RadialGradientBrush)
                {
                    RadialGradientBrush oldBrushBorder = OldBrushBorder as RadialGradientBrush;
                    RadialGradientBrush newRadialGradientBrush = PropertiesGeneral.NewBrush as RadialGradientBrush;

                    int count = 0;

                    if (oldBrushBorder.Center != newRadialGradientBrush.Center || oldBrushBorder.ColorInterpolationMode != newRadialGradientBrush.ColorInterpolationMode || oldBrushBorder.GradientOrigin != newRadialGradientBrush.GradientOrigin || oldBrushBorder.MappingMode != newRadialGradientBrush.MappingMode || oldBrushBorder.Opacity != newRadialGradientBrush.Opacity || oldBrushBorder.RadiusX != newRadialGradientBrush.RadiusX || oldBrushBorder.RadiusY != newRadialGradientBrush.RadiusY || oldBrushBorder.SpreadMethod != newRadialGradientBrush.SpreadMethod || oldBrushBorder.GradientStops.Count != newRadialGradientBrush.GradientStops.Count)
                    {
                        ImageSer.ColorBorder = newRadialGradientBrush;

                        imageControl.templateBorder.BorderBrush = newRadialGradientBrush;

                        ((AppWPF)Application.Current).SaveTabItem(ImageSer.ControlItem.CanvasTab.TabItemParent);
                    }
                    else
                    {
                        GradientStop newGradientStop;

                        foreach (GradientStop runGradientStop in oldBrushBorder.GradientStops)
                        {
                            newGradientStop = newRadialGradientBrush.GradientStops[count];

                            if (runGradientStop.Color.A != newGradientStop.Color.A || runGradientStop.Color.B != newGradientStop.Color.B || runGradientStop.Color.G != newGradientStop.Color.G || runGradientStop.Color.R != newGradientStop.Color.R || runGradientStop.Color.ScA != newGradientStop.Color.ScA || runGradientStop.Color.ScB != newGradientStop.Color.ScB || runGradientStop.Color.ScG != newGradientStop.Color.ScG || runGradientStop.Color.ScR != newGradientStop.Color.ScR)
                            {
                                ImageSer.ColorBorder = newRadialGradientBrush;

                                imageControl.templateBorder.BorderBrush = newRadialGradientBrush;

                                ((AppWPF)Application.Current).SaveTabItem(ImageSer.ControlItem.CanvasTab.TabItemParent);

                                break;
                            }

                            count++;
                        }
                    }
                }
                else
                {
                    ImageSer.ColorBorder = PropertiesGeneral.NewBrush;

                    imageControl.templateBorder.BorderBrush = PropertiesGeneral.NewBrush;

                    ((AppWPF)Application.Current).SaveTabItem(ImageSer.ControlItem.CanvasTab.TabItemParent);
                }
            }           
                                                                           
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
     
        private void TreeViewProperties_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem Selected = (TreeViewItem)e.NewValue;
            if ((string)Selected.Header == "Общие")
            {
                if (PropertiesGrid.Children.Contains(PropertiesLibrary))
                {
                    PropertiesGrid.Children.Remove(PropertiesLibrary);
                }

                if (PropertiesGrid.Children.Contains(PropertiesGeneral))
                {
                    PropertiesGrid.Children.Remove(PropertiesGeneral);
                }

                PropertiesGeneral.SetValue(Grid.ColumnProperty, 1);

                PropertiesGrid.Children.Add(PropertiesGeneral);
            }
            else if ((string)Selected.Header == "Библиотека изображений")
            {
                if (PropertiesGrid.Children.Contains(PropertiesLibrary))
                {
                    PropertiesGrid.Children.Remove(PropertiesLibrary);
                }

                if (PropertiesGrid.Children.Contains(PropertiesGeneral))
                {
                    PropertiesGrid.Children.Remove(PropertiesGeneral);
                }

                PropertiesLibrary.SetValue(Grid.ColumnProperty, 1);

                PropertiesGrid.Children.Add(PropertiesLibrary);
            }

            e.Handled = true;
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}

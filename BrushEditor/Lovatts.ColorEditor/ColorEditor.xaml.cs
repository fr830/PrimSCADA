using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using BrushEditor;

namespace Lovatts.ColorEditor
{
    /// <summary>
    /// Interaction logic for ColorEditor.xaml
    /// </summary>
    public partial class ColorEditor : UserControl
    {
        private readonly Dictionary<double, SimpleBitmap> palette = new Dictionary<double, SimpleBitmap>();
        private ColorEditorViewModel _colorEditorViewModel;
        private double oldHue = -1;

        public ColorEditor()
        {
            InitializeComponent();

            ColorEditorViewModel = new ColorEditorViewModel();

            DataContext = ColorEditorViewModel;

            MakeHexTextBoxOverType();

            RefreshPalette(0);
        }

        public ColorEditorViewModel ColorEditorViewModel
        {
            get { return _colorEditorViewModel; }
            set
            {
                if (_colorEditorViewModel != null)
                {
                    _colorEditorViewModel.HSL.PropertyChanged -= HSL_PropertyChanged;
                    _colorEditorViewModel.PropertyChanged -= new PropertyChangedEventHandler(_colorEditorViewModel_PropertyChanged);
                }

                _colorEditorViewModel = value;

                _colorEditorViewModel.HSL.PropertyChanged += HSL_PropertyChanged;
                _colorEditorViewModel.PropertyChanged += new PropertyChangedEventHandler(_colorEditorViewModel_PropertyChanged);
            }
        }

        void _colorEditorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Color")
            {
                SetThumbPosition(new Point(ColorEditorViewModel.HSL.S * 255, ColorEditorViewModel.HSL.L * 255));
            }
        }

        private void MakeHexTextBoxOverType()
        {
            PropertyInfo textEditorProperty = typeof (TextBox).GetProperty("TextEditor", BindingFlags.NonPublic | BindingFlags.Instance);
            object textEditor = textEditorProperty.GetValue(hexTextBox, null);

            // set _OvertypeMode on the TextEditor
            PropertyInfo overtypeModeProperty = textEditor.GetType().GetProperty("_OvertypeMode", BindingFlags.NonPublic | BindingFlags.Instance);
            overtypeModeProperty.SetValue(textEditor, true, null);
        }

        private void HSL_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "H")
            {
                HueChanged();
            }
        }

        private void HueChanged()
        {
            double top = ColorEditorViewModel.HSL.L*255;
            double left = ColorEditorViewModel.HSL.S*255;

            Canvas.SetLeft(myThumb, left);
            Canvas.SetTop(myThumb, top);

            RefreshPalette(ColorEditorViewModel.HSL.H);
        }

        private void RefreshPalette(double hue)
        {
            hue = Math.Round(hue, 2);

            if (hue == oldHue) return;

            oldHue = hue;

            if (!palette.ContainsKey(hue))
            {
                var b = new SimpleBitmap(255, 255);

                var colors = new Color[255,255];

                for (int x = 0; x < 255; x++)
                {
                    for (int y = 0; y < 255; y++)
                    {
                        Color color = RGBHSL.HSL_to_RGB(new RGBHSL.HSL(hue, x/255.0, y/255.0, 1));
                        colors[x, y] = color;
                    }
                }

                b.SetPixelsFromArray(colors);

                palette[hue] = b;
            }

            image.Source = palette[hue].BaseBitmap;
        }

        void SetThumbPosition(Point p)
        {
            double left = p.X;
            double top = p.Y;

            if (left < 0 || left > canvas.ActualWidth) return;
            if (top < 0 || top > canvas.ActualHeight) return;

            ColorEditorViewModel.HSL.LockHue = true;
            ColorEditorViewModel.HSL.L = top / 255;
            ColorEditorViewModel.HSL.S = left / 255;
            ColorEditorViewModel.HSL.LockHue = false;


            Canvas.SetLeft(myThumb, left);
            Canvas.SetTop(myThumb, top);
        }
        
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            SetThumbPosition(new Point(Canvas.GetLeft(myThumb) + e.HorizontalChange, Canvas.GetTop(myThumb) + e.VerticalChange));
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(sender as IInputElement);

            ColorEditorViewModel.HSL.LockHue = true;
            ColorEditorViewModel.HSL.L = point.Y/255;
            ColorEditorViewModel.HSL.S = point.X/255;
            ColorEditorViewModel.HSL.LockHue = false;

            Canvas.SetLeft(myThumb, point.X);
            Canvas.SetTop(myThumb, point.Y);
        }
    }
}
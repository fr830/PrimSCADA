using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BrushEditor
{
    public class ColorEditorViewModel : ViewModelBase
    {
        private readonly SimpleBitmap _simpleBitmap = new SimpleBitmap(255, 255);
        private Color _color;

        private RGBA _rgb;
        private bool _hslChanging; 
        private bool _rgbChanging;

        public ColorEditorViewModel()
        {
            HSL = new HSLA();
            RGB = new RGBA();


            HSL.PropertyChanged += HSL_PropertyChanged;
            HSL.A = 1;
        }

        public RGBA RGB
        {
            get { return _rgb; }
            set
            {
                if (_rgb != null) RGB.PropertyChanged -= RGB_PropertyChanged;

                _rgb = value;

                UpdateHSL();

                OnPropertyChanged(() => RGB);
                OnPropertyChanged(() => HSL);
                OnPropertyChanged(() => Color);
                OnPropertyChanged(() => Brush);

                RGB.PropertyChanged += RGB_PropertyChanged;
            }
        }


        public HSLA HSL { get; private set; }

        /// <summary>
        /// Nasty setter, needs re-writing. The upshot is it's trying to update the RGB + HSL values whilst at the same time changing for example, an R value automatically updates the HSL value, which we don't want. They should be set all at once.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set
            {
                if (_color == value) return;

                _color = value;

                RGBHSL.HSL hsl = RGBHSL.RGB_to_HSL(_color);

                _rgbChanging = true;
                _hslChanging = true;

                HSL.H = hsl.H;
                HSL.S = hsl.S;
                HSL.L = hsl.L;
                HSL.A = hsl.A;

                RGB.A = _color.A;
                RGB.R = _color.R;
                RGB.G = _color.G;
                RGB.B = _color.B;                

                _rgbChanging = false;
                _hslChanging = false;

                OnPropertyChanged(() => RGB);
                OnPropertyChanged(() => HSL);
                OnPropertyChanged(() => Color);
                OnPropertyChanged(() => Brush);
            }
        }

        public Brush Brush
        {
            get { return new SolidColorBrush(Color); }
        }

        public WriteableBitmap Bitmap
        {
            get { return _simpleBitmap.BaseBitmap; }
        }

        private void UpdateHSL()
        {
            if (_hslChanging) return;

            Color color = Color.FromArgb(RGB.A, RGB.R, RGB.G, RGB.B);

            if (Color == color) return;
            Color = color;

            RGBHSL.HSL hsl = RGBHSL.RGB_to_HSL(color);


            _rgbChanging = true; // so we don't stack overflow on recursive calls to HSL_Changed and RGB_Changed

            HSL.H = hsl.H;
            HSL.S = hsl.S;
            HSL.L = hsl.L;
            HSL.A = hsl.A;

            _rgbChanging = false;                   
        }

        private void RGB_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateHSL();
        }

        /// <summary>
        /// If a HSL property changed, we should update the RGB values all at once. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HSL_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {           
            if (_rgbChanging) return; // don't change the RGB values if that's why this method was called.

            Color color = RGBHSL.HSL_to_RGB(new RGBHSL.HSL(HSL)); // convert HSL to RGB

            if (Color == color) return; 

            if (e.PropertyName != "H") HSL.LockHue = true; // We lock the Hue unless we intended to change that value (due to errors in conversion from RGB to HSL0

            Color = color;

            _hslChanging = true;  // so we don't stack overflow on recursive calls to HSL_Changed and RGB_Changed           
            RGB.R = color.R;
            RGB.G = color.G;
            RGB.B = color.B;
            RGB.A = color.A;            
            _hslChanging = false;

            if (e.PropertyName == "H") HSL.LockHue = false; // Now we can unlock the hue
        }
    }
}
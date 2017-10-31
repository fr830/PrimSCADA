using System.Windows.Media;

namespace BrushEditor
{
    public class GradientStopViewModel : ViewModelBase
    {
        private Color _color;
        private double _offset;

        public GradientStopViewModel()
        {
            _color = Colors.Black;
        }

        public GradientStopViewModel(Color color, double offset)
        {
            _color = color;
            _offset = offset;
        }

        public GradientStopViewModel(GradientStop gradientStop)
        {
            _color = gradientStop.Color;
            _offset = gradientStop.Offset;
        }

        public double Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;

                OnPropertyChanged(() => Offset);
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;

                OnPropertyChanged(() => Color);
                OnPropertyChanged(() => Brush);
            }
        }

        public SolidColorBrush Brush
        {
            get { return new SolidColorBrush(Color); }
        }
    }
}
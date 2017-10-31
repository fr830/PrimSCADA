using System;
using System.Globalization;
using System.Windows.Data;

namespace BrushEditor
{
    public class RGBHexConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rgb = (RGBA) value;

            return string.Format("#{0}{1}{2}{3}", rgb.A.ToString("X2"), rgb.R.ToString("X2"), rgb.G.ToString("X2"), rgb.B.ToString("X2"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string v = value.ToString();

            if (v.Length != 9) return new RGBA();

            if (v[0] != '#') return new RGBA();

            try
            {
                var a = (byte) System.Convert.ToInt32(v.Substring(1, 2), 16);
                var r = (byte) System.Convert.ToInt32(v.Substring(3, 2), 16);
                var g = (byte) System.Convert.ToInt32(v.Substring(5, 2), 16);
                var b = (byte) System.Convert.ToInt32(v.Substring(7, 2), 16);

                return new RGBA {A = a, R = r, G = g, B = b};
            }
            catch
            {
                return new RGBA();
            }
        }

        #endregion
    }
}
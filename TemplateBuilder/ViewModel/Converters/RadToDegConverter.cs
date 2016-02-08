using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TemplateBuilder.ViewModel.Converters
{
    public class RadToDegConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double rad = (double)value;
            return rad * (180.0 / Math.PI);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double deg = (double)value;
            return Math.PI * deg / 180.0;
        }
    }
}

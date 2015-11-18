using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateBuilderMVVM.ViewModel.Converters
{
    public class RadToDeg : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double rad = (double)value;
            return rad * (180.0 / Math.PI);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double deg = (double)value;
            return Math.PI * deg / 180.0;
        }
    }
}
